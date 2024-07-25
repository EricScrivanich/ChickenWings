using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPackPigMovement : MonoBehaviour
{
    public float speed;
    private float speedVar;
    private bool hasPlayedAudio;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * speedVar * Time.deltaTime);

        if (transform.position.x < -30)
        {
            speedVar = 0;
        }

        if (!hasPlayedAudio)
        {
            if (speed > 0 && transform.position.x < BoundariesManager.rightBoundary)
            {
                AudioManager.instance.PlayPigJetPackSound();
                hasPlayedAudio = true;

            }

        }
    }

    private void OnEnable()
    {
        hasPlayedAudio = false;
        speedVar = speed;

    }
}