using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class AmmoStateManager : MonoBehaviour
{
    [SerializeField] private bool startHidden;
    [SerializeField] private Color zeroAmmoEggImageColor;
    private AudioSource audioSource;

    private float basePowerWidth;
    private float powerHeight;
    [SerializeField] private RectTransform scythePowerFill;
    [SerializeField] private CanvasGroup scythePowerGroup;

    [SerializeField] private Image mainButton;
    public PlayerID player;
    private RectTransform rect;
    AmmoBaseState currentState;
    private bool scopeOnZeroState;

    [SerializeField] private Sprite cageHitImage;

    [SerializeField] private GameObject swipeButton;


    private bool fromCage = false;

    private bool cageEquipped = false;

    [SerializeField] private Image[] normalRaycasts;

    private bool pressing = false;
    private bool hide = false;

    private bool isHidden = false;

    [SerializeField] private SidebarParent eggSidebar;
    [SerializeField] private float powerBarScale;

    private Vector2 scopeRectSize;



    [SerializeField] private Image eggSidebarImage;
    // [SerializeField] private Image[] switchAmmoImages;



    private int currentAmmoType;

    private bool isRotating = false;
    [SerializeField] private Sprite[] ammoSprites;

    [SerializeField] private Image[] ammoEggImages;
    // [SerializeField] private SwipedEggUI[] swipedRects;

    [SerializeField] private TextMeshProUGUI[] ammoDisplayTexts;
    [SerializeField] private float moveAmountForAmmoText;
    [SerializeField] private Image cage;

    // [SerializeField] private RectTransform swipeRectsParent;
    private float swipeRectsStartPos;

    [SerializeField] private int rotateAmountForEggImage;
    [SerializeField] private float rotateDurationForEggImage;
    // private TextMeshProUGUI currentDisplayedAmmoText;
    private int currentEggImageIndex;

    private Sequence rotateSeq;
    public Sequence pressSeq;
    private Sequence flashRedSeq;
    private Sequence flashTextRedSeq;
    private Sequence chainGroupFadeSeq;
    private Sequence rotateSidebarSeq;
    private Sequence sideButtonSeq;

    private Sequence cageSeq;
    private Sequence scytheBarSeq;

    // private Sequence otherButtonColorSeq;

    private Sequence directionArrowFadeSeq;
    // [SerializeField] private Image directionArrow;
    private bool startedDirection;

    public ButtonColorsSO colorSO;
    [SerializeField] private Image scopeFill;
    [SerializeField] private Image scopeTimerFill;
    // [SerializeField] private Image ringFill;
    [SerializeField] private RectTransform scopeRect;
    [SerializeField] private float pressScale;
    [SerializeField] private float pressDuration;
    [SerializeField] private float releaseDuration;


    [Header("Cage")]
    private Image cageImage;
    [SerializeField] private float cageInDuration;
    [SerializeField] private float cageOutDuration;
    [SerializeField] private float scaleChange;
    private float totalStartY;
    [SerializeField] private float totalMoveY;
    [SerializeField] private float cageStartScale;
    [SerializeField] private int cageStartY;
    [SerializeField] private int cageEndY;

    private float originalYPos;
    private int addedYOnPress = 35;

    private bool startOfLifetime = true;
    private bool hidingButton = false;
    private bool showingButton = false;
    private Vector2 normalPowerSize;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        Debug.Log("AmmoStateManager Awake called");
        rect = GetComponent<RectTransform>();

        originalYPos = scopeRect.localPosition.y;
        // swipeRectsStartPos = swipeRectsParent.localPosition.y;
        totalStartY = rect.localPosition.y;

        scopeTimerFill.fillAmount = 0;


    }
    void Start()
    {
        Debug.Log("AmmoStateManager Start called");
        if (swipeButton != null)
            swipeButton.SetActive(false);

        // cageImage.raycastTarget = false;

        if (cage != null)
            cage.gameObject.SetActive(false);
        // directionArrow.color = colorSO.normalButtonColorFull;
        scopeTimerFill.color = colorSO.disabledButtonColorFull;
        scopeFill.color = colorSO.normalButtonColor;
        scopeRect.localScale = Vector3.one;
        ammoDisplayTexts[1].gameObject.SetActive(false);
        ammoEggImages[1].gameObject.SetActive(false);

        if (cage != null)
        {
            cageImage = cage.GetComponent<Image>();
            cage.gameObject.SetActive(false);
        }


        if (scythePowerGroup != null)
        {
            basePowerWidth = scythePowerFill.sizeDelta.x;
            powerHeight = scythePowerFill.sizeDelta.y;
            normalPowerSize = new Vector2(basePowerWidth, powerHeight);
            // scythePowerFill.GetComponent<Image>().color = colorSO.WeaponColor;
            scythePowerFill.sizeDelta = new Vector2(0, powerHeight);
            scythePowerGroup.alpha = 0;
        }
        scopeRectSize = scopeRect.sizeDelta;
        scythePowerFill.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();


        // EquipCage(true);




        // ResetToHiddenPosition(false);

        // joystickController = GetComponent<ModifiedOnScreenStick>();

        // directionArrow.DOFade(0, 0);
        // joystickController.enabled = false;



    }

    // Update is called once per frame



    public void SetNewDisplayText(int amount)
    {


        // currentDisplayedAmmoText = t;
        // currentDisplayedAmmoText.text = amount.ToString();
        ammoDisplayTexts[currentEggImageIndex].text = amount.ToString();
        if (amount > 0) scopeOnZeroState = false;
        else scopeOnZeroState = true;

        if (scopeOnZeroState)
        {
            // currentDisplayedAmmoText.color = colorSO.disabledButtonColorFull;
            ammoDisplayTexts[currentEggImageIndex].color = colorSO.disabledButtonColorFull;
            ammoEggImages[currentEggImageIndex].color = zeroAmmoEggImageColor;
            ScopeOnZeroTween();

        }
        else
        {
            if (flashRedSeq != null && flashRedSeq.IsPlaying())
                flashRedSeq.Kill();

            // currentDisplayedAmmoText.color = Color.white;
            ammoDisplayTexts[currentEggImageIndex].color = Color.white;
            ammoEggImages[currentEggImageIndex].color = Color.white;

            scopeFill.color = colorSO.normalButtonColor;
            // ringFill.color = colorSO.normalButtonColor;
        }




    }
    public void UpdateAmmoAmountText(int amount)
    {

        // currentDisplayedAmmoText.text = amount.ToString();
        ammoDisplayTexts[currentEggImageIndex].text = amount.ToString();

        if (scopeOnZeroState && amount > 0)
        {
            scopeOnZeroState = false;
            // currentDisplayedAmmoText.color = Color.white;
            ammoDisplayTexts[currentEggImageIndex].color = Color.white;
            ammoEggImages[currentEggImageIndex].color = Color.white;
            scopeFill.color = colorSO.normalButtonColor;
            // ringFill.color = colorSO.normalButtonColor;
        }
    }



    private void ResetShowingButton()
    {
        if (showingButton)
        {
            pressSeq.Complete();
            showingButton = false;
        }
    }


    public void SwitchAmmo(int nextAmmoType, int amountOfAmmo, int direction)
    {

        if (nextAmmoType == -2)
        {
            EquipCage(true);

            return;
        }

        if (fromCage)
        {
            fromCage = false;
            return;

        }



        ResetShowingButton();

        if (direction == 0)
        {
            // HideOrShowEggButton(false);
            eggSidebar.EquipAmmo(nextAmmoType);
            currentAmmoType = nextAmmoType;

            ammoEggImages[currentEggImageIndex].sprite = ammoSprites[nextAmmoType];

            SetNewDisplayText(amountOfAmmo);
            return;


            // ResetScopeRotation();
            // if (nextAmmoType == -1)
            // {
            //     // if (!pressing)
            //     //     HideOrShowEggButton(true);
            //     // else
            //     // {

            //     //     hide = true;

            //     // }

            // }
            // else
            // {
            //     
            // }

            // return;

        }
        else if (direction == -2)
            direction = 1;

        AudioManager.instance.PlayChamberClick();
        if (flashTextRedSeq != null && flashTextRedSeq.IsPlaying())
            flashTextRedSeq.Kill();


        TweenEggImageToScope(NextEggImageIndex(), direction);
        eggSidebar.UnEquipAmmo(currentAmmoType);
        eggSidebar.EquipAmmo(nextAmmoType);
        currentAmmoType = nextAmmoType;

        ammoEggImages[currentEggImageIndex].sprite = ammoSprites[nextAmmoType];

        SetNewDisplayText(amountOfAmmo);



    }


    // private void ResetToHiddenPosition(bool hide)
    // {
    //     if (hide)
    //     {
    //         // player.canPressEggButton = false;
    //         scopeRect.localPosition = new Vector2(scopeRect.localPosition.x, originalYPos - 430);
    //         swipeRectsParent.localPosition = new Vector2(swipeRectsParent.localPosition.x, swipeRectsStartPos - 430);
    //     }
    //     else
    //     {
    //         // player.canPressEggButton = true;
    //         scopeRect.localPosition = new Vector2(scopeRect.localPosition.x, originalYPos);
    //         swipeRectsParent.localPosition = new Vector2(swipeRectsParent.localPosition.x, swipeRectsStartPos);

    //     }

    // }

    private void EnableSwipeButton(bool enable)
    {
        swipeButton.SetActive(enable);
    }

    // private void HideOrShowEggButton(bool h)
    // {
    //     if (h)
    //     {
    //         mainButton.enabled = false;
    //         isHidden = true;
    //         hide = false;
    //         hidingButton = true;

    //         ResetScope(true);


    //         pressSeq = DOTween.Sequence();
    //         pressSeq.Append(scopeRect.DOLocalMoveY(originalYPos - 430, .3f));
    //         // pressSeq.Join(swipeRectsParent.DOLocalMoveY(swipeRectsStartPos - 430, .3f));
    //         eggSidebar.ShowOrHide(true, pressSeq);
    //         pressSeq.Play().SetUpdate(true);



    //     }
    //     else
    //     {
    //         mainButton.enabled = true;

    //         isHidden = false;
    //         showingButton = true;
    //         if (pressSeq != null && pressSeq.IsPlaying())
    //             pressSeq.Kill();

    //         pressSeq = DOTween.Sequence();
    //         pressSeq.Append(scopeRect.DOLocalMoveY(originalYPos, .3f).SetEase(Ease.OutBack));
    //         // pressSeq.Join(swipeRectsParent.DOLocalMoveY(swipeRectsStartPos, .3f));
    //         eggSidebar.ShowOrHide(false, pressSeq);
    //         pressSeq.Play().SetUpdate(true).OnComplete(() => showingButton = false);


    //     }

    // }

    private void OnFinishShowButton()
    {
        // player.canPressEggButton = true;
        // AudioManager.instance.PlayChamberCock();

    }

    private void TweenEggImageToScope(int nextCurrentEggIndex, int direction)
    {
        int prevIndex = 0;
        int nextIndex = nextCurrentEggIndex;

        ammoEggImages[nextCurrentEggIndex].gameObject.SetActive(true);
        // swipedRects[nextCurrentEggIndex].gameObject.SetActive(true);
        ammoDisplayTexts[nextCurrentEggIndex].gameObject.SetActive(true);




        if (nextCurrentEggIndex == 0)
        {
            prevIndex = 1;
        }

        isRotating = true;

        if (rotateSeq != null && rotateSeq.IsPlaying())
            rotateSeq.Kill();

        rotateSeq = DOTween.Sequence();



        rotateSeq.Append(ammoEggImages[prevIndex].rectTransform.DORotate(Vector3.forward * rotateAmountForEggImage * direction, rotateDurationForEggImage).From(Vector3.zero));
        // rotateSeq.Join(swipedRects[prevIndex].ReturnRect().DORotate(Vector3.forward * rotateAmountForEggImage * direction, rotateDurationForEggImage).From(Vector3.zero));
        rotateSeq.Join(ammoDisplayTexts[prevIndex].rectTransform.DOLocalMoveX((moveAmountForAmmoText + 40) * -direction, rotateDurationForEggImage).From(0));
        rotateSeq.Join(ammoEggImages[currentEggImageIndex].rectTransform.DORotate(Vector3.zero, rotateDurationForEggImage).From(-Vector3.forward * rotateAmountForEggImage * direction).SetEase(Ease.OutBack));
        // rotateSeq.Join(swipedRects[currentEggImageIndex].ReturnRect().DORotate(Vector3.zero, rotateDurationForEggImage).From(-Vector3.forward * rotateAmountForEggImage * direction).SetEase(Ease.OutBack));
        rotateSeq.Join(ammoDisplayTexts[currentEggImageIndex].rectTransform.DOLocalMoveX(0, rotateDurationForEggImage).From(moveAmountForAmmoText * direction).SetEase(Ease.OutBack));
        rotateSeq.Play().SetUpdate(true).OnComplete(OnFinishRotation);



    }



    private void OnFinishRotation()
    {
        isRotating = false;
        AudioManager.instance.PlayChamberCock();
        ammoEggImages[NextEggImageIndex(true)].gameObject.SetActive(false);
        // swipedRects[NextEggImageIndex(true)].gameObject.SetActive(false);
        ammoDisplayTexts[NextEggImageIndex(true)].gameObject.SetActive(false);
    }

    private int NextEggImageIndex(bool getPrev = false)
    {
        if (getPrev)
        {
            if (currentEggImageIndex == 0) return 1;
            else return 0;

        }
        if (currentEggImageIndex == 0)
        {
            currentEggImageIndex = 1;
            return 1;
        }
        else
        {
            currentEggImageIndex = 0;
            return 0;
        }
    }

    public void PressScope(float maxHoldDuration)
    {
        ResetScope(true);
        pressing = true;

        // ringFill.color = colorSO.normalButtonColorFull;
        pressSeq = DOTween.Sequence();
        if (scopeOnZeroState) ScopeOnZeroTween();
        pressSeq.Append(scopeRect.DOScale(pressScale, pressDuration));
        pressSeq.Join(scopeRect.DOLocalMoveY(originalYPos + addedYOnPress, pressDuration));
        pressSeq.Join(scopeFill.DOColor(colorSO.highlightButtonColor, pressDuration));
        ;
        if (maxHoldDuration <= 0)
        {
            pressSeq.Append(scopeRect.DOScale(1, releaseDuration).OnComplete(FinishRelease));
            pressSeq.Join(scopeRect.DOLocalMoveY(originalYPos, releaseDuration));
            if (!scopeOnZeroState)
            {
                // pressSeq.Join(ringFill.DOColor(colorSO.normalButtonColor, pressDuration));
                pressSeq.Join(scopeFill.DOColor(colorSO.normalButtonColor, releaseDuration));
            }


        }
        else
        {
            pressSeq.Join(scopeTimerFill.DOFillAmount(1, maxHoldDuration));

        }

        pressSeq.Play();

    }

    private void FinishRelease()
    {
        pressing = false;

        // if (hide)
        // {
        //     hide = false;
        //     HideOrShowEggButton(true);
        // }

    }

    private void ScopeOnZeroTween()
    {
        if (flashRedSeq != null && flashRedSeq.IsPlaying())
            flashRedSeq.Kill();

        flashRedSeq = DOTween.Sequence();

        flashRedSeq.Append(scopeFill.DOColor(colorSO.DisabledScopeFillColor, .15f));
        // flashRedSeq.Join(ringFill.DOColor(colorSO.disabledButtonColorFull, .15f));

        flashRedSeq.Append(scopeFill.DOColor(colorSO.disabledButtonColor, .15f));
        // flashRedSeq.Join(ringFill.DOColor(colorSO.disabledButtonColor, .15f));

        flashRedSeq.Append(scopeFill.DOColor(colorSO.DisabledScopeFillColor, .15f));
        // flashRedSeq.Join(ringFill.DOColor(colorSO.disabledButtonColorFull, .15f));

        flashRedSeq.Append(scopeFill.DOColor(colorSO.disabledButtonColor, .15f));
        // flashRedSeq.Join(ringFill.DOColor(colorSO.disabledButtonColor, .15f));



        flashRedSeq.Play().SetUpdate(true);

    }

    public void SetScopeZero(bool fromPress)
    {
        scopeOnZeroState = true;
        if (fromPress)
        {
            HapticFeedbackManager.instance.PlayerButtonFailure();
            ScopeOnZeroTween();
        }
        else
        {
            ammoEggImages[currentEggImageIndex].color = zeroAmmoEggImageColor;
            // currentDisplayedAmmoText.color = colorSO.disabledButtonColorFull;
            ammoDisplayTexts[currentEggImageIndex].color = colorSO.disabledButtonColorFull;
        }



    }
    private bool ignoreLerp = false;
    private bool powerBarShown;

    private Sequence audioSeq;
    private void ChangeScythePowerPitch(float pitch)
    {
        // Kill any existing sequence to prevent conflicts
        if (audioSeq != null && audioSeq.IsActive())
        {
            audioSeq.Kill();
        }

        // Create a new sequence
        audioSeq = DOTween.Sequence();

        // Add a tween to change the pitch over 0.3 seconds
        audioSeq.Append(DOTween.To(
            () => audioSource.pitch, // Getter: Current pitch
            x => audioSource.pitch = x, // Setter: Apply new pitch
            pitch, // Target pitch
            0.3f // Duration
        ));

        // Optional: Set ease type (e.g., smooth transition)
        audioSeq.Play().SetEase(Ease.Linear);

        // Optional: Auto-kill the sequence when done

    }
    private void ActivateScythePowerBar(bool act, float dur)
    {
        ignoreLerp = true;
        if (scytheBarSeq != null && scytheBarSeq.IsPlaying())
            scytheBarSeq.Kill();
        scytheBarSeq = DOTween.Sequence();

        if (act)
        {
            scytheBarSeq.Append(scythePowerFill.DOSizeDelta(normalPowerSize, dur));
            if (!powerBarShown)
            {
                audioSource.Play();
                Debug.Log("Should play audio");

                scythePowerFill.gameObject.SetActive(true);

                // scopeRect.sizeDelta = scopeRectSize * 1.5f;
                ammoEggImages[currentEggImageIndex].maskable = false;
                mainButton.raycastTarget = false;
                eggSidebar.SetPressable(false);


                scytheBarSeq.Join(rect.DOScale(powerBarScale, dur));
                scytheBarSeq.Join(scythePowerGroup.DOFade(1, dur));
                scytheBarSeq.Join(scythePowerGroup.transform.DOScale(1, dur));

            }
            else if (scythePowerGroup.alpha != 1)
            {
                scythePowerGroup.alpha = 1;
                scythePowerGroup.transform.localScale = Vector3.one;
            }



        }
        else
        {
            audioSource.Stop();
            mainButton.raycastTarget = true;
            scythePowerFill.gameObject.SetActive(false);
            eggSidebar.SetPressable(true);
            // scopeRect.sizeDelta = scopeRectSize;
            powerBarShown = false;
            scytheBarSeq.Append(scythePowerGroup.DOFade(0, dur));
            scytheBarSeq.Join(scythePowerGroup.transform.DOScale(1.15f, dur).OnComplete(() => ammoEggImages[currentEggImageIndex].maskable = true));
        }
        scytheBarSeq.Play().OnComplete(() => ignoreLerp = false);


    }
    public void HandleScythePowerFill(float p)
    {
        if (!ignoreLerp)
            scythePowerFill.sizeDelta = new Vector2(p * basePowerWidth, powerHeight);

    }

    public void ResetScope(bool instant)
    {
        ResetShowingButton();

        if (isRotating)
        {
            rotateSeq?.Kill();
            ResetScopeRotation();
        }
        if (pressSeq != null && pressSeq.IsPlaying())
            pressSeq.Kill();

        if (instant)
        {
            if (flashRedSeq != null && flashRedSeq.IsPlaying())
                flashRedSeq.Kill();

            scopeRect.localScale = BoundariesManager.vectorThree1;
            scopeTimerFill.fillAmount = 0;
            if (!scopeOnZeroState)
            {
                scopeFill.color = colorSO.normalButtonColor;
                ammoEggImages[currentEggImageIndex].color = Color.white;

                // ringFill.color = colorSO.normalButtonColor;
            }

            else
            {
                scopeFill.color = colorSO.DisabledScopeFillColor;
                ammoEggImages[currentEggImageIndex].color = zeroAmmoEggImageColor;
                // ringFill.color = colorSO.disabledButtonColorFull;
            }

        }
        else
        {
            if (scopeOnZeroState) ScopeOnZeroTween();



            pressSeq = DOTween.Sequence();
            pressSeq.Append(scopeRect.DOScale(1, releaseDuration));
            pressSeq.Join(scopeRect.DOLocalMoveY(originalYPos, releaseDuration));

            pressSeq.Join(scopeTimerFill.DOFillAmount(0, releaseDuration));
            if (!scopeOnZeroState)
            {
                pressSeq.Join(scopeFill.DOColor(colorSO.normalButtonColor, releaseDuration));
                // ringFill.color = colorSO.normalButtonColor;
            }

            pressSeq.Play().OnComplete(FinishRelease);

        }
    }

    private void ResetScopeRotation()
    {

        ammoEggImages[currentEggImageIndex].rectTransform.eulerAngles = Vector3.zero;
        // swipedRects[currentEggImageIndex].ReturnRect().eulerAngles = Vector3.zero;
        ammoDisplayTexts[currentEggImageIndex].rectTransform.localPosition = Vector2.zero;

        ammoEggImages[NextEggImageIndex(true)].gameObject.SetActive(false);
        // swipedRects[NextEggImageIndex(true)].gameObject.SetActive(false);
        ammoDisplayTexts[NextEggImageIndex(true)].gameObject.SetActive(false);

    }

    private void PressSideButton(int type)
    {
        if (sideButtonSeq != null && sideButtonSeq.IsPlaying())
            sideButtonSeq.Kill();

        switch (type)
        {
            case 0:
                sideButtonSeq = DOTween.Sequence();
                sideButtonSeq.Append(eggSidebarImage.DOColor(colorSO.highlightButtonColor, .1f));
                sideButtonSeq.Append(eggSidebarImage.DOColor(colorSO.normalButtonColor, .25f));
                sideButtonSeq.Play().SetUpdate(true);

                break;
                // case 1:
                //     switchAmmoImages[currentEggImageIndex].color = colorSO.highlightButtonColor;
                //     break;
                // case 2:
                //     switchAmmoImages[currentEggImageIndex].color = colorSO.normalButtonColor;
                //     break;
        }
    }

    private void FlashAmmoText(TextMeshProUGUI t)
    {
        if (flashTextRedSeq != null && flashTextRedSeq.IsPlaying())
            flashTextRedSeq.Kill();
        flashTextRedSeq = DOTween.Sequence();
        flashTextRedSeq.Append(t.DOColor(colorSO.disabledButtonColorFull, .2f).From(colorSO.normalButtonColorFull));
        flashTextRedSeq.Append(t.DOColor(Color.white, .2f));
        flashTextRedSeq.Play().SetLoops(-1);

    }

    private void ShowChainedAmmo(bool usingChain)
    {
        if (usingChain)
        {
            FlashAmmoText(ammoDisplayTexts[currentEggImageIndex]);



        }
        else
        {
            if (flashTextRedSeq != null && flashTextRedSeq.IsPlaying())
                flashTextRedSeq.Kill();
            scopeOnZeroState = true;

            // SetNewDisplayText(swipedRects[currentEggImageIndex].ReturnTextObject(), 0);


        }

    }

    // private void SetJoystickActive(bool active)
    // {
    //     joystickController.enabled = true;
    //     Debug.Log("Joystick is " + active + "joystick is " + joystickController.enabled);   

    // }

    private void ShowJoystickArrow(Vector2 target)
    {
        if (!startedDirection && target != Vector2.zero)
        {
            if (directionArrowFadeSeq != null && directionArrowFadeSeq.IsPlaying())
                directionArrowFadeSeq.Kill();

            directionArrowFadeSeq = DOTween.Sequence();

            // directionArrowFadeSeq.Append(directionArrow.DOFade(1, .2f));
            directionArrowFadeSeq.Play().SetUpdate(true);
            startedDirection = true;
        }

        if (startedDirection)
        {

            if (target == Vector2.zero)
            {
                startedDirection = false;
                if (directionArrowFadeSeq != null && directionArrowFadeSeq.IsPlaying())
                    directionArrowFadeSeq.Kill();

                directionArrowFadeSeq = DOTween.Sequence();

                // directionArrowFadeSeq.Append(directionArrow.DOFade(0, .1f));
                directionArrowFadeSeq.Play().SetUpdate(true);

            }
            else
            {
                float targetAngle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;

                // Set the rotation of the arrow to match the direction
                // directionArrow.rectTransform.rotation = Quaternion.Euler(0, 0, targetAngle - 90);

            }

        }


    }

    private void SetRaycasts(bool raycast, int type)
    {
        // if (type == -2)
        // {
        //     cageImage.raycastTarget = !raycast;
        // }
        // foreach (var r in normalRaycasts)
        // {
        //     r.raycastTarget = raycast;
        // }
    }


    private void EquipCage(bool equiped)
    {
        cageEquipped = equiped;
        SetRaycasts(!equiped, -2);
        if (hide) hide = false;

        if (pressSeq != null && pressSeq.IsPlaying())
            pressSeq.Complete();

        ResetScope(true);

        // if (hidingButton)


        if (cageSeq != null && cageSeq.IsPlaying())
            cageSeq.Kill();
        cageSeq = DOTween.Sequence();
        if (equiped)
        {
            fromCage = true;
            cage.gameObject.SetActive(true);
            cageSeq.Append(cageImage.DOFade(1, cageInDuration).From(0));
            cageSeq.Join(cageImage.transform.DOScale(1, cageInDuration).From(cageStartScale));
            cageSeq.Join(cageImage.transform.DOLocalMoveY(cageEndY, cageInDuration).From(cageStartY));
            eggSidebar.CollectCage(true, false, cageSeq, cageInDuration);

            if (!isHidden)
            {
                // cageSeq.Join(swipeRectsParent.DOLocalMoveY(swipeRectsStartPos - totalMoveY, cageInDuration).From(swipeRectsStartPos));
                cageSeq.Join(scopeRect.DOLocalMoveY(originalYPos - totalMoveY, cageInDuration).From(originalYPos));
                // cageSeq.Join(swipeRectsParent.DOScale(scaleChange, cageInDuration).From(1));
                cageSeq.Join(scopeRect.DOScale(scaleChange, cageInDuration).From(1));

            }


            cageSeq.Play().SetUpdate(true).SetEase(Ease.OutSine);

        }
        else
        {
            cageSeq.Append(cageImage.DOFade(0, cageOutDuration));
            cageSeq.Join(cageImage.transform.DOScale(cageStartScale, cageOutDuration));
            cageSeq.Join(cageImage.transform.DOLocalMoveY(cageStartY, cageOutDuration));
            eggSidebar.CollectCage(false, isHidden, cageSeq, cageOutDuration);

            if (!isHidden)
            {
                // cageSeq.Join(swipeRectsParent.DOLocalMoveY(swipeRectsStartPos, cageInDuration).From(swipeRectsStartPos - totalMoveY));
                cageSeq.Join(scopeRect.DOLocalMoveY(originalYPos, cageInDuration).From(originalYPos - totalMoveY));
                // cageSeq.Join(swipeRectsParent.DOScale(1, cageInDuration).From(scaleChange));
                cageSeq.Join(scopeRect.DOScale(1, cageInDuration).From(scaleChange));

            }

            cageSeq.Play().SetUpdate(true).OnComplete(() => cage.gameObject.SetActive(false));

        }




    }

    private void CageHit()
    {


        if (cageEquipped)
        {
            cage.rectTransform.DOShakeRotation(.2f, 20, 4);
        }
        cage.sprite = cageHitImage;

    }

    private void OnEnable()
    {
        player.UiEvents.OnSwitchDisplayedWeapon += SwitchAmmo;
        player.UiEvents.OnPressWeaponButton += PressScope;
        player.UiEvents.ReleaseScope += ResetScope;
        player.UiEvents.OnUseAmmo += UpdateAmmoAmountText;
        player.UiEvents.OnCageHit += CageHit;
        // player.events.OnAimJoystick += ShowJoystickArrow;
        // player.UiEvents.OnUseJoystick += SetJoystickActive;
        player.UiEvents.OnSetAmmoZero += SetScopeZero;
        player.UiEvents.OnUseChainedAmmo += ShowChainedAmmo;
        player.UiEvents.OnPressAmmoSideButton += PressSideButton;
        player.UiEvents.OnShowCage += EquipCage;
        player.UiEvents.OnAllowSwipe += EnableSwipeButton;
        player.UiEvents.UseScythePower += HandleScythePowerFill;
        player.UiEvents.ShowScythePower += ActivateScythePowerBar;
        player.UiEvents.ChangeScythePowerPitch += ChangeScythePowerPitch;


    }

    private void OnDisable()
    {
        player.UiEvents.OnSwitchDisplayedWeapon -= SwitchAmmo;
        player.UiEvents.OnPressWeaponButton -= PressScope;
        player.UiEvents.ReleaseScope -= ResetScope;
        player.UiEvents.OnUseAmmo -= UpdateAmmoAmountText;
        player.UiEvents.OnCageHit -= CageHit;
        // player.events.OnAimJoystick -= ShowJoystickArrow;
        // player.UiEvents.OnUseJoystick -= SetJoystickActive;
        player.UiEvents.OnSetAmmoZero -= SetScopeZero;
        player.UiEvents.OnUseChainedAmmo -= ShowChainedAmmo;
        player.UiEvents.OnPressAmmoSideButton -= PressSideButton;
        player.UiEvents.OnShowCage -= EquipCage;
        player.UiEvents.OnAllowSwipe -= EnableSwipeButton;
        player.UiEvents.UseScythePower -= HandleScythePowerFill;
        player.UiEvents.ShowScythePower -= ActivateScythePowerBar;
        player.UiEvents.ChangeScythePowerPitch -= ChangeScythePowerPitch;


        if (rotateSeq != null && rotateSeq.IsPlaying())
            rotateSeq.Kill();
        if (pressSeq != null && pressSeq.IsPlaying())
            pressSeq.Kill();
        if (flashRedSeq != null && flashRedSeq.IsPlaying())
            flashRedSeq.Kill();
        if (flashTextRedSeq != null && flashTextRedSeq.IsPlaying())
            flashTextRedSeq.Kill();
        if (chainGroupFadeSeq != null && chainGroupFadeSeq.IsPlaying())
            chainGroupFadeSeq.Kill();
        if (rotateSidebarSeq != null && rotateSidebarSeq.IsPlaying())
            rotateSidebarSeq.Kill();
        if (sideButtonSeq != null && sideButtonSeq.IsPlaying())
            sideButtonSeq.Kill();
        if (cageSeq != null && cageSeq.IsPlaying())
            cageSeq.Kill();
        if (scytheBarSeq != null && scytheBarSeq.IsPlaying())
            scytheBarSeq.Kill();











    }
}
