using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShotgunBlast : MonoBehaviour
{
    
    [SerializeField] private float forceAmount;
    [SerializeField] private int xForceMultiplier;
    private Rigidbody2D rb;
    private Sequence sequence;

    private Color startColor = new Color(1, 1, 1, 1);
    private Color endColor = new Color(.4f, .35f, .35f, 0f);
    private BoxCollider2D col;

    public Sprite[] img;
    public float[] spriteDelays;
    public Vector2[] scales;

    private Vector3 StartScale = new Vector3(.25f, .5f, 1);

    public float[] opacities;
    public float[] scaleDelays;
    private float time;
    private int currentSpriteDelayIndex;
    private int currentScaleDelayIndex;
    private bool finished = false;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update


    private void OnEnable()
    {
        col.enabled = true;
        transform.localScale = StartScale;
        sr.color = startColor;
        time = 0;
        sr.sprite = img[0];
        Vector2 force = transform.right * forceAmount;


        float xVelRatio = force.x / forceAmount;
        float addedX = 0;

        // if (yVelRatio > 0) addedY = yVelRatio * yForceMultiplier;
        // else if (yVelRatio < 0) addedY = yVelRatio * yForceMultiplier * -1;
        addedX = Mathf.Abs(xVelRatio * xForceMultiplier);

        Vector2 finalForce = new Vector2(force.x - addedX, force.y);


        rb.velocity = finalForce;

        ScaleAndOpacity();


    }

    // public void Initilaize(Vector2 direction)
    // {
    //     transform.localScale = StartScale;
    //     sr.color = startColor;
    //     gameObject.SetActive(true);
    //     rb.velocity = direction * forceAmount;

    // }
    private void OnDisable()
    {

        finished = false;
        currentScaleDelayIndex = 0;
    }

    private void Update()
    {

        if (finished) return;
        else time += Time.deltaTime;
        if (time > spriteDelays[currentScaleDelayIndex] && !finished)
        {
            currentSpriteDelayIndex++;
            sr.sprite = img[currentSpriteDelayIndex];
            time = 0;
            if (currentSpriteDelayIndex > spriteDelays.Length - 1)
            {
                currentSpriteDelayIndex = 0;
                finished = true;
                time = 0;
                return;

            }
        }
    }

    private void ScaleAndOpacity()
    {
        // Ensure to kill any previous sequence if it's still running
        // if (sequence != null && sequence.IsActive())
        // {
        //     sequence.Kill();
        // }

        // Create a new sequence
        sequence = DOTween.Sequence();

        // Iterate over the scales, opacities, and scaleDelays arrays
        for (int i = 0; i < scales.Length; i++)
        {

            if (i == scales.Length - 1)
            {
                sequence.AppendCallback(() => Invoke("DisableCollider", .15f));
                sequence.Append(transform.DOScale(scales[i], scaleDelays[i]).SetEase(Ease.OutSine));
                sequence.Join(sr.DOColor(endColor, scaleDelays[i]).SetEase(Ease.OutSine));
            }
            else
            {
                sequence.Append(transform.DOScale(scales[i], scaleDelays[i]));
                sequence.Join(sr.DOFade(opacities[i], scaleDelays[i]));

            }

            // Append the scaling tween to the sequence







            // Optionally, add a callback at each step for debugging or additional functionality

        }

        sequence.Play();

        // Once the sequence is complete, mark the animation as finished
        // sequence.OnComplete(() => gameObject.SetActive(false));
    }

    private void DisableCollider()
    {
        col.enabled = false;
    }


}
