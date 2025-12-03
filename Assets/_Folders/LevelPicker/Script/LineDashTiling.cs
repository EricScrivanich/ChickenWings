using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineDashTiling : MonoBehaviour
{
    [SerializeField] private AnimationCurve tilingCurve = AnimationCurve.Linear(0, 1, 1, 3);
    [SerializeField] private int segments = 50;
    [SerializeField] private Vector3[] roadPath; // Define your road path points
    [SerializeField] private float baseTiling = 5f; // Base texture repeat

    private LineRenderer lineRenderer;
    private Material lineMaterial;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Create material instance with custom shader
        lineMaterial = new Material(lineRenderer.material);
        lineRenderer.material = lineMaterial;

        // Set texture mode to stretch
        lineRenderer.textureMode = LineTextureMode.Stretch;

        // Set base tiling in shader
        lineMaterial.SetFloat("_BaseTiling", baseTiling);

        UpdateLineTiling();
    }

    void UpdateLineTiling()
    {
        // If no custom path, create a simple straight line
        if (roadPath == null || roadPath.Length < 2)
        {
            roadPath = new Vector3[2];
            roadPath[0] = Vector3.zero;
            roadPath[1] = new Vector3(0, 0, 10);
        }

        lineRenderer.positionCount = segments;

        // Create gradient to store tiling multiplier in alpha channel
        Gradient colorGradient = new Gradient();

        // Keep colors white (or your desired color)
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(Color.white, 0);
        colorKeys[1] = new GradientColorKey(Color.white, 1);

        // Store tiling multipliers in alpha keys
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[segments];

        // Calculate positions along the path
        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);

            // Interpolate position along path
            Vector3 pos = EvaluatePath(t);
            lineRenderer.SetPosition(i, pos);

            // Sample tiling curve and store in alpha
            float tilingMultiplier = tilingCurve.Evaluate(t);
            alphaKeys[i] = new GradientAlphaKey(tilingMultiplier, t);
        }

        // Apply gradient
        colorGradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = colorGradient;
    }

    Vector3 EvaluatePath(float t)
    {
        // Simple linear interpolation between path points
        if (roadPath.Length == 2)
        {
            return Vector3.Lerp(roadPath[0], roadPath[1], t);
        }

        // For multiple points, find which segment we're on
        float scaledT = t * (roadPath.Length - 1);
        int index = Mathf.FloorToInt(scaledT);

        if (index >= roadPath.Length - 1)
        {
            return roadPath[roadPath.Length - 1];
        }

        float segmentT = scaledT - index;
        return Vector3.Lerp(roadPath[index], roadPath[index + 1], segmentT);
    }

    void OnValidate()
    {
        if (Application.isPlaying && lineRenderer != null)
        {
            UpdateLineTiling();
        }
    }

    void OnDestroy()
    {
        if (lineMaterial != null)
        {
            Destroy(lineMaterial);
        }
    }

    // Helper to visualize the path in editor
    void OnDrawGizmos()
    {
        if (roadPath != null && roadPath.Length > 1)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < roadPath.Length - 1; i++)
            {
                Gizmos.DrawLine(roadPath[i], roadPath[i + 1]);
            }
        }
    }
}