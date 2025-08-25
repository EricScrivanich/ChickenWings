using UnityEngine;

public class ShotgunBlastManualParticle : MonoBehaviour
{

    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private ShotgunParticleID particleID;
    [SerializeField] private float angle;
    [SerializeField] private int bulletCount;

    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SerializeField] private float startScale;
    [SerializeField] private float endScale;
    private Rigidbody2D[] particleRBs;

    [SerializeField] private float outerRadius;
    [SerializeField] private float outerRadiusArc;
    [SerializeField] private float outerRadiusAngleDamp;
    [SerializeField] private float innerRadius;
    [SerializeField] private float innerRadiusArc;
    [SerializeField] private float innerRadiusAngleDamp;
    [SerializeField] private int outerRadiusCount;

    [SerializeField] private bool shoot;
    [SerializeField] private bool reset;
    private float timer = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particleRBs = new Rigidbody2D[bulletCount];
        particleID.SetTransformData();
        for (int i = 0; i < bulletCount; i++)
        {
            GameObject obj = Instantiate(particlePrefab, this.transform);
            particleRBs[i] = obj.GetComponent<Rigidbody2D>();
            // obj.SetActive(false);


        }


        ResetParticlePositions();




    }

    void OnValidate()
    {
        if (shoot)
        {
            this.enabled = true;
            shoot = false;

            ShootParicles();
        }
        if (reset)
        {
            timer = 0;
            this.enabled = false;
            reset = false;
            ResetParticlePositions();
        }
    }
    private void ResetParticlePositions()
    {
        // make it so we place partciles in a semi circle in front of postion based on angle
        // first do outer radius make sure it is centered based on radius and angle,
        //then set rbs rotation to be facing outwards from center
        // then do inner radius the same way
        for (int i = 0; i < particleID.positionData.Length; i++)
        {


            particleRBs[i].transform.SetPositionAndRotation(particleID.positionData[i], Quaternion.Euler(0f, 0f, particleID.rotationData[i]));
            particleRBs[i].transform.localScale = Vector3.one * startScale;
            particleRBs[i].linearVelocity = Vector2.zero;
            particleRBs[i].gameObject.SetActive(false);
        }




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
            Vector3 localPos = new Vector3(Mathf.Cos(angRad) * radius,
                                           Mathf.Sin(angRad) * radius,
                                           0f);

            var rb = particleRBs[i + addedIndex];
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;


            // Set local transform (don’t care about world/physics here)
            rb.transform.localPosition = localPos;
            rb.transform.localScale = Vector3.one * startScale;

            // Point “outward” along the arc (z-rotation = angDeg)
            rb.transform.localRotation = Quaternion.Euler(0f, 0f, angDeg * damp);

            // Optional: zero physics state if you’ll activate them later

        }
    }

    private void ShootParicles()
    {
        for (int i = 0; i < particleRBs.Length; i++)
        {
            var rb = particleRBs[i];
            rb.gameObject.SetActive(true);


            // Activate physics


            // Apply force in the direction the particle is facing
            Vector2 forceDir = rb.transform.right; // local +X axis
            if (i >= outerRadiusCount) rb.linearVelocity = forceDir * particleID.speed * .9f;
            else
                rb.linearVelocity = forceDir * particleID.speed;

            // Schedule deactivation

        }

    }

    // Update is called once per frame
    // void FixedUpdate()
    // {
    //     timer += Time.fixedDeltaTime;
    //     float scale = Mathf.Lerp(startScale, endScale, timer / lifeTime);
    //     for (int i = 0; i < particleRBs.Length; i++)
    //     {

    //         particleRBs[i].transform.localScale = Vector3.one * scale;
    //     }
    //     if (timer > lifeTime)
    //     {
    //         timer = 0;
    //         reset = false;
    //         SetPostionsAndRoations(false);
    //         SetPostionsAndRoations(true);
    //         this.enabled = false;
    //     }

    // }
}
