using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PorkChop : SpawnedPigBossObject
{
    [SerializeField] private Transform pupil;
    private Vector3 flippedScale;
    private Vector3 normalScale;
    private Sequence flapMovement;
    [SerializeField] private SpriteRenderer glow;
    private float gravity;

    [SerializeField] private Color glowColorAttack;
    [SerializeField] private Color glowColorDefend;

    [SerializeField] private Transform playerTran;
    [SerializeField] private Vector2 playerOffset;
    private bool attacking;
    private bool hasAttacked;
    private bool _canAttack = true;

    private bool defendingUp;
    [SerializeField] private Transform playerDistanceCheck;

    [SerializeField] private Transform arms;
    [SerializeField] private Transform ears;
    [SerializeField] private Transform legs;
    [SerializeField] private Transform body;

    [SerializeField] private float bodyMoveAmount;
    [SerializeField] private float ligamentMoveAmount;
    [SerializeField] private float earRotateAmount;
    [SerializeField] private float armRotateAmount;
    [SerializeField] private float legRotateAmount;
    private Vector2 earsStartPos;
    private Vector2 legsStartPos;
    private Vector2 armsStartPos;

    private Vector2 currentJumpTarget;
    [SerializeField] private float xJumpTargetOffset;
    [SerializeField] private float yJumpTargetOffset;

    [SerializeField] private float upMoveDur;
    [SerializeField] private float downMoveDur;
    [SerializeField] private Vector2 force;
    private Rigidbody2D rb;

    [SerializeField] private Vector2 xRange;
    [SerializeField] private Vector2 yRange;
    private Vector2 lastTarget;

    private bool switchingDef;
    private float originalGravityScale;


    private bool isFlipped = false;
    private Animator anim;

    private float attackCooldown;
    private EnemyPositioningLogic positionLogic;
    [SerializeField] private Transform heartParent;

    private float heartScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        float s = transform.localScale.x;
        flippedScale = new Vector3(-s, s, s);
        normalScale = new Vector3(s, s, s);
        gravity = Physics2D.gravity.y * rb.gravityScale;


        earsStartPos = ears.localPosition;
        legsStartPos = legs.localPosition;
        armsStartPos = arms.localPosition;
    }

    private void Start()
    {
        positionLogic = GetComponent<EnemyPositioningLogic>();
        heartScale = heartParent.localScale.y;
        forceDir = normForce;
        SwitchGlowColor(false);
        originalGravityScale = rb.gravityScale;
        anim.SetBool("DefUp", true);
        flapTimeVar = normalFlapTime;
        currentJumpTarget = rightSideTarget;
        defendingUp = true;

    }
    private bool switchingSides;
    private bool onRightSide = true;
    [SerializeField] private float switchSideTime;
    [SerializeField] private float switchAnimRatio;
    [SerializeField] private Vector2 rightSideTarget;
    [SerializeField] private Vector2 leftSideTarget;

    [SerializeField] private Vector2 switchSidesForce;

    private IEnumerator SwitchSides()
    {
        switchingSides = true;

        int dir = -1;

        if (isFlipped) dir = 1;

        bool goRight = isFlipped;
        switchingDef = true;
        flapTimeVar = .55f;
        bool switchedAnim = false;
        float t = 0;
        Vector2 target = rightSideTarget;
        Vector2 lastPos = currentJumpTarget;
        anim.SetBool("DefDown", false);
        anim.SetBool("DefUp", true);
        if (onRightSide)
        {

            target = leftSideTarget;
        }



        while (t < switchSideTime)
        {

            t += Time.deltaTime;
            rb.AddForce(switchSidesForce * dir);

            if (!switchedAnim && t >= switchSideTime * switchAnimRatio)
            {
                switchedAnim = true;
                anim.SetTrigger("Flip");

            }

            // currentJumpTarget = Vector2.Lerp(lastPos, target, t / switchSideTime);

            yield return null;

        }
        flapTimeVar = normalFlapTime;
        switchingSides = false;
        onRightSide = !onRightSide;
        switchingDef = false;


    }
    private Vector2 flippedDirection = new Vector2(-1, 1);
    private void MoveEyesWithTicker()
    {
        if (playerTran != null)
        {
            Vector2 direction = playerTran.position - pupil.position; // Calculate the direction to the playerTran
            // Ensure it's 2D
            direction.Normalize(); // Normalize the direction

            if (isFlipped)
            {
                direction *= flippedDirection;
            }

            // Move the pupil within the eye's radius
            pupil.localPosition = direction * .04f;
        }

    }



    [SerializeField] private float reachTargetTime;
    public void FlapUp()
    {

        if (!hasAttacked)
        {
            if (attacking)
            {
                lastTarget = (Vector2)playerTran.position + playerOffset;

                // SetVelocityToReachPosition(lastTarget, reachTargetTime);

                // StartCoroutine(TestC());
                attacking = false;

            }
            // float ranX = 0;
            // float ranY = 0;

            // if (!switchingSides)
            // {
            //     ranX = Random.Range(-xJumpTargetOffset, xJumpTargetOffset);
            //     ranY = Random.Range(-yJumpTargetOffset, yJumpTargetOffset);
            // }



            // Vector2 r = new Vector2(ranX + currentJumpTarget.x, ranY + currentJumpTarget.y);

            // SetVelocityToReachPosition(r, .8f);



        }

        if (flapMovement != null && flapMovement.IsPlaying())
            flapMovement.Kill();

        flapMovement = DOTween.Sequence();

        // flapMovement.Append(body.DOLocalMoveY(bodyMoveAmount, upMoveDur));
        flapMovement.Append(ears.DOLocalRotate(Vector3.forward * earRotateAmount, upMoveDur));
        flapMovement.Join(legs.DOLocalRotate(Vector3.forward * legRotateAmount, upMoveDur));
        flapMovement.Join(arms.DOLocalRotate(Vector3.forward * armRotateAmount, upMoveDur));
        flapMovement.Join(arms.DOLocalMoveY(armsStartPos.y - ligamentMoveAmount, upMoveDur));
        flapMovement.Join(legs.DOLocalMoveY(legsStartPos.y - ligamentMoveAmount, upMoveDur));

        flapMovement.Play().SetEase(Ease.OutSine);

    }

    public void FlapDown()
    {
        addedForce = false;
        if (flapMovement != null && flapMovement.IsPlaying())
            flapMovement.Kill();

        flapMovement = DOTween.Sequence();

        // flapMovement.Append(body.DOLocalMoveY(0, downMoveDur));
        flapMovement.Append(ears.DOLocalRotate(Vector3.zero, downMoveDur));
        flapMovement.Join(legs.DOLocalRotate(Vector3.zero, downMoveDur));
        flapMovement.Join(arms.DOLocalRotate(Vector3.zero, downMoveDur));
        flapMovement.Join(arms.DOLocalMoveY(armsStartPos.y, downMoveDur));
        flapMovement.Join(legs.DOLocalMoveY(legsStartPos.y, downMoveDur));

        flapMovement.Play().SetEase(Ease.InSine);

    }

    public void Flap()
    {
        anim.SetTrigger("Flap");
        // if (flapMovement != null && flapMovement.IsPlaying())
        //     flapMovement.Kill();

        // flapMovement = DOTween.Sequence();
        // if (started)
        // {
        //     anim.SetTrigger("Flap");
        //     flapMovement.Append(body.DOLocalMoveY(bodyMoveAmount, upMoveDur));
        //     flapMovement.Join(ears.DOLocalRotate(Vector3.forward * earRotateAmount, upMoveDur));
        //     flapMovement.Join(legs.DOLocalRotate(Vector3.forward * legRotateAmount, upMoveDur));
        //     flapMovement.Join(arms.DOLocalRotate(Vector3.forward * armRotateAmount, upMoveDur));
        //     flapMovement.Join(arms.DOLocalMoveY(armsStartPos.y - ligamentMoveAmount, upMoveDur));
        //     flapMovement.Join(legs.DOLocalMoveY(legsStartPos.y - ligamentMoveAmount, upMoveDur));

        // }
        // else
        // {
        //     flapMovement.Append(body.DOLocalMoveY(0, downMoveDur));
        //     flapMovement.Join(ears.DOLocalRotate(Vector3.zero, downMoveDur));
        //     flapMovement.Join(legs.DOLocalRotate(Vector3.zero, downMoveDur));
        //     flapMovement.Join(arms.DOLocalRotate(Vector3.zero, downMoveDur));
        //     flapMovement.Join(arms.DOLocalMoveY(armsStartPos.y, downMoveDur));
        //     flapMovement.Join(legs.DOLocalMoveY(legsStartPos.y, downMoveDur));

        // }
        // flapMovement.Play();

    }
    [SerializeField] private float attackRange;
    [SerializeField] private float dampRange;
    [SerializeField] private float dampAmount;
    [SerializeField] private float minSwitchDefDistance;
    [SerializeField] private float minSwitchSideDistance;
    [SerializeField] private float minSwitchDefTime;
    private bool reachedTarget;

    private void ResetSwitchingDef()
    {
        switchingDef = false;
    }

    [Header("Jumping")]
    private float targetY = .2f;
    private float yTarget = .2f;
    private bool addedForce;
    private bool goingDown;
    [SerializeField] private float maxYForce;
    [SerializeField] private Vector2 addedYForce;
    private bool addUpForce;
    [SerializeField] private float isGoingDownForce;

    [SerializeField] private Vector2 yJumpForceRange;

    [SerializeField] private float jumpForce;

    private float flapTimer;
    [SerializeField] private float normalFlapTime = .82f;

    [SerializeField] private float bobSpeed = 2f; // Speed of the bobbing
    [SerializeField] private float bobAmount = 0.5f; // How much it moves up/down

    private float bobTime;
    private void FixedUpdate()
    {
        // bobTime += Time.fixedDeltaTime * bobSpeed; // Increases over time

        // float yOffset = Mathf.Sin(bobTime) * bobAmount; // Calculate bobbing offset

        // rb.MovePosition(rb.position + new Vector2(0, yOffset) * Time.fixedDeltaTime);

        // if (rb.position.y < yTarget && !addedForce)
        // {
        //     Flap(true);
        //     addedForce = true;
        //     addUpForce = true;

        //     // rb.linearVelocity = new Vector2(rb.linearVelocityX, Random.Range(yJumpForceRange.x, yJumpForceRange.y));
        //     yTarget = targetY + Random.Range(-.4f, .3f);


        // }

        // if (addUpForce)
        // {
        //     rb.AddForce(addedYForce);

        //     if (rb.linearVelocityY >= maxYForce)
        //     {
        //         addUpForce = false;
        //         goingDown = false;
        //     }
        // }

        // else if (!goingDown && rb.linearVelocityY < isGoingDownForce)
        // {
        //     Flap(false);
        //     goingDown = true;
        //     addedForce = false;
        // }

    }
    private bool doFlap = true;
    private float flapTimeVar;

    private bool waitForYTarget;
    private float yTargetForAttack;

    // Update is called once per frame
    void Update()
    {




        if (!switchingSides && !doingAttack)
        {
            if (onRightSide && transform.position.x < playerTran.position.x - minSwitchSideDistance)
            {
                // Flap();
                flapTimer = 0;
                switchingSides = true;
                StartCoroutine(SwitchSides());
            }
            else if (!onRightSide && transform.position.x > playerTran.position.x + minSwitchSideDistance)
            {
                // Flap();
                flapTimer = 0;
                switchingSides = true;
                StartCoroutine(SwitchSides());
            }
        }

        // if (doFlap)
        // {
        //     flapTimer += Time.deltaTime;
        //     if (flapTimer >= flapTimeVar)
        //     {
        //         Flap();
        //         flapTimer = 0;
        //     }
        // }
        if (!switchingDef && !switchingSides)
        {
            if (!defendingUp && transform.position.y < playerTran.position.y - minSwitchDefDistance && Mathf.Abs(transform.position.x - playerTran.position.x) > 4.2f)
            {
                anim.SetBool("DefDown", false);
                anim.SetBool("DefUp", true);
                defendingUp = true;
                switchingDef = true;
                Invoke("ResetSwitchingDef", minSwitchDefTime);
            }
            else if (transform.position.y > playerTran.position.y + minSwitchDefDistance && defendingUp && Mathf.Abs(transform.position.x - playerTran.position.x) > 4.2f)
            {
                anim.SetBool("DefUp", false);
                anim.SetBool("DefDown", true);
                defendingUp = false;
                switchingDef = true;
                Invoke("ResetSwitchingDef", minSwitchDefTime);
            }

        }

        if (waitForYTarget && !hasAttacked)
        {
            // if (transform.position.y < yTargetForAttack)
            // {
            //     hasAttacked = true;
            //     waitForYTarget = false;
            //     anim.SetTrigger("SwingUp");
            //     rb.gravityScale = 0;
            //     rb.linearVelocity = new Vector2(.5f, 0);



            // }
            hasAttacked = true;
            waitForYTarget = false;
            anim.SetTrigger("SwingUp");
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(.5f, 0);

        }

        //attack logic

        if (!doingAttack && !switchingSides && Vector2.Distance(playerDistanceCheck.position, playerTran.position) < attackRange && _canAttack)
        {
            SwitchGlowColor(true);

            if (defendingUp)
            {
                anim.SetBool("QuickFlap", true);
                _canAttack = false;
                reachedTarget = false;
                anim.SetTrigger("SwingDown");


                attacking = true;
            }
            else
            {
                doFlap = false;
                yTargetForAttack = playerTran.position.y;
                waitForYTarget = true;
                _canAttack = false;
                // StartCoroutine(TestC());
            }


        }

        if (attacking && hasAttacked && !reachedTarget)
        {
            reachedTarget = true;

            if (Vector2.Distance(transform.position, lastTarget) < dampRange)
            {
                rb.linearDamping = dampAmount;

                // attacking = false;

            }
        }

        if (addingAttackForce && Vector2.Distance(playerTran.position, transform.position) < 4.1f)
        {
            rb.linearVelocity *= .3f;
            addingAttackForce = false;
        }

    }


    public void SetVelocityToReachPosition(Vector2 targetPosition, float time)
    {
        if (time <= 0)
        {
            Debug.LogError("Time must be greater than zero!");
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 displacement = targetPosition - currentPosition;

        // Gravity (y-axis acceleration)


        // Calculate required velocity
        float velocityX = displacement.x / time; // v = d / t (No acceleration on X)
        float velocityY = (displacement.y - 0.5f * gravity * time * time) / time; // v = (d - 0.5 * a * t²) / t

        // Apply the velocity directly
        rb.linearVelocity = new Vector2(velocityX, velocityY);
    }

    private Sequence changeGlowSeq;

    public void StartAttack(int type)
    {
        doingAttack = true;
        Debug.Log("Start Attack");

        // rb.constraints = RigidbodyConstraints2D.FreezePosition;
        positionLogic.SetDoMovement(false);
        rb.linearVelocity = Vector2.zero;


        // SwitchGlowColor(true);
    }

    [SerializeField] private Vector2 swingUpSideForce;
    [SerializeField] private float swingUpDamp;
    [SerializeField] private Vector2 swingUpUpForce;
    [SerializeField] private Vector2 swingDownSideForce;
    [SerializeField] private Vector2 swingDownDownForce;
    private Vector2 flipForce = new Vector2(-1, 1);
    private Vector2 normForce = new Vector2(1, 1);
    private Vector2 forceDir;

    private bool addingAttackForce = false;

    // private IEnumerator SlerpToPosition(float localEnd, float radius)
    // {
    //     Vector2 localStart = rb.position;
    // }

    private IEnumerator ParriedForce()
    {
        SwitchGlowColor(false);
        yield return new WaitForSeconds(1.4f);
        positionLogic.SetDoMovement(true);
        anim.SetBool("QuickFlap", false);
        StartCoroutine(WaitForNextAttack(3.5f));
        doingAttack = false;
    }
    [SerializeField] private float parriedForce;
    public void AddAttackForce(int type)
    {
        if (type == 5)
        {
            Debug.Log("PARY UYUG");
            Vector2 directionToObject = ((Vector2)transform.position - (Vector2)playerTran.position).normalized;
            rb.linearDamping = .4f;

            rb.linearVelocity = directionToObject * parriedForce;
            StartCoroutine(ParriedForce());
        }
        else
        {
            Vector2 f = Vector2.zero;
            int dir = isFlipped ? 1 : -1; // Set direction (1 = Right, -1 = Left)

            float xDist = Mathf.Abs(transform.position.x - playerTran.position.x); // Distance to player
            float yDist = playerTran.position.y - transform.position.y;

            // If distance is greater than 2, apply lerped force
            if (xDist > 2f)
            {
                // Lerp force between 2 and 6 based on distance (clamped to max 6)
                float forceX = Mathf.Lerp(1f, 6f, Mathf.InverseLerp(2f, 8f, xDist));

                // Apply force in the correct direction
                f.x = forceX * dir;
            }

            if (yDist > 3.5f) f.y = 2f;
            else if (yDist < -3.5f) f.y = -2f;

            rb.AddForce(f, ForceMode2D.Impulse);




        }
        // if (type == 1)
        // {
        //     rb.linearDamping = swingUpDamp;
        //     rb.linearVelocity = swingUpSideForce * forceDir;
        //     addingAttackForce = true;

        // }
        // else if (type == -1)
        // {
        //     addingAttackForce = false;

        //     rb.linearVelocity = swingUpUpForce * forceDir;
        // }
        // else if (type == 2)
        // {
        //     rb.linearVelocity = swingDownSideForce * forceDir;

        // }
        // else if (type == -2)
        // {
        //     rb.linearVelocity = swingDownDownForce * forceDir;

        // }
    }
    private IEnumerator WaitForNextAttack(float dur)
    {
        positionLogic.SetAvoidPlayer(true);

        yield return new WaitForSeconds(1f);

        positionLogic.SetAvoidPlayer(false);
        yield return new WaitForSeconds(dur);


        hasAttacked = false;
        _canAttack = true;
    }
    private bool doingAttack = false;
    public void StopAttack(int type)
    {
        SwitchGlowColor(false);
        doingAttack = false;
        Debug.Log("Stopping Attack");
        // rb.constraints = RigidbodyConstraints2D.None;
        positionLogic.SetDoMovement(true);
        anim.SetBool("QuickFlap", false);



        // if (type == 1)
        // {
        //     rb.linearDamping = 0;
        //     hasAttacked = false;
        //     currentJumpTarget = transform.position;
        //     flapTimer = 0;
        //     doFlap = true;
        //     rb.gravityScale = originalGravityScale;
        //     FlapUp();

        // }
        StartCoroutine(WaitForNextAttack(3.5f));
    }

    private IEnumerator TestC()
    {


        yield return new WaitForSeconds(.2f);
        rb.linearDamping = 0;

        yield return new WaitForSeconds(.25f);

        attacking = false;
        hasAttacked = false;
        doFlap = true;
        rb.gravityScale = originalGravityScale;


        yield return new WaitForSeconds(3f);
        _canAttack = true;




    }

    private void SwitchGlowColor(bool attack)
    {


        if (changeGlowSeq != null && changeGlowSeq.IsPlaying())
            changeGlowSeq.Kill();

        changeGlowSeq = DOTween.Sequence();
        if (attack)
        {
            changeGlowSeq.Append(glow.DOColor(glowColorAttack, .25f));

        }
        else
        {
            changeGlowSeq.Append(glow.DOColor(glowColorDefend, .3f));

        }
    }


    public void Flip()
    {
        if (isFlipped)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            transform.localScale = normalScale;
            forceDir = normForce;
            isFlipped = false;
            heartParent.transform.localScale = Vector3.one * heartScale;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.None;
            transform.localScale = flippedScale;
            forceDir = flipForce;
            isFlipped = true;
            heartParent.transform.localScale = new Vector3(-heartScale, heartScale, heartScale);

        }
        anim.SetTrigger("Reverse");


    }

    public override void OnEnableLogic()
    {
        Ticker.OnTickAction015 += MoveEyesWithTicker;
    }
    private void OnDisable()
    {
        Ticker.OnTickAction015 -= MoveEyesWithTicker;
    }
}
