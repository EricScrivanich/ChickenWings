using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlappyPigMovement : MonoBehaviour, IEggable
{
    public float scaleFactor;
    [SerializeField] private AnimationDataSO animData;
    [SerializeField] private SpriteRenderer yolkSR;
    [SerializeField] private Vector3 yolkStartScale;
    [SerializeField] private Vector3 yolkEndScale;
    [SerializeField] private float yolkStartY;
    [SerializeField] private float yolkEndY;
    [SerializeField] private float yolkDripDuration;
    [SerializeField] private float yolkScaleMultiplier;
    [SerializeField] private float blindedTime;
    [SerializeField] private Transform sprite;




    [SerializeField] private float addUpForceMinY;
    [SerializeField] private float addUpForceAmountBlinded;
    [SerializeField] private float addUpForceAmount;
    private Sequence flapSeq;
    private Sequence yolkSeq;

    private Coroutine EggYolkRoutine;

    [SerializeField] private float bounceForceMagnitude;
    [SerializeField] private float eggForceMagnitude;

    [SerializeField] private float moveFreeDurationBounce;
    [SerializeField] private float moveFreeDurationEgg;
    [SerializeField] private float moveFreeDurationInverse;


    public float inverseMagnitude;

    private Rigidbody2D rb;
    private Animator anim;
    public float forceAdded;



    public float minYVelocity;


    [SerializeField] private CircleCollider2D col;

    private bool moveFree;
    private float moveFreeDuration;

    private float OnDirectionTime;

    private bool eggYolkUsed;


    private Transform playerTarget;

    [SerializeField] private Transform pupils;
    [SerializeField] private Transform pupilStartPositions;

    [SerializeField] private PlayerID player;
    // private LineRenderer lineRenderer;

    [Header("Rotation")]
    [SerializeField] private float maxRotation = 30f;


    private float eyeRadius = .03f;

    private bool blinded = false;


    private float currentMoveSpeed;

    [Header("Base Values")]
    public float switchFromTargetToPlayerRange;

    public float lerpSpeed;
    public float OnDirectionTimeMax;
    public float predictionTime;

    [SerializeField] private float tweenUpAmount;
    [SerializeField] private float tweenUpDuration;
    [SerializeField] private float tweenDownDuration;
    public float maxVel;



    [Header("Ranges")]
    [SerializeField] private Vector2 minMaxScale;
    public Vector2 minMaxTimeScale;
    public Vector2 minMaxSwitchDirectionRange;
    public Vector2 minMaxTweenYAmount;
    public Vector2 minMaxOnDirectionTimeMax;
    public Vector2 minMaxLookAheadTime;
    public Vector2 minMaxLerpTowardsPlayerSpeed;

    public Vector2 minMaxMagntiude;
    public Vector2 minMaxMass;

    // 3.5, 5.5 is base


    private PigMaterialHandler pigMatHandler;


    [SerializeField] private float blindTime;



    [Header("Movement Settings")]



    private float blindDurationVar;
    private float timeScale;
    private float maxMoveSpeed; // Max speed of leftward movement
    private float minMoveSpeed; // Min speed when close to the player

    private float sineTime; // Time to complete one sine wave cycle (used to define frequency)

    private Vector2 inverseDirection;

    private bool waitingOnDelay;

    private bool isTargetReached = true;

    private Vector2 targetPosition;



    private float cackleTime;

    private float delayTime;
    private float delayDuration;


    void Start()
    {

        playerTarget = GameObject.Find("Player").GetComponent<Transform>();
        pigMatHandler = GetComponent<PigMaterialHandler>();

        yolkSR.enabled = false;

        // pupilStartPos = new Transform[2];

        // for (int i = 0; i < 2; i++)
        // {
        //     pupilStartPos[i] = pupils[i].transform;
        // }
        // rb = GetComponent<Rigidbody2D>();


        // Determine initial direction based on starting position


        // InvokeRepeating("ConstantJump", jumpTime, jumpTime);
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        // lineRenderer = GetComponent<LineRenderer>();
    }



    private IEnumerator Egged()
    {
        // yolkSR.transform.localScale = yolkStartScale;
        if (yolkSeq != null && yolkSeq.IsPlaying())
            yolkSeq.Kill();
        yolkSeq = DOTween.Sequence();
        yolkSR.enabled = true;
        eggYolkUsed = true;
        yolkSR.sprite = animData.sprites[0];
        yolkSeq.Append(sprite.DORotate(Vector3.zero, yolkDripDuration));
        yolkSeq.Join(yolkSR.transform.DOScale(yolkEndScale, yolkDripDuration).From(yolkStartScale).SetEase(Ease.OutSine));
        yolkSeq.Join(yolkSR.transform.DOLocalMoveY(yolkEndY, yolkDripDuration).From(yolkStartY).SetEase(Ease.OutSine));
        yolkSeq.Play();



        yield return new WaitForSeconds(yolkDripDuration * .7f);
        yolkSR.sprite = animData.sprites[1];
        while (blindDurationVar > 0)
        {
            blindDurationVar -= Time.deltaTime;
            yield return null;
        }
        eggYolkUsed = false;

        if (yolkSeq != null && yolkSeq.IsPlaying())
            yolkSeq.Kill();
        yolkSeq = DOTween.Sequence();

        yolkSeq.Append(sprite.DOShakeRotation(.3f, 80, 10, 50));
        yolkSeq.Join(yolkSR.transform.DOScale(yolkEndScale * yolkScaleMultiplier, .3f));
        yolkSeq.Play();




        yolkSR.sprite = animData.sprites[2];
        yield return new WaitForSeconds(.1f);
        yolkSR.sprite = animData.sprites[3];
        yield return new WaitForSeconds(.1f);
        yolkSR.sprite = animData.sprites[4];
        yield return new WaitForSeconds(.1f);
        yolkSR.enabled = false;
        blinded = false;







    }

    private void SetVariablesBasedOnScale(float percentage)
    {

        // minMoveSpeed = Mathf.Lerp(minMinMaxMoveSpeed.x, maxMinMaxMoveSpeed.x, percentage);
        // maxMoveSpeed = Mathf.Lerp(minMinMaxMoveSpeed.y, maxMinMaxMoveSpeed.y, percentage);
        // yOffsetLerpSpeed = Mathf.Lerp(minMaxYSpeed.x, minMaxYSpeed.y, percentage);

        tweenUpAmount = Mathf.Lerp(minMaxTweenYAmount.x, minMaxTweenYAmount.y, percentage);
        rb.mass = Mathf.Lerp(minMaxMass.x, minMaxMass.y, percentage);
        lerpSpeed = Mathf.Lerp(minMaxLerpTowardsPlayerSpeed.x, minMaxLerpTowardsPlayerSpeed.y, percentage);
        switchFromTargetToPlayerRange = Mathf.Lerp(minMaxSwitchDirectionRange.x, minMaxSwitchDirectionRange.y, percentage);

        predictionTime = Mathf.Lerp(minMaxLookAheadTime.x, minMaxLookAheadTime.y, percentage);
        maxVel = Mathf.Lerp(minMaxMagntiude.x, minMaxMagntiude.y, percentage);

        timeScale = Mathf.Lerp(minMaxTimeScale.x, minMaxTimeScale.y, percentage);
        tweenUpDuration = .3f / timeScale;
        tweenDownDuration = .4f / timeScale;
        cackleTime = tweenUpDuration;
        sineTime = .7f / timeScale;


        // sineAmplitude = Mathf.Lerp(minMaxAmplitude.x, minMaxAmplitude.y, percentage);


    }

    private void OnEnable()
    {
        player.globalEvents.OnPlayerVelocityChange += UpdateTargetPosition;
        waitingOnDelay = true;

        transform.localScale = BoundariesManager.vectorThree1 * scaleFactor;
        anim.speed = 0;


        float scalePercentage = Mathf.InverseLerp(minMaxScale.x, minMaxScale.y, scaleFactor);

        SetVariablesBasedOnScale(scalePercentage);

        currentMoveSpeed = maxMoveSpeed;
        delayDuration = Random.Range(0f, sineTime);
        sprite.localScale = BoundariesManager.vectorThree1;
        rb.simulated = true;

    }

    private void OnDisable()
    {
        if (flapSeq != null && flapSeq.IsPlaying())
            flapSeq.Kill();
        if (yolkSeq != null && yolkSeq.IsPlaying())
            yolkSeq.Kill();
        player.globalEvents.OnPlayerVelocityChange -= UpdateTargetPosition;
    }

    private void TweenPig()
    {
        flapSeq = DOTween.Sequence();
        flapSeq.Append(sprite.DOLocalMoveY(tweenUpAmount, tweenUpDuration)).SetEase(Ease.OutSine);
        flapSeq.Append(sprite.DOLocalMoveY(0, tweenDownDuration)).SetEase(Ease.InSine);
        flapSeq.Play().SetLoops(-1);

    }


    void Update()
    {

        if (waitingOnDelay)
        {
            delayTime += Time.deltaTime;

            if (delayTime > delayDuration)
            {
                waitingOnDelay = false;

                // anim.speed = 1 * timeScale;
                anim.speed = 1;
                TweenPig();
                // Time.timeScale = .4f;



            }

            return;
        }



        cackleTime += Time.deltaTime;

        if (cackleTime > sineTime)
        {
            AudioManager.instance.PlayFlappyPigCackleSound();
            cackleTime = 0;
        }
        if (blinded) return;
        float xRange = transform.position.x - playerTarget.position.x;

        float rotTarget = Mathf.Lerp(0, maxRotation, Mathf.Abs(xRange) * .06f);

        if (xRange < 0)
            rotTarget *= -1;


        sprite.eulerAngles = new Vector3(0, 0, rotTarget);

        Vector2 direction = playerTarget.position - pupilStartPositions.position; // Calculate the direction to the player

        direction.Normalize(); // Normalize the direction

        // Move the pupil within the eye's radius
        pupils.localPosition = direction * eyeRadius;


    }



    void FixedUpdate()
    {


        col.offset = new Vector2(0, sprite.localPosition.y);

        if (moveFree)
        {
            moveFreeDuration -= Time.fixedDeltaTime;

            if (inverseDirection != Vector2.zero)
            {
                rb.AddForce(inverseDirection);
            }
            if (moveFreeDuration <= 0)
            {
                moveFree = false;
                moveFreeDuration = 0;
            }
            else
                return;
        }


        if (blinded && blindDurationVar > blindDurationVar * .35f && transform.position.y < -3)
        {
            rb.AddForce(Vector2.up * addUpForceAmountBlinded);
            return;

        }
        else if (transform.position.y < addUpForceMinY)
        {
            rb.AddForce(Vector2.up * addUpForceAmount);

        }

        OnDirectionTime += Time.fixedDeltaTime;

        if (OnDirectionTime >= OnDirectionTimeMax && rb.velocity.y < minYVelocity)
        {
            moveFreeDuration = moveFreeDurationInverse;

            // targetPosition = targetPosition * inverseMagnitude;
            inverseDirection = rb.velocity.normalized * inverseMagnitude;
            // rb.AddForce(rb.velocity.normalized * inverseMagnitude, ForceMode2D.Impulse);
            isTargetReached = true;
            // rb.AddForce(targetPosition, ForceMode2D.Impulse);
            OnDirectionTime = 0;
            moveFree = true;
            Debug.LogError("Inversing Direction");
            return;

        }


        if (!isTargetReached)
        {
            targetPosition = Vector2.MoveTowards(targetPosition, playerTarget.position, lerpSpeed * Time.fixedDeltaTime);

            MoveTowardsTarget(targetPosition);
        }
        else
        {
            MoveTowardsTarget(playerTarget.position);  // Move directly towards the player
        }

        CheckIfTargetReached();
    }

    private void MoveTowardsTarget(Vector2 target)
    {

        Vector2 direction = (target - rb.position).normalized;
        rb.AddForce(direction * forceAdded);
        if (rb.velocity.magnitude > maxVel)
        {
            rb.velocity = rb.velocity.normalized * maxVel;
        }

        // UpdateLineRenderer(target);

    }

    private void CheckIfTargetReached()
    {
        // Check proximity to the target to determine if it has been reached
        if ((targetPosition - rb.position).magnitude < switchFromTargetToPlayerRange)  // Threshold for reaching the target
        {
            isTargetReached = true;
        }
    }
    private void UpdateTargetPosition(Vector2 playerVelocity, float playerGravityScale)
    {
        if (!isTargetReached) return;

        OnDirectionTime = 0;

        // Calculate the future position of the player based on current velocity and gravity
        Vector2 futurePosition = (Vector2)playerTarget.position + playerVelocity * predictionTime;
        futurePosition += Vector2.up * Physics2D.gravity.y * playerGravityScale * Mathf.Pow(predictionTime, 2) / 2;

        // Determine the inverse magnitude to apply
        // This value determines how far in the opposite direction the target will be placed

        // if (futurePosition.y < yChangeMin)

        // {
        //     // 20% chance to invert direction
        //     if (Random.value < chanceOfInversion)
        //     {
        //         Debug.LogError("Changing Direction");
        //         // Normalize and invert the player's velocity
        //         Vector2 invertedDirection = playerVelocity.normalized * -1;
        //         // Apply the inverse magnitude to calculate the new target position
        //         futurePosition = (Vector2)playerTarget.position + invertedDirection * inverseMagnitude;
        //     }
        // }

        if (futurePosition.y < -3.7f)
            futurePosition = new Vector2(futurePosition.x, playerTarget.position.y + 2.5f);

        targetPosition = futurePosition;
        isTargetReached = false;  // Reset the target reached flag
    }

    // private void UpdateLineRenderer(Vector2 target)
    // {
    //     if (lineRenderer != null)
    //     {
    //         lineRenderer.SetPosition(0, transform.position);
    //         lineRenderer.SetPosition(1, target);
    //     }
    // }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            if (EggYolkRoutine != null)
                StopCoroutine(EggYolkRoutine);
            if (flapSeq != null && flapSeq.IsPlaying())
                flapSeq.Kill();

            flapSeq = DOTween.Sequence();
            rb.simulated = false;
            flapSeq.Append(sprite.DOScale(new Vector3(1.15f, .8f, 1), .25f).SetEase(Ease.OutSine));
            flapSeq.Join(sprite.DOLocalMoveY(-.4f, .25f).SetEase(Ease.OutSine));
            flapSeq.Play();

            pigMatHandler.Damage(1);
            return;

        }
        // Check for the first contact point
        if (other.contacts.Length > 0)
        {
            ContactPoint2D contact = other.contacts[0];

            // The normal vector points directly away from the surface of collision
            Vector2 forceDirection = contact.normal;

            // Apply a force in the opposite direction of the collision
            rb.AddForce(forceDirection * bounceForceMagnitude, ForceMode2D.Impulse);
            moveFreeDuration = moveFreeDurationBounce;
            inverseDirection = Vector2.zero;
            moveFree = true;


        }
    }


    public void OnEgged()
    {
        blindDurationVar = blindedTime;

        if (blinded)
        {
            moveFreeDuration = moveFreeDurationEgg + .2f;
            // rb.velocity = Vector2.zero;
            rb.velocity *= .4f;
            rb.AddForce(Vector2.down * eggForceMagnitude * 1.25f, ForceMode2D.Impulse);
        }
        else
        {
            rb.velocity *= .3f;
            moveFreeDuration = moveFreeDurationEgg;
            rb.AddForce(Vector2.down * eggForceMagnitude, ForceMode2D.Impulse);
        }

        inverseDirection = Vector2.zero;
        moveFree = true;
        if (blinded && !eggYolkUsed)
        {
            StopCoroutine(EggYolkRoutine);
            EggYolkRoutine = StartCoroutine(Egged());

        }
        else if (!blinded)
        {
            blinded = true;
            EggYolkRoutine = StartCoroutine(Egged());
        }



    }


}