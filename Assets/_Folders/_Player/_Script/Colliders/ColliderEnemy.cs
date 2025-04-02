using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEnemy : MonoBehaviour
{
    public PlayerID ID;
    private bool floorCollision = true;
    public GameEvent DeadEvent;



    [SerializeField] private GameObject featherParticles;
    [SerializeField] private GameObject smokeParticles;
    // private SpriteRenderer spriteRenderer;
    private bool isFlashing;
    private float flashDuration;
    [SerializeField] private int numberOfFlashes = 5; // Number of times to flash
    [SerializeField] private float totalFlashTime = 1f;

    private PlayerStateManager player;



    private void Awake()
    {

    }
    void Start()
    {
        player = GetComponentInParent<PlayerStateManager>();
        player.SetColliders(this, null);

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

        if (collider.CompareTag("Plane") || collider.CompareTag("Block") || collider.CompareTag("Weapon")) // && !isFlashing 
        {
            player.HandleDamaged();
            return;

        }

        // else if (collider.CompareTag("Weapon"))
        // {
        //     if (player.CanPerectParry || player.CanParry)
        //     {
        //         IParryable parryableAttack = collider.gameObject.GetComponent<IParryable>();
        //         if (parryableAttack != null)
        //         {
        //             parryableAttack.Parry();
        //             AudioManager.instance.PlayParryNoise(true);
        //         }
        //     }


        //     else player.HandleDamaged();


        // }

        IExplodable explodableEntity = collider.gameObject.GetComponent<IExplodable>();
        if (explodableEntity != null)
        {
            player.HandleDamaged();
            explodableEntity.Explode(false);
        }



    }






}

