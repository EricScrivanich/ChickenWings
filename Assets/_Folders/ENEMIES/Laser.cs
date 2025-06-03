using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private PooledQueue particlePool;
    [SerializeField] private SpriteRenderer laserSprite;
    private BoxCollider2D laserCollider;
    private Transform laserGroundHit;

    [SerializeField] private float nothing;
    private bool useParticles = true;

    private LaserParent laserParent;







    private bool shooting;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        if (Time.timeScale == 0)
        {
            useParticles = false;
            return;
        }
        // SetLaserGroundHit();
        // SetLaserLength();
        // laserCollider.size = new Vector2(laserCollider.size.x, 0);
        // laserCollider.offset = new Vector2(laserCollider.offset.x, 0);
        // laserGroundHit.gameObject.SetActive(false);

    }

    public void SetLaserParent(LaserParent parent)
    {
        laserParent = parent;
    }
    private void Awake()
    {
        laserCollider = laserSprite.gameObject.GetComponent<BoxCollider2D>();
        StopParticles();
    }

    // private void OnValidate()
    // {
    //     Debug.Log(Time.timeScale);
    //     if (Time.timeScale == 0)
    //     {
    //         Debug.LogError("TimeScale is 0");
    //         Time.timeScale = 1;
    //     }

    // }

    public void SetLaserMaterial(Material mat)
    {
        Debug.Log("Setting laser material");
        laserSprite.material = mat;
    }

    public void HandleLaserCollider(bool isActive)
    {
        laserCollider.enabled = isActive;

    }
    private void OnEnable()
    {
        laserCollider.enabled = false;
    }

    public void HandleShooting(bool isShooting)
    {
        if (isShooting)
        {
            HandleLaserCollider(true);
            shooting = true;
            // laserSprite.gameObject.SetActive(true);

        }
        else
        {
            // laserSprite.gameObject.SetActive(false);
            HandleLaserCollider(false);

            shooting = false;
            if (laserGroundHit != null)
            {
                laserGroundHit.GetComponent<LaserParticleScript>().StopParicles();

            }
        }

    }

    public void StopParticles()
    {
        useParticles = false;
        if (laserGroundHit != null)
        {


            particlePool.ReturnPooledObject(laserGroundHit.gameObject);
            laserGroundHit = null;


        }

    }

    public void SetUseParticles(bool use)
    {
        useParticles = use;


    }

    public void SetLaserScale(float scale)
    {
        laserSprite.transform.localScale = new Vector3(scale, 1, 1);

    }

    public void SetLaserGroundHit()
    {
        if (!useParticles) return;
        Vector2 start = laserSprite.transform.position;




        // Convert rotation to direction (0° = down)
        float angle = laserSprite.transform.eulerAngles.z;
        Vector2 direction = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), -Mathf.Cos(angle * Mathf.Deg2Rad)).normalized;

        float groundY = BoundariesManager.GroundPosition - 0.65f;

        // Only continue if pointing downward
        if (direction.y >= 0f)
        {
            if (laserGroundHit != null)
            {

                particlePool.ReturnPooledObject(laserGroundHit.gameObject);
                laserGroundHit = null;
            }

            return;
        }

        // Calculate t where the line hits ground: start.y + t * direction.y = groundY
        float t = (groundY - start.y) / direction.y;

        // If t is negative, it's behind us, so ignore
        if (t < 0)
        {
            if (laserGroundHit != null)
            {


                particlePool.ReturnPooledObject(laserGroundHit.gameObject);
                laserGroundHit = null;
            }

            return;
        }

        // Compute hit point
        Vector2 hitPoint = start + direction * t;

        // Activate and place the particle system
        if (laserGroundHit == null)
        {

            laserGroundHit = particlePool.GetPooledObject().transform;
            laserGroundHit.gameObject.SetActive(true);
            laserGroundHit.GetComponent<LaserParticleScript>().SetLaserFadeAmount(laserParent.currentLaserFadeAmount, laserParent.currentTimeLeft);

        }


        laserGroundHit.position = new Vector2(hitPoint.x, groundY);

        // laserGroundHit.eulerAngles = Vector3.zero;
    }


}
