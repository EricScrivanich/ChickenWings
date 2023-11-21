using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PowerUps : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private Coroutine returnAfterTimeCoroutine;
    private float timeToReturn = 3f;

    [SerializeField] private float dropSpeed;
    [SerializeField] private float bounceHeight;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashSpeed;
    [SerializeField] private GameObject fireballSpawn;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float dashCooldownTime = 1f;
    [SerializeField] private float dropCooldownTime = 1f;
    private float maxFallSpeed;
    private float dashCooldownTimer;
    private float dropCooldownTimer;

    private bool isBouncing;
    private bool isDashing;
    public bool isDropping;
    private bool canDash = true;
    private bool canDrop = true;
    private float originalGravityScale;


    public ObjectPool<GameObject> pool;

    void Awake()
    {
       
        
        pool = new ObjectPool<GameObject>(() => 
        {
            return Instantiate (fireballPrefab);
        }, fireball => 
        {
            fireball.gameObject.SetActive(true);
        }, fireball => 
        {
            fireball.gameObject.SetActive(false);
        }, fireball => 
        {
            Destroy(fireball.gameObject);
        }, false, 8, 15);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
        playerMovement = GetComponent<PlayerMovement>();
        maxFallSpeed = playerMovement.maxFallSpeed;
       
    }

    void Update()
    {
        HandleCooldowns();

        // if (Input.GetKeyDown(KeyCode.D) && canDash)
        // {
        //     StartDash();
        // }

        // if (Input.GetKeyDown(KeyCode.S) && canDrop)
        // {
        //     StartDrop();
        // }

        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     Fireball();
        // }
    }

    private void HandleCooldowns()
    {
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (dropCooldownTimer > 0)
        {
            dropCooldownTimer -= Time.deltaTime;
        }
    }

    public void StartDash()
    {
        if (!isDashing && dashCooldownTimer <= 0)
        {
            isDashing = true;
            playerMovement.disableKeyPress = true;
            AudioManager.instance.PlayDashSound();
            StartCoroutine(Dash());
            dashCooldownTimer = dashCooldownTime;
        }
    }

    public void StartDrop()
    {
        if (!isDropping && dropCooldownTimer <= 0)
        {
            playerMovement.disableKeyPress = true;
            isDropping = true;
            playerMovement.maxFallSpeed = -30f;
            Drop();
            dropCooldownTimer = dropCooldownTime;
        }
    }

   

    private IEnumerator Dash()
    {
        float startTime = Time.time;
        
        // Set the dashing velocity
        rb.gravityScale = 0;
        rb.velocity = new Vector2(dashSpeed, 0);

        // Wait for dash to complete
        while (Time.time < startTime + dashDuration)
        {
            yield return null; // wait for next frame
        }

        // Reset the velocity after dashing
        rb.velocity = new Vector2(1f, rb.velocity.y);
        rb.gravityScale = originalGravityScale;
        
        playerMovement.disableKeyPress = false;
        isDashing = false;
    }

    public void Drop()
    {
        rb.velocity = new Vector2(0, dropSpeed);
        AudioManager.instance.PlayDownJumpSound(); 

        
    }
    public void Bounce()
    {
   
    
     if (isDropping)
        
            rb.velocity = new Vector2(.5f, bounceHeight);
            AudioManager.instance.PlayBounceSound(); 
            isDropping = false;
            playerMovement.disableKeyPress = false;
            playerMovement.maxFallSpeed = maxFallSpeed;
     
         
    
    }

    public void Fireball()
    {
        var fireball = pool.Get();
        fireball.transform.position = fireballSpawn.transform.position;
        
        if (playerMovement.sideJumpBool)
        {
            // when flipping
            float zRotation = RoundToNearest45(gameObject.transform.rotation.eulerAngles.z);
            fireball.transform.rotation = Quaternion.Euler(0, 0, zRotation);
        }
        else
        {
            fireball.transform.rotation = Quaternion.identity;
        }

        returnAfterTimeCoroutine = StartCoroutine(ReturnFireballAfterTime(fireball));
    }

    private float RoundToNearest45(float angle)
    {
        return Mathf.Round(angle / 45f) * 45f;
    }

    IEnumerator ReturnFireballAfterTime(GameObject fireball)
    {
        yield return new WaitForSeconds(timeToReturn);
        pool.Release(fireball);
    }
}
