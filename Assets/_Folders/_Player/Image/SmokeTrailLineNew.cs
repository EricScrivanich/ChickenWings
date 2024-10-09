using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SmokeTrailLineNew : MonoBehaviour
{


    private float speed;
    private bool isStopped = false;


    private ParticleSystem ps;
    // private Vector2 addedXOnEndPoint = new Vector2(2f, 0,0);



    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

    }

    public void Initialize(Vector2 pos, float s)
    {

        transform.position = pos;
        ps.Play();

        isStopped = false;
        speed = s;
        gameObject.SetActive(true);


    }



    void Update()
    {
        if (!isStopped)
            transform.Translate(Vector2.left * speed * Time.deltaTime);
    }
    public void FadeOut()
    {
        if (ps != null)
            ps.Stop();


        speed = 0;

    }



    // private void UpdateLineRendererPositionsForJetpack()
    // {

    //     Vector3 point0 = lineRenderer.GetPosition(0);
    //     // if (point0.x <= -13)
    //     // {
    //     //     moveStart = false;
    //     //     // textureOffset = Vector2.zero;
    //     //     return;
    //     // }
    //     Vector3 point2 = lineRenderer.GetPosition(2);
    //     point2.x -= point1Speed * Time.deltaTime;
    //     lineRenderer.SetPosition(2, point2);

    //     point0.x -= point0Speed * Time.deltaTime;
    //     lineRenderer.SetPosition(0, point0);
    //     lineRenderer.SetPosition(1, point0 + point1Offset);

    //     if (Mathf.Abs(point0.x - point2.x) < 1.5f)
    //         gameObject.SetActive(false);

    //     // if (point2.x < BoundariesManager.leftBoundary)
    //     //     gameObject.SetActive(false);
    //     // textureOffset.x -= initialOffsetSpeed * Time.deltaTime;  // Update the texture offset

    // }


    // private void UpdateLineRendererPositionsForDelayed()
    // {


    //     if (!stopFront)
    //     {
    //         Vector3 point0 = lineRenderer.GetPosition(0);


    //         point0.x -= point0Speed * Time.deltaTime;
    //         lineRenderer.SetPosition(0, point0);
    //         lineRenderer.SetPosition(1, point0 + Vector3.right * sideLength);
    //         if (point0.x <= -13 - sideLength)
    //             stopFront = true;

    //     }



    //     if (cloudDelayTimer < gasCloudDelay)
    //     {
    //         cloudDelayTimer += Time.deltaTime;
    //     }
    //     else
    //     {
    //         Vector3 point2 = lineRenderer.GetPosition(2);
    //         Vector3 point3 = lineRenderer.GetPosition(3);
    //         point2.x -= point1Speed * Time.deltaTime;
    //         point3.x -= point1Speed * Time.deltaTime;

    //         lineRenderer.SetPosition(2, point2);
    //         lineRenderer.SetPosition(3, point3);






    //         if (point3.x < BoundariesManager.leftBoundary)
    //             gameObject.SetActive(false);

    //     }

    //     // if (point0.x <= -13)
    //     // {
    //     //     moveStart = false;
    //     //     // textureOffset = Vector2.zero;
    //     //     return;
    //     // }




    //     // textureOffset.x -= initialOffsetSpeed * Time.deltaTime;  // Update the texture offset

    // }




}



