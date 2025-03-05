using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EggShield : MonoBehaviour
{
    public Transform followPos;
    [SerializeField] private Sprite[] images;
    private CapsuleCollider2D coll;
    private SpriteRenderer mainSprite;
    [SerializeField] private SpriteRenderer glow;
    private float glowStartAlpha;
    [SerializeField] private float glowFadeDur;
    [SerializeField] private float mainFadeDur;
    [SerializeField] private float startScale;
    [SerializeField] private float endScale;
    private int currentFrame;
    private Sequence endSeq;
    [SerializeField] private float parryDur;
    private float time;
    private bool isActive;

   

[SerializeField] private float[] frameTimes;
// private Rigidbody2D rb;

// Start is called once before the first execution of Update after the MonoBehaviour is created
// void Start()
// {

// }
private void Awake()
{
    coll = GetComponent<CapsuleCollider2D>();
    mainSprite = GetComponent<SpriteRenderer>();

}
public void Initialize(Vector2 pos)
{
    if (endSeq != null && endSeq.IsPlaying())
        endSeq.Kill();
    transform.position = pos;
    glow.enabled = false;

    transform.localScale = Vector3.one * startScale;
    mainSprite.DOFade(1, frameTimes[1]).From(.7f);
    isActive = true;
    coll.enabled = true;
    time = 0;
    currentFrame = 0;
    mainSprite.sprite = images[0];
    AudioManager.instance.PlayParryNoise(false);
    gameObject.SetActive(true);
}

private void FixedUpdate()
{

    if (isActive)
    {
        time += Time.fixedDeltaTime;

        float l = Mathf.Lerp(startScale, endScale, time / parryDur);
        transform.localScale = Vector3.one * l;


        if (currentFrame == 0 && time > frameTimes[0])
        {
            currentFrame++;
            mainSprite.sprite = images[1];


        }
        else if (currentFrame == 1 && time > frameTimes[1])
        {
            currentFrame++;
            glow.DOFade(0, glowFadeDur).From(glowStartAlpha);
            glow.enabled = true;
            mainSprite.sprite = images[2];


        }
        else if (time > parryDur)
        {
            isActive = false;
            coll.enabled = false;
            DoEndSeq();


            // mainSprite.DOFade(0, mainFadeDur);
        }

    }



}

private void DoEndSeq()
{
    endSeq = DOTween.Sequence();
    endSeq.AppendInterval(.1f);
    endSeq.Append(transform.DOScale(endScale, .12f));
    endSeq.Join(mainSprite.DOFade(0, .12f));
    endSeq.Play().OnComplete(() => gameObject.SetActive(false));
}

private void OnTriggerEnter2D(Collider2D other)
{
    // if (doingTimer) return;



   

    // if (parryableAttack.parryWindow == -1) playerMan.HandleDamaged();

    // else if (isParrying)
    // {
    //     parryableAttack.Parry();
    //     playerMan.HandleSuccesfulParry();
    // }
    // else if (parryableAttack.parryWindow == 0) playerMan.HandleDamaged();
    // else if (!doingTimer)
    // {
    //     lastAttack = parryableAttack;
    //     parryTimer = StartCoroutine(ParryCoroutine(parryableAttack.parryWindow));

    // }

    // if (parryableAttack != null)
    // {
    //     parryableAttack.Parry();
    //     player.events.OnSuccesfulParry?.Invoke();
    // }

}





    // Update is called once per frame
    // void LateUpdate()
    // {
    //     transform.position = followPos.position;

    // }
}
