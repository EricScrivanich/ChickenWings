using UnityEngine;
using DG.Tweening;

public class CheckPointFlag : MonoBehaviour, ICollectible
{

    private AudioSource audioSource;
    public ushort spawnedTimeStep { get; private set; }

    [SerializeField] private AudioClip flagLiftSound;
    private float flagLiftVolume = .45f;

    private SpriteRenderer sr;
    [SerializeField] private Transform flagTransform;

    private Vector2 topFlapPos = new Vector2(-.54f, -0.07f);
    private float bottomFlapXPos;
    private Vector3 topFlapScale = new Vector3(1.3f, 1.4f, 1.3f);
    private Vector3 bottomFlapScale = new Vector3(1.25f, 0, 1.25f);



    private float groundOffset = .4f;
    [SerializeField] private bool testHasCollected;
    private Rigidbody2D rb;
    private bool isCollected = false;
    private BoxCollider2D boxCollider;
    private float baseVolume = .37f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        if (audioSource != null)
        {
            audioSource.volume = AudioManager.instance.SfxVolume * baseVolume;

        }



    }



    public void Initialize(bool hasCollected)
    {
        float desiredTotalHeight = Mathf.Abs(transform.position.y - BoundariesManager.GroundPosition + groundOffset);
        sr.size = new Vector2(desiredTotalHeight, sr.size.y);


        if (hasCollected)
        {
            isCollected = true;
            boxCollider.enabled = false;
            flagTransform.localPosition = topFlapPos;
            flagTransform.localScale = topFlapScale;
            // transform.position = new Vector2(2, transform.position.y);
            GetComponent<ConstantSpriteSwitch>().enabled = true;
            audioSource.Play();


        }
        else
        {
            bottomFlapXPos = -desiredTotalHeight + .2f;
            sr.enabled = true;
            flagTransform.localPosition = new Vector2(bottomFlapXPos, flagTransform.localPosition.y);
            flagTransform.localScale = bottomFlapScale;
        }
    }

    private void CheckFlagPosition()
    {
        if (flagTransform.localPosition.y < BoundariesManager.GroundPosition + groundOffset)
        {
            flagTransform.localPosition = new Vector3(flagTransform.localPosition.x, BoundariesManager.GroundPosition + groundOffset, flagTransform.localPosition.z);
        }
    }

    public void CollectFlag()
    {
        GetComponent<ConstantSpriteSwitch>().enabled = true;
        isCollected = true;
        boxCollider.enabled = false;
        audioSource.Play();
        audioSource.PlayOneShot(flagLiftSound, flagLiftVolume);
        flagTransform.DOLocalMoveX(topFlapPos.x, 0.5f);
        flagTransform.DOScale(topFlapScale, 0.3f);


    }

    private void SetPitch(float pitch)
    {
        if (audioSource != null)
        {
            audioSource.pitch = pitch;
        }
    }

    private void OnEnable()
    {

        AudioManager.instance.OnSetAudioPitch += SetPitch;
        audioSource.pitch = AudioManager.instance.SfxPitch;

    }
    private void OnDisable()
    {

        AudioManager.instance.OnSetAudioPitch -= SetPitch;
        DOTween.Kill(flagTransform);

    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector2.left * BoundariesManager.GroundSpeed * Time.fixedDeltaTime);
        if (transform.position.x < BoundariesManager.leftViewBoundary + 2f)
        {
            audioSource.volume = Mathf.InverseLerp(BoundariesManager.leftBoundary, BoundariesManager.leftViewBoundary + 2f, transform.position.x) * AudioManager.instance.SfxVolume * baseVolume;
            if (transform.position.x < BoundariesManager.leftBoundary) gameObject.SetActive(false);
        }
    }
    public void SetSpawnedTimeStep(ushort timeStep)
    {
        spawnedTimeStep = timeStep;
    }

    public Vector2 GetPositionAtStep(float time)
    {
        float realSpawnedTime = spawnedTimeStep * LevelRecordManager.TimePerStep;

        if (time < realSpawnedTime) return Vector2.zero;
        Vector2 pos = new Vector2(BoundariesManager.rightBoundary - (BoundariesManager.GroundSpeed * (time - realSpawnedTime)), -.49f);
        if (pos.x < BoundariesManager.leftBoundary)
        {
            return Vector2.zero;
        }
        return pos;
    }

    public void Collected()
    {
        if (isCollected) return;
        CollectFlag();
    }


    // Update is called once per frame

}
