using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrailLineNew : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public float point0Speed;
    private float point1Speed = 7;

    public GameObject particles;

    private Material smokeMaterial;
    private bool moveMask = true;

    private bool moveStart = true;

    private float lerpTimer;
    private float lerpDuration;
    private float cloudDelayTimer;

    private Vector3 point1Offset = new Vector3(2, 0, 0);

    [SerializeField] private float sideLength;
    [SerializeField] private float addedRightLength;
    [SerializeField] private float middLength = 24;
    public float gasCloudDelay;
    private bool usingGasCloud;
    private bool stopFront;

    // private Vector2 addedXOnEndPoint = new Vector2(2f, 0,0);

    void Start()
    {
        // lineRenderer.material = new Material(lineRenderer.material);
        // smokeMaterial = lineRenderer.material;
        // lerpDuration = Mathf.Abs(lineRenderer.GetPosition(0).x + 13) / point0Speed;
        // textureOffset = Vector2.zero;
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

    }
    private void OnEnable()
    {
        Initialize(transform.position, point0Speed);
    }

    public void Initialize(Vector2 pos, float speed)
    {
        usingGasCloud = gasCloudDelay > 0;
        lineRenderer.SetPosition(0, new Vector3(pos.x, pos.y, 0));



        if (usingGasCloud)
        {
            lineRenderer.SetPosition(1, new Vector3(pos.x + sideLength, pos.y, 0));
            lineRenderer.SetPosition(2, new Vector3(pos.x + middLength + sideLength, pos.y, 0));
            lineRenderer.SetPosition(3, new Vector3(pos.x + sideLength + middLength + addedRightLength, pos.y, 0));


        }
        else
        {
            lineRenderer.SetPosition(1, new Vector3(pos.x, pos.y, 0) + point1Offset);
            lineRenderer.SetPosition(2, new Vector3(pos.x + 2.5f, pos.y, 0));

        }

        point0Speed = speed;
        gameObject.SetActive(true);

    }

    void Update()
    {
        if (usingGasCloud)
        {

            UpdateLineRendererPositionsForDelayed();
        }
        else
            UpdateLineRendererPositionsForJetpack();

    }

    private void UpdateLineRendererPositionsForDelayed()
    {


        if (!stopFront)
        {
            Vector3 point0 = lineRenderer.GetPosition(0);


            point0.x -= point0Speed * Time.deltaTime;
            lineRenderer.SetPosition(0, point0);
            lineRenderer.SetPosition(1, point0 + Vector3.right * sideLength);
            if (point0.x <= -13 - sideLength)
                stopFront = true;

        }



        if (cloudDelayTimer < gasCloudDelay)
        {
            cloudDelayTimer += Time.deltaTime;
        }
        else
        {
            Vector3 point2 = lineRenderer.GetPosition(2);
            Vector3 point3 = lineRenderer.GetPosition(3);
            point2.x -= point1Speed * Time.deltaTime;
            point3.x -= point1Speed * Time.deltaTime;

            lineRenderer.SetPosition(2, point2);
            lineRenderer.SetPosition(3, point3);






            if (point3.x < BoundariesManager.leftBoundary)
                gameObject.SetActive(false);

        }

        // if (point0.x <= -13)
        // {
        //     moveStart = false;
        //     // textureOffset = Vector2.zero;
        //     return;
        // }




        // textureOffset.x -= initialOffsetSpeed * Time.deltaTime;  // Update the texture offset

    }


    private void UpdateLineRendererPositionsForJetpack()
    {

        Vector3 point0 = lineRenderer.GetPosition(0);
        // if (point0.x <= -13)
        // {
        //     moveStart = false;
        //     // textureOffset = Vector2.zero;
        //     return;
        // }
        Vector3 point2 = lineRenderer.GetPosition(2);
        point2.x -= point1Speed * Time.deltaTime;
        lineRenderer.SetPosition(2, point2);

        point0.x -= point0Speed * Time.deltaTime;
        lineRenderer.SetPosition(0, point0);
        lineRenderer.SetPosition(1, point0 + point1Offset);


        if (point2.x < BoundariesManager.leftBoundary)
            gameObject.SetActive(false);
        // textureOffset.x -= initialOffsetSpeed * Time.deltaTime;  // Update the texture offset

    }


}



