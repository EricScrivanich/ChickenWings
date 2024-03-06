using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
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
    [SerializeField] private float dodgeDistance;

    void Start()
    {
        lives = ID.Lives;
       
       
     
       
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
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

        if (collision.gameObject.CompareTag("Ring"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 normal = contact.normal;

                // Determine direction based on the normal
                // Assuming a simple scenario where a positive normal.x suggests a hit from the left
                if (normal.x > 0)
                {
                    // Move player to the right
                    transform.position += Vector3.right * dodgeDistance;
                }
                else
                {
                    // Move player to the left
                    transform.position += Vector3.left * dodgeDistance;
                }

                break; // Assuming we only care about the first contact point
            }
        }
    }
    

     private void OnTriggerEnter2D(Collider2D collider)
    {
    
         if (collider.gameObject.tag == "Plane" && !isFlashing) // && !isFlashing 
        {
            
            ID.Lives -= 1;
            if (ID.Lives <= 0)
            {
                Kill();
                DeadEvent.TriggerEvent();
                return;
            }
            
            AudioManager.instance.PlayDamageSound();
            Instantiate(featherParticles, transform.position, Quaternion.identity);
            StartCoroutine(Flash()); // Start the flashing coroutine
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
               
}

private void OnEnable() 
{
    // ID.events.FloorCollsion += Dropping;
}

private void OnDisable() 
{
    // ID.events.FloorCollsion -= Dropping;
}
}


