using UnityEngine;
using Cinemachine;

public class CameraPathFollower : MonoBehaviour
{
    private CinemachineDollyCart dollyCart;
    public Transform player;

    public float minDistanceX = 5f; // Min distance to stop catchup mode (X-axis)
    public float maxDistanceX = 10f; // Max distance to start catchup mode (X-axis)
    public float minDistanceY = 3f; // Min distance to stop catchup mode (Y-axis)
    public float maxDistanceY = 7f; // Max distance to start catchup mode (Y-axis)

    public float normalSpeed = 5f;
    public float catchupSpeed = 10f;

    private bool inCatchupMode = false; // Track whether the camera is in catchup mode

    private void Start()
    {
        dollyCart = GetComponent<CinemachineDollyCart>();
        dollyCart.m_Speed = normalSpeed;
    }

    void Update()
    {
        // Calculate the forward direction of the cart (taking into account X-axis rotation)
        Vector3 cartForward = dollyCart.transform.forward;

        // Calculate the vector from the cart to the player
        Vector3 cartToPlayer = player.position - dollyCart.transform.position;

        // Project the player's position onto the cart's forward direction
        float distanceAlongPath = Vector3.Dot(cartForward, cartToPlayer);

        // Calculate X and Y weights based on the forward direction of the cart
        float xWeight = Mathf.Abs(cartForward.x);
        float yWeight = Mathf.Abs(cartForward.y);

        // Interpolate between the X and Y min/max distances based on the cart's movement direction
        float blendedMaxDistance = Mathf.Lerp(maxDistanceY, maxDistanceX, xWeight);
        float blendedMinDistance = Mathf.Lerp(minDistanceY, minDistanceX, xWeight);

        // If the player's distance along the path exceeds the blended maxDistance, enter catchup mode
        if (distanceAlongPath > blendedMaxDistance)
        {
            inCatchupMode = true;
            dollyCart.m_Speed = catchupSpeed;
        }

        // If the player is within the blended minDistance along the path and we are in catchup mode, return to normal speed
        if (inCatchupMode && distanceAlongPath <= blendedMinDistance)
        {
            inCatchupMode = false;
            dollyCart.m_Speed = normalSpeed;
        }

        Debug.Log($"Distance Along Path: {distanceAlongPath}, BlendedMaxDistance: {blendedMaxDistance}, BlendedMinDistance: {blendedMinDistance}");
    }
}