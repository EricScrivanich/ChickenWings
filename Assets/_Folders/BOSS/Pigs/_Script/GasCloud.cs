using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class GasCloud : MonoBehaviour
{

    [SerializeField] private AnimationDataSO animData;
    [SerializeField] private float maxYSpeed;


    private readonly int spriteCount = 3;

    private int currentSpriteIndex = 0;

    private readonly float spriteSwitchTime = .1f;
    private float time = 0;
    private readonly float tweenTime = .6f;
    private Rigidbody2D rb;
    private float endSpeed;
    private float forceAdded = .7f;
    private float forceAddedVar;
    private bool hitTarget = false;

    private bool flipped;







    private SpriteRenderer sr;
    // Start is called before the first frame update
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        float startX = transform.localPosition.x;

        // transform.DOLocalMoveX(startX + 1.7f, .5f).SetEase(Ease.OutSine);
        // transform.DOMoveY(7f, 6.5f);
    }

    private void OnDisable()
    {
        DOTween.Kill(this);
    }

    public void Eject(float endSpeedVar, bool f)
    {
        flipped = f;
        gameObject.SetActive(true);
        float rand = Random.Range(.95f, 1.05f);


        transform.DOScale(animData.endScale * rand, tweenTime).From(animData.startScale).SetEase(Ease.InSine);

        rb.angularVelocity = Random.Range(-60, 60);

        if (flipped)
        {
            rb.linearVelocity = Vector2.left * (rand - endSpeedVar) * 1.8f;
            forceAddedVar = -forceAdded;
        }
        else
        {
            rb.linearVelocity = Vector2.right * (.05f);
            forceAddedVar = forceAdded + .35f + Random.Range(.1f, .2f);
        }


        endSpeed = endSpeedVar;


    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        SmokeTrailPool.GetGasCloudHitParicles(transform.position);
        gameObject.SetActive(false);

    }

    private void FixedUpdate()
    {


        rb.AddForce(Vector2.left * forceAddedVar);

        if (transform.position.y > 7) gameObject.SetActive(false);

        if (!flipped)
            rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, endSpeed, 5), Mathf.Clamp(rb.linearVelocity.y, -3, maxYSpeed));

        else
            rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -10, endSpeed), Mathf.Clamp(rb.linearVelocity.y, -3, maxYSpeed));


    }

    // Update is called once per frame
    void Update()
    {

        time += Time.deltaTime;

        if (time > spriteSwitchTime)
        {
            currentSpriteIndex++;

            if (currentSpriteIndex > spriteCount)
            {
                currentSpriteIndex = 0;
            }
            sr.sprite = animData.sprites[currentSpriteIndex];
            time = 0;

        }



    }
}
