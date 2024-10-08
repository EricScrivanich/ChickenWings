using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPackPigMovement : MonoBehaviour
{
    public float speed;
    private float speedVar;
    private bool hasPlayedAudio;
    private float finishedTime;

    public int id;

    private int smokeID;

    [SerializeField] private Transform smokePoint;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * speedVar * Time.deltaTime);



        if (transform.position.x < -30 && speedVar != 0)
        {
            speedVar = 0;

        }
        if (speedVar == 0)
        {
            finishedTime += Time.deltaTime;
            if (finishedTime > 3f)
            {
                gameObject.SetActive(false);
            }

        }

        if (!hasPlayedAudio && Mathf.Abs(transform.position.x) < BoundariesManager.rightBoundary)
        {

            SmokeTrailPool.GetJetpackSmokeTrail?.Invoke(smokePoint.position, speed, id);
            AudioManager.instance.PlayPigJetPackSound();
            hasPlayedAudio = true;



        }
    }

    private void OnDisable()
    {
        if (Mathf.Abs(transform.position.x) < BoundariesManager.rightPlayerBoundary)
            SmokeTrailPool.OnDisableSmokeTrail?.Invoke(id);
    }

    private void OnEnable()
    {
        hasPlayedAudio = false;
        speedVar = speed;
        finishedTime = 0;

    }
}