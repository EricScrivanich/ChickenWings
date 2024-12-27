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
        Debug.Log("collide solid");

        if (collision.gameObject.CompareTag("Floor")) //&& floorCollision
        {

            ID.events.HitGround?.Invoke();
            // ID.Lives = 0;
            // Kill();
            // DeadEvent.TriggerEvent();
        }
        else if (collision.gameObject.CompareTag("Manager"))
        {
            ID.events.OnWater?.Invoke(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Manager"))
        {
            ID.events.OnWater?.Invoke(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("collide trig");
        Debug.Log(collider.gameObject);

        if (collider.CompareTag("Plane")) // && !isFlashing 
        {
            ID.events.LoseLife?.Invoke();

        }
        IExplodable explodableEntity = collider.gameObject.GetComponent<IExplodable>();
        if (explodableEntity != null)
        {
            explodableEntity.Explode(false);
        }



    }






}

