using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TipSignMovement : MonoBehaviour
{
    [SerializeField] private bool dropOnEnable;
    [SerializeField] private RectTransform target;


    [SerializeField] private float dropDelay;
    private Vector2 endPosition = new Vector2(0, -700);


    private Sequence sequence;
    // Start is called before the first frame update

    private void OnEnable()
    {
        if (dropOnEnable) DropSignTween();
    }

    // Update is called once per frame

    public void DropSignTween()
    {
        if (sequence != null && sequence.IsPlaying())
        {
            sequence.Kill();
        }

        else
        {
            target.anchoredPosition = Vector2.zero;
            Debug.Log("zero anchor");
        }
        Vector3 overshootPosition = endPosition - new Vector2(0, 30);

        sequence = DOTween.Sequence();
        sequence.AppendInterval(dropDelay).SetUpdate(true);
        sequence.Append(target.DOAnchorPos(endPosition, 1.1f)
            .SetEase(Ease.Linear)
            .SetUpdate(true));

        sequence.Append(target.DOAnchorPos(overshootPosition, .8f)
                    .SetEase(Ease.OutElastic)
                    .SetUpdate(true));

        sequence.Play();
        // Create the main drop tween
        // Tweener mainDropTween = target.DOAnchorPos(endPosition, 1.1f)
        //     .SetEase(Ease.Linear)
        //     .SetUpdate(true);

        // // Create the overshoot tween

        // Tweener overshootTween = target.DOAnchorPos(overshootPosition, .8f)
        //     .SetEase(Ease.OutElastic)
        //     .SetUpdate(true);

        // // Chain the tweens together
        // sequence = DOTween.Sequence();
        // sequence.AppendInterval(dropDelay).SetUpdate(true)
        // .Append(mainDropTween).SetUpdate(true)
        //         .Append(overshootTween).SetUpdate(true)
        //         .SetUpdate(true);
    }

    public void Retract()
    {
        if (sequence != null && sequence.IsPlaying())
        {
            sequence.Kill();
        }


        float currentPos = target.anchoredPosition.y - 70;


        Tweener bounceTween = target.DOAnchorPosY(currentPos, .35f).SetEase(Ease.OutSine).SetUpdate(true);





        Tweener riseTween = target.DOAnchorPosY(0, .5f)
           .SetEase(Ease.InSine)
           .SetUpdate(true);


        sequence = DOTween.Sequence();
        sequence.Append(bounceTween).SetUpdate(true);
        sequence.Append(riseTween)
                .SetUpdate(true);

    }

}
