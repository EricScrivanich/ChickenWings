using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class StateInputSystem : MonoBehaviour
{
    private InputController controls;
    private Sequence dropButtonSeq;
    private Coroutine dropButtonCor;
    private Sequence dashButtonSeq;
    private bool coolingDownDrop;

    [Header("Global Colors")]
    [SerializeField] private Color normalButtonColor;
    [SerializeField] private Color highlightButtonColor;
    [SerializeField] private Color disabledButtonColor;
    [SerializeField] private Color DashImageManaHighlight;
    [SerializeField] private Color DashImageManaDisabled;
    [SerializeField] private Color DashImageManaDisabledFilling;
    [SerializeField] private Color coolDownColorRed;
    [SerializeField] private Color coolDownColorBlue;

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

    [SerializeField] private RectTransform touchButtonRectTransform;

    private void Awake()
    {
        ID.UsingClocker = false;



        controls = new InputController();

        // Bind existing actions to methods


        controls.Movement.Jump.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnJump?.Invoke();
        };

        controls.Movement.JumpRight.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnFlipRight?.Invoke(true);
        };
        controls.Movement.JumpRight.canceled += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnFlipRight?.Invoke(false);
        };

        controls.Movement.JumpLeft.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnFlipLeft?.Invoke(true);
        };
        controls.Movement.JumpLeft.canceled += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnFlipLeft?.Invoke(false);
        };

        controls.Movement.Dash.performed += ctx =>
        {
            if (ButtonsEnabled)
            {
                if (canDash)
                {
                    ID.events.OnDash?.Invoke(true);

                }


                else if (canDashSlash)
                {
                    manaFull = false;

                    HandleDashSlashImage(false);
                    ID.events.OnDashSlash?.Invoke();
                }



            }
        };
        controls.Movement.Dash.canceled += ctx =>
        {
            if (ButtonsEnabled)
            {
                if (!coolingDownDash && !canDash)
                {

                    ID.events.OnDash?.Invoke(false);


                }

            }
        };



        controls.Movement.Drop.performed += ctx =>
        {
            if (ButtonsEnabled && canDrop)
            {
                ID.events.OnDrop?.Invoke();
                StartCoroutine(DropCooldown());

            }
        };
        controls.Movement.DropEgg.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnEggDrop?.Invoke();
        };
        controls.Movement.Fireball.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnAttack?.Invoke(true);
        };
        controls.Movement.Fireball.canceled += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnAttack?.Invoke(false);
        };

        controls.Movement.JumpHold.performed += ctx =>
        {
            if (ButtonsEnabled) ID.events.OnJumpHeld?.Invoke(true);
        };

        controls.Movement.JumpHold.canceled += ctx =>
        {
            ID.events.OnJumpHeld?.Invoke(false);

        };

        controls.Movement.Fireball.performed += ctx => ID.events.OnAttack?.Invoke(!ID.UsingClocker);
        controls.Movement.Parachute.performed += ctx => ID.events.OnParachute?.Invoke(true);
        controls.Movement.Parachute.canceled += ctx => ID.events.OnParachute?.Invoke(false);



    }

    void Start()
    {
        if (GameObject.Find("DropButton") != null)
        {
            DropButton = GameObject.Find("DropButton").GetComponent<Image>();
            DropButton.color = normalButtonColor;
            dropCooldownIN = GameObject.Find("DropCooldownIN").GetComponent<Image>();
            dropIcon = GameObject.Find("DropICON").GetComponent<RectTransform>();
            dropCooldownOUT = GameObject.Find("DropCooldownOUT").GetComponent<Image>();
            originalDropY = dropIcon.anchoredPosition.y;
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
        DropButton.DOColor(highlightButtonColor, .15f);
        dropIcon.DOScale(largeIconScale, .3f).SetEase(Ease.OutSine);
        dropIcon.DOAnchorPosY(originalDropY - 30, .2f).SetEase(Ease.OutSine);
        dropCooldownIN.DOFade(.7f, .15f);
        dropCooldownOUT.DOFade(1, .15f);

        dropCooldownIN.DOFillAmount(0, 2.25f).From(1);
        yield return new WaitForSeconds(.2f);
        DropButton.DOColor(disabledButtonColor, .2f);
        dropIcon.DOScale(smallIconScale, .15f).SetEase(Ease.InSine);
        dropIcon.DOAnchorPosY(originalDropY, .3f).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(.15f);
        dropIcon.DOScale(smallIconScale2, .25f).SetEase(Ease.InOutSine);



        yield return new WaitForSeconds(1.75f);
        dropCooldownIN.DOFade(0, 0);
        dropCooldownOUT.DOFade(0, .1f);

        // dashButtonIMG.DOColor(ButtonNormalColor, .15f);
        DropButton.DOColor(normalButtonColor, .13f);
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
                dashImage.color = highlightButtonColor;
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

            dashCooldownIN.color = coolDownColorBlue;

        }
        else
        {
            ID.globalEvents.SetCanDashSlash?.Invoke(false);
            dashCooldownIN.color = coolDownColorRed;
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
                dashCooldownIN.color = coolDownColorRed;
                dashImageMana.DOColor(DashImageManaDisabled, .1f);

            }
            IconTween(2);

        }
        else
        {
            if (manaFull)
            {
                canDashSlash = true;
                dashCooldownIN.color = coolDownColorBlue;
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
        dashTimeLeft = .35f;
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
        dashImage.DOColor(normalButtonColor, .15f);

        canDash = true;

        coolingDownDash = false;
        if (manaFull)
        {
            canDashSlash = true;
            dashCooldownIN.color = coolDownColorBlue;
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
        DropButton.DOColor(normalButtonColor, .13f).SetUpdate(true);
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
        Debug.Log("COOOOLING");

        if (manaFull)
        {
            canDashSlash = true;
            flashSequence.Kill();
            dashImageMana.DOColor(DashImageManaHighlight, .1f);

        }
        else
        {
            Debug.Log("Waiting for: " + dashTimeLeft + " - seconds");
            yield return new WaitForSeconds(dashTimeLeft);

            IconTween(2);
        }
        // else
        // {
        //     dashImageMana.color = DashImageManaDisabledFilling;
        // }
        // dashButtonIMG.DOColor(DisabledButtonColor, .25f);
        dashImage.DOColor(disabledButtonColor, .25f);

        dashCooldownGroup.DOFade(1, .15f);

        dashCooldownIN.DOFillAmount(0, 1.3f).From(1);
        yield return new WaitForSeconds(1.1f);
        Debug.Log("Finsihed cooling bug");

        dashCooldownGroup.DOFade(0, .15f);

        // dashButtonIMG.DOColor(ButtonNormalColor, .15f);
        dashImage.DOColor(normalButtonColor, .15f);
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

    private void OnEnable()
    {
        controls.Movement.Enable();
        ID.events.EnableButtons += ActivateButtons;
        ID.events.OnDash += HandleDashNew;


        ID.globalEvents.CanDashSlash += ExitDash;
        if (manaUsed)
            ID.globalEvents.OnGetMana += GatherMana;
    }

    private void OnDisable()
    {
        controls.Movement.Disable();
        ID.events.EnableButtons -= ActivateButtons;
        ID.events.OnDash -= HandleDashNew;
        ID.globalEvents.CanDashSlash -= ExitDash;



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


