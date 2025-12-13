using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineGlowUV2Setter : MonoBehaviour
{
    LineRenderer lr;
    Mesh bakedMesh;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        bakedMesh = new Mesh();
        bakedMesh.MarkDynamic();
    }

    void LateUpdate()
    {
        UpdateUV2();
    }

    public void UpdateUV2()
    {
        int count = lr.positionCount;
        if (count < 2) return;

        Vector3[] points = new Vector3[count];
        lr.GetPositions(points);

        float[] dist = new float[count];
        dist[0] = 0f;
        float total = 0f;

        for (int i = 1; i < count; i++)
        {
            total += Vector3.Distance(points[i], points[i - 1]);
            dist[i] = total;
        }

        for (int i = 0; i < count; i++)
            dist[i] /= total;

        // Bake mesh from LineRenderer
        bakedMesh.Clear();
        lr.BakeMesh(bakedMesh, true);

        int vCount = bakedMesh.vertexCount;
        Vector2[] uv2 = new Vector2[vCount];

        // Each point makes 2 vertices (top + bottom)
        for (int v = 0; v < vCount; v++)
        {
            int pointIndex = v / 2;
            uv2[v] = new Vector2(dist[pointIndex], 0);
        }

        bakedMesh.uv2 = uv2;

        // Assign the mesh
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter != null)
            filter.sharedMesh = bakedMesh;
    }
}