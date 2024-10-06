using UnityEngine;
using System.Collections.Generic;

public class SideScrollingCameraController : MonoBehaviour
{
    public Transform playerTransform;
    public float baseScrollSpeed = 2f;
    public float maxPlayerAheadDistance = 3f;
    public float catchupSpeed = 5f;

    [System.Serializable]
    public class Checkpoint
    {
        public float xPosition;
        public float scrollSpeedMultiplier = 1f;
    }

    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    private Camera cam;
    private int currentCheckpointIndex = 0;
    private float leftBoundary;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (checkpoints.Count == 0)
        {
            Debug.LogError("No checkpoints set! Please add checkpoints in the inspector.");
            enabled = false;
            return;
        }
        transform.position = new Vector3(checkpoints[0].xPosition, transform.position.y, transform.position.z);
        leftBoundary = transform.position.x - (cam.orthographicSize * cam.aspect);
    }

    void Update()
    {
        if (playerTransform == null) return;

        float currentScrollSpeed = baseScrollSpeed * checkpoints[currentCheckpointIndex].scrollSpeedMultiplier;

        // Move the camera
        transform.Translate(Vector3.right * currentScrollSpeed * Time.deltaTime);

        // Update left boundary
        leftBoundary = transform.position.x - (cam.orthographicSize * cam.aspect);

        // Check if player is too far behind
        if (playerTransform.position.x < leftBoundary)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = playerTransform.position.x + (cam.orthographicSize * cam.aspect);
            transform.position = newPosition;
        }

        // Check if player is too far ahead
        float rightBoundary = transform.position.x + (cam.orthographicSize * cam.aspect);
        if (playerTransform.position.x > rightBoundary + maxPlayerAheadDistance)
        {
            float targetX = playerTransform.position.x - maxPlayerAheadDistance;
            float newX = Mathf.MoveTowards(transform.position.x, targetX, catchupSpeed * Time.deltaTime);
            Vector3 newPosition = transform.position;
            newPosition.x = newX;
            transform.position = newPosition;
        }

        // Check for checkpoint
        if (currentCheckpointIndex < checkpoints.Count - 1 &&
            transform.position.x >= checkpoints[currentCheckpointIndex + 1].xPosition)
        {
            currentCheckpointIndex++;
            Debug.Log($"Reached checkpoint {currentCheckpointIndex}. New scroll speed: {baseScrollSpeed * checkpoints[currentCheckpointIndex].scrollSpeedMultiplier}");
        }
    }

    void OnDrawGizmos()
    {
        if (!cam) cam = GetComponent<Camera>();

        // Draw checkpoints
        Gizmos.color = Color.yellow;
        foreach (var checkpoint in checkpoints)
        {
            Vector3 checkpointPosition = new Vector3(checkpoint.xPosition, transform.position.y, transform.position.z);
            Gizmos.DrawSphere(checkpointPosition, 0.5f);
        }

        // Draw player boundaries
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            float cameraHeight = 2f * cam.orthographicSize;
            Vector3 leftBoundaryStart = new Vector3(leftBoundary, transform.position.y - cameraHeight / 2, transform.position.z);
            Vector3 leftBoundaryEnd = new Vector3(leftBoundary, transform.position.y + cameraHeight / 2, transform.position.z);
            Gizmos.DrawLine(leftBoundaryStart, leftBoundaryEnd);

            Gizmos.color = Color.green;
            float rightBoundary = transform.position.x + (cam.orthographicSize * cam.aspect) + maxPlayerAheadDistance;
            Vector3 rightBoundaryStart = new Vector3(rightBoundary, transform.position.y - cameraHeight / 2, transform.position.z);
            Vector3 rightBoundaryEnd = new Vector3(rightBoundary, transform.position.y + cameraHeight / 2, transform.position.z);
            Gizmos.DrawLine(rightBoundaryStart, rightBoundaryEnd);
        }
    }
}