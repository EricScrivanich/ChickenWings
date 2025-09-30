using UnityEngine;
using System.Collections;


public class FireballParticle : SpawnedQueuedObject, IExplodable
{
    [SerializeField] private AnimationDataSO animData;

    [ExposedScriptableObject]
    [SerializeField] private AnimationCurveSO curve;
    private float timer;
    private ushort index;
    private SpriteRenderer sr;
    private readonly Vector2 minMaxGravity = new Vector2(-.15f, .2f);
    private readonly float gravityTime = 2f;
    private bool hit;
    private readonly Vector3 startScale = new Vector3(.6f, 2.2f, 1.9f);
    private readonly Vector3 endScale = new Vector3(1.9f, 1.9f, 1.9f);

    private readonly WaitForSeconds wait = new WaitForSeconds(.08f);
    private readonly WaitForFixedUpdate fixedWait = new WaitForFixedUpdate();
    [SerializeField] private ParticleSystem ps;


    public void Explode(bool isGround)
    {
        if (hit) return;
        rb.simulated = false;
        hit = true;
        ps.Stop();

        StartCoroutine(ExplodeRoutine());


    }
    public override void OnSpawnLogic()
    {
        sr.color = Color.white;
        sr.enabled = true;
        if (hit)
        {
            hit = false;
            rb.simulated = true;
        }


        StartCoroutine(ChangeSize());
    }
    private IEnumerator ChangeSize()
    {
        Debug.Log("Changing size");

        float timer = 0;

        float time = .3f;

        while (timer < time)
        {
            timer += Time.fixedDeltaTime;
            // transform.localScale = Vector3.LerpUnclamped(startScale, endScale, curve.ReturnValue(timer / time));
            rb.gravityScale = Mathf.Lerp(minMaxGravity.x, minMaxGravity.y, curve.ReturnValue(timer / time));
            yield return fixedWait;

        }



    }
    private bool isMaxSize;
    private bool isMaxGravity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

    }

    private IEnumerator ExplodeRoutine()
    {
        for (int i = 0; i < 3; i++)
        {
            transform.localScale *= 1.2f;
            sr.sprite = animData.sprites[i + 8];
            yield return wait;
        }
        transform.localScale *= 1.2f;
        sr.color = Color.white * .4f;
        yield return wait;
        sr.enabled = false;

    }
    private float lifetime = 0;
    void FixedUpdate()
    {
        lifetime += Time.fixedDeltaTime;
        // if (!isMaxSize)
        // {
        //     transform.localScale = Vector3.LerpUnclamped(startScale, endScale, curve.ReturnValue(lifetime / .55f));
        //     if (lifetime >= .55f)
        //     {
        //         isMaxSize = true;
        //         transform.localScale = endScale;
        //     }

        // }
        // if (!isMaxGravity)
        // {
        //     rb.gravityScale = Mathf.Lerp(minMaxGravity.x, minMaxGravity.y, curve.ReturnValue(lifetime / gravityTime));
        //     if (lifetime >= gravityTime)
        //     {
        //         isMaxGravity = true;
        //         rb.gravityScale = minMaxGravity.y;
        //     }
        // }

    }

    private bool isFinalScale;

    // Update is called once per frame
    void Update()
    {
        if (hit) return;

        timer += Time.deltaTime;



        if (timer >= animData.constantSwitchTime)
        {
            timer = 0;
            sr.sprite = animData.sprites[index];
            index++;
            if (index >= 10)
                index = 2;

        }



    }

    void OnDisable()
    {

        // transform.localScale = startScale;
        lifetime = 0;
        timer = -.12f;
        index = 0;
        sr.sprite = animData.sprites[0];


        ReturnToPool();
    }

}
