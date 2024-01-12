using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketScript : MonoBehaviour
{
    public RingID ID;
    public PlayerID Player;
    public float speed;
  
    private float speedVar;
    public int order;
    [SerializeField] private SpriteRenderer[] strips;

    private Collider2D[] colliders;

    [SerializeField] private GameObject RingRim;
    private SpriteRenderer ringSprite;
    private Animator anim;
    private float slowDownDuration;
    // Start is called before the first frame update

    void Awake()
    {
        anim = GetComponent<Animator>();
        ringSprite = RingRim.GetComponent<SpriteRenderer>();
        colliders = GetComponents<Collider2D>();

    }
    void Start()
    {
       

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        
    }

    // ID.ringEvent.OnCreateNewSequence?.Invoke(true);
    // AudioManager.instance.PlayRingSuccessSound();

    // print("won- start");

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

        // Ensure all children are fully transparent after fading out
        
        gameObject.SetActive(false);
    }

    private IEnumerator SlowDown()
    {
        float currentTime = 0;

        if ((speed > 0 && transform.position.x < -6f) || (speed < 0 && transform.position.x > 6f))
        {
            slowDownDuration = 1f;
        }
        else if ((speed > 0 && transform.position.x > 6f) || (speed < 0 && transform.position.x < -6f))
        {
            slowDownDuration = 2f;

        }
        else
        {
            slowDownDuration = 1.5f;
        }

        while (currentTime < slowDownDuration)
        {
            float lerpAnim = Mathf.Lerp(1.0f, 0.0f, currentTime / slowDownDuration);
            float lerpSpeed = Mathf.Lerp(speedVar, 0, currentTime / slowDownDuration);
            anim.speed = lerpAnim;
            speed = lerpSpeed;

            currentTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        anim.SetTrigger("ExplodeTrigger");

        // Ensure all children are fully transparent after fading out


    }

   
    public void Completed()
    {
        DisableColliders();
        AudioManager.instance.PlayRingSuccessSound();
        StartCoroutine(SlowDown());
        // ID.ringEvent.OnCreateNewSequence?.Invoke(true);

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
        ID.ringEvent.OnCheckOrder += SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence += NewSetup;

        EnableColliders();
        FadeChildren(1f);
        speedVar = speed;
       
        if (ringSprite != null)
        {
            ringSprite.material = ID.defaultMaterial;

        }

        foreach (SpriteRenderer strip in strips)
        {
            if (strip != null)
            {
                strip.material = ID.defaultMaterial;

            }
        }


    }


    private void OnDisable() 
    {
        ID.ringEvent.OnCheckOrder -= SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence -= NewSetup;
        
    }
}
