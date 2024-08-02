using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.Collections;

public class ManaHandler : MonoBehaviour
{
    [Header("Dash")]
    public PlayerID player;
    [SerializeField] private bool manaUsed;

    private Sequence flashSequence;
    private bool coolingDownDash;

    private bool canDash = true;

    [SerializeField] private Color DisabledButtonColor;
    [SerializeField] private Color ButtonHeldColor;
    [SerializeField] private Color ButtonNormalColor;

    [SerializeField] private Color DashImageManaHighlight;
    [SerializeField] private Color DashImageManaDisabled;
    [SerializeField] private Color DashImageManaDisabledFilling;
    [SerializeField] private Color coolDownColorRed;
    [SerializeField] private Color coolDownColorBlue;

    [Header("Dash Icon")]
    private Sequence dashIconSequence;
    private float startingDashIconX;
    [SerializeField] private float moveDashIconX;
    private Vector3 dashIconNormalScale = new Vector3(1, 1, 1);
    private Vector3 dashIconLargeScale = new Vector3(1.3f, 1.25f, 1.25f);
    private Vector3 dashIconSmallScale = new Vector3(.85f, .9f, .9f);





    private Coroutine mainFillCourintine;
    private bool manaFull;
    [SerializeField] private float mainManaFillDuration;
    private int manaGainedFromCollectable = 40;
    private float flashDuration = .7f;


    private Image dashImage;
    private Image dashImageMana;
    private Image dashCooldownIN;
    private CanvasGroup dashCooldownGroup;


    [SerializeField] private Color fillingManaColor;
    [SerializeField] private Color canUseDashSlashImageColor1;
    [SerializeField] private Color canUseDashSlashImageColor2;
    private RectTransform dashIcon;

    private float dashTimeLeft;
    private bool canDashSlash;

    [Header("Mana Settings")]
    private float maxMana = 100f;
    private float currentMana = 0f; // Mana to regenerate per second




    private void Start()
    {
        coolingDownDash = false;
        dashImage = GameObject.Find("DashIMG").GetComponent<Image>();
        dashImageMana = GameObject.Find("DashIMGMana").GetComponent<Image>();


        dashCooldownIN = GameObject.Find("DashCooldownIN").GetComponent<Image>();
        dashIcon = GameObject.Find("DashICON").GetComponent<RectTransform>();
        dashCooldownGroup = GameObject.Find("DashCooldownGroup").GetComponent<CanvasGroup>();

        startingDashIconX = dashIcon.anchoredPosition.x;


        dashCooldownGroup.alpha = 0;
        dashImageMana.color = fillingManaColor;
        canDashSlash = false;




        dashImageMana.enabled = true;
        player.globalEvents.SetCanDashSlash?.Invoke(false);
        dashImageMana.fillAmount = (currentMana / maxMana);
        FullMana(false);

        if (manaUsed)
            mainFillCourintine = StartCoroutine(RegenerateMana());
        else
            dashImageMana.gameObject.SetActive(false);

        // player.globalEvents.SetCanDashSlash += HandleDashSlashImage;
        // player.globalEvents.CanDashSlash += HandleDashSlashButton;




        // Start mana regeneration coroutine

    }

    void MinimumDashTime(float time)
    {
        // dashTimeLeft = time;
        // Debug.Log("Set new time of: " + dashTimeLeft);
    }



    private void IconTween(int type)
    {
        if (dashIconSequence != null && dashIconSequence.IsPlaying())
            dashIconSequence.Kill();

        if (type == 0)
        {
            dashIconSequence.Append(dashIcon.DOAnchorPosX(startingDashIconX, .2f).SetEase(Ease.OutSine));
            dashIconSequence.Join(dashIcon.DOScale(dashIconNormalScale, .1f));
            dashIconSequence.Play();

        }
        else if (type == 1)
        {
            dashIconSequence.Append(dashIcon.DOAnchorPosX(startingDashIconX + moveDashIconX, .35f).SetEase(Ease.OutSine));

            dashIconSequence.Join(dashIcon.DOScale(dashIconLargeScale, .2f));
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
        if (isDashing && canDash)
        {
            canDash = false;
            // dashButtonIMG.color = ButtonHeldColor;
            dashImage.color = ButtonHeldColor;
            StartCoroutine(CalcualteDashTimeLeft());
            IconTween(1);

        }
        else if (canDashSlash && isDashing)
        {
            manaFull = false;

            HandleDashSlashImage(false);
            player.events.OnDashSlash?.Invoke();

        }
        else
        {

            if (!coolingDownDash)
            {
                StartCoroutine(DashCooldown());

            }

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
            dashCooldownIN.color = coolDownColorBlue;

        }
        else
            dashCooldownIN.color = coolDownColorRed;

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
                FlashDashImage(1);

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



    private IEnumerator DashCooldown()
    {
        coolingDownDash = true;
        canDash = false;

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
        dashImage.DOColor(DisabledButtonColor, .25f);

        dashCooldownGroup.DOFade(1, .15f);

        dashCooldownIN.DOFillAmount(0, 1.2f);
        yield return new WaitForSeconds(1.1f);

        dashCooldownGroup.DOFade(0, .15f);

        // dashButtonIMG.DOColor(ButtonNormalColor, .15f);
        dashImage.DOColor(ButtonNormalColor, .15f);
        IconTween(0);


        yield return new WaitForSeconds(.16f);
        dashCooldownIN.fillAmount = 1;

        coolingDownDash = false;
        canDash = true;
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

    private void OnEnable()
    {
        // player.globalEvents.CanDash += HandleDash;
        player.events.OnDash += HandleDashNew;
        player.globalEvents.timeLeftOfMinDash += MinimumDashTime;

        player.globalEvents.CanDashSlash += ExitDash;
        if (manaUsed)
            player.globalEvents.OnGetMana += GatherMana;

    }

    private void OnDisable()
    {
        // player.globalEvents.CanDash -= HandleDash;
        player.globalEvents.timeLeftOfMinDash -= MinimumDashTime;

        player.events.OnDash -= HandleDashNew;
        player.globalEvents.CanDashSlash -= ExitDash;



        // player.globalEvents.SetCanDashSlash -= HandleDashSlashImage;
        // player.globalEvents.CanDashSlash -= HandleDashSlashButton;
        if (manaUsed)
            player.globalEvents.OnGetMana -= GatherMana;


    }


}