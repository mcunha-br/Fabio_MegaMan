using UnityEngine;
using System.Collections;

public class EnemyHealthClass : MonoBehaviour {

    public int startingHealth;    
    //public float timeSinceLastHit;    
    //public new AudioSource audio;
    public float timer = 0f;
    public Animator anim;    
    public bool isAlive;
    public Rigidbody2D rigidBody;
    public BoxCollider2D boxCollider;
    public int currentHealth;
    //public ParticleSystem blood;

    public Renderer rend;    
    public float flashTime;

    public bool IsAlive
    {
        get { return isAlive; }
    }

    // Use this for initialization
    public virtual void Start()
    {        
        //timeSinceLastHit = 0f;        
        
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();        
        anim = GetComponent<Animator>();        
        isAlive = true;
        currentHealth = startingHealth;

        rend = GetComponent<Renderer>();        
        rend.material.shader = Shader.Find("Custom/HSVRangeShader");

        //GameManager.instance.RegisterEnemy(this);
        //audio = GetComponent<AudioSource>();
        //blood = GetComponentInChildren<ParticleSystem>();
    }

    public void HitCheck(Collider2D weaponCol, int damage)
    {
        if (tag != "Dead")

        //StopAllCoroutines(); 
        TakeHit(damage);        
        timer = 0f;   
    }

    void TakeHit( int damage)    {
        
        if (currentHealth > 0)
        {
            StartCoroutine("HitFrames");
            //audio.PlayOneShot(audio.clip);            
            //anim.Play("Hurt");
            currentHealth -= damage;
        }

        if (currentHealth <= 0)
        {
            isAlive = false;
            KillEnemy();
        }
    }

    void KillEnemy()
    {
        //anim.SetTrigger("EnemyDie");
        Destroy(gameObject);

        /*
        boxCollider.enabled = false;        
        rigidBody.isKinematic = true;
        tag = "Dead";
        GameManager.instance.KilledEnemy(this);
        anim.SetTrigger("EnemyDie");
        */
    }

    IEnumerator HitFrames()
    {
        Vector4 hit_HSVA = new Vector4(-3.61f, -1f, 0.4f, 0f);

        Vector4 normal_HSVA = rend.material.GetVector("_HSVAAdjust");          

        rend.material.SetVector("_HSVAAdjust", hit_HSVA);

        yield return new WaitForSeconds(flashTime);
   
        rend.material.SetVector("_HSVAAdjust", normal_HSVA);
    }

    /* void OnTriggerEnter(Collider other)
{
 //(timer >= timeSinceLastHit && !GameManager.instance.GameOver && tag != "Dead")

 if (!GameManager.instance.GameOver && tag != "Dead")
 {
     if (other.tag == "PlayerWeapon")
     {
         takeHit();
         blood.Play();
         timer = 0f;
     }
 }
}*/


}
