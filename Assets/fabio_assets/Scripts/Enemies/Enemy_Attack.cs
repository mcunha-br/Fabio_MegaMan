using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform fireLocation;
    [SerializeField] float speed=10f;
    [SerializeField] float lifetime=5.0f;

    private bool facingRight;


    private void Update()
    {
        facingRight = GetComponent<Enemy_Move>().facingRight;
    }

    //projetil
    public void FireShot()
    {
        //checks which direction character is facing
        int dir = 0;

        if (facingRight)
        {
            dir = 1;
        }
        else
        {
            dir = -1;
        }

        // instantiate bullet projectile and it's position and rotation
        GameObject newProj = Instantiate(bullet, fireLocation.position, transform.rotation) as GameObject;

        //defines projectile lifetime
        newProj.GetComponent<Projectile>().lifetime = 5.0f;

        // defines projectile position and rotation
        //newProj.transform.position = fireLocation.position;
        //newProj.transform.rotation = transform.rotation;

        // defines projectilevelocity
        newProj.GetComponent<Projectile>().DefineVel(dir * transform.right * speed);        
    }
}
