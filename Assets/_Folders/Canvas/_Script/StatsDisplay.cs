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

    private StatsManager statsMan;
    [SerializeField] private Color lightRed;

    void Start()
    {
        statsMan = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();
        maxStamina = player.MaxStamina;





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
            scoreText.text = "Score: " + score.ToString();
    }

    void UpdateAmmo(int ammo)
    {
        if (ammoText != null)
            ammoText.text = "Ammo: " + ammo.ToString();
    }

    void Update()
    {
        if (isUsingStamina)
        {
            // float staminaPercentage = player.CurrentStamina / player.MaxStamina;
            staminaBarMaterial.SetFloat("_OffsetUvX", player.StaminaUsed/maxStamina);
            Color staminaColor = Color.Lerp(Color.white, lightRed, player.StaminaUsed / maxStamina);
            staminaBarMaterial.SetColor("_Color", staminaColor);
            // Debug.Log(player.StaminaUsed * .01f);
        }


    }

    private IEnumerator FadeStaminaBar(bool beingUsed)
    {
        float time = 0;

        float startAlpha = beingUsed ? 0 : 1; // Starting alpha value
        float endAlpha = beingUsed ? 1 : 0; // Ending alpha value

        while (time < staminaBarFadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / staminaBarFadeTime);
            staminaBarMaterial.SetFloat("_Alpha", alpha);
            yield return null;
        }

        // Ensure the final alpha is set correctly
        staminaBarMaterial.SetFloat("_Alpha", endAlpha);
    }

    private void HandleStaminaBar(bool usingStamina)
    {
        Debug.Log("Invoked Use" + usingStamina);

        isUsingStamina = usingStamina;
        StartCoroutine(FadeStaminaBar(usingStamina));
        // Assuming you have a way to get the current stamina value from PlayerID

    }
    private void OnEnable()
    {
        staminaBarMaterial.SetFloat("_Alpha", 0);
        StatsManager.OnScoreChanged += UpdateScore;
        StatsManager.OnAmmoChanged += UpdateAmmo;
        player.globalEvents.OnUseStamina += HandleStaminaBar;

    }
    private void OnDisable()
    {
        staminaBarMaterial.SetColor("_Color", Color.white);
        staminaBarMaterial.SetFloat("_OffsetUvX", 0);

        StatsManager.OnScoreChanged -= UpdateScore;
        StatsManager.OnAmmoChanged -= UpdateAmmo;
        player.globalEvents.OnUseStamina -= HandleStaminaBar;
    }


}


