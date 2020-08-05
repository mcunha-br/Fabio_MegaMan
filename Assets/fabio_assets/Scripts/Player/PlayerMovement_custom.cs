// This script controls the player's movement and physics within the game

using UnityEngine;

public class PlayerMovement_custom : MonoBehaviour
{
	public bool drawDebugRaycasts = true;	//Should the environment checks be visualized

	[Header("Movement Properties")]
	public float walkSpeed = 8f;			//Player walkSpeed
	public float crouchSpeedDivisor = 3f;	//Speed reduction when crouching
	public float coyoteDuration = .05f;		//How long the player can jump after falling
	public float maxFallSpeed = -25f;		//Max speed player can fall
    public float repell_contact = 1000;    //force to repel player from enemy upon contact

	[Header("Jump Properties")]
	public float jumpForce = 6.3f;			//Initial force of jump
	public float crouchJumpBoost = 2.5f;	//Jump boost when crouching
	public float hangingJumpForce = 15f;	//Force of wall hanging jumo
	public float jumpHoldForce = 1.9f;		//Incremental force when jump is held
	public float jumpHoldDuration = .1f;	//How long the jump key can be held
    public float jumpdelay = 0.3f;

    [Header("Dash Properties")]
    public float dashSpeed = 15f;           //Player dashSpeed    
    public float dashHoldTime;
    public float dashTimer;
    public float dashdelay = 1f;
    float dashdelay_time;

    [Header("Environment Check Properties")]
	public float footOffset = .4f;			//X Offset of feet raycast
	public float eyeHeight = 1.5f;			//Height of wall checks
	public float reachOffset = .7f;			//X offset for wall grabbing
	public float headClearance = .5f;		//Space needed above the player's head
	public float groundDistance = 0.5f;		//Distance player is considered to be on the ground
	public float grabDistance = .4f;		//The reach distance for wall grabs
	public LayerMask groundLayer;			//Layer of the ground
    public LayerMask enemyLayer;            //Layer to repell landing

    [Header ("Status Flags")]
	public bool isOnGround;					//Is the player on the ground?
	public bool isJumping;					//Is player jumping?
    public bool isDashing;                  //Is player jumping?    
    public bool isAttacking;
    public bool isHanging;					//Is player hanging?
	public bool isCrouching;				//Is player crouching?
	public bool isHeadBlocked;              //Is head blocked?
    public bool canMove;
    public bool dashRequested = false;      //Is player dash requested?
    public bool attackRequested = false;    //Is player attack requested?
    
    public int comboCounter = 0;                //combo counter

    PlayerInput_custom input;				//The current inputs for the player
	BoxCollider2D bodyCollider;				//The collider component
	Rigidbody2D rigidBody;					//The rigidbody component
    WeaponCollider hitbox_Collider;         //The player Hitbox

    float jumpTime;							//Variable to hold jump duration
    float jumpdelay_time;                    //Variable to hold jump time delay
    float coyoteTime;                       //Variable to hold coyote duration    

    float playerHeight;						//Height of the player

	float originalXScale;					//Original scale on X axis
	int direction = 1;						//Direction player is facing

	Vector2 colliderStandSize;				//Size of the standing collider
	Vector2 colliderStandOffset;			//Offset of the standing collider
	Vector2 colliderCrouchSize;				//Size of the crouching collider
	Vector2 colliderCrouchOffset;			//Offset of the crouching collider

	const float smallAmount = .05f;			//A small amount used for hanging position

    private Animator anim;			        //Animator component
    
    void Start ()
	{
        //assuming player can move
        canMove = true;

        //Get a reference to the required components
        input = GetComponent<PlayerInput_custom>();
		rigidBody = GetComponent<Rigidbody2D>();
		bodyCollider = GetComponent<BoxCollider2D>();
        hitbox_Collider = GetComponentInChildren<WeaponCollider>();
        anim = GetComponent<Animator>();

        //Record the original x scale of the player
        originalXScale = transform.localScale.x;

		//Record the player's height from the collider
		playerHeight = bodyCollider.size.y;

		//Record initial collider size and offset
		colliderStandSize = bodyCollider.size;
		colliderStandOffset = bodyCollider.offset;

		//Calculate crouching collider size and offset
		colliderCrouchSize = new Vector2(bodyCollider.size.x, bodyCollider.size.y / 2f);
		colliderCrouchOffset = new Vector2(bodyCollider.offset.x, bodyCollider.offset.y / 2f);
	}

    
    private void Update()
    {        
        //Debug.Log("onground: " + isOnGround.ToString());
        Movementanim();
    }
    

    void FixedUpdate()
	{
        
        //Check the environment to determine status
        PhysicsCheck();

		//Process ground and air movements
		GroundMovement();		
		MidAirMovement();

    }

	void PhysicsCheck()
	{

        //Start by assuming the player isn't on the ground and the head isn't blocked        
        isOnGround = false;
		isHeadBlocked = false;

        //--------------------------------------------//
        //----------------Ground Check----------------//
        //--------------------------------------------//

        //Cast rays to check colision on ground for left and right foot
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, groundLayer);
		RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        
        //If either ray hit the ground, the player is on the ground
        if (leftCheck || rightCheck)
			isOnGround = true;

        //--------------------------------------------//
        //----------------Enemy Check-----------------//
        //--------------------------------------------//
        //Cast rays to check colision on top of enemy for left and right foot

        RaycastHit2D leftCheck_enemy = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, enemyLayer);
        RaycastHit2D rightCheck_enemy = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, enemyLayer);

        //If either ray hit an enemy layer
        if (leftCheck_enemy || rightCheck_enemy)
            Repell_Landing();

        //Cast the ray to check above the player's head
        RaycastHit2D headCheck = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, headClearance, groundLayer);

		//If that ray hits, the player's head is blocked
		if (headCheck)
			isHeadBlocked = true;

        //------------------------------------//
        //------Wall Grab Code Disabled-------//
        //-----------------------------------//
        /*
        //Determine the direction of the wall grab attempt
        Vector2 grabDir = new Vector2(direction, 0f);

		//Cast three rays to look for a wall grab
		RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance);
		RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance);
		RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance);

		//If the player is off the ground AND is not hanging AND is falling AND
		//found a ledge AND found a wall AND the grab is NOT blocked...
		if (!isOnGround && !isHanging && rigidBody.velocity.y < 0f && 
			ledgeCheck && wallCheck && !blockedCheck)
		{ 
			//...we have a ledge grab. Record the current position...
			Vector3 pos = transform.position;
			//...move the distance to the wall (minus a small amount)...
			pos.x += (wallCheck.distance - smallAmount) * direction;
			//...move the player down to grab onto the ledge...
			pos.y -= ledgeCheck.distance;
			//...apply this position to the platform...
			transform.position = pos;
			//...set the rigidbody to static...
			rigidBody.bodyType = RigidbodyType2D.Static;
			//...finally, set isHanging to true
			isHanging = true;
		}
        */
	}

	void GroundMovement()
	{
        if (canMove)
        {

            //Handle crouching input. If holding the crouch button but not crouching, crouch
            if (input.crouchHeld && !isCrouching && isOnGround)
                Crouch();
            //Otherwise, if not holding crouch but currently crouching, stand up
            else if (!input.crouchHeld && isCrouching)
                StandUp();
            //Otherwise, if crouching and no longer on the ground, stand up
            else if (!isOnGround && isCrouching)
                StandUp();

            //horizontal speed at current frame, can be dash or walking speed             
            float frameSpeedX = walkSpeed;
            float frameSpeedY = rigidBody.velocity.y;

            //Calculate the desired velocity based on inputs
            float xVelocity = frameSpeedX * input.horizontal;

            //Dash movement
            if ((input.dashHeld) && (dashTimer > 0) && (Time.time > dashdelay_time))
            {
                dashRequested = true;

                dashTimer -= Time.deltaTime;

                frameSpeedX = dashSpeed;
                frameSpeedY = 0f;
                xVelocity = frameSpeedX * direction;
            }
            else if (dashRequested)
            {
                dashRequested = false;
                dashTimer = dashHoldTime;
                dashdelay_time = Time.time + dashdelay;
            } else
            {
                dashRequested = false;
            }

            //If the sign of the velocity and direction don't match, flip the character
            if (input.horizontal * direction < 0f)
            {
                dashRequested = false;
                xVelocity = 0f;
                FlipCharacterDirection();
            }
            //If the player is crouching, reduce the velocity
            if (isCrouching)
                xVelocity = 0;

            //Apply the desired velocity 
            rigidBody.velocity = new Vector2(xVelocity, frameSpeedY);

            //If the player is on the ground, extend the coyote time window
            if (isOnGround)
                coyoteTime = Time.time + coyoteDuration;
        }
            if (input.attackPressed)
            {
                attackRequested = true;                
                //Debug.Log("AttackPressed: " + attackRequested.ToString());            
            }
            else
            {
            attackRequested = false;
            }
            
    }

    void MidAirMovement()
    {
        //player y position
        float ypos = transform.position.y;

        //If the jump key is pressed AND the player isn't already jumping 
        //AND jumpdelay is already passed
        //AND EITHER the player is on the ground or within the coyote time window...            
        //&& (jumpdelay_time <= Time.time)

        if (input.jumpPressed && !isJumping && canMove && (isOnGround || coyoteTime > Time.time))
        {
            //...check to see if crouching AND not blocked. If so...
            if (isCrouching && !isHeadBlocked)
            {
                //...stand up and apply a crouching jump boost
                StandUp();
                rigidBody.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            }

            //...The player is no longer on the ground and is jumping...
            isOnGround = false;
            isJumping = true;

            //...record the time the player will stop being able to boost their jump...
            jumpTime = Time.time + jumpHoldDuration;
            jumpdelay_time = Time.time + jumpdelay;

            //--------------Custom Fabio--------------//
            //---------------------------------------//
            //increment y coordinate of character per jump time
            /*
            transform.position = new Vector3 (transform.position.x, 
                transform.position.y + jumpForce, transform.position.z);
            */
            //---------------------------------------//
            //---------------------------------------//

            //...add the jump force to the rigidbody...
            rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            //...and tell the Audio Manager to play the jump audio
            //AudioManager.PlayJumpAudio();

        }
        //Otherwise, if currently within the jump time window...
        else if (jumpTime >= Time.time)
        {
            //...and the jump button is held, apply an incremental force to the rigidbody...
            if (input.jumpHeld)
            {
                //rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);                
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpHoldForce);
            }
            else
            {
                //teste de pulo estilo hollow knigh link: https://pastebin.com/X0AytNFR
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            }
        }
        else if (isJumping)
        { 
            //if jump time is past, set isJumping to false
            if (jumpTime <= Time.time)
            {
                isJumping = false;

            }
        }

        //If player is falling too fast, reduce the Y velocity to the max
        if (rigidBody.velocity.y < maxFallSpeed)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);
        }
    }

    void Repell_Landing()
    {
        // force is how forcefully we will push the player away from the enemy.
        float force = repell_contact;
                       
        float pos_2d = transform.position.x;

        // Calculate Angle Between the collision point and the player
        Vector2 dir = new Vector2(-1, 0);

        // We then get the opposite (-Vector) and normalize it
        //dir = -dir.normalized;
        // And finally we add force in the direction of dir and multiply it by force. 
        // This will push back the player
        rigidBody.AddForce(dir * force);        
    }

    void FlipCharacterDirection()
	{
		//Turn the character by flipping the direction
		direction *= -1;

		//Record the current scale
		Vector3 scale = transform.localScale;

		//Set the X scale to be the original times the direction
		scale.x = originalXScale * direction;

		//Apply the new scale
		transform.localScale = scale;
	}

	void Crouch()
	{
		//The player is crouching
		isCrouching = true;

		//Apply the crouching collider size and offset
		bodyCollider.size = colliderCrouchSize;
		bodyCollider.offset = colliderCrouchOffset;
	}

	void StandUp()
	{
		//If the player's head is blocked, they can't stand so exit
		if (isHeadBlocked)
			return;

		//The player isn't crouching
		isCrouching = false;
	
		//Apply the standing collider size and offset
		bodyCollider.size = colliderStandSize;
		bodyCollider.offset = colliderStandOffset;
	}
    
    //executes in beginning of attack sequence, player cannot move while attacking
    void Attack_init_stop()
    {
        isAttacking = true;
        Stop_mov();
    }

    //executes in beginning of event, player cannot move
    void Stop_mov()
    {        
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        canMove = false;
    } 


    //executes in the end of attack sequence
    void Attack_end(int comboAnim)
    {
        isAttacking = false;
        canMove = true;
        //hitbox_Collider.ClearList();

        if (comboCounter == comboAnim)
        {
            comboCounter = 0;
        }
    }

    void Hurt_init()
    {
        rigidBody.velocity = new Vector2(0, 0);
        canMove = false;
    }

    void Hurt_end()
    {        
        canMove = true;
        anim.SetFloat("Hurt", 0f);
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
	{
		//Record the player's position
		Vector2 pos = transform.position;

		//Send out the desired raycasr and record the result
		RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

		//If we want to show debug raycasts in the scene...
		if (drawDebugRaycasts)
		{
			//...determine the color based on if the raycast hit...
			Color color = hit ? Color.red : Color.green;
			//...and draw the ray in the scene view
			Debug.DrawRay(pos + offset, rayDirection * length, color);
		}

		//Return the results of the raycast
		return hit;
	}

    public void Movementanim() {
        anim.SetBool("isOnGround", isOnGround);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("attackRequested", attackRequested);
        anim.SetBool("dashRequested", dashRequested);
        anim.SetInteger("comboCounter", comboCounter);
        anim.SetFloat("horizontalVelocity", Mathf.Abs(input.horizontal));
        anim.SetFloat("verticalVelocity", rigidBody.velocity.y);
    }
}
