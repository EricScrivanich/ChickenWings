using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class LevelPickerUIPopup : MonoBehaviour, IButtonListener
{
    [SerializeField] private ChallengesUIManager challengeManager;

    [SerializeField] private LevelData levelData;
    [SerializeField] private string sceneLoadOvverride;
    [field: SerializeField] public Sprite[] ammoImages { get; private set; }

    [SerializeField] private GameObject statDisplayPrefab;
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private Image progressBarFill;

    [SerializeField] private RectTransform flagImage;
    [SerializeField] private GameObject startButton;

    private LevelPickerManager levelPickerManager;
    private bool willOverwriteCheckPointSave = false;
    private List<ButtonTouchByIndex> buttons = new List<ButtonTouchByIndex>();



    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI levelNumberText;
    private Sequence moveStatSequence;
    [SerializeField] private RectTransform statDisplayParent;
    private RectTransform[] statDisplays;
    [SerializeField] private RectTransform mainStatDisplayPos;
    [SerializeField] private RectTransform topStatDisplayPos;
    [SerializeField] private RectTransform hiddenTopStatDisplayPos;
    [SerializeField] private RectTransform hiddenBottomStatDisplayPos;

    [SerializeField] private float topPosScale = .8f;
    [SerializeField] private float normPosScale;
    [SerializeField] private float tweenDur;
    private int currentStatDisplayIndex = 0;
    private int checkPointToLoad = -1;
    private int currentSelectedLevelDifficulty = 1; // Default difficulty is 1 (normal)

    private TemporaryAllLevelCheckPointData checkpointData = null;
    private int amount = 0;
    public int SetMainIndex = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ShowData(levelData);

    }
    // private void OnValidate()
    // {
    //     if (SetMainIndex >= 0)
    //     {
    //         MoveStatItems(SetMainIndex);
    //         checkPointToLoad = SetMainIndex - 1;
    //         SetMainIndex = -1;


    //     }
    // }


    public void MoveStatItems(int mainIndex)
    {
        if (mainIndex != currentStatDisplayIndex && mainIndex >= 0 && mainIndex < amount)
        {
            if (moveStatSequence != null && moveStatSequence.IsActive())
            {
                moveStatSequence.Kill();
            }

            currentStatDisplayIndex = mainIndex;
            statDisplays[mainIndex].GetComponent<LevelPickerUIStatBar>().SetColor(true);

            moveStatSequence = DOTween.Sequence();
            moveStatSequence.Append(statDisplays[mainIndex].DOAnchorPos(mainStatDisplayPos.anchoredPosition, tweenDur));
            moveStatSequence.Join(statDisplays[mainIndex].DOScale(normPosScale, tweenDur));

            for (int i = 0; i < amount; i++)
            {
                if (i == mainIndex) continue;

                statDisplays[i].GetComponent<LevelPickerUIStatBar>().SetColor(false);

                if (i > mainIndex)
                {
                    moveStatSequence.Join(statDisplays[i].DOAnchorPos(hiddenBottomStatDisplayPos.anchoredPosition, tweenDur));
                    moveStatSequence.Join(statDisplays[i].DOScale(topPosScale, tweenDur));
                }
                else if (i == currentStatDisplayIndex - 1)
                {
                    moveStatSequence.Join(statDisplays[i].DOAnchorPos(topStatDisplayPos.anchoredPosition, tweenDur));
                    moveStatSequence.Join(statDisplays[i].DOScale(topPosScale, tweenDur));
                }
                else
                {
                    moveStatSequence.Join(statDisplays[i].DOAnchorPos(hiddenTopStatDisplayPos.anchoredPosition, tweenDur));
                    moveStatSequence.Join(statDisplays[i].DOScale(topPosScale, tweenDur));
                }
            }

            moveStatSequence.Play();
        }

    }



    public void ShowData(LevelData data, LevelPickerManager manager, int levelDifficulty = 1)
    {
        if (manager != null)
            levelPickerManager = manager;

        this.levelData = data;
        LevelDataConverter.instance.ReturnAndLoadWorldLevelData(levelData);
        LevelDataConverter.instance.LoadLevelSavedData(levelData);
        levelNameText.text = data.LevelName;
        currentSelectedLevelDifficulty = levelDifficulty;
        willOverwriteCheckPointSave = false;
        checkPointToLoad = -1;
        var b = startButton.GetComponent<ButtonTouchByIndex>();
        // b.SetData(-1, this, false);
        buttons.Add(b);
        if (levelDifficulty == 1)
            progressBarFill.fillAmount = (float)LevelDataConverter.instance.ReturnLevelSavedData().FurthestCompletion / (float)data.finalSpawnStep;
        else if (levelDifficulty == 0)
            progressBarFill.fillAmount = (float)LevelDataConverter.instance.ReturnLevelSavedData().FurthestCompletionEasy / (float)data.finalSpawnStep;


        int validCheckPoints = 0;
        amount = 1;
        bool usingCheckpoints = false;

        if (LevelDataConverter.instance.CheckIfLevelOverwrite(data.levelWorldAndNumber, levelDifficulty))
        {
            willOverwriteCheckPointSave = true;
            Debug.LogError("Will Overwrite Checkpoint Save for Level: " + data.levelWorldAndNumber + " Difficulty: " + levelDifficulty);

        }
        else
        {
            checkpointData = LevelDataConverter.instance.ReturnAllCheckPointDataForLevel();
            if (checkpointData != null && checkpointData.LevelCheckPointData != null && checkpointData.LevelCheckPointData.Length > 0)
            {
                amount += checkpointData.LevelCheckPointData.Length;
                validCheckPoints = checkpointData.LevelCheckPointData.Length;
                usingCheckpoints = true;
                checkPointToLoad = checkpointData.LevelCheckPointData.Length - 1;
            }
        }

        statDisplays = new RectTransform[amount];
        if (!usingCheckpoints)
        {
            var o = Instantiate(statDisplayPrefab, statDisplayParent);
            RectTransform t = o.GetComponent<RectTransform>();
            statDisplays[0] = t;

            var statDisplay = statDisplays[0].GetComponent<LevelPickerUIStatBar>();
            currentStatDisplayIndex = 0;
            t.position = mainStatDisplayPos.position;
            t.localScale = Vector3.one;
            short[] ammos = data.StartingAmmos;
            statDisplay.CreateSelf(this, 0, new Vector2Int(data.StartingLives, data.StartingLives), ammos, true);
        }

        else
        {
            var checkpointData = LevelDataConverter.instance.ReturnAllCheckPointDataForLevel();
            int maxLives = data.StartingLives;

            short[] ammos = data.StartingAmmos;

            if (levelDifficulty == 0)
            {
                maxLives = data.easyStartingLives;
                ammos = data.easyStartingAmmos;
            }
            Debug.LogError("Amount: " + amount);
            int currentLives = maxLives;
            Vector2Int lives = new Vector2Int(currentLives, maxLives);
            for (int i = 0; i < amount; i++)
            {
                if (i > 0)
                {
                    lives = new Vector2Int(checkpointData.LevelCheckPointData[i - 1].CurrentLives, maxLives);
                    ammos = checkpointData.LevelCheckPointData[i - 1].CurrentAmmos;
                }

                var o = Instantiate(statDisplayPrefab, statDisplayParent);
                RectTransform t = o.GetComponent<RectTransform>();
                statDisplays[i] = t;

                var statDisplay = statDisplays[i].GetComponent<LevelPickerUIStatBar>();
                if (statDisplay != null)
                {
                    if (i == amount - 1)
                    {
                        currentStatDisplayIndex = i;
                        t.position = mainStatDisplayPos.position;
                        t.localScale = Vector3.one * normPosScale;
                        t.localScale = Vector3.one;
                        statDisplay.CreateSelf(this, i, lives, ammos, true);
                    }
                    else if (i == amount - 2)
                    {
                        t.position = topStatDisplayPos.position;
                        t.localScale = Vector3.one * topPosScale;
                        statDisplay.CreateSelf(this, i, lives, ammos, false);

                    }
                    else
                    {
                        t.position = hiddenTopStatDisplayPos.position;
                        t.localScale = Vector3.one * topPosScale;
                        statDisplay.CreateSelf(this, i, lives, ammos, false);

                    }

                }
            }


        }


        if (data.levelWorldAndNumber != Vector3Int.zero)
        {
            levelNumberText.text = $"{data.levelWorldAndNumber.x}-{data.levelWorldAndNumber.y}";
            if (data.checkPointSteps != null && data.checkPointSteps.Length > 0)
            {
                for (int i = 0; i < data.checkPointSteps.Length; i++)
                {
                    if (i == 0)
                    {
                        flagImage.localPosition = new Vector2(GetXOnProgressBar(GetPercent(data.checkPointSteps[i])), flagImage.localPosition.y);
                        buttons.Add(flagImage.GetComponent<ButtonTouchByIndex>());
                    }
                    else
                    {
                        var f = Instantiate(flagImage, flagImage.parent);
                        f.localPosition = new Vector2(GetXOnProgressBar(GetPercent(data.checkPointSteps[i])), flagImage.localPosition.y);
                        buttons.Add(f.GetComponent<ButtonTouchByIndex>());
                    }





                }




            }

            else
                flagImage.gameObject.SetActive(false);

            for (int i = 0; i < buttons.Count; i++)
            {
                bool canPress = i - 1 < validCheckPoints;
                buttons[i].SetData(i - 1, this, !canPress);
            }
            CheckCheckPoints(checkPointToLoad);
        }
        else
            levelNumberText.gameObject.SetActive(false);



    }

    public void PlayLevel()
    {
        HapticFeedbackManager.instance.PlayerButtonPress();

        if (LevelDataConverter.instance.ReturnAllCheckPointDataForLevel() == null || willOverwriteCheckPointSave)
        {
            LevelDataConverter.instance.OverwriteCheckpointDataForNewLevel(levelData.levelWorldAndNumber, currentSelectedLevelDifficulty);
        }
        else if (LevelDataConverter.instance.CheckIfCheckPointOverwrite((short)checkPointToLoad))
        {
            LevelDataConverter.instance.OverwriteCheckPoint((short)checkPointToLoad);
        }
        if (levelData.tutorialData == null)
        {
            SceneManager.LoadScene("MainLevelPlayer");
        }
        else
        {
            SceneManager.LoadScene("MainLevelPlayTutorial");
        }

    }
    public void ExitView()
    {
        if (levelPickerManager != null)
        {
            levelPickerManager.BackOut();
            
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void CheckCheckPoints(int index)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].CheckIfIndex(index);
        }

        TemporaryLevelCheckPointData progressData = null;

        if (index >= 0 && checkpointData != null)
        {
            progressData = checkpointData.LevelCheckPointData[index];
        }

        challengeManager?.ShowChallengesForLevelPicker(levelData.GetLevelChallenges(true, progressData), LevelDataConverter.instance.ReturnLevelSavedData());

    }

    private float GetPercent(ushort step)
    {
        return (float)step / (float)levelData.finalSpawnStep;
    }

    private float GetXOnProgressBar(float percent)
    {
        return (progressBar.rect.width * percent) - progressBar.rect.width * .5f;
    }

    public void Press(int index)
    {
        MoveStatItems(index + 1);
        checkPointToLoad = index;
        CheckCheckPoints(index);
    }

    // Update is called once per frame

}
