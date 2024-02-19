using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : PlayerSystem
{
    public GameEvent DeadEvent;
    private int lives;
    [SerializeField] private GameObject featherParticles;
    private SpriteRenderer spriteRenderer;
    private bool isFlashing;
    private float flashDuration;
    [SerializeField] private int numberOfFlashes = 5; // Number of times to flash
    [SerializeField] private float totalFlashTime = 1f;

    void Start()
    {
        lives = player.ID.Lives;
       
       
     
       
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        flashDuration = totalFlashTime / (numberOfFlashes * 2);
    }
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            if (player.isDropping)
            {
                //pwr.bounce = true;
                player.ID.events.OnBounce?.Invoke();
            }
            else
            {
                lives = 0;
                DeadEvent.TriggerEvent(); 
            }
        }
    }

     private void OnTriggerEnter2D(Collider2D collider)
    {
    
         if (collider.gameObject.tag == "Plane" && !isFlashing) // && !isFlashing 
        {
            
            player.ID.Lives -= 1;
            if (player.ID.Lives <= 0)
            {
                DeadEvent.TriggerEvent();
                return;
            }
            
            AudioManager.instance.PlayDamageSound();
            Instantiate(featherParticles, _transform.position, Quaternion.identity);
            StartCoroutine(Flash()); // Start the flashing coroutine
        }

         
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
}
