using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HotAirBalloon : MonoBehaviour
{


    private int currentSpriteIndex = 0;

    private readonly float spriteSwitchTime = .08f;

    public int type;
    public float speed;

    public float xTrigger;
    public float yTarget;
    public float delay;




    private Rigidbody2D rb;
    private float time = 0;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private AnimationDataSO animData;
    [SerializeField] private AnimationDataSO pigAnimData;
    [SerializeField] private ExplosivesPool pool;
    [SerializeField] private float dropDelay;
    [SerializeField] private float initialDropDelay;


    [SerializeField] private Transform dropPosition;
    [SerializeField] private Vector2 minMaxYVel;
    [SerializeField] private float velocityLerpSpeed;
    [SerializeField] private float liftForce = 5f;

    private bool addingYForce;
    private float targetYVelocity;

    private Animator anim;
    private bool startedAnim;
    private float startDelayTimer = 0;


    // Start is called before the first frame update


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!startedAnim)
        {
            startDelayTimer += Time.deltaTime;

            if (startDelayTimer > xTrigger)
            {
                anim.speed = Random.Range(.75f, .9f);
                startedAnim = true;
            }

        }

        time += Time.deltaTime;


        if (time > spriteSwitchTime)
        {
            currentSpriteIndex++;

            if (currentSpriteIndex > animData.sprites.Length - 1)
            {
                currentSpriteIndex = 0;
            }
            sr.sprite = animData.sprites[currentSpriteIndex];
            time = 0;

        }


    }

    private void OnEnable()
    {
        time = 0;
        startDelayTimer = 0;
        startedAnim = false;
        currentSpriteIndex = Random.Range(0, animData.sprites.Length - 1);
        sr.sprite = animData.sprites[currentSpriteIndex];
        rb.velocity = new Vector2(speed, 0);
        anim.speed = 0;


    }

    private void FixedUpdate()
    {

        if (transform.position.y > yTarget + .15f && addingYForce)
        {
            // When above the yTarget, lerp down to minYVelocity

            addingYForce = false;
            targetYVelocity = minMaxYVel.x;
        }
        else if (transform.position.y < yTarget - .2f && !addingYForce)
        {
            // When below the yTarget, lerp up to maxYVelocity
            targetYVelocity = minMaxYVel.y;
            addingYForce = true;
        }

        // Lerp the current Y velocity towards the target Y velocity
        float newYVelocity = Mathf.Lerp(rb.velocity.y, targetYVelocity, velocityLerpSpeed * Time.fixedDeltaTime);

        // Set the Rigidbody2D's velocity with the updated Y velocity
        rb.velocity = new Vector2(rb.velocity.x, newYVelocity);



    }

    private void LerpYVelocity()
    {



    }

    private void ApplyLiftForce()
    {
        // Apply an upward force to simulate lift
        rb.AddForce(Vector2.up * liftForce);
    }




    // private IEnumerator DropBombRoutine()
    // {
    //     yield return new WaitForSeconds(initialDropDelay);

    //     while (true)
    //     {
    //         // pig.DOLocalMoveY(.15f, .2f);

    //         // for[int i = 0; initialDropDelay < ]
    //         // yield return new WaitForSeconds(.05f);
    //         // armSR.sprite = pigAnimData.sprites[0];



    //         yield return new WaitForSeconds(dropDelay);
    //         Debug.Log("Drop bomb");
    //         pool.GetBalloonBomb(dropPosition.position, new Vector2(-speed, 0));
    //     }

    // }

    public void DropBomb()
    {
        pool.GetBalloonBomb(dropPosition.position, new Vector2(speed, 0));

    }
}
