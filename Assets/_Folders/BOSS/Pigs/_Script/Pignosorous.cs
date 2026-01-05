using UnityEngine;
using System.Collections;

public class Pignosorous : SpawnedPigBossObject
{

    [SerializeField] private PlayerID player;
    [SerializeField] private GameObject pathPrefab;
    private ObjectPathFollower pathFollower;
    [SerializeField] private AnimationCurveSO phaseDifficultyCurve;
    [SerializeField] private QPool fireballPool;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource loopedAudioSource;
    [SerializeField] private AudioClip rocketThrustSound;
    [SerializeField] private AudioClip rocketPassBySound;
    [SerializeField] private AudioClip fireballEjectSound;

    [SerializeField] private float rocketThrustVolume;
    [SerializeField] private float rocketPassByVolume;
    [SerializeField] private float fireballEjectVolume;

    private Animator anim;

    [SerializeField] private AimRotationTransform leftJetPack;
    [SerializeField] private AimRotationTransform rightJetPack;

    [SerializeField] private Animator leftJetPackAnim;
    [SerializeField] private Animator rightJetPackAnim;
    private Transform playerTarget;

    private bool isFlipped = false;
    [SerializeField] private float addForceTime;
    [SerializeField] private float aimRotationTime;
    [SerializeField] private float addForceAmount;
    [SerializeField] private float initialForceAmount;
    [SerializeField] private float maxMagnitude;
    private WaitForFixedUpdate fixedWait = new WaitForFixedUpdate();

    [SerializeField] private float maxTorque;
    [SerializeField] private float smoothTime;
    [SerializeField] private float switchToFuturePositionTime;

    [SerializeField] private float moveToPositionTime;

    [SerializeField] private Transform fireSprite;
    private AimRotation rotationScript;

    [SerializeField] private bool doChase = true;
    [SerializeField] private bool doRotate = true;
    private int flipValue = 1;
    [SerializeField] private bool doBackAndForth = true;

    [SerializeField] private float fireballVelocityPercent;
    [SerializeField] private float fireballMagPercent;

    [Header("Chase")]

    [SerializeField] private float fireballChaseVel;
    [SerializeField] private float fireballChaseInialVelPercent;
    [SerializeField] private float fireballChaseFloatUpVel;

    [Header("Back and Forth Settings")]
    [SerializeField] private float startBackAndForthY = 2.5f;
    [SerializeField] private float changeBackForthY;
    [SerializeField] private float shootBackForthTime;
    [SerializeField] private float backForthFireballShootVel;
    [SerializeField] private float backForthFireballShootDelay;


    [SerializeField] private int backForthAmount;
    [SerializeField] private float backForthSpeed;


    private int currentBackForthAmount;

    private float currentBackForthY = 2.5f;

    [Header("Fireball Eject Settings")]
    private float ejectSpeed;
    private float currentVelPercent;
    private Vector2 additionalVel;

    private Vector2 maxRotationByY = new Vector2(60, 0);
    private Vector2 yRange = new Vector2(4.5f, -3.1f);

    private int rotateShootAmount;
    private float rotateShootDelay;

    private float[] attackChances;
    private int currentChaseAmount;
    private int minChaseAmount;

    [Header("Chase Phase Changes")]
    [SerializeField] private Vector2 moveToPostionChange;
    [SerializeField] private Vector2 aimAtTargetChange;
    [SerializeField] private Vector2 maxForceChange;
    [SerializeField] private Vector2 attackChanceChase;
    [SerializeField] private Vector2 minChaseAmountChange;

    [Header("BackForth Phase Changes")]
    [SerializeField] private Vector2 shootDelayChangeBackForth;
    [SerializeField] private Vector2 amountChangeBackForth;
    [SerializeField] private Vector2 speedChangeBackForth;
    [SerializeField] private Vector2 attackChanceBackForth;

    [Header("Rotate Around Phase Changes")]
    [SerializeField] private Vector2 shootDelayChangeRotate;
    [SerializeField] private Vector2 shootAmountChangeRotate;
    [SerializeField] private Vector2 attackChanceRotate;
    private PigHeartPositioner heartPositioner;


