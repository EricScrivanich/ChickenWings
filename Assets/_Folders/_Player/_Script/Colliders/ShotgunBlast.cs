using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShotgunBlast : MonoBehaviour
{

    [SerializeField] private float forceAmount;
    [SerializeField] private float xForceMultiplier;
    private Rigidbody2D rb;
    private Sequence shotgunBlastSeq;

    [SerializeField] private Rigidbody2D[] bulletParticles;
    [SerializeField] private Vector3[] countAndVelocitys;

    private Color startColor = new Color(1, 1, 1, 1);
    private Color endColor = new Color(.35f, .3f, .3f, 0.05f);
    private BoxCollider2D col;

    public Sprite[] img;
    public float[] spriteDelays;
    public Vector2[] scales;

    private Vector3 StartScale = new Vector3(.25f, .5f, 1);

    public float[] opacities;
    public float[] scaleDelays;
    private float time;
    private int currentSpriteDelayIndex;
    private int currentScaleDelayIndex;
    private bool finished = false;
    private bool isChained;
    private int id;
    private SpriteRenderer sr;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update

    public void Initialize(bool chained, int iD)
    {
        isChained = chained;
        id = iD;

    }

    private void OnEnable()
    {
        col.enabled = true;
        transform.localScale = StartScale;
        sr.color = startColor;
        time = 0;
        sr.sprite = img[0];
        Vector2 force = transform.right * forceAmount;
        hasBeenBlocked = false;

        float xVelRatio = force.x / forceAmount;
        float addedX = 0;

        // if (yVelRatio > 0) addedY = yVelRatio * yForceMultiplier;
        // else if (yVelRatio < 0) addedY = yVelRatio * yForceMultiplier * -1;
        addedX = Mathf.Abs(xVelRatio * xForceMultiplier);

        Vector2 finalForce = new Vector2(force.x - addedX, force.y);


        // rb.linearVelocity = finalForce;

        ScaleAndOpacity();

        for (int i = 0; i < countAndVelocitys.Length; i++)
        {
            for (int n = (int)countAndVelocitys[i].x; n < (int)countAndVelocitys[i].y; n++)
            {
                bulletParticles[n].linearVelocity = bulletParticles[n].transform.right * countAndVelocitys[i].z;
            }
        }


    }

    // public void Initilaize(Vector2 direction)
    // {
    //     transform.localScale = StartScale;
    //     sr.color = startColor;
    //     gameObject.SetActive(true);
    //     rb.velocity = direction * forceAmount;

    // }
    private void OnDisable()
    {
        rb.linearVelocity = Vector2.zero;
        finished = false;
        currentScaleDelayIndex = 0;
    }

    private void Update()
    {

        if (finished) return;
        else time += Time.deltaTime;
        if (time > spriteDelays[currentScaleDelayIndex] && !finished)
        {
            currentSpriteDelayIndex++;
            sr.sprite = img[currentSpriteDelayIndex];
            time = 0;
            if (currentSpriteDelayIndex > spriteDelays.Length - 1)
            {
                currentSpriteDelayIndex = 0;
                finished = true;
                time = 0;
                return;

            }
        }
    }

    private void ScaleAndOpacity()
    {
        // Ensure to kill any previous sequence if it's still running
        // if (sequence != null && sequence.IsActive())
        // {
        //     sequence.Kill();
        // }

        // Create a new sequence
        if (shotgunBlastSeq != null && shotgunBlastSeq.IsActive())
        {
            shotgunBlastSeq.Kill();
        }
        shotgunBlastSeq = DOTween.Sequence();

        // Iterate over the scales, opacities, and scaleDelays arrays
        for (int i = 0; i < scales.Length; i++)
        {

            if (i == scales.Length - 1)
            {
                shotgunBlastSeq.AppendCallback(() => Invoke("DisableCollider", .12f));
                shotgunBlastSeq.Append(transform.DOScale(scales[i], scaleDelays[i]).SetEase(Ease.OutSine));
                shotgunBlastSeq.Join(sr.DOColor(endColor, scaleDelays[i]).SetEase(Ease.OutSine));
            }
            else
            {
                shotgunBlastSeq.Append(transform.DOScale(scales[i], scaleDelays[i]));
                shotgunBlastSeq.Join(sr.DOFade(opacities[i], scaleDelays[i]));

            }

        }

        shotgunBlastSeq.Play().SetUpdate(UpdateType.Fixed).OnComplete(() => gameObject.SetActive(false));

        // Once the sequence is complete, mark the animation as finished
        // sequence.OnComplete(() => gameObject.SetActive(false));
    }
    private bool hasBeenBlocked;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Block"))
        {
            hasBeenBlocked = true;
            AudioManager.instance.PlayParrySound();
            rb.linearVelocity *= .4f;
            DisableCollider();
            Debug.Log("Attack was blocked boi");
            return;
        }

        IDamageable damageableEntity = collider.gameObject.GetComponent<IDamageable>();
        if (damageableEntity != null && !hasBeenBlocked)
        {
            int type = 1;
            if (isChained) type = 2;
            damageableEntity.Damage(1, type, id);
            return;

        }
        IExplodable explodable = collider.gameObject.GetComponent<IExplodable>();
        if (explodable != null && !hasBeenBlocked)
        {

            explodable.Explode(false);
            return;

        }

    }

    private void DisableCollider()
    {
        col.enabled = false;
    }


}
