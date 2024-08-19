using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class MissilePigScript : MonoBehaviour
{
    public bool flippedPig;
    public bool flippedWeapon;
    private bool flipped;
    private bool canShoot = true;
    private Vector3 aimAdjustment = new Vector3(0, 180, 0);
    [SerializeField] private Transform weaponStrap;
    private Vector3 aimAdjustmentVar;

    private int maxMissiles;
    private int currentMissilesShot;


    private Vector3 adjustAimAngle;

    private float normalMoveSpeed = 4.9f;
    private float flippedMoveSpeed = -3.2f;

    private float speed;
    [SerializeField] private float downYAmountForReload;
    [SerializeField] private float reloadTimeRotationToMissileRatio;
    [SerializeField] private ParticleSystem ps;
    private Transform player;
    [SerializeField] private float shootTime;
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

    private Pool pool; // Drag your pool reference here in the inspector


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        pool = PoolKit.GetPool("ExplosionPool");

        player = GameObject.Find("Player").GetComponent<Transform>();

        if (flippedPig)
            anim.speed = 1.3f;
        else
            anim.speed = 1f;


        // Adjust speed and ranges based on the flipped state



        // Adjust rotation based on the flipped state

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        if (transform.position.x < BoundariesManager.leftBoundary)
        {
            gameObject.SetActive(false);
        }

        if (player != null && canShoot)
        {
            float distance = player.transform.position.x - transform.position.x;
            bool inRange = (distance > rangesMinMaxVar.x && distance < rangesMinMaxVar.y);

            if (inRange)
            {
                StartCoroutine(LaunchMissile());
                canShoot = false;
                missileTimer = 0;
            }
        }
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
                // if (flippedPig)
                // {
                //     angle = Mathf.Clamp(angle, -90f, -10f);  // Adjust clamping for flipped pig
                // }
                // else
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

    private void OnEnable()
    {
        missileImage.enabled = true;
        canShoot = true;

        if (flippedPig)
        {
            rangesMinMaxVar = new Vector2(-rangesMinMax.y, -rangesMinMax.x);
            speed = flippedMoveSpeed;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            if (flippedWeapon)
            {
                flipped = false;

                weaponStrap.rotation = Quaternion.identity;
            }
            else
            {
                flipped = true;
                weaponStrap.rotation = Quaternion.Euler(0f, 180f, 0f);
            }



        }
        else
        {
            speed = normalMoveSpeed;
            transform.rotation = Quaternion.identity;

            if (flippedWeapon)
            {
                flipped = true;
                weaponStrap.rotation = Quaternion.Euler(0f, 180f, 0f);


            }
            else
            {
                weaponStrap.rotation = Quaternion.identity;
                flipped = false;

            }


        }

        if (flipped)
        {

            rangesMinMaxVar = new Vector2(-rangesMinMax.y, -rangesMinMax.x);

        }
        else
            rangesMinMaxVar = rangesMinMax;

        missileTimer = shootTime;

        transform.position = startPos;
    }
} // Flip rotation on the Y-axis