    void CalculateScreenPercents()
    {

    }

    public void SetPhaseVariables(float p)
    {
        Debug.Log("Setting Phase Varaibles iwth percent: " + p);

        moveToPositionTime = LerpFloatVariable(moveToPostionChange, p);
        aimRotationTime = LerpFloatVariable(aimAtTargetChange, p);
        maxMagnitude = LerpFloatVariable(maxForceChange, p);
        minChaseAmount = Mathf.RoundToInt(LerpFloatVariable(minChaseAmountChange, p));

        backForthFireballShootDelay = LerpFloatVariable(shootDelayChangeBackForth, p);
        backForthSpeed = LerpFloatVariable(speedChangeBackForth, p);
        backForthAmount = Mathf.RoundToInt(LerpFloatVariable(amountChangeBackForth, p));

        rotateShootDelay = LerpFloatVariable(shootDelayChangeRotate, p);
        rotateShootAmount = Mathf.RoundToInt(LerpFloatVariable(shootAmountChangeRotate, p));

        attackChances[0] = LerpFloatVariable(attackChanceChase, p);
        attackChances[1] = LerpFloatVariable(attackChanceBackForth, p) + attackChances[0];
        attackChances[2] = LerpFloatVariable(attackChanceRotate, p) + attackChances[1];





    }

    public void DoNextAttack(int lastType)
    {
        int picked = -1;
        float p = ((float)lives / (float)startingLives);
        Debug.Log("Lives: " + lives + " starting Lives: " + startingLives + " Percent: " + (1 - p));

        SetPhaseVariables(phaseDifficultyCurve.ReturnValue(1 - p));


        if (currentChaseAmount < minChaseAmount)
        {
            picked = 0;
        }
        else
        {
            float val = Random.Range(0, attackChances[attackChances.Length - 1]);
            Debug.Log("Total Chance: " + attackChances[attackChances.Length - 1] + " Random: " + val);
            for (int i = 0; i < attackChances.Length; i++)
            {
                if (val <= attackChances[i])
                {
                    picked = i;
                    break;
                }
            }
            if (picked != 0 && picked == lastType)
            {
                DoNextAttack(lastType);
                return;
            }
        }

        switch (picked)
        {
            case 0:
                if (lastType != 0) currentChaseAmount = 0;
                StartCoroutine(RotateThenMoveNew());
                currentChaseAmount++;
                return;

            case 1:

                currentBackForthAmount = 0;

                rotationScript.SetMaxRotation(360);
                rotationScript.enabled = false;
                StartCoroutine(BackAndForth(true));

                return;

            case 2:
                StartCoroutine(ShootFromPosition());
                return;
        }

        StartCoroutine(RotateThenMoveNew());



    }
    public override void Hit(int type)
    {
        // lives--;
        // if (lives < 0) return;



    }

