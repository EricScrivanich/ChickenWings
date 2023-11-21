using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StatsDisplay : MonoBehaviour
{
    public PlayerID player;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI ammoText;

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
        statsMan.OnScoreChanged += UpdateScore;
        statsMan.OnAmmoChanged += UpdateAmmo;
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

    private void OnDestroy()
    {
        statsMan.OnScoreChanged -= UpdateScore;
        statsMan.OnAmmoChanged -= UpdateAmmo;
    }

    private void OnEnable() 
    {
        player.globalEvents.OnUpdateAmmo += UpdateAmmo;
    }
    private void OnDisable() 
    {
        player.globalEvents.OnUpdateAmmo -= UpdateAmmo;
    }
}


