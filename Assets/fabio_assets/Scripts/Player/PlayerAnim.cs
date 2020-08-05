using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator anim;			        //Animator component
    private PlayerMovement_custom movClass; //PlayerMov class
    PlayerInput_custom input;				//The current inputs for the player
    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput_custom>();
        anim = GetComponent<Animator>();
        movClass = GetComponent<PlayerMovement_custom>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movementanim();
    }

    public void Movementanim()
    {
        anim.SetBool("isOnGround", movClass.isOnGround);
        anim.SetBool("isJumping", movClass.isJumping);
        anim.SetBool("isCrouching", movClass.isCrouching);

        anim.SetBool("attackRequested", movClass.attackRequested);
        anim.SetInteger("comboCounter", movClass.comboCounter);

        anim.SetFloat("horizontalVelocity", Mathf.Abs(input.horizontal));
        anim.SetFloat("verticalVelocity", rigidBody.velocity.y);        
    }
}