    private float LerpFloatVariable(Vector2 l, float p, bool isInt = false)
    {


        return Mathf.Lerp(l.x, l.y, p);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (pathFollower == null)
        {
            GameObject pathObj = Instantiate(pathPrefab);
            pathFollower = pathObj.GetComponentInChildren<ObjectPathFollower>();
        }
        heartPositioner = heart.gameObject.GetComponentInParent<PigHeartPositioner>();
        attackChances = new float[3];
        SetPhaseVariables(0);
        lives = startingLives;
        heartPositioner.enabled = false;


        rotationScript = GetComponent<AimRotation>();
        playerTarget = player._transform;
        rotationScript.SetConstantRotation(-1, playerTarget);

        if (transform.position.x < 0)
            FlipPig(true);


        if (doBackAndForth)
        {

            currentBackForthAmount = 0;

            rotationScript.SetMaxRotation(360);
            rotationScript.enabled = false;
            StartCoroutine(BackAndForth(true));
        }

        else if (doChase)
            StartCoroutine(RotateThenMoveNew());
        else if (doRotate)
            StartCoroutine(ShootFromPosition());


        fireSprite.gameObject.SetActive(false);
        // Time.timeScale = .3f;





    }
    void FlipPig(bool flip)
    {
        if (flip) flipValue = -1;
        else flipValue = 1;
        isFlipped = flip;
        heartPositioner.FlipScale(flip);
        rotationScript.SetIsFlipped(flip);
        leftJetPack.SetIsFlipped(flip);
        rightJetPack.SetIsFlipped(flip);

        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);


    }

    [SerializeField] private float idk = 0;
    private Vector2 idk2;

    private int state = 0;


    public void EjectFireball(Vector2 direction, Vector2 pos, float rot)
    {
        Vector2 added = additionalVel;
        if (isFlipped) added.x *= -1;

        if (currentVelPercent != 0)
            added += rb.linearVelocity * currentVelPercent;



        fireballPool.SpawnWithVelocityAndRotation(pos, (direction * ejectSpeed) + added, rot);
        audioSource.PlayOneShot(fireballEjectSound, fireballEjectVolume);

    }

    private void SetFireballEjectSettings(float speed, Vector2 addedVel, float velPercent = 0, float maxRotation = 360)
    {
        ejectSpeed = speed;
        currentVelPercent = velPercent;
        additionalVel = addedVel;
        // leftJetPack.SetMaxRotation(maxRotation);
        // rightJetPack.SetMaxRotation(maxRotation);
    }
    private Vector2 addedBackForthY = new Vector2(0, .2f);
    private IEnumerator BackAndForth(bool firstTime = true)
    {

        currentBackForthY = startBackAndForthY + (changeBackForthY * currentBackForthAmount);
        heartPositioner.enabled = true;

        rb.SetRotation(-.5f * flipValue);






        rb.linearVelocity = Vector2.zero;
        rb.position = new Vector2((BoundariesManager.rightBoundary) * flipValue, currentBackForthY);

        if (firstTime)
        {
            yield return null;
            heartPositioner.enabled = false;
            leftJetPack.SetConstantRotation(0, leftJetPack.transform);
            rightJetPack.SetConstantRotation(0, rightJetPack.transform);
            leftJetPack.SetOffset(1.5f, -1);
            rightJetPack.SetOffset(1, .4f);

            yield return new WaitForSeconds(1f);
            anim.SetTrigger("Accelerate");
        }


        rb.linearVelocity = Vector2.left * flipValue * backForthSpeed + addedBackForthY;
        audioSource.PlayOneShot(rocketPassBySound, rocketPassByVolume);
        WaitForSeconds shootBackForthDelay = new WaitForSeconds(backForthFireballShootDelay);

        yield return fixedWait;
        yield return fixedWait;
        yield return fixedWait;
        ejectSpeed = backForthFireballShootVel;
        // SetFireballEjectSettings(backForthFireballShootVel, Vector2.zero);

        while (Mathf.Abs(rb.position.x) < BoundariesManager.rightBoundary)
        {


            yield return shootBackForthDelay;
            leftJetPackAnim.SetTrigger("EjectSlow");

            // fireballPool.SpawnWithVelocityAndRotation(leftJetPack.transform.position, ((Vector2)(-leftJetPack.transform.up) * backForthFireballShootVel), leftJetPack.transform.eulerAngles.z);
            yield return shootBackForthDelay;
            rightJetPackAnim.SetTrigger("EjectSlow");
            // fireballPool.SpawnWithVelocityAndRotation(rightJetPack.transform.position, ((Vector2)(-rightJetPack.transform.up) * backForthFireballShootVel), rightJetPack.transform.eulerAngles.z);


        }
        yield return fixedWait;
        yield return fixedWait;
        yield return fixedWait;

        FlipPig(!isFlipped);
        currentBackForthAmount++;

        if (currentBackForthAmount < backForthAmount)
        {
            currentBackForthY += changeBackForthY;
            StartCoroutine(BackAndForth(false));
        }
        else
        {
            yield return new WaitForSeconds(.7f);
            rotationScript.enabled = true;
            DoNextAttack(1);
        }








    }




    private IEnumerator RotateThenMoveNew()
    {
        float timer = 0;



        heartPositioner.enabled = true;

        anim.SetTrigger("Idle");
        rb.linearVelocity = Vector2.zero;
        Vector2 p = Vector2.zero;

        p.x = (BoundariesManager.rightBoundary + 1) * flipValue;

        rotationScript.SetConstantRotation(1, playerTarget);

        // yield return new WaitForSeconds(.5f);

        rotationScript.EditRotationValues(1, 1);
        leftJetPack.SetConstantRotation(3, leftJetPack.transform);
        rightJetPack.SetConstantRotation(3, rightJetPack.transform);
        leftJetPack.SetOffset(-.3f, -1);
        rightJetPack.SetOffset(.3f, -1);

        if (state == 0)
            p.y = Mathf.Clamp(transform.position.y, -2.7f, 3.3f);
        else if (state == 1)
            p.y = Mathf.Clamp(transform.position.y, -1.2f, 1.6f);

        rb.position = p;
        p.x -= 6 * flipValue;
        float percent = Mathf.InverseLerp(yRange.x, yRange.y, transform.position.y);
        rotationScript.SetMaxRotation(Mathf.Lerp(maxRotationByY.x, maxRotationByY.y, percent));

        // fireSprite.gameObject.SetActive(false);



        rightJetPack.SetOffset(.3f, -1);
        yield return new WaitForSeconds(.4f);


        while (Mathf.Abs(rb.position.x - p.x) > .35f)
        {
            timer += Time.fixedDeltaTime;
            Vector2 pos = Vector2.SmoothDamp(rb.position, p, ref idk2, moveToPositionTime);
            rb.MovePosition(pos);
            yield return fixedWait;
        }
        timer = 0;


        bool skip = false;

        while (timer < aimRotationTime)
        {
            timer += Time.fixedDeltaTime;

            float timeLeft = aimRotationTime - timer;

            if (!skip)
            {
                rotationScript.SetTargetAngle(playerTarget.position);

                if (timeLeft <= switchToFuturePositionTime)
                {
                    skip = true;
                    audioSource.PlayOneShot(rocketThrustSound, rocketThrustVolume);
                    rightJetPack.SetOffset(1, -.3f);
                    rightJetPack.EditRotationValues(2.5f, .3f);

                    rotationScript.SetTargetAngle(player.ReturnFuturePosition(switchToFuturePositionTime));
                    rotationScript.EditRotationValues(1.5f, .8f);

                }
                yield return fixedWait;
                timer += Time.fixedDeltaTime;
                yield return fixedWait;


            }
            else
            {
                timer += Time.fixedDeltaTime;
                yield return fixedWait;
            }


        }

        timer = 0;
        rightJetPack.EditRotationValues(1, 1);

        yield return fixedWait;
        yield return fixedWait;

        rb.linearVelocity = Vector2.left * flipValue * initialForceAmount;
        rotationScript.EditRotationValues(.7f, .7f);
        rotationScript.SetConstantRotation(0, playerTarget);
        bool hitMaxForce = false;
        if (isFlipped) flipValue = -1;
        Vector3 start = new Vector3(1.25f, .4f, 1);
        Vector3 end = new Vector3(1.5f, 1.55f, 1);
        // fireSprite.localScale = start;
        // fireSprite.gameObject.SetActive(true);
        int frameCount = 6;
        int maxFrames = 12;

        anim.SetTrigger("Accelerate");

        SetFireballEjectSettings(fireballChaseVel, Vector2.up * fireballChaseFloatUpVel, fireballChaseInialVelPercent);
        while (timer < addForceTime)
        {
            timer += Time.fixedDeltaTime;
            float mag = rb.linearVelocity.magnitude;
            if (!hitMaxForce)
            {
                rb.AddRelativeForce(addForceAmount * Vector2.left * flipValue);



                // fireSprite.localScale = Vector3.Lerp(start, end, mag / maxMagnitude);

                if (mag > maxMagnitude)
                {

                    hitMaxForce = true;
                    // rb.linearVelocity = Vector2.left * flip * maxMagnitude;
                }
            }

            // if (frameCount < maxFrames)

            //     frameCount++;
            // else
            // {


            //     // fireballPool.SpawnWithVelocityAndRotation(rightJetPack.transform.position, ((Vector2)(-rightJetPack.transform.up) * fireballChaseVel) + (rb.linearVelocity * fireballChaseInialVelPercent) + (Vector2.up * fireballChaseFloatUpVel), rightJetPack.transform.eulerAngles.z);
            //     rightJetPackAnim.SetTrigger("Eject");
            //     frameCount = 0;
            //     if (maxFrames > 10)
            //         maxFrames--;
            // }





            percent = Mathf.InverseLerp(yRange.x, yRange.y, transform.position.y);
            rotationScript.SetMaxRotation(Mathf.Lerp(maxRotationByY.x, maxRotationByY.y, percent));
            rb.linearVelocity = -transform.right * flipValue * mag;


            yield return fixedWait;
        }
        FlipPig(!isFlipped);
        timer = 0;

        DoNextAttack(0);




        // After rotating, launch forward
        // rb.AddForce(transform.right * addForceAmount, ForceMode2D.Impulse);
    }

    private IEnumerator ShootFromPosition()
    {
        float timer = 0;
        heartPositioner.enabled = true;
        rb.linearVelocity = Vector2.zero;
        pathFollower.InitializePath(flipValue);
        Vector2 p = pathFollower.GetFirstPosition();





        rb.position = new Vector2((BoundariesManager.rightBoundary - .5f) * flipValue, p.y);


        // fireSprite.gameObject.SetActive(false);

        rotationScript.SetMaxRotation(360);
        rotationScript.SetExactAngle(-83, 700, .7f);
        // rightJetPack.SetOffset(-1, 2.8f);
        // leftJetPack.SetOffset(0, -3f);
        leftJetPack.SetOffset(-1, 2.3f);
        rightJetPack.SetOffset(1, -2f);
        anim.SetTrigger("RotateHead");



        leftJetPack.SetConstantRotation(1, pathFollower.targetFollow);
        rightJetPack.SetConstantRotation(1, pathFollower.targetFollow);

        leftJetPack.EditRotationValues(2, .4f);
        rightJetPack.EditRotationValues(2, .4f);



        while (Mathf.Abs(rb.position.x - p.x) > .35f)
        {

            Vector2 pos = Vector2.SmoothDamp(rb.position, p, ref idk2, moveToPositionTime);
            rb.MovePosition(pos);
            yield return fixedWait;
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        // rotationScript.enabled = false;


        float d = rotateShootDelay;
        WaitForSeconds shootDelay = new WaitForSeconds(d);
        pathFollower.EnableAndDoPath((rotateShootAmount * d * 2) + 1.5f, rb, rotationScript);

        // rotater.DoRotation(rb, 4.5f, 90, 0, flipValue);



        // rotationScript.SetExactAngle(0, 100, 3.2f);
        // rotationScript.SetRotateAround(true);
        int count = 0;
        var left = leftJetPack.transform;
        var right = rightJetPack.transform;

        SetFireballEjectSettings(9, Vector2.zero, 0, 30);

        while (count < rotateShootAmount)
        {
            Vector2 directionToTarget = playerTarget.position - transform.position;
            directionToTarget.Normalize();
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

            leftJetPack.RecalculateOffset(angle);
            rightJetPack.RecalculateOffset(angle);
            yield return shootDelay;
            // rb.AddForce(Vector2.up * 5f);
            leftJetPackAnim.SetTrigger("Eject");
            // fireballPool.SpawnWithVelocityAndRotation(left.position, -left.up * 10, left.eulerAngles.z);

            yield return shootDelay;
            // rb.AddForce(Vector2.up * 5f);
            rightJetPackAnim.SetTrigger("Eject");
            //fireballPool.SpawnWithVelocityAndRotation(right.position, -right.up * 10, right.eulerAngles.z);

            count++;
        }
        leftJetPack.EditRotationValues(1, 1);
        rightJetPack.EditRotationValues(1, 1);

        // Vector2 p = Vector2.zero;
        pathFollower.DisablePath();
        FlipPig(!isFlipped);
        // int flip = 1;
        // if (isFlipped) flip = -1;
        // rotationScript.SetRotateAround(false);
        // rotationScript.SetConstantRotation(-1, playerTarget);
        // p.x = BoundariesManager.rightBoundary * flip;

        // anim.SetTrigger("Idle");

        // if (state == 0)
        //     p.y = Mathf.Clamp(transform.position.y, -2.7f, 3.3f);
        // else if (state == 1)
        //     p.y = Mathf.Clamp(transform.position.y, -1.2f, 1.6f);

        // rb.position = p;
        // p.x -= 6 * flip;
        // while (timer < moveToPositionTime)
        // {
        //     timer += Time.fixedDeltaTime;
        //     Vector2 pos = Vector2.SmoothDamp(rb.position, p, ref idk2, .5f, 4);
        //     rb.MovePosition(pos);
        //     yield return fixedWait;
        // }

        rb.bodyType = RigidbodyType2D.Dynamic;

        DoNextAttack(2);

    }





    // private IEnumerator RotateThenMove()
    // {
    //     float timer = 0;
    //     // yield return new WaitForSeconds(.5f);
    //     float o = 180;
    //     float targetAngle = 0;


    //     if (isFlipped) o = 0;

    //     while (timer < aimRotationTime)
    //     {
    //         timer += Time.fixedDeltaTime;
    //         Vector2 directionToPlayer = -transform.position;
    //         float timeLeft = aimRotationTime - timer;
    //         bool skip = false;
    //         if (!skip)
    //         {
    //             directionToPlayer += (Vector2)playerTarget.position;
    //             targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + o;

    //             if (timeLeft > switchToFuturePositionTime)
    //             {
    //                 skip = true;
    //                 directionToPlayer += player.ReturnFuturePosition(timeLeft);

    //             }

    //             // Rigidbody2D.rotation is in degrees
    //         }

    //         float currentAngle = rb.rotation;



    //         // float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle); // Handles wrap-around correctly

    //         // float torque = Mathf.Clamp(angleDifference / 90f, -1f, 1f) * maxTorque;

    //         // float l = Mathf.LerpAngle(currentAngle, targetAngle, 1) * torque;
    //         float angle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref idk, smoothTime, maxTorque);

    //         rb.MoveRotation(angle);

    //         // Apply full torque if angle difference is more than 90 degrees
    //         // if (Mathf.Abs(angleDifference) > 90f)
    //         // {
    //         //     torque = Mathf.Sign(angleDifference) * maxTorque;
    //         // }

    //         // rb.AddTorque(torque, ForceMode2D.Force);
    //         // rb.MoveRotation(targetAngle);




    //     }


    //     timer = 0;
    //     int flip = 1;




    //     while (timer < addForceTime)
    //     {
    //         timer += Time.fixedDeltaTime;
    //         rb.AddRelativeForce(addForceAmount * Vector2.left * flip);
    //         yield return fixedWait;
    //     }
    //     FlipPig(!isFlipped);
    //     timer = 0;
    //     rb.linearVelocity = Vector2.zero;
    //     Vector2 p = new Vector2(BoundariesManager.rightBoundary * -flip, Mathf.Clamp(transform.position.y, -2.7f, 3.3f));
    //     rb.position = p;
    //     p.x -= 6 * -flip;


    //     StartCoroutine(RotateThenMove());

    //     while (timer < moveToPositionTime)
    //     {
    //         timer += Time.fixedDeltaTime;
    //         Vector2 pos = Vector2.SmoothDamp(rb.position, p, ref idk2, .5f, 4);
    //         rb.MovePosition(pos);

    //         yield return fixedWait;
    //     }
    //     rightJetPack.SetOffset(new Vector2(.3f, -1));


    //     // After rotating, launch forward
    //     // rb.AddForce(transform.right * addForceAmount, ForceMode2D.Impulse);
    // }


    // Update is called once per frame

}
