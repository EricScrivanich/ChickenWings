using UnityEngine;
using System;

public class RingMovement : MonoBehaviour
{
    public int doesTriggerInt;
    public float xCordinateTrigger;
    private bool hasNotTriggered;
    public RingID ID;
    public float speed;
    public int order;
    private bool isCorrect = false;
    [SerializeField] private Collider2D coll2D;

    private Transform _transform;

    [SerializeField] private SpriteRenderer centerSprite;
    [SerializeField] private ParticleSystem particles;

    private bool isFaded = false;
    private Collider2D[] colliders;

    private SpriteRenderer sprite;
    private Animator anim;

    // Declare the hash
    private static readonly int BurstBoolHash = Animator.StringToHash("BurstBool");
    private static readonly int FadeCenterHash = Animator.StringToHash("FadeCenterBool");


    private void Awake()
    {
        _transform = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        colliders = GetComponents<Collider2D>();
    }

    private void Update()
    {
        _transform.position += Vector3.left * speed * Time.deltaTime;


        if (doesTriggerInt != 0 && hasNotTriggered)
        {
            if ((speed > 0 && _transform.position.x < xCordinateTrigger) || (speed < 0 && _transform.position.x > xCordinateTrigger))
            {
                ID.ringEvent.OnRingTrigger?.Invoke(doesTriggerInt);
                hasNotTriggered = false;


            }

        }

        if (speed > 0)
        {

            if (isCorrect && _transform.position.x < BoundariesManager.leftViewBoundary)
            {
                ID.ringEvent.OnCreateNewSequence?.Invoke(false);
                Debug.Log("TooLong" + order);


                isCorrect = false;
            }

            else if (_transform.position.x < BoundariesManager.leftBoundary)
            {

                gameObject.SetActive(false);
            }

        }

        else
        {
            if (isCorrect && _transform.position.x > BoundariesManager.rightViewBoundary)
            {
                ID.ringEvent.OnCreateNewSequence?.Invoke(false);
                Debug.Log("TooLong");

                isCorrect = false;
            }

            else if (_transform.position.x > BoundariesManager.rightBoundary)
            {

                gameObject.SetActive(false);
            }


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

    public void SetCorrectRing()
    {
        if (order == ID.CorrectRing)
        {
            sprite.material = ID.highlightedMaterial;
            isCorrect = true;
        }

    }
    public void CheckOrder()
    {

        if (order == ID.CorrectRing)
        {

            isCorrect = false;
            ID.CorrectRing++;

            DisableColliders();
            isCorrect = false;
            AudioManager.instance.PlayRingPassSound();
            anim.SetBool(BurstBoolHash, true);
            sprite.material = ID.passedMaterial;
            Debug.Log(isCorrect);
            ID.ringEvent.OnCheckOrder?.Invoke();

        }

        else
        {
            ID.ringEvent.OnCreateNewSequence?.Invoke(false);
            isCorrect = false;
            Debug.Log("Resseting Rings");
        }

        // print("ring" + order);
    }

    public void FadeCenter()
    {



    }

    public void NewSetup(bool correctSequence)
    {
        if (correctSequence == false)
        {
            DisableColliders();
            isFaded = true;
            isCorrect = false;
            anim.SetBool(FadeCenterHash, true);
        }


    }



    public int GetOrder()
    {
        return order;
    }

    private void OnEnable()
    {
        sprite.material = ID.defaultMaterial;
        centerSprite.color = ID.CenterColor;
        particles = ID.ringParticles;

        ID.ringEvent.OnCheckOrder += SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence += NewSetup;

        EnableColliders();
        hasNotTriggered = true;

    }

    void OnDisable()
    {
        ID.ringEvent.OnCheckOrder -= SetCorrectRing;
        ID.ringEvent.OnCreateNewSequence -= NewSetup;
        if (!isFaded)
        {
            anim.SetBool(BurstBoolHash, false); 
        }
        else
        {
            anim.SetBool(FadeCenterHash, false);
            isFaded = false;

        }

        if (sprite != null)
        {
            
            coll2D.enabled = true;
        }
        isCorrect = false;
    }
}
