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
    [SerializeField] private float maxRotation;
    [SerializeField] private float moveSSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private Vector2 speedRef;

    [SerializeField] private Vector2 offset;
    private Coroutine shootCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<FlappyPigMovement>();

        shotgunBlasts.Initialize();
        playerTarget = player._transform;
        shotgunAimLeft.SetConstantRotation(1, playerTarget);
        // shotgunAimLeft.SetIsFlipped(true);
        shotgunAimLeft.SetMaxRotation(maxRotation);
        shotgunAimRight.SetConstantRotation(1, playerTarget);
        shotgunAimRight.SetMaxRotation(maxRotation);

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
        float dur = aimDelay;
        float rot = 90;
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
            if (!flipped)
                rot = 60f;
            else
                rot = 120f;
            offset.y = 2f;
            dur += .15f;
        }
        moveToPos = true;

        movementScript.SetRotationTargets(dur, rot + direction, 0, 80, flipped);
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
            timer = 0;

        }
        else
        {
            timer = 0;
            doTimer = true;

        }

    }
}
