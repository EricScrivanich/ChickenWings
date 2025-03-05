using UnityEngine;
using System.Collections.Generic;

// [RequireComponent(typeof(LineRenderer))]
public class WavePathRenderer : MonoBehaviour
{
    [Header("Prediction Settings")]
    [Tooltip("How long to simulate the pig's movement (seconds).")]
    public float predictionDuration = 5f;
    [Tooltip("Number of points in the trajectory.")]
    public int pathResolution = 100;
    [Tooltip("Time step per simulation step.")]
    public float timeStep = 0.05f;

    private BigPigMovement pigMovement;
    private List<Vector3> predictedPath = new List<Vector3>();

    void Awake()
    {
        pigMovement = GetComponent<BigPigMovement>();
    }

    // void OnDrawGizmos()
    // {
    //     // if (!Application.isPlaying) return; // Only show in Play Mode

    //     PredictPath();
    //     DrawPathGizmos();
    // }

    /// <summary>
    /// Predicts the entire movement path before the pig starts moving.
    /// </summary>
    void PredictPath()
    {
        predictedPath.Clear();

        // Initialize simulated physics state
        Vector2 simulatedPosition = transform.position;
        Vector2 simulatedVelocity = new Vector2(-pigMovement.speed, 0); // Constant leftward movement
        Vector2 gravity = Physics2D.gravity * pigMovement.GetComponent<Rigidbody2D>().gravityScale;

        float elapsedTime = 0f;
        bool flapped = false; // Track if a flap was applied

        while (elapsedTime < predictionDuration)
        {
            // Apply gravity
            simulatedVelocity += gravity * timeStep;

            // Apply flapping force when falling below the flap threshold
            if (simulatedPosition.y < (pigMovement.GetInitialY() - pigMovement.distanceToFlap) && !flapped)
            {
                simulatedVelocity.y += pigMovement.yForce; // Apply upward flap force
                flapped = true; // Mark flap as applied
            }
            else if (simulatedPosition.y > (pigMovement.GetInitialY() - pigMovement.distanceToFlap))
            {
                flapped = false; // Reset flap state when rising
            }

            // Update position
            simulatedPosition += simulatedVelocity * timeStep;

            // Store position for visualization
            predictedPath.Add(simulatedPosition);

            elapsedTime += timeStep;
        }
    }

    /// <summary>
    /// Draws the predicted movement path using Gizmos.
    /// </summary>
    void DrawPathGizmos()
    {
        Gizmos.color = Color.magenta; // Use magenta to make it stand out

        for (int i = 0; i < predictedPath.Count - 1; i++)
        {
            Gizmos.DrawLine(predictedPath[i], predictedPath[i + 1]);
        }
    }
}