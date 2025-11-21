using UnityEngine;
using DG.Tweening;


public class PigAwakeAnim : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private bool doFlap;
    [SerializeField] private float inTime;
    [SerializeField] private Ease inEase;
    [SerializeField] private float outTime;
    [SerializeField] private Ease outEase;

    [SerializeField] private float startLocalY;
    [SerializeField] private float endLocalY;
    [SerializeField] private Transform pigTransform;
    [SerializeField] private float animationSpeed = 1;

    [SerializeField] private Transform shadow;

    [SerializeField] private Transform shadowEnd;
    [SerializeField] private Vector2 shadowStartEndScale;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.speed = animationSpeed;


        Sequence sequence = DOTween.Sequence();
        sequence.Append(pigTransform.DOLocalMoveY(endLocalY, inTime).SetEase(inEase).From(startLocalY));

        sequence.JoinCallback(DoFlap);
        if (shadow != null)
        {
            sequence.Join(shadow.DOLocalMove(shadowEnd.localPosition, inTime).SetEase(inEase).From(Vector2.zero));
            sequence.Join(shadow.DOScale(shadowStartEndScale.y, inTime).SetEase(inEase).From(shadowStartEndScale.x));
        }


        sequence.Append(pigTransform.DOLocalMoveY(startLocalY, outTime).SetEase(outEase));

        if (shadow != null)
        {
            sequence.Join(shadow.DOLocalMove(Vector2.zero, outTime).SetEase(outEase));
            sequence.Join(shadow.DOScale(shadowStartEndScale.x, outTime).SetEase(outEase));
        }

        sequence.Play().SetLoops(-1);



    }

    void DoFlap()
    {
        if (doFlap)
            anim.SetTrigger("Flap");
    }

    // Update is called once per frame

}
