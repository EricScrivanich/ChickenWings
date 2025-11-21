using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlappyPigMovement : SpawnedPigBossObject, IRecordableObject
{
    [SerializeField] private bool isEnemySubType;
    [SerializeField] private bool FallEasy;
    public float scaleFactor;
    private bool hasFullyEntered;
    private Sequence flapSeq;
    private Sequence yolkSeq;

    private Coroutine EggYolkRoutine;

    [SerializeField] private Transform pupils;
    [SerializeField] private Transform pupilStartPositions;

    [SerializeField] private PlayerID player;
    private PigMaterialHandler pigMatHandler;
    [SerializeField] private Transform sprite;
    [SerializeField] private AnimationDataSO animData;
    [SerializeField] private SpriteRenderer yolkSR;

    private Animator anim;
    public float forceAdded;
    private Transform playerTarget;
    private float eyeRadius = .03f;
    [SerializeField] private CircleCollider2D col;
    [SerializeField] private CircleCollider2D colRigid;

    private bool blinded = false;










    // 3.5, 5.5 is base
    [Header("Variables")]


    private bool moveFree;
    private float moveFreeDuration;

    private float OnDirectionTime;

    private bool eggYolkUsed;
    public float switchFromTargetToPlayerRange;

    public float lerpSpeed;
    public float OnDirectionTimeMax;
    public float predictionTime;

    [SerializeField] private float tweenUpAmount;
    [SerializeField] private float tweenUpDuration;
    [SerializeField] private float tweenDownDuration;
    public float maxVel;

    public float cachedMaxVel;




    private float blindDurationVar;
    private float timeScale;



    private float sineTime; // Time to complete one sine wave cycle (used to define frequency)

    private Vector2 inverseDirection;

    private bool waitingOnDelay;

    private bool isTargetReached = true;

    private Vector2 targetPosition;




    private float cackleTime;

    private float delayTime;
    private float delayDuration;
    [Header("Static Variables")]
    [Header("Static Variables")]

    private static readonly Vector3 yolkStartScale = new Vector3(1.3f, 2.8f, 1f);
    private static readonly Vector3 yolkEndScale = new Vector3(2.2f, 2f, 1f);
    private static readonly float yolkStartY = 1.5f;
    private static readonly float yolkEndY = 0.9f;
    private static readonly float yolkDripDuration = 0.5f;
    private static readonly float yolkScaleMultiplier = 1.5f;
    private static readonly float blindedTime = 2.2f;

    private static readonly float addUpForceMinY = -2.9f;
    private static readonly float addUpForceAmountBlinded = 5.2f;
    private static readonly float addUpForceAmount = 3.8f;

    private static readonly float bounceForceMagnitude = 4f;
    private static readonly float eggForceMagnitude = 8.76f;

    private static readonly float moveFreeDurationBounce = 0.3f;
    private float moveFreeDurationEgg = 0.52f;
    private static readonly float moveFreeDurationInverse = 0.7f;

    private static readonly float inverseMagnitude = -6f;

    private static readonly float minYVelocity = -1.5f;
    private static readonly float maxRotation = 25f;

    private static readonly Vector2 minMaxScale = new Vector2(0.7f, 1.1f);
    private static readonly Vector2 minMaxTimeScale = new Vector2(1.1f, 0.9f);
    private static readonly Vector2 minMaxSwitchDirectionRange = new Vector2(1.7f, 2.5f);
    private static readonly Vector2 minMaxTweenYAmount = new Vector2(0.5f, 1.2f);
    private static readonly Vector2 minMaxOnDirectionTimeMax = new Vector2(3.2f, 3.9f);
    private static readonly Vector2 minMaxLookAheadTime = new Vector2(0.7f, 1f);
    private static readonly Vector2 minMaxLerpTowardsPlayerSpeed = new Vector2(1.3f, 2.8f);

    private static readonly Vector2 minMaxMagntiude = new Vector2(10f, 6.5f);
    private static readonly Vector2 minMaxMass = new Vector2(0.59f, 1.05f);
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform armsTransform;
    [SerializeField] private Transform armLeftTransform;
    [SerializeField] private Transform armRightTransform;

    [SerializeField] private Transform wingsTransform;


    private bool turnedAnimation = false;
    private bool stopMovement = false;



    void Start()
    {
        if (GameObject.Find("Player") != null)
            playerTarget = GameObject.Find("Player").GetComponent<Transform>();
        pigMatHandler = GetComponent<PigMaterialHandler>();



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
    private bool setRotationTarget = false;


    public void RevertToNormalRotation()
    {
        anim.SetTrigger("RevertTurn");

        // float xRange = transform.position.x - playerTarget.position.x;

        // float rotTarget = Mathf.Lerp(0, maxRotation, Mathf.Abs(xRange) * .07f);
        // float rotTarget2 = Mathf.Lerp(0, maxRotation, Mathf.Abs(xRange) * .03f);

        // if (xRange < 0)
        //     rotTarget *= -1;


        // headTransform.eulerAngles = Vector3.forward * rotTarget;
        // bodyTransform.eulerAngles = Vector3.forward * rotTarget * 1.08f;
        // armsTransform.localEulerAngles = Vector3.forward * rotTarget * .3f;

        armLeftTransform.DOLocalRotate(Vector3.forward * -40, .3f).SetEase(Ease.OutSine);
        armRightTransform.DOLocalRotate(Vector3.forward * 40, .3f).SetEase(Ease.OutSine);
        bodyTransform.DORotate(Vector3.zero, .4f).SetEase(Ease.OutSine);


        // sprite.DORotate(Vector3.zero, .6f).SetEase(Ease.OutSine).OnComplete(() => setRotationTarget = false);
        headTransform.DORotate(Vector3.zero, .5f).SetEase(Ease.OutSine).OnComplete(FinishRotation);

    }
    public void FinishRotation()
    {
        setRotationTarget = false;

    }

    public void SetRotationTargets(float dur, float mainTarget, float mainArm, float otherArm, bool aimLeft)
    {
        setRotationTarget = true;
        waitingOnDelay = true;
        if (flapSeq != null && flapSeq.IsPlaying())
            flapSeq.Kill();


        if (aimLeft)
        {

            armLeftTransform.DOLocalRotate(Vector3.forward * mainArm, dur).SetEase(Ease.OutSine);
            armRightTransform.DOLocalRotate(Vector3.forward * otherArm, dur).SetEase(Ease.OutSine);
        }
        else
        {
            wingsTransform.localScale = new Vector3(-1, 1, 1);
            armRightTransform.DOLocalRotate(Vector3.forward * mainArm, dur).SetEase(Ease.OutSine);
            armLeftTransform.DOLocalRotate(Vector3.forward * (otherArm + 180), dur).SetEase(Ease.OutSine);
        }


        anim.speed = 1;
        anim.SetTrigger("Turn");
        armsTransform.DOLocalRotate(Vector3.zero, dur * .6f).SetEase(Ease.OutSine);
        sprite.DOLocalMoveY(0, .3f);
        bodyTransform.DORotate(Vector3.forward * mainTarget, dur * .8f, RotateMode.FastBeyond360).SetEase(Ease.OutSine);
        headTransform.DORotate(Vector3.forward * mainTarget, dur * 1.1f, RotateMode.FastBeyond360);





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
        yolkSeq.Append(headTransform.DORotate(Vector3.zero, yolkDripDuration));
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

        yolkSeq.Append(headTransform.DOShakeRotation(.3f, 80, 10, 50));
        yolkSeq.Join(yolkSR.transform.DOScale(yolkEndScale * yolkScaleMultiplier, .3f));
        yolkSeq.Play();




        yolkSR.sprite = animData.sprites[2];
        yield return new WaitForSeconds(.1f);
        yolkSR.sprite = animData.sprites[3];
        yield return new WaitForSeconds(.1f);
        yolkSR.sprite = animData.sprites[4];
        yield return new WaitForSeconds(.1f);
        yolkSR.enabled = false;
        if (isEnemySubType)
        {
            GetComponent<IEnemySubType>().Egged(false);
        }
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
        cachedMaxVel = Mathf.Lerp(minMaxMagntiude.x, minMaxMagntiude.y, percentage);
        maxVel = cachedMaxVel * .75f;

        timeScale = Mathf.Lerp(minMaxTimeScale.x, minMaxTimeScale.y, percentage);
        tweenUpDuration = .3f / timeScale;
        tweenDownDuration = .4f / timeScale;
        cackleTime = tweenUpDuration;
        sineTime = .7f / timeScale;


        // sineAmplitude = Mathf.Lerp(minMaxAmplitude.x, minMaxAmplitude.y, percentage);


    }

    public override void OnEnableLogic()
    {
        // rb.simulated = false;
        // rb.simulated = true;

        blinded = false;
        yolkSR.enabled = false;
        transform.eulerAngles = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        hasFullyEntered = false;

        player.globalEvents.OnPlayerVelocityChange += UpdateTargetPosition;
        Ticker.OnTickAction015 += MoveEyesWithTicker;
        // Debug.LogError($"Pooled Enemy - ScaleFactor: {scaleFactor}, Mass: {rb.mass}, Velocity: {rb.velocity}");
        waitingOnDelay = true;
        col.offset = Vector2.zero;
        if (colRigid != null) colRigid.offset = Vector2.zero;
        sprite.localPosition = Vector2.zero;


        cackleTime = 0;
        anim.speed = 0;

        if (scaleFactor <= 0) scaleFactor = (minMaxScale.x + minMaxScale.y) * .5f;


        float scalePercentage = Mathf.InverseLerp(minMaxScale.x, minMaxScale.y, scaleFactor);


        SetVariablesBasedOnScale(scalePercentage);


        delayDuration = Random.Range(0f, sineTime);
        sprite.localScale = BoundariesManager.vectorThree1;
        rb.simulated = true;
        StartCoroutine(AnimationDelay());

    }

    private void OnDisable()
    {
        Ticker.OnTickAction015 -= MoveEyesWithTicker;

        if (flapSeq != null && flapSeq.IsPlaying())
            flapSeq.Kill();
        if (yolkSeq != null && yolkSeq.IsPlaying())
            yolkSeq.Kill();
        player.globalEvents.OnPlayerVelocityChange -= UpdateTargetPosition;
        triggerWhenDead = false;


    }

    private void TweenPig()
    {
        if (flapSeq != null && flapSeq.IsPlaying())
        {
            flapSeq.Kill();
        }


        flapSeq = DOTween.Sequence();
        flapSeq.Append(sprite.DOLocalMoveY(tweenUpAmount, tweenUpDuration).From(0)).SetEase(Ease.OutSine);
        flapSeq.Append(sprite.DOLocalMoveY(0, tweenDownDuration)).SetEase(Ease.InSine);
        flapSeq.Play().SetLoops(-1);

    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(delayDuration);
        DoFlapAnim();

    }
    public void DoFlapAnim()
    {
        anim.speed = 1 * timeScale;
        sprite.localPosition = Vector3.zero;
        anim.SetTrigger("Flap");

        wingsTransform.localScale = Vector3.one;

        cackleTime = 0;
        // anim.speed = 1;
        TweenPig();
        waitingOnDelay = false;
    }


    void Update()
    {

        // if (waitingOnDelay)
        // {
        //     delayTime += Time.deltaTime;

        //     if (delayTime > delayDuration)
        //     {
        //         waitingOnDelay = false;

        //         anim.speed = 1 * timeScale;
        //         // anim.speed = 1;
        //         TweenPig();
        //         // Time.timeScale = .4f;



        //     }

        //     return;
        // }


        cackleTime += Time.deltaTime;

        if (!waitingOnDelay && cackleTime > sineTime)
        {
            AudioManager.instance.PlayFlappyPigCackleSound();
            cackleTime = 0;
        }



    }

    private void MoveEyesWithTicker()
    {
        if (blinded) return;

        Vector2 direction = playerTarget.position - pupilStartPositions.position; // Calculate the direction to the player

        direction.Normalize(); // Normalize the direction

        // Move the pupil within the eye's radius
        pupils.localPosition = direction * eyeRadius;

    }



    void FixedUpdate()
    {
        if (stopMovement) return;

        if (!blinded && !setRotationTarget)
        {
            float xRange = transform.position.x - playerTarget.position.x;

            float rotTarget = Mathf.Lerp(0, maxRotation, Mathf.Abs(xRange) * 0.07f);


            if (xRange < 0)
                rotTarget *= -1;

            float headZ = Mathf.MoveTowardsAngle(headTransform.eulerAngles.z, rotTarget, 35 * Time.deltaTime);
            float bodyZ = Mathf.MoveTowardsAngle(bodyTransform.eulerAngles.z, rotTarget * 1.08f, 40 * Time.deltaTime);
            float armsZ = Mathf.MoveTowardsAngle(armsTransform.localEulerAngles.z, rotTarget * 0.3f, 50 * Time.deltaTime);

            headTransform.eulerAngles = Vector3.forward * headZ;
            bodyTransform.eulerAngles = Vector3.forward * bodyZ;
            armsTransform.localEulerAngles = Vector3.forward * armsZ;
        }



        col.offset = Vector2.up * sprite.localPosition.y;


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
                if (blinded)
                {
                    rb.linearVelocityY *= .3f;
                    flapSeq = DOTween.Sequence();
                    flapSeq.Append(sprite.DOLocalMoveY(0, tweenUpDuration * 1.2f)).SetEase(Ease.InSine).OnComplete(TweenPig);
                }
                moveFreeDuration = 0;
            }
            else
                return;
        }
        else
            colRigid.offset = Vector2.up * sprite.localPosition.y;

        if (blinded && FallEasy)
        {
            rb.AddForce(Vector2.down * 1.5f);
        }
        else if (blinded && transform.position.y < -2.5)
        {
            if (!FallEasy)
                rb.AddForce(Vector2.up * addUpForceAmountBlinded);

            Debug.LogError("Adding blinded up force of: " + Vector2.up * addUpForceAmountBlinded + " with duration of: " + blindDurationVar * .5f);
            return;

        }
        else if (transform.position.y < addUpForceMinY)
        {
            rb.AddForce(Vector2.up * addUpForceAmount);

        }
        else if (rb.linearVelocityY > 0 && transform.position.y > BoundariesManager.TopViewBoundary)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * .8f);
        }

        OnDirectionTime += Time.fixedDeltaTime;

        if (OnDirectionTime >= OnDirectionTimeMax && rb.linearVelocity.y < minYVelocity)
        {
            moveFreeDuration = moveFreeDurationInverse;

            // targetPosition = targetPosition * inverseMagnitude;
            inverseDirection = rb.linearVelocity.normalized * inverseMagnitude;
            // rb.AddForce(rb.velocity.normalized * inverseMagnitude, ForceMode2D.Impulse);
            isTargetReached = true;
            // rb.AddForce(targetPosition, ForceMode2D.Impulse);
            OnDirectionTime = 0;
            moveFree = true;
            Debug.LogError("Inversing Direction");
            return;

        }

        if (setRotationTarget) return;
        if (!isTargetReached)
        {
            targetPosition = Vector2.MoveTowards(targetPosition, playerTarget.position, lerpSpeed * Time.fixedDeltaTime);

            MoveTowardsTarget(targetPosition);
        }
        else if (playerTarget != null)
        {
            MoveTowardsTarget(playerTarget.position);  // Move directly towards the player
        }

        CheckIfTargetReached();
    }

    private void MoveTowardsTarget(Vector2 target)
    {

        Vector2 direction = (target - rb.position).normalized;
        rb.AddForce(direction * forceAdded);
        if (rb.linearVelocity.magnitude > maxVel)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVel;
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
        if (!isTargetReached || playerTarget == null || setRotationTarget) return;

        if (!hasFullyEntered)
        {
            maxVel = cachedMaxVel;
            hasFullyEntered = true;
        }

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

        if (futurePosition.y < -3.4f)
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

        if (blinded && other.gameObject.CompareTag("Floor"))
        {
            moveFree = false;
            Damage(1, 0, -1);
            stopMovement = false;
            rb.gravityScale = 0;

            if (lives <= 0)
            {

                if (EggYolkRoutine != null)
                    StopCoroutine(EggYolkRoutine);
                if (flapSeq != null && flapSeq.IsPlaying())
                    flapSeq.Kill();
                rb.simulated = false;
                this.enabled = false;
                anim.enabled = false;

                sprite.DOLocalMoveY(-tweenUpAmount * 1.6f, .25f).SetEase(Ease.OutSine);

                // flapSeq = DOTween.Sequence();


                // flapSeq.Append(sprite.DOScale(new Vector3(1.15f, .8f, 1), .25f).SetEase(Ease.OutSine));
                // flapSeq.Join(sprite.DOLocalMoveY(-.4f, .25f).SetEase(Ease.OutSine));
                // flapSeq.Play();


                // pigMatHandler.Damage(1, 0, -1);
                if (other.gameObject.GetComponent<groundExplosion>() != null)
                {
                    other.gameObject.GetComponent<groundExplosion>().SetYolkTransform(yolkSR.transform);
                    yolkSR.enabled = false;
                }
                return;

            }
            else
            {
                float xVal = 0;
                float r = Random.Range(4f, 5.5f);
                if (transform.position.x < -7f) xVal = r;

                else if (transform.position.x > 7f) xVal = -r;

                else xVal = r * (Random.value < .5f ? 1 : -1);


                Vector2 vel = new Vector2(xVal, Random.Range(1.2f, 2.5f));
                rb.AddForce(vel, ForceMode2D.Impulse);
            }

        }
        // Check for the first contact point
        if (other.contacts.Length > 0)
        {

            ContactPoint2D contact = other.contacts[0];

            // The normal vector points directly away from the surface of collision
            Vector2 forceDirection = contact.normal;

            if (other.gameObject.CompareTag("Fireball"))
            {
                rb.AddForce(forceDirection * bounceForceMagnitude * 5, ForceMode2D.Impulse);
                moveFreeDuration = moveFreeDurationBounce * 5;
            }
            else
            {
                rb.AddForce(forceDirection * bounceForceMagnitude, ForceMode2D.Impulse);
                moveFreeDuration = moveFreeDurationBounce;

            }
            inverseDirection = Vector2.zero;
            moveFree = true;

            // Apply a force in the opposite direction of the collision



        }
    }


    public override void OnPigEgged()
    {
        blindDurationVar = blindedTime;
        if (rb.linearVelocityY > 0) rb.linearVelocityY *= .4f;

        if (blinded)
        {
            moveFreeDuration = moveFreeDurationEgg + .14f;
            rb.linearVelocity = Vector2.zero;
            // rb.velocity *= .4f;
            rb.AddForce(Vector2.down * eggForceMagnitude * 1.2f, ForceMode2D.Impulse);
        }
        else
        {
            if (isEnemySubType)
            {
                GetComponent<IEnemySubType>().Egged(true);
            }
            // rb.velocity *= .2f;
            rb.linearVelocity = Vector2.zero;

            moveFreeDuration = moveFreeDurationEgg;
            rb.AddForce(Vector2.down * eggForceMagnitude, ForceMode2D.Impulse);


        }
        if (flapSeq != null && flapSeq.IsPlaying())
            flapSeq.Kill();

        flapSeq = DOTween.Sequence();
        flapSeq.Append(sprite.DOLocalMoveY(-tweenUpAmount * 1.1f, moveFreeDuration * .9f));
        colRigid.offset = Vector2.up * -tweenUpAmount * 1.1f;
        if (FallEasy)
        {
            rb.gravityScale = 1f;
            stopMovement = true;
        }



        inverseDirection = Vector2.up * 1.9f;
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



    public override void ApplyFloatOneData(DataStructFloatOne data)
    {
        transform.position = data.startPos;
        transform.localScale = BoundariesManager.vectorThree1 * data.float1 * .9f;
        scaleFactor = data.float1 - .1f;
        gameObject.SetActive(true);
    }

    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        transform.localScale = BoundariesManager.vectorThree1 * data.float1 * .9f;
    }

    public bool ShowLine()
    {
        return false;
    }

    public float TimeAtCreateObject(int index)
    {
        throw new System.NotImplementedException();
    }

    public Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset)
    {
        return currPos;
    }

    public float ReturnPhaseOffset(float x)
    {
        return 0;
    }
}