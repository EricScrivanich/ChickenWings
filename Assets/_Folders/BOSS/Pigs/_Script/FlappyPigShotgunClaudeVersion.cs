using UnityEngine;
using System.Collections;

public class FlappyPigShotgunClaudeVersion : MonoBehaviour, IEnemySubType
{
    private FlappyPigMovement movementScript;

    [Header("References")]
    [SerializeField] private QPool shotgunBlasts;
    [SerializeField] private Transform bulletSpawnRight;
    [SerializeField] private Transform bulletSpawnLeft;
    [SerializeField] private SpriteRenderer leftShotgunAimSprite;
    [SerializeField] private SpriteRenderer rightShotgunAimSprite;
    [SerializeField] private PlayerID player;
    [SerializeField] private AimRotationTransform shotgunAimRight;
    [SerializeField] private AimRotationTransform shotgunAimLeft;
    [SerializeField] private Transform armLeft;
    [SerializeField] private Transform armRight;
    [SerializeField] private AimRotationTransform aimRotationTransformRightArm;
    [SerializeField] private AimRotationTransform aimRotationTransformLeftArm;
    [SerializeField] private Transform body;

    [Header("Base Stats")]
    [SerializeField] private float baseShootDelay = 1.8f;
    [SerializeField] private float shootDistance = 8f;
    [SerializeField] private float recoilForce = 6f;
    [SerializeField] private float aimDelay = 0.6f;
    [SerializeField] private float maxRotation;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float smoothSpeed = 0.3f;

    private Transform playerTarget;
    private Rigidbody2D rb;
    private float shootTimer = 0;
    private bool canShoot = true;
    private bool doTimer = false;
    private bool moveToPos = false;
    private bool doingLogic = false;
    private Vector2 speedRef;
    private Vector2 currentOffset;
    private Transform currentArm;
    private Coroutine shootCoroutine;
    private Coroutine behaviorCoroutine;

    // Dynamic AI State
    private enum AttackPattern { Standard, Aggressive, Flanking, Retreating, Feint }
    private AttackPattern currentPattern = AttackPattern.Standard;

    private enum TacticalState { Engaging, Repositioning, Waiting, Pursuing }
    private TacticalState tacticalState = TacticalState.Engaging;

    // Aggression & Adaptation
    private float aggression = 0.5f; // 0 = passive, 1 = very aggressive
    private int consecutiveHits = 0;
    private int consecutiveMisses = 0;
    private float playerDodgeRate = 0f; // Tracks how often player dodges
    private float lastPlayerY;
    private int playerVerticalChanges = 0;

    // Pattern timing
    private float patternDuration = 0f;
    private float patternTimer = 0f;
    private float currentShootDelay;

    // Feint system
    private bool isFeinting = false;
    private int feintCount = 0;
    private const int maxFeintsBeforeShot = 2;

    // Positional memory
    private Vector2 lastShotPosition;
    private float timeSinceLastShot = 0f;
    private float preferredEngagementRange = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<FlappyPigMovement>();
        shotgunBlasts.Initialize();
        playerTarget = player._transform;

        aimRotationTransformRightArm.SetConstantRotation(0, playerTarget);
        aimRotationTransformLeftArm.SetConstantRotation(0, playerTarget);
        aimRotationTransformRightArm.enabled = false;
        aimRotationTransformLeftArm.enabled = false;

        currentShootDelay = baseShootDelay;
        lastPlayerY = playerTarget.position.y;

        // Start behavior loop
        behaviorCoroutine = StartCoroutine(BehaviorUpdateLoop());
    }

    void FixedUpdate()
    {
        if (moveToPos)
        {
            ExecuteMovement();
        }

        UpdateShootLogic();
        TrackPlayerBehavior();
    }

    private void ExecuteMovement()
    {
        Vector2 targetPos = (Vector2)playerTarget.position + currentOffset;
        targetPos.y = Mathf.Max(targetPos.y, BoundariesManager.GroundPosition + 0.5f);

        // Add slight prediction based on player velocity
        if (tacticalState == TacticalState.Pursuing)
        {
            Vector2 playerVel = player.playerRB.linearVelocity;
            targetPos += playerVel * 0.15f;
        }

        Vector2.SmoothDamp(transform.position, targetPos, ref speedRef, smoothSpeed, maxSpeed);
        rb.linearVelocity = speedRef;
    }

    private void UpdateShootLogic()
    {
        if (canShoot)
        {
            float distToPlayer = Vector2.Distance(playerTarget.position, transform.position);

            if (distToPlayer < shootDistance && ShouldAttemptShot())
            {
                canShoot = false;
                shootTimer = 0;
                doTimer = false;

                bool flipped = playerTarget.position.x < transform.position.x;
                shootCoroutine = StartCoroutine(ExecuteAttackPattern(flipped));
            }
        }
        else if (doTimer)
        {
            shootTimer += Time.fixedDeltaTime;
            if (shootTimer > currentShootDelay)
            {
                canShoot = true;
                SelectNewPattern();
            }
        }

        timeSinceLastShot += Time.fixedDeltaTime;
    }

    private void TrackPlayerBehavior()
    {
        // Track vertical movement patterns
        float currentY = playerTarget.position.y;
        if (Mathf.Abs(currentY - lastPlayerY) > 0.5f)
        {
            playerVerticalChanges++;
        }
        lastPlayerY = currentY;
    }

    private IEnumerator BehaviorUpdateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            // Adapt aggression based on fight state
            AdaptAggression();

            // Update tactical state
            UpdateTacticalState();

            // Analyze player patterns
            AnalyzePlayerPatterns();

            // Reset pattern tracking
            playerVerticalChanges = 0;
        }
    }

    private void AdaptAggression()
    {
        // Increase aggression if landing hits
        if (consecutiveHits > 2)
        {
            aggression = Mathf.Min(1f, aggression + 0.1f);
            currentShootDelay = Mathf.Max(0.8f, baseShootDelay * (1f - aggression * 0.4f));
        }
        // Decrease if missing
        else if (consecutiveMisses > 3)
        {
            aggression = Mathf.Max(0.2f, aggression - 0.15f);
            currentShootDelay = baseShootDelay * (1f + (1f - aggression) * 0.3f);
        }

        // Adjust preferred range based on success
        if (consecutiveHits > 1)
            preferredEngagementRange = Mathf.Max(3f, preferredEngagementRange - 0.5f);
        else if (consecutiveMisses > 2)
            preferredEngagementRange = Mathf.Min(7f, preferredEngagementRange + 0.5f);
    }

    private void UpdateTacticalState()
    {
        float distToPlayer = Vector2.Distance(playerTarget.position, transform.position);

        if (distToPlayer > shootDistance * 1.2f)
        {
            tacticalState = TacticalState.Pursuing;
        }
        else if (distToPlayer < preferredEngagementRange * 0.6f)
        {
            tacticalState = TacticalState.Repositioning;
        }
        else if (timeSinceLastShot > 4f)
        {
            tacticalState = TacticalState.Engaging;
        }
        else
        {
            tacticalState = TacticalState.Waiting;
        }
    }

    private void AnalyzePlayerPatterns()
    {
        // If player moves vertically a lot, they're likely dodging
        if (playerVerticalChanges > 4)
        {
            playerDodgeRate = Mathf.Min(1f, playerDodgeRate + 0.2f);
        }
        else
        {
            playerDodgeRate = Mathf.Max(0f, playerDodgeRate - 0.1f);
        }
    }

    private bool ShouldAttemptShot()
    {
        // Always shoot if very aggressive
        if (aggression > 0.8f) return true;

        // More likely to shoot if player is relatively still
        if (player.playerRB.linearVelocity.magnitude < 2f) return true;

        // Random chance based on aggression
        return Random.value < aggression;
    }

    private void SelectNewPattern()
    {
        float roll = Random.value;

        // High dodge rate player - use feints more
        if (playerDodgeRate > 0.6f)
        {
            if (roll < 0.4f) currentPattern = AttackPattern.Feint;
            else if (roll < 0.7f) currentPattern = AttackPattern.Flanking;
            else currentPattern = AttackPattern.Standard;
        }
        // Aggressive when doing well
        else if (aggression > 0.7f)
        {
            if (roll < 0.5f) currentPattern = AttackPattern.Aggressive;
            else if (roll < 0.8f) currentPattern = AttackPattern.Flanking;
            else currentPattern = AttackPattern.Standard;
        }
        // Play safer when struggling
        else if (aggression < 0.4f)
        {
            if (roll < 0.4f) currentPattern = AttackPattern.Retreating;
            else if (roll < 0.7f) currentPattern = AttackPattern.Standard;
            else currentPattern = AttackPattern.Feint;
        }
        else
        {
            // Balanced mix
            if (roll < 0.3f) currentPattern = AttackPattern.Standard;
            else if (roll < 0.5f) currentPattern = AttackPattern.Aggressive;
            else if (roll < 0.7f) currentPattern = AttackPattern.Flanking;
            else if (roll < 0.85f) currentPattern = AttackPattern.Retreating;
            else currentPattern = AttackPattern.Feint;
        }

        feintCount = 0;
    }

    private IEnumerator ExecuteAttackPattern(bool flipped)
    {
        switch (currentPattern)
        {
            case AttackPattern.Standard:
                yield return StartCoroutine(StandardShot(flipped));
                break;
            case AttackPattern.Aggressive:
                yield return StartCoroutine(AggressiveShot(flipped));
                break;
            case AttackPattern.Flanking:
                yield return StartCoroutine(FlankingShot(flipped));
                break;
            case AttackPattern.Retreating:
                yield return StartCoroutine(RetreatShot(flipped));
                break;
            case AttackPattern.Feint:
                yield return StartCoroutine(FeintShot(flipped));
                break;
        }
    }

    private IEnumerator StandardShot(bool flipped)
    {
        doingLogic = true;

        // Standard positioning - maintain distance
        CalculateStandardOffset(flipped);

        yield return StartCoroutine(AimAndShoot(flipped, 0.8f, false));

        doingLogic = false;
    }

    private IEnumerator AggressiveShot(bool flipped)
    {
        doingLogic = true;

        // Close distance aggressively
        currentOffset.x = flipped ? 2.5f : -2.5f;
        currentOffset.y = (transform.position.y < playerTarget.position.y) ? -1f : 1.5f;

        // Shorter aim time for pressure
        yield return StartCoroutine(AimAndShoot(flipped, 0.5f, false));

        // Quick follow-up chance
        if (aggression > 0.7f && Random.value < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
            flipped = playerTarget.position.x < transform.position.x;
            yield return StartCoroutine(AimAndShoot(flipped, 0.4f, false));
        }

        doingLogic = false;
    }

    private IEnumerator FlankingShot(bool flipped)
    {
        doingLogic = true;

        // Move to opposite vertical position
        bool goHigh = transform.position.y < playerTarget.position.y;
        currentOffset.x = flipped ? 3.5f : -3.5f;
        currentOffset.y = goHigh ? 3f : -2.5f;

        speedRef = rb.linearVelocity;
        moveToPos = true;

        // Wait while repositioning
        yield return new WaitForSeconds(0.4f);

        // Re-evaluate flip after movement
        flipped = playerTarget.position.x < transform.position.x;

        yield return StartCoroutine(AimAndShoot(flipped, 0.7f, true));

        doingLogic = false;
    }

    private IEnumerator RetreatShot(bool flipped)
    {
        doingLogic = true;

        // Create distance
        currentOffset.x = flipped ? 5f : -5f;
        currentOffset.y = Random.Range(-1f, 2f);

        speedRef = rb.linearVelocity;
        moveToPos = true;

        yield return new WaitForSeconds(0.3f);

        // Longer, more careful aim
        yield return StartCoroutine(AimAndShoot(flipped, 1f, false));

        doingLogic = false;
    }

    private IEnumerator FeintShot(bool flipped)
    {
        doingLogic = true;

        // Show aim indicator but don't shoot
        SpriteRenderer sr = flipped ? leftShotgunAimSprite : rightShotgunAimSprite;

        CalculateStandardOffset(flipped);
        speedRef = rb.linearVelocity;
        moveToPos = true;

        // Fake aim
        sr.color = new Color(1, 0.29f, 0.8f, 0);
        sr.transform.localScale = Vector3.one;
        sr.enabled = true;

        float feintDuration = Random.Range(0.3f, 0.5f);
        float t = 0;
        while (t < feintDuration)
        {
            sr.color = new Color(1, 0.29f, 0.8f, t / feintDuration * 0.7f);
            sr.transform.localScale = Vector3.one * Mathf.Lerp(1, 1.4f, t / feintDuration);
            t += Time.deltaTime;
            yield return null;
        }

        // Cancel the feint
        sr.enabled = false;
        moveToPos = false;
        feintCount++;

        // After enough feints, do a real shot
        if (feintCount >= maxFeintsBeforeShot || Random.value < 0.4f)
        {
            yield return new WaitForSeconds(Random.Range(0.15f, 0.3f));
            flipped = playerTarget.position.x < transform.position.x;

            // Real shot with shorter wind-up to catch player off guard
            yield return StartCoroutine(AimAndShoot(flipped, 0.45f, false));
            feintCount = 0;
        }
        else
        {
            // Reset timer faster to feint again
            currentShootDelay = 0.4f;
        }

        doingLogic = false;
    }

    private void CalculateStandardOffset(bool flipped)
    {
        currentOffset.x = flipped ? 4f : -4f;

        if (transform.position.y < playerTarget.position.y)
        {
            currentOffset.y = -2f;
        }
        else
        {
            currentOffset.y = 2f;
        }
    }

    private IEnumerator AimAndShoot(bool flipped, float aimDuration, bool fullRot)
    {
        float t = 0;
        SpriteRenderer sr = flipped ? leftShotgunAimSprite : rightShotgunAimSprite;
        sr.color = new Color(1, 0.29f, 0.8f, 0);
        sr.transform.localScale = Vector3.one;

        int direction = flipped ? 180 : 0;
        float rot = CalculateAimRotation(flipped);

        if (flipped)
            aimRotationTransformLeftArm.enabled = true;
        else
            aimRotationTransformRightArm.enabled = true;

        speedRef = rb.linearVelocity;
        moveToPos = true;
        rb.linearVelocity = Vector2.zero;

        currentArm = flipped ? armLeft : armRight;

        movementScript.SetRotationTargets(aimDuration, rot + direction, 0, 80, flipped, fullRot);
        sr.enabled = true;

        // Aim telegraph with slight tracking
        while (t < aimDuration)
        {
            float alpha = t / aimDuration;
            sr.color = new Color(1, 0.29f, 0.8f, alpha);
            sr.transform.localScale = Vector3.one * Mathf.Lerp(1, 1.8f, alpha);
            t += Time.deltaTime;
            yield return null;
        }

        sr.enabled = false;

        // Fire
        Transform bulletSpawn = flipped ? bulletSpawnLeft : bulletSpawnRight;
        AudioManager.instance.PlayShoutgunNoise(0);
        shotgunBlasts.SpawnTransformOverride(bulletSpawn.position, bulletSpawn.eulerAngles.z, -1);

        moveToPos = false;
        lastShotPosition = transform.position;
        timeSinceLastShot = 0f;

        rb.AddForce(-bulletSpawn.transform.right * recoilForce, ForceMode2D.Impulse);

        if (flipped)
            aimRotationTransformLeftArm.ResetToZero();
        else
            aimRotationTransformRightArm.ResetToZero();

        yield return new WaitForSeconds(0.15f);
        movementScript.RevertToNormalRotation();

        yield return new WaitForSeconds(0.5f);
        doTimer = true;
    }

    private float CalculateAimRotation(bool flipped)
    {
        float rot;

        // Predict player movement for aim
        Vector2 predictedPos = (Vector2)playerTarget.position + player.playerRB.linearVelocity * 0.2f;
        bool aimingUp = transform.position.y < predictedPos.y;

        if (aimingUp)
        {
            rot = flipped ? 60 : 120;
        }
        else
        {
            rot = flipped ? 120 : 60;
        }

        return rot;
    }

    public void RegisterHit()
    {
        consecutiveHits++;
        consecutiveMisses = 0;
    }

    public void RegisterMiss()
    {
        consecutiveMisses++;
        consecutiveHits = 0;
    }

    public void Egged(bool isEgged)
    {
        if (isEgged)
        {
            if (shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
            }

            if (doingLogic)
            {
                movementScript.RevertToNormalRotation();
            }

            moveToPos = false;
            doingLogic = false;
            doTimer = false;
            canShoot = false;
            isFeinting = false;

            leftShotgunAimSprite.enabled = false;
            rightShotgunAimSprite.enabled = false;

            // Getting hit increases aggression slightly
            aggression = Mathf.Min(1f, aggression + 0.05f);
        }
        else
        {
            // Recover faster when aggressive
            float recoveryMult = aggression > 0.6f ? 0.25f : 0.35f;
            if (shootTimer > currentShootDelay) shootTimer = currentShootDelay;
            shootTimer *= recoveryMult;
            doTimer = true;
        }
    }

    private void OnDisable()
    {
        if (behaviorCoroutine != null)
        {
            StopCoroutine(behaviorCoroutine);
        }
    }
}
