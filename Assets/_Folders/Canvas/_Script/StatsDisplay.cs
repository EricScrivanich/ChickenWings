using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    public PlayerID player;
    private bool isUsingStamina;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI ammoText;
    public Material staminaBarMaterial; // Reference to the stamina bar's material

    private StatsManager statsMan;

    void Start()
    {
        statsMan = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();

        // Find the StatsPanel
        Transform statsPanel = transform.Find("StatsPanel");
        if (statsPanel != null)
        {
            scoreText = statsPanel.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            ammoText = statsPanel.Find("AmmoText")?.GetComponent<TextMeshProUGUI>();
        }

        // Subscribe to events
        StatsManager.OnScoreChanged += UpdateScore;
        StatsManager.OnAmmoChanged += UpdateAmmo;
        player.globalEvents.OnUseStamina += HandleStaminaBar;
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
            staminaBarMaterial.SetFloat("_ClipUvRight", player.StaminaUsed * .01f);
            // Debug.Log(player.StaminaUsed * .01f);
        }


    }

    private void HandleStaminaBar(bool usingStamina)
    {
        Debug.Log("Yes");
        isUsingStamina = usingStamina;
        // Assuming you have a way to get the current stamina value from PlayerID

    }

    private void OnDestroy()
    {
        StatsManager.OnScoreChanged -= UpdateScore;
        StatsManager.OnAmmoChanged -= UpdateAmmo;
        player.globalEvents.OnUseStamina -= HandleStaminaBar;
    }
}


