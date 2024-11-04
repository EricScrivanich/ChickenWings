using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Lofelt.NiceVibrations;
using System;



public class EggAmmoDisplay : MonoBehaviour
{
    [SerializeField] private bool ignoreSwitch;
    [SerializeField] private bool startHidden;
    private Image buttonImage;
    public static Action<int, int> SwitchAmmoEvent;
    public static Action<int> EquipAmmoEvent;
    public static Action<int> UnequipAmmoEvent;
    public static Action<bool> HideEggButtonEvent;
    public static Action<bool, int> AmmoOnZero;
    public static Action CheckIfChained;

    [SerializeField] private Image arrows;
    [SerializeField] private float arrowRotateSpeed;

    private bool holdingShotgunButton;




    [SerializeField] private Image timerFill;
    [SerializeField] private Image chainedFill;

    private bool arrowsAreShown;
    private bool arrowsAreClockwise;

    private bool usingChain;

    [SerializeField] private float slowMaxDuration;

    [SerializeField] private TextMeshProUGUI shotgunText;
    [SerializeField] private CanvasGroup chainedShotgunGroup;
    private Vector2 startChainedGroupPosition;
    private Vector2 endChainedGroupPosition;

    [SerializeField] private float originalEggY = 550;

    private int currentAmmo;
    [SerializeField] private ButtonColorsSO colorSO;

    [SerializeField] private RectTransform scopeRect;
    [SerializeField] private RectTransform swipeRects;

    private RectTransform currentEgg;

    private float originalYPos;
    [SerializeField] private float addedYOnPress;

    private Sequence normalEggPressSequence;
    private Sequence shotgunEggPressSequence;
    private Sequence shotgunCooldownSequence;
    private Sequence shotgunRotateFade;
    private Sequence ChainedShotgunSequence;
    private Sequence MaxWaitChainedShotgunSequence;
    private Sequence MoveChainedGroupSeq;

    private Coroutine rotateShotgunRoutine;

    private bool switchFromShotgunAfterTween = false;

    [SerializeField] private float scaleAmountOnPress;
    [SerializeField] private float buttonPressTweenDuration;
    [SerializeField] private Image scopeFill;
    [SerializeField] private Image ringFill;

    [SerializeField] private TextMeshProUGUI ammoText;
    private Image ButtonImage;
    [SerializeField] private Image EggImage;
    private ModifiedOnScreenStick joystickController;

    [SerializeField] private Transform[] rotationPoints;
    private float lastPointerPosition;

    private int currentEggType;
    [SerializeField] private int amountOfEggs;

    [SerializeField] private SwipedEggUI swipe1;
    [SerializeField] private SwipedEggUI swipe2;
    [SerializeField] private SwipedEggUI[] eggs;

    public bool allAmmoZero { get; private set; }

    private bool shotgunOnZeroAmmo = false;
    private bool normalOnZeroAmmo = false;

    private bool onSwipe1 = true;

    private bool onZero = false;

    private Vector3 rotateAmount = new Vector3(0, 0, 90);

    [SerializeField] private RectTransform chamberRect;

    public Sprite[] eggSprites;

    private int currentAmmoType = 0;
    private bool over10;
    private bool over100;
    private bool outOfAmmo;
    public PlayerID player;
    private int previousAmmo;

    public bool eggButtonHidden;
    private int shotgunRotateVal = -2;





    [SerializeField] private int dragThreshold;
    private bool isPressed = false;
    private bool isRotating = false;

    private bool ignoreTweenFinish = false;

    private int rotateArrowsDirection = 1;

    [SerializeField] private Vector2 moveAmount;



    // Start is called before the first frame update
    void Start()
    {
        chainedFill.color = colorSO.disabledButtonColorFull;
        shotgunText.color = colorSO.disabledButtonColorFull;
        startChainedGroupPosition = chainedShotgunGroup.transform.localPosition;
        endChainedGroupPosition = startChainedGroupPosition + moveAmount;
        chainedShotgunGroup.gameObject.GetComponent<Image>().color = colorSO.normalButtonColor;
        chainedShotgunGroup.alpha = 0;
        buttonImage = GetComponent<Image>();


        timerFill.color = colorSO.disabledButtonColorFull;
        arrows.color = colorSO.normalButtonColorFull;
        scopeFill.color = colorSO.normalButtonColor;
        timerFill.DOFade(0, 0);
        arrows.DOFade(0, 0);
        joystickController = GetComponent<ModifiedOnScreenStick>();

        originalYPos = scopeRect.localPosition.y;

        ButtonImage = GetComponent<Image>();
        // ammoText.text = player.Ammo.ToString();
        outOfAmmo = player.Ammo < 1;

        // swipe1.SetTweenAmounts(addedYOnPress, buttonPressTweenDuration);
        // swipe2.SetTweenAmounts(addedYOnPress, buttonPressTweenDuration);

        foreach (var e in eggs)
        {
            e.SetTweenAmounts(addedYOnPress, buttonPressTweenDuration);
            e.gameObject.SetActive(false);

        }
        allAmmoZero = false;

        if (player.ShotgunAmmo <= 0)
        {
            shotgunOnZeroAmmo = true;
        }

        if (player.Ammo <= 0)
        {
            normalOnZeroAmmo = true;
        }


        if ((shotgunOnZeroAmmo && normalOnZeroAmmo) || startHidden)
        {
            currentAmmoType = 0;
            allAmmoZero = true;



            EggAmmoDisplay.HideEggButtonEvent?.Invoke(true);


        }
        else if (player.Ammo > player.ShotgunAmmo)
        {
            eggs[0].Equip(false, 0);
            currentAmmoType = 0;

            EggAmmoDisplay.EquipAmmoEvent?.Invoke(0);
            EggAmmoDisplay.UnequipAmmoEvent?.Invoke(1);



        }
        else
        {
            eggs[1].Equip(false, 1);
            currentAmmoType = 1;

            EggAmmoDisplay.EquipAmmoEvent?.Invoke(1);
            EggAmmoDisplay.UnequipAmmoEvent?.Invoke(0);


        }
        // swipe2.gameObject.SetActive(false);

        // if (outOfAmmo)
        // {
        //     ButtonImage.color = outOfAmmoButtonColor;
        //     ammoText.color = outOfAmmoTextColor;
        //     EggImage.color = outOfAmmoEggColor;

        // }
        // else
        // {
        //     ButtonImage.color = Color.white;
        //     ammoText.color = ammoTextColor;
        //     EggImage.color = eggColor;


        // }

        // CheckTextSize(player.Ammo);


    }

    public int ReturnCurrentAmmoType()
    {
        return currentAmmoType;
    }


    private void HideEggButton(bool hide)
    {

        if (hide)
        {
            if (shotgunEggPressSequence != null && shotgunEggPressSequence.IsPlaying())
                shotgunEggPressSequence.Kill();
            if (normalEggPressSequence != null && normalEggPressSequence.IsPlaying())
                normalEggPressSequence.Kill();

            player.events.OnSwitchAmmoType?.Invoke(0);
            Debug.LogError("Hiding Egg button");
            scopeRect.DOLocalMoveY(originalYPos - 420, .3f).SetUpdate(true);
            swipeRects.DOLocalMoveY(swipeRects.localPosition.y - 420, .3f).SetUpdate(true).OnComplete(() => buttonImage.enabled = false);

            // if (currentAmmoType >= 0)
            //     eggs[currentAmmoType].UnEquip(false, currentAmmoType);

        }
        else
        {

            buttonImage.enabled = true;

            if (currentAmmoType == 0 && player.Ammo == 0 && player.ShotgunAmmo > 0)
            {
                eggs[0].UnEquip(false, 0);
                eggs[1].Equip(false, 1);
                EggAmmoDisplay.EquipAmmoEvent(1);
                EggAmmoDisplay.UnequipAmmoEvent(0);
                currentAmmoType = 1;

            }
            else if (currentAmmoType == 1 && player.Ammo > 0 && player.ShotgunAmmo == 0)
            {
                eggs[1].UnEquip(false, 1);
                eggs[0].Equip(false, 0);
                EggAmmoDisplay.EquipAmmoEvent(0);
                EggAmmoDisplay.UnequipAmmoEvent(1);
                currentAmmoType = 0;



            }
            else
            {
                if (currentAmmoType == 0)
                {
                    eggs[1].UnEquip(false, 1);
                    eggs[0].Equip(false, 0);

                    EggAmmoDisplay.EquipAmmoEvent(0);
                    EggAmmoDisplay.UnequipAmmoEvent(1);

                    if (player.Ammo == 0) ZeroAmmoPress(false);

                }
                else if (currentAmmoType == 1)
                {
                    eggs[0].UnEquip(false, 0);
                    eggs[1].Equip(false, 1);

                    EggAmmoDisplay.EquipAmmoEvent(1);
                    EggAmmoDisplay.UnequipAmmoEvent(0);
                    if (player.ShotgunAmmo == 0) ZeroAmmoPress(false);
                }

            }


            scopeRect.DOLocalMoveY(scopeRect.localPosition.y + 420, .3f).SetUpdate(true).SetEase(Ease.OutBack);
            swipeRects.DOLocalMoveY(swipeRects.localPosition.y + 420, .3f).SetUpdate(true);
            // if (currentAmmoType < 0)
            //     currentAmmoType = 0;
            // eggs[currentAmmoType].Equip(false, currentAmmoType);


        }
        eggButtonHidden = hide;

        player.globalEvents.OnHideEggButton?.Invoke(hide);
    }




    public void ChangeZeroAmmo(bool onZero, int type)
    {
        if (onZero)
        {
            if (type == 0) normalOnZeroAmmo = true;
            else if (type == 1) shotgunOnZeroAmmo = true;
        }
        else
        {
            if (type == 0)
            {

                OnZeroAmmo(false, type);

            }
            else if (type == 1)
            {

                OnZeroAmmo(false, type);
            }


        }
    }
    public void OnZeroAmmo(bool onZero, int type)
    {
        if (onZero)
        {
            if (type == 0)
            {
                // normalOnZeroAmmo = true;
                // ignoreTweenFinish = true;

                if (shotgunOnZeroAmmo && !ignoreSwitch)
                {
                    EggAmmoDisplay.HideEggButtonEvent?.Invoke(true);


                }
                else if (currentAmmoType == type && !ignoreSwitch)
                {
                    // eggs[0].UnEquip(false, 0);
                    // eggs[1].Equip(false, 1);
                    // EggAmmoDisplay.EquipAmmoEvent?.Invoke(1);
                    // EggAmmoDisplay.UnequipAmmoEvent?.Invoke(0);
                    RotateChamber(false);

                }
                else if (ignoreSwitch)
                {
                    ZeroAmmoPress(false);
                }


            }
            else if (type == 1)
            {
                // shotgunOnZeroAmmo = true;
                // ignoreTweenFinish = true;
                Debug.LogError("Shotgun on zero");
                joystickController.enabled = false;

                if (normalOnZeroAmmo && !ignoreSwitch)
                {
                    EggAmmoDisplay.HideEggButtonEvent?.Invoke(true);
                }
                else if (currentAmmoType == type && !ignoreSwitch)
                {
                    Debug.LogError("Should switch on shotgun zero");
                    // eggs[1].UnEquip(false, 0);
                    // eggs[0].Equip(false, 1);
                    // EggAmmoDisplay.EquipAmmoEvent?.Invoke(0);
                    // EggAmmoDisplay.UnequipAmmoEvent?.Invoke(1);
                    RotateChamber(false);



                }

                else if (ignoreSwitch)
                {
                    ZeroAmmoPress(false);
                }
            }
        }
        else
        {
            if (eggButtonHidden && normalOnZeroAmmo && shotgunOnZeroAmmo)
            {
                // currentAmmoType = type;
                // EggAmmoDisplay.EquipAmmoEvent?.Invoke(type);



                if (type == 0)
                {
                    currentAmmoType = 1;
                    normalOnZeroAmmo = false;


                    RotateChamber(false);
                }
                else if (type == 1)
                {
                    currentAmmoType = 0;
                    shotgunOnZeroAmmo = false;

                    RotateChamber(false);
                    joystickController.enabled = true;
                }
            }
            else if (type == 0)
            {
                normalOnZeroAmmo = false;

                if (shotgunOnZeroAmmo && currentAmmoType != type)
                {

                    RotateChamber(false);

                }

            }
            else if (type == 1)
            {
                shotgunOnZeroAmmo = false;

                if (normalOnZeroAmmo && currentAmmoType != type)
                {
                    RotateChamber(false);
                    joystickController.enabled = true;


                }
                else if (currentAmmoType == type)
                {
                    joystickController.enabled = true;
                }

            }
        }


    }

    public void GetAmmoOnAllZero(int type)
    {
        if (eggButtonHidden)
        {
            // HideEggButton(false);
            currentAmmoType = type;

            eggs[currentAmmoType].Equip(false, type);
            EquipAmmoEvent?.Invoke(type);

        }
        else
        {

            if (type == 0)
            {
                eggs[currentAmmoType].Equip(false, 0);
                eggs[currentAmmoType].UnEquip(false, 1);
            }
            else if (type == 1)
            {
                eggs[currentAmmoType].Equip(false, 1);
                eggs[currentAmmoType].UnEquip(false, 0);
            }

        }
    }

    private void MoveChainedGroupTween()
    {

    }

    private void HandleTimerFillTween(bool show)
    {
        if (shotgunCooldownSequence != null && shotgunCooldownSequence.IsPlaying())
            shotgunCooldownSequence.Kill();

        shotgunCooldownSequence = DOTween.Sequence();

        if (show)
        {
            // shotgunCooldownSequence.Append(cooldownGroup.DOFade(1, .15f));
            shotgunCooldownSequence.Append(timerFill.DOFillAmount(1, slowMaxDuration).SetEase(Ease.OutSine).From(0));



            shotgunCooldownSequence.Join(timerFill.DOFade(1, .25f).SetEase(Ease.OutSine).From(0));

        }
        else
        {
            shotgunCooldownSequence.Append(timerFill.DOFade(0, .15f).SetEase(Ease.OutSine));

            // shotgunCooldownSequence.Append(cooldownGroup.DOFade(0, .2f));
            shotgunCooldownSequence.Join(timerFill.DOFillAmount(0, .2f));

        }
        shotgunCooldownSequence.Play();

    }

    public void ShotgunEggPressTween(bool holding)
    {
        holdingShotgunButton = holding;

        if (currentAmmo == 0 && !usingChain)
        {
            if (holding)
                ZeroAmmoPress(true);
            return;
        }
        if (shotgunEggPressSequence != null && shotgunEggPressSequence.IsPlaying())
            shotgunEggPressSequence.Kill();
        if (normalEggPressSequence != null && normalEggPressSequence.IsPlaying())
            normalEggPressSequence.Kill();

        HandleTimerFillTween(holding);
        shotgunEggPressSequence = DOTween.Sequence();

        if (holding)
        {

            HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);

            shotgunEggPressSequence.Append(scopeRect.DOScale(BoundariesManager.vectorThree1 * scaleAmountOnPress, buttonPressTweenDuration));
            shotgunEggPressSequence.Join(currentEgg.DOScale(BoundariesManager.vectorThree1 * (scaleAmountOnPress + .05f), buttonPressTweenDuration).SetEase(Ease.OutSine));
            shotgunEggPressSequence.Join(currentEgg.DOLocalMoveY(originalEggY + addedYOnPress + 5, buttonPressTweenDuration));
            shotgunEggPressSequence.Join(scopeRect.DOLocalMoveY(originalYPos + addedYOnPress, buttonPressTweenDuration));
            shotgunEggPressSequence.Join(scopeFill.DOColor(colorSO.highlightButtonColor, buttonPressTweenDuration));

            if (usingChain)
                shotgunEggPressSequence.Join(chainedShotgunGroup.transform.DOLocalMove(endChainedGroupPosition, buttonPressTweenDuration + .1f).SetEase(Ease.OutSine));
        }
        else

        {


            HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);


            shotgunEggPressSequence.Append(scopeRect.DOScale(BoundariesManager.vectorThree1, buttonPressTweenDuration));
            shotgunEggPressSequence.Join(currentEgg.DOScale(BoundariesManager.vectorThree1, buttonPressTweenDuration));
            shotgunEggPressSequence.Join(currentEgg.DOLocalMoveY(originalEggY, buttonPressTweenDuration).SetEase(Ease.InSine));
            shotgunEggPressSequence.Join(scopeRect.DOLocalMoveY(originalYPos, buttonPressTweenDuration));
            shotgunEggPressSequence.Join(scopeFill.DOColor(colorSO.normalButtonColor, buttonPressTweenDuration));
            if (usingChain)
                shotgunEggPressSequence.Join(chainedShotgunGroup.transform.DOLocalMove(startChainedGroupPosition, buttonPressTweenDuration + .1f).SetEase(Ease.OutSine));


        }
        shotgunEggPressSequence.Play();




    }

    public void NormalEggPressTween()
    {
        if (normalEggPressSequence != null && normalEggPressSequence.IsPlaying())
            normalEggPressSequence.Kill();
        normalEggPressSequence = DOTween.Sequence();

        if (shotgunEggPressSequence != null && shotgunEggPressSequence.IsPlaying())
            shotgunEggPressSequence.Kill();


        if (onZero)
            ZeroAmmoPress(true);

        else
        {



            normalEggPressSequence.Append(scopeRect.DOScale(BoundariesManager.vectorThree1 * scaleAmountOnPress, buttonPressTweenDuration));
            normalEggPressSequence.Join(currentEgg.DOScale(BoundariesManager.vectorThree1 * (scaleAmountOnPress + .05f), buttonPressTweenDuration).SetEase(Ease.OutSine));
            normalEggPressSequence.Join(currentEgg.DOLocalMoveY(originalEggY + addedYOnPress + 5, buttonPressTweenDuration));
            normalEggPressSequence.Join(scopeRect.DOLocalMoveY(originalYPos + addedYOnPress, buttonPressTweenDuration));
            normalEggPressSequence.Join(scopeFill.DOColor(colorSO.highlightButtonColor, buttonPressTweenDuration).OnComplete(PressTweenFinish));
            // normalEggPressSequence.Join(ringFill.DOColor(colorSO.highlightButtonColor, buttonPressTweenDuration).OnComplete(PressTweenFinish));

            normalEggPressSequence.Play();
        }



    }

    private void PressTweenFinish()
    {



        if (normalEggPressSequence != null && normalEggPressSequence.IsPlaying())
            normalEggPressSequence.Kill();
        normalEggPressSequence = DOTween.Sequence();

        normalEggPressSequence.Append(scopeRect.DOScale(BoundariesManager.vectorThree1, buttonPressTweenDuration));
        normalEggPressSequence.Join(currentEgg.DOScale(BoundariesManager.vectorThree1, buttonPressTweenDuration));
        normalEggPressSequence.Join(currentEgg.DOLocalMoveY(originalEggY, buttonPressTweenDuration).SetEase(Ease.InSine));
        normalEggPressSequence.Join(scopeRect.DOLocalMoveY(originalYPos, buttonPressTweenDuration));

        if (currentAmmo <= 0 && !onZero)
        {
            onZero = true;

            // normalEggPressSequence.Join(ringFill.DOColor(colorSO.disabledButtonColor, buttonPressTweenDuration));
            // normalEggPressSequence.Join(scopeFill.DOColor(colorSO.DisabledScopeFillColor, buttonPressTweenDuration));

            normalEggPressSequence.OnComplete(() => OnZeroAmmo(true, 0));


            // normalEggPressSequence.Join(ringFill.DOColor(colorSO.disabledButtonColorFull, buttonPressTweenDuration));

        }
        else
        {
            normalEggPressSequence.Join(scopeFill.DOColor(colorSO.normalButtonColor, buttonPressTweenDuration));
            normalEggPressSequence.Join(ringFill.DOColor(colorSO.normalButtonColor, buttonPressTweenDuration));


        }
        normalEggPressSequence.Play();
    }

    private IEnumerator RotateArrowsCoroutine()
    {
        while (true)
        {
            float rotateAmount = arrowRotateSpeed * rotateArrowsDirection * Time.deltaTime;
            arrows.rectTransform.Rotate(0, 0, rotateAmount);
            yield return null;
        }
    }

    private void OnRotateWithShotgun(int direction)
    {
        if (currentAmmoType == 1)
        {

            if (!arrowsAreShown)
            {

                arrowsAreShown = true;

                if (direction == 1)
                {
                    arrows.rectTransform.localScale = new Vector3(-1, 1, 1);
                    arrowsAreClockwise = false;

                }
                else if (direction == -1)
                {
                    arrows.rectTransform.localScale = new Vector3(1, 1, 1);
                    arrowsAreClockwise = true;
                }


                rotateArrowsDirection = direction;
                OnFadeArrows(true);
                rotateShotgunRoutine = StartCoroutine(RotateArrowsCoroutine());



            }

            if (arrowsAreClockwise && direction == 1)
            {
                Debug.LogError("Roting SHifted to normal");
                arrows.rectTransform.localScale = new Vector3(-1, 1, 1);
                arrowsAreClockwise = false;
                rotateArrowsDirection = 1;


            }
            else if (!arrowsAreClockwise && direction == -1)
            {
                Debug.LogError("Roting SHifted to counterclock");

                arrowsAreClockwise = true;
                arrows.rectTransform.localScale = new Vector3(1, 1, 1);
                rotateArrowsDirection = -1;


            }

            // arrows.rectTransform.eulerAngles = new Vector3(arrows.rectTransform.eulerAngles.x, 0, arrows.rectTransform.eulerAngles.z + (arrowRotateSpeed * direction * Time.deltaTime));

            if (direction == -2 && arrowsAreShown)
            {
                arrowsAreShown = false;
                OnFadeArrows(false);
            }

        }

    }


    private void OnFadeArrows(bool fadeIn)
    {
        if (shotgunRotateFade != null && shotgunRotateFade.IsPlaying())
            shotgunRotateFade.Kill();

        shotgunRotateFade = DOTween.Sequence();
        if (fadeIn)
        {
            // shotgunCooldownSequence.Append(cooldownGroup.DOFade(1, .15f));
            // shotgunCooldownSequence.Append(timerFill.DOFillAmount(1, slowMaxDuration).SetEase(Ease.OutSine).From(0));

            shotgunRotateFade.Append(arrows.DOFade(1, .05f).From(0));
            // shotgunRotateFade.Join(arrows.rectTransform.DORotate(new Vector3(0, 0, -200), 3));
        }
        else
        {

            shotgunRotateFade.Append(arrows.DOFade(0, .15f).SetEase(Ease.OutSine).From(0)).OnComplete(() => StopCoroutine(rotateShotgunRoutine));
            //shotgunRotateFade.Append(cooldownGroup.DOFade(0, .2f));
            //shotgunRotateFade.Append(timerFill.DOFillAmount(0, .2f));

        }
        shotgunRotateFade.Play();


    }


    public void RotateChamber(bool clockwise)
    {

        if (holdingShotgunButton)
        {
            Debug.LogError("Holding shotgun button");
            return;
        }
        onZero = true;



        int lastEgg = currentAmmoType;

        currentAmmoType = GetNextEgg(clockwise);



        // SwitchAmmoEvent?.Invoke(lastEgg, currentAmmoType);
        // UnequipAmmoEvent?.Invoke(lastEgg);


        eggs[lastEgg].UnEquip(clockwise, lastEgg);
        eggs[currentAmmoType].Equip(clockwise, currentAmmoType);
        EggAmmoDisplay.EquipAmmoEvent?.Invoke(currentAmmoType);
        EggAmmoDisplay.UnequipAmmoEvent?.Invoke(lastEgg);

        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
        AudioManager.instance.PlayChamberClick();

        bool justPressed = false;

        if ((normalEggPressSequence != null && normalEggPressSequence.IsPlaying()))
        {
            justPressed = true;
            normalEggPressSequence.Kill();
        }

        if (shotgunEggPressSequence != null && shotgunEggPressSequence.IsPlaying())
        {
            justPressed = true;
            shotgunEggPressSequence.Kill();
        }

        if (justPressed)
        {
            shotgunEggPressSequence = DOTween.Sequence();

            shotgunEggPressSequence.Append(scopeRect.DOScale(BoundariesManager.vectorThree1, buttonPressTweenDuration));
            shotgunEggPressSequence.Join(currentEgg.DOScale(BoundariesManager.vectorThree1, buttonPressTweenDuration));
            shotgunEggPressSequence.Join(currentEgg.DOLocalMoveY(originalEggY, buttonPressTweenDuration).SetEase(Ease.InSine));
            shotgunEggPressSequence.Join(scopeRect.DOLocalMoveY(originalYPos, buttonPressTweenDuration));

            if (currentAmmo != 0)
            {
                shotgunEggPressSequence.Join(scopeFill.DOColor(colorSO.normalButtonColor, buttonPressTweenDuration));
                shotgunEggPressSequence.Join(ringFill.DOColor(colorSO.normalButtonColor, buttonPressTweenDuration));
            }

            shotgunEggPressSequence.Play();
        }
        if (currentAmmo <= 0)
        {
            ZeroAmmoPress(false);
            joystickController.enabled = false;
        }
        else if (currentAmmoType == 1 && currentAmmo > 0)
            joystickController.enabled = true;







        // swipe1.SwitchType(type, !clockwise);
        // swipe2.SwitchType(type, !clockwise);






        // float targetZRotation = clockwise ? chamberRect.localEulerAngles.z + 90f : chamberRect.localEulerAngles.z - 90f;

        // chamberRect.DOLocalRotate(new Vector3(0, 0, targetZRotation), 0.2f, RotateMode.FastBeyond360)
        //     .SetEase(Ease.OutCubic).OnComplete(FinishRotation);



    }

    public void SetJoystick(RectTransform target, int type)
    {
        // currentEgg = target;
        // if (type == 1)
        // {
        //     joystickController.SetNewRect(target);
        //     joystickController.enabled = true;
        // }
        // else
        // {
        //     joystickController.enabled = false;
        // }

    }

    public void SetAmmo(int amount)
    {
        currentAmmo = amount;

        if ((onZero || usingChain) && currentAmmo > 0)
        {
            onZero = false;

            if (normalEggPressSequence != null && normalEggPressSequence.IsPlaying())
                normalEggPressSequence.Kill();
            normalEggPressSequence = DOTween.Sequence();
            normalEggPressSequence.Append(scopeFill.DOColor(colorSO.normalButtonColor, .15f));
            normalEggPressSequence.Join(ringFill.DOColor(colorSO.normalButtonColor, .15f));
            normalEggPressSequence.Play();

        }


    }

    private void UpdateChainedAmmoMaxWait(bool use)
    {
        if (MaxWaitChainedShotgunSequence != null && MaxWaitChainedShotgunSequence.IsPlaying())
            MaxWaitChainedShotgunSequence.Kill();


        if (use)
        {
            MaxWaitChainedShotgunSequence = DOTween.Sequence();
            MaxWaitChainedShotgunSequence.Append(chainedFill.DOFillAmount(1, .1f));
            MaxWaitChainedShotgunSequence.Append(chainedFill.DOFillAmount(0, 2.3f).SetEase(Ease.OutSine));

            MaxWaitChainedShotgunSequence.Play().OnComplete(() => player.globalEvents.OnUseChainedAmmo?.Invoke(false));

        }


    }

    private void ZeroAmmoPress(bool pressed)
    {


        if (pressed)
        {
            HapticFeedbackManager.instance.PlayerButtonFailure();

        }


        if (normalEggPressSequence != null && normalEggPressSequence.IsPlaying())
            normalEggPressSequence.Kill();

        normalEggPressSequence = DOTween.Sequence();


        normalEggPressSequence.Append(scopeFill.DOColor(colorSO.disabledButtonColor, .15f));
        normalEggPressSequence.Join(ringFill.DOColor(colorSO.disabledButtonColorFull, .15f));
        normalEggPressSequence.Append(scopeFill.DOColor(colorSO.DisabledScopeFillColor, .15f));
        normalEggPressSequence.Join(ringFill.DOColor(colorSO.disabledButtonColor, .15f));

        normalEggPressSequence.Append(scopeFill.DOColor(colorSO.disabledButtonColor, .15f));
        normalEggPressSequence.Join(ringFill.DOColor(colorSO.disabledButtonColorFull, .15f));

        normalEggPressSequence.Append(scopeFill.DOColor(colorSO.DisabledScopeFillColor, .15f));
        normalEggPressSequence.Join(ringFill.DOColor(colorSO.disabledButtonColor, .15f));


        normalEggPressSequence.Play();

    }


    private void ShowChainedAmmo(bool show)
    {

        usingChain = show;

        if (show)
        {
            // chainedFill.color = colorSO.DashImageManaHighlight;
            chainedShotgunGroup.DOFade(1, .3f).From(0);
            chainedShotgunGroup.transform.DOScale(BoundariesManager.vectorThree1, .3f).From(BoundariesManager.vectorThree1 * 1.3f);
        }
        else
        {
            // swipe1.FlashAmmoTween(false);
            // swipe2.FlashAmmoTween(false);
            eggs[1].FlashAmmoTween(false);
            UpdateChainedAmmoMaxWait(false);

            chainedShotgunGroup.DOFade(0, .3f);
            chainedShotgunGroup.transform.DOScale(BoundariesManager.vectorThree1 * 1.3f, .3f);
            if (arrowsAreShown)
                OnRotateWithShotgun(-2);



            if (player.ShotgunAmmo <= 0)
            {
                onZero = true;
                EggAmmoDisplay.AmmoOnZero?.Invoke(true, 1);
                HandleTimerFillTween(false);
                OnZeroAmmo(true, 1);


                // ZeroAmmoPress(false);

            }

        }
    }

    private void UpdateChainedAmmoText(int amount)
    {
        UpdateChainedAmmoMaxWait(true);
        shotgunText.text = amount.ToString();

    }

    private void OnCollectShotgunAmmoWhileChained()
    {
        if (usingChain)
            player.globalEvents.OnUseChainedAmmo?.Invoke(false);
    }




    private void OnEnable()
    {
        player.events.OnEggDrop += NormalEggPressTween;
        player.globalEvents.OnUpdateChainedShotgunAmmo += UpdateChainedAmmoText;
        player.events.OnAttack += ShotgunEggPressTween;
        EggAmmoDisplay.CheckIfChained += OnCollectShotgunAmmoWhileChained;

        player.globalEvents.OnSwitchAmmo += RotateChamber;

        player.globalEvents.OnUseChainedAmmo += ShowChainedAmmo;
        // EggUnequippedParent.OnHideEggButton += HideEggButton;

        EggAmmoDisplay.HideEggButtonEvent += HideEggButton;

        EggAmmoDisplay.AmmoOnZero += ChangeZeroAmmo;
        player.events.OnAimJoystick += OnRotateWithShotgun;



    }

    private void OnDisable()
    {
        player.events.OnEggDrop -= NormalEggPressTween;
        player.events.OnAttack -= ShotgunEggPressTween;
        EggAmmoDisplay.CheckIfChained -= OnCollectShotgunAmmoWhileChained;

        player.globalEvents.OnUpdateChainedShotgunAmmo -= UpdateChainedAmmoText;
        player.globalEvents.OnUseChainedAmmo -= ShowChainedAmmo;
        player.globalEvents.OnSwitchAmmo -= RotateChamber;


        // EggUnequippedParent.OnHideEggButton -= HideEggButton;
        EggAmmoDisplay.HideEggButtonEvent -= HideEggButton;
        EggAmmoDisplay.AmmoOnZero -= ChangeZeroAmmo;
        player.events.OnAimJoystick -= OnRotateWithShotgun;


        DOTween.Kill(this);







    }



    private int GetNextEgg(bool clockwise)
    {

        if (clockwise)
        {

            currentAmmoType--;
            if (currentAmmoType < 0)
            {
                currentAmmoType = amountOfEggs - 1;
            }


        }
        else
        {
            currentAmmoType++;
            if (currentAmmoType >= amountOfEggs)
            {
                currentAmmoType = 0;
            }
        }
        Debug.Log("Returning: " + currentAmmoType);

        return currentAmmoType;

    }

    private void FinishRotation()
    {


    }

    private void CheckTextSize(int number)
    {
        if (number > 99 && !over100)
        {
            ammoText.fontSize = 90;
            over100 = true;


        }
        else if (number < 100 && over100)
        {
            over100 = false;
            ammoText.fontSize = 118;
            over10 = true;
        }
        else if (number > 9 && !over10 && !over100)
        {
            ammoText.fontSize = 118;
            over10 = true;
        }
        else if (number < 10 && over10)
        {
            ammoText.fontSize = 155;
            over10 = false;

        }
    }


    public void HighlightButtons(bool pressed)
    {


        isPressed = pressed;
        eggs[currentAmmoType].HighlightButton(pressed);
        // swipe1.HighlightButton(pressed);
        // swipe2.HighlightButton(pressed);


    }


    // public void OnPointerUp(PointerEventData eventData)
    // {


    //     // Enable further rotation after release, using the last known angular velocity
    // }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     if (!isPressed)
    //     {
    //         lastPointerPosition = eventData.position.x;
    //         isPressed = true;
    //         Debug.Log("Button Pressed");
    //         swipe1.HighlightButton(true);
    //         swipe2.HighlightButton(true);

    //     }




    //     // Enable further rotation after release, using the last known angular velocity
    // }

    // public void OnDrag(PointerEventData eventData)
    // {
    //     if (!isRotating && isPressed)
    //     {
    //         float dragDistanceX = eventData.position.x - lastPointerPosition;

    //         if (dragDistanceX < -dragThreshold) RotateChamber(false);
    //         else if (dragDistanceX > dragThreshold) RotateChamber(true);

    //         isRotating = true;
    //     }






    // }
    // void UpdateAmmo()
    // {
    //     int currentAmmo = player.Ammo;
    //     if (currentAmmo == 0)
    //     {
    //         if (!outOfAmmo)
    //         {
    //             ButtonImage.color = outOfAmmoButtonColor;
    //             ammoText.color = outOfAmmoTextColor;
    //             EggImage.color = outOfAmmoEggColor;

    //             outOfAmmo = true;
    //         }

    //     }
    //     else if (outOfAmmo)
    //     {
    //         ButtonImage.color = Color.white;
    //         ammoText.color = ammoTextColor;
    //         EggImage.color = eggColor;
    //         outOfAmmo = false;
    //     }
    //     CheckTextSize(currentAmmo);
    //     ammoText.text = currentAmmo.ToString();

    // }

    // Update is called once per frame
}
