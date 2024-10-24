using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPackPigMovement : MonoBehaviour
{
    public float speed;
    private bool hasPlayedAudio;
    private float finishedTime;

    private bool hasInitialized = false;

    [HideInInspector]
    public int id;

    [SerializeField] private Transform smokePoint;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);



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
}