using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : SpawnedObject, IRecordableObject
{

    [SerializeField] private Rigidbody2D fanRb;
  
    private SpriteRenderer sr;
    private bool hasSpawned;

    private HingeJoint2D joint;

    public int bladeAmount;
    public float bladeScaleMultiplier;
    public float bladeSpeed;
    public int startRot;
    [SerializeField] private Vector2 minMaxWindSoundDelay;
    [SerializeField] private Vector2 minMaxWindSpeed;
    private float windMillSoundDelay;
    private float windMillSoundTimer;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<HingeJoint2D>();
        hasSpawned = true;

    }

    private void Start()
    {

    }
    private void OnEnable()
    {
        if (hasSpawned)
        {
            AdjustHeightToGround();
            fanRb.angularVelocity = 0;
            fanRb.transform.eulerAngles = Vector3.forward * startRot;
            windMillSoundTimer = 0;
            rb.linearVelocity = Vector2.left * BoundariesManager.GroundSpeed;
            fanRb.transform.localScale = BoundariesManager.vectorThree1 * bladeScaleMultiplier;
            Move();
        }
        else hasSpawned = true;

    }
    private void AdjustHeightToGround()
    {
        if (sr == null)
        {
            Debug.LogError("SpriteRenderer not found on windmill.");
            return;
        }
        float desiredTotalHeight = Mathf.Abs(transform.position.y - BoundariesManager.GroundPosition + .14f);
        sr.size = new Vector2(sr.size.x, desiredTotalHeight);

    }
    void Move()
    {

        fanRb.angularVelocity = bladeSpeed;

        AudioManager.instance.ChangeWindMillPitch(Mathf.Lerp(minMaxWindSoundDelay.x, minMaxWindSoundDelay.y, (Mathf.Abs(bladeSpeed) - minMaxWindSpeed.x) / (minMaxWindSpeed.y - minMaxWindSpeed.x)));

        windMillSoundDelay = 360 / bladeAmount / Mathf.Abs(bladeSpeed);
    }



    // Update is called once per frame
    void FixedUpdate()
    {

        if (transform.position.x < BoundariesManager.leftBoundary - 2)
        {
            gameObject.SetActive(false);
        }
        else
        {
            windMillSoundTimer += Time.fixedDeltaTime;

            if (windMillSoundTimer >= windMillSoundDelay)
            {
                AudioManager.instance.PlayWindMillSound();
                windMillSoundTimer = 0;

            }

        }
    }


    
    public override void ApplyFloatThreeData(DataStructFloatThree data)
    {
        transform.position = data.startPos;
        bladeSpeed = data.float1;
        Vector3 scale = new Vector3(1, data.float2, 1);
        if (data.type == 0) bladeSpeed *= -1;
        startRot = Mathf.RoundToInt(data.float3);

        foreach (Transform child in fanRb.gameObject.transform)
        {
            child.localScale = scale;
        }

        AdjustHeightToGround();

        gameObject.SetActive(true);

    }
   

    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        bladeSpeed = data.float1;
        Vector3 scale = new Vector3(1, data.float2, 1);
        if (data.type == 0) bladeSpeed *= -1;
        startRot = Mathf.RoundToInt(data.float3);

        foreach (Transform child in fanRb.gameObject.transform)
        {
            child.localScale = scale;
        }

        AdjustHeightToGround();



    }

    public bool ShowLine()
    {
        return false;
    }

    public float TimeAtCreateObject(int index)
    {
        throw new System.NotImplementedException();
    }

    public Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset)
    {
        float rot = (bladeSpeed * time) + startRot;
        fanRb.transform.eulerAngles = Vector3.forward * rot;
        return new Vector2(currPos.x - (BoundariesManager.GroundSpeed * time), transform.position.y);
    }
    public float ReturnPhaseOffset(float x)
    {
        return 0;
    }
}
