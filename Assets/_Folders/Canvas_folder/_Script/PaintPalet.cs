using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PaintPalet : MonoBehaviour
{
    [SerializeField] private float fadeInDur;
    [SerializeField] private float fadeOutDur;
    [SerializeField] private float appendInDur;
    [SerializeField] private float appendOutDur;
    [SerializeField] private float delayDur;
    [SerializeField] private Image[] paints;
    [SerializeField] private Image[] highlights;
    [SerializeField] private Color[] paintColors;
    [SerializeField] private Color[] highlightColors;

    [Header("Move Settings")]
    private RectTransform rect;
    [SerializeField] private float moveAmount;
    [SerializeField] private float moveDuration;
    [SerializeField] private float moveBrushAmount;


    [SerializeField] private RectTransform brush;

    private Sequence brushSeq;

    private Sequence moveSeq;
    private float startPosY;
    private float startBrushPosY;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        for (int i = 0; i < paints.Length; i++)
        {
            paints[i].color = paintColors[i];
        }
        for (int i = 0; i < paints.Length; i++)
        {
            highlights[i].color = highlightColors[i];
            highlights[i].DOFade(0, 0);

        }
        startPosY = rect.localPosition.y;
        startBrushPosY = brush.localPosition.y;
        IdleTween();
        StartCoroutine(TweenRoutine());

        // HighlightSeq();


    }

    public void OnPress()
    {
        moveSeq.Kill();
        brushSeq.Kill();
        moveSeq = DOTween.Sequence();


        brush.DOShakeRotation(1.2f, 40);
        moveSeq.Append(rect.DOLocalMoveY(startPosY + 200, .8f).SetEase(Ease.OutSine));
        moveSeq.Append(rect.DOLocalMoveY(startPosY + 90, .7f).SetEase(Ease.InOutSine));
        moveSeq.Play().OnComplete(IdleTween);



    }
    private void OnDisable()
    {
        if (moveSeq != null && moveSeq.IsPlaying())
            moveSeq.Kill();
        if (brushSeq != null && brushSeq.IsPlaying())
            brushSeq.Kill();
    }

    private void IdleTween()
    {

        moveSeq = DOTween.Sequence();
        moveSeq.Append(rect.DOLocalMoveY(startPosY - moveAmount, moveDuration).From(startPosY + moveAmount));
        moveSeq.Join(brush.DOLocalMoveY(startBrushPosY + moveBrushAmount, moveDuration).From(startBrushPosY - moveBrushAmount).SetEase(Ease.OutSine));
        moveSeq.Append(rect.DOLocalMoveY(startPosY + moveAmount, moveDuration));
        moveSeq.Join(brush.DOLocalMoveY(startBrushPosY - moveBrushAmount, moveDuration).SetEase(Ease.OutSine));
        moveSeq.Play().SetLoops(-1);


        brushSeq = DOTween.Sequence();
        brushSeq.Append(brush.DORotate(Vector3.forward * -65, 2.8f).From(Vector3.forward * -73));

        brushSeq.Append(brush.DORotate(Vector3.forward * -73, 2.8f));


        brushSeq.Play().SetLoops(-1);

    }

    // private void HighlightSeq()
    // {
    //     highlightSeq = DOTween.Sequence();
    //     for (int i = 0; i < paints.Length; i++)
    //     {
    //         // Start fade-in for the current highlight
    //         highlightSeq.Append(highlights[i].DOFade(1, fadeInDur).SetDelay(delayDur));

    //         // Add a delay to give time before the next highlight fades in
    //         highlightSeq.AppendInterval(appendIntDur);

    //         // Start fade-out while simultaneously preparing the next fade-in
    //         highlightSeq.Join(highlights[i].DOFade(0, fadeOutDur));
    //     }

    //     highlightSeq.Play().SetLoops(-1); // Loop indefinitely
    // }

    // Update is called once per frame
    private IEnumerator TweenRoutine()
    {
        int i = 0;
        while (true)
        {


            highlights[i].DOFade(1, fadeInDur);
            yield return new WaitForSeconds(appendInDur);
            highlights[i].DOFade(0, fadeOutDur).SetEase(Ease.InSine);
            yield return new WaitForSeconds(appendOutDur);
            i++;
            if (i >= highlightColors.Length)
                i = 0;
            yield return null;



        }
    }
}
