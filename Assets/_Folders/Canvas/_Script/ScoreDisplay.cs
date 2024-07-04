using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ScoreDisplay : MonoBehaviour
{
    public PlayerID player;
    [SerializeField] private TextMeshProUGUI scoreText;
    private TextMeshProUGUI finalScore;
    [SerializeField] private TextMeshProUGUI temporaryScoreText;
    private int temporaryScore = 0;
    private int scoreDisplayed;
    private Coroutine fadeOutCoroutine = null;


    // Start is called before the first frame update
    void Start()
    {
        scoreDisplayed = player.Score;
        scoreText.text = "Score: " + scoreDisplayed.ToString();
       
        temporaryScoreText.alpha = 0;
        
        
    }

    // Update is called once per frame
    public void UpdateFinalScore()
    {
        finalScore = GameObject.Find("FinalScore").GetComponent<TextMeshProUGUI>();

        if (finalScore != null)
        {
            finalScore.text = "Your Score: " + player.Score.ToString();


        }
    }

    void UpdateScore(int scoreAdded)
    {
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

        for (int i = startScore + 1; i <= endScore; i++)
        {
            scoreDisplayed += 1;
            // CheckDigitAmount(scoreDisplayed);
            scoreText.text = "Score: " + scoreDisplayed.ToString();
            yield return new WaitForSeconds(parseTime); // Adjust the speed of score update as needed
        }

    }

    private void OnEnable()
    {
        player.globalEvents.OnAddScore += UpdateScore;
    }
    private void OnDisable()
    {
        player.globalEvents.OnAddScore -= UpdateScore;
    }



}
