using UnityEngine;
using DG.Tweening;

public class TrackedCameraPosition : MonoBehaviour
{
    private Quaternion targetRotation; // The target rotation for the object to Slerp towards
    public float slerpSpeed = 5f;

    public float startSmoothRotationDistance;

    private Sequence changeDirectionSeq;

    [SerializeField] private float tweenDuration;
    private bool changingDirection = false;
    private bool hasChangedDirection = false;



    [Header("Path Settings")]
    public Transform[] pathPoints; // Array of points defining the path
    public float moveSpeed = 5f; // Normal speed at which the object moves along the path
    public float catchupSpeed = 10f; // Speed the object moves at when catching up
    public float maxDistance = 5f; // Max distance allowed before speeding up
    public float maxDistanceCrossTarget = -5f; // Range after exceeding maxDistance to apply catchupSpeed

    [Header("Player Tracking")]
    public Transform player; // Reference to the player
    private LineRenderer lineRenderer; // LineRenderer component to draw the player's distance

    private int currentPointIndex = 0; // The current point in the path the object is moving towards
    private Vector3 lastPosition; // The last position of the object to determine direction of travel
    private Vector3 directionOfTravel; // Direction of travel vector

    private float playerDistanceAlongPath; // Player distance along the path
    private float playerPerpendicularDistance; // Player perpendicular distance from path

    private float currentSpeed; // The current speed of the object
    private bool inCatchupMode = false; // Whether the object is in catchup mode
    private float distanceTraveledAtCatchup; // Track how much distance has been traveled in catchup mode
    private float totalCatchupDistance; // Total distance to travel with catchup speed

    [Header("Max Distance Settings (X and Y)")]
    public float maxDistanceX = 5f; // Max distance allowed for X-axis before speeding up
    public float maxDistanceY = 3f; // Max distance allowed for Y-axis before speeding up
    public float maxDistanceCrossTargetX = -5f; // Target distance for X-axis after exceeding maxDistanceX to apply catchupSpeed
    public float maxDistanceCrossTargetY = -3f;

    public float smoothDistance = 3f; // The distance from the next point where we start logging the direction change


    void Start()
    {
        if (pathPoints.Length > 0)
        {
            transform.position = pathPoints[0].position;
            lastPosition = transform.position;
        }

        // Initialize speed
        currentSpeed = moveSpeed;

        // Calculate the total distance the object will travel with catchup speed
        totalCatchupDistance = Mathf.Abs(maxDistanceCrossTarget - maxDistance);

        // Get or add the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configure LineRenderer
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2; // The line will have two points
    }

    void Update()
    {
        if (pathPoints.Length == 0 || player == null) return;

        // Move towards the next point in the path
        MoveAlongPath();

        // Calculate and log the player's distance relative to this object's movement direction
        CalculatePlayerDistance();

        // Update the line renderer to draw the distance
        UpdateLineRenderer();

        // Adjust movement speed based on player distance
        AdjustSpeed();
    }

    private void ChangeDirectionTween(Quaternion targetRot)
    {

        // changingDirection = true;
        // if (changeDirectionSeq != null && changeDirectionSeq.IsPlaying())
        //     changeDirectionSeq.Kill();
        // changeDirectionSeq = DOTween.Sequence();


        // changeDirectionSeq.Append(transform.DORotateQuaternion(targetRot, tweenDuration).SetEase(Ease.InOutSine));

        // changeDirectionSeq.Play().OnComplete(() => changingDirection = false);




    }



