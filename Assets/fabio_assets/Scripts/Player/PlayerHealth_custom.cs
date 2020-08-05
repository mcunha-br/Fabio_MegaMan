using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class PlayerHealth_custom : MonoBehaviour {

    [SerializeField] private int startingHealth = 50;

    [SerializeField] private float invTime = 2.5f;    

    private float timer = 0f;
    private float timerHit = 0f;

    private Animator anim;

    public int currentHealth;    

    private PlayerMovement_custom playerMovement;

    private SpriteRenderer sprRend;
    
    public float flashTime;

    /*
    private Renderer rend;
    public int colorMask = 10;
    */
        
    // Use this for initialization
    void Start () {

        anim = GetComponent<Animator>();

        //rend = GetComponent<Renderer>();        
        //rend.material.shader = Shader.Find("Custom/HSVRangeShader");                   

        sprRend = GetComponent<SpriteRenderer>();

        currentHealth = startingHealth;
        playerMovement = GetComponent<PlayerMovement_custom>();
        
    }
	
	// Update is called once per frame
	void Update () {
        
        timer += Time.deltaTime;        
        /*
        if (timer >= invTime)
            rend.material.SetFloat("_ColorMask", 15);
        */
    }
        
    public void TakeHit(int damage){

        //Debug.Log("timer: " + timer.ToString());
        //Debug.Log("currentHealth: " + currentHealth.ToString());
        //&& (timer >= invTime)
        
        if ((currentHealth > 0) && (timer >= invTime))
        {
            timer = 0;
            StartCoroutine("InvincibleFrames");

            //Debug.Log("currentHealth: " + currentHealth.ToString());
            anim.SetFloat("Hurt", 0.5f);            

            currentHealth -= damage;            
            
            //healthSlider.value = currentHealth;
            //audio.PlayOneShot(audio.clip);
            //blood.Play();
        }
        else
        {
            if (currentHealth <= 0)
            {
                Debug.Log("morto");
                // KillPlayer();
            }
        }                     
    }

    IEnumerator InvincibleFrames()
    {
        while (true)
        {
            if (timer >= invTime)
            {
                sprRend.enabled = true;
                yield break;
            }

            sprRend.enabled = !sprRend.enabled;
            yield return new WaitForSeconds(flashTime);            
        }
    }

    /*
    void KillPlayer(){
        
        anim.SetTrigger("HeroDie");
        characterController.enabled = false;
        audio.PlayOneShot(audio.clip);
        blood.Play();
    }
    */

    /*
    public void PowerUpHealth()
    {
        if (currentHealth <= 70){
            CurrentHealth += 30;
        }
        else if (currentHealth < startingHealth){
            CurrentHealth = startingHealth; 
        }
        healthSlider.value = currentHealth;
    }
    */

}
