using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPackPigMovement : MonoBehaviour, IRecordableObject
{
    public float speed;
    private bool hasPlayedAudio;
    private float finishedTime;

    private bool hasInitialized = false;

    [HideInInspector]
    public int id;

    private Rigidbody2D rb;

    [SerializeField] private Transform smokePoint;
    // Start is called before the first frame update

    // Update is called once per frame
    // void Update()
    // {
    //     transform.Translate(Vector2.left * speed * Time.deltaTime);



    //     if (!hasPlayedAudio && Mathf.Abs(transform.position.x) < BoundariesManager.rightBoundary)
    //     {

    //         SmokeTrailPool.GetJetpackSmokeTrail?.Invoke(smokePoint.position, speed, id);
    //         AudioManager.instance.PlayPigJetPackSound();
    //         hasPlayedAudio = true;

    //     }
    // }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {

        rb.MovePosition(rb.position + Vector2.left * speed * Time.fixedDeltaTime);
        if (!hasPlayedAudio && Mathf.Abs(transform.position.x) < BoundariesManager.rightBoundary)
        {

            SmokeTrailPool.GetJetpackSmokeTrail?.Invoke(smokePoint.position, speed, id);
            AudioManager.instance.PlayPigJetPackSound();
            hasPlayedAudio = true;

        }

    }

    private void OnDisable()
    {
        if (!hasInitialized)
        {
            hasInitialized = true;
        }
        else if (Mathf.Abs(transform.position.x) < BoundariesManager.rightPlayerBoundary)
            SmokeTrailPool.OnDisableSmokeTrail?.Invoke(id);
    }

    private void OnEnable()
    {
        hasPlayedAudio = false;
        finishedTime = 0;

    }


    public void ApplyRecordedData(RecordedDataStruct data)
    {
        transform.position = data.startPos;
        speed = data.speed;
        int scaleFlip = 1;
        float addedYScale = 0;
        if (data.scale > 1) addedYScale = (data.scale - 1) * 1.1f;
        if (speed < 0) scaleFlip = -1;

        transform.localScale = new Vector3(data.scale * scaleFlip * .9f, (.9f * data.scale) + addedYScale, 1);
        gameObject.SetActive(true);

        // if (speed < 0) transform.localScale = BoundariesManager.FlippedXScale * (data.scale * .9f);
        // else
        //     transform.localScale = Vector3.one * (data.scale * .9f);
    }

    public void     ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        speed = data.speed;
        int scaleFlip = 1;
        float addedYScale = 0;
        if (data.scale > 1) addedYScale = (data.scale - 1) * 1.1f;
        if (speed < 0) scaleFlip = -1;

        Debug.LogError("Applying custom data, speed is: " + speed);

        transform.localScale = new Vector3(data.scale * scaleFlip * .9f, (.9f * data.scale) + addedYScale, 1);
    }



    public Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset)
    {
        return new Vector2(currPos.x + (-speed * time), currPos.y);
    }

    public bool ShowLine()
    {
        return true;
    }

    public float ReturnPhaseOffset(float x)
    {
        return 0;
    }
    public float TimeAtCreateObject(int index)
    {
        return 0;
    }
}