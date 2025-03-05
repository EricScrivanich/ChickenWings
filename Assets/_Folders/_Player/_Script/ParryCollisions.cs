using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ParryCollisions : MonoBehaviour
{
    [SerializeField] private PlayerID player;
    [SerializeField] private GameObject parryExplosionPrefab;
    private GameObject parryExplosion;


    [SerializeField] private float perfectParryTime;
    [SerializeField] private float parryTime;
    private float timer;

    [SerializeField] private Vector2 startEndScale;

    private PlayerStateManager playerMan;
    private Coroutine parryTimer;
    private bool isParrying = false;
    private bool doingTimer;
    private CircleCollider2D col;
    private IParryable lastAttack;
    private bool success = false;

    private Sequence circleSeq;

    [SerializeField] private SpriteRenderer circleLine;
    [SerializeField] private SpriteRenderer circleBlur;
    [SerializeField] private Transform circle;

    [SerializeField] private float circleStartScale;
    [SerializeField] private float circleEndScale;
    [SerializeField] private float circleBigScale;
    [SerializeField] private float circleParryDur;
    [SerializeField] private float circleBigDur;
    private CapsuleCollider2D coll;
    private bool hasInitilaized = false;



    // private void Start()
    // {

    //     circleLine.enabled = false;
    //     circleBlur.enabled = false;


    // }
    private void Start()
    {
        playerMan = GetComponentInParent<PlayerStateManager>();
        coll = GetComponent<CapsuleCollider2D>();
        if (parryExplosion == null)
        {
            parryExplosion = Instantiate(parryExplosionPrefab);
            parryExplosion.SetActive(false);
        }

        hasInitilaized = true;
        playerMan.SetColliders(null, this);



        // col = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        if (hasInitilaized)
        {
            hasParried = false;
            timer = 0;
            coll.enabled = true;

            transform.localScale = Vector3.one * startEndScale.x;
            canParry = true;
            canPerfectParry = true;
        }

    }
    private bool canParry;
    private bool canPerfectParry;

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (canParry)
        {


            float s = Mathf.Lerp(startEndScale.x, startEndScale.y, timer / parryTime);

            if (canPerfectParry && timer >= perfectParryTime) canPerfectParry = false;
            else if (canParry && timer >= parryTime)
            {
                canParry = false;
                coll.enabled = false;
                playerMan.SetParry(false);

            }

        }
        else if (timer > .36f) gameObject.SetActive(false);



    }

    private bool hasParried;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (hasParried) return;
        if (collider.CompareTag("Plane") || collider.CompareTag("Block")) // && !isFlashing 
        {
            playerMan.HandleDamaged();
            return;

        }
        else if (collider.CompareTag("Weapon"))
        {
            if (canPerfectParry || canParry)
            {
                hasParried = true;
                IParryable parryableAttack = collider.gameObject.GetComponent<IParryable>();
                if (parryableAttack != null)
                {


                    parryableAttack.Parry();
                    AudioManager.instance.PlayParryNoise(true);
                    parryExplosion.transform.position = transform.position;
                    parryExplosion.SetActive(true);
                    canParry = false;
                    coll.enabled = false;
                    playerMan.SetUndamagable(false, .5f);
                    playerMan.SetParry(false);
                    gameObject.SetActive(false);
                }
            }


            else playerMan.HandleDamaged();


        }

        IExplodable explodableEntity = collider.gameObject.GetComponent<IExplodable>();
        if (explodableEntity != null)
        {
            explodableEntity.Explode(false);
        }


    }

    // if (parryableAttack != null)
    // {
    //     parryableAttack.Parry();
    //     player.events.OnSuccesfulParry?.Invoke();
    // }



    private IEnumerator ParryCoroutine(float dur)
    {
        doingTimer = true;
        yield return new WaitForSeconds(dur);
        if (!isParrying && doingTimer) playerMan.HandleDamaged();
        doingTimer = false;


    }


    public void SuccesfulParry()
    {
        circleLine.enabled = false;
        // cursor.EnableSwiper(playerMan.transform.position);
        transform.localScale = BoundariesManager.vectorThree1 * .6f;
        if (circleSeq != null && circleSeq.IsPlaying())
            circleSeq.Kill();
        circleSeq = DOTween.Sequence();
        circleBlur.enabled = true;

        circleSeq.Append(circleBlur.DOFade(1, circleBigDur).From(0));
        circleSeq.Join(circle.DOScale(circleBigScale, circleBigDur).From(circleEndScale));

        circleSeq.Play().SetEase(Ease.OutSine);

        success = true;

        // transform.DOScale(1.2f, .3f);
        // sr.DOFade(.1f, .3f).OnComplete(Reset);
    }

    public void StopAttackMode()
    {
        isParrying = false;
        success = false;


        if (circleSeq != null && circleSeq.IsPlaying())
            circleSeq.Kill();
        circleSeq = DOTween.Sequence();

        circleSeq.Append(circleBlur.DOFade(0, .3f).From(1));
        circleSeq.Join(circle.DOScale(circleBigScale * 1.1f, .3f).From(circleBigScale));
        circleSeq.Play().OnComplete(() => circleBlur.enabled = false);




    }

    private void Reset()
    {
        isParrying = false;
        success = false;
        transform.localScale = BoundariesManager.vectorThree1 * .6f;

        col.enabled = true;
    }
    public void DoParry(bool on)
    {
        if (circleSeq != null && circleSeq.IsPlaying())
            circleSeq.Kill();

        if (on)
        {
            if (doingTimer)
            {
                StopCoroutine(parryTimer);
                doingTimer = false;
                lastAttack.Parry();
                playerMan.HandleSuccesfulParry();
                return;

            }
            isParrying = true;
            transform.localScale = BoundariesManager.vectorThree1 * .7f;
            circleLine.enabled = true;
            circleBlur.enabled = false;

            circleSeq = DOTween.Sequence();

            circleSeq.Append(circle.DOScale(circleEndScale, circleParryDur).From(circleStartScale));
            // circleSeq.Join(circleLine.DOFade(.4f, circleParryDur).From(1));
            circleSeq.Play().SetEase(Ease.OutSine);
            // sr.enabled = true;



        }
        else if (!success)
        {
            circleLine.enabled = false;
            isParrying = false;
            transform.localScale = BoundariesManager.vectorThree1 * .6f;
            // sr.enabled = false;

        }



    }


}
