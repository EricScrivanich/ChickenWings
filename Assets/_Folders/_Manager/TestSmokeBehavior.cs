using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TestSmokeBehavior : MonoBehaviour
{
    private SpriteRenderer sr;
    public float speed;

    private Vector2 startPos;

    [SerializeField] private float lerpScaleDuration;

    private Material smokeMaterial;

    [SerializeField] private Sprite baseSprite;

    [SerializeField] private float initialTextureScrollSpeed;
    [SerializeField] private float baseTextureScrollSpeed;
    [SerializeField] private float endTextureScrollSpeed;

    [SerializeField] private float delayToFinish;


    public LineRenderer lineRenderer;
    public GameObject followObject;

    public Vector2 point0StartPos;
    public Vector2 point1StartPos;
    public float point0Speed = 2f;
    public float point1Speed = 1f;
    public float point2Speed = 1f;

    public float tileLerpSpeed;
    public Vector2 tileSizeRange;

    private float lerpTimer = 0;
    private float delayTimer = 0;
    private bool moveEndPoint = false;

    [SerializeField] private float moveEndDelay;

    private bool moveInitial = true;
    private bool moveBase;
    private bool moveEnd;





    // Start is called before the first frame update
    // void Start()
    // {

    //     // sr = GetComponent<SpriteRenderer>();
    //     // startPos = transform.position;


    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     // float addedSize = sr.size.x + (speed * Time.deltaTime);


    //     // // transform.Translate(Vector2.left * speed * Time.deltaTime);
    //     // // float desiredTotalHeight = Mathf.Abs(transform.position.x - startPos.x);
    //     // // sr.size = new Vector2(desiredTotalHeight * 1.3f, sr.size.y);

    //     // sr.size = new Vector2(addedSize, sr.size.y);

    // }


    void Start()
    {
        // Ensure the line renderer uses its own material instance
        lineRenderer.material = new Material(lineRenderer.material);

        // Initialize the line renderer positions
        lineRenderer.positionCount = 3;

        smokeMaterial = lineRenderer.material;

        smokeMaterial.SetFloat("_TextureScrollXSpeed", initialTextureScrollSpeed);
        // lineRenderer.SetPosition(0, point0StartPos);
        // lineRenderer.SetPosition(1, point1StartPos);
    }

    void Update()
    {
        // Move the points horizontally at different speeds
        MovePoints();

        // Lerp the texture tiling on the material

    }

    private void MovePoints()
    {
        if (moveEnd)
        {
            Vector3 point2 = lineRenderer.GetPosition(2);
            point2.x -= point2Speed * Time.deltaTime;

            lineRenderer.SetPosition(2, point2);

            if (point2.x < BoundariesManager.leftBoundary)
            {
                gameObject.SetActive(false);
            }
        }
        else if (moveBase)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer >= moveEndDelay)
            {
                moveBase = false;
                smokeMaterial.SetFloat("_TextureScrollXSpeed", endTextureScrollSpeed);
                moveEnd = true;
                return;

            }
        }

        else if (moveInitial)
        {
            Vector3 point0 = lineRenderer.GetPosition(0);
            Vector3 point1 = lineRenderer.GetPosition(1);

            if (point1.x < BoundariesManager.leftBoundary)
            {
                moveInitial = false;
                smokeMaterial.SetFloat("_TextureScrollXSpeed", baseTextureScrollSpeed);
                moveBase = true;
                return;
            }



            point0.x -= point0Speed * Time.deltaTime;
            point1.x -= point1Speed * Time.deltaTime;

            lineRenderer.SetPosition(0, point0);

            lineRenderer.SetPosition(1, point1);


        }





        // Update the GameObject's position to follow the first point

    }

    // private void UpdateTextureTiling()
    // {
    //     if (lerpTimer < lerpScaleDuration)
    //     {
    //         lerpTimer += Time.deltaTime;
    //         float newScale = Mathf.Lerp(tileSizeRange.x, tileSizeRange.y, lerpTimer / lerpScaleDuration);
    //         lineRenderer.textureScale = new Vector2(newScale, lineRenderer.textureScale.y);

    //     }


    // }
}
