using System;
using UnityEngine;

public class BackGroundObject : MonoBehaviour
{

    [SerializeField] private string animationToTrigger;

    [SerializeField] private Vector2 speedsByDistance;
    [SerializeField] private Vector2 scalesByDistance;
    [SerializeField] private Vector2 magnitudesByDistance;
    [SerializeField] private Vector2 freqsByDistance;
    [SerializeField] private Vector2 animTimeByDistance;
    [SerializeField] private float animationSpeed = 1;
    private Animator anim;
    private float animTime;
    private float timer;

    private float speed;
    private float sineMagnitude;
    private float sineFrequency;
    private float maxDistanceToTravel;
    private float startX;
    private bool doAnimCheck = false;

    private BackgroundObjects bgParent;

    [SerializeField] private bool doColor;
    [SerializeField] private bool doMat;
    [SerializeField] private Color color;

    [SerializeField] private Material mat;

    public void ApplyMaterialToAllSprites(Material mat, Transform parent = null, BackgroundObjects p = null)
    {
        // iterate over all children and app
        if (p != null)
        {
            bgParent = p;
        }
        if (parent == null)
        {
            parent = transform;
            if (GetComponent<SpriteRenderer>() != null)
            {

                if (mat != null)
                {
                    GetComponent<SpriteRenderer>().material = mat;
                }
            }


        }
        foreach (Transform child in parent)
        {
            if (child.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
            {
                sr.material = mat;
            }
            if (child.childCount > 0)
            {
                ApplyMaterialToAllSprites(mat, child);
            }

        }
    }


    public void ApplyColorToAllSprites(Color color, Transform parent = null, BackgroundObjects p = null)
    {
        // iterate over all children and app
        if (p != null)
        {
            bgParent = p;
        }
        if (parent == null)
        {
            parent = transform;
            if (GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().color = color;
                if (mat != null)
                {
                    GetComponent<SpriteRenderer>().material = mat;
                }
            }


        }
        foreach (Transform child in parent)
        {

            if (child.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
            {
                sr.color = color;
                if (mat != null)
                {
                    sr.material = mat;
                }
            }
            if (child.childCount > 0)
            {
                ApplyColorToAllSprites(color, child);
            }

        }
    }
    void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim != null)
            anim.speed = animationSpeed;
        if (string.IsNullOrEmpty(animationToTrigger))
        {
            doAnimCheck = false;
        }
        else
        {
            doAnimCheck = true;
        }


    }
    void Start()
    {
        if (doColor)
        {
            ApplyColorToAllSprites(color);
            this.enabled = false;

        }
        else if (doMat)
        {
            ApplyMaterialToAllSprites(mat);
            this.enabled = false;
        }


    }



    public void Initialize(Vector2 pos, float distance)
    {
        transform.position = pos;
        startX = pos.x;
        // maxDistanceToTravel = maxDist;

        speed = Mathf.Lerp(speedsByDistance.x, speedsByDistance.y, distance);
        float scale = Mathf.Lerp(scalesByDistance.x, scalesByDistance.y, distance);
        sineMagnitude = Mathf.Lerp(magnitudesByDistance.x, magnitudesByDistance.y, distance);
        sineFrequency = Mathf.Lerp(freqsByDistance.x, freqsByDistance.y, distance);
        animTime = Mathf.Lerp(animTimeByDistance.x, animTimeByDistance.y, distance);
        transform.localScale = Vector3.one * scale;
        gameObject.SetActive(true);


    }

    public bool CheckForDespawn(float x)
    {
        if (gameObject.activeSelf == false) return false;
        else if (transform.position.x < x)
        {
            gameObject.SetActive(false);
            return true;

        }
        else return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (doAnimCheck)
        {
            timer += Time.deltaTime;
            if (timer >= animTime)
            {
                timer = 0;
                anim.SetTrigger(animationToTrigger);

            }
        }

        float period = Mathf.Sin((transform.position.x - startX) * sineFrequency) * sineMagnitude;
        transform.position = new Vector2(transform.position.x - speed * Time.deltaTime * bgParent.baseSpeedMultiplier, transform.position.y + period * sineMagnitude);
        // if (transform.position.x < -maxDistanceToTravel)
        // {
        //     gameObject.SetActive(false);

        // }



    }
}
