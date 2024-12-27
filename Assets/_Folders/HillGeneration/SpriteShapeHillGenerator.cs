using UnityEngine;
using UnityEngine.U2D;

public class CustomHillGenerator : MonoBehaviour
{
    public SpriteShapeController spriteShapeController; // Reference to Sprite Shape Controller
    public int pointCount = 10;        // Total number of control points
    public float minHeight = 2f;       // Minimum height deviation from base
    public float maxHeight = 5f;       // Maximum height deviation from base
    public float minTangentLength = 1f; // Minimum length of horizontal tangents
    public float maxTangentLength = 3f; // Maximum length of horizontal tangents
    public float hillBaseY = 0f;       // Base Y position of the hills

    void Start()
    {
        GenerateHills();
    }

    void GenerateHills()
    {
        Spline spline = spriteShapeController.spline;
        spline.Clear(); // Clear existing points

        float currentX = 0f; // Start at x = 0
        bool isGoingUp = true; // Start with the first hill going up

        for (int i = 0; i < pointCount; i++)
        {
            // Alternate heights between higher and lower
            float height = Random.Range(minHeight, maxHeight);
            if (!isGoingUp) height = -height; // Alternate up and down
            isGoingUp = !isGoingUp; // Switch direction for the next point

            // Set position for the control point
            float y = hillBaseY + height;
            Vector2 position = new Vector2(currentX, y);

            spline.InsertPointAt(i, position);
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);

            // Randomize tangent lengths
            float tangentLength = Random.Range(minTangentLength, maxTangentLength);
            Vector3 leftTangent = new Vector3(-tangentLength, 0f, 0f);
            Vector3 rightTangent = new Vector3(tangentLength, 0f, 0f);

            spline.SetLeftTangent(i, leftTangent);
            spline.SetRightTangent(i, rightTangent);

            // Increment the x-position for the next point
            currentX += tangentLength * 2; // Distance between points is related to tangent length
        }

        // Close the shape for ground fill
        float finalX = currentX;
        spline.InsertPointAt(pointCount, new Vector2(finalX, -10f)); // Drop down to ground
        spline.InsertPointAt(pointCount + 1, new Vector2(0, -10f)); // Return to the start
        spline.InsertPointAt(pointCount + 2, spline.GetPosition(0)); // Close the loop
        spriteShapeController.RefreshSpriteShape();
    }
}