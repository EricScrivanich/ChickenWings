using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    public PlayerID player;
    private float maxStamina;
    private bool isUsingStamina;
    [SerializeField] private float staminaBarFadeTime;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI ammoText;
    public Material staminaBarMaterial; // Reference to the stamina bar's material
    public CanvasGroup StaminaGroup;

    public Image StaminaBG; // Reference to the UI element you want to flash
    private bool canFlashStaminaBG = true;
    [SerializeField] private Color BGFlashColor; // The color to flash to
    [SerializeField] private int numverOfBGFlashes; // Total number of flashes
    [SerializeField] private float totalStaminaBGFlashDuration; //
    private Color originalStaminaBGColor;


    private StatsManager statsMan;
    [SerializeField] private Color lightRed;

    void Start()
    {
        statsMan = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();
        maxStamina = player.MaxStamina;
        originalStaminaBGColor = StaminaBG.color;





        // Find the StatsPanel
        Transform statsPanel = transform.Find("StatsPanel");
        if (statsPanel != null)
        {
            scoreText = statsPanel.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            ammoText = statsPanel.Find("AmmoText")?.GetComponent<TextMeshProUGUI>();
        }

        // Subscribe to events

    }

    void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();

        }


    }

    void UpdateAmmo()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + player.Ammo.ToString();
        }
    }

    void Update()
    {
        if (isUsingStamina)
        {
            // float staminaPercentage = player.CurrentStamina / player.MaxStamina;
            staminaBarMaterial.SetFloat("_OffsetUvX", player.StaminaUsed / maxStamina);
            Color staminaColor = Color.Lerp(Color.white, lightRed, player.StaminaUsed / maxStamina);
            staminaBarMaterial.SetColor("_Color", staminaColor);
            // Debug.Log(player.StaminaUsed * .01f);
        }


    }
    private IEnumerator FlashBG()
    {
        canFlashStaminaBG = false;
        float flashDuration = totalStaminaBGFlashDuration / (numverOfBGFlashes * 2); // Calculate the duration of each flash

        for (int i = 0; i < numverOfBGFlashes; i++)
        {
            // Change to the flash color
            StaminaBG.color = BGFlashColor
    ;
            yield return new WaitForSeconds(flashDuration);

            // Revert to the original color
            StaminaBG.color = originalStaminaBGColor;
            yield return new WaitForSeconds(flashDuration);
        }
        canFlashStaminaBG = true;
    }

    private void FlashStamimaBG()
    {
        if (canFlashStaminaBG)
        {
            StartCoroutine(FlashBG());
        }
        else
        {
            return;
        }

    }


    private IEnumerator FadeStaminaBar(bool beingUsed) 
    {
        float time = 0;

        float startAlpha = beingUsed ? 0 : .9f; // Starting alpha value
        float endAlpha = beingUsed ? .9f : 0; // Ending alpha value
        


        while (time < staminaBarFadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / staminaBarFadeTime);
            // staminaBarMaterial.SetFloat("_Alpha", alpha);
            StaminaGroup.alpha = alpha;
            yield return null;
        }

        // Ensure the final alpha is set correctly
        StaminaGroup.alpha = endAlpha;
    }

    private void HandleStaminaBar(bool usingStamina)
    {
        if (isUsingStamina == usingStamina)
        {
            return;
        }

        isUsingStamina = usingStamina;
        StartCoroutine(FadeStaminaBar(usingStamina));
        // Assuming you have a way to get the current stamina value from PlayerID

    }
    private void OnEnable()
    {
        StatsManager.OnScoreChanged += UpdateScore;
        player.globalEvents.OnUpdateAmmo += UpdateAmmo;
        player.globalEvents.OnUseStamina += HandleStaminaBar;
        player.globalEvents.OnZeroStamina += FlashStamimaBG;

        StaminaGroup.alpha = 1;


    }
    private void OnDisable()
    {
        StatsManager.OnScoreChanged -= UpdateScore;
        player.globalEvents.OnUpdateAmmo -= UpdateAmmo;
        player.globalEvents.OnUseStamina -= HandleStaminaBar;
        player.globalEvents.OnZeroStamina -= FlashStamimaBG;
        staminaBarMaterial.SetColor("_Color", Color.white);

        staminaBarMaterial.SetFloat("_OffsetUvX", 0);


    }


}


