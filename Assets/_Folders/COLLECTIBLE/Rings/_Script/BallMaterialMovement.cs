using UnityEngine;
using System.Collections;

public class BallMaterialMovement : MonoBehaviour
{
    public RingID ID;
    [SerializeField] private float uiCutoff;
    [SerializeField] private float playerCutoff;
    [SerializeField] private float explodeDuration;

    [SerializeField] private ParticleSystem ParticlePrefab;
    private SpriteRenderer sprite;
    [SerializeField] private Collider2D coll2D;
    private TrailRenderer trail;
    private bool hasCrossedBoundary;

    private Vector2 startingScale = new Vector2 (.5f, .4f);

    public float speed = 200f;
    public Vector2 startPosition;
    public Vector2 targetPosition;
    public GameObject targetObject;
    [SerializeField] private float minSpeed = 7f; 
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float slowingRadius = 5f;
    [SerializeField] private float arcHeight = 5f; // Height of the arc for the parabolic movement

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        Vector2 targetPos = targetObject != null ? targetObject.transform.position : targetPosition;
        float distance = Vector2.Distance(transform.position, targetPos);
        float highlightedAmount = distance < slowingRadius ? Mathf.Lerp(0f, 0.5f, 1 - (distance / slowingRadius)) : 0f;

        // Apply the highlighted amount to the material
       
            ID.highlightedMaterial.SetFloat("_HitEffectBlend", highlightedAmount);
        

        // Calculate dynamic speed based on distance to target
        float dynamicSpeed = distance < slowingRadius ? Mathf.Lerp(minSpeed, maxSpeed, distance / slowingRadius) : maxSpeed;

        if (targetObject != null)
        {
            // Linear movement towards a moving target (GameObject)
            MoveTowards(targetPos, dynamicSpeed);
        }
        else
        {
            // Arc movement towards a fixed position
            MoveWithArc(targetPos, dynamicSpeed);
        }
    }

    private void MoveTowards(Vector2 targetPos, float dynamicSpeed)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, dynamicSpeed * Time.deltaTime);

        // Deactivate when close to the target
        if (Vector2.Distance(transform.position, targetPos) < playerCutoff)
        {
            gameObject.SetActive(false);
        }
    }

    private void MoveWithArc(Vector2 targetPos, float dynamicSpeed)
    {
        float distanceToTarget = Vector2.Distance(transform.position, targetPos);
        Vector2 nextPosition = Vector2.MoveTowards(transform.position, targetPos, dynamicSpeed * Time.deltaTime);

        // Calculate vertical offset for the arc
        float arcRatio = Mathf.Sin(Mathf.PI * (1 - distanceToTarget / Vector2.Distance(startPosition, targetPos)));
        nextPosition.y += arcHeight * arcRatio;

        transform.position = nextPosition;

        // Deactivate the ball when it's close enough to the target position
        if (distanceToTarget < uiCutoff && !hasCrossedBoundary)
        {
            StartCoroutine(ExpandAndExplode());
            hasCrossedBoundary = true;

        }
    }

    private void OnEnable()
    {

        
        if (ID != null)
        {
            coll2D.enabled = ID.IDIndex == 0;
            sprite.material = ID.highlightedMaterial;
            trail.material = ID.highlightedMaterial;
        }
    }
    private void OnDisable()
    {

        transform.localScale = startingScale;
        hasCrossedBoundary = false;
        if (ID != null)
        {
            ID.highlightedMaterial.SetFloat("_HitEffectBlend", 0);
            ID.highlightedMaterial.SetFloat("_Alpha", 1);
        }
       
    }

    private IEnumerator ExpandAndExplode()
    {
        Vector2 startScale = transform.localScale; // Starting scale
        Vector2 endScale = new Vector3(1.2f, 1f, 1f); // Example end scale, adjust as needed
        float duration = .2f; // Duration of the lerp
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Lerp the scale
            transform.localScale = Vector2.Lerp(startScale, endScale,t);

            // Optionally, move the GameObject based on the _HitEffectBlend value
            float blend = Mathf.Lerp(0f, .5f, t); // Example blend value calculation
            ID.highlightedMaterial.SetFloat("_HitEffectBlend", blend);
            // Here, you could adjust the GameObject's position based on the blend value

            yield return null;
        }

        duration = .3f;
        time = 0;
        ParticlePrefab.Play();

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float blend = Mathf.Lerp(1f, 0f, t);

            ID.highlightedMaterial.SetFloat("_Alpha", blend);
            yield return null;
        }

        // Ensure the final values are set
        gameObject.SetActive(false);
        // transform.localScale = endScale;
        // ID.highlightedMaterial.SetFloat("_HitEffectBlend", f);
        // Set final position based on the blend value if needed
    }
}




