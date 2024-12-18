using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketScript : MonoBehaviour, ICollectible
{
    public RingID ID;
    private int ResetCounter = 0;
    public PlayerID Player;
    public float speed;
    public int index => ID.IDIndex;
    private bool ready = false;
    private bool isExploded = false;
    private bool isCorrect = false;

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
    public Transform baseTransform;

    [SerializeField] private GameObject RingRim;
    private Transform ringTransform;
    private Vector2 ringTransformOriginal;
    [SerializeField] private SpriteRenderer ringSpriteFront;

    [SerializeField] private SpriteRenderer ringSpriteBack;
    private Animator anim;
    private float slowDownDuration;
    // Start is called before the first frame update

    void Awake()
    {
        anim = GetComponent<Animator>();
        ringTransform = RingRim.GetComponent<Transform>();
        colliders = GetComponents<Collider2D>();
        coloredParticleSprite = ColoredParticles.GetComponent<SpriteRenderer>();
        whiteParticleSprite = WhiteParticles.GetComponent<SpriteRenderer>();
        ringTransformOriginal = ringTransform.localPosition;



    }
    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (isCorrect)
        {
            if (speed > 0 && transform.position.x < BoundariesManager.leftBoundary)
            {
                HandleCorrectRing();
                ID.ringEvent.OnCreateNewSequence?.Invoke(false, index);

            }
            else if (speed < 0 && transform.position.x > BoundariesManager.rightBoundary)
            {
                HandleCorrectRing();
                ID.ringEvent.OnCreateNewSequence?.Invoke(false, index);



            }
        }


        if (isExploded)
        {
            float angleInRadians = ringTransform.eulerAngles.z * Mathf.Deg2Rad; // Convert degrees to radians
            Vector3 direction = new Vector3(-Mathf.Cos(angleInRadians), -Mathf.Sin(angleInRadians), 0);
            ringTransform.localPosition += direction * Time.deltaTime * 40; // Move in the direction based on the rotation

        }

    }

    private void HandleCorrectRing()
    {
        if (isCorrect && index == 0)
        {
            isCorrect = false;
            ID.ringEvent.OnGetBall?.Invoke(ringTransform.position);
            ringSpriteFront.material = ID.passedMaterial;
            ringSpriteBack.material = ID.passedMaterial;

        }
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
            ringSpriteFront.material = ID.highlightedMaterial;
            ringSpriteBack.material = ID.highlightedMaterial;
            isCorrect = true;

        }

    }
    private void NewSetup(bool correctSequence, int index)
    {

        if (correctSequence == false)
        {

            isCorrect = false;
            ResetBucket();

        }

    }



    private void ResetBucket()
    {
        DisableColliders();
        StartCoroutine(FadeOut(.8f));


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

        // gameObject.SetActive(false);
    }

    private IEnumerator SlowDown()
    {
        float currentTime = 0;

        slowDownDuration = 1.1f;

        while (currentTime < slowDownDuration)
        {
            float lerpSpeed = Mathf.Lerp(speedVar, 0, currentTime / slowDownDuration);
            speed = lerpSpeed;
            currentTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        speed = 0;
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
        AudioManager.instance.PlayBucketBurstSound();

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
        ID.GetSonicWave(transform.position);
        Player.globalEvents.OnBucketExplosion?.Invoke(index);
        isExploded = true;
        // anim.ResetTrigger("ParticleTrigger");
        StartCoroutine(FadeOutParticles(0.35f));

        StartCoroutine(SetUnactive(2f));

    }

    private void ResetTrigger()
    {

        anim.SetTrigger("ResetTrigger");
        anim.SetBool("RestartBool", false);

    }


    private IEnumerator SetUnactive(float time)
    {
        yield return new WaitForSeconds(time);
        anim.SetBool("RestartBool", true);
        gameObject.SetActive(false);
        ID.ringEvent.OnCreateNewSequence?.Invoke(true, index);
    }


    public void Collected()
    {
        if (isCorrect)
        {
            ready = true;
            Player.events.OnCompletedRingSequence?.Invoke(baseTransform);
            DisableColliders();
            // AudioManager.instance.PlayRingSuccessSound();
            StartCoroutine(SlowDown());
            isCorrect = false;

        }
        else
        {
            // ResetBucket(); 

        }

    }

    private void DisableBucket(int indexVar)
    {
        if (index == indexVar)
        {
            gameObject.SetActive(false);
        }

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


    private void OnEnable()
    {
        isCorrect = false;

        anim.SetBool("RestartBool", true);
        EnableColliders();
        ID.ringEvent.OnCheckOrder += SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence += NewSetup;
        ID.ringEvent.DisableRings += DisableBucket;
        EnableColliders();
        FadeChildren(1f);
        StartCoroutine(ResetRing());



        coloredParticleSprite.color = ID.defaultMaterial.color;
        speedVar = speed;


        ringSpriteFront.material = ID.defaultMaterial;
        ringSpriteBack.material = ID.defaultMaterial;



        foreach (SpriteRenderer strip in coloredObjects)
        {
            if (strip != null)
            {
                strip.color = ID.defaultMaterial.color;

            }
        }



    }

    private IEnumerator ResetRing()
    {
        ringTransform.gameObject.SetActive(false);
        yield return new WaitForSeconds(.3f);
        ringTransform.localPosition = ringTransformOriginal;
        yield return new WaitForSeconds(.2f);

        ringTransform.gameObject.SetActive(true);
        anim.SetBool("RestartBool", false);

    }


    private void OnDisable()
    {
        isExploded = false;





        // Debug.Log("reset " + ResetCounter);
        ResetCounter++;
        ready = false;

        ID.ringEvent.OnCheckOrder -= SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence -= NewSetup;
        ID.ringEvent.DisableRings -= DisableBucket;



    }
}
