using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LaserParticleScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem laserParticle;
    [SerializeField] private float startParticleSpeed;
    [SerializeField] private float startBlurAlpha;
    private float endBlurAlpha = .95f;
    private SpriteRenderer blurSprite;

    private Color particleColor;
    private Color startParticleColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    private void Awake()
    {
        blurSprite = GetComponent<SpriteRenderer>();
        particleColor = laserParticle.main.startColor.color;
        startParticleColor = new Color(particleColor.r, particleColor.g, particleColor.b, 0);

    }



    public void SetLaserFadeAmount(float amount, float timeLeft)
    {
        if (amount == 1)
        {
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



    public IEnumerator LaserParticleCoroutine(float amount, float timeLeft)
    {
        Debug.Log("Coroutine started");
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

            // if (amount < .3f) yield return null;


            // main.simulationSpeed = Mathf.Lerp(startParticleSpeed, 1, t / timeLeft);
            // main.startColor = Color.Lerp(startParticleColor, particleColor, t / timeLeft);
            main.maxParticles = Mathf.RoundToInt(Mathf.Lerp(0, 8, t / timeLeft));
            color.a = Mathf.Lerp(startBlurAlpha, endBlurAlpha, t / timeLeft);
            blurSprite.color = color;

            yield return null;
        }


        main.maxParticles = 50;
    }

    public void StopParicles()
    {
        laserParticle.Stop();
        // blurSprite.DOFade(0, .3f).OnComplete(() => gameObject.SetActive(false));
        blurSprite.DOFade(0, .3f);



    }



}
