using UnityEngine;

public class BackGroundObject : MonoBehaviour
{

    [SerializeField] private string animationToTrigger;

    [SerializeField] private Vector2 speedsByDistance;
    [SerializeField] private Vector2 scalesByDistance;
    [SerializeField] private Vector2 magnitudesByDistance;
    [SerializeField] private Vector2 freqsByDistance;
    [SerializeField] private Vector2 animTimeByDistance;
    private Animator anim;
    private float animTime;
    private float timer;

    private float speed;
    private float sineMagnitude;
    private float sineFrequency;
    private float maxDistanceToTravel;
    private float startX;


    public void ApplyMaterialToAllSprites(Material mat, Transform parent = null)
    {
        // iterate over all children and app

        if (parent == null)
        {
            parent = transform;

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
    void Awake()
    {
        anim = GetComponent<Animator>();

    }



    public void Initialize(Vector2 pos, float distance, float maxDist)
    {
        transform.position = pos;
        startX = pos.x;
        maxDistanceToTravel = maxDist;

        speed = Mathf.Lerp(speedsByDistance.x, speedsByDistance.y, distance);
        float scale = Mathf.Lerp(scalesByDistance.x, scalesByDistance.y, distance);
        sineMagnitude = Mathf.Lerp(magnitudesByDistance.x, magnitudesByDistance.y, distance);
        sineFrequency = Mathf.Lerp(freqsByDistance.x, freqsByDistance.y, distance);
        animTime = Mathf.Lerp(animTimeByDistance.x, animTimeByDistance.y, distance);
        transform.localScale = Vector3.one * scale;
        gameObject.SetActive(true);


    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= animTime)
        {
            timer = 0;
            anim.SetTrigger(animationToTrigger);

        }
        float period = Mathf.Sin((transform.position.x - startX) * sineFrequency) * sineMagnitude;
        transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y + period * sineMagnitude);
        if (transform.position.x < -maxDistanceToTravel)
        {
            gameObject.SetActive(false);

        }



    }
}
