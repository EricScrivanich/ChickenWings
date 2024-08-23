using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class MissilePigScript : MonoBehaviour
{
    [SerializeField] private float turn_speed;
    public bool flippedPig;
    public bool flippedWeapon;
    private bool flipped;
    private bool canShoot = true;
    private float flippedAngleAdj;
    private int flippedXAdj;
    private Vector3 aimAdjustment = new Vector3(0, 180, 0);
    [SerializeField] private Transform weaponStrap;
    private Vector3 aimAdjustmentVar;

    private Vector3 initalLaunchAimPosition;
    private Vector3 initalLaunchAimRotation;
    private Vector3 initalMissilePosition;

    private int maxMissiles;
    private int currentMissilesShot;


    private Vector3 adjustAimAngle;

    private float normalMoveSpeed = 5.3f;
    private float flippedMoveSpeed = -3.3f;

    private float speed;
    [SerializeField] private float downYAmountForReload;
    [SerializeField] private float reloadTimeRotationToMissileRatio;
    [SerializeField] private ParticleSystem ps;
    private Transform player;

    [SerializeField] private float frontRange;
    [SerializeField] private float backRange;

    [SerializeField] private Vector2 rangesMinMax;
    private Vector2 rangesMinMaxVar;
    [SerializeField] private float aimTime;
    [SerializeField] private float reloadTime;  // Time for reloading
    private float missileTimer;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Transform entireWeaponAndStrap;
    [SerializeField] private Transform launchAim;
    [SerializeField] private Transform missileSpawnPos;

    [SerializeField] private SpriteRenderer missileImage;
    private Animator anim;

    private float yPos = -3.56f;

    private Pool pool; // Drag your pool reference here in the inspector


    // Start is called before the first frame update
    void Start()
    {


        pool = PoolKit.GetPool("ExplosionPool");

        player = GameObject.Find("Player").GetComponent<Transform>();
    }
    void Awake()
    {
        anim = GetComponent<Animator>();
        initalMissilePosition = missileImage.transform.localPosition;
        initalLaunchAimPosition = launchAim.localPosition;
        initalLaunchAimRotation = launchAim.localEulerAngles;


    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        if (transform.position.x < BoundariesManager.leftBoundary)
        {
            gameObject.SetActive(false);
        }

        if (player != null && canShoot && transform.position.x < 10 && transform.position.x > -9.5f)
        {
            float distance = player.transform.position.x - transform.position.x;
            bool inRange = (distance > rangesMinMaxVar.x && distance < rangesMinMaxVar.y);

            if (inRange)
            {
                StartCoroutine(LaunchMissileNew());
                canShoot = false;

            }
        }
    }

    private IEnumerator LaunchMissileNew()
    {
        float elapsedTime = 0f;



        while (elapsedTime < aimTime)
        {
            elapsedTime += Time.deltaTime;

            // Calculate direction to the player
            Vector3 direction = player.position - launchAim.position;

            // Use LookAt to rotate the launchAim towards the player
            // Since we're working in 2D, we only want to adjust the Z axis (ignoring the Y and X axes)
            float angle = Mathf.Atan2(direction.y, flippedXAdj * direction.x) * Mathf.Rad2Deg - 90;

            // if (angle < 5)
            // {
            //     Debug.LogError("angle at 5: " + angle);
            //     Debug.Log("Rotation is" + launchAim.localRotation.eulerAngles);
            //     yield return null;


            // }
            // else if (angle > 90)
            // {
            //     Debug.Log("Rotation is" + launchAim.localRotation.eulerAngles);
            //     Debug.LogError("angle at 95");
            //     yield return null;
            // }
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            float currentRotZ = launchAim.localEulerAngles.z;

            if (currentRotZ > 95 && currentRotZ < 320)
            {
                launchAim.localRotation = Quaternion.Euler(0, 0, 95);



            }

            else if (currentRotZ > 330 || currentRotZ < 2)
            {
                launchAim.localRotation = Quaternion.Euler(0, 0, 2);


            }

            else
                launchAim.localRotation = Quaternion.Slerp(launchAim.localRotation, targetRotation, Time.deltaTime * turn_speed);

            yield return null;
        }
        AudioManager.instance.PlayMissileLaucnh();

        ps.Play();
        yield return new WaitForSeconds(.2f);

        // Spawn the missile from the pool with the final rotation
        pool.Spawn("missile", missileImage.transform.position, launchAim.rotation);
        missileImage.enabled = false;
        yield return new WaitForSeconds(.2f);
        Reload();
    }

    private IEnumerator LaunchMissile()
    {
        float elapsedTime = 0f;
        bool playerInRange = true;
        Quaternion targetRotation;
        Quaternion initialRotation = launchAim.rotation;
        Vector2 direction = player.position - launchAim.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 270;

        if (flipped)
        {
            angle -= 270;
            targetRotation = Quaternion.Euler(0f, 180, angle);
        }
        else
            targetRotation = Quaternion.Euler(0f, 0, angle);





        while (elapsedTime < aimTime)
        {
            elapsedTime += Time.deltaTime;

            // Check if the player is still in range
            float distance = player.transform.position.x - transform.position.x;
            playerInRange = (distance > rangesMinMaxVar.x && distance < rangesMinMaxVar.y);

            if (playerInRange)
            {


                // Adjust the direction based on the flipped state


                // Calculate the rotation angle


                // Clamp the rotation to not exceed 90 degrees and not go below 10 degrees
                if (flippedPig)
                {
                    angle = Mathf.Clamp(angle, -90f, -10f);  // Adjust clamping for flipped pig
                }
                else
                    // {
                    angle = Mathf.Clamp(angle, 10f, 90f);
                // }



                launchAim.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / aimTime);
            }

            yield return null;
        }

        ps.Play();
        yield return new WaitForSeconds(.2f);

        // Spawn the missile from the pool with the final rotation
        pool.Spawn("missile", missileImage.transform.position, launchAim.rotation);
        missileImage.enabled = false;
        yield return new WaitForSeconds(.2f);
        Reload();
    }

    private IEnumerator ReloadCoroutine()
    {
        float elapsedTime = 0f;
        float t = reloadTime * reloadTimeRotationToMissileRatio;
        float t2 = reloadTime - t;
        float t3 = .25f;
        Vector3 initialPosition = launchAim.localPosition;
        Vector3 targetPosition = initialPosition + new Vector3(0f, downYAmountForReload, 0f); // Adjust the amount you want to move down

        while (elapsedTime < t)
        {
            elapsedTime += Time.deltaTime;

            // Rotate launchAim to 90 degrees over the reloadTime period
            launchAim.rotation = Quaternion.Slerp(launchAim.rotation, Quaternion.Euler(0f, launchAim.rotation.eulerAngles.y, 90f), elapsedTime / t);

            // Move the launchAim's local position down over the reloadTime period
            launchAim.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / t);

            yield return null;
        }

        // Ensure final position and rotation are exact
        launchAim.rotation = Quaternion.Euler(0f, launchAim.rotation.eulerAngles.y, 90f);
        launchAim.localPosition = targetPosition;
        elapsedTime = 0;
        Vector3 targetPosition2 = missileImage.transform.localPosition;
        Vector3 initialPosition2 = targetPosition2 + new Vector3(0f, -.15f, 0f);
        missileImage.transform.localPosition = initialPosition2;
        missileImage.enabled = true;

        while (elapsedTime < t2)
        {
            elapsedTime += Time.deltaTime;

            // Rotate launchAim to 90 degrees over the reloadTime period


            // Move the launchAim's local position down over the reloadTime period
            missileImage.transform.localPosition = Vector3.Lerp(initialPosition2, targetPosition2, elapsedTime / t2);

            yield return null;
        }

        elapsedTime = 0;

        while (elapsedTime < t3)
        {
            elapsedTime += Time.deltaTime;

            launchAim.localPosition = Vector3.Lerp(targetPosition, initialPosition, elapsedTime / t3);

            yield return null;
        }
        launchAim.localPosition = initialPosition;

        canShoot = true;
    }

    private void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    public void Initialize(bool flippedP, bool flippedW, float rangeAdj, float xPos)
    {

        missileImage.enabled = true;
        canShoot = true;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        missileImage.transform.localPosition = initalMissilePosition;
        launchAim.localPosition = initalLaunchAimPosition;
        launchAim.localRotation = Quaternion.Euler(0, 0, 90);
        weaponStrap.localRotation = Quaternion.Euler(0f, 0, 0f);




        if (flippedP)
        {

            speed = flippedMoveSpeed;
            anim.speed = 1.4f;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            if (flippedW)
            {
                flipped = false;


                weaponStrap.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {

                weaponStrap.localRotation = Quaternion.Euler(0, 0, 0);
                flipped = true;

            }

        }
        else
        {
            speed = normalMoveSpeed;
            anim.speed = 1f;


            if (flippedW)
            {
                flipped = true;
                weaponStrap.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else
            {

                flipped = false;
            }
        }



        if (flipped)
        {

            rangesMinMaxVar = new Vector2(-rangesMinMax.y, -rangesMinMax.x + rangeAdj);
            flippedAngleAdj = 180;
            flippedXAdj = -1;

        }
        else
        {
            rangesMinMaxVar = new Vector2(rangesMinMax.x - rangeAdj, rangesMinMax.y);
            flippedAngleAdj = 0;
            flippedXAdj = 1;


        }


        transform.position = new Vector2(xPos, yPos);
        gameObject.SetActive(true);

    }

    // private void OnEnable()
    // {
    //     missileImage.enabled = true;
    //     canShoot = true;
    //     transform.rotation = Quaternion.Euler(0, 0, 0);
    //     missileImage.transform.localPosition = initalMissilePosition;
    //     launchAim.localPosition = initalLaunchAimPosition;
    //     launchAim.localRotation = Quaternion.Euler(0, 0, 90);
    //     weaponStrap.localRotation = Quaternion.Euler(0f, 0, 0f);




    //     if (flippedPig)
    //     {

    //         speed = flippedMoveSpeed;
    //         anim.speed = 1.4f;
    //         transform.rotation = Quaternion.Euler(0f, 180f, 0f);

    //         if (flippedWeapon)
    //         {
    //             flipped = false;


    //             weaponStrap.localRotation = Quaternion.Euler(0, 180, 0);
    //         }
    //         else
    //         {
    //             Debug.Log("UsignFLFFDFDFDF");
    //             weaponStrap.localRotation = Quaternion.Euler(0, 0, 0);
    //             flipped = true;

    //         }

    //     }
    //     else
    //     {
    //         speed = normalMoveSpeed;
    //         anim.speed = 1f;


    //         if (flippedWeapon)
    //         {
    //             flipped = true;
    //             weaponStrap.localRotation = Quaternion.Euler(0f, 180f, 0f);
    //         }
    //         else
    //         {

    //             flipped = false;
    //         }
    //     }



    //     if (flipped)
    //     {

    //         rangesMinMaxVar = new Vector2(-rangesMinMax.y, -rangesMinMax.x);
    //         flippedAngleAdj = 180;
    //         flippedXAdj = -1;

    //     }
    //     else
    //     {
    //         rangesMinMaxVar = new Vector2(rangesMinMax.x, rangesMinMax.y);
    //         flippedAngleAdj = 0;
    //         flippedXAdj = 1;


    //     }

    // }
}
