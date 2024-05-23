using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEnemy : MonoBehaviour
{
    public PlayerID ID;
    private bool floorCollision = true;
    public GameEvent DeadEvent;
    private int lives;
    [SerializeField] private GameObject featherParticles;
    [SerializeField] private GameObject smokeParticles;
    // private SpriteRenderer spriteRenderer;
    private bool isFlashing;
    private float flashDuration;
    [SerializeField] private int numberOfFlashes = 5; // Number of times to flash
    [SerializeField] private float totalFlashTime = 1f;



    private void Awake()
    {

    }
    void Start()
    {
        lives = ID.Lives;
        // spriteRenderer = GetComponentInParent<SpriteRenderer>();




        // Get the SpriteRenderer component
        flashDuration = totalFlashTime / (numberOfFlashes * 2);
    }
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor" ) //&& floorCollision
        {
           
            ID.events.HitGround?.Invoke();
            // ID.Lives = 0;
            // Kill();
            // DeadEvent.TriggerEvent();
        }
    }
 
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.gameObject);
        IExplodable explodableEntity = collider.gameObject.GetComponent<IExplodable>();
        if (explodableEntity != null)
        {
            explodableEntity.Explode(false);
        }

        if (collider.CompareTag("Plane") ) // && !isFlashing 
        {
            ID.events.LoseLife?.Invoke();
            // ID.Lives -= 1;

            // if (ID.Lives <= 0)
            // {
            //     Kill();
            //     DeadEvent.TriggerEvent();
            //     return;
            // }

            // AudioManager.instance.PlayDamageSound();
            // Instantiate(featherParticles, transform.position, Quaternion.identity);
            // StartCoroutine(Flash()); // Start the flashing coroutine
        }

        // if (collider.CompareTag("Ring"))
        // {
        //     RingMovement ring = collider.GetComponent<RingMovement>();

        //     if (ring != null)
        //     {
        //         ring.CheckOrder();

        //     }
        //     else
        //     {
        //         BucketScript bucket = collider.GetComponent<BucketScript>();
        //         Debug.Log("bucket");
        //         // ID.events.OnCompletedRingSequence?.Invoke(bucket);
        //         bucket.Completed();
        //     }
        }




    }

    // private void Dropping(bool floorCollisionVar)
    // {
    //     floorCollision = floorCollisionVar;

    // }

    // private IEnumerator Flash()
    // {
    //     isFlashing = true;

    //     for (int i = 0; i < numberOfFlashes; i++)
    //     {
    //         // spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // Set opacity to 0
    //         ID.PlayerMaterial.SetFloat("_Alpha", 0);
    //         yield return new WaitForSeconds(flashDuration);
    //         // spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // Set opacity to 1
    //         ID.PlayerMaterial.SetFloat("_Alpha", .9f);
    //         yield return new WaitForSeconds(flashDuration);
    //     }
    //     ID.PlayerMaterial.SetFloat("_Alpha", 1);

    //     isFlashing = false;
    // }


    // private void Kill()
    // {
    //     Instantiate(featherParticles, transform.position, Quaternion.identity);
    //     Instantiate(smokeParticles, transform.position, Quaternion.identity);
    //     AudioManager.instance.PlayDeathSound();

    //     gameObject.SetActive(false);

    //     if (transform.parent != null) // Check if this GameObject has a parent
    //     {
    //         transform.parent.gameObject.SetActive(false);
    //     }
    //     else
    //     { 
    //         gameObject.SetActive(false); // If no parent, disable the current GameObject
    //     }

    // }

    // private void OnEnable()
    // {
        
    // }

    // private void OnDisable()
    // {
       
    // }

