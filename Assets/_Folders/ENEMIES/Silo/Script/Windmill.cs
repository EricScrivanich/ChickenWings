using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : MonoBehaviour
{

    [SerializeField] private Rigidbody2D fanRb;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private HingeJoint2D joint;

    public int bladeAmount;
    public float bladeScaleMultiplier;
    public float bladeSpeed;
    public int startRot;
    [SerializeField] private Vector2 minMaxWindSoundDelay;
    [SerializeField] private Vector2 minMaxWindSpeed;
    private float windMillSoundDelay;
    private float windMillSoundTimer;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<HingeJoint2D>();
    }
    private void OnEnable()
    {
        AdjustHeightToGround();
        fanRb.rotation = startRot;
        windMillSoundTimer = 0;
        rb.velocity = Vector2.left * BoundariesManager.GroundSpeed;
        fanRb.transform.localScale = BoundariesManager.vectorThree1 * bladeScaleMultiplier;
        Move();
    }
    private void AdjustHeightToGround()
    {
        if (sr == null)
        {
            Debug.LogError("SpriteRenderer not found on windmill.");
            return;
        }
        float desiredTotalHeight = Mathf.Abs(transform.position.y - BoundariesManager.GroundPosition + .15f);
        sr.size = new Vector2(sr.size.x, desiredTotalHeight);

    }
    void Move()
    {

        fanRb.angularVelocity = bladeSpeed;

        AudioManager.instance.ChangeWindMillPitch(Mathf.Lerp(minMaxWindSoundDelay.x, minMaxWindSoundDelay.y, (Mathf.Abs(bladeSpeed) - minMaxWindSpeed.x) / (minMaxWindSpeed.y - minMaxWindSpeed.x)));

        windMillSoundDelay = 360 / bladeAmount / Mathf.Abs(bladeSpeed);
    }



    // Update is called once per frame
    void FixedUpdate()
    {

        if (transform.position.x < BoundariesManager.leftBoundary)
        {
            gameObject.SetActive(false);
        }
        else
        {
            windMillSoundTimer += Time.fixedDeltaTime;

            if (windMillSoundTimer >= windMillSoundDelay)
            {
                AudioManager.instance.PlayWindMillSound();
                windMillSoundTimer = 0;

            }

        }
    }
}
