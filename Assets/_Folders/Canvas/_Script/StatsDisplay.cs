using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class StatsDisplay : MonoBehaviour
{
    public PlayerID player;
    [SerializeField] private LevelManagerID LvlID;

    #region Stamina/Mana
    // [SerializeField] private Image ManaBar;
    // [SerializeField] private Image ManaBarBig;
    // private Coroutine fillManaBarCoroutine;
    // private Coroutine fillBigManaBarCoroutine;
    // public CanvasGroup StaminaGroup;
    // public Material staminaBarMaterial;
    // public Image StaminaBG; 
    // private bool canFlashStaminaBG = true;
    // [SerializeField] private float staminaBarFadeTime;
    // [SerializeField] private Color BGFlashColor;
    // [SerializeField] private int numverOfBGFlashes;
    // [SerializeField] private float totalStaminaBGFlashDuration;
    // private Color originalStaminaBGColor;
    // private bool isFilllingMana;

    // private float maxStamina;
    // private bool isUsingStamina;
    // private float bigTargetFill;
    // private float smallTargetFill;

    #endregion

    [Header("RingPass")]
    [SerializeField] private GameObject RingPanel;
    [SerializeField] private TextMeshProUGUI RingNumber;

    private bool hasHit10;
    private int lastUpdatedScore;
    private bool hasHit100;
    private bool hasHit1000;
    // 



    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScore;
    [SerializeField] private TextMeshProUGUI ammoText;


    [SerializeField] private Color lightRed;
    private int scoreDisplayed;



    [SerializeField] private TextMeshProUGUI temporaryScoreText;
    private int temporaryScore = 0;
    private Coroutine fadeOutCoroutine = null;
    private int lastKnownScore = 0;

    void Start()
    {
        // ManaBar.fillAmount = player.CurrentMana / player.MaxMana;
        // ManaBarBig.fillAmount = ((player.numberOfPowersThatCanBeUsed * player.ManaNeeded) / player.MaxMana);
        // originalStaminaBGColor = StaminaBG.color;
        // isFilllingMana = false;
        // maxStamina = player.MaxStamina;

        // if (LvlID != null && LvlID.areRingsRequired)
        // {
        //     RingPanel.SetActive(LvlID.areRingsRequired);
        //     RingNumber.text = LvlID.currentRingsPassed.ToString() + " / " + LvlID.ringsNeeded.ToString();

        // }


        scoreDisplayed = player.Score;
        scoreText.text = "Score: " + scoreDisplayed.ToString();
        UpdateAmmo();
        temporaryScoreText.alpha = 0;
        hasHit10 = false;
        hasHit100 = false;
        hasHit1000 = false;
    }

    // private void CheckDigitAmount(int digit)
    // {
    //     if (digit >= 1000 && !hasHit1000)
    //     {
    //         temporaryScoreText.margin = new Vector4(99, 0, 0, 0);
    //         hasHit1000 = true;
    //     }
    //     else if (digit >= 100 && !hasHit100)
    //     {
    //         temporaryScoreText.margin = new Vector4(66, 0, 0, 0);
    //         hasHit100 = true;

    //     }
    //     else if (digit >= 10 && !hasHit10)
    //     {
    //         temporaryScoreText.margin = new Vector4(33, 0, 0, 0);
    //         hasHit10 = true;

    //     }
    // }


    private void UpdateRingPanel(int number)
    {
        RingNumber.text = number.ToString() + " / " + LvlID.ringsNeeded.ToString();

    }

    public void UpdateFinalScore()
    {
        if (finalScore != null)
        {
            finalScore.text = "Your Score: " + player.Score.ToString();


        }
    }
    void UpdateScore(int scoreAdded)
    {
        // CheckDigitAmount(scoreDisplayed);


        if (scoreAdded == 1)
        {

            scoreDisplayed += 1;
            scoreText.text = "Score: " + scoreDisplayed.ToString();

            return;

        }
        temporaryScore += scoreAdded; // Accumulate the temporary score
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
        lastUpdatedScore = player.Score;
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

        for (int i = startScore + 1; i <= endScore; i++)
        {
            scoreDisplayed += 1;
            // CheckDigitAmount(scoreDisplayed);
            scoreText.text = "Score: " + scoreDisplayed.ToString();
            yield return new WaitForSeconds(parseTime); // Adjust the speed of score update as needed
        }


    }





    void UpdateAmmo()
    {
        if (ammoText != null)
        {
            // ammoText.text = "Ammo: " + player.Ammo.ToString();
            ammoText.text = player.Ammo.ToString();
        }
    }




    private void HandleDashArrow(bool canDash)
    {


    }
    private void OnEnable()
    {

        player.globalEvents.OnAddScore += UpdateScore;
        player.globalEvents.OnUpdateAmmo += UpdateAmmo;

        if (LvlID != null)
        {
            LvlID.outputEvent.RingParentPass += UpdateRingPanel;
        }

        // player.globalEvents.OnUseStamina += HandleStaminaBar;
        // player.globalEvents.OnZeroStamina += FlashStamimaBG;
        // player.globalEvents.AddMana += FillMana;
        // player.globalEvents.AddPowerUse += FillBigMana;
        // player.globalEvents.UsePower += UseMana;

        // StaminaGroup.alpha = 0;


    }
    private void OnDisable()
    {
        player.globalEvents.OnAddScore -= UpdateScore;
        player.globalEvents.OnUpdateAmmo -= UpdateAmmo;
        if (LvlID != null)
        {
            LvlID.outputEvent.RingParentPass -= UpdateRingPanel;
        }

        // player.globalEvents.OnUseStamina -= HandleStaminaBar;
        // player.globalEvents.OnZeroStamina -= FlashStamimaBG;
        // player.globalEvents.AddMana -= FillMana;
        // player.globalEvents.AddPowerUse -= FillBigMana;
        // player.globalEvents.UsePower -= UseMana;



        // staminaBarMaterial.SetColor("_Color", Color.white);

        // staminaBarMaterial.SetFloat("_OffsetUvX", 0);


    }


}

