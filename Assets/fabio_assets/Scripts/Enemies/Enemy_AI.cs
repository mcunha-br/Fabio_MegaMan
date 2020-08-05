using System.Collections;
using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    //public bool facingRight=false;
    public Transform target;
    public float alertDistance = 6;
    public float rangeDistance = 4;
    public float timeBetweenAttacks = 1f;
    public bool playerInRange = false;
    public bool playerAlert = false;           
    private Enemy_Move move;

    private void Start()
    {
        move = GetComponent<Enemy_Move>();
        StartCoroutine(Attack());
    }

    void Update()
    {
        Vector3 targetDir = target.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
        //inserir verificações de posição a esquerda e direito na funcao de Update e criar variavel facingRight            

        if (target.position.x <= transform.position.x)
        {
            move.facingRight = false;
        }
        else
        {
            if (target.position.x > transform.position.x)
            {
                move.facingRight = true;
            }
        }
    
        if ((targetDir.magnitude <= alertDistance) && (angle > 45.0f))
        {
            playerAlert = true;

            if ((targetDir.magnitude <= rangeDistance))
            {
                playerInRange = true;
                playerAlert = false;
            }
            else
            {
                playerInRange = false;
            }
        }
        else
        {
            playerAlert = false;            
        }

        FollowPlayer();

    }

    void FollowPlayer()
    {
        if (playerAlert)
        {
            if (!playerInRange)
            {
                //inserir verificações de posição a esquerda e direito na funcao de Update e criar variavel facingRight
                if (!move.facingRight)
                {
                    move.horizontal = -1;                    
                }
                else
                {
                    if (move.facingRight)
                    {
                        move.horizontal = 1;                        
                    }
                }
            }
        }
        else
        {
            move.horizontal = 0;
        }
    }

    //Realiza ataque 
    IEnumerator Attack(){
        if(playerInRange == true)
        {
            move.isAttacking = true;            
            yield return new WaitForSeconds(timeBetweenAttacks);
        }

        yield return null;
        StartCoroutine(Attack());
    }            
}