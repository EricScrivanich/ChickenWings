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
    private SpriteRenderer spriteRenderer;
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
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
       
       
     
       
        // Get the SpriteRenderer component
        flashDuration = totalFlashTime / (numberOfFlashes * 2);
    }
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor" && floorCollision)
        {
            lives = 0;
            Kill();
            DeadEvent.TriggerEvent(); 
        }
    }

     private void OnTriggerEnter2D(Collider2D collider)
    {
    
         if (collider.CompareTag("Plane") && !isFlashing) // && !isFlashing 
        {
            lives -= 1;
            ID.globalEvents.LoseLife?.Invoke(lives);
            if (lives <= 0)
            {
                Kill();
                DeadEvent.TriggerEvent(); 
                return;
            }
            
            AudioManager.instance.PlayDamageSound();
            Instantiate(featherParticles, transform.position, Quaternion.identity);
            StartCoroutine(Flash()); // Start the flashing coroutine
        }

        if (collider.CompareTag("Ring"))
        {
            RingMovement ring = collider.GetComponent<RingMovement>();
            
            if (ring != null)
            {
                ring.CheckOrder();

            }
        }

         
    }

private void Dropping(bool floorCollisionVar)
{
    floorCollision = floorCollisionVar;

}

    private IEnumerator Flash()
{
    isFlashing = true;

    for (int i = 0; i < numberOfFlashes; i++)
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // Set opacity to 0
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // Set opacity to 1
        yield return new WaitForSeconds(flashDuration);
    }

    isFlashing = false;
}


private void Kill()
{
    Instantiate(featherParticles, transform.position, Quaternion.identity);
    Instantiate(smokeParticles,transform.position, Quaternion.identity);
    AudioManager.instance.PlayDeathSound(); 
                
    gameObject.SetActive(false);
    
    if(transform.parent != null) // Check if this GameObject has a parent
    {
        transform.parent.gameObject.SetActive(false);
    }
    else
    {
        gameObject.SetActive(false); // If no parent, disable the current GameObject
    }     
               
}

private void OnEnable() 
{
    ID.events.FloorCollsion += Dropping;
}

private void OnDisable() 
{
    ID.events.FloorCollsion -= Dropping;
}
}
