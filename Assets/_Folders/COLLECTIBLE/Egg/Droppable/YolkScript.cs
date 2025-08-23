using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YolkScript : SpawnedQueuedObject
{
    [SerializeField] private Transform eggYolk;
    [SerializeField] private Transform eggWhite;

    [Header("Lerp Settings for Egg Yolk Scale")]
    [SerializeField] private Vector3 yolkTargetScale = new Vector3(1f, 1f, 1f);
    // Duration in seconds
    private float yolkScaleTime = 0f;

    [Header("Lerp Settings for Egg Yolk Position")]
    [SerializeField] private Vector2 yolkTargetPosition = new Vector2(0f, 0f);
    // Duration in seconds
    private float yolkPositionTime = 0f;

    [Header("Lerp Settings for Egg White Scale")]
    [SerializeField] private Vector3 whiteTargetScale = new Vector3(1f, 1f, 1f);
    // Duration in seconds
    private float whiteScaleTime = 0f;

    [SerializeField] private Vector3 yolkInitialScale;
    [SerializeField] private Vector2 yolkInitialPosition;
    [SerializeField] private Vector3 whiteInitialScale;

    private float lerpDuration = .1f;
    private float time;


    private void OnEnable()
    {
        time = 0;
        Ticker.OnTickAction015 += CheckTransform;
    }
    void OnDisable()
    {
        Ticker.OnTickAction015 -= CheckTransform;

        base.ReturnToPool();
    }

    void CheckTransform()
    {
        if (transform.position.x < BoundariesManager.leftBoundary)
        {
            gameObject.SetActive(false);

        }

    }

    void Update()
    {
        // Lerp egg yolk scale
        if (time < lerpDuration)
        {
            eggYolk.localScale = Vector3.Lerp(yolkInitialScale, yolkTargetScale, time / lerpDuration);
            eggYolk.localPosition = Vector2.Lerp(yolkInitialPosition, yolkTargetPosition, time / lerpDuration);
            eggWhite.localScale = Vector3.Lerp(whiteInitialScale, whiteTargetScale, time / lerpDuration);
            time += Time.deltaTime;
        }

        // Lerp egg yolk position


        // Move the entire game object to the left (assumed movement code)
        transform.Translate(Vector2.left * BoundariesManager.GroundSpeed * Time.deltaTime);
    }
}
