using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Transform playerTransform; // Assign your player's transform in the inspector

    public float minYPosition = 5.15f; // The minimum Y position to start zooming and moving
    public float maxYPosition = 6.3f; // The maximum Y position for max zoom and max height
    public float minZoom = 5.0f; // The camera's minimum orthographic size
    public float maxZoom = 5.7f; // The camera's maximum orthographic size
    public float minHeight = 0f; // Minimum height above the initial position
    public float maxHeight = 2f; // Maximum height above the initial position

    private Vector3 initialCameraPosition;
    private float targetOrthographicSize;
    private float velocityZoom = 0f; // Velocity reference for smooth damp on zoom
    private Vector3 velocityPosition = Vector3.zero; // Velocity reference for smooth damp on position
   [SerializeField] private float smoothTimeY; // Time to smooth the transition
    [SerializeField] private float smoothTimeZoom;

    void Start()
    {
        // Save the initial position of the camera
        initialCameraPosition = transform.position;
        targetOrthographicSize = Camera.main.orthographicSize; // Initialize with the current size
    }

    void Update()
    {
        if (playerTransform.position.y > minYPosition)
        {
            float proportion = (playerTransform.position.y - minYPosition) / (maxYPosition - minYPosition);
            proportion = Mathf.Clamp(proportion, 0, 1); // Ensure proportion is between 0 and 1

            // Calculate the target zoom size
            float targetZoom = Mathf.Lerp(minZoom, maxZoom, proportion);
            // Smoothly transition to the target zoom
            Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, targetZoom, ref velocityZoom, smoothTimeZoom);

            // Calculate the new height and smoothly transition to it
            float newHeight = Mathf.Lerp(minHeight, maxHeight, proportion);
            Vector3 targetPosition = new Vector3(transform.position.x, initialCameraPosition.y + newHeight, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocityPosition, smoothTimeY);
        }
        else
        {
            // Smoothly return the camera to its original position and zoom level
            Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, minZoom, ref velocityZoom, smoothTimeZoom);
            transform.position = Vector3.SmoothDamp(transform.position, initialCameraPosition, ref velocityPosition, smoothTimeY);
        }
    }
}
