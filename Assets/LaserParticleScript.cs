using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LaserParticleScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem laserParticle;
    [SerializeField] private float startParticleSpeed;
    [SerializeField] private float startBlurAlpha;
    [SerializeField] private int maxParticles = 50;
    private float endBlurAlpha = .95f;
    private float minVolume = 0;
    private float maxVolume = .3f;
    private float midVolume = .15f;
    [SerializeField] private float basePitch = 1;


    [SerializeField] private float baseMaxVolume = .3f;
    private float baseMidVolume = .3f;

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

        var main = laserParticle.main;
        main.maxParticles = 0;


        // main.simulationSpeed = startSpeed;


        var color = blurSprite.color;
        color.a = 0;
        blurSprite.color = color;

    }

    private void Start()
    {
        maxVolume = baseMaxVolume * AudioManager.instance.SfxVolume;
        maxVolume = baseMaxVolume * AudioManager.instance.SfxVolume;
        audioSource.volume = 0;
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

    public void ChangeAudioPitch(float pitch)
    {

        audioSource.pitch = pitch * basePitch;
    }
    private void OnEnable()
    {

        Ticker.OnTickAction015 += ChangeVolume;
        AudioManager.instance.OnSetAudioPitch += ChangeAudioPitch;
        audioSource.pitch = AudioManager.instance.SfxPitch * basePitch;
        audioSource.volume = 0;

    }

    private void OnDisable()
    {
        Ticker.OnTickAction015 -= ChangeVolume;
        AudioManager.instance.OnSetAudioPitch -= ChangeAudioPitch;
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
            maxVolume = Mathf.Lerp(0, midVolume, t / timeLeft);

            // if (amount < .3f) yield return null;


            // main.simulationSpeed = Mathf.Lerp(startParticleSpeed, 1, t / timeLeft);
            // main.startColor = Color.Lerp(startParticleColor, particleColor, t / timeLeft);
            main.maxParticles = Mathf.RoundToInt(Mathf.Lerp(0, 8, t / timeLeft));
            color.a = Mathf.Lerp(startBlurAlpha, endBlurAlpha, t / timeLeft);
            blurSprite.color = color;

            yield return null;
        }
        ignoreSound = false;



        main.maxParticles = maxParticles;
        maxVolume = baseMaxVolume * AudioManager.instance.SfxVolume;
    }

    public void StopParicles()
    {
        ignoreSound = true;
        laserParticle.Stop();
        // blurSprite.DOFade(0, .3f).OnComplete(() => gameObject.SetActive(false));
        blurSprite.DOFade(0, .3f);
        maxVolume = 0;
        audioSource.volume = 0;



    }
    public void FadeOut(float dur)
    {
        StartCoroutine(LaserFadeCoroutine(dur));
    }
    public IEnumerator LaserFadeCoroutine(float timeLeft)
    {
        float t = 0;
        var main = laserParticle.main;
        var color = blurSprite.color;
        while (t < timeLeft)
        {
            t += Time.deltaTime;

            maxVolume = Mathf.Lerp(0, midVolume, t / timeLeft);

            // if (amount < .3f) yield return null;


            // main.simulationSpeed = Mathf.Lerp(startParticleSpeed, 1, t / timeLeft);
            // main.startColor = Color.Lerp(startParticleColor, particleColor, t / timeLeft);
            main.maxParticles = Mathf.RoundToInt(Mathf.Lerp(maxParticles, 3, t / timeLeft));
            color.a = Mathf.Lerp(endBlurAlpha, 0, t / timeLeft);
            blurSprite.color = color;

            yield return null;
        }

    }



}