#region ManaStuff

// private void FillMana()
// {
//     smallTargetFill = player.CurrentMana / player.MaxMana;
//     if (!isFilllingMana)
//     {
//         fillManaBarCoroutine = StartCoroutine(FillManaBar());
//     }
// }
// private void FillBigMana(float fillAmount)
// {
//     bigTargetFill = fillAmount;
//     fillBigManaBarCoroutine = StartCoroutine(FillBigManaBar(fillAmount));
// }

// private void UseMana()
// {
//     StartCoroutine(UseManaCourintine());

// }
// private void StopManaCourintines()
// {
//     StopCoroutine(fillBigManaBarCoroutine);
//     StopCoroutine(fillManaBarCoroutine);
//     isFilllingMana = false;
//     // ManaBar.fillAmount = smallTargetFill;
//     // ManaBarBig.fillAmount = bigTargetFill;
// }
// private IEnumerator UseManaCourintine()
// {
//     StopManaCourintines();
//     float fillSpeed = .4f;
//     bool hasReachedBigTarget = false;
//     bool hasReachedSmallTarget = false;
//     float bigTarget = (player.numberOfPowersThatCanBeUsed * player.ManaNeeded) / player.MaxMana;
//     Debug.Log("BigTarget: " + bigTarget);
//     float smallTarget = player.CurrentMana / player.MaxMana;


//     while (!hasReachedBigTarget || !hasReachedSmallTarget)
//     {
//         if (ManaBarBig.fillAmount > bigTarget)
//         {
//             ManaBarBig.fillAmount -= fillSpeed * Time.deltaTime;
//         }
//         else
//         {
//             hasReachedBigTarget = true;
//         }

//         if (ManaBar.fillAmount > smallTarget)
//         {
//             ManaBar.fillAmount -= fillSpeed * Time.deltaTime;
//         }
//         else
//         {
//             hasReachedSmallTarget = true;
//         }

