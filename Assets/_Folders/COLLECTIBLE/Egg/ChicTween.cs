using UnityEngine;
using DG.Tweening;

public class ChicTween : MonoBehaviour
{

    private Sequence BarnSeq;
    private bool finisedInitialTween = false;

    private float originalX;
    private float originalY;

    private Vector2 centerPos;
    private Vector2 leftPos;
    private Vector2 rightPos;
    private int audioIndex = 0;


    public void DoBarnTween()
    {

        DoChicHappySound();
        originalX = transform.localPosition.x;
        centerPos = new Vector2(originalX, 0);
        leftPos = new Vector2(originalX - .3f, .6f);
        rightPos = new Vector2(originalX + .3f, .6f);
        float initialDur = .5f;
        float secondDur = .4f;
        BarnSeq = DOTween.Sequence();



        BarnSeq.Append(transform.DOMoveY(transform.position.y + .7f, initialDur));
        BarnSeq.Join(transform.DORotate(Vector3.forward * -10, initialDur));
        BarnSeq.Join(transform.DOLocalMoveX(originalX + .3f, initialDur));
        BarnSeq.Join(transform.DOScale(transform.localScale * 1.1f, initialDur));
        BarnSeq.Append(transform.DOLocalMove(centerPos, secondDur));
        BarnSeq.Join(transform.DORotate(Vector3.zero, secondDur));

        // BarnSeq.Join(transform.DOScale(transform.localScale / 1.2f, secondDur));



        BarnSeq.Play().SetEase(Ease.OutSine).OnComplete(FinishSeq);
        // BarnSeq.Join(transform.DOScale(1.2f, .4f));





    }
    void OnEnable()
    {
        DoBarnTween();
    }

    private void FinishSeq()
    {
        float dur = .4f;
        BarnSeq = DOTween.Sequence();

        BarnSeq.Append(transform.DOLocalMove(leftPos, dur).From(centerPos));
        BarnSeq.Join(transform.DORotate(Vector3.forward * 10, dur));


        BarnSeq.Append(transform.DOLocalMove(centerPos, dur));
        BarnSeq.Join(transform.DORotate(Vector3.zero, dur));


        BarnSeq.Append(transform.DOLocalMove(rightPos, dur).OnComplete(DoChicHappySound));
        BarnSeq.Join(transform.DORotate(Vector3.forward * -10, dur));


        BarnSeq.Append(transform.DOLocalMove(centerPos, dur));
        BarnSeq.Join(transform.DORotate(Vector3.zero, dur));

        BarnSeq.Play().SetLoops(-1);

    }

    private void OnDisable()
    {
        if (BarnSeq != null && BarnSeq.IsPlaying()) BarnSeq.Kill();
        DOTween.Kill(transform);
        Destroy(this.gameObject);
    }
    // private void Update()
    // {
    //     transform.Translate(Vector2.left * BoundariesManager.GroundSpeed);
    //     if (transform.position.x < BoundariesManager.leftBoundary)
    //     {
    //         DOTween.Kill(transform);
    //         if (BarnSeq != null && BarnSeq.IsPlaying())
    //             BarnSeq.Kill();

    //     }


    // }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void DoChicHappySound()
    {
        if (audioIndex >= 2) audioIndex = 0;
        AudioManager.instance.PlayChicHappySound(audioIndex);
        audioIndex++;

    }

}

