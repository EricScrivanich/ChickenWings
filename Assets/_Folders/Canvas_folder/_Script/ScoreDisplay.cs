using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
public class ScoreDisplay : MonoBehaviour
{
    public PlayerID player;
    public LevelManagerID lvlID;
    private TextMeshProUGUI highScoreText;
    private Sequence newBestSeq;

    [SerializeField] private ButtonColorsSO colorSO;
    [SerializeField] private TextMeshProUGUI scoreText;
    private TextMeshProUGUI finalScore;
    [SerializeField] private TextMeshProUGUI temporaryScoreText;
    private int temporaryScore = 0;
    private int scoreDisplayed;
    private Coroutine fadeOutCoroutine = null;
    private int highScore;

    private bool beatenHighScore = false;

    [SerializeField] private bool flappyFrenzy;

    [SerializeField] private Color highScoreColor;


    // Start is called before the first frame update
    void Start()
    {

        beatenHighScore = false;
        highScoreText = GetComponent<TextMeshProUGUI>();
        highScore = PlayerPrefs.GetInt("EndlessHighScore", 0);
        highScoreText.text = ("Best: " + highScore.ToString());
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
        lvlID.inputEvent.OnUpdateObjective?.Invoke("score", scoreAdded);
        if (player.Score > highScore && !beatenHighScore && !FrameRateManager.under1)
        {
            beatenHighScore = true;
            highScoreText.text = "NEW BEST";
            highScoreText.color = scoreText.color;
            newBestSeq = DOTween.Sequence();
            newBestSeq.Append(highScoreText.DOColor(colorSO.goUnPressedColor, .7f).From(colorSO.goPressedColor));
            newBestSeq.Append(highScoreText.DOColor(colorSO.goPressedColor, .8f));
            newBestSeq.AppendInterval(.4f);
            newBestSeq.Play().SetLoops(-1);
        }
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
        yield return new WaitForSeconds(1.8f); // Wait for 1.5 seconds
        float fadeDuration = 1.2f;
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


    // 0-Normal; 1-Jetpack; 2-BigPig; 3-Tenderizer; 4-Pilot; 5- Missile; 6-BomberPlane; 7-Flappy; 8-Gas; 9-Balloon
    private void KillPigScore(int type)
    {

        if (!flappyFrenzy)
        {
            switch (type)
            {
                case (0):
                    player.AddScore(5);
                    break;

                case (1):
                    player.AddScore(10);
                    break;
                case (2):
                    player.AddScore(10);
                    break;
                case (3):
                    player.AddScore(20);
                    break;
                case (4):
                    player.AddScore(15);
                    break;

                case (5):
                    player.AddScore(15);
                    break;
                case (6):
                    player.AddScore(25);
                    break;
                case (7):
                    player.AddScore(15);
                    break;
                case (8):
                    player.AddScore(10);
                    break;
                case (9):
                    player.AddScore(10);
                    break;

            }
        }

        else if (flappyFrenzy)
        {
            switch (type)
            {
                // case (0):
                //     player.AddScore(5);
                //     break;

                // case (1):
                //     player.AddScore(10);
                //     break;
                // case (2):
                //     player.AddScore(10);
                //     break;
                // case (3):
                //     player.AddScore(20);
                //     break;
                // case (4):
                //     player.AddScore(15);
                //     break;

                // case (5):
                //     player.AddScore(15);
                //     break;
                // case (6):
                //     player.AddScore(25);
                //     break;
                case (7):
                    player.AddScore(5);

                    GameTimer.OnAddFlappyPig?.Invoke(false);
                    break;
                    // case (8):
                    //     player.AddScore(10);
                    //     break;
                    // case (9):
                    //     player.AddScore(10);
                    //     break;

            }

        }



    }

    private void OnSwitchTimeScale(bool isOver1)
    {
        if (!isOver1 && beatenHighScore)
        {
            if (newBestSeq != null && newBestSeq.IsPlaying())
                newBestSeq.Kill();


            PlayerPrefs.SetInt("EndlessHighScore", player.Score);
            PlayerPrefs.Save();

            highScore = PlayerPrefs.GetInt("EndlessHighScore", 0);

            highScoreText.color = highScoreColor;
            highScoreText.text = ("Best: " + highScore.ToString());

        }

    }

    private void OnEnable()
    {
        player.globalEvents.OnAddScore += UpdateScore;
        player.globalEvents.OnKillPig += KillPigScore;
        FrameRateManager.OnChangeGameTimeScale += OnSwitchTimeScale;
    }
    private void OnDisable()
    {
        player.globalEvents.OnAddScore -= UpdateScore;
        player.globalEvents.OnKillPig -= KillPigScore;
        FrameRateManager.OnChangeGameTimeScale -= OnSwitchTimeScale;

        if (player.Score > highScore && !FrameRateManager.under1)
        {
            PlayerPrefs.SetInt("EndlessHighScore", player.Score);
            PlayerPrefs.Save();
        }


        if (newBestSeq != null && newBestSeq.IsPlaying())
            newBestSeq.Kill();


    }



}
