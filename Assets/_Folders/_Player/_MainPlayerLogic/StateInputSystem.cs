using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;








public class StateInputSystem : MonoBehaviour
{


    private InputController controls;

    private int trackedTouchIndex = -1;
    private int touchCount;
    private bool isTrackingTouch = false;

    private bool specialEnableButtonsActive = false;

    private Image flipLeftImage;
    private Image flipRightImage;


    private bool earlyDropReady;
    private bool earlyDashReady;
    private bool earlyDashTriggered;
    private bool stillHoldingDash;
    private bool earlyDropTried;
    private bool earlyDashTried;

    private bool usingShotgun = false;
    private Sequence dropButtonSeq;
    private Coroutine dropButtonCor;
    private Sequence dashButtonSeq;
    private bool coolingDownDrop;

    private bool eggButtonHidden = false;

    [Header("Global Colors")]
    [SerializeField] private ButtonColorsSO colorsSO;
    // [SerializeField] private Color normalButtonColor;
    // [SerializeField] private Color highlightButtonColor;
    // [SerializeField] private Color disabledButtonColor;
    // [SerializeField] private Color DashImageManaHighlight;
    // [SerializeField] private Color DashImageManaDisabled;
    // [SerializeField] private Color DashImageManaDisabledFilling;
    // [SerializeField] private Color coolDownColorRed;
    // [SerializeField] private Color coolDownColorBlue;

    [Header("Drop Stuff")]
    private Image DropButton;
    private Image DashButton;
    private bool finsihedCooldownDelay;
    private RectTransform dropIcon;
    private Image dropCooldownIN;
    private Image dropCooldownOUT;
    private bool canDrop;
    private Vector3 largeIconScale = new Vector3(1.2f, .8f, 1f);
    private Vector3 smallIconScale = new Vector3(.85f, 1.1f, 1);
    private Vector3 smallIconScale2 = new Vector3(.9f, .9f, .9f);
    private Vector3 normalIconScale = new Vector3(1, 1, 1);


    [Header("Dash Stuff")]

    private bool manaUsed = false;

    private Sequence flashSequence;
    private bool coolingDownDash;

    private bool canDash = true;
    private RectTransform dashIcon;
    private float dashTimeLeft;
    private bool canDashSlash;
    private Image dashImage;
    private Image dashImageMana;
    private Image dashCooldownIN;
    private CanvasGroup dashCooldownGroup;




    [Header("Dash Icon")]
    private Sequence dashIconSequence;
    private float startingDashIconX;
    [SerializeField] private float moveDashIconX;
    private Vector3 dashIconNormalScale = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 dashIconLargeScale;
    [SerializeField] private Vector3 dashIconLargeScale2;
    private Vector3 dashIconSmallScale = new Vector3(.85f, .9f, 1);






    [Header("Mana Settings")]
    private float maxMana = 100f;
    private bool manaFull = false;
    [SerializeField] private float mainManaFillDuration;
    private int manaGainedFromCollectable = 50;
    private float flashDuration = .7f;

    private Coroutine mainFillCourintine;


    private float currentMana = 0f; // Mana to regenerate per second

    [SerializeField] private Color fillingManaColor;
    [SerializeField] private Color canUseDashSlashImageColor1;
    [SerializeField] private Color canUseDashSlashImageColor2;

    private bool ButtonsEnabled;

    private Vector2 touchStartPosition;
    private float originalDropY;
    private float touchStartTime;
    private const float swipeThreshold = 0.3f; // Adjust as needed
    private const float tapTimeThreshold = 0.2f; // Adjust as needed
    public PlayerID ID;
    private bool parrySwipe = false;

    private int currentTouchCount;

    private int currentTrackedSwipe = 0;

    private bool currentlyTrackingSwipe = false;
    private bool isMousePressed = false;
    private bool trackingCenterTouch;



    [SerializeField] private RectTransform touchButtonRectTransform;


    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     Debug.Log("Touches is: " + Touch.activeTouches.Count);
    //     // Check if there are exactly two touches on the screen
    //     if (Touch.activeTouches.Count == 2)
    //     {
    //         // Two-finger touch start detected, invoke parachute open event
    //         // ID.IsTwoTouchPoints = true;
    //         parrySwipe = true;
    //         ID.events.OnPerformParry?.Invoke(true);
    //     }
    // }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     if (parrySwipe && Touch.activeTouches.Count < 2)
    //     {
    //         parrySwipe = false;
    //         ID.events.OnPerformParry?.Invoke(false);
    //     }
    //     // Debug.Log("Up");


    // }

    private bool trackingTwoFingerTouch;
    private void SetSwipesActive(bool doubleTap, Vector2 pos)
    {
        if (ButtonsEnabled && !trackingCenterTouch && !trackingTwoFingerTouch)
        {
            trackingCenterTouch = true;
            ID.events.OnTouchCenter?.Invoke(pos);
            ID.events.OnShowCursor?.Invoke(pos, 0);
        }


        // if ()
    }
    private void QuickCenterRelease()
    {
        // ID.events.ReleaseSwipe?.Invoke();
        trackingCenterTouch = false;
        ID.events.OnReleaseCenter?.Invoke();
    }

    private void TwoFingerTouch()
    {
        if (ButtonsEnabled)
        {
            ID.events.OnPerformParry?.Invoke(true);
            trackingTwoFingerTouch = true;
            trackingCenterTouch = false;
        }
    }
    private void Awake()
    {

        ID.UsingClocker = false;





        controls = new InputController();



        // control

        controls.Movement.ScytheStick.performed += ctx =>
        {
            ID.events.OnStuckScytheSwipe?.Invoke(ctx.ReadValue<Vector2>());
            // Debug.Log("Joystick Value: " + ctx.ReadValue<Vector2>());

        };

        // controls.Movement.ScytheStick.canceled += ctx =>
        // {
        //     ID.events.OnReleaseStick?.Invoke();
        //     // Debug.Log("Joystick Value: " + ctx.ReadValue<Vector2>());

        // };


        controls.Movement.MouseClick.performed += ctx => isMousePressed = true;
        controls.Movement.MouseClick.canceled += ctx =>
        {

            isMousePressed = false;
            if (trackingCenterTouch)
            {
                // ID.events.ReleaseSwipe?.Invoke();
                trackingCenterTouch = false;
                ID.events.OnReleaseCenter?.Invoke();
            }
        };

        controls.Movement.Cursor.performed += ctx =>
        {

            if (trackingCenterTouch && isMousePressed)
            {
                Vector2 p = ctx.ReadValue<Vector2>();
                ID.events.SendParrySwipeData?.Invoke(p);
                ID.events.OnDragCenter?.Invoke(p);

            }




        };







        // controls.Movement.Parry.canceled += ctx =>
        // {
        //     ID.events.OnPerformParry?.Invoke(false);
        // };

        controls.Movement.Finger1.performed += ctx =>
      {

          if (trackingCenterTouch && trackedTouchIndex == 1)
          {
              Vector2 p = ctx.ReadValue<Vector2>();
              ID.events.SendParrySwipeData?.Invoke(p);
              ID.events.OnDragCenter?.Invoke(p);
          }

      };


        controls.Movement.Finger1Press.canceled += ctx =>
       {


           if (trackingCenterTouch && trackedTouchIndex == 1)
           {
               trackingCenterTouch = false;
               ID.events.OnReleaseCenter?.Invoke();
           }


       };

        controls.Movement.Finger2.performed += ctx =>
        {
            if (trackingCenterTouch && trackedTouchIndex == 2)
            {
                Vector2 p = ctx.ReadValue<Vector2>();
                ID.events.SendParrySwipeData?.Invoke(p);
                ID.events.OnDragCenter?.Invoke(p);
            }



        };

        controls.Movement.Finger2Press.canceled += ctx =>
       {
           trackingTwoFingerTouch = false;
           if (trackingCenterTouch && trackedTouchIndex == 2)
           {
               trackingCenterTouch = false;
               ID.events.OnReleaseCenter?.Invoke();
           }


       };

        controls.Movement.Finger1Press.performed += ctx =>
        {


            if (!trackingCenterTouch)
                trackedTouchIndex = 1;
            // currentTouchCount++;

            // if (currentTouchCount == 2 && !ID.pressingButton)
            // {
            //     ID.events.OnPerformParry?.Invoke(true);
            // }

            // 


        };

        controls.Movement.Finger2Press.performed += ctx =>
       {
           if (!trackingCenterTouch)
               trackedTouchIndex = 2;

       };

        controls.Movement.Parry.performed += ctx =>
         {
             if (ButtonsEnabled)
                 ID.events.OnPerformParry?.Invoke(true);
         };






        //     controls.Movement.Finger1Press.canceled += ctx =>
        //    {
        //        if (ID.trackParrySwipe)
        //        {
        //            // ID.events.OnTrackParrySwipe?.Invoke(false);
        //            ID.events.ReleaseSwipe?.Invoke();

        //            currentlyTrackingSwipe = false;
        //        }


        //    };

        // controls.Movement.Finger2Press.canceled += ctx =>
        // {
        //     currentTouchCount--;

        //     if (currentTouchCount < 0) currentTouchCount = 0;

        //     if (ID.trackParrySwipe)
        //     {
        //         ID.events.OnTrackParrySwipe?.Invoke(false);
        //         currentlyTrackingSwipe = false;
        //         if (currentTouchCount == 0)
        //         {
        //             ID.events.OnPerformParry?.Invoke(false);
        //             currentTrackedSwipe = 0;

        //         }
        //         else if (currentTouchCount == 1)
        //         {
        //             currentTrackedSwipe = 2;
        //         }
        //     }


        // };









        // controls.Movement.Finger2.performed += ctx =>
        // {



        //     if (ID.trackParrySwipe && currentTrackedSwipe == 2)
        //     {
        //         if (!currentlyTrackingSwipe)
        //         {
        //             ID.events.OnTrackParrySwipe?.Invoke(true);
        //             currentlyTrackingSwipe = true;
        //         }
        //         Vector2 moveAmount = ctx.ReadValue<Vector2>();
        //         // Debug.Log("Finger2 is : " + moveAmount);
        //         ID.events.SendParrySwipeData?.Invoke(moveAmount);
        //     }




        // };




        //     controls.Movement.Finger1.canceled += ctx =>
        //    {





        //    };

        //     controls.Movement.Finger2.canceled += ctx =>
        //    {

        //        currentTouchCount--;

        //        if (currentTouchCount < 0) currentTouchCount = 0;

        //        if (ID.trackParrySwipe)
        //        {
        //            ID.events.OnTrackParrySwipe?.Invoke(false);
        //            currentlyTrackingSwipe = false;


        //            if (currentTouchCount == 0)
        //            {
        //                ID.events.OnPerformParry?.Invoke(false);
        //                currentTrackedSwipe = 0;

        //            }
        //            else if (currentTouchCount == 1)
        //            {
        //                currentTrackedSwipe = 2;
        //            }
        //        }




        //    };


        // Bind existing actions to methods


        controls.Movement.Jump.performed += ctx =>
        {
            if (ButtonsEnabled && !ID.trackParrySwipe)
            {
                Debug.LogError("Pressed Jump");
                SpecialEnableButtonsCheck(0);
                ID.events.OnJump?.Invoke();
                HapticFeedbackManager.instance.PlayerButtonPress();


            }
        };

        controls.Movement.JumpRight.performed += ctx =>
        {
            ID.pressingFlipRButton = true;

            if (ButtonsEnabled)
            {
                SpecialEnableButtonsCheck(1);
                flipRightImage.color = colorsSO.highlightButtonColor;
                ID.events.OnFlipRight?.Invoke(true);
                HapticFeedbackManager.instance.PlayerButtonPress();

            }
        };
        controls.Movement.JumpRight.canceled += ctx =>
        {
            ID.pressingFlipRButton = false;

            if (ButtonsEnabled) ID.events.OnFlipRight?.Invoke(false);
            flipRightImage.color = colorsSO.normalButtonColor;
        };

        controls.Movement.JumpLeft.performed += ctx =>
        {
            ID.pressingFlipLButton = true;

            if (ButtonsEnabled)
            {
                SpecialEnableButtonsCheck(2);
                flipLeftImage.color = colorsSO.highlightButtonColor;

                ID.events.OnFlipLeft?.Invoke(true);
                HapticFeedbackManager.instance.PlayerButtonPress();

            }
        };
        controls.Movement.JumpLeft.canceled += ctx =>
        {
            ID.pressingFlipLButton = false;
            if (ButtonsEnabled) ID.events.OnFlipLeft?.Invoke(false);
            flipLeftImage.color = colorsSO.normalButtonColor;
        };

        controls.Movement.EggJoystick.performed += ctx =>
        {


            if (ID.canUseJoystick)
            {
                Vector2 moveAmount = ctx.ReadValue<Vector2>().normalized;
                ID.events.OnAimJoystick?.Invoke(moveAmount);
            }
        };

        controls.Movement.EggJoystick.canceled += ctx => ID.events.OnAimJoystick(Vector2.zero);

        controls.Movement.Dash.performed += ctx =>
        {
            ID.pressingDashButton = true;
            if (ButtonsEnabled)
            {

                if (canDash)
                {
                    SpecialEnableButtonsCheck(3);
                    Debug.LogError("Pressed Dash");
                    ID.events.OnDash?.Invoke(true);
                    HapticFeedbackManager.instance.PlayerButtonPress();


                }
                else if (earlyDashReady)
                {
                    SpecialEnableButtonsCheck(3);
                    stillHoldingDash = true;
                    Debug.LogError("Tried to use coyote time");

                    earlyDashTried = true;
                }
                else if (!earlyDashReady)
                {

                    HapticFeedbackManager.instance.PlayerButtonFailure();
                }



                // else if (canDashSlash)
                // {
                //     manaFull = false;

                //     HandleDashSlashImage(false);
                //     ID.events.OnDashSlash?.Invoke();
                // }



            }
        };
        controls.Movement.Dash.canceled += ctx =>
        {
            ID.pressingDashButton = false;
            if (ButtonsEnabled)
            {
                stillHoldingDash = false;
                if (!coolingDownDash && !canDash)
                {

                    ID.events.OnDash?.Invoke(false);


                }
                else if (earlyDashTriggered)
                {
                    ID.events.OnDash?.Invoke(false);
                    earlyDashTriggered = false;

                }


            }
        };

        controls.Movement.SwitchAmmoRight.performed += ctx =>
        {
            // if (!eggButtonHidden)
            //     ID.globalEvents.OnSwitchAmmo?.Invoke(false);
            // if (ID.canPressEggButton)
            ID.UiEvents.OnSwitchWeapon?.Invoke(1, -1);


        };




        controls.Movement.Drop.canceled += ctx =>
        {
            ID.pressingDropButton = false;
        };

        controls.Movement.Drop.performed += ctx =>
        {
            ID.pressingDropButton = true;
            if (ButtonsEnabled)
            {


                if (canDrop)
                {
                    SpecialEnableButtonsCheck(4);
                    ID.events.OnDrop?.Invoke();
                    HapticFeedbackManager.instance.PlayerButtonPress();


                    StartCoroutine(DropCooldown());

                }
                else if (earlyDropReady)
                {
                    SpecialEnableButtonsCheck(4);
                    earlyDropTried = true;
                }


                else if (!earlyDropReady)
                {

                    HapticFeedbackManager.instance.PlayerButtonFailure();
                }



            }
        };
        controls.Movement.DropEgg.performed += ctx =>
        {
            ID.pressingEggButton = true;
            if (ButtonsEnabled && !eggButtonHidden)
            {
                ID.events.OnPressAmmo?.Invoke(true);


            }
        };

        controls.Movement.DropEgg.canceled += ctx =>
       {
           ID.pressingEggButton = false;
           //    if (ID.canPressEggButton)
           ID.events.OnPressAmmo?.Invoke(false);

           //    if (usingShotgun)
           //        ID.events.OnAttack?.Invoke(false);
       };

        controls.Movement.JumpHold.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnJumpHeld?.Invoke(true);
        };

        controls.Movement.JumpHold.canceled += ctx =>
        {
            ID.events.OnJumpHeld?.Invoke(false);

        };


        controls.Movement.Parachute.performed += ctx => ID.events.OnParachute?.Invoke(true);
        controls.Movement.Parachute.canceled += ctx => ID.events.OnParachute?.Invoke(false);





    }






    private void SpecialEnableButtonsCheck(int type)
    {

        if (specialEnableButtonsActive)
        {
            ID.globalEvents.OnInputWithSpecialEnableButtons?.Invoke();
            specialEnableButtonsActive = false;
        }
        else
        {
            ID.AddPlayerInput(type);
        }
    }

    private void TrackTouchPosition(Vector2 pos)
    {

    }

    void Start()
    {
        // normalButtonColor = colorsSO.normalButtonColor;
        // highlightButtonColor = colorsSO.highlightButtonColor;
        // disabledButtonColor = colorsSO.disabledButtonColor;
        // DashImageManaHighlight = colorsSO.DashImageManaHighlight;
        // DashImageManaDisabled = colorsSO.DashImageManaDisabled;
        // DashImageManaDisabledFilling = colorsSO.DashImageManaDisabledFilling;
        // coolDownColorRed = colorsSO.coolDownColorRed;
        // coolDownColorBlue = colorsSO.coolDownColorBlue;
        // fillingManaColor = colorsSO.fillingManaColor;
        // canUseDashSlashImageColor1 = colorsSO.canUseDashSlashImageColor1;
        // canUseDashSlashImageColor2 = colorsSO.canUseDashSlashImageColor2;

        if (GameObject.Find("FlipRightIMG") != null)
            flipRightImage = GameObject.Find("FlipRightIMG").GetComponent<Image>();

        if (GameObject.Find("FlipLeftIMG") != null)
            flipLeftImage = GameObject.Find("FlipLeftIMG").GetComponent<Image>();

        if (GameObject.Find("DropButton") != null)
        {
            DropButton = GameObject.Find("DropButton").GetComponent<Image>();
            DropButton.color = colorsSO.normalButtonColor;
            dropCooldownIN = GameObject.Find("DropCooldownIN").GetComponent<Image>();
            dropIcon = GameObject.Find("DropICON").GetComponent<RectTransform>();
            dropCooldownOUT = GameObject.Find("DropCooldownOUT").GetComponent<Image>();
            originalDropY = dropIcon.anchoredPosition.y;
            dropCooldownIN.color = colorsSO.disabledButtonColorFull;
            dropCooldownIN.DOFade(0, 0);
            dropCooldownOUT.DOFade(0, 0);


        }
        canDrop = true;

        if (GameObject.Find("DashButton") != null)
        {
            dashImage = GameObject.Find("DashIMG").GetComponent<Image>();


            dashCooldownIN = GameObject.Find("DashCooldownIN").GetComponent<Image>();
            dashIcon = GameObject.Find("DashICON").GetComponent<RectTransform>();
            dashCooldownGroup = GameObject.Find("DashCooldownGroup").GetComponent<CanvasGroup>();

            startingDashIconX = dashIcon.anchoredPosition.x;


            dashCooldownGroup.alpha = 0;

            canDashSlash = false;
            coolingDownDash = false;



            dashImageMana = GameObject.Find("DashIMGMana").GetComponent<Image>();


            ID.globalEvents.SetCanDashSlash?.Invoke(false);

            FullMana(false);

            if (manaUsed)
            {

                dashImageMana.color = fillingManaColor;
                dashImageMana.enabled = true;
                dashImageMana.fillAmount = (currentMana / maxMana);
                mainFillCourintine = StartCoroutine(RegenerateMana());



            }
            else
                dashImageMana.gameObject.SetActive(false);


        }



    }

    public bool dashSlashReady()
    {
        if (!coolingDownDash && manaFull)
            return true;
        else
            return false;
    }




    private IEnumerator DropCooldown()
    {
        coolingDownDrop = true;
        canDrop = false;
        // dashButtonIMG.DOColor(DisabledButtonColor, .25f);
        DropButton.DOColor(colorsSO.highlightButtonColor, .15f);
        dropIcon.DOScale(largeIconScale, .3f).SetEase(Ease.OutSine);
        dropIcon.DOAnchorPosY(originalDropY - 30, .2f).SetEase(Ease.OutSine);
        dropCooldownIN.DOFade(.7f, .15f);
        dropCooldownOUT.DOFade(1, .15f);

        dropCooldownIN.DOFillAmount(0, 2.29f).From(1);
        yield return new WaitForSeconds(.2f);
        DropButton.DOColor(colorsSO.disabledButtonColor, .2f);
        dropIcon.DOScale(smallIconScale, .15f).SetEase(Ease.InSine);
        dropIcon.DOAnchorPosY(originalDropY, .3f).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(.15f);
        dropIcon.DOScale(smallIconScale2, .25f).SetEase(Ease.InOutSine);



        yield return new WaitForSeconds(1.54f);
        // earlyDropTried = false;
        earlyDropReady = true;
        yield return new WaitForSeconds(.13f);
        dropCooldownIN.DOFade(0, 0);
        dropCooldownOUT.DOFade(0, .1f);

        // dashButtonIMG.DOColor(ButtonNormalColor, .15f);
        DropButton.DOColor(colorsSO.normalButtonColor, .13f);
        dropIcon.DOScale(normalIconScale, .13f);






        coolingDownDrop = false;
        canDrop = true;
        earlyDropReady = false;
        if (earlyDropTried)
        {
            Debug.LogError("Coyote time was used on dropppp");
            ID.events.OnDrop?.Invoke();
            HapticFeedbackManager.instance.PlayerButtonPress();
            earlyDropTried = false;


            StartCoroutine(DropCooldown());

        }

    }

    private void HideEggButton(bool hidden)
    {
        eggButtonHidden = hidden;
    }

    private void IconTween(int type)
    {
        if (dashIconSequence != null && dashIconSequence.IsPlaying())
        {
            dashIconSequence.Kill();


        }
        dashIconSequence = DOTween.Sequence();

        if (type == 0)
        {
            dashIconSequence.Append(dashIcon.DOAnchorPosX(startingDashIconX, .2f).SetEase(Ease.OutSine));
            dashIconSequence.Join(dashIcon.DOScale(dashIconNormalScale, .2f));
            dashIconSequence.Play();

        }
        else if (type == 1)
        {

            dashIconSequence.Append(dashIcon.DOAnchorPosX(startingDashIconX + (moveDashIconX * .6f), .15f));
            dashIconSequence.Join(dashIcon.DOScale(dashIconLargeScale, .15f).SetEase(Ease.InSine));
            dashIconSequence.Append(dashIcon.DOAnchorPosX(startingDashIconX + moveDashIconX, .2f).SetEase(Ease.OutSine));
            dashIconSequence.Join(dashIcon.DOScale(dashIconLargeScale2, .2f).SetEase(Ease.InSine));

            dashIconSequence.Play();

        }
        else if (type == 2)
        {
            dashIconSequence.Append(dashIcon.DOAnchorPosX(startingDashIconX, .4f).SetEase(Ease.OutSine));
            dashIconSequence.Join(dashIcon.DOScale(dashIconSmallScale, .3f));
            dashIconSequence.Play();

        }

    }

    void HandleDashNew(bool isDashing)
    {
        if (isDashing)
        {
            if (canDash)
            {
                canDash = false;
                dashImage.color = colorsSO.highlightButtonColor;
                StartCoroutine(CalcualteDashTimeLeft());
                IconTween(1);

            }

        }


        if (!coolingDownDash && !isDashing && !canDash)
        {
            StartCoroutine(DashCooldown());


        }



    }

    // private void FlashButton()
    // {
    //     canDashSlash = true;
    //     flashButtonSequence = DOTween.Sequence();
    //     flashButtonSequence.Append(dashButtonIMG.DOColor(ButtonCanSlashColor1, .2f));
    //     flashButtonSequence.Append(dashButtonIMG.DOColor(ButtonCanSlashColor2, .2f));
    //     flashButtonSequence.Append(dashButtonIMG.DOColor(ButtonCanSlashColor1, .2f));
    //     flashButtonSequence.Append(dashButtonIMG.DOColor(ButtonCanSlashColor2, .15f));


    //     flashButtonSequence.OnComplete(EndFlashButton);
    //     flashButtonSequence.Play();

    // }


    private void FullMana(bool isFull)
    {
        manaFull = isFull;

        if (isFull)
        {
            ID.globalEvents.SetCanDashSlash?.Invoke(true);

            dashCooldownIN.color = colorsSO.coolDownColorBlue;

        }
        else
        {
            ID.globalEvents.SetCanDashSlash?.Invoke(false);
            dashCooldownIN.color = colorsSO.disabledButtonColorFull;
            Debug.Log("Set Color");


        }

    }
    private void ExitDash(bool notExited)
    {
        if (!notExited)
        {
            canDashSlash = false;
            if (manaFull)
            {
                dashCooldownIN.color = colorsSO.disabledButtonColorFull;
                dashImageMana.DOColor(colorsSO.DashImageManaDisabled, .1f);

            }
            IconTween(2);

        }
        else
        {
            if (manaFull)
            {
                canDashSlash = true;
                dashCooldownIN.color = colorsSO.coolDownColorBlue;
                FlashDashImage(.8f);

            }
            else
            {
                dashImageMana.color = fillingManaColor;

            }

        }
    }

    private IEnumerator CalcualteDashTimeLeft()
    {
        // dashTimeLeft = .35f;
        dashTimeLeft = .32f;
        while (dashTimeLeft >= 0 && !coolingDownDash)
        {
            dashTimeLeft -= Time.deltaTime;
            yield return null;
        }
    }

    public void FinishCooldowns()
    {
        // type 0 = dash;  type 1 = drop;

        ID.events.OnDash?.Invoke(false);
        StopAllCoroutines();

        dashCooldownGroup.alpha = 0;
        if (dashIconSequence != null && dashIconSequence.IsPlaying())
            dashIconSequence.Kill();

        dashIcon.DOAnchorPosX(startingDashIconX, .2f).SetEase(Ease.OutSine).SetUpdate(true);
        dashIcon.DOScale(dashIconNormalScale, .2f).SetUpdate(true);
        // dashButtonIMG.DOColor(ButtonNormalColor, .15f);
        dashImage.DOColor(colorsSO.normalButtonColor, .15f);

        canDash = true;

        coolingDownDash = false;
        if (manaFull)
        {
            canDashSlash = true;
            dashCooldownIN.color = colorsSO.coolDownColorBlue;
            if (flashSequence != null && flashSequence.IsPlaying())
                flashSequence.Kill();

            flashSequence = DOTween.Sequence();
            flashSequence.Append(dashImageMana.DOColor(canUseDashSlashImageColor1, flashDuration).SetEase(Ease.OutSine).From(canUseDashSlashImageColor2).SetUpdate(true));
            flashSequence.Append(dashImageMana.DOColor(canUseDashSlashImageColor2, flashDuration).SetEase(Ease.OutSine).SetUpdate(true));
            flashSequence.SetLoops(-1).SetUpdate(true);
            flashSequence.Play().SetUpdate(true);


        }
        else if (manaUsed)
        {
            dashImageMana.color = fillingManaColor;

            StartCoroutine(RegenerateMana());

        }

        dropCooldownIN.DOFade(0, .13f).SetUpdate(true);
        dropCooldownOUT.DOFade(0, .13f).SetUpdate(true);

        // dashButtonIMG.DOColor(ButtonNormalColor, .15f);
        DropButton.DOColor(colorsSO.normalButtonColor, .13f).SetUpdate(true);
        dropIcon.DOScale(normalIconScale, .13f).SetUpdate(true);




        coolingDownDrop = false;
        canDrop = true;



    }

    private void SetFinishedCooldownDelayFalse()
    {
        coolingDownDash = false;
    }



    private IEnumerator DashCooldown()
    {

        coolingDownDash = true;
        canDash = false;


        if (manaFull)
        {
            canDashSlash = true;
            flashSequence.Kill();
            dashImageMana.DOColor(colorsSO.DashImageManaHighlight, .1f);

        }
        else
        {

            yield return new WaitForSeconds(dashTimeLeft);

            IconTween(2);
        }
        // else
        // {
        //     dashImageMana.color = DashImageManaDisabledFilling;
        // }
        // dashButtonIMG.DOColor(DisabledButtonColor, .25f);
        dashImage.DOColor(colorsSO.disabledButtonColor, .25f);

        dashCooldownGroup.DOFade(1, .15f);

        dashCooldownIN.DOFillAmount(0, 1.3f).From(1);
        yield return new WaitForSeconds(.95f);
        // earlyDashTried = false;
        earlyDashReady = true;
        yield return new WaitForSeconds(.15f);



        dashCooldownGroup.DOFade(0, .15f);

        // dashButtonIMG.DOColor(ButtonNormalColor, .15f);

        dashImage.DOColor(colorsSO.normalButtonColor, .15f);
        earlyDashReady = false;
        if (earlyDashTried && ButtonsEnabled)
        {
            coolingDownDash = false;
            earlyDashTriggered = true;
            Debug.LogError("Coyote time was used");
            canDash = true;
            ID.events.OnDash?.Invoke(true);
            HapticFeedbackManager.instance.PlayerButtonPress();
            earlyDashTried = false;
            if (!stillHoldingDash)
            {
                ID.events.OnDash?.Invoke(false);
                earlyDashTriggered = false;
            }

            // earlyDashTried = false;
        }
        else
        {
            IconTween(0);

            canDash = true;

            coolingDownDash = false;
            ExitDash(true);
        }


    }

    private void FlashDashImage(float duration)
    {
        if (flashSequence != null && flashSequence.IsPlaying())
            flashSequence.Kill();

        if (duration > 0)
        {

            flashSequence = DOTween.Sequence();
            flashSequence.Append(dashImageMana.DOColor(canUseDashSlashImageColor1, flashDuration).SetEase(Ease.OutSine).From(canUseDashSlashImageColor2));
            flashSequence.Append(dashImageMana.DOColor(canUseDashSlashImageColor2, flashDuration).SetEase(Ease.OutSine));
            flashSequence.SetLoops(-1);
            flashSequence.Play();
        }







    }

    void HandleDashSlashImage(bool canSlash)
    {
        // canDashSlash = canSlash;

        if (!canSlash)
        {
            dashImageMana.color = fillingManaColor;
            FlashDashImage(0);
            // HandleDashSlashButton(false);
            StartCoroutine(SetMana(0, .3f));
        }
        else
        {


            FlashDashImage(.8f);
        }
    }

    private void GatherMana()
    {
        if (currentMana < maxMana)
            StartCoroutine(SetMana(currentMana + manaGainedFromCollectable, .5f));
    }

    private IEnumerator RegenerateMana()
    {
        Debug.Log("Please Regen");
        float elapsedTime = 0;
        float previousMana = currentMana;
        float duration = (maxMana - currentMana) * (mainManaFillDuration / maxMana);



        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // Regenerate mana at the specified rate
            currentMana = Mathf.Lerp(previousMana, maxMana, elapsedTime / duration);
            dashImageMana.fillAmount = (currentMana / maxMana);
            yield return null;
        }
        // canDashSlash = true;

        HandleDashSlashImage(true);
        FullMana(true);

        currentMana = maxMana;




    }


    private IEnumerator SetMana(float target, float duration)
    {
        float elapsedTime = 0;
        float previousMana = currentMana;
        bool finished = false;
        if (target != 0)
        {
            StopCoroutine(mainFillCourintine);
            // if (target >= maxMana)
            // {
            //     HandleDashSlashImage(true);
            //     // player.globalEvents.SetCanDashSlash(true);
            // }

        }


        while (elapsedTime < duration && !finished)
        {
            elapsedTime += Time.deltaTime;
            // Regenerate mana at the specified rate
            currentMana = Mathf.Lerp(previousMana, target, elapsedTime / duration);
            dashImageMana.fillAmount = (currentMana / maxMana);


            if (currentMana > maxMana)
            {
                FullMana(true);
                HandleDashSlashImage(true);
                finished = true;


            }
            else if (currentMana <= 0)
            {
                finished = true;
                currentMana = 0;
                FullMana(false);
                dashImageMana.fillAmount = (0);
                yield return new WaitForSeconds(.3f);
                mainFillCourintine = StartCoroutine(RegenerateMana());
            }
            yield return null;
        }

        if (!finished)
        {
            currentMana = target;
            mainFillCourintine = StartCoroutine(RegenerateMana());
        }



    }

    public void ActivateButtons(bool IsActive)
    {
        ButtonsEnabled = IsActive;


    }

    public void SpecialActivateButtons(bool isActive)
    {
        ButtonsEnabled = isActive;

        if (!isActive)
            specialEnableButtonsActive = true;

    }

    private void SetUsingShotgun(int type)
    {
        if (type == 1)
            usingShotgun = true;
        else
            usingShotgun = false;

    }



    private void OnEnable()
    {
        controls.Movement.Enable();

        ID.events.OnPointerCenter += SetSwipesActive;
        ID.events.TwoFingerCenterTouch += TwoFingerTouch;
        ID.globalEvents.OnHideEggButton += HideEggButton;

        ID.events.EnableButtons += ActivateButtons;
        ID.events.OnDash += HandleDashNew;

        ID.events.OnSwitchAmmoType += SetUsingShotgun;


        ID.globalEvents.CanDashSlash += ExitDash;
        ID.events.SpecialEnableButtons += SpecialActivateButtons;
        ID.events.QuickCenterRelease += QuickCenterRelease;

        if (manaUsed)
            ID.globalEvents.OnGetMana += GatherMana;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        DOTween.Kill(this);
        controls.Movement.Disable();
        ID.globalEvents.OnHideEggButton -= HideEggButton;
        ID.events.TwoFingerCenterTouch -= TwoFingerTouch;


        ID.events.EnableButtons -= ActivateButtons;
        ID.events.OnDash -= HandleDashNew;
        ID.globalEvents.CanDashSlash -= ExitDash;
        ID.events.OnSwitchAmmoType -= SetUsingShotgun;
        ID.events.SpecialEnableButtons -= SpecialActivateButtons;


        ID.events.OnPointerCenter -= SetSwipesActive;
        ID.events.QuickCenterRelease -= QuickCenterRelease;




        // player.globalEvents.SetCanDashSlash -= HandleDashSlashImage;
        // player.globalEvents.CanDashSlash -= HandleDashSlashButton;
        if (manaUsed)
            ID.globalEvents.OnGetMana -= GatherMana;

    }
}

// private void StartTouch(InputAction.CallbackContext context)
// {
//     touchStartPosition = controls.Movement.TouchPosition.ReadValue<Vector2>();
//      Debug.Log($"Tap Detected - Start Position: {touchStartPosition}");
//     touchStartTime = Time.time;
// }

// private void UpdateTouchPosition(InputAction.CallbackContext context)
// {
//     // Optional: Logic for continuous touch position updates
// }

// private void EndTouch(InputAction.CallbackContext context)
// {
//     Vector2 touchEndPosition = controls.Movement.TouchPosition.ReadValue<Vector2>();
//     float touchDuration = Time.time - touchStartTime;
//     Vector2 swipeDelta = touchEndPosition - touchStartPosition;

//      Debug.Log($"End Position: {touchEndPosition} (Before Threshold Check)");

//     if (touchDuration < tapTimeThreshold && swipeDelta.magnitude < swipeThreshold)

//     if (IsTouchWithinButton(touchEndPosition))
//     {
//         if (touchDuration < tapTimeThreshold && swipeDelta.magnitude < swipeThreshold)
//         {
//              Debug.Log($"Tap Detected - Start Position: {touchStartPosition}, End Position: {touchEndPosition}");
//             // It's a tap within the button bounds
//             ID.events.OnEggDrop?.Invoke();
//         }
//         else if (swipeDelta.y < -swipeThreshold) // Negative Y indicates a swipe down
//         {
//             // It's a swipe down within the button bounds
//             ID.events.OnDrop?.Invoke();
//         }
//     }
// }

// private bool IsTouchWithinButton(Vector2 touchPosition)
// {
//     if (touchButtonRectTransform == null) return false;

//     Vector2 localPoint;
//     RectTransformUtility.ScreenPointToLocalPointInRectangle(
//         touchButtonRectTransform,
//         touchPosition,
//         null,
//         out localPoint
//     );

//     return touchButtonRectTransform.rect.Contains(localPoint);
// }



// Add similar methods for other actions if needed
// ...


