using UnityEngine;

public class Projectile : MonoBehaviour {

    public float lifetime;
    public int damage = 5;

    //Default velocity X and Y

    [SerializeField]
    public float velX = -15;

    float velY = 0;

    Vector2 velocity;
    Rigidbody2D rigidBody;            

    //Initialize default velocity
    public virtual void Start()
    {        
        rigidBody = GetComponent<Rigidbody2D>();

        if ((rigidBody.velocity.x == 0) & (rigidBody.velocity.y == 0))
        {
            Vector2 vel_default = new Vector2(velX, velY);

            DefineVel(vel_default);
        }
    }

    //Change Velocity of RB
    public void DefineVel(Vector2 vel)
    {
        rigidBody = GetComponent<Rigidbody2D>();
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

    public void Update()
    {
        //Destroy projectile after lifetime is over.
        Destroy(gameObject, lifetime);
    }
    
}
