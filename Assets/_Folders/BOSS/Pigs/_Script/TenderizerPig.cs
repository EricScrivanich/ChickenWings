using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenderizerPig : MonoBehaviour, ICollectible
{
    public float speed;
    public bool hasHammer;
    private CircleCollider2D detection;
    private float amplitude = .3f;
    private float baseFrequency = 1.4f;
    private float baseSpeed = 3.5f;
    private float frequency = 1.4f;
    private float initialY;

    private bool parried = false;
    private Animator anim;
    [SerializeField] private GameObject Hammer;

    private Vector2 flippedDirection = new Vector2(-1, 1);
    private Transform player; // Reference to the player
    public Transform eye; // Reference to the eye
    public Transform pupil; // Reference to the pupil
    public float eyeRadius = 0.5f; // Radius within which the pupil can move

    private bool flipped;



    private void Awake()
    {
        detection = GetComponent<CircleCollider2D>();
    }
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        anim = GetComponent<Animator>();





    }


    private void OnEnable()
    {
        Ticker.OnTickAction015 += MoveEyesWithTicker;
        frequency = baseFrequency + ((baseSpeed - Mathf.Abs(speed)) * .27f);
        initialY = transform.position.y;
        flipped = false;
        if (speed < 0)
        {
            flipped = true;
            transform.eulerAngles = new Vector3(0, 180, 0);
            speed *= -1;
        }

        detection.enabled = hasHammer;
        Hammer.SetActive(hasHammer);

    }

    private void OnDisable()
    {
        Ticker.OnTickAction015 -= MoveEyesWithTicker;
    }
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);


        float y = Mathf.Sin(transform.position.x * frequency) * amplitude + initialY;
        transform.position = new Vector2(transform.position.x, y);

    }

    private void MoveEyesWithTicker()
    {
    
        if (player != null && hasHammer)
        {
            Vector2 direction = player.position - pupil.position; // Calculate the direction to the player
            // Ensure it's 2D
            direction.Normalize(); // Normalize the direction

            if (flipped)
            {
                direction *= flippedDirection;
            }

            // Move the pupil within the eye's radius
            pupil.localPosition = direction * eyeRadius;
        }

    }

    private IEnumerator AfterSwing()
    {
        yield return new WaitForSeconds(.45f);
        AudioManager.instance.PlayPigHammerSwingSound();
        yield return new WaitForSeconds(1.9f);

        if (!parried)
            detection.enabled = true;

        else
        {
            yield return new WaitForSeconds(4f);
            parried = false;
            detection.enabled = true;

        }



    }

    public void GetParried()
    {
        detection.enabled = false;
        anim.SetTrigger("Parried");
        parried = true;
    }

    public void Collected()
    {
        detection.enabled = false;
        anim.SetTrigger("SwingTrigger");
        StartCoroutine(AfterSwing());
        // Invoke("PlaySound", .5f);

    }

    private void PlaySound()
    {
        AudioManager.instance.PlayPigHammerSwingSound();
    }
    // Update is called once per frame

}