    private void MoveAlongPath()
    {
        if (currentPointIndex >= pathPoints.Length) return;

        // Move towards the next point
        Vector3 targetPoint = pathPoints[currentPointIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, currentSpeed * Time.deltaTime);

        // Calculate the new direction of travel
        Vector3 newDirectionOfTravel = (targetPoint - transform.position).normalized;

        // Check if the direction of travel has changed
        if (newDirectionOfTravel != directionOfTravel)
        {
            directionOfTravel = newDirectionOfTravel;

            // Calculate the weight of X and Y movement direction based on the new angle
            float xWeight = Mathf.Abs(directionOfTravel.x);
            float yWeight = Mathf.Abs(directionOfTravel.y);

            // Calculate the blended maxDistance for the new direction
            float blendedMaxDistance = Mathf.Lerp(maxDistanceY, maxDistanceX, xWeight);

            // If the player is still beyond the new maxDistance, keep catchup mode; otherwise, stop catchup mode
            if (playerDistanceAlongPath > blendedMaxDistance)
            {
                inCatchupMode = true;
                currentSpeed = catchupSpeed;
            }
            else
            {
                inCatchupMode = false;
                currentSpeed = moveSpeed;
            }
        }
        // if (Vector3.Distance(transform.position, targetPoint) < startSmoothRotationDistance && !hasChangedDirection)
        // {
        //     int trackedPoint = currentPointIndex + 1;
        //     Vector3 nextPoint = pathPoints[trackedPoint].position;
        //     Vector3 directionToNextPoint = (nextPoint - transform.position).normalized;
        //     targetRotation = Quaternion.LookRotation(Vector3.forward, directionToNextPoint);
        //     // hasChangedDirection = true;

        //     ChangeDirectionTween(targetRotation);
        //     currentPointIndex++;
        // }


        if (Vector3.Distance(transform.position, targetPoint) < 1f)
        {
            currentPointIndex++;
            hasChangedDirection = false;
            Vector3 nextPoint = pathPoints[currentPointIndex].position;
            Vector3 directionToNextPoint = (nextPoint - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(Vector3.forward, directionToNextPoint);
        }
        // transform.rotation = targetRotation;

        // // Move in the direction the object is facing using Translate
        // transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);

        // Update the last position
        lastPosition = transform.position;
    }
    private void CalculatePlayerDistance()
    {
        // Calculate player's position relative to this object
        Vector3 playerRelativePos = player.position - transform.position;

        // Project the player's relative position onto the direction of travel (X-axis distance)
        playerDistanceAlongPath = Vector3.Dot(playerRelativePos, directionOfTravel);

        // Calculate perpendicular distance (Y-axis distance) by projecting onto the perpendicular direction
        playerPerpendicularDistance = Vector3.Dot(playerRelativePos, new Vector3(-directionOfTravel.y, directionOfTravel.x, 0));

        // Log the distances
        // Debug.Log("Player Distance Along Path: " + playerDistanceAlongPath);
        // Debug.Log("Player Perpendicular Distance: " + playerPerpendicularDistance);
    }

    private void AdjustSpeed()
    {
        // Calculate the weight of X and Y movement direction based on the angle
        float xWeight = Mathf.Abs(directionOfTravel.x);
        float yWeight = Mathf.Abs(directionOfTravel.y);

        // Calculate the blended maxDistance and maxDistanceCrossTarget based on movement direction
        float blendedMaxDistance = Mathf.Lerp(maxDistanceY, maxDistanceX, xWeight);
        float blendedMinDistance = Mathf.Lerp(maxDistanceCrossTargetY, maxDistanceCrossTargetX, xWeight);
        float totalCatchupDistance = Mathf.Abs(blendedMinDistance - blendedMaxDistance);

        // If the object is in catchup mode
        if (inCatchupMode)
        {
            // Track how far the object has traveled in catchup mode
            distanceTraveledAtCatchup += currentSpeed * Time.deltaTime;

            // Calculate the percentage of the catchup distance traveled
            float travelPercentage = Mathf.Clamp01(distanceTraveledAtCatchup / totalCatchupDistance);

            // Smoothly interpolate speed back to normal speed
            currentSpeed = Mathf.Lerp(catchupSpeed, moveSpeed, travelPercentage);

            // If the object has traveled the full catchup range, reset to normal speed
            if (distanceTraveledAtCatchup >= totalCatchupDistance)
            {
                inCatchupMode = false;
                currentSpeed = moveSpeed;
                distanceTraveledAtCatchup = 0f;
            }
        }
        else
        {
            // If the player's distance along the path exceeds the blended maxDistance, start catchup mode
            if (playerDistanceAlongPath > blendedMaxDistance)
            {
                inCatchupMode = true;
                currentSpeed = catchupSpeed;
                Debug.Log($"Player Distance Along Path (Hit maxDistance): {playerDistanceAlongPath}");
            }
        }
    }
    private void UpdateLineRenderer()
    {
        // Set the first point of the line to the position of the tracked camera object
        lineRenderer.SetPosition(0, transform.position);

        // Calculate the position along the path where the player is projected
        Vector3 playerDistanceAlongPathVector = directionOfTravel * playerDistanceAlongPath;

        // Set the second point of the line to the player's projected position along the path
        lineRenderer.SetPosition(1, transform.position + playerDistanceAlongPathVector);
    }

    // Draw gizmos in runtime for debugging player distance
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;

            // Draw line from camera target to player to show the relative position
            Gizmos.DrawLine(transform.position, player.position);

            // Draw a line in the direction of travel for better visualization
            if (directionOfTravel != Vector3.zero)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + directionOfTravel * 2f);
            }

            // Visualize the player's distance along the path
            Vector3 playerDistanceAlongPathVector = directionOfTravel * playerDistanceAlongPath;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + playerDistanceAlongPathVector);

            // Visualize the player's perpendicular distance from the path
            Vector3 perpendicularDir = new Vector3(-directionOfTravel.y, directionOfTravel.x, 0);
            Vector3 playerPerpendicularVector = perpendicularDir * playerPerpendicularDistance;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + playerDistanceAlongPathVector, transform.position + playerPerpendicularVector);
        }

        // Draw the path points in editor
        if (pathPoints != null && pathPoints.Length > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}