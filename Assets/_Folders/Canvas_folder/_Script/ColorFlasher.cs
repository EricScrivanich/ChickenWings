using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ColorFlasher : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Image image;
    private Sequence flashSeq;
    [ExposedScriptableObject]
    [SerializeField] private ButtonColorsSO colorSO;

    private Color color1;
    private Color color2;
    [SerializeField] private float flashDur;
    [SerializeField] private float inDur;
    [SerializeField] private float delay;

    private void OnEnable()
    {
        if (flashSeq != null && flashSeq.IsPlaying())
            flashSeq.Kill();
        flashSeq = DOTween.Sequence();


        flashSeq.AppendInterval(delay);
        flashSeq.Append(image.DOColor(colorSO.flashButtonColor1, inDur));

        flashSeq.Play().SetUpdate(true).OnComplete(TweenColors);



    }
    void TweenColors()
    {
        if (flashSeq != null && flashSeq.IsPlaying())
            flashSeq.Kill();
        flashSeq = DOTween.Sequence();

        flashSeq.Append(image.DOColor(colorSO.flashButtonColor2, flashDur));
        flashSeq.Append(image.DOColor(colorSO.flashButtonColor1, flashDur));
        flashSeq.Play().SetUpdate(true).SetLoops(-1);

    }

    public void OnDisable()
    {
        if (flashSeq != null && flashSeq.IsPlaying())
            flashSeq.Kill();

        image.color = colorSO.normalButtonColor;
        // flashSeq = DOTween.Sequence();

        // flashSeq.Append(image.DOColor(colorSO.normalButtonColor, flashDur));

        // flashSeq.Play().SetUpdate(true);

    }
}
