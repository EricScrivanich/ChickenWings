using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] private GameObject Light;
    [SerializeField] private HelicopterID ID;
    private Pool pool;
    public float OffsetAmount;

    private Vector3 explosionScale = new Vector3(.7f, .7f, .7f);
    private Vector3 groundExplosionScale = new Vector3(1.1f, 1.2f, 1);
    [SerializeField] private float offDuration;
    [SerializeField] private float onDuration;
    [SerializeField] private Transform player; // Reference to the player
    [SerializeField] private float startingRotateSpeed; // Reference to the player
    private float rotateSpeed; // Reference to the player
    [SerializeField] private float speed = 5f; // Speed of the missile
    [SerializeField] private float maxVelocity;
    [SerializeField] private float waveFrequency = 2f; // Frequency of the wavy motion
    [SerializeField] private float waveMagnitude = 0.5f; // Magnitude of the wavy motion
    [SerializeField] private float detectionRange = 3f; // Range to switch to straight-line slowing down
    [SerializeField] private float straightLineDuration = 2f; // Duration to fly in a straight line while slowing down
    [SerializeField] private float lifeTimeDuration;
    [SerializeField] private float fallDuration;
    [SerializeField] private float offsetDuration;
    private float offset;
    private float offsetTime;
    private bool reachedOffset;
    private Collider2D col;

    private float time;
    private bool lightOn;
    private Rigidbody2D rb;
    private bool isSlowingDown = false;
    private float slowDownTimer = 0f;
    private Vector2 finalDirection;
    private float lifeTime;
    private bool hasFallen;
    private Vector2 OffsetPosition;

    private float currentOffset = 0f;
    private float offsetDirection = 1f;



    // Start is called before the first frame update
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    void Start()
    {


        pool = PoolKit.GetPool("ExplosionPool");

    }
    public void OnSpawn(Pool pool)
    {




        // Add Spawn stuff here!
    }
    public void OnDespawn()
    {
        // Add Despawn stuff here!
    }

    private void OnEnable()
    {
        
        reachedOffset = false;
        rotateSpeed = startingRotateSpeed;


        // offset = Random.Range(-9f, 10f);
        // offsetTime = 0;


        time = 0;
        hasFallen = false;
        lightOn = false;
        Light.SetActive(false);
        isSlowingDown = false;
        slowDownTimer = 0f;
        lifeTime = 0;
        currentOffset = offset / 2; // Start at half of the random offset

        if (ID.GetOffset() == 0)
        {
            reachedOffset = true;
            return;
        }
        OffsetPosition = new Vector2(player.position.x, ID.GetOffset());
        

    }

    private void OnDisable()
    {
        rb.gravityScale = .8f;
        col.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        // Handle the light blinking
        time += Time.deltaTime;
        lifeTime += Time.deltaTime;

        if (lifeTime > fallDuration && !hasFallen)
        {
            hasFallen = true;

            rb.velocity = new Vector2(rb.velocity.x, 0);
            col.enabled = true;
            rb.gravityScale = 0;
        }

        if (lightOn && time > onDuration)
        {
            lightOn = false;
            Light.SetActive(false);
            time = 0;
        }
        else if (!lightOn && time > offDuration)
        {
            lightOn = true;
            Light.SetActive(true);
            time = 0;
        }
    }

    private void FixedUpdate()
    {
        if (hasFallen)
        {
            // Check the distance to the player
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // if ((distanceToPlayer <= detectionRange || offsetTime > offsetDuration) && !isSlowingDown)
            if (lifeTime > lifeTimeDuration && !isSlowingDown)
            {
                isSlowingDown = true;
                rotateSpeed = 130;
                onDuration = 0.2f;
                offDuration = 0.2f;
                slowDownTimer = straightLineDuration;
                finalDirection = rb.velocity.normalized; // Keep the current direction
            }

            if (isSlowingDown)
            {
                // Continue flying in a straight line while slowing down
                slowDownTimer -= Time.fixedDeltaTime;
                speed = maxVelocity * slowDownTimer / straightLineDuration;
                if (slowDownTimer <= 0)
                {
                    NormalExplosion();
                    rb.velocity = Vector2.zero; // Stop the missile when the timer ends
                }

                Vector2 direction = (Vector2)player.position - rb.position;

                // Normalize the direction vector
                direction.Normalize();

                // Calculate rotation
                float rotateAmount = Vector3.Cross(direction, transform.up).z;

                // Apply rotation and velocity to the rigidbody
                rb.angularVelocity = -rotateAmount * rotateSpeed;
                rb.velocity = transform.up * speed;
            }
            else if (!reachedOffset)
            {



                float distanceToTarget = Vector2.Distance(transform.position, OffsetPosition);
                if (distanceToTarget < 2.5f)
                {
                    reachedOffset = true;
                    rotateSpeed += (Random.Range(130, 150));
                }
        
                Vector2 direction = OffsetPosition - rb.position;

                direction.Normalize();

           
                float rotateAmount = Vector3.Cross(direction, transform.up).z;

      
                rb.angularVelocity = -rotateAmount * rotateSpeed;
                rb.velocity = transform.up * maxVelocity;
            }

            else
            {
                Vector2 direction = (Vector2)(player.position) - rb.position;

          
                direction.Normalize();
                float rotateAmount = Vector3.Cross(direction, transform.up).z;
                rb.angularVelocity = -rotateAmount * rotateSpeed;
                rb.velocity = transform.up * maxVelocity;

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.gameObject);

        IDamageable damageableEntity = collider.gameObject.GetComponent<IDamageable>();
       

        if (collider.gameObject.CompareTag("Floor"))
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y - .4f);
            pool.Spawn("GroundExplosion", position, Vector3.zero, groundExplosionScale);
            pool.Spawn("ExplosionBlemish", position, Vector3.zero, groundExplosionScale);
            pool.Despawn(this.gameObject);
        }
        else
        {
            if (damageableEntity != null)
            {
                damageableEntity.Damage(1);
               

            }
            NormalExplosion();

        }


    }

    private void NormalExplosion()
    {
        pool.Spawn("NormalExplosion", transform.position, transform.rotation, explosionScale);
        pool.Despawn(this.gameObject);
     
      

    }


    // public void Explode(bool isGround)
    // {
    //     if (isGround)
    //     {
    //         Vector2 position = new Vector2(transform.position.x, transform.position.y - .4f);
    //         pool.Spawn("GroundExplosion", position, Vector3.zero, groundExplosionScale);
    //         pool.Spawn("ExplosionBlemish", position, Vector3.zero, groundExplosionScale);
    //         pool.Despawn(this.gameObject);
    //     }
    //     else
    //     {
    //         NormalExplosion();


    //     }


    // }
}
