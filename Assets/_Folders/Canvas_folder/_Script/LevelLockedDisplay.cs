using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class LevelLockedDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private CanvasGroup group;
    private Sequence sequence;
    [SerializeField] private RectTransform rect;

    [SerializeField] private GameObject exitButton;

    [SerializeField] private float baseHeight;
    [SerializeField] private float extraLineHeight;
    [SerializeField] private float padding;

    // Start is called before the first frame update


    public void Show(string s, bool showExit, bool enlarge)
    {
        gameObject.SetActive(true);
        if (group == null) group = GetComponent<CanvasGroup>();
        text.text = s;

        AdjustHeight();

        group.alpha = 0;

        // rect.anchoredPosition = Vector2.up * rect.sizeDelta.y * transform.localScale.y * .5f;
        rect.anchoredPosition = Vector2.zero;

        if (enlarge)
        {
            transform.localScale = new Vector3(.95f, .95f, .95f);
        }
        if (!showExit)
        {
            exitButton.SetActive(false);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(group.DOFade(1, .4f).SetEase(Ease.OutSine));

            sequence.Join(rect.DOAnchorPosY(15, 1.4f).SetEase(Ease.InOutSine));

            sequence.Append(rect.DOAnchorPosY(0, .75f).SetEase(Ease.InSine));
            sequence.Append(rect.DOAnchorPosY(-30, .45f));
            sequence.Join(group.DOFade(0, .7f).SetEase(Ease.InOutSine));


            sequence.Play().SetUpdate(true).OnComplete(() => group.gameObject.SetActive(false));

        }
        else
        {

            exitButton.SetActive(true);
            group.DOFade(1, .3f).SetEase(Ease.OutSine).SetUpdate(true);
            exitButton.SetActive(true);
            var img = exitButton.GetComponent<Image>();
            sequence = DOTween.Sequence();
            sequence.Append(img.DOFade(1, .7f).From(.7f));
            sequence.Append(img.DOFade(.7f, .7f));
            sequence.Play().SetUpdate(true).SetLoops(-1);


        }





    }
    private void OnDisable()
    {
        DOTween.Kill(this);
    }
    public void FadeOutFromButton()
    {
        DOTween.Kill(this);

        if (sequence != null && sequence.IsPlaying())
            sequence.Kill();

        sequence = DOTween.Sequence();

        sequence.Append(group.DOFade(0, .7f).SetEase(Ease.InOutSine));


        sequence.Play().SetUpdate(true).OnComplete(() => group.gameObject.SetActive(false));


    }
    private void AdjustHeight()
    {
        if (text == null || rect == null) return;

        // Force TextMeshPro to update text layout to get accurate line count
        text.ForceMeshUpdate();

        // Get the number of lines in the current text
        int lineCount = text.textInfo.lineCount;

        // Calculate the new height: baseHeight for one line + extraLineHeight for each additional line
        float newHeight = baseHeight + (Mathf.Max(0, lineCount - 1) * extraLineHeight) + padding;

        // Apply the new height to the image
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
    }
}
