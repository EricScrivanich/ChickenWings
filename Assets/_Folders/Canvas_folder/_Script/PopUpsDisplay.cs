using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PopUpsDisplay : MonoBehaviour
{

    public PlayerID ID;
    [SerializeField] private bool isLevelCreator = false;

    [SerializeField] private GameObject levelChallenges;

    [SerializeField] private RectTransform levelChallengesPos;

    [SerializeField] private SceneManagerSO sceneSO;


    [Header("Level Title Display")]
    [SerializeField] private GameObject LevelNamePrefab;
    // [SerializeField] private TextMeshProUGUI GameSpeedText;

    [SerializeField] private float fadeInDuration = 1f; // Duration of the fade-in
    [SerializeField] private float fadeOutDuration = 1f; // Duration of the fade-out
    [SerializeField] private float displayDuration = 3f; // How long to display the text before fading out
    [SerializeField] private float textMoveDuration = 1f; // Duration for the text to move to the target position
    [SerializeField] private Vector2 textTargetPosition;
    [SerializeField] private Ease moveInEase;
    private bool isLevel;
    private Sequence newHighScoreSeq;


    [SerializeField] private bool showScore;

    private Vector3 gameOverScale = new Vector3(1, 1, 1);

    [SerializeField] private GameObject GameOverPrefab;
    [SerializeField] private GameObject LevelCompletePrefab;
    [SerializeField] private LevelManagerID LvlID;


    [SerializeField] private GameObject Frozen;
    private PlayerMovement playerMove;
    [SerializeField] private float lerpTime = 1f;
    private float currentLerpTime = 0f;
    private GameObject player;

    private RectTransform gameOver;


    private float positionArrivalThreshold = 1f;


    [SerializeField] private Material frozenMaterial;



    private string levelName;
    private int levelNum;
    private float fadeInDurationFroz = .7f;
    private float fadeOutDurationFroz = .2f;
    [SerializeField] private float frozenTime = .2f;




    // New variable for fade amount of frozenOverlayMaterial
    private void Awake()
    {
        isLevel = true;


        string s = SceneManager.GetActiveScene().name;
        levelNum = 0;

        for (int i = 0; i < sceneSO.LevelsCount(); i++)
        {
            if (sceneSO.ReturnSceneNameLevel(i) == s)
            {
                Debug.Log("YEE LOOP: " + 1);
                levelNum = i;
                sceneSO.SetLevelNumber(levelNum);

                break;
            }

        }

        if (levelNum == 0)
        {
            for (int i = 0; i < sceneSO.LevelsCount(); i++)
            {

                if (sceneSO.ReturnSceneNameGameMode(i) == s)
                {

                    levelNum = i;
                    sceneSO.SetLevelNumber(-1);
                    isLevel = false;
                    break;

                }

            }

        }
    }

    void Start()
    {

        // gameOver.gameObject.SetActive(false);
        Frozen.SetActive(false);
        // GameSpeedText.text = "Game Speed: " + PlayerPrefs.GetFloat("GameSpeed", 1).ToString("F2");

        var levelData = LevelDataConverter.instance.ReturnLevelData();


        if (levelData != null)
        {
            if (levelData.Difficulty > 0)
            {
                levelChallenges.GetComponent<ChallengesUIManager>().ShowChallengesForLevelPicker(levelData.GetLevelChallenges(false, null), LevelDataConverter.instance.ReturnLevelSavedData());
                levelChallenges.SetActive(true);
            }
            else
            {
                levelChallenges.SetActive(false);
            }






            LvlID.inputEvent.OnGetLevelNumber?.Invoke(levelNum);
            var textObj = Instantiate(LevelNamePrefab, Vector2.zero, Quaternion.identity, gameObject.GetComponentInParent<Transform>());
            TextMeshProUGUI textLevelName = textObj.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI textLevelNum = textObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            showScore = false;
            if (levelData.levelWorldAndNumber != Vector3Int.zero)
            {

                textLevelNum.text = "Level " + levelData.levelWorldAndNumber.x.ToString() + "-" + levelData.levelWorldAndNumber.y.ToString();
                LvlID.LevelTitle = textLevelNum.text;
                textLevelName.text = levelData.LevelName;

            }
            else
            {

                textLevelNum.text = "";
                textTargetPosition *= 1.1f;
                displayDuration *= .6f;
                textMoveDuration *= .7f;
                textLevelName.text = levelData.LevelName;
            }


            Sequence textSequence = DOTween.Sequence();

            // Move the text from the top of the screen to the target position
            RectTransform textRectTransform = textLevelName.GetComponent<RectTransform>();
            textRectTransform.anchoredPosition = new Vector2(0, Screen.height);
            var chall = levelChallenges.GetComponent<CanvasGroup>();

            // Add tweens to the sequence
            textSequence
                // Fade in both text objects
                .Append(textLevelName.DOFade(1, fadeInDuration))
                .Join(textLevelNum.DOFade(1, fadeInDuration))
                .Join(chall.DOFade(1, fadeInDuration * .8f).From(0))

                // Move the text to the target position
                .Join(textRectTransform.DOAnchorPos(textTargetPosition, textMoveDuration).SetEase(moveInEase))
                // .Join(GameSpeedText.DOFade(1, textMoveDuration).SetEase(Ease.InSine))


                // Keep the text on screen for displayDuration
                .AppendInterval(displayDuration)

                // Fade out both text objects
                .Append(textLevelName.DOFade(0, fadeOutDuration)).SetEase(Ease.InSine)
                .Join(textLevelNum.DOFade(0, fadeOutDuration)).SetEase(Ease.InSine)
                .Join(chall.DOFade(0, fadeOutDuration + .3f));
            // .Join(GameSpeedText.DOFade(0, fadeOutDuration)).SetEase(Ease.InSine);


            textSequence.Play().SetUpdate(true).OnComplete(() => DeleteDisplayObjects(textObj, levelChallenges)); // destroy gameSpeed here


        }
        else
        {
            Debug.LogError("LevelData is null");
        }


        return;

        // if (sceneSO.ReturnLevelChallenges() != null)
        // {
        //     Debug.LogError("YERERERE");
        //     Instantiate(levelChallengesPrefab, levelChallengesPos.position, Quaternion.identity, gameObject.GetComponentInParent<Transform>());
        // }
        // else
        //     Debug.LogError("Nothing here");


        // if (levelNum > 0 || !isLevel)
        // {
        //     LvlID.inputEvent.OnGetLevelNumber?.Invoke(levelNum);
        //     var textObj = Instantiate(LevelNamePrefab, Vector2.zero, Quaternion.identity, gameObject.GetComponentInParent<Transform>());
        //     TextMeshProUGUI textLevelName = textObj.GetComponent<TextMeshProUGUI>();
        //     TextMeshProUGUI textLevelNum = textObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();


        //     if (isLevel)
        //     {

        //         textLevelNum.text = "Level " + levelNum.ToString();
        //         LvlID.LevelTitle = textLevelNum.text;
        //         textLevelName.text = sceneSO.ReturnLevelName(levelNum);
        //         showScore = false;
        //     }
        //     else
        //     {

        //         textLevelNum.text = "";
        //         textTargetPosition *= 1.1f;
        //         displayDuration *= .6f;
        //         textMoveDuration *= .7f;
        //         textLevelName.text = sceneSO.ReturnGameModeName(levelNum);
        //     }


        //     Sequence textSequence = DOTween.Sequence();

        //     // Move the text from the top of the screen to the target position
        //     RectTransform textRectTransform = textLevelName.GetComponent<RectTransform>();
        //     textRectTransform.anchoredPosition = new Vector2(0, Screen.height);

        //     // Add tweens to the sequence
        //     textSequence
        //         // Fade in both text objects
        //         .Append(textLevelName.DOFade(1, fadeInDuration))
        //         .Join(textLevelNum.DOFade(1, fadeInDuration))

        //         // Move the text to the target position
        //         .Join(textRectTransform.DOAnchorPos(textTargetPosition, textMoveDuration).SetEase(moveInEase))
        //         .Join(GameSpeedText.DOFade(1, textMoveDuration).SetEase(Ease.InSine))


        //         // Keep the text on screen for displayDuration
        //         .AppendInterval(displayDuration)

        //         // Fade out both text objects
        //         .Append(textLevelName.DOFade(0, fadeOutDuration)).SetEase(Ease.InSine)
        //         .Join(textLevelNum.DOFade(0, fadeOutDuration)).SetEase(Ease.InSine)
        //         .Join(GameSpeedText.DOFade(0, fadeOutDuration)).SetEase(Ease.InSine);


        //     textSequence.Play().SetUpdate(true).OnComplete(() => DeleteDisplayTexts(textObj, GameSpeedText.gameObject));

        // }

        // sceneSO.isLevel = isLevel;



    }

    private void DeleteDisplayObjects(GameObject levelName, GameObject challengeCards)
    {
        Destroy(levelName);
        if (challengeCards != null)
            Destroy(challengeCards);
        // Destroy(gameSpeed);

    }




    void OnEnable()
    {
        ID.globalEvents.Frozen += FrozenEvent;
        ID.globalEvents.OnFinishedLevel += ShowFinishLevelPopup;

        if (LvlID != null)
        {
            LvlID.outputEvent.FinishedLevel += FinishLevel;
        }

    }

    private void ShowFinishLevelPopup()
    {
        if (isLevelCreator) return;
        Debug.Log("SHould finish Level");

        Instantiate(LevelCompletePrefab, transform);
        FinishLevel();
        LevelDataConverter.instance.SaveLevelDataForLevelComplete();
    }

    void OnDisable()
    {
        ID.globalEvents.Frozen -= FrozenEvent;
        ID.globalEvents.OnFinishedLevel -= ShowFinishLevelPopup;
        if (newHighScoreSeq != null && newHighScoreSeq.IsPlaying())
            newHighScoreSeq.Kill();
        DOTween.Kill(this);
        if (LvlID != null)
        {
            LvlID.outputEvent.FinishedLevel -= FinishLevel;
        }

    }

    public void GameOver()
    {
        gameOver = Instantiate(GameOverPrefab).GetComponent<RectTransform>();
        gameOver.SetParent(GameObject.Find("Canvas").transform);
        var scoreText = gameOver.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        if (showScore)
        {
            int high = PlayerPrefs.GetInt("EndlessHighScore", 0);
            if (high > ID.Score)
                scoreText.text = ("Final Score: " + ID.Score.ToString());
            else
            {
                scoreText.text = ("New High Score: " + ID.Score.ToString());
                Color prevCol = scoreText.color;

                newHighScoreSeq = DOTween.Sequence();

                newHighScoreSeq.Append(scoreText.DOColor(Color.white, .7f).From(prevCol));
                newHighScoreSeq.Append(scoreText.DOColor(prevCol, .8f));
                newHighScoreSeq.AppendInterval(.3f);

                newHighScoreSeq.Play().SetLoops(-1);

            }


        }
        else
            scoreText.gameObject.SetActive(false);
        gameOver.localScale = gameOverScale;
        gameOver.anchoredPosition = new Vector2(0, Screen.height + 100);

        gameOver.DOAnchorPos(new Vector2(0, 110), 1.4f).SetEase(Ease.OutSine).SetUpdate(true);

        // gameoverImage.gameObject.SetActive(true);
        // targetPosition = new Vector2(0, 1);

        // currentLerpTime = 0f;
        // StartCoroutine(MoveGameOverImage());
    }

    // private IEnumerator MoveGameOverImage()
    // {
    //     while (currentLerpTime < lerpTime)
    //     {
    //         currentLerpTime += Time.deltaTime;
    //         float perc = currentLerpTime / lerpTime;
    //         float smoothT = Mathf.SmoothStep(0, 1, perc);

    //         gameoverImage.transform.position = Vector2.Lerp(initialPosition, targetPosition, smoothT);

    //         yield return null;
    //     }
    // }

    private void FrozenEvent()
    {
        StartCoroutine(FadeInFrozenUI());
    }

    private IEnumerator FadeInFrozenUI()
    {
        Frozen.SetActive(true);
        // FrozenOverlay.SetActive(true);
        float elapsedTime = 0;
        while (elapsedTime < fadeInDuration)
        {
            float fadeAmount = Mathf.Lerp(1, 0, elapsedTime / fadeInDurationFroz);
            frozenMaterial.SetFloat("_FadeAmount", fadeAmount);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(frozenTime);
        StartCoroutine(FadeOutFrozenUI());
    }

    private IEnumerator FadeOutFrozenUI()
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeOutDuration)
        {
            float fadeAmount = Mathf.Lerp(0, 1, elapsedTime / fadeOutDurationFroz);
            frozenMaterial.SetFloat("_FadeAmount", fadeAmount);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Frozen.SetActive(false);
    }

    private void FinishLevel()
    {
        Debug.Log("Finish Level");
        Time.timeScale = 0;

    }


}
