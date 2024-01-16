using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketScript : MonoBehaviour
{
    public RingID ID;
    private int ResetCounter = 0;
    public PlayerID Player;
    public float speed;
    private bool ready = false;
    private bool isExploded = false;

    [SerializeField] private float timeToSlow;
    

    [SerializeField] private GameObject WhiteParticles;
    [SerializeField] private GameObject ColoredParticles;


    private SpriteRenderer whiteParticleSprite;
    private SpriteRenderer coloredParticleSprite;

    private float speedVar;
    public int order;
    [SerializeField] private SpriteRenderer[] coloredObjects;


    [SerializeField] private GameObject[] children;

    private Collider2D[] colliders;

    [SerializeField] private GameObject RingRim;
    private Transform ringTransform;
    private SpriteRenderer ringSprite;
    private Animator anim;
    private float slowDownDuration;
    // Start is called before the first frame update

    void Awake()
    {
        anim = GetComponent<Animator>();
        ringSprite = RingRim.GetComponent<SpriteRenderer>();
        ringTransform = RingRim.GetComponent<Transform>();
        colliders = GetComponents<Collider2D>();
        coloredParticleSprite = ColoredParticles.GetComponent<SpriteRenderer>();
        whiteParticleSprite = WhiteParticles.GetComponent<SpriteRenderer>();


    }
    // Update is called once per frame
    void Update()
    {
        transform.localPosition += Vector3.left * speed * Time.deltaTime;


        // if (isExploded)
        // {
        //     float angleInRadians = ringTransform.eulerAngles.z * Mathf.Deg2Rad; // Convert degrees to radians
        //     Vector3 direction = new Vector3(-Mathf.Cos(angleInRadians), -Mathf.Sin(angleInRadians), 0);
        //     ringTransform.localPosition += direction * Time.deltaTime * 40; // Move in the direction based on the rotation

        // }

    }


    private void DisableColliders()
    {

        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
    private void EnableColliders()
    {

        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
    }

    private void SetCorrectRing()
    {
        if (order == ID.CorrectRing)
        {
            ringSprite.material = ID.highlightedMaterial;

        }

    }
    private void NewSetup(bool correctSequence)
    {
        if (correctSequence == false)
        {
            DisableColliders();
            StartCoroutine(FadeOut(1f));
        }

    }

    // public void Reset()
    // {
    //     foreach (GameObject child in children)
    //     {
    //         child.SetActive(true);
    //         // Debug.Log(child);
    //     }
    //     WhiteParticles.SetActive(false);
    //     ColoredParticles.SetActive(false);
    // }

    private IEnumerator FadeOut(float fadeOutDuration)
    {
        float currentTime = 0;

        while (currentTime < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(1.0f, 0.0f, currentTime / fadeOutDuration);
            FadeChildren(alpha);
            currentTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        gameObject.SetActive(false);
    }

    private IEnumerator SlowDown()
    {
        float currentTime = 0;

        slowDownDuration = 1.3f;

        while (currentTime < slowDownDuration)
        {
            float lerpSpeed = Mathf.Lerp(speedVar, 0, currentTime / slowDownDuration);
            speed = lerpSpeed;
            currentTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        speed = 0;
    }

    private IEnumerator SlowDownNew(float slowSpeed)
    {
        float currentTime = 0;

        while (slowDownDuration < anim.speed)
        {
            float lerpAnim = Mathf.Lerp(1.0f, 0.0f, currentTime / timeToSlow);

            anim.speed = lerpAnim;


            currentTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }



        // Ensure all children are fully transparent after fading out

    }



    public void StartSlowDown()
    {
        if (ready)
        {
            anim.SetTrigger("SlowTrigger");
        }
    }


    // Called by Animator
    public void StartExplosion()
    {
        anim.SetTrigger("ExplodeTrigger");
        // anim.SetBool("RestartBool", false);
        // anim.SetBool("RestartBool", false);

    }

    private void SetParticleOpacity(SpriteRenderer spriteRenderer, float opacity)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = opacity;
            spriteRenderer.color = color;
        }
    }

    private IEnumerator FadeOutParticles(float fadeDuration)
    {
        float currentTime = 0f;
        while (currentTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
            SetParticleOpacity(whiteParticleSprite, alpha);
            SetParticleOpacity(coloredParticleSprite, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    //Called by Animator

    public void Explode()
    {
        anim.SetTrigger("ParticleTrigger");
        Player.globalEvents.OnBucketExplosion?.Invoke();
        isExploded = true;
        // anim.ResetTrigger("ParticleTrigger");
        StartCoroutine(FadeOutParticles(0.35f));

        StartCoroutine(SetUnactive(2f));

    }

    private void ResetTrigger()
    {
        print("PLeaseReset");
        anim.SetTrigger("ResetTrigger");
        anim.SetBool("RestartBool", false);

    }


    private IEnumerator SetUnactive(float time)
    {
        yield return new WaitForSeconds(time);
        anim.SetBool("RestartBool", true);
        gameObject.SetActive(false);
        ID.ringEvent.OnCreateNewSequence?.Invoke(true);
    }


    public void Completed()
    {
        ready = true;
        DisableColliders();
        AudioManager.instance.PlayRingSuccessSound();
        StartCoroutine(SlowDown());
    }


    private void FadeChildren(float opacityValue)
    {
        foreach (Transform child in transform)
        {
            var renderer = child.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = opacityValue;
                renderer.color = color;
            }
        }
    }

    public void AnimCheck(int yer)
    {
        Debug.Log(yer);
    }

    private void OnEnable()
    {
        anim.SetBool("RestartBool", true);
        EnableColliders();
        ID.ringEvent.OnCheckOrder += SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence += NewSetup;
        EnableColliders();
        FadeChildren(1f);
        coloredParticleSprite.color = ID.defaultMaterial.color;
        speedVar = speed;

        if (ringSprite != null)
        {
            ringSprite.material = ID.defaultMaterial;

        }

        foreach (SpriteRenderer strip in coloredObjects)
        {
            if (strip != null)
            {
                strip.color = ID.defaultMaterial.color;

            }
        }


    }


    private void OnDisable()
    {
        // Debug.Log("reset " + ResetCounter);
        ResetCounter++;
        ready = false;
        isExploded = false;
        ID.ringEvent.OnCheckOrder -= SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence -= NewSetup;

    }
}
