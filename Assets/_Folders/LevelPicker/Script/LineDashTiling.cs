using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineDashTiling : MonoBehaviour
{
    public float dashWorldLength = 1f;   // how long one dash should be in world units
    public bool updateEveryFrame = true; // or call UpdateTiling() manually

    LineRenderer lr;
    Material matInstance;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        // Use a material instance so we don't change the shared material for all lines
        matInstance = Instantiate(lr.sharedMaterial);
        lr.material = matInstance;
        lr.textureMode = LineTextureMode.Tile;
        UpdateTiling();
    }

    void Update()
    {
        if (updateEveryFrame)
        {
            UpdateTiling();
        }
    }

    public void UpdateTiling()
    {
        float length = 0f;

        int count = lr.positionCount;
        if (count < 2) return;

        Vector3 prev = lr.GetPosition(0);
        for (int i = 1; i < count; i++)
        {
            Vector3 p = lr.GetPosition(i);
            length += Vector3.Distance(prev, p);
            prev = p;
        }

        if (dashWorldLength <= 0.0001f) dashWorldLength = 0.1f;
        float repeats = length / dashWorldLength;

        // X is along the line, Y is across the line
        matInstance.mainTextureScale = new Vector2(repeats, 1f);
    }
}