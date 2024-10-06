using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class EggUnequippedParent : MonoBehaviour
{
    [SerializeField] private float hideButtonDuration;
    [SerializeField] private float showButtonDuration;
    [SerializeField] private float EquipDuration;
    [SerializeField] private float UnEquipDuration;

    private Button button;

    private Sequence switchingAmmoSeq;


    [SerializeField] private float startAmmoSwitchTweenDuration;
    [SerializeField] private float endAmmoSwitchTweenDuration;


    [SerializeField] private Ease equipEase;
    [SerializeField] private Ease unEquipEase;
    [SerializeField] private Ease hidingButtonEase;
    [SerializeField] private Ease showingButtonEase;
    [SerializeField] private RectTransform mainRect;
    [SerializeField] private RectTransform outlineRect;
    public static Action<bool> OnHideEggButton;
    private bool isShown = false;

    private int currentEquippedAmmoType;

    [SerializeField] private Vector2 startEndScale;
    [SerializeField] private Vector2 startEndWidth;
    [SerializeField] private Vector2 startEndPostionX;
    [SerializeField] private float tweenDuration;

    [SerializeField] private RectTransform mainRectStartPos;
    [SerializeField] private RectTransform mainRectHiddenButtonPosition;
    [SerializeField] private RectTransform eggEquippedAndHiddenPostion;
    [SerializeField] private RectTransform eggUnequippedPosition1;
    [SerializeField] private RectTransform eggButtonHiddenPosition1;
    [SerializeField] private RectTransform eggButtonHiddenPosition2;
    [SerializeField] private RectTransform UnequippedParentPostionOnAmmoSwitch;

    [SerializeField] private RectTransform EggType0;
    [SerializeField] private RectTransform EggType1;
    [SerializeField] private RectTransform[] eggs;



    // Start is called before the first frame update

    private void Awake()
    {
        button = GetComponent<Button>();
        foreach (var item in eggs)
        {
            item.localPosition = eggEquippedAndHiddenPostion.localPosition;

        }
    }

    private void OnEnable()
    {

        EggAmmoDisplay.EquipAmmoEvent += EquipAmmo;
        EggAmmoDisplay.UnequipAmmoEvent += UnEquipAmmo;
        EggAmmoDisplay.HideEggButtonEvent += HideEggButton;


    }
    private void OnDisable()
    {
        EggAmmoDisplay.EquipAmmoEvent -= EquipAmmo;
        EggAmmoDisplay.UnequipAmmoEvent -= UnEquipAmmo;
        EggAmmoDisplay.HideEggButtonEvent -= HideEggButton;


    }

    private void SwitchAmmoTween()
    {
        if (switchingAmmoSeq != null && switchingAmmoSeq.IsPlaying())
            switchingAmmoSeq.Kill();

        button.enabled = false;

        switchingAmmoSeq = DOTween.Sequence();
        switchingAmmoSeq.Append(mainRect.DOLocalMove(UnequippedParentPostionOnAmmoSwitch.localPosition, startAmmoSwitchTweenDuration));
        switchingAmmoSeq.Join(mainRect.DOLocalRotate(UnequippedParentPostionOnAmmoSwitch.eulerAngles, startAmmoSwitchTweenDuration));
        switchingAmmoSeq.Append(mainRect.DOLocalMove(mainRectStartPos.localPosition, endAmmoSwitchTweenDuration));
        switchingAmmoSeq.Join(mainRect.DOLocalRotate(Vector3.zero, endAmmoSwitchTweenDuration));
        switchingAmmoSeq.Play().OnComplete(() => button.enabled = true);
    }


    public void EquipAmmo(int type)
    {
        Debug.Log("EquippingAmmo");
        currentEquippedAmmoType = type;

        if (isShown)
        {
            Debug.Log("EquippingAmmo while hidden");

            OnPress();
            return;
        }
        SwitchAmmoTween();



        eggs[type].DOLocalMove(eggEquippedAndHiddenPostion.localPosition, EquipDuration).SetEase(equipEase);




    }

    public void UnEquipAmmo(int type)
    {
        if (type < 0) return;
        Debug.Log("UNNNNEquippingAmmo");

        eggs[type].DOLocalMove(eggUnequippedPosition1.localPosition, UnEquipDuration).SetEase(unEquipEase);

        Debug.Log("Moving egg: " + type + " to unequpped position: " + eggUnequippedPosition1.localPosition);

    }


    // Update is called once per frame
    public void OnPress()
    {
        int nonEquippedEgg = 0;

        if (currentEquippedAmmoType == 0)
            nonEquippedEgg = 1;

        if (isShown)
        {
            isShown = false;
            // EquipAmmo(currentEquippedAmmoType);
            // UnEquipAmmo(nonEquippedEgg);
            mainRect.DOSizeDelta(new Vector2(startEndWidth.x, mainRect.sizeDelta.y), showButtonDuration).SetEase(showingButtonEase);
            outlineRect.DOSizeDelta(new Vector2(startEndWidth.x, outlineRect.sizeDelta.y), showButtonDuration).SetEase(showingButtonEase);

            mainRect.DOScale(startEndScale.x, showButtonDuration);
            mainRect.DOLocalMoveX(mainRectStartPos.localPosition.x, showButtonDuration).SetEase(showingButtonEase);

        }
        else
        {
            isShown = true;

            eggs[currentEquippedAmmoType].DOLocalMove(eggButtonHiddenPosition1.localPosition, hideButtonDuration);
            eggs[nonEquippedEgg].DOLocalMove(eggButtonHiddenPosition2.localPosition, hideButtonDuration);
            mainRect.DOSizeDelta(new Vector2(startEndWidth.y, mainRect.sizeDelta.y), hideButtonDuration).SetEase(hidingButtonEase);
            outlineRect.DOSizeDelta(new Vector2(startEndWidth.y, outlineRect.sizeDelta.y), hideButtonDuration).SetEase(hidingButtonEase);

            mainRect.DOScale(startEndScale.y, hideButtonDuration);
            mainRect.DOLocalMoveX(mainRectHiddenButtonPosition.localPosition.x, hideButtonDuration).SetEase(hidingButtonEase);


        }

        EggAmmoDisplay.HideEggButtonEvent?.Invoke(isShown);



        // OnHideEggButton?.Invoke(isShown)
    }

    private void HideEggButton(bool hide)
    {
        Debug.LogError("Hiding Egg Button Called");
        if (hide && isShown == false)
        {
            Debug.LogError("Hiding Egg Button Doing");

            isShown = true;
            int nonEquippedEgg = 0;

            if (currentEquippedAmmoType == 0)
                nonEquippedEgg = 1;
            eggs[currentEquippedAmmoType].DOLocalMove(eggButtonHiddenPosition1.localPosition, hideButtonDuration);
            eggs[nonEquippedEgg].DOLocalMove(eggButtonHiddenPosition2.localPosition, hideButtonDuration);
            mainRect.DOSizeDelta(new Vector2(startEndWidth.y, mainRect.sizeDelta.y), hideButtonDuration).SetEase(hidingButtonEase);
            outlineRect.DOSizeDelta(new Vector2(startEndWidth.y, outlineRect.sizeDelta.y), hideButtonDuration).SetEase(hidingButtonEase);

            mainRect.DOScale(startEndScale.y, hideButtonDuration);
            mainRect.DOLocalMoveX(mainRectHiddenButtonPosition.localPosition.x, hideButtonDuration).SetEase(hidingButtonEase);



        }


    }
}
