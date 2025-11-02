using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SteroidVial : MonoBehaviour
{
    [SerializeField] private float rotateWindowMoveAmount;
    [SerializeField] private float rotateWindowYRot;

    [SerializeField] private Image liquidImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI slotAmountText;
    [SerializeField] private RectTransform corkRect;
    [SerializeField] private RectTransform drugFactRect;
    [SerializeField] private RectTransform corkDots;
    [SerializeField] private RectTransform mainRect;
    [SerializeField] private float corkDotMoveAmount;
    [SerializeField] private TextMeshProUGUI ingredientText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private SteroidVial sideVial;

    private SteroidSO steroidData;
    public Action<int> OnSteroidSelected;
    private Vector2 startPos;
    private Sequence selectSeq;
    private Sequence rotateSeq;
    private int index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetData(SteroidSO steroid, int i, bool isDisplay = false, RectTransform pos = null)
    {
        steroidData = steroid;
        index = i;
        liquidImage.color = steroid.LiquidColor;
        iconImage.sprite = steroid.Icon;
        if (isDisplay)
        {
            startPos = pos.anchoredPosition;
            GetComponent<RectTransform>().position = pos.position;
            slotAmountText.text = "(" + steroid.Spaces.ToString() + " mL." + ")";
            ingredientText.text = "<b>" + "Active Ingredients: " + "</b>" + steroid.Ingredients;
            descriptionText.text = "<b><size=+5>Uses: </size></b>" + steroid.Description;
            sideVial.SetData(steroid, i);
        }
        else if (slotAmountText != null)
            slotAmountText.text = steroid.Spaces.ToString();
    }

    public void DoSelectAnimation(RectTransform targetPos, float upDuration, float rotDur)
    {

        selectSeq = DOTween.Sequence();
        selectSeq.Append(GetComponent<RectTransform>().DOAnchorPos(targetPos.anchoredPosition, upDuration).SetEase(Ease.OutBack));


        rotateSeq = DOTween.Sequence();
        rotateSeq.AppendInterval(upDuration * .4f);
        rotateSeq.Append(iconImage.rectTransform.DORotate(Vector3.up * rotateWindowYRot, rotDur).SetEase(Ease.InOutSine));
        rotateSeq.Join(iconImage.rectTransform.DOAnchorPosX(-rotateWindowMoveAmount, rotDur).SetEase(Ease.InOutSine));
        rotateSeq.Join(corkDots.DOAnchorPosX(-corkDotMoveAmount, rotDur).SetEase(Ease.InOutSine));
        rotateSeq.Join(drugFactRect.DORotate(Vector3.zero, rotDur).From(Vector3.up * -rotateWindowMoveAmount).SetEase(Ease.InOutSine));
        rotateSeq.Join(drugFactRect.DOAnchorPosX(0, rotDur).SetEase(Ease.InOutSine));

        selectSeq.Play().SetUpdate(true);
        rotateSeq.Play().SetUpdate(true);


    }

    public void HideOrShowEquippedVial(bool hide, RectTransform layoutGroup)
    {

        if (hide)
        {
            mainRect.DOAnchorPosY(mainRect.anchoredPosition.y - 450, .7f).SetEase(Ease.InBack);
            selectSeq = DOTween.Sequence();

            selectSeq.AppendInterval(0.3f);
            selectSeq.Append(GetComponent<RectTransform>().DOScaleX(0, .6f));

            selectSeq.Play().SetUpdate(true).OnUpdate(() => { LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup); }).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
        else
        {
            mainRect.anchoredPosition = new Vector2(mainRect.anchoredPosition.x, mainRect.anchoredPosition.y - 450);
            GetComponent<RectTransform>().DOScaleX(1, .4f).SetUpdate(true).OnUpdate(() => { LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup); });
            selectSeq = DOTween.Sequence();
            selectSeq.AppendInterval(0.25f);
            selectSeq.Append(mainRect.DOAnchorPosY(mainRect.anchoredPosition.y + 450, .7f).SetEase(Ease.OutBack));
            selectSeq.Play().SetUpdate(true);



        }


    }

    public void HideSelectedVial(bool equipped)
    {
        if (selectSeq != null && selectSeq.IsPlaying())
            selectSeq.Complete();

        startPos.x += 800;
        selectSeq = DOTween.Sequence();


        selectSeq.Append(GetComponent<RectTransform>().DOAnchorPos(startPos, .7f));
        selectSeq.Play().SetUpdate(true).SetEase(Ease.InSine).OnComplete(() =>
        {
            Debug.Log("Destroying steroid vial");
            Destroy(this.gameObject);
        });







    }
    void OnDestroy()
    {
        if (rotateSeq != null && rotateSeq.IsPlaying())
            rotateSeq.Kill();
    }

    public void OnPress()
    {
        OnSteroidSelected?.Invoke(index);

    }



    // Update is called once per frame

}
