using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Move : MonoBehaviour
{
    public bool drawDebugRaycasts = true;   //Should the environment checks be visualized

    [Header("Movement Properties")]
    public float speed = 8f;                //Player speed
    public float maxFallSpeed = -25f;       //Max speed player can fall

    [Header("Jump Properties")]
    public float jumpForce = 6.3f;          //Initial force of jump

    [Header("Environment Check Properties")]
    public float footOffset = .4f;          //X Offset of feet raycast
    public float eyeHeight = 1.5f;          //Height of wall checks
    public float reachOffset = .7f;         //X offset for wall grabbing
    public float headClearance = .5f;       //Space needed above the player's head
    public float groundDistance = 0.5f;     //Distance player is considered to be on the ground
    public float grabDistance = .4f;        //The reach distance for wall grabs
    public LayerMask groundLayer;           //Layer of the ground

    [Header("Status Flags")]
    public bool isOnGround;                 //Is the player on the ground?
    public bool isJumping;                  //Is player jumping?
    public bool isHanging;                  //Is player hanging?
    public bool isCrouching;                //Is player crouching?
    public bool isHeadBlocked;              //Is head blocked?
    public bool canMove;
    public bool isAttacking;                //Is player attacking?

    //Enemy inputs

    [Header("Enemy inputs")]
    public float horizontal;      //Float that stores horizontal input
    [HideInInspector] public bool attackPressed;   //Bool that stores attack pressed

    BoxCollider2D bodyCollider;             //The collider component
    Rigidbody2D rigidBody;                  //The rigidbody component

    float jumpTime;							//Variable to hold jump duration
    float jumpdelay_time;                    //Variable to hold jump time delay
    float coyoteTime;                       //Variable to hold coyote duration
    float playerHeight;                     //Height of the player

    float originalXScale;                   //Original scale on X axis
    public bool facingRight = true;         //Direction enemy is facing

    Vector2 colliderStandSize;              //Size of the standing collider
    Vector2 colliderStandOffset;            //Offset of the standing collider
    Vector2 colliderCrouchSize;             //Size of the crouching collider
    Vector2 colliderCrouchOffset;           //Offset of the crouching collider    

    private Animator anim;                  //Animator component

    void Start()
    {
        //assuming player can move
        canMove = true;
        
        rigidBody = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
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
    }


    void FixedUpdate()
    {

        //Check the environment to determine status
        PhysicsCheck();

        //Process ground and air movements
        GroundMovement();      

        Movementanim();
    }

    void PhysicsCheck()
    {

        //Start by assuming the player isn't on the ground and the head isn't blocked        
        isOnGround = false;
        isHeadBlocked = false;

        //Cast rays for the left and right foot
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance);


        //If either ray hit the ground, the player is on the ground
        if (leftCheck || rightCheck)
            isOnGround = true;


        //Cast the ray to check above the player's head
        RaycastHit2D headCheck = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, headClearance);

        //If that ray hits, the player's head is blocked
        if (headCheck)
            isHeadBlocked = true;       
    }

    void GroundMovement()
    {
        if (canMove)
        {

            //Calculate the desired velocity based on inputs
            float xVelocity = horizontal* speed;

            //If the sign of the velocity and direction don't match, flip the character
            FlipCharacterDirection(facingRight);

            //Apply the desired velocity 
            rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);
            
        }       
    }      

    void FlipCharacterDirection(bool facingRight)
    {
        //Record the current scale
        Vector3 scale = transform.localScale;

        if (facingRight == true)
        {
            //Set the X scale to be the original times the direction
            scale.x = originalXScale;
        }
        else
        {
            scale.x = originalXScale * (-1);
        }        

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

    //executes in beginning of attack sequence, player cannot move while attacking
    void Attack_init_stop()
    {
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        canMove = false;
    }

    //executes in the end of attack sequence
    void Attack_end()
    {
        isAttacking = false;
        canMove = true;
    }


    //These two Raycast methods wrap the Physics2D.Raycast() and provide some extra
    //functionality
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length)
    {
        //Call the overloaded Raycast() method using the ground layermask and return 
        //the results
        return Raycast(offset, rayDirection, length, groundLayer);
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

    public void Movementanim()
    {
        anim.SetBool("isOnGround", isOnGround);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isAttacking", isAttacking);

        //

        anim.SetFloat("horizontalVelocity", Mathf.Abs(rigidBody.velocity.x));
        anim.SetFloat("verticalVelocity", rigidBody.velocity.y);
    }


}

