using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BalloonBomb : SpawnedQueuedObject
{


    [SerializeField] private AnimationDataSO animData;

    private int currentSpriteIndex = 0;
    private static readonly Vector3 StartScale = new Vector3(.4f, .3f, 1);
    private static readonly Vector3 EndScale = new Vector3(1f, 1.45f, 1);

    private readonly float spriteSwitchTime = .07f;
    private float time = 0;
    private float scaleTimer;
    [SerializeField] private SpriteRenderer sr;
    // Start is called before the first frame update


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        scaleTimer = 0;
        time = 0;
        currentSpriteIndex = Random.Range(0, animData.sprites.Length - 1);
        sr.sprite = animData.sprites[currentSpriteIndex];
        sr.transform.localScale = StartScale;

        // sr.transform.DOScale(EndScale, 1.35f).From(StartScale).SetEase(Ease.InSine);
    }


    private void Update()
    {
        time += Time.deltaTime;


        if (scaleTimer < 1.6f)
        {
            scaleTimer += Time.deltaTime;
            float t = Mathf.Clamp01(scaleTimer / 1.6f);
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f); // InSine ease
            sr.transform.localScale = Vector3.Lerp(StartScale, EndScale, easedT);
        }

        if (time > spriteSwitchTime)
        {
            currentSpriteIndex++;

            if (currentSpriteIndex > animData.sprites.Length - 1)
            {
                currentSpriteIndex = 0;
            }
            sr.sprite = animData.sprites[currentSpriteIndex];
            time = 0;

        }
    }

    private void OnDisable()
    {


        // DOTween.Kill(sr.transform);
        ReturnToPool();
    }

    // Update is called once per frame
    public void Initilaize(Vector2 force)
    {
        gameObject.SetActive(true);

        rb.linearVelocity = force;
    }
}
