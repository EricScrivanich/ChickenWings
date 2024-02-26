using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class StatsDisplay : MonoBehaviour
{
    public PlayerID player;
    private bool hasHit10;
    private bool hasHit100;
    private bool hasHit1000;
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



    [SerializeField] private TextMeshProUGUI temporaryScoreText; // Add this to your class variables
    private int temporaryScore = 0;
    private Coroutine fadeOutCoroutine = null;
    private int lastKnownScore = 0;

    void Start()
    {

        temporaryScoreText.alpha = 0;


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

        scoreText.text = "Score: " + player.Score.ToString();
        hasHit10 = false;
        hasHit100 = false;
        hasHit1000 = false;

        // Subscribe to events

    }

    private void CheckDigitAmount(int digit)
    {
        if (digit >= 1000 && !hasHit1000)
        {
            temporaryScoreText.margin = new Vector4(99, 0, 0, 0);
            hasHit1000 = true;
        }
        else if (digit >= 100 && !hasHit100)
        {
            temporaryScoreText.margin = new Vector4(66, 0, 0, 0);
            hasHit100 = true;

        }
        else if (digit >= 10 && !hasHit10)
        {
            temporaryScoreText.margin = new Vector4(33, 0, 0, 0);
            hasHit10 = true;

        }
    }
    void UpdateScore(int score)
    {
        temporaryScore += score; // Accumulate the temporary score
        temporaryScoreText.text = "+" + temporaryScore.ToString(); // Update the temporary score display

        // Fade in the temporary score text
        temporaryScoreText.alpha = .9f;

        // Restart the fade-out coroutine every time the score updates
        if (fadeOutCoroutine != null) StopCoroutine(fadeOutCoroutine);
        fadeOutCoroutine = StartCoroutine(FadeOutTempScore());
    }


    private IEnumerator FadeOutTempScore()
    {
        yield return new WaitForSeconds(2f); // Wait for 1.5 seconds
        float fadeDuration = 1.7f;
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float fadeAmount = Mathf.Lerp(.9f, 0, time / fadeDuration);
            temporaryScoreText.alpha = fadeAmount;
            yield return null;
        }

        // Fade out the temporary score text
        StartCoroutine(ParseScore(temporaryScore));
        temporaryScore = 0;

        // Gradually update the main score


        // Reset the temporary score

    }

    private IEnumerator ParseScore(int tempScoreVar)
    {
        float parseTime = .1f;
        if (tempScoreVar < 15)
        {
            parseTime = .1f;

        }
        else if (tempScoreVar < 30)
        {
            parseTime = .05f;


        }
        else if (tempScoreVar < 60)
        {
            parseTime = .02f;
        }
        else
        {
            parseTime = .01f;

        }

        
        int startScore = int.Parse(scoreText.text.Replace("Score: ", ""));
        int endScore = startScore + tempScoreVar;
        CheckDigitAmount(endScore);
        for (int i = startScore + 1; i <= endScore; i++)
        {
            scoreText.text = "Score: " + i;
            yield return new WaitForSeconds(parseTime); // Adjust the speed of score update as needed
        }

    }


    // void UpdateScore(int score)
    // {
    //     if (scoreText != null)
    //     {
    //         scoreText.text = "Score: " + score.ToString();

    //     }


    // }

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

        float startAlpha = beingUsed ? 0 : .95f; // Starting alpha value
        float endAlpha = beingUsed ? .95f : 0; // Ending alpha value



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
        player.globalEvents.OnUpdateScore += UpdateScore;
        player.globalEvents.OnUpdateAmmo += UpdateAmmo;
        player.globalEvents.OnUseStamina += HandleStaminaBar;
        player.globalEvents.OnZeroStamina += FlashStamimaBG;

        StaminaGroup.alpha = 0;


    }
    private void OnDisable()
    {
        player.globalEvents.OnUpdateScore -= UpdateScore;
        player.globalEvents.OnUpdateAmmo -= UpdateAmmo;
        player.globalEvents.OnUseStamina -= HandleStaminaBar;
        player.globalEvents.OnZeroStamina -= FlashStamimaBG;
        staminaBarMaterial.SetColor("_Color", Color.white);

        staminaBarMaterial.SetFloat("_OffsetUvX", 0);


    }


}


