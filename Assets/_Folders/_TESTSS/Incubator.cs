using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Incubator : SpawnedObject, IRecordableObject
{
#if UNITY_EDITOR

    [SerializeField] private bool testing;
    [SerializeField] private float testMainDur;
    [SerializeField] private float testShowDur;


#endif
    [SerializeField] private SpriteRenderer backBlur;
    [SerializeField] private SpriteRenderer frontBlur;
    [SerializeField] private Lightning lightning;


    private readonly Color startBackBlurColor = new Color(.98f, .98f, .6f, .72f);
    private readonly Color endBackBlurColor = new Color(.94f, .945f, .39f, 1);
    private Sequence showSeq;
    private float downSpeed;
    [SerializeField] private SpriteRenderer sr;

    [SerializeField] private LineRenderer line;
    private Transform incubator;
    [SerializeField] private GameObject incubatorEgg;
    [SerializeField] private PlayerID player;
    float xSize;
    [SerializeField] private float targetHeight;
    [SerializeField] private float intialtDur;
    [SerializeField] private float finalDur;
    public float mainDuration;
    private float time;
    private float startY;
    private float endY;
    private BoxCollider2D coll;
    float xSpeed = 0;
    private bool moveObject = false;
    [SerializeField] private LaserParticleScript particle;


    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        xSpeed = data.float1;
        mainDuration = data.float2;

    }
    public override void ApplyFloatTwoData(DataStructFloatTwo data)
    {
        // transform.position = data.startPos;

        mainDuration = data.float2;
        startY = data.startPos.y;

        // gameObject.SetActive(true);

        incubator = Instantiate(incubatorEgg, transform.position, Quaternion.identity).transform;
        xSpeed = data.float1;
        ShowSequence();

        // StartCoroutine(DoSpriteHeight());
    }
    private void ShowSequence()
    {
        frontBlur.color = startBackBlurColor;
        particle.SetLaserFadeAmount(0, intialtDur + finalDur);
        // sr.enabled = false;

        // StartCoroutine(DoSpriteHeight());
        showSeq = DOTween.Sequence();
        // showSeq.Append(transform.DOScaleX(.5f, heightDur).From(.2f));
        showSeq.Append(frontBlur.transform.DOScaleX(3.9f, intialtDur).From(1));
        showSeq.Join(frontBlur.DOFade(.72f, intialtDur).From(0));

        // showSeq.Join(backBlur.DOColor(endBackBlurColor, heightDur).From(startBackBlurColor));
        showSeq.Play().OnComplete(CompleteSeq);

    }
    private void CompleteSeq()
    {
        // GetComponent<BoxCollider2D>().enabled = true;

        StartCoroutine(DoSpriteHeight(finalDur));
        showSeq = DOTween.Sequence();
        showSeq.Append(sr.transform.DOScaleX(.5f, finalDur));
        showSeq.Join(backBlur.DOFade(.72f, finalDur));
        showSeq.Join(frontBlur.DOColor(endBackBlurColor, finalDur));
        showSeq.Join(sr.DOFade(1f, intialtDur).From(0));
        showSeq.Play().SetUpdate(UpdateType.Fixed);

        // sr.size = new Vector2(1.1f, targetHeight);
        // sr.enabled = true;

    }

    public Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset)
    {
        if (incubator == null)
        {
            startY = currPos.y;
            sr.transform.localScale = new Vector3(.6f, 1, 1);
            incubator = Instantiate(incubatorEgg, transform.position, Quaternion.identity, transform).transform;
            Vector3 lossyScale = incubator.lossyScale;
            incubator.localScale = new Vector3(1 / lossyScale.x, 1, 1);
        }
        float time2 = time - ((intialtDur + finalDur) / FrameRateManager.BaseTimeScale);
        if (time2 < 0) time2 = 0;
        incubator.position = new Vector2(transform.position.x, Mathf.Lerp(startY, endY, time2 / mainDuration));
        return new Vector2(currPos.x - (xSpeed * time), currPos.y);
    }

    public float ReturnPhaseOffset(float x)
    {
        if (incubator == null)
        {
            return 0;
        }
        if (incubator.position.y <= endY)
        {
            Debug.Log("Deactivating Incubatror from offset check: " + incubator.position.y);
            return -1;
        }

        else return 0;
    }

    public bool ShowLine()
    {
        return false;
    }

    public float TimeAtCreateObject(int index)
    {
        throw new System.NotImplementedException();
    }
    private IEnumerator DoSpriteHeight(float dur)
    {
        float time = 0;

        while (time < dur)
        {
            time += Time.deltaTime;
            sr.size = new Vector2(Mathf.Lerp(.4f, 1, time / dur), 12);

            yield return null;
        }
        sr.size = new Vector2(1.1f, targetHeight);
        moveObject = true;
        GetComponent<BoxCollider2D>().enabled = true;
    }
    private WaitForSeconds flash = new WaitForSeconds(.1f);

    private IEnumerator DoHitEffect()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        bool show = false;

        yield return flash;
        for (int i = 0; i < 6 && !stopFlash; i++)
        {
            SetImageAlpha(show);
            show = !show;
            yield return flash;
        }
        if (stopFlash) yield break;

        GetComponent<BoxCollider2D>().enabled = true;



    }
    private bool stopFlash;
    private void SetImageAlpha(bool show)
    {
        int layer = -10;
        if (show) layer = 0;
        sr.transform.position = new Vector3(sr.transform.position.x, sr.transform.position.y, layer);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //     void Start()
    //     {
    // #if UNITY_EDITOR

    //         mainDuration = testMainDur;
    //         startY = transform.position.y;

    //         // gameObject.SetActive(true);
    //         // transform.DOScaleX(.5f, heightDur).From(0).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed).OnComplete(() => rb.simulated = true);
    //         incubator = Instantiate(incubatorEgg, transform.position, Quaternion.identity).transform;
    //         ShowSequence();



    // #endif

    //         // startY = transform.position.y;
    //         // transform.DOScaleX(.5f, heightDur).From(0).SetEase(Ease.InOutSine);
    //         // incubator = Instantiate(incubatorEgg, transform.position, Quaternion.identity).transform;

    //         // StartCoroutine(DoSpriteHeight());

    //     }
    void Awake()
    {
        // sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        endY = BoundariesManager.GroundPosition;
        Vector2 size = new Vector2(1.28f, sr.size.y);
        backBlur.size = size;
        frontBlur.size = size;
        xSize = sr.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (!moveObject) return;
        if (time < mainDuration)
        {
            time += Time.deltaTime;
            incubator.position = new Vector2(transform.position.x, Mathf.Lerp(startY, endY, time / mainDuration));
        }
        else
        {

            moveObject = false;
            stopFlash = true;
            SetImageAlpha(true);
            GetComponent<BoxCollider2D>().enabled = false;
            FinishTween();
        }

    }
    [SerializeField] private float particleScaleStart;
    [SerializeField] private float particleScaleEnd;
    [SerializeField] private float finalDurStart;
    [SerializeField] private float finalDurEnd;
    void FinishTween()
    {
        if (showSeq! != null && showSeq.IsPlaying())
            showSeq.Kill();
        showSeq = DOTween.Sequence();
        showSeq.Append(particle.transform.DOScale(particleScaleStart, finalDurStart));
        showSeq.AppendCallback(() => incubator.gameObject.GetComponent<MoveOnGround>().enabled = true);
        showSeq.AppendCallback(() => particle.FadeOut(finalDurEnd));
        showSeq.Append(particle.transform.DOScale(particleScaleEnd, finalDurEnd));
        showSeq.Join(sr.transform.DOScaleX(0, finalDurEnd));
        showSeq.Play().OnComplete(() => gameObject.SetActive(false));

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Plane"))
        {
            Debug.Log("Pig On Trigger");
            var i = other.GetComponent<IDamageable>();
            lightning.SetTarget(new Vector2(transform.position.x, other.transform.position.y), player._transform, player);
            AudioManager.instance.PlayIncubatorHit();
            StartCoroutine(DoHitEffect());
            if (i != null)
            {
                i.Damage(1, -2);
            }



        }
    }
    void FixedUpdate()
    {
        if (xSpeed != 0)
        {
            rb.MovePosition(rb.position + (Vector2.left * xSpeed * Time.fixedDeltaTime));
        }
    }
}
