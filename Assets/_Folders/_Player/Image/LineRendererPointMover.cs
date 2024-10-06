using UnityEngine;

public class LineRendererPointMover : MonoBehaviour
{
    public LineRenderer lineRenderer;     // Reference to the LineRenderer

    public int firstPointIndex = 0;       // Index of the first point to move
    public float firstPointSpeed = 1f;    // Speed of the first point along the x-axis

    public int secondPointIndex = 1;      // Index of the second point to move
    public float secondPointSpeed = 1.5f; // Speed of the second point along the x-axis

    private Vector3[] linePoints;         // Store the positions of the line's points

    void Start()
    {
        // Get all the points in the LineRenderer
        linePoints = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(linePoints);
    }

    void Update()
    {
        // Move the first point
        if (firstPointIndex >= 0 && firstPointIndex < linePoints.Length)
        {
            linePoints[firstPointIndex].x += firstPointSpeed * Time.deltaTime;
            lineRenderer.SetPosition(firstPointIndex, linePoints[firstPointIndex]);
        }

        // Move the second point
        if (secondPointIndex >= 0 && secondPointIndex < linePoints.Length)
        {
            linePoints[secondPointIndex].x += secondPointSpeed * Time.deltaTime;
            lineRenderer.SetPosition(secondPointIndex, linePoints[secondPointIndex]);
        }
    }
}