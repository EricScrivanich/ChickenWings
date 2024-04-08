
using UnityEngine;
public class CameraZoom : MonoBehaviour
{
    public Transform playerTransform; // Assign your player's transform in the inspector
    public float minYPosition = 5.15f; // The minimum Y position to start zooming
    public float maxYPosition = 6.3f; // The maximum Y position for max zoom
    public float minZoom = 5.0f; // The camera's minimum orthographic size
    public float maxZoom = 5.7f; // The camera's maximum orthographic size

    void Update()
    {
        if (playerTransform.position.y > minYPosition)
        {
            float proportion = (playerTransform.position.y - minYPosition) / (maxYPosition - minYPosition);
            proportion = Mathf.Clamp(proportion, 0, 1); // Ensure proportion is between 0 and 1

            // Lerp the camera size based on the player's position
            Camera.main.orthographicSize = Mathf.Lerp(minZoom, maxZoom, proportion);
        }
    }
}