using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRingMovement : MonoBehaviour, ICollectible
{
    public RingID ID;

    private EdgeCollider2D coll2D;

    private RingParent parent;
    private ParticleSystem ps;

    [SerializeField] private LevelManagerID lvlID;
    [SerializeField] private SpriteRenderer center;
    private Vector3 startBurstScale = new Vector3(.5f, 1, 1);
    private Vector3 endBurstScale = new Vector3(1.1f, 1, 1);


    [SerializeField] private int order;


    [SerializeField] private int finalOrder;

    private Transform _transform;
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer backRing;


    private readonly int BurstBoolHash = Animator.StringToHash("BurstBool");
    private readonly int FadeCenterHash = Animator.StringToHash("FadeCenterBool");


    private bool isFaded = false;

    // private Rigidbody2D rb;

    private SpriteRenderer sprite;

    // Start is called before the first frame update


    private void Awake()
    {


        sprite = GetComponent<SpriteRenderer>();
        coll2D = GetComponent<EdgeCollider2D>();


    }
    private void SpecialFadeIn(float duration)
    {
        StartCoroutine(FadeInCenter(duration));
    }

    private IEnumerator FadeInCenter(float duration)
    {
        float elapsedTime = 0f;

        Color startColor = center.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1); // Target color with 0 alpha

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            center.color = Color.Lerp(startColor, endColor, elapsedTime / 1);
            // burst.color = center.color;
            yield return null;
        }

        center.color = endColor;

    }
    private IEnumerator FadeCenter()
    {
        // Duration of the fade
        float elapsedTime = 0f;

        Color startColor = center.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0); // Target color with 0 alpha

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            center.color = Color.Lerp(startColor, endColor, elapsedTime / 1);
            // burst.color = center.color;
            yield return null;
        }

        center.color = endColor;
        // burst.color = endColor;

    }

    private IEnumerator BurstCenter()
    {
        // Duration of the fade
        float elapsedTime = 0f;



        Color startColor = center.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, .4f); // Target color with 0 alpha

        while (elapsedTime < .22f)
        {
            elapsedTime += Time.deltaTime;
            center.transform.localScale = Vector3.Lerp(startBurstScale, endBurstScale, elapsedTime / .22f);
            center.color = Color.Lerp(startColor, endColor, elapsedTime / .22f);
            // burst.color = center.color;

            yield return null;
        }

        elapsedTime = 0;
        startColor = endColor;
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        while (elapsedTime < .2f)
        {
            elapsedTime += Time.deltaTime;
            center.transform.localScale = Vector3.Lerp(endBurstScale, startBurstScale, elapsedTime / .2f);
            center.color = Color.Lerp(startColor, endColor, elapsedTime / .2f);
            // burst.color = center.color;
            yield return null;

        }

        center.color = endColor;
        // burst.color = center.color;

    }

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        parent = GetComponentInParent<RingParent>();
    }

    // Update is called once per frame
    public void CheckForHighlight(int correctRing)
    {
        if (order == correctRing)
        {
            sprite.material = ID.highlightedMaterial;
            backRing.material = ID.highlightedMaterial;
        }
    }

    public void CheckOrder(bool isCorrect)
    {
        if (isCorrect)
        {

            AudioManager.instance.PlayRingPassSound(order);
            StartCoroutine(BurstCenter());
            ps.Play();

            sprite.material = ID.passedMaterial;
            backRing.material = ID.passedMaterial;

        }


        // print("ring" + order);
    }

    public void Collected()
    {
        CheckOrder(parent.CheckOrder(order));


    }



    // print("ring" + order);




    private void RingEffect()
    {

    }


    private void OnEnable()
    {
        if (order == 1)
        {
            sprite.material = ID.highlightedMaterial;
            backRing.material = ID.highlightedMaterial;

        }
        else
        {
            sprite.material = ID.defaultMaterial;
            backRing.material = ID.defaultMaterial;
        }

        center.color = ID.CenterColor;
       
        coll2D.enabled = true;

        lvlID.outputEvent.SpecialRingFadeIn += SpecialFadeIn;
        // if (order == parent.correctRing)
        // {
        //     sprite.material = ID.highlightedMaterial;
        //     backRing.material = ID.highlightedMaterial;
        // }



    }

    private void OnDisable()
    {
        lvlID.outputEvent.SpecialRingFadeIn -= SpecialFadeIn;


    }
}
