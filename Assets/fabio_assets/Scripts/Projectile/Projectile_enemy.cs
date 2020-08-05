using UnityEngine;

public class Projectile_enemy : EnemyHealthClass
{
    [SerializeField]
    public float velX = -15;

    [SerializeField]
    public int damage = 5;


    float velY = 0;

    public override void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        Vector2 vel_default = new Vector2(velX, velY);

        DefineVel(vel_default);

        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        isAlive = true;
        currentHealth = startingHealth;
        rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Custom/HSVRangeShader");        
    }

    //Change Velocity of RB
    public void DefineVel(Vector2 vel)
    {
        rigidBody.velocity = vel;
    }

    //Handle Colisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            var player = other.gameObject.GetComponentInParent<PlayerHealth_custom>();

            player.TakeHit(damage);

            Destroy(gameObject);

        }

        if (other.tag == "Weapon")
        {
            Debug.Log("shot eraser");
            Destroy(gameObject);
        }
    }

}
