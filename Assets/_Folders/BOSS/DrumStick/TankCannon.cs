
using UnityEngine;

public class TankCannon : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Transform playerTransform;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float rotationSpeed = 10.0f; // degrees per second
    [SerializeField] private float maxRotationAngle = 60.0f; // degrees
    [SerializeField] private float reloadTime = 2.0f; // seconds
    [SerializeField] private float firePauseTimeBefore = 1.0f; // seconds
    [SerializeField] private float firePauseTimeAfter = 1.0f; // seconds

    private float timeSinceLastFire = 0.0f;
    private float pauseEndTime = 0.0f;
    private float desiredRotation;

    void Start()
    {
        playerTransform = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
            
        }

        // Fire logic
        timeSinceLastFire += Time.deltaTime;
        if (timeSinceLastFire >= reloadTime + firePauseTimeBefore && Time.time >= pauseEndTime)
        {
            AimAndFire();
            timeSinceLastFire = 0f;
            pauseEndTime = Time.time + firePauseTimeAfter + firePauseTimeBefore;
        }
        
        // If not in pause state, rotate towards desired position
        if (Time.time >= pauseEndTime - firePauseTimeBefore)
        {
            Vector3 direction = GetPredictiveDirection();
            desiredRotation = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            
            // Clamp the rotation between -maxRotationAngle and +maxRotationAngle relative to the initial rotation
            desiredRotation = Mathf.Clamp(desiredRotation, -maxRotationAngle, maxRotationAngle);
        }
        
        Quaternion targetRotation = Quaternion.Euler(0, 0, desiredRotation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    Vector3 GetPredictiveDirection()
    {
        var playerPos = playerTransform.position;
        Vector3 direction = playerPos - transform.position;

        // If the player is below 0 on the Y-axis, aim above them; otherwise, aim below
        direction.y += playerPos.y < 0 ? 1 : -1;

        return direction;
    }

    void AimAndFire()
    {
        // Assuming you want the bullet to be instantiated at the tank's position with the same rotation
        Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, desiredRotation));
    }
}
