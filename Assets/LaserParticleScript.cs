using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LaserParticleScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem laserParticle;
    [SerializeField] private float startParticleSpeed;
    [SerializeField] private float startBlurAlpha;
    private float endBlurAlpha = .95f;
    private float minVolume = 0;
    private float maxVolume = .3f;

    private SpriteRenderer blurSprite;
    private AudioSource audioSource;

    private Color particleColor;
    private Color startParticleColor;
    private bool ignoreSound = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        blurSprite = GetComponent<SpriteRenderer>();
        particleColor = laserParticle.main.startColor.color;
        startParticleColor = new Color(particleColor.r, particleColor.g, particleColor.b, 0);

    }

    private void ChangeVolume()
    {
        if (ignoreSound) return;
        float vol = Mathf.Lerp(minVolume, maxVolume, (BoundariesManager.rightBoundary - Mathf.Abs(transform.position.x)) / BoundariesManager.rightBoundary);

        audioSource.volume = vol;



    }



    public void SetLaserFadeAmount(float amount, float timeLeft)
    {
        Debug.LogError("SetLaserFadeAmount: " + amount + " " + timeLeft);
        if (amount == 1)
        {
            ignoreSound = false;

            ChangeVolume();
            var main = laserParticle.main;
            // main.simulationSpeed = 1;
            var color = blurSprite.color;
            color.a = endBlurAlpha;
            blurSprite.color = color;
            // main.startColor = particleColor;


        }
        else
        {

            StartCoroutine(LaserParticleCoroutine(amount, timeLeft));
        }

    }
    private void OnEnable()
    {
        audioSource.volume = 0;
        Ticker.OnTickAction015 += ChangeVolume;

    }

    private void OnDisable()
    {
        Ticker.OnTickAction015 -= ChangeVolume;
    }
    public IEnumerator LaserParticleCoroutine(float amount, float timeLeft)
    {

        float t = 0;
        float currentAmount = amount;


        // float startSpeed = Mathf.Lerp(startParticleSpeed, 1, amount);
        var main = laserParticle.main;
        main.maxParticles = 0;
        laserParticle.Play();

        // main.simulationSpeed = startSpeed;
        float startAlpha = Mathf.Lerp(0, endBlurAlpha, amount);

        var color = blurSprite.color;
        color.a = startAlpha;
        blurSprite.color = color;

        while (t < timeLeft)
        {
            t += Time.deltaTime;
            currentAmount = Mathf.Lerp(amount, 1, t / timeLeft);
            maxVolume = Mathf.Lerp(0, .15f, t / timeLeft);

            // if (amount < .3f) yield return null;


            // main.simulationSpeed = Mathf.Lerp(startParticleSpeed, 1, t / timeLeft);
            // main.startColor = Color.Lerp(startParticleColor, particleColor, t / timeLeft);
            main.maxParticles = Mathf.RoundToInt(Mathf.Lerp(0, 8, t / timeLeft));
            color.a = Mathf.Lerp(startBlurAlpha, endBlurAlpha, t / timeLeft);
            blurSprite.color = color;

            yield return null;
        }
        ignoreSound = false;



        main.maxParticles = 50;
        maxVolume = .3f;
    }

    public void StopParicles()
    {
        ignoreSound = true;
        laserParticle.Stop();
        // blurSprite.DOFade(0, .3f).OnComplete(() => gameObject.SetActive(false));
        blurSprite.DOFade(0, .3f);
        maxVolume = 0;



    }



}
