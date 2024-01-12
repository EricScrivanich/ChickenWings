using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg_Regular : MonoBehaviour
{
    private Vector2 startVelocity;
    [SerializeField] private GameObject player;
    private Rigidbody2D playerRB;
    public PlayerID ID;
    private Rigidbody2D rb;
    private Collider2D coll2D;
    private bool isCracked;
    private Animator anim;
    private bool isFirstActivation = true;
    private bool ready = false;


    // private Vector2 playerForce;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll2D = GetComponent<Collider2D>(); 
        

    }
    void Start()
    {

        // playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
       
        
        anim = GetComponent<Animator>();
        ready = true;

    }

    // Update is called once per frame
    void Update()
    {


        if (isCracked)
        {
            transform.Translate(-5 * Time.deltaTime, 0, 0);
            if (transform.position.x < BoundariesManager.leftBoundary)
            {

                gameObject.SetActive(false);
            }

        }


    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Floor"))
        {
            rb.simulated = false;
            coll2D.enabled = false;
            anim.SetBool("Cracked", true);
            AudioManager.instance.PlayCrackSound();
            isCracked = true;



        }
        if (collider.gameObject.CompareTag("Barn"))
        {

            gameObject.SetActive(false);

        }
    }
    private void OnEnable()
    {
        
        rb.velocity = new Vector2(ID.AddEggVelocity, -1.5f);
        
        


    }

    private void OnDisable()
    {
       
            rb.simulated = true;
            coll2D.enabled = true;
            isCracked = false;
       
    }


}
// private void OnEnable() {


//     if (!isFirstActivation)
//     {
//         rb.velocity = new Vector2(ID.AddEggVelocity, rb.velocity.y);
//     }
//     else
//     {
//         isFirstActivation = false;
//         return;
//     }
// }





// }
// private void GetVelocity(float velx)
// {
//     rb.velocity = new Vector2(velx,rb.velocity.y);

// }

// private void OnEnable() {
//     player.globalEvents.eggVelocity += GetVelocity;
// }
//  private void OnDisable() {
//     player.globalEvents.eggVelocity += GetVelocity;
// }