//         yield return null; // Wait for the next frame
//     }
// }

// private IEnumerator FillManaBar()
// {


//     isFilllingMana = true;
//     float fillSpeed = 1f; // Adjust this value to control the fill speed


//     while (ManaBar.fillAmount < smallTargetFill - .005f)   //!Mathf.Approximately(ManaBar.fillAmount, targetFillAmount)
//     {
//         ManaBar.fillAmount = Mathf.Lerp(ManaBar.fillAmount, smallTargetFill, fillSpeed * Time.deltaTime);
//         yield return null; // Wait for the next frame

//         // Update the target fill amount in case more stamina was added


//     }
//     ManaBar.fillAmount = smallTargetFill;
//     isFilllingMana = false;
// }

// private IEnumerator FillBigManaBar(float targetFillAmount)
// {
//     float fillSpeed = 2f; // Adjust this value to control the fill speed
//                           // Small threshold to determine "close enough"

//     while (ManaBarBig.fillAmount < targetFillAmount - .005f) // Subtract threshold to avoid floating-point precision issues
//     {
//         ManaBarBig.fillAmount = Mathf.Lerp(ManaBarBig.fillAmount, targetFillAmount, fillSpeed * Time.deltaTime);
//         yield return null; // Wait for the next frame
//     }

//     ManaBarBig.fillAmount = targetFillAmount; // Ensure the fill amount is exactly the target value when done
//     Debug.Log("Finished");
// }

// void Update()
// {
//     if (isUsingStamina)
//     {
//         // float staminaPercentage = player.CurrentStamina / player.MaxStamina;
//         staminaBarMaterial.SetFloat("_OffsetUvX", player.StaminaUsed / maxStamina);
//         Color staminaColor = Color.Lerp(Color.white, lightRed, player.StaminaUsed / maxStamina);
//         staminaBarMaterial.SetColor("_Color", staminaColor);
//         // Debug.Log(player.StaminaUsed * .01f);
//     }


// }

// private IEnumerator FlashBG()
// {
//     canFlashStaminaBG = false;
//     float flashDuration = totalStaminaBGFlashDuration / (numverOfBGFlashes * 2); // Calculate the duration of each flash

//     for (int i = 0; i < numverOfBGFlashes; i++)
//     {
//         // Change to the flash color
//         StaminaBG.color = BGFlashColor
// ;
//         yield return new WaitForSeconds(flashDuration);

//         // Revert to the original color
//         StaminaBG.color = originalStaminaBGColor;
//         yield return new WaitForSeconds(flashDuration);
//     }
//     canFlashStaminaBG = true;
// }

// private void FlashStamimaBG()
// {
//     if (canFlashStaminaBG)
//     {
//         StartCoroutine(FlashBG());
//     }
//     else
//     {
//         return;
//     }

// }


// private IEnumerator FadeStaminaBar(bool beingUsed)
// {
//     float time = 0;

//     float startAlpha = beingUsed ? 0 : .95f; // Starting alpha value
//     float endAlpha = beingUsed ? .95f : 0; // Ending alpha value



//     while (time < staminaBarFadeTime)
//     {
//         time += Time.deltaTime;
//         float alpha = Mathf.Lerp(startAlpha, endAlpha, time / staminaBarFadeTime);
//         // staminaBarMaterial.SetFloat("_Alpha", alpha);
//         StaminaGroup.alpha = alpha;
//         yield return null;
//     }

//     // Ensure the final alpha is set correctly
//     StaminaGroup.alpha = endAlpha;
// }

// private void HandleStaminaBar(bool usingStamina)
// {
//     if (isUsingStamina == usingStamina)
//     {
//         return;
//     }

//     isUsingStamina = usingStamina;
//     StartCoroutine(FadeStaminaBar(usingStamina));
//     // Assuming you have a way to get the current stamina value from PlayerID

// }

#endregion



