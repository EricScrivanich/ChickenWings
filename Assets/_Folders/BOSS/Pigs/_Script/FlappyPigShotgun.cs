using UnityEngine;
using System.Collections;


public class FlappyPigShotgun : MonoBehaviour, IEnemySubType
{
    private FlappyPigMovement movementScript;
    [SerializeField] private QPool shotgunBlasts;
    [SerializeField] private Transform bulletSpawnRight;
    [SerializeField] private Transform bulletSpawnLeft;
    [SerializeField] private SpriteRenderer leftShotgunAimSprite;
    [SerializeField] private SpriteRenderer rightShotgunAimSprite;

    [SerializeField] private float minShootDelay;
    [SerializeField] private PlayerID player;

    [SerializeField] private float shootDistance;
    [SerializeField] private float recoilForce;
    [SerializeField] private float aimDelay;
    private Transform playerTarget;
    private Rigidbody2D rb;
    private float timer = 0;
    private bool canShoot = true;

    [SerializeField] private AimRotationTransform shotgunAimRight;
    [SerializeField] private AimRotationTransform shotgunAimLeft;

    [SerializeField] private Transform armLeft;
    [SerializeField] private Transform armRight;

    [SerializeField] private AimRotationTransform aimRotationTransformRightArm;
    [SerializeField] private AimRotationTransform aimRotationTransformLeftArm;

    private bool aimingWithLeft;

    [SerializeField] private Transform body;


    private Transform currentArm;
    [SerializeField] private float maxRotation;
    [SerializeField] private float moveSSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private Vector2 speedRef;

    [SerializeField] private Vector2 offset;

    [SerializeField] private float additionalAimSpeed;
    private Coroutine shootCoroutine;

    private float currentAngle;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<FlappyPigMovement>();
       

        shotgunBlasts.Initialize();
        playerTarget = player._transform;
        // shotgunAimLeft.SetConstantRotation(1, playerTarget);
        // // shotgunAimLeft.SetIsFlipped(true);
        // shotgunAimLeft.SetMaxRotation(maxRotation);
        // shotgunAimRight.SetConstantRotation(1, playerTarget);
        // shotgunAimRight.SetMaxRotation(maxRotation);
        aimRotationTransformRightArm.SetConstantRotation(0, playerTarget);
        aimRotationTransformLeftArm.SetConstantRotation(0, playerTarget);
        aimRotationTransformRightArm.enabled = false;
        aimRotationTransformLeftArm.enabled = false;

    }

    // Update is called once per frame
    private bool moveToPos = false;
    void FixedUpdate()
    {
        if (moveToPos)
        {
            Vector2 targetPos = (Vector2)(playerTarget.position) + offset;
            if (targetPos.y < BoundariesManager.GroundPosition + .5f) targetPos.y = BoundariesManager.GroundPosition + .5f;
            Vector2.SmoothDamp(transform.position, targetPos, ref speedRef, smoothSpeed, maxSpeed);
            rb.linearVelocity = speedRef;

            // Vector2 direction = (currentArm.position - playerTarget.position).normalized;
            // float trueAngle = rb.rotation;
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // // if (angle < 0) angle += 360;
            // Debug.Log("body andle: " + body.eulerAngles.z + "true andle: " + trueAngle + " Other Angle: " + angle);

            // if (angle < body.eulerAngles.z + trueAngle)
            // {

            //     trueAngle -= additionalAimSpeed * Time.fixedDeltaTime;
            // }
            // else
            // {
            //     trueAngle += additionalAimSpeed * Time.fixedDeltaTime;
            // }
            // rb.MoveRotation(trueAngle);
        }


        if (canShoot)
        {
            if (Vector2.Distance(playerTarget.position, transform.position) < shootDistance)
            {
                // bool flipped = playerTarget.position.x < transform.position.x;
                // Transform bulletSpawn = flipped ? bulletSpawnLeft : bulletSpawnRight;
                // shotgunBlasts.SpawnTransformOverride(bulletSpawn.position, bulletSpawn.eulerAngles.z + (flipped ? 180 : 0), -1);
                // canShoot = false;

                // if (flipped)
                //     rb.AddForce(bulletSpawn.transform.right * recoilForce, ForceMode2D.Impulse);
                // else
                //     rb.AddForce(-bulletSpawn.transform.right * recoilForce, ForceMode2D.Impulse);
                // timer = 0;
                canShoot = false;
                timer = 0;
                doTimer = false;
                shootCoroutine = StartCoroutine(Shoot(playerTarget.position.x < transform.position.x));

            }
        }
        else if (doTimer)
        {
            timer += Time.deltaTime;
            if (timer > minShootDelay) canShoot = true;

        }




    }
    private bool doTimer = false;

    public void SetShootData(ref float dur, ref float rot)
    {


        if (transform.position.y < playerTarget.position.y)
        {
            if (transform.position.x < playerTarget.position.x)
            {
                offset.x = -3;

            }
            else
            {
                offset.x = 3f;
            }
            offset.y = 1.5f;
            rot = 110;
            dur += .1f;

        }
        else
        {
            if (transform.position.x < playerTarget.position.x)
            {
                offset.x = -3f;


            }
            else
            {
                offset.x = 3f;
            }
            rot = 70f;
            offset.y = -1f;
            dur -= .1f;
        }
    }
    private bool doingLogic = false;
    private IEnumerator Shoot(bool flipped)
    {
        doingLogic = true;
        float t = 0;
        SpriteRenderer sr = flipped ? leftShotgunAimSprite : rightShotgunAimSprite;
        sr.color = new Color(1, .29f, .8f, 0);
        sr.transform.localScale = Vector3.one;
        int direction = flipped ? 180 : 0;
        float dur = .8f;

        float rot = 90;
        float arm = 80;
        bool fullRot = true;
        // SetShootData(ref dur, ref rot);
        if (transform.position.x < playerTarget.position.x)
        {
            offset.x = -4;

        }
        else
        {

            offset.x = 4f;
        }
        if (transform.position.y < playerTarget.position.y)
        {
            Debug.Log("Lower");


            offset.y = -2f;
            if (!flipped)
                rot = 120;
            else
                rot = 60;
            dur -= .1f;

        }
        else
        {
            fullRot = false;
            if (!flipped)
                rot = 60f;
            else
                rot = 120f;
            offset.y = 2f;
            dur += .23f;
        }
        // aimingWithLeft = flipped;

        if (flipped)
            aimRotationTransformLeftArm.enabled = true;
        else
            aimRotationTransformRightArm.enabled = true;

        speedRef = rb.linearVelocity;
        moveToPos = true;

        rb.linearVelocity = Vector2.zero;

        currentAngle = rot + direction;

        if (flipped)
        {
            currentArm = armLeft;
            //  if (additionalAimSpeed < 0) additionalAimSpeed *= -1;
        }

        else
        {
            currentArm = armRight;
            //  if (additionalAimSpeed > 0) additionalAimSpeed *= -1;
        }


        movementScript.SetRotationTargets(dur, rot + direction, 0, 80, flipped, fullRot);
        sr.enabled = true;
        while (t < aimDelay)
        {
            // fade the sprite in over time 
            sr.color = new Color(1, .29f, .8f, t / aimDelay);
            sr.transform.localScale = Vector3.one * Mathf.Lerp(1, 1.8f, t / aimDelay);
            t += Time.deltaTime;
            yield return null;

        }
        sr.enabled = false;




        Transform bulletSpawn = flipped ? bulletSpawnLeft : bulletSpawnRight;
        AudioManager.instance.PlayShoutgunNoise(0);
        shotgunBlasts.SpawnTransformOverride(bulletSpawn.position, bulletSpawn.eulerAngles.z, -1);

        moveToPos = false;

        rb.AddForce(-bulletSpawn.transform.right * recoilForce, ForceMode2D.Impulse);
        timer = 0;
        if (flipped)
            aimRotationTransformLeftArm.ResetToZero();
        else
            aimRotationTransformRightArm.ResetToZero();

        yield return new WaitForSeconds(.15f);
        doingLogic = false;
        movementScript.RevertToNormalRotation();

        yield return new WaitForSeconds(.6f);


        doTimer = true;


    }

    public void Egged(bool isEgged)
    {
        if (isEgged)
        {
            if (!doTimer)
            {
                StopCoroutine(shootCoroutine);
                if (doingLogic)
                    movementScript.RevertToNormalRotation();
            }
            moveToPos = false;
            doingLogic = false;
            doTimer = false;
            canShoot = false;


        }
        else
        {
            if (timer > minShootDelay) timer = minShootDelay;
            timer *= .35f;
            doTimer = true;

        }

    }
}
