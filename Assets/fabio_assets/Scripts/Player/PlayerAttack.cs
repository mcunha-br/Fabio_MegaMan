using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;	//Animator component        
    private PlayerMovement_custom movClass; //PlayerMov class
    public int max_combo;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        movClass = GetComponent<PlayerMovement_custom>();

    }

    // Update is called once per frame
    void Update()
    {
        ComboCheck(max_combo);
    }

    public void ComboCheck(int max_combo)
    {        
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1_ground") || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2_ground")) && movClass.attackRequested == true)
        {   //If the first animation is still playing attack is requested increase combo number

            if (movClass.comboCounter >= max_combo)
            {
                movClass.comboCounter = max_combo;
            }
            else
            {
                movClass.comboCounter += 1;
            }            
        }
    }
}
