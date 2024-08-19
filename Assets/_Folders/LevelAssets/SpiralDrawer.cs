using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SpiralDrawer : MonoBehaviour
{
    [SerializeField] private float outerRadius = 5f;
    [SerializeField] private int turns = 3;
    [SerializeField] private int pointsPerTurn = 100;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private float entryAngle = 0f; // in degrees
    [SerializeField] private float entranceLineRadius = 1f; // Radius of the entrance semicircle
    [SerializeField] private int entranceLinePoints = 50; // Number of points in the entrance semicircle
    [SerializeField] private bool flipOrientation = false; // Boolean to flip the semicircle and spiral orientation

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        DrawSpiral();
    }

    private void DrawSpiral()
    {
        int spiralPoints = turns * pointsPerTurn;
        int totalPoints = spiralPoints + entranceLinePoints;
        Vector3[] positions = new Vector3[totalPoints];
        float entryAngleRad = Mathf.Deg2Rad * entryAngle;

        // Draw the entrance semicircle
        for (int i = 0; i < entranceLinePoints; i++)
        {
            float t = (float)i / (entranceLinePoints - 1);
            float angle = Mathf.PI * (1 - t); // Semicircle angle, opposite direction

            float x = entranceLineRadius * Mathf.Cos(angle);
            float y = entranceLineRadius * Mathf.Sin(angle);

            // Rotate the semicircle based on the entry angle
            float rotatedX = x * Mathf.Cos(entryAngleRad) - y * Mathf.Sin(entryAngleRad);
            float rotatedY = x * Mathf.Sin(entryAngleRad) + y * Mathf.Cos(entryAngleRad);

            // Flip the semicircle if needed
            if (flipOrientation)
            {
                rotatedY = -rotatedY;
            }

            positions[i] = new Vector3(rotatedX + outerRadius, rotatedY, 0); // Shift to start from the outer radius
        }

        // Calculate the start point for the spiral
        Vector3 entranceEnd = positions[entranceLinePoints - 1];
        float startRadius = entranceLineRadius;
        float spiralEntryAngle = Mathf.Atan2(entranceEnd.y, entranceEnd.x);

        // Draw the spiral
        for (int i = 0; i < spiralPoints; i++)
        {
            float t = (float)i / (spiralPoints - 1);
            float angle = 2 * Mathf.PI * turns * t;
            float currentRadius = startRadius * (1 - t);

            if (flipOrientation)
            {
                angle = -angle;
            }

            float x = currentRadius * Mathf.Cos(angle + spiralEntryAngle);
            float y = currentRadius * Mathf.Sin(angle + spiralEntryAngle);

            positions[entranceLinePoints + i] = new Vector3(x, y, 0);
        }

        lineRenderer.positionCount = totalPoints;
        lineRenderer.SetPositions(positions);
    }

    void OnValidate()
    {
        if (lineRenderer != null)
        {
            DrawSpiral();
        }
    }
}