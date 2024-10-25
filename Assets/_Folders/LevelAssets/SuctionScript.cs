using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SuctionScript : MonoBehaviour
{
    [SerializeField] private AnimationDataSO animSO;
    private bool hasEnteredTrigger = false;
    

    [SerializeField] private float startCircleScale;
    [SerializeField] private float circleTweenDur;
    [SerializeField] private Color startCircleColor;
    [SerializeField] private Color endCircleColor;

    [SerializeField] private float endCircleScale;

    private Sequence circleHighlightSeq;

    [SerializeField] private SpriteRenderer circleHighlight;
    [SerializeField] private float rotateSpeed;
    private TriggerNextSection parentScript;
    [SerializeField] private PlayerID player;
    [SerializeField] private float duration;
    [SerializeField] private bool clockwise;
    private Transform playerTrans;

    [SerializeField] private Material mat;


    private Collider2D colliderObj;
    private Vector3 initialScale = new Vector3(1, 1, 1);
    // [SerializeField] private CircleCollider2D centerCollider;

    [SerializeField] private Transform spriteTrans;
    [SerializeField] private SpriteRenderer spriteRen;

    // private Animator anim;


    // Start is called before the first frame update

    // private void Awake()
    // {

    //     // anim = GetComponent<Animator>();

    //     if (parentScript.isCheckPoint)
    //     {
    //     }

    // }



    private void Start()
    {


        colliderObj = GetComponent<Collider2D>();
        parentScript = GetComponentInParent<TriggerNextSection>();
        circleHighlight.gameObject.SetActive(false);


        if (!parentScript.isCheckPoint)
        {
            colliderObj.enabled = false;
            spriteTrans.DOScale(initialScale, .6f).From(.25f).SetUpdate(true).OnComplete(() => colliderObj.enabled = true);
            spriteRen.DOFade(.75f, .5f).From(0).SetUpdate(true);
            mat.SetFloat("_WaveStrength", 5.4f);
            mat.SetFloat("_Glow", 6.5f);
            StartCoroutine(SetGlow());

        }
        else
        {
            mat.SetFloat("_WaveStrength", 0f);
            mat.SetFloat("_Glow", 0f);
            colliderObj.enabled = false;
            MakeSmallTween(true);

        }
    }

    private void OnStartFinished()
    {

    }

    private void CircleHighlightTween(bool show)
    {
        if (circleHighlightSeq != null && circleHighlightSeq.IsPlaying())
            circleHighlightSeq.Kill();

        circleHighlightSeq = DOTween.Sequence();

        if (show)
        {
            circleHighlight.gameObject.SetActive(true);
            circleHighlightSeq.Append(circleHighlight.transform.DOScale(endCircleScale, circleTweenDur).From(startCircleScale));
            circleHighlightSeq.Join(circleHighlight.DOColor(endCircleColor, circleTweenDur).From(startCircleColor).SetEase(Ease.OutSine));
            circleHighlightSeq.Play().SetUpdate(true).SetLoops(-1);

        }
        else
        {
            circleHighlightSeq.Append(circleHighlight.DOFade(0, .4f));
            circleHighlightSeq.Play().SetUpdate(true);
        }
    }

    private IEnumerator SetGlow()
    {
        float duration = 1.4f;
        float t = 0;
        // yield return new WaitForSecondsRealtime(0.5f);

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            // Calculate normalized time (0 to 1)
            float normalizedTime = t / duration;

            // Use SmoothStep for easing out effect
            float easedValue = Mathf.SmoothStep(6.5f, 0, normalizedTime);

            // Set the material's glow using the eased value
            mat.SetFloat("_Glow", easedValue);

            yield return null;
        }
        // Ensure the glow is set to 0 at the end
        mat.SetFloat("_Glow", 0);
        if (!hasEnteredTrigger)
            CircleHighlightTween(true);
    }


    private IEnumerator SpriteAnim()
    {
        float d = .04f;
        for (int i = 0; i < 16; i++)
        {
            spriteRen.sprite = animSO.sprites[i];
            yield return new WaitForSecondsRealtime(d);
            d += .004f;
        }
        spriteTrans.eulerAngles = Vector3.zero;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasEnteredTrigger) return;

        hasEnteredTrigger = true;

        Vector2 direction = other.transform.position - transform.position;
        CircleHighlightTween(false);


        // Calculate the angle in degrees, and add 90 degrees to rotate the sprite
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 2;

        // Apply the calculated rotation to the sprite's transform on the Z axis
        spriteTrans.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        spriteTrans.DORotate(spriteTrans.rotation.eulerAngles * 1.25f, .5f);
        mat.SetFloat("_WaveStrength", 0);


        StartCoroutine(SpriteAnim());
        AudioManager.instance.PlayBlobNoise(true);

        parentScript.TriggerEventOnEnterSuction();
        player.globalEvents.OnEnterNextSectionTrigger?.Invoke(duration, parentScript.duration, clockwise, transform, parentScript.setPlayerPositionTransform.position, true);
        colliderObj.enabled = false;
        // Vector2 direction = other.transform.position - transform.position;

        // // Determine the angle in degrees
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // // Apply the rotation with an offset of -90 degrees
        // transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180));
        // // anim.SetTrigger("EnterTrigger");
        // MakeSmallTween();
        MakeSmallTween(false);



    }


    private void MakeSmallTween(bool checkPoint)
    {
        if (checkPoint)
            spriteTrans.DOScale(initialScale * .55f, duration).SetEase(Ease.InSine).SetUpdate(true);

        else
            spriteTrans.DOScale(initialScale * .55f, duration).SetEase(Ease.InSine).OnComplete(() => { parentScript.EnterSection(); });
        // StartCoroutine(ShrinkCollider());



    }
    private IEnumerator ShrinkCollider()
    {
        float elapsedTime = 0;
        float duration = .8f;
        // float startRadius = centerCollider.radius;
        // float targetRadius = startRadius * .3f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;



            yield return new WaitForFixedUpdate();

        }




        // parentTriggerCollider.enabled = true;

    }





    private void OnEnable()
    {
        // player.globalEvents.OnFinishSectionTrigger += SetSectionActive;



    }

    private void OnDisable()
    {
        // player.globalEvents.OnFinishSectionTrigger -= SetSectionActive;


    }

    // Update is called once per frame

}
