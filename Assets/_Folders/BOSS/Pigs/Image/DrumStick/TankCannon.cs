
using UnityEngine;
using System.Collections;

public class TankCannon : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Transform playerTransform;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform spawnPos;
    private GameObject bullet;
    [SerializeField] private float rotationSpeed = 10.0f; // degrees per second
    [SerializeField] private float maxRotationAngle = 60.0f; // degrees
    [SerializeField] private float reloadTime = 2.0f; // seconds
    [SerializeField] private float firePauseTimeBefore = 1.0f; // seconds
    [SerializeField] private float firePauseTimeAfter = 1.0f; // seconds

    private float timeSinceLastFire = 0.0f;
    private float pauseEndTime = 0.0f;
    private float desiredRotation;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();

    }
    void Start()
    {
        bullet = Instantiate(bulletPrefab);
        bullet.SetActive(false);
        // playerTransform = player.transform;

    }
    private void OnEnable()
    {
        anim.SetTrigger("Reset");
        StartCoroutine(RandomFire());


    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (player == null)
    //     {


    //     }

    //     // Fire logic
    //     timeSinceLastFire += Time.deltaTime;
    //     if (timeSinceLastFire >= reloadTime + firePauseTimeBefore && Time.time >= pauseEndTime)
    //     {
    //         AimAndFire();
    //         timeSinceLastFire = 0f;
    //         pauseEndTime = Time.time + firePauseTimeAfter + firePauseTimeBefore;
    //     }

    //     // If not in pause state, rotate towards desired position
    //     if (Time.time >= pauseEndTime - firePauseTimeBefore)
    //     {
    //         Vector3 direction = GetPredictiveDirection();
    //         desiredRotation = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;

    //         // Clamp the rotation between -maxRotationAngle and +maxRotationAngle relative to the initial rotation
    //         desiredRotation = Mathf.Clamp(desiredRotation, -maxRotationAngle, maxRotationAngle);
    //     }

    //     Quaternion targetRotation = Quaternion.Euler(0, 0, desiredRotation);
    //     transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    // }

    Vector3 GetPredictiveDirection()
    {
        var playerPos = playerTransform.position;
        Vector3 direction = playerPos - transform.position;

        // If the player is below 0 on the Y-axis, aim above them; otherwise, aim below
        direction.y += playerPos.y < 0 ? 1 : -1;

        return direction;
    }

    private IEnumerator RandomFire()
    {
        bool shotHigh = false;
        float randomZ = 0;
        Quaternion targetRotation;

        while (true)
        {
            // Set the randomZ based on shotHigh toggle
            if (shotHigh)
            {
                randomZ = Random.Range(-14.0f, -6.0f);
            }
            else
            {
                randomZ = Random.Range(-34.0f, -18.0f);
            }
            shotHigh = !shotHigh;

            // Calculate target rotation
            targetRotation = Quaternion.Euler(0, 0, randomZ);

            // Lerp rotation towards target rotation
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5);
                yield return null; // Ensure the loop does not block the execution
            }

            // Call shoot function after reaching the target rotation
            AimAndFire(randomZ);

            // Wait for a short delay before starting the next rotation
            yield return new WaitForSeconds(3.0f);
        }
    }
    void AimAndFire(float rot)
    {
        bullet.SetActive(false);

        bullet.transform.position = spawnPos.position;
        bullet.transform.rotation = Quaternion.Euler(0, 0, rot);
        bullet.SetActive(true);
        // Assuming you want the bullet to be instantiated at the tank's position with the same rotation
        // Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, desiredRotation));

        anim.SetTrigger("ShootTrigger");
    }
    void OnDisable()
    {
        if (bullet != null)
        {
            bullet.SetActive(false);

        }
    }
}
