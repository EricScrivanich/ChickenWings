using UnityEngine;
using DG.Tweening;


public class PigAwakeAnim : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float inTime;
    [SerializeField] private Ease inEase;
    [SerializeField] private float outTime;
    [SerializeField] private Ease outEase;

    [SerializeField] private float startLocalY;
    [SerializeField] private float endLocalY;
    [SerializeField] private Transform pigTransform;
    [SerializeField] private float animationSpeed = 1;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.speed = animationSpeed;


        Sequence sequence = DOTween.Sequence();
        sequence.Append(pigTransform.DOLocalMoveY(endLocalY, inTime).SetEase(inEase).From(startLocalY))
        .JoinCallback(DoFlap)

                .Append(pigTransform.DOLocalMoveY(startLocalY, outTime).SetEase(outEase)).SetLoops(-1);
        ;


    }

    void DoFlap()
    {
        anim.SetTrigger("Flap");
    }

    // Update is called once per frame

}
