using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SteroidVial : UIButton
{
    [SerializeField] private float rotateWindowMoveAmount;
    [SerializeField] private float rotateWindowYRot;
    [SerializeField] private float rotateDuration;

    private SteroidUI uiParent;

    [SerializeField] private Image liquidImage;
    [SerializeField] private Image iconImage;

    [SerializeField] private Image bgImageFill;
    [SerializeField] private Image bgImageOutline;
    [SerializeField] private TextMeshProUGUI slotAmountText;
    [SerializeField] private RectTransform corkRect;
    [SerializeField] private RectTransform drugFactRect;
    [SerializeField] private RectTransform corkDotsOrLocked;
    private float corkNormalPosY;
    [SerializeField] private float corkEquippedPosY;
    [SerializeField] private RectTransform mainRect;
    [SerializeField] private float corkDotMoveAmount;
    [SerializeField] private TextMeshProUGUI ingredientText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private SteroidVial sideVial;
    private bool isEquipped = false;
    private bool canBeEquipped = true;
    private bool isSelected = false;
    private bool isUnlocked = true;
    private bool isEquippedSlot = false;


    private SteroidSO steroidData;
    public Action<int> OnSteroidSelected;
    private Vector2 startPos;
    private Sequence selectSeq;
    private Sequence rotateSeq;
    private Sequence corkSeq;
    private int index;
    private float corkDotStartX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetData(SteroidUI p, SteroidSO steroid, int i, bool isDisplay = false, RectTransform pos = null, bool equipped = false, bool unlocked = true, bool isEquippedSlot = false)
    {
        if (!unlocked)
        {
            isUnlocked = false;
            slotAmountText.text = "";
            bgImageFill.color = p.lockedVialBGFillColor;
            bgImageOutline.color = p.lockedVialBGOutlineColor;
            mainRect.gameObject.SetActive(false);
            corkDotsOrLocked.gameObject.SetActive(true);
            Destroy(GetComponent<Button>());
            GetComponent<Image>().raycastTarget = false;

            return;

        }

        uiParent = p;
        steroidData = steroid;
        index = i;
        liquidImage.color = steroid.LiquidColor;
        iconImage.sprite = steroid.Icon;
        iconImage.color = steroid.ImageColor;
        if (isDisplay)
        {
            startPos = pos.anchoredPosition;
            corkDotStartX = corkDotsOrLocked.anchoredPosition.x;
            corkNormalPosY = corkRect.anchoredPosition.y;
            corkEquippedPosY += corkNormalPosY;
            GetComponent<RectTransform>().position = pos.position;
            slotAmountText.text = "(" + steroid.Spaces.ToString() + " mL." + ")";
            string ingredients = steroid.Ingredients;

            // Replace each comma with a bold, larger comma
            ingredients = ingredients.Replace(",", "<b><size=+4>,</size></b>");

            ingredientText.text = "<b><size=+2>Active Ingredients: </size></b>" + ingredients;
            descriptionText.text = "<b><size=+5>Uses: </size></b>" + steroid.Description;
            sideVial.SetData(p, steroid, i, isEquippedSlot: true);

            isEquipped = equipped;


            sideVial.SetIsEquipped(equipped);
            sideVial.SetCanBeEquipped(p.GetAvailableSyringeSlots());

            if (equipped)
            {
                sideVial.gameObject.SetActive(false);
            }
            Invoke("CheckIngredientLineCount", 0.1f);
        }
        else if (slotAmountText != null)
            slotAmountText.text = steroid.Spaces.ToString();

        if (isEquippedSlot)
        {

            bgImageFill.color = Color.clear;
            this.isEquippedSlot = true;

        }
        else if (bgImageFill != null && bgImageOutline != null)
        {
            bgImageFill.color = uiParent.normalVialBGFillColor;
            bgImageOutline.color = uiParent.normalVialBGOutlineColor;
        }
    }

    private void CheckIngredientLineCount()
    {
        ingredientText.ForceMeshUpdate();
        var textInfo = ingredientText.textInfo;
        int lineCount = textInfo.lineCount;
        if (lineCount > 2)
        {
            bgImageFill.rectTransform.anchoredPosition = new Vector2(bgImageFill.rectTransform.anchoredPosition.x, bgImageFill.rectTransform.anchoredPosition.y - ((lineCount - 2) * 26));

        }
    }

    public void DoSelectAnimationBig(RectTransform targetPos, float upDuration, float rotDur)
    {

        selectSeq = DOTween.Sequence();
        corkSeq = DOTween.Sequence();

        selectSeq.Append(GetComponent<RectTransform>().DOAnchorPos(targetPos.anchoredPosition, upDuration).SetEase(Ease.OutBack));

        float appendInterval = upDuration * .4f;
        rotateSeq = DOTween.Sequence();
        rotateSeq.AppendInterval(appendInterval);
        corkSeq.AppendInterval(appendInterval);
        rotateSeq.Append(iconImage.rectTransform.DORotate(Vector3.up * rotateWindowYRot, rotDur).SetEase(Ease.InOutSine));
        rotateSeq.Join(iconImage.rectTransform.DOAnchorPosX(-rotateWindowMoveAmount, rotDur).SetEase(Ease.InOutSine));

        rotateSeq.Join(drugFactRect.DORotate(Vector3.zero, rotDur).From(Vector3.up * -rotateWindowMoveAmount).SetEase(Ease.InOutSine));
        rotateSeq.Join(drugFactRect.DOAnchorPosX(0, rotDur).SetEase(Ease.InOutSine));
        if (!isEquipped)
            corkSeq.Append(corkDotsOrLocked.DOAnchorPosX(-corkDotMoveAmount, rotDur).SetEase(Ease.InOutSine));
        else
            corkSeq.Append(corkRect.DOAnchorPosY(corkEquippedPosY, rotDur).SetEase(Ease.OutBack));

        selectSeq.Play().SetUpdate(true);
        rotateSeq.Play().SetUpdate(true);
        corkSeq.Play().SetUpdate(true);

    }
    [SerializeField] private float corkTwistDur;
    [SerializeField] private float corkUpDur;
    [SerializeField] private float corkUpInitial;

    [SerializeField] private float corkDownDur;


    public void SetEquippedBig(bool equipped)
    {
        isEquipped = equipped;
        if (corkSeq != null && corkSeq.IsPlaying())
        {
            DOTween.Kill(mainRect);
            corkSeq.Kill();
        }


        corkSeq = DOTween.Sequence();

        if (equipped)
        {
            mainRect.DOShakeRotation(corkTwistDur, 10).SetUpdate(true);
            corkSeq.Append(corkDotsOrLocked.DOAnchorPosX(corkDotStartX, corkTwistDur).SetEase(Ease.OutSine));
            corkSeq.Join(corkRect.DOAnchorPosY(corkNormalPosY + corkUpInitial, corkTwistDur).SetEase(Ease.OutSine));
            corkSeq.Append(corkRect.DOAnchorPosY(corkEquippedPosY, corkUpDur).SetEase(Ease.OutBack));
        }
        else
        {
            corkSeq.Append(corkRect.DOAnchorPosY(corkNormalPosY, corkDownDur).SetEase(Ease.OutSine));
            corkSeq.Join(corkDotsOrLocked.DOAnchorPosX(-corkDotMoveAmount, corkDownDur).SetEase(Ease.OutSine));
            corkSeq.Join(mainRect.DOAnchorPosY(-20, corkDownDur).SetEase(Ease.OutSine));
            corkSeq.Append(mainRect.DOAnchorPosY(0, corkDownDur).SetEase(Ease.InOutBack));
        }

        corkSeq.Play().SetUpdate(true);
    }

    // public void HideOrShowEquippedVial(bool hide, RectTransform layoutGroup)
    // {

    //     if (hide)
    //     {
    //         mainRect.DOAnchorPosY(mainRect.anchoredPosition.y - 450, .7f).SetEase(Ease.InBack);
    //         selectSeq = DOTween.Sequence();

    //         selectSeq.AppendInterval(0.3f);
    //         selectSeq.Append(GetComponent<RectTransform>().DOScaleX(0, .6f));

    //         selectSeq.Play().SetUpdate(true).OnUpdate(() => { LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup); }).OnComplete(() =>
    //         {
    //             Destroy(gameObject);
    //         });
    //     }
    //     else
    //     {
    //         mainRect.anchoredPosition = new Vector2(mainRect.anchoredPosition.x, mainRect.anchoredPosition.y - 450);
    //         GetComponent<RectTransform>().DOScaleX(1, .4f).SetUpdate(true).OnUpdate(() => { LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup); });
    //         selectSeq = DOTween.Sequence();
    //         selectSeq.AppendInterval(0.25f);
    //         selectSeq.Append(mainRect.DOAnchorPosY(mainRect.anchoredPosition.y + 450, .7f).SetEase(Ease.OutBack));
    //         selectSeq.Play().SetUpdate(true);



    //     }


    // }



    public void SelectEquippedVial(int index, float fadeOnlyDur = 0)
    {
        if (selectSeq != null && selectSeq.IsPlaying())
            selectSeq.Complete();

        if (rotateSeq != null && rotateSeq.IsPlaying())
        {
            rotateSeq.Kill();
            mainRect.eulerAngles = Vector3.zero;

        }


        selectSeq = DOTween.Sequence();

        if (fadeOnlyDur > 0)
        {
            isSelected = false;
            selectSeq.Append(bgImageFill.DOColor(Color.clear, fadeOnlyDur));
            selectSeq.Play().SetUpdate(true);
            return;
        }

        if (!isSelected && index == this.index || index == -1)
        {
            isSelected = true;
            selectSeq.Append(transform.DOScale(uiParent.equippedVialScaleSelected, .3f))
            .Join(bgImageFill.DOColor(uiParent.equippedVialBGBlurColor, .3f));

            rotateSeq = DOTween.Sequence();

            rotateSeq.Append(mainRect.DORotate(Vector3.forward * rotateWindowYRot, rotateDuration).From(Vector3.forward * -rotateWindowYRot).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
            rotateSeq.Play().SetUpdate(true);


        }
        else if (isSelected && index != this.index)
        {
            isSelected = false;
            selectSeq.Append(transform.DOScale(uiParent.equippedVialScale, .3f))
           .Join(bgImageFill.DOColor(Color.clear, .3f));



        }



        selectSeq.Play().SetUpdate(true);

    }

    public RectTransform GetSideVialRectForEquip()
    {


        var o = sideVial.GetComponent<RectTransform>();

        sideVial.GetComponent<Image>().raycastTarget = true;
        sideVial.GetComponent<Button>().enabled = true;
        sideVial = Instantiate(sideVial, this.transform);
        sideVial.GetComponent<RectTransform>().position = o.position;
        sideVial.gameObject.SetActive(false);
        return o;



    }

    public Vector2 GetSideVialRectForUnEquip(RectTransform newVial, float fadeDur)
    {

        Vector2 pos = sideVial.GetComponent<RectTransform>().anchoredPosition;
        newVial.transform.parent = this.transform;
        newVial.GetComponent<Button>().enabled = false;
        Destroy(sideVial.gameObject);
        sideVial = newVial.GetComponent<SteroidVial>();
        sideVial.SelectEquippedVial(0, fadeDur);
        return pos;

    }



    public void DoSelectAnimationUnlockedVial(int selectedIndex)
    {
        if (!isUnlocked)
            return;

        if (selectedIndex != index && !isSelected)
            return;

        if (selectSeq != null && selectSeq.IsPlaying())
            selectSeq.Complete();

        selectSeq = DOTween.Sequence();

        if (selectedIndex == index)
        {
            isSelected = true;
            selectSeq.Append(transform.DOScale(uiParent.selectedVialScale, .3f).SetEase(Ease.OutBack))
            .Join(bgImageFill.DOColor(uiParent.selectedVialBGFillColor, .3f));
            if (!isEquipped)
                selectSeq.Join(bgImageOutline.DOColor(uiParent.selectedVialBGOutlineColor, .3f));
        }
        else if (isSelected)
        {
            isSelected = false;
            selectSeq.Append(transform.DOScale(uiParent.baseVialScale, .25f).SetEase(Ease.OutBack))
           .Join(bgImageFill.DOColor(uiParent.normalVialBGFillColor, .25f));

            if (!isEquipped)
                selectSeq.Join(bgImageOutline.DOColor(uiParent.normalVialBGOutlineColor, .25f));

        }
        selectSeq.Play().SetUpdate(true);


    }

    public void SetIsEquipped(bool equipped)
    {
        if (!isUnlocked)
            return;

        if (isEquippedSlot)
        {
            isEquipped = equipped;
            return;
        }
        isEquipped = equipped;

        if (equipped)
        {
            bgImageOutline.color = uiParent.equippedVialBGOutlineColor;
            // slotAmountText.color = uiParent.equippedVialBGOutlineColor;
        }
        else
        {
            bgImageOutline.color = uiParent.selectedVialBGOutlineColor;
            // slotAmountText.color = uiParent.selectedVialBGOutlineColor;
        }
    }

    public void SetCanBeEquipped(int availableSlots)
    {
        if (!isUnlocked)
            return;

        if (isEquipped)
        {
            canBeEquipped = true;
            return;
        }

        bool canEquip = steroidData.Spaces <= availableSlots;

        if (!canEquip)
        {
            // bgImageOutline.color = uiParent.cannotBeEquippedVialBGOutlineColor;
            slotAmountText.color = uiParent.cannotBeEquippedVialBGOutlineColor;

        }
        else if (canEquip)
        {
            // bgImageOutline.color = uiParent.normalVialBGOutlineColor;
            slotAmountText.color = Color.white;
        }




        canBeEquipped = canEquip;
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

    public override void OnHighlight(bool highlighted)
    {
        if (highlighted)
        {
            if (!isEquippedSlot)
                bgImageFill.color = uiParent.highlightVialBGFillColor;
            else
                bgImageFill.color = Color.white;
        }
        else if (!isSelected)
        {
            if (!isEquippedSlot)
                bgImageFill.color = uiParent.normalVialBGFillColor;
            else
                bgImageFill.color = Color.clear;
        }
        else if (isSelected)
        {
            if (!isEquippedSlot)
                bgImageFill.color = uiParent.selectedVialBGFillColor;
            else
                bgImageFill.color = uiParent.equippedVialBGBlurColor;
        }


    }

    public override void OnPress()
    {
        if (!isEquippedSlot)
            OnSteroidSelected?.Invoke(index);
        else
            uiParent.SelectVial(index);

    }



    // Update is called once per frame

}
