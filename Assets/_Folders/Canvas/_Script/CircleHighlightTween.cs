using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CircleHighlightTween : MonoBehaviour
{
    private Sequence sequence;
    private RectTransform target;
    private Image targetImage;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 endScale;
    [SerializeField] private float duration;
    [SerializeField] private float startAlpha;
    [SerializeField] private bool pressButton;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;


    // Start is called before the first frame update


    void OnEnable()
    {
        if (pressButton)
        {
            PressButtonTween();
        }
        else
            Tween();

    }
    void Awake()
    {
        target = GetComponent<RectTransform>();
        targetImage = GetComponent<Image>();
    }

    // Update is called once per frame


    private void OnDisable()
    {
        if (sequence != null && sequence.IsPlaying())
            sequence.Kill();

    }

    void Tween()
    {
        if (sequence != null && sequence.IsPlaying())
            sequence.Kill();

        sequence = DOTween.Sequence();

        // Set the initial scale and alpha before starting the tween
        target.localScale = startScale;
        Color color = targetImage.color;
        color.a = startAlpha;
        targetImage.color = color;

        // Create the tween sequence
        sequence.Append(target.DOScale(endScale, duration).SetEase(Ease.OutSine).SetUpdate(true));
        sequence.Join(targetImage.DOFade(0, duration).SetEase(Ease.InSine).SetUpdate(true));

        // Loop the sequence infinitely
        sequence.SetLoops(-1, LoopType.Restart);
        sequence.Play().SetUpdate(true);

    }

    void PressButtonTween()
    {

        if (sequence != null && sequence.IsPlaying())
            sequence.Kill();

        sequence = DOTween.Sequence();

        // Set the initial scale and alpha before starting the tween
        target.localScale = startScale;
        Color color = targetImage.color;
        color.a = startAlpha;
        targetImage.color = color;

        // Create the tween sequence
        sequence.Append(target.DOScale(endScale, duration).SetEase(Ease.OutSine).From(startScale));
        sequence.Join(targetImage.DOColor(color2, duration).From(color1));

        sequence.Append(target.DOScale(startScale, duration).SetEase(Ease.OutSine));
        sequence.Join(targetImage.DOColor(color1, duration));


        // Loop the sequence infinitely
        sequence.SetLoops(-1);
        sequence.Play().SetUpdate(true);

    }

}
