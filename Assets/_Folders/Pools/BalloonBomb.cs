using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BalloonBomb : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private AnimationDataSO animData;

    private int currentSpriteIndex = 0;
    private static readonly Vector3 StartScale = new Vector3(.5f, .4f, 1);
    private static readonly Vector3 EndScale = new Vector3(1f, 1.4f, 1);

    private readonly float spriteSwitchTime = .07f;
    private float time = 0;
    private float totalTime = 0;
    [SerializeField] private SpriteRenderer sr;
    // Start is called before the first frame update


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        currentSpriteIndex = Random.Range(0, animData.sprites.Length - 1);
        sr.sprite = animData.sprites[currentSpriteIndex];

        sr.transform.DOScale(EndScale, 1.35f).From(StartScale).SetEase(Ease.InSine);
    }

    private void Update()
    {
        time += Time.deltaTime;
        totalTime += Time.deltaTime;

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
        Debug.Log("Total time: " + totalTime);

        DOTween.Kill(this);
    }

    // Update is called once per frame
    public void Initilaize(Vector2 force)
    {
        gameObject.SetActive(true);
        Debug.Log("Bomb initillized");
        rb.linearVelocity = force;
    }
}
