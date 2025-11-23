using UnityEngine;
using PathCreation;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoadMesh2D : MonoBehaviour
{
    [Header("References")]
    public PathCreator pathCreator;

    [Header("Base Settings")]
    public float baseWidth = 4f;          // overall scale
    public float uScale = 1f;             // UV tiling scale
    public bool autoUpdate = true;

    [Header("Perspective Width Settings")]
    public float minVerticalWidth = 1f;   // width when path is moving vertically
    public float maxHorizontalWidth = 4f; // width when path moves sideways (left/right)

    [Header("Depth Settings")]
    public float depthNearScale = 1.4f;   // width multiplier when depth is small (near)
    public float depthFarScale = 0.7f;    // width multiplier when deep/far

    private Mesh mesh;

    private void OnEnable()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Road Mesh 2D";
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        if (pathCreator != null)
            pathCreator.pathUpdated += OnPathChanged;

        CreateRoadMesh();
    }

    private void OnDisable()
    {
        if (pathCreator != null)
            pathCreator.pathUpdated -= OnPathChanged;
    }

    private void OnPathChanged()
    {
        if (autoUpdate)
            CreateRoadMesh();
    }

    public void CreateRoadMesh()
    {
        if (pathCreator == null)
        {
            mesh.Clear();
            return;
        }

        VertexPath p = pathCreator.path;
        int numPoints = p.NumPoints;

        Vector3[] verts = new Vector3[numPoints * 2];
        Vector2[] uvs = new Vector2[numPoints * 2];
        int[] tris = new int[(numPoints - 1) * 6];

        for (int i = 0; i < numPoints; i++)
        {
            float t = (float)i / (numPoints - 1);
            Vector3 point = p.GetPoint(i);

            // -----------------------------------------
            // (1) Get direction of the road at this point
            // -----------------------------------------
            Vector3 dir;
            if (i < numPoints - 1)
                dir = (p.GetPoint(i + 1) - p.GetPoint(i)).normalized;
            else
                dir = (p.GetPoint(i) - p.GetPoint(i - 1)).normalized;

            // -----------------------------------------
            // (2) Compute horizontal/vertical perspective
            // -----------------------------------------
            float horizontalAmount = Mathf.Abs(dir.x); // 1 = sideways, 0 = vertical

            float horizontalFactor = Mathf.Lerp(
                minVerticalWidth,    // narrower when vertical
                maxHorizontalWidth,  // wider when horizontal
                horizontalAmount
            );

            // -----------------------------------------
            // (3) Depth-based width adjustment 
            // -----------------------------------------
            float depth = pathCreator.GetCustomPointDistanceAtPercent(t);

            float depthFactor = Mathf.Lerp(depthFarScale, depthNearScale, depth);

            // -----------------------------------------
            // (4) Final width
            // -----------------------------------------
            float width = baseWidth * horizontalFactor * depthFactor;

            // -----------------------------------------
            // (5) Compute cross-direction normal 
            // -----------------------------------------
            Vector3 normal = Vector3.Cross(dir, Vector3.forward).normalized;

            Vector3 left = point - normal * (width * 0.5f);
            Vector3 right = point + normal * (width * 0.5f);

            verts[i * 2 + 0] = transform.InverseTransformPoint(left);
            verts[i * 2 + 1] = transform.InverseTransformPoint(right);

            // -----------------------------------------
            // (6) UV mapping
            // -----------------------------------------
            float dist = p.GetClosestDistanceAlongPath(point);

            // how often your dashed texture repeats per meter
            float v = dist * uScale;

            uvs[i * 2 + 0] = new Vector2(0, v);
            uvs[i * 2 + 1] = new Vector2(1, v);

            // -----------------------------------------
            // (7) Triangles
            // -----------------------------------------
            if (i < numPoints - 1)
            {
                int ti = i * 6;
                int vi = i * 2;

                tris[ti + 0] = vi;
                tris[ti + 1] = vi + 2;
                tris[ti + 2] = vi + 1;

                tris[ti + 3] = vi + 1;
                tris[ti + 4] = vi + 2;
                tris[ti + 5] = vi + 3;
            }
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}