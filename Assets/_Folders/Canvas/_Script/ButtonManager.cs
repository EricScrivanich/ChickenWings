using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    [Header("Dash")]
    public PlayerID player;
    [SerializeField] private bool DashSlashOn;

    [SerializeField] private Button dashButton;
    [SerializeField] private Image dashImage;
    [SerializeField] private Image dashImageMana;
    [SerializeField] private float dashSlashCooldown;
    [SerializeField] private Color PressedColorNormal;
    [SerializeField] private Color PressedColorMana;
    [SerializeField] private Color fillingManaColor;
    [SerializeField] private Color canUseDashSlashButtonColor;
    [SerializeField] private Color canUseDashSlashImageColor;
    [SerializeField] private Color orignialButtonColor;

    

    [Header("Drop")]
    [SerializeField] private Button dropButton;
    [SerializeField] private Image dropImage;
    private Color dropColor;
    [SerializeField] private Color canDropColor = new Color(1f, 1f, 1f, 0.6f);
    [SerializeField] private Color canNotDropColor = new Color(1f, 1f, 1f, 0.45f);
    private bool canDashSlash;


    public Image leftFlipArrow; // Assign this in the Unity Inspector
    private Tween currentTween; // To keep track of the current tween

    // Call this method to start rotating with potential for speeding up

    private void Awake()
    {



    }
    private void Start()
    {

        canDashSlash = false;
        if (!player.isTutorial)
        {
            dashImageMana.enabled = true;

            player.globalEvents.SetCanDashSlash?.Invoke(false);

        }
        else
        {
            dashImageMana.enabled = false;
        }
    }

    void HandleDash(bool canDash)
    {
        if (canDashSlash && !canDash)
        {
            return;
        }
        dashButton.interactable = canDash;

    }
    void HandleDashSlashButton(bool slashPressable)
    {
        Debug.Log(slashPressable);
        if (slashPressable)
        {
            ColorBlock colors = dashButton.colors;
            colors.normalColor = canUseDashSlashButtonColor;
            colors.selectedColor = canUseDashSlashButtonColor;
            colors.pressedColor = PressedColorMana;
            dashButton.colors = colors;
        }
        else
        {
            ColorBlock colors = dashButton.colors;
            colors.normalColor = orignialButtonColor;
            colors.selectedColor = orignialButtonColor;

            colors.pressedColor = PressedColorNormal;


            dashButton.colors = colors;
            dashButton.interactable = false;

        }



    }
    void HandleDashSlashImage(bool canSlash)
    {
        canDashSlash = canSlash;
        if (!canSlash)
        {
            dashImageMana.color = fillingManaColor;

            StartCoroutine(FillImage(dashSlashCooldown));

        }
        else
        {
            dashImageMana.color = canUseDashSlashImageColor;
        }

    }

    void HandleDrop(bool canDrop)
    {
        // dropButton.interactable = canDrop;
        // dropImage.color = canDrop ? canDropColor : canNotDropColor;
    }


    private void OnEnable()
    {
        player.globalEvents.CanDrop += HandleDrop;
        player.globalEvents.CanDash += HandleDash;
        if (DashSlashOn)
        {
            
            player.globalEvents.SetCanDashSlash += HandleDashSlashImage;
            player.globalEvents.CanDashSlash += HandleDashSlashButton;

        }





    }

    private void OnDisable()
    {
        player.globalEvents.CanDrop -= HandleDrop;
        player.globalEvents.CanDash -= HandleDash;
        if (DashSlashOn)
        {
        

            player.globalEvents.SetCanDashSlash -= HandleDashSlashImage;
            player.globalEvents.CanDashSlash -= HandleDashSlashButton;
        }


    }

    public IEnumerator FillImage(float duration)
    {
        float elapsedTime = 0f;
        float currentFill = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the current fill based on the elapsed time
            currentFill = Mathf.Lerp(0f, 0.9f, elapsedTime / duration);
            dashImageMana.fillAmount = currentFill;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure the fill amount is set to 0.9 at the end to avoid any precision issues
        dashImageMana.fillAmount = 0.9f;

        player.globalEvents.SetCanDashSlash?.Invoke(true);
    }


}