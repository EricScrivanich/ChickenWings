using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpecialEggMovement : MonoBehaviour, ICollectible
{
    public PlayerID ID;

    private bool hasCollected = false;

    [SerializeField] private float tweenDuration;
    [SerializeField] private float upDownDuration;
    [SerializeField] private float upDownAmount;

    [SerializeField] private Ease easeType;

    [SerializeField] private bool isShotgun;
    [SerializeField] private bool isThree;



    private Color whiteColorStart = new Color(1, 1, 1, 1);
    private Color whiteColorEnd = new Color(1, 1, 1, 0);
    private Color redColorStart = new Color(.809f, .05f, .05f, 1);
    private Color redColorEnd = new Color(.809f, .05f, .05f, 0);
    private Color startColor;
    private Color endColor;

    private float eggSinFrequency = .45f;
    private float eggSinAmplitude = .3f;
    private float yPos;
    [SerializeField] private float speed;

    private Vector3 startScale = new Vector3(1f, 1f, 1f);
    private Vector3 endScale;

    private SpriteRenderer mainSprite;

    [SerializeField] private Sprite[] blurImage;
    [SerializeField] private SpriteRenderer threeImage;

    [SerializeField] private SpriteRenderer blur;
    [SerializeField] private ParticleSystem ps;


    public void Collected()
    {
        if (hasCollected) return;

        hasCollected = true;
        if (isShotgun)
        {
            if (isThree)
                ID.ShotgunAmmo += 3;
            else
                ID.ShotgunAmmo += 3;
        }
        StartCoroutine(Burst());


    }

    private IEnumerator Burst()
    {
        float burstTimer = 0;
        float duration = .15f;
        blur.enabled = true;
        float startSpeed = speed;
        float endSpeed = startSpeed * .2f;


        while (burstTimer < duration)
        {
            burstTimer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, burstTimer / duration);
            blur.color = Color.Lerp(endColor, startColor, burstTimer / duration);
            speed = Mathf.Lerp(startSpeed, endSpeed, burstTimer / duration);

            yield return null;

        }
        ps.Play();
        burstTimer = 0;
        duration = .2f;
        mainSprite.enabled = false;

        threeImage.enabled = false;
        while (burstTimer < duration)
        {
            burstTimer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(endScale, endScale * 1.1f, burstTimer / duration);
            blur.color = Color.Lerp(startColor, endColor, burstTimer / duration);

            yield return null;

        }
        yield return new WaitForSecondsRealtime(.5f);
        gameObject.SetActive(false);


    }

    // Start is called before the first frame update
    void Start()
    {
        mainSprite = GetComponent<SpriteRenderer>();
        blur.enabled = false;
        if (isShotgun)
        {
            startColor = redColorStart;
            endColor = redColorEnd;
        }

        transform.DOMoveX(0, tweenDuration).SetEase(easeType);

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLocalMoveY(upDownAmount + 2, upDownDuration).From(-upDownAmount + 2).SetEase(Ease.InOutSine));
        seq.Append(transform.DOLocalMoveY(-upDownAmount + 2, upDownDuration).From(upDownAmount + 2).SetEase(Ease.InOutSine));
        seq.Play().SetLoops(-1);




    }
    private void OnDisable()
    {
        DOTween.Kill(this);
    }

    // Update is called once per frame
    void Update()
    {

        // float distanceToTarget = Mathf.Abs(transform.position.x - 0);

        // // Lerp the speed down as the object approaches the target X position
        // speed = Mathf.Lerp(speed, 0, 1 - (distanceToTarget / Mathf.Abs(transform.position.x)));

        // Move the object left by the current speed



        // Calculate the y position using a sine function
        // float x = transform.position.x;
        // float y = Mathf.Sin(x * eggSinFrequency) * eggSinAmplitude + yPos;
        // transform.position = new Vector3(x, y, 0);

    }
}
