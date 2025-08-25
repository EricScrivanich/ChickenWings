using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunParticleID", menuName = "ScriptableObjects/ShotgunParticleID")]
public class ShotgunParticleID : ScriptableObject
{
    public Sprite[] sprites;
    public Vector2[] positionData { get; private set; }
    public float[] rotationData { get; private set; }
    [SerializeField] private int bulletCount;
    public float startScale = 1;
    public float endScale = 1;
    public float speed;

    public Color startColor;
    public Color endColor;
    public float lifeTime = 1;


    [SerializeField] private float outerRadius;
    [SerializeField] private float outerRadiusArc;
    [SerializeField] private float outerRadiusAngleDamp;
    [SerializeField] private float innerRadius;
    [SerializeField] private float innerRadiusArc;
    [SerializeField] private float innerRadiusAngleDamp;
    [SerializeField] private int outerRadiusCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetTransformData()
    {
        positionData = new Vector2[bulletCount];
        rotationData = new float[bulletCount];
        SetPostionsAndRoations(false);
        SetPostionsAndRoations(true);
    }

    private void SetPostionsAndRoations(bool inner)
    {
        int count = outerRadiusCount;
        // semicircle to the right: -90..+90 around +X
        float start = -outerRadiusArc;
        float end = outerRadiusArc;
        int addedIndex = 0;
        float damp = outerRadiusAngleDamp;
        float radius = outerRadius;



        if (inner)
        {
            count = bulletCount - outerRadiusCount;
            start = -innerRadiusArc;
            end = innerRadiusArc;
            damp = innerRadiusAngleDamp;
            addedIndex = outerRadiusCount;
            radius = innerRadius;

        }

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0.5f : (float)i / (count - 1); // even spread, single goes at center (0°)
            float angDeg = Mathf.Lerp(start, end, t);
            float angRad = angDeg * Mathf.Deg2Rad;

            // Local-space position on the arc (centered at parent's origin)
            Vector2 localPos = new Vector3(Mathf.Cos(angRad) * radius, Mathf.Sin(angRad) * radius);


            positionData[i + addedIndex] = localPos;
            rotationData[i + addedIndex] = angDeg * damp;


        }
    }

    // Update is called once per frame

}
