using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

public class PolyBomb : MonoBehaviour
{
    private AudioSource audioSource;
    [Header("Audio")]
    [SerializeField] private AudioClip beepSound;
    [SerializeField] private AudioClip chargeSound;
    [SerializeField] private AudioClip explodeSound;
    [SerializeField] private float beepVolume;
    [SerializeField] private float chargeVolume;
    [SerializeField] private float explodeVolume;

    [Header("Prefab Parts")]
    [SerializeField] private SpriteRenderer[] lightSprites;
    [SerializeField] private CircleCollider2D explosionColl;

    private SpriteRenderer baseSR;
    [SerializeField] private SpriteRenderer explosionSR;
    [SerializeField] private SpriteRenderer mainLight;
    [SerializeField] private SpriteRenderer mainLightBlur;
    [SerializeField] private SpriteRenderer circleLight;

    [SerializeField] private Color lightOnColor;
    [SerializeField] private Color lightOffColor;
    [SerializeField] private Color circleEndColor;
    [SerializeField] private Color circleEndExplosionColor;

    [Header("Times")]
    [SerializeField] private float baseTime;
    [SerializeField] private float flashInRatio;
    [SerializeField] private float circleRatio;
    [SerializeField] private float lightBlurAlpha;
    [SerializeField] private float circleStartScale;
    [SerializeField] private float circleEndScale;
    [SerializeField] private float circleStartAlpha;
    [SerializeField] private float circleEndAlpha;
    [SerializeField] private float circleToZeroAlphaRatio;
    [SerializeField] private float sideLightDuration;

    [Header("Explosion")]
    [SerializeField] private Sprite[] explosionSprites;


    [SerializeField] private float blurLightEndScale;
    [SerializeField] private float explosionTimePerFrame;
    [SerializeField] private float explosionStartScale;
    [SerializeField] private float explosionEndScale;
    [SerializeField] private int startExplosionFadeIndex;
    private int currentExplosionIndex;

    [Header("Spikes")]

    [SerializeField] private bool useSpikes;
    [SerializeField] private Rigidbody2D[] spikes;
    [SerializeField] private float spikeForce;



    private float mainLightFadeIn;
    private float mainLightFadeOut;
    private float circleDuration;
    private float circleMainFadeDuration;
    private float circleEndFadeDuration;

    private int currentLightIndex;
    private bool readyToExplode = false;


    private Sequence flashSeq;
    private Sequence circleSeq;


