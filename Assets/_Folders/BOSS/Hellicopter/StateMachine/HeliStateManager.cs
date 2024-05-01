
using UnityEngine;
using System;
using System.Collections;

public class HeliStateManager : MonoBehaviour, IDamageable
{
    [ExposedScriptableObject]
    public HelicopterID ID;
    public bool targetRight;
    private bool isFlipped;

    public bool finishedFlipping;
    private Coroutine rotateShoot;

    public Transform player;
    private bool isHit;
    HeliBaseState currentState;
    public HeliNormalState NormalState = new HeliNormalState();
    public HeliFlipState FlipState = new HeliFlipState();
    public HeliDipState DipState = new HeliDipState();
    public HeliFollowState FollowState = new HeliFollowState();

    public Animator anim;

    public Rigidbody2D rb { get; private set; }

    private void Awake()
    {
        isHit = false;
        finishedFlipping = false;
        ID.isFlipped = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        targetRight = false;
        isFlipped = false;
        anim = GetComponent<Animator>();
        ID.shootingCooldown = 5.6f;
        ID.Lives = 3;
        ID.bulletAmount = 3;
        ID.shotDuration = 2.5f;
        ID.minSwitchTime = 5.5f;
        ID.minSwitchTime = 8f;

        rb = GetComponent<Rigidbody2D>();


        currentState = NormalState;

        currentState.EnterState(this);
    }

    public void Hit(int damageAmount)
    {
        if (isHit)
        {
            return;
        }

        isHit = true;
        StartCoroutine(HitEffect());
        anim.SetTrigger("HitTrigger");
        ID.Lives--;

        if (ID.Lives == 2)
        {
            Invoke("TempSwitch", 3);
            ID.bulletAmount = 4;
            ID.shootingCooldown = 4;
        }
        else if (ID.Lives == 1)
        {
            ID.minSwitchTime = 3.2f;
            ID.minSwitchTime = 4.5f;
            ID.shootingCooldown = 2.6f;

        }
        else if (ID.Lives == 0)
        {
            gameObject.SetActive(false);
        }




    }
    void OnTriggerEnter2D(Collider2D collider)
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }
    public void SwitchState(HeliBaseState state)
    {
        currentState.ExitState(this);
        currentState = state;
        Debug.Log(state);
        state.EnterState(this);

    }
    private void OnDisable()
    {

    }

    public void RotateAndShoot(bool start)
    {
        if (start)
        {
            rotateShoot = StartCoroutine(RotateAndShootCourintine(35, 3.2f));
        }
        else if (rotateShoot != null)
        {
            StopCoroutine(rotateShoot);

        }


    }

    public IEnumerator RotateAndShootCourintine(float rotationSpeed, float rotationDuration)
    {
        float elapsedTime = 0f;
        bool isFlipped = transform.localScale.x < 0;  // Check if the helicopter is flipped along the Y-axis

        // Calculate the initial and target rotation
        Quaternion initialRotation = transform.rotation;
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;

        // If flipped, adjust the angle
        if (isFlipped)
        {
            angle -= 180f;  // Adjust the angle to account for the flip
        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Invoke shooting event or method
        ID.events.shoot?.Invoke(1.2f, 5);

        // Rotate over time
        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationDuration;
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            yield return null;
        }

        // Call your shoot function here, if not done in the event
        // Shoot();

        yield return new WaitForSeconds(ID.shootingCooldown);

        // Restart the coroutine to repeat the behavior
       rotateShoot =  StartCoroutine(RotateAndShootCourintine(rotationSpeed, rotationDuration));
    }

    public void TempSwitch()
    {
        SwitchState(DipState);

    }




    public void FlipRotation(bool flip)
    {
        if (flip != isFlipped)
        {
            StartCoroutine(RotateY(.3f, flip));
            isFlipped = !isFlipped;

        }
        else
        {
            finishedFlipping = true;
        }

    }

    private IEnumerator RotateY(float duration, bool flipVar)
    {
        float startScale = 1.1f;
        float endScale = -1.1f;
        float initialZ = transform.rotation.eulerAngles.z;
        bool hasFlipped = false;
        ID.isFlipped = true;
        if (!flipVar)
        {
            ID.isFlipped = false;
            startScale = -1.1f;
            endScale = 1.1f;


        }
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            float currentScale = Mathf.Lerp(startScale, endScale, timeElapsed / duration);


            transform.localScale = new Vector2(currentScale, 1.1f);
            if (Mathf.Abs(currentScale) < 0.05f && !hasFlipped) // Threshold for flipping rotation
            {
                transform.rotation = Quaternion.Euler(0, 0, -initialZ);
                hasFlipped = true;

            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector2(endScale, 1.1f);
        transform.rotation = Quaternion.Euler(0, 0, -initialZ);

        finishedFlipping = true;

    }
    //     float startAngle = 0;
    //     float endAngle = 40;
    //     float snapAngle = 140;
    //     float finalAngle = 180;
    //     // First rotation from 0 to 20 degrees
    //     if (!flipVar)
    //     {
    //         startAngle = 180;
    //         endAngle = 140;
    //         snapAngle = 40;
    //         finalAngle = 0;
    //     }

    //     float timeElapsed = 0;

    //     while (timeElapsed < duration)
    //     {
    //         float currentAngle = Mathf.Lerp(startAngle, endAngle, timeElapsed / duration);
    //         transform.rotation = Quaternion.Euler(0, currentAngle, transform.rotation.eulerAngles.z);
    //         timeElapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     // Snap rotation to 160 degrees
    //     transform.rotation = Quaternion.Euler(0, snapAngle, transform.rotation.eulerAngles.z);

    //     // Second rotation from 160 to 180 degrees

    //     timeElapsed = 0;

    //     while (timeElapsed < duration)
    //     {
    //         float currentAngle = Mathf.Lerp(snapAngle, finalAngle, timeElapsed / duration);
    //         transform.rotation = Quaternion.Euler(0, currentAngle, transform.rotation.eulerAngles.z);
    //         timeElapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     transform.rotation = Quaternion.Euler(0, finalAngle, transform.rotation.eulerAngles.z);
    //     finishedFlipping = true;
    // }


    private IEnumerator HitEffect()
    {
        yield return new WaitForSeconds(1.5f);

        isHit = false;

    }

    public void Damage(int damageAmount)
    {
        Hit(damageAmount);
    }
}

