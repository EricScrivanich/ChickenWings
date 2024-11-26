using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static int lastRingOrderPassed = 0;
    private Rigidbody2D rb;
    [SerializeField] private GameObject frozenChicken;

    [Header("Jump Settings")]
    [SerializeField] private float sideJumpStrengthX;
    [SerializeField] private float sideJumpStrengthY;
    [SerializeField] private float jumpStrength;
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationLerpSpeed = .5f;
    [SerializeField] private int jumpRotationSpeed;
    [SerializeField] private int sideRotationSpeed;
    [SerializeField] private int frozenRotationSpeed;
    [SerializeField] private int maxRotationUp = 15;
    [SerializeField] private int maxRotationDown = -30;
    private float rotZ;
    private float frozenRotZ;
    private int sideRotationVar;

    [Header("Boundary Settings")]
    [SerializeField] private float topBoundry = 6.4f;
    [SerializeField] private float unfreezeBoundry = 3f;
    [SerializeField] private float upperSdJmpBoundry = 6.5f;
    [SerializeField] private float lowerSdJmpBoundry = -5f;

    [Header("Floating Settings")]
    [SerializeField] private float floatingForceJmp;
    [SerializeField] private float startFloatingJmp;
    [SerializeField] private float floatingDurationJmp;
    [SerializeField] private float floatingForceFlp;
    [SerializeField] private float floatingForceFlp_X;
    [SerializeField] private float startFloatingFlp;
    [SerializeField] private float floatingDurationFlp;
    private float floatingForce;
    private float startFloating;
    private float floatingDuration;
    private float floatingTimer;
    private float floatingForce_X;
    
    
    [Header("State Variables")]
    public bool isFrozen;
    public bool sideJumpBool;
    public bool disableKeyPress;
    private bool jumpHeld = false;
    public float maxFallSpeed;
    private float originalGravityScale;
    private float addJumpForceVar;

 void Awake()
    {
        isFrozen = false;
        addJumpForceVar = floatingForce;
        sideJumpBool = false;
        sideRotationVar = 0;
        disableKeyPress = false;
        floatingDuration = floatingDurationJmp;
    }

    void Start()
    {
        frozenChicken.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
        rb.linearVelocity = new Vector2(0, 4f);
        AudioManager.instance.PlayStartSound();
    }
public void StartJumpHold()
{
    floatingTimer = 0;
 
    
    
    jumpHeld = true;
}

public void StopJumpHold()
{
  
    jumpHeld = false;
}

    void FixedUpdate()
{
    
   
    // Check if the player's downward velocity exceeds the max
    if (rb.linearVelocity.y < maxFallSpeed)
    {
        // If it does, limit it to the max fall speed
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);
    }
     if (jumpHeld)
    {
        floatingTimer += Time.fixedDeltaTime;
        rb.AddForce(new Vector2 (0,floatingForceJmp));
    }
    if (floatingTimer >= floatingDuration)
    {
   
        jumpHeld = false;
    }
}

  void Update()
    {
        // KeyPress();
        JumpRotation();
        Frozen();
    }

    public void Jump()
    {
         if (disableKeyPress)
        {
            return;
        }
        rb.linearVelocity = new Vector2(0, jumpStrength);
        AudioManager.instance.PlayCluck();
    }

    public void JumpRight()
    {
         if (disableKeyPress)
        {
            return;
        }
        if (rb.linearVelocity.y >= lowerSdJmpBoundry && rb.linearVelocity.y <= upperSdJmpBoundry)
        {
            rb.linearVelocity = new Vector2(sideJumpStrengthX, sideJumpStrengthY);
            AudioManager.instance.PlayCluck();
            sideRotationVar = -sideRotationSpeed;
            sideJumpBool = true;
        }
    }

    public void JumpLeft()
    { 
        if (disableKeyPress)
        {
            return;
        }
        if (rb.linearVelocity.y >= lowerSdJmpBoundry && rb.linearVelocity.y <= upperSdJmpBoundry)
        {
            rb.linearVelocity = new Vector2(-sideJumpStrengthX - .2f, sideJumpStrengthY - .4f);
            AudioManager.instance.PlayCluck();
            sideRotationVar = sideRotationSpeed;
            sideJumpBool = true;
        }
    }

        void Frozen()
    {
        if (transform.position.y > topBoundry && !disableKeyPress)
        {
            disableKeyPress = true;
            AudioManager.instance.PlayFrozenSound();
            isFrozen = true;
            frozenChicken.SetActive(true);
        }

        if (isFrozen)
        {
            frozenRotZ += frozenRotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0,0, frozenRotZ);
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2 (0, -6f);
        }

        if (transform.position.y < unfreezeBoundry && isFrozen)
        {
            isFrozen = false;
            frozenChicken.SetActive(false);
            disableKeyPress = false;
            rb.gravityScale = originalGravityScale;
        }
    }

    void JumpRotation()
    {
        if (disableKeyPress)
        {
            return;
        }
        
        if (sideJumpBool)
        {
            rotZ += sideRotationVar * Time.deltaTime;

            if (rotZ >= 390 || rotZ <= -360)
            {
                rotZ = 0;
                sideJumpBool = false;
            }
        }
        else
        {
            // If the object is moving upwards, rotate it upwards
            if (rb.linearVelocity.y > 0 && rotZ < maxRotationUp)
            {
                // Calculate the new rotation
                rotZ += jumpRotationSpeed * Time.deltaTime;
            }
            // If the object is moving downwards, rotate it downwards
            else if (rb.linearVelocity.y < 0 && rotZ > maxRotationDown)
            {
                // Calculate the new rotation
                rotZ -= jumpRotationSpeed * Time.deltaTime;
            }
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0, rotZ), Time.deltaTime* rotationLerpSpeed);
    }

}


    



    


 