    private float timeInterval;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Countdown());

    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        baseSR = GetComponent<SpriteRenderer>();
        currentLightIndex = 0;
        timeInterval = baseTime / FrameRateManager.BaseTimeScale;

        mainLightFadeIn = timeInterval * flashInRatio;
        mainLightFadeOut = timeInterval - mainLightFadeIn;
        circleDuration = timeInterval * circleRatio;
        circleEndFadeDuration = circleDuration * circleToZeroAlphaRatio;
        circleMainFadeDuration = circleDuration - circleEndFadeDuration;



    }

    private void OnEnable()
    {

        foreach (var s in spikes)
        {
            s.gameObject.SetActive(useSpikes);
        }

        explosionColl.enabled = false;
        currentExplosionIndex = 0;
        explosionSR.sprite = explosionSprites[0];

        explosionSR.enabled = false;
        readyToExplode = false;
        foreach (var l in lightSprites)
        {
            l.color = lightOnColor;
        }

        mainLight.color = lightOffColor;
        mainLightBlur.color = lightOnColor;
        circleLight.color = lightOnColor;
        circleLight.DOFade(0, 0);
        mainLightBlur.DOFade(0, 0);
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1);
        LightFlashOn();



    }


    private void LightFlashOn()
    {
        flashSeq = DOTween.Sequence();
        flashSeq.Append(mainLight.DOColor(lightOnColor, mainLightFadeIn));
        flashSeq.Join(mainLightBlur.DOFade(lightBlurAlpha, mainLightFadeIn).SetEase(Ease.InSine).OnComplete(CircleLight));

        flashSeq.Append(mainLight.DOColor(lightOffColor, mainLightFadeOut));
        flashSeq.Join(mainLightBlur.DOFade(0, mainLightFadeOut).SetEase(Ease.OutSine).OnComplete(LightFlashExplode));

        flashSeq.Play().SetLoops(-1);


    }

    private void LightFlashExplode()
    {
        if (!readyToExplode) return;

        if (flashSeq != null && flashSeq.IsPlaying())
            flashSeq.Kill();
        flashSeq = DOTween.Sequence();


        flashSeq.Append(mainLight.DOColor(lightOnColor, mainLightFadeIn));
        flashSeq.Join(mainLightBlur.DOFade(lightBlurAlpha, mainLightFadeIn).SetEase(Ease.InSine).OnComplete(Explode));
        flashSeq.Join(mainLightBlur.transform.DOScale(blurLightEndScale, timeInterval + .2f));
        flashSeq.Play();


    }

    private void CircleLight()
    {
        // if (readyToExplode)
        // {
        //     Explode();
        //     return;
        // }
        TurnOffLight();
        if (circleSeq != null && circleSeq.IsPlaying())
            circleSeq.Kill();
        circleSeq = DOTween.Sequence();
        circleLight.transform.localScale = circleStartScale * BoundariesManager.vectorThree1;
        circleLight.transform.DOScale(circleEndScale, circleDuration);
        circleSeq.Append(circleLight.DOColor(circleEndColor, circleMainFadeDuration).From(lightOnColor));
        circleSeq.Append(circleLight.DOFade(0, circleEndFadeDuration));

        circleSeq.Play();
    }

    private void TurnOffLight()
    {

        audioSource.PlayOneShot(beepSound, beepVolume);

        lightSprites[currentLightIndex].DOColor(lightOffColor, sideLightDuration);

        currentLightIndex++;

        if (currentLightIndex >= lightSprites.Length)
        {
            readyToExplode = true;
            // audioSource.PlayOneShot(chargeSound, chargeVolume);
        }


    }

    private void Explode()
    {


        mainLightBlur.DOFade(0, mainLightFadeOut).SetEase(Ease.OutSine);
        baseSR.enabled = false;
        audioSource.PlayOneShot(explodeSound, explodeVolume);
        foreach (var l in lightSprites)
        {
            l.enabled = false;
        }
        mainLight.enabled = false;
        explosionSR.enabled = true;
        StartCoroutine(ExplosionAnimation());
        if (useSpikes)
        {
            ShootSpikes();
        }






    }

    private void CircleExplosion(float dur)
    {

        explosionColl.enabled = true;
        circleSeq = DOTween.Sequence();
        circleLight.transform.localScale = circleStartScale * BoundariesManager.vectorThree1;
        circleLight.transform.DOScale(circleEndScale, dur).SetEase(Ease.OutSine);
        circleSeq.Append(circleLight.DOColor(circleEndExplosionColor, dur * .8f).From(lightOnColor).OnComplete(() => explosionColl.enabled = false));
        circleSeq.Append(circleLight.DOFade(0, .3f));

        // circleSeq.Play().OnComplete(() => gameObject.SetActive(false));
        circleSeq.Play();

    }



    private IEnumerator ExplosionAnimation()
    {
        bool active = true;
        bool startedFade = false;
        float scaleDur = explosionSprites.Length * explosionTimePerFrame;
        float fadeDur = explosionTimePerFrame * (explosionSprites.Length - startExplosionFadeIndex);

        explosionSR.transform.DOScale(explosionEndScale, fadeDur).From(explosionStartScale).SetEase(Ease.InSine);
        CircleExplosion(scaleDur);

        while (active)
        {
            yield return new WaitForSeconds(explosionTimePerFrame);
            currentExplosionIndex++;
            explosionSR.sprite = explosionSprites[currentExplosionIndex];

            if (currentExplosionIndex >= explosionSprites.Length - 1)
            {
                active = false;

            }
            else if (currentExplosionIndex == startExplosionFadeIndex && !startedFade)
            {
                startedFade = true;
                explosionSR.DOFade(0, fadeDur);
            }
        }


    }

    private void ShootSpikes()
    {
        foreach (var s in spikes)
        {
            s.linearVelocity = s.transform.right * spikeForce;
        }
    }




}
