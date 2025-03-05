using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;


public class SpecialStateInputSystem : MonoBehaviour
{
    [SerializeField] private bool useFlips;
    [SerializeField] private bool useDash;
    [SerializeField] private bool useDrop;
    [SerializeField] private bool useEgg;
    [SerializeField] private bool useShotgun;

    private bool isInitialized;

    private bool usingShotgun = false;
    private bool fillAllMana = false;


    private bool earlyDropReady;
    private bool earlyDashReady;
    private bool earlyDashTriggered;
    private bool stillHoldingDash;
    private bool earlyDropTried;
    private bool earlyDashTried;


    private Color originalButtonColor;
    private Sequence flashButtonSeq;
    private List<Image> currentFlashImages;

    private Image flipLeftImage;
    private Image flipRightImage;
    private Image eggImage;
    [SerializeField] private LevelManagerID lvlID;
    private InputController controls;
    private Sequence dropButtonSeq;
    private Coroutine dropButtonCor;
    private Sequence dashButtonSeq;
    private bool coolingDownDrop;

    private bool eggButtonHidden = false;

    private List<string> inputsTracked = new List<string>();

    [Header("Global Colors")]

    [SerializeField] private ButtonColorsSO ColorSO;
    // private Color normalButtonColor;
    // private Color highlightButtonColor;
    // private Color disabledButtonColor;
    // private Color DashImageManaHighlight;
    // private Color DashImageManaDisabled;
    // private Color DashImageManaDisabledFilling;
    // private Color coolDownColorRed;
    // private Color coolDownColorBlue;

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

    [SerializeField] private bool manaUsed;

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

    private int currentEquipedAmmo = 0;



    [Header("Dash Icon")]
    private Sequence dashIconSequence;
    private float startingDashIconX;
    private int moveDashIconX = 30;
    private Vector3 dashIconNormalScale = new Vector3(1, 1, 1);
    private Vector3 dashIconLargeScale = new Vector3(1.5f, .8f, 1);
    private Vector3 dashIconLargeScale2 = new Vector3(1.1f, 1.2f, 1.2f);

    private Vector3 dashIconSmallScale = new Vector3(.85f, .9f, 1);



    private bool usingDash;
    private bool usingDrop;






    [Header("Mana Settings")]
    private float maxMana = 100f;
    private bool manaFull = false;
    [SerializeField] private float mainManaFillDuration;
    [SerializeField] private int manaGainedFromCollectable = 40;
    private float flashDuration = .7f;

    private Coroutine mainFillCourintine;


    private float currentMana = 0f; // Mana to regenerate per second

    private bool lockAfterInputCheck = false;

    private Color fillingManaColor;
    private Color canUseDashSlashImageColor1;
    private Color canUseDashSlashImageColor2;


    private bool ButtonsEnabled;

    private bool doCooldownsBool = true;

    private Vector2 touchStartPosition;
    private float originalDropY;
    private float touchStartTime;
    private const float swipeThreshold = 0.3f; // Adjust as needed
    private const float tapTimeThreshold = 0.2f; // Adjust as needed
    public PlayerID ID;

    [SerializeField] private RectTransform touchButtonRectTransform;

    [SerializeField] private bool cannotUseManaUntilLockedInputCheck;




    private bool mustHold;
    private bool startedHold;
    private bool trackingInputs;
    private float mustHoldDuration;

    private bool anyInput;

    private bool flashDashBool = false;
    private bool flashDropBool = false;
    private bool startedFlashing = false;




    private void Awake()
    {
        ID.UsingClocker = false;
        startedHold = false;





        controls = new InputController();

        // Bind existing actions to methods

        controls.Movement.Jump.performed += ctx =>
          {
              if (!ButtonsEnabled) return;
              if (!trackingInputs)
              {
                  ID.events.OnJump?.Invoke();
                  HapticFeedbackManager.instance.PlayerButtonPress();
              }
              else if (CheckInputs("Jump"))
              {
                  if (!startedHold)
                  {
                      ID.events.OnJump?.Invoke();
                      HapticFeedbackManager.instance.PlayerButtonPress();

                  }

                  else
                  {
                      Time.timeScale = FrameRateManager.TargetTimeScale;
                      lvlID.outputEvent.SetPressButtonText?.Invoke(false, 3, "JUMP");
                      return;
                  }


                  if (!mustHold && !lockAfterInputCheck) trackingInputs = false;
                  else
                  {
                      startedHold = true;
                      StartCoroutine(MustHoldInput(mustHoldDuration));
                  }
              }
          };

        if (useFlips) controls.Movement.JumpRight.performed += ctx =>
        {
            if (!ButtonsEnabled) return;
            flipRightImage.color = ColorSO.highlightButtonColor;

            if (!trackingInputs)
            {
                ID.events.OnFlipRight?.Invoke(true);
                HapticFeedbackManager.instance.PlayerButtonPress();
            }
            else if (CheckInputs("FlipRight"))
            {
                if (!startedHold)
                {
                    ID.events.OnFlipRight?.Invoke(true);
                    HapticFeedbackManager.instance.PlayerButtonPress();
                }

                else
                {
                    Time.timeScale = FrameRateManager.TargetTimeScale;
                    lvlID.outputEvent.SetPressButtonText?.Invoke(false, 3, "JUMP");
                    return;
                }


                if (!mustHold && !lockAfterInputCheck) trackingInputs = false;
                else
                {
                    startedHold = true;
                    StartCoroutine(MustHoldInput(mustHoldDuration));
                }
            }
        };

        if (useFlips) controls.Movement.JumpRight.canceled += ctx =>
        {
            flipRightImage.color = ColorSO.normalButtonColor;

            if (!mustHold)
                ID.events.OnFlipRight?.Invoke(false);

            else if (ButtonHeldCheckInput("FlipRight"))
            {
                Time.timeScale = 0;
                lvlID.outputEvent.SetPressButtonText?.Invoke(true, 3, "Front Flip");
            }
        };

        if (useFlips) controls.Movement.JumpLeft.performed += ctx =>
        {
            if (!ButtonsEnabled) return;

            flipLeftImage.color = ColorSO.highlightButtonColor;

            if (!trackingInputs)
            {
                HapticFeedbackManager.instance.PlayerButtonPress();
                ID.events.OnFlipLeft?.Invoke(true);
            }
            else if (CheckInputs("FlipLeft"))
            {
                if (!startedHold)
                {
                    ID.events.OnFlipLeft?.Invoke(true);
                    HapticFeedbackManager.instance.PlayerButtonPress();
                }

                else
                {
                    Time.timeScale = FrameRateManager.TargetTimeScale;
                    lvlID.outputEvent.SetPressButtonText?.Invoke(false, 3, "JUMP");
                    return;
                }


                if (!mustHold && !lockAfterInputCheck) trackingInputs = false;
                else
                {
                    startedHold = true;
                    StartCoroutine(MustHoldInput(mustHoldDuration));
                }
            }
        };

        if (useFlips) controls.Movement.JumpLeft.canceled += ctx =>
        {
            flipLeftImage.color = ColorSO.normalButtonColor;

            if (!mustHold)
                ID.events.OnFlipLeft?.Invoke(false);

            else if (ButtonHeldCheckInput("FlipLeft"))
            {
                Time.timeScale = 0;
                lvlID.outputEvent.SetPressButtonText?.Invoke(true, 3, "Back Flip");
            }
        };

        if (useDash) controls.Movement.Dash.performed += ctx =>
        {

            if (!ButtonsEnabled) return;


            if (!trackingInputs)
            {
                if (canDash)
                {
                    ID.events.OnDash?.Invoke(true);
                    HapticFeedbackManager.instance.PlayerButtonPress();
                }
                else if (canDashSlash)
                {
                    if (cannotUseManaUntilLockedInputCheck)
                        return;
                    manaFull = false;
                    HandleDashSlashImage(false);
                    ID.events.OnDashSlash?.Invoke();
                }
                else
                {
                    HapticFeedbackManager.instance.PlayerButtonFailure();
                }
            }
            else if (CheckInputs("Dash"))
            {
                if (canDash && !startedHold)
                {
                    HapticFeedbackManager.instance.PlayerButtonPress();
                    ID.events.OnDash?.Invoke(true);
                }
                else if (!canDashSlash && startedHold)
                {
                    Time.timeScale = FrameRateManager.TargetTimeScale;
                    lvlID.outputEvent.SetPressButtonText?.Invoke(false, 3, "JUMP");
                    return;
                }

                else if (canDashSlash && !startedHold)
                {
                    manaFull = false;
                    HandleDashSlashImage(false);
                    ID.events.OnDashSlash?.Invoke();
                }
                if (!mustHold && !lockAfterInputCheck) trackingInputs = false;
                else
                {
                    startedHold = true;
                    StartCoroutine(MustHoldInput(mustHoldDuration));
                }
            }
        };

        if (useDash) controls.Movement.Dash.canceled += ctx =>
        {
            if (!mustHold)
            {
                if (!coolingDownDash && !canDash)
                {
                    ID.events.OnDash?.Invoke(false);
                }
            }


            else if (ButtonHeldCheckInput("Dash"))
            {
                Time.timeScale = 0;
                lvlID.outputEvent.SetPressButtonText?.Invoke(true, 3, "DASH");
            }

        };

        if (useDrop) controls.Movement.Drop.performed += ctx =>
        {
            if (!ButtonsEnabled) return;
            if (!trackingInputs)
            {
                if (canDrop)
                {
                    HapticFeedbackManager.instance.PlayerButtonPress();
                    ID.events.OnDrop?.Invoke();
                    StartCoroutine(DropCooldown());
                }
                else
                {
                    HapticFeedbackManager.instance.PlayerButtonFailure();
                }
            }
            else if (CheckInputs("Drop"))
            {
                if (canDrop)
                {
                    HapticFeedbackManager.instance.PlayerButtonPress();
                    ID.events.OnDrop?.Invoke();
                    StartCoroutine(DropCooldown());
                }
                if (!mustHold && !lockAfterInputCheck) trackingInputs = false;

            }
        };



        // controls.Movement.EggJoystick.performed += ctx =>
        // {
        //     // float xMoveAmount = ctx.ReadValue<Vector2>().x;
        //     // if (xMoveAmount < -.25f)
        //     //     ID.events.OnAimJoystick(1);
        //     // else if (xMoveAmount > .25f)
        //     //     ID.events.OnAimJoystick(-1);


        //     // ID.events.OnAimJoystick(ctx.ReadValue<Vector2>());
        // };
        // // controls.Movement.EggJoystick.canceled += ctx => ID.events.OnAimJoystick(Vector2.zero);
        // controls.Movement.EggJoystick.canceled += ctx => ID.events.OnAimJoystick(-2);
        controls.Movement.EggJoystick.performed += ctx =>
        {


            if (ID.canUseJoystick)
            {
                Vector2 moveAmount = ctx.ReadValue<Vector2>().normalized;
                ID.events.OnAimJoystick?.Invoke(moveAmount);
            }
        };

        controls.Movement.EggJoystick.canceled += ctx => ID.events.OnAimJoystick(Vector2.zero);



        controls.Movement.JumpHold.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnJumpHeld?.Invoke(true);
        };

        controls.Movement.JumpHold.canceled += ctx =>
        {
            if (!mustHold)
                ID.events.OnJumpHeld?.Invoke(false);

            else if (ButtonHeldCheckInput("Jump"))
            {
                Time.timeScale = 0;
                lvlID.outputEvent.SetPressButtonText?.Invoke(true, 3, "JUMP");
            }

        };

        // if (useEgg) controls.Movement.DropEgg.performed += ctx =>
        // {
        //     if (!ButtonsEnabled) return;
        //     if (!trackingInputs)
        //     {
        //         ID.events.OnEggDrop?.Invoke();
        //     }
        //     else if (CheckInputs("Egg"))
        //     {
        //         var player = GetComponent<PlayerStateManager>();
        //         player.EnterIdleStateWithVel(new Vector2(0, .5f));
        //         ID.events.OnEggDrop?.Invoke();

        //         if (!mustHold && !lockAfterInputCheck) trackingInputs = false;

        //     }
        // };


        //     controls.Movement.DropEgg.performed += ctx =>
        //    {
        //        if (ButtonsEnabled && !eggButtonHidden)
        //        {

        //            if (!trackingInputs)
        //            {
        //                if (!usingShotgun)
        //                {
        //                    ID.events.OnEggDrop?.Invoke();
        //                }

        //                else if (usingShotgun)
        //                    ID.events.OnAttack?.Invoke(true);

        //            }
        //            else if (!usingShotgun && CheckInputs("Egg"))
        //            {
        //                var player = GetComponent<PlayerStateManager>();
        //                player.EnterIdleStateWithVel(new Vector2(0, .5f));
        //                ID.events.OnEggDrop?.Invoke();

        //                if (!mustHold && !lockAfterInputCheck) trackingInputs = false;
        //            }
        //            else if (usingShotgun && CheckInputs("Shotgun"))
        //            {
        //                ID.events.OnAttack?.Invoke(true);
        //                if (!mustHold && !lockAfterInputCheck) trackingInputs = false;

        //            }
        //            // ID.globalEvents.OnEggButton?.Invoke(true);

        //        }
        //    };

        controls.Movement.DropEgg.performed += ctx =>
        {
            if (ButtonsEnabled && !eggButtonHidden )
            {
                if (!trackingInputs)
                    ID.events.OnPressAmmo?.Invoke(true);
                else if (currentEquipedAmmo == 0 && CheckInputs("Egg"))
                {
                    var player = GetComponent<PlayerStateManager>();
                    player.EnterIdleStateWithVel(new Vector2(0, .5f));
                    ID.events.OnPressAmmo?.Invoke(true);
                    if (!mustHold && !lockAfterInputCheck) trackingInputs = false;

                }
                else if (currentEquipedAmmo == 1 && CheckInputs("Shotgun"))
                {
                    ID.events.OnPressAmmo?.Invoke(true);
                    if (!mustHold && !lockAfterInputCheck) trackingInputs = false;
                }


            }
        };

        controls.Movement.DropEgg.canceled += ctx =>
       {
           ID.events.OnPressAmmo?.Invoke(false);
       };

        controls.Movement.SwitchAmmoRight.performed += ctx =>
     {
         //  if (!eggButtonHidden)
         //      ID.globalEvents.OnSwitchAmmo?.Invoke(false);
         if (ID.canPressEggButton)
             ID.UiEvents.OnSwitchWeapon?.Invoke(1,-1);

     };









    }

    private void HideEggButton(bool hidden)
    {
        eggButtonHidden = hidden;
    }


    private void FillAllMana()
    {
        fillAllMana = true;


    }

    // private void WaitForFinishToFillMana()
    // {
    //     currentMana = maxMana;
    //     ID.globalEvents.SetCanDashSlash?.Invoke(true);
    //     Debug.LogError("FDFDFDFDFDFD");


    //     HandleDashSlashImage(true);
    //     FullMana(true);


    // }


    void Start()
    {
        // normalButtonColor = ColorSO.normalButtonColor;
        // highlightButtonColor = ColorSO.highlightButtonColor;
        // disabledButtonColor = ColorSO.disabledButtonColor;
        // DashImageManaHighlight = ColorSO.DashImageManaHighlight;
        // DashImageManaDisabled = ColorSO.DashImageManaDisabled;
        // DashImageManaDisabledFilling = ColorSO.DashImageManaDisabledFilling;
        // coolDownColorRed = ColorSO.disabledButtonColorFull;
        // coolDownColorBlue = ColorSO.coolDownColorBlue;
        fillingManaColor = ColorSO.fillingManaColor;
        canUseDashSlashImageColor1 = ColorSO.canUseDashSlashImageColor1;
        canUseDashSlashImageColor2 = ColorSO.canUseDashSlashImageColor2;
        if (useFlips)
        {
            flipLeftImage = GameObject.Find("FlipLeftIMG").GetComponent<Image>();
            flipRightImage = GameObject.Find("FlipRightIMG").GetComponent<Image>();
        }
        if (useEgg)
        {
            eggImage = GameObject.Find("RingFill").GetComponent<Image>();
        }
        if (useDrop && GameObject.Find("DropButton") != null)
        {
            DropButton = GameObject.Find("DropButton").GetComponent<Image>();
            // DropButton.color = normalButtonColor;
            dropCooldownIN = GameObject.Find("DropCooldownIN").GetComponent<Image>();
            dropIcon = GameObject.Find("DropICON").GetComponent<RectTransform>();
            dropCooldownOUT = GameObject.Find("DropCooldownOUT").GetComponent<Image>();
            originalDropY = dropIcon.anchoredPosition.y;
            dropCooldownIN.color = ColorSO.disabledButtonColorFull;
            dropCooldownIN.DOFade(0, 0);
            dropCooldownOUT.DOFade(0, 0);
            canDrop = true;
            usingDrop = true;


        }
        else
        {
            usingDrop = false;
        }


        if (useDash && GameObject.Find("DashButton") != null)
        {
            dashImage = GameObject.Find("DashIMG").GetComponent<Image>();


            dashCooldownIN = GameObject.Find("DashCooldownIN").GetComponent<Image>();
            dashIcon = GameObject.Find("DashICON").GetComponent<RectTransform>();
            dashCooldownGroup = GameObject.Find("DashCooldownGroup").GetComponent<CanvasGroup>();

            startingDashIconX = dashIcon.anchoredPosition.x;
            dashCooldownIN.color = ColorSO.disabledButtonColorFull;

            dashCooldownGroup.alpha = 0;

            canDashSlash = false;
            coolingDownDash = false;








            usingDash = true;
            dashImageMana = GameObject.Find("DashIMGMana").GetComponent<Image>();
            if (manaUsed)
            {

                dashImageMana.enabled = true;

                if (fillAllMana)
                {
                    currentMana = maxMana;

                    dashImageMana.fillAmount = (1);
                    HandleDashSlashImage(true);



                }
                else
                {
                    dashImageMana.color = fillingManaColor;

                    dashImageMana.fillAmount = (currentMana / maxMana);
                    mainFillCourintine = StartCoroutine(RegenerateMana());
                }




            }
            else
                dashImageMana.gameObject.SetActive(false);

            FullMana(fillAllMana);


        }
        else
            usingDash = false;



    }

    public bool dashSlashReady()
    {
        if (!coolingDownDash && manaFull)
            return true;
        else
            return false;
    }


    private IEnumerator MustHoldInput(float duration)
    {
        yield return new WaitForSeconds(duration);
        mustHold = false;
        startedHold = false;
        trackingInputs = false;
    }


    private void SetUsingShotgun(int type)
    {
        if (type == 1)
            usingShotgun = true;
        else
            usingShotgun = false;

    }




    private IEnumerator DropCooldown()
    {
        coolingDownDrop = true;
        canDrop = false;
        // dashButtonIMG.DOColor(DisabledButtonColor, .25f);
        DropButton.DOColor(ColorSO.highlightButtonColor, .15f);
        dropIcon.DOScale(largeIconScale, .3f).SetEase(Ease.OutSine);
        dropIcon.DOAnchorPosY(originalDropY - 30, .2f).SetEase(Ease.OutSine);
        dropCooldownIN.DOFade(.7f, .15f);
        dropCooldownOUT.DOFade(1, .15f);

        dropCooldownIN.DOFillAmount(0, 2.25f).From(1);
        yield return new WaitForSeconds(.2f);
        DropButton.DOColor(ColorSO.disabledButtonColor, .2f);
        dropIcon.DOScale(smallIconScale, .15f).SetEase(Ease.InSine);
        dropIcon.DOAnchorPosY(originalDropY, .3f).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(.15f);
        dropIcon.DOScale(smallIconScale2, .25f).SetEase(Ease.InOutSine);



        yield return new WaitForSeconds(1.67f);
        dropCooldownIN.DOFade(0, 0);
        dropCooldownOUT.DOFade(0, .1f);

        // dashButtonIMG.DOColor(ButtonNormalColor, .15f);
        DropButton.DOColor(ColorSO.normalButtonColor, .13f);
        dropIcon.DOScale(normalIconScale, .13f);


        yield return new WaitForSeconds(.1f);

        coolingDownDrop = false;
        canDrop = true;

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
                dashImage.color = ColorSO.highlightButtonColor;
                StartCoroutine(CalcualteDashTimeLeft());
                IconTween(1);

            }

        }


        if (!coolingDownDash && !isDashing && !canDash)
        {
            StartCoroutine(DashCooldown());


        }



    }




    private void FullMana(bool isFull)
    {
        manaFull = isFull;

        if (isFull)
        {
            ID.globalEvents.SetCanDashSlash?.Invoke(true);

            dashCooldownIN.color = ColorSO.coolDownColorBlue;

        }
        else
        {
            ID.globalEvents.SetCanDashSlash?.Invoke(false);
            dashCooldownIN.color = ColorSO.disabledButtonColorFull;


        }

    }
    private void ExitDash(bool notExited)
    {
        if (!notExited)
        {
            canDashSlash = false;
            if (manaFull)
            {
                dashCooldownIN.color = ColorSO.disabledButtonColorFull;
                dashImageMana.DOColor(ColorSO.DashImageManaDisabled, .1f);

            }
            IconTween(2);

        }
        else
        {
            if (manaFull)
            {
                canDashSlash = true;
                dashCooldownIN.color = ColorSO.coolDownColorBlue;
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
        StopAllCoroutines();



        if (useDash)

        {
            ID.events.OnDash?.Invoke(false);

            dashCooldownGroup.alpha = 0;
            if (dashIconSequence != null && dashIconSequence.IsPlaying())
                dashIconSequence.Kill();

            dashIcon.DOAnchorPosX(startingDashIconX, .2f).SetEase(Ease.OutSine).SetUpdate(true);
            dashIcon.DOScale(dashIconNormalScale, .2f).SetUpdate(true);
            // dashButtonIMG.DOColor(ButtonNormalColor, .15f);
            dashImage.DOColor(ColorSO.normalButtonColor, .15f).SetUpdate(true).OnComplete(FlashDashToPress);

            canDash = true;

            coolingDownDash = false;
            if (manaFull)
            {
                canDashSlash = true;
                dashCooldownIN.color = ColorSO.coolDownColorBlue;
                if (flashSequence != null && flashSequence.IsPlaying())
                    flashSequence.Kill();

                flashSequence = DOTween.Sequence();
                flashSequence.Append(dashImageMana.DOColor(canUseDashSlashImageColor1, flashDuration).SetEase(Ease.OutSine).From(canUseDashSlashImageColor2));
                flashSequence.Append(dashImageMana.DOColor(canUseDashSlashImageColor2, flashDuration).SetEase(Ease.OutSine));

                flashSequence.Play().SetLoops(-1).SetUpdate(true);


            }
            else if (manaUsed)
            {
                dashImageMana.color = fillingManaColor;

                StartCoroutine(RegenerateMana());

            }
        }

        if (useDrop)
        {
            dropCooldownIN.DOFade(0, .13f).SetUpdate(true);
            dropCooldownOUT.DOFade(0, .13f).SetUpdate(true);

            // dashButtonIMG.DOColor(ButtonNormalColor, .15f);
            DropButton.DOColor(ColorSO.normalButtonColor, .13f).SetUpdate(true).OnComplete(FlashDropToPress);
            dropIcon.DOScale(normalIconScale, .13f).SetUpdate(true);
            coolingDownDrop = false;
            canDrop = true;
        }

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
            dashImageMana.DOColor(ColorSO.DashImageManaHighlight, .1f);

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
        dashImage.DOColor(ColorSO.disabledButtonColor, .25f);

        dashCooldownGroup.DOFade(1, .15f);

        dashCooldownIN.DOFillAmount(0, 1.3f).From(1);
        yield return new WaitForSeconds(1.1f);


        dashCooldownGroup.DOFade(0, .15f);

        // dashButtonIMG.DOColor(ButtonNormalColor, .15f);
        dashImage.DOColor(ColorSO.normalButtonColor, .15f);
        IconTween(0);

        canDash = true;

        coolingDownDash = false;
        ExitDash(true);

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

    public void UpdateCooldownColor()
    {
        FinishCooldowns();



        dashCooldownIN.color = ColorSO.disabledButtonColorFull;
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

    public IEnumerator InvokedActivateButtons(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        ButtonsEnabled = true;


    }

    private void FlashButtonCheckBeforeTween(string[] inpVar)
    {
        currentFlashImages = new List<Image>();
        if (inpVar.Length == 1)
        {
            string inp = inpVar[0];

            switch (inp)
            {
                case ("FlipRight"):

                    currentFlashImages.Add(flipRightImage);

                    break;
                case ("Dash"):
                    currentFlashImages.Add(dashImage);


                    break;
                case ("Drop"):
                    currentFlashImages.Add(DropButton);


                    break;
                case ("Egg"):
                    currentFlashImages.Add(eggImage);
                    break;
            }
        }

        else if (inpVar.Length == 2 && (inpVar[0] == "FlipLeft" || inpVar[0] == "FlipRight"))
        {
            currentFlashImages.Add(flipLeftImage);
            currentFlashImages.Add(flipRightImage);

        }


        FlashButtonTween(currentFlashImages);

    }
    private void FlashButtonTween(List<Image> imgVar)
    {
        if (flashButtonSeq != null && flashButtonSeq.IsPlaying())
            flashButtonSeq.Kill();

        originalButtonColor = imgVar[0].color;
        flashButtonSeq = DOTween.Sequence();

        foreach (var i in imgVar)
        {
            i.color = ColorSO.flashButtonColor2;
        }

        if (imgVar.Count == 1)
        {
            Image img = imgVar[0];


            flashButtonSeq.Append(img.DOColor(ColorSO.flashButtonColor1, .8f).SetEase(Ease.OutSine).From(ColorSO.flashButtonColor2));
            flashButtonSeq.AppendInterval(.5f);
            flashButtonSeq.Append(img.DOColor(ColorSO.flashButtonColor2, .6f));
            flashButtonSeq.Play().SetLoops(-1).SetUpdate(true);
        }
        else if (imgVar.Count == 2)
        {


            flashButtonSeq.Append(imgVar[0].DOColor(ColorSO.flashButtonColor1, .3f));
            flashButtonSeq.Join(imgVar[1].DOColor(ColorSO.flashButtonColor1, .3f));
            flashButtonSeq.Append(imgVar[0].DOColor(ColorSO.flashButtonColor2, .3f));
            flashButtonSeq.Append(imgVar[1].DOColor(ColorSO.flashButtonColor2, .3f));
            flashButtonSeq.Play().SetLoops(-1).SetUpdate(true);




        }



    }



    private void CancelFlashButtonTween()
    {
        if (flashButtonSeq != null && flashButtonSeq.IsPlaying())
        {
            flashButtonSeq.Kill();
            foreach (var img in currentFlashImages)
            {
                img.color = originalButtonColor;
            }
        }

    }

    private void FlashDashToPress()
    {
        if (flashDashBool)
        {
            FlashButtonCheckBeforeTween(inputsTracked.ToArray());
        }


    }
    private void FlashDropToPress()
    {
        if (flashDropBool)
        {
            FlashButtonCheckBeforeTween(inputsTracked.ToArray());
        }


    }

    public int ReturnCurrentAmmo()
    {
        return currentEquipedAmmo;

    }




    private void SetInputs(bool checkAny, string[] inp, float mustHoldDurationP, float delayForInputs, bool flashImage, bool lockInputsAfterCheck)
    {
        bool alreadyStartedFlash = false;
        trackingInputs = true;

        if (flashImage)
        {
            if (inp[0] == "Dash")
            {
                flashDashBool = true;
                flashDropBool = false;
                alreadyStartedFlash = true;
            }
            else if (inp[0] == "Drop")
            {
                flashDashBool = false;
                flashDropBool = true;
                alreadyStartedFlash = true;

            }
            else
            {
                flashDashBool = false;
                flashDropBool = false;
                alreadyStartedFlash = false;


            }
        }
        else
        {
            flashDashBool = false;
            flashDropBool = false;
            alreadyStartedFlash = false;

        }

        lockAfterInputCheck = lockInputsAfterCheck;


        if (!lockAfterInputCheck && doCooldownsBool)
        {
            FinishCooldowns();


        }
        else if (!doCooldownsBool)
        {
            doCooldownsBool = true;
            cannotUseManaUntilLockedInputCheck = false;
        }

        if (delayForInputs > 0)
        {
            ActivateButtons(false);
            StartCoroutine(InvokedActivateButtons(delayForInputs));
        }
        else
        {
            ActivateButtons(true);

        }


        if (checkAny)
        {
            anyInput = true;
            return;
        }

        anyInput = false;
        inputsTracked = new List<string>();

        foreach (var s in inp)
        {
            inputsTracked.Add(s);
        }


        if (flashImage && !alreadyStartedFlash)
        {
            FlashButtonCheckBeforeTween(inputsTracked.ToArray());
        }

        if (mustHoldDurationP <= 0)
        {
            mustHold = false;
            mustHoldDuration = 0;


        }
        else
        {
            mustHold = true;


            mustHoldDuration = mustHoldDurationP;

        }

    }

    private bool ButtonHeldCheckInput(string s)
    {
        if (anyInput)
        {

            return true;
        }

        else
        {
            foreach (var sVar in inputsTracked)
            {
                if (sVar == s)
                {

                    return true;
                }
            }
        }

        return false;

    }

    private bool CheckInputs(string s)
    {

        if (anyInput)
        {
            ID.globalEvents.ExitSectionTrigger?.Invoke();
            return true;
        }

        else
        {
            foreach (var sVar in inputsTracked)
            {
                if (sVar == s)
                {
                    CancelFlashButtonTween();
                    ID.globalEvents.ExitSectionTrigger?.Invoke();
                    if (lockAfterInputCheck)
                    {
                        inputsTracked = null;
                        ActivateButtons(false);
                        doCooldownsBool = false;
                    }
                    return true;
                }
            }
        }

        return false;

    }

    private void SetNewEquipedAmmo(int nextAmmoType, int amountOfAmmo, int direction)
    {
        currentEquipedAmmo = nextAmmoType;

    }

    private void OnEnable()
    {
        controls.Movement.Enable();
        isInitialized = true;
        ID.events.EnableButtons += ActivateButtons;
        ID.events.OnDash += HandleDashNew;
        ID.globalEvents.OnSetInputs += SetInputs;
        ID.globalEvents.FillPlayerMana += FillAllMana;


        ID.globalEvents.CanDashSlash += ExitDash;

        ID.globalEvents.OnHideEggButton += HideEggButton;
        ID.events.OnSwitchAmmoType += SetUsingShotgun;
        ID.UiEvents.OnSwitchDisplayedWeapon += SetNewEquipedAmmo;
        if (manaUsed)
            ID.globalEvents.OnGetMana += GatherMana;
    }



    private void OnDisable()
    {
        isInitialized = false;
        CancelFlashButtonTween();
        controls.Movement.Disable();
        ID.events.EnableButtons -= ActivateButtons;
        ID.globalEvents.OnSetInputs -= SetInputs;
        ID.globalEvents.FillPlayerMana -= FillAllMana;
        ID.globalEvents.OnHideEggButton -= HideEggButton;
        ID.events.OnSwitchAmmoType -= SetUsingShotgun;
        ID.UiEvents.OnSwitchDisplayedWeapon -= SetNewEquipedAmmo;



        ID.events.OnDash -= HandleDashNew;
        ID.globalEvents.CanDashSlash -= ExitDash;
        StopAllCoroutines();
        DOTween.Kill(this);
        DOTween.Kill(flashSequence);
        DOTween.Kill(dashIconSequence);
        DOTween.Kill(dropButtonSeq);



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


