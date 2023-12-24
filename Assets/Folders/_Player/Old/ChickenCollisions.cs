using System.Collections;
using UnityEngine;

public class ChickenCollisions : MonoBehaviour
{
    private StatsManager statsMan;
    private Transform _transform;
    private PowerUps pwr;
    [SerializeField] private GameObject featherParticles;

    [SerializeField] private int numberOfFlashes = 5; // Number of times to flash
    [SerializeField] private float totalFlashTime = 1f; // Total duration of flashing (in seconds)
    private float flashDuration;
    private bool isFlashing = false; // Flag to check if the flashing is in progress
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
   

    void Start()
    {
        
        statsMan = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();
       
        _transform = GetComponent<Transform>();
        pwr = gameObject.GetComponent<PowerUps>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        flashDuration = totalFlashTime / (numberOfFlashes * 2);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
    
        if (collider.gameObject.tag == "Plane" && !isFlashing)
        {
            statsMan.LoseLife(1);
            int lives = statsMan.GetLives();



            if (lives <= 0)
            {
                return;
            }
            AudioManager.instance.PlayDamageSound();
            Instantiate(featherParticles, _transform.position, Quaternion.identity);
            StartCoroutine(Flash()); // Start the flashing coroutine
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            if (pwr.isDropping)
            {
                //pwr.bounce = true;
                pwr.Bounce();
            }
            else
            {
                statsMan.LoseLife(3);
            }
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
