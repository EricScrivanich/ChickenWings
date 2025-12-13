using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class CheckPointSelection : MonoBehaviour, IButtonListener
{
    [SerializeField] private GameObject statDisplayPrefab;
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private Image progressBarFill;

    [SerializeField] private RectTransform flagImage;
    [SerializeField] private GameObject startButton;
    private bool willOverwriteCheckPointSave = false;
    [SerializeField] private RectTransform chickenObject;
    private List<ButtonTouchByIndex> buttons = new List<ButtonTouchByIndex>();

    [SerializeField] private RectTransform mainStatDisplayPos;
    [SerializeField] private RectTransform topStatDisplayPos;
    [SerializeField] private RectTransform hiddenTopStatDisplayPos;
    [SerializeField] private RectTransform hiddenBottomStatDisplayPos;
    [SerializeField] private RectTransform statDisplayParent;
    private RectTransform[] statDisplays;
    [SerializeField] private LevelData levelData;
    private TemporaryAllLevelCheckPointData checkpointData = null;

    private int amount = 0;

    private Sequence moveStatSequence;

    private int currentStatDisplayIndex = 0;
    private int checkPointToLoad = -1;
    private int currentSelectedLevelDifficulty = 1;

    [SerializeField] private float topPosScale = .8f;
    [SerializeField] private float normPosScale;
    [SerializeField] private float tweenDur;

    [field: SerializeField] public Sprite[] ammoImages { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ShowData(LevelData data, LevelPickerManager manager, int levelDifficulty = 1, bool redo = false)
    {
        data.Difficulty = levelDifficulty;
        bool isChallenge = false;
        if (levelDifficulty == 3) isChallenge = true;
        if (redo)
        {
            foreach (Transform child in statDisplayParent)
            {
                if (child.GetComponent<LevelPickerUIStatBar>() != null)
                    Destroy(child.gameObject);
            }

            DOTween.Kill(progressBarFill);
            if (levelDifficulty >= 1)
            {
                float fill = (float)LevelDataConverter.instance.ReturnLevelSavedData().FurthestCompletion / (float)data.finalSpawnStep;
                progressBarFill.DOFillAmount(fill, .7f).SetUpdate(true);
            }

            else if (levelDifficulty == 0)
            {
                float fill = (float)LevelDataConverter.instance.ReturnLevelSavedData().FurthestCompletionEasy / (float)data.finalSpawnStep;
                progressBarFill.DOFillAmount(fill, .7f).SetUpdate(true);

            }
        }
        else if (manager != null)
        {
          
            if (levelDifficulty >= 1)
                progressBarFill.fillAmount = (float)LevelDataConverter.instance.ReturnLevelSavedData().FurthestCompletion / (float)data.finalSpawnStep;
            else if (levelDifficulty == 0)
                progressBarFill.fillAmount = (float)LevelDataConverter.instance.ReturnLevelSavedData().FurthestCompletionEasy / (float)data.finalSpawnStep;

            var b = startButton.GetComponent<ButtonTouchByIndex>();
            // b.SetData(-1, this, false);
            buttons.Add(b);
        }


        currentSelectedLevelDifficulty = levelDifficulty;
        willOverwriteCheckPointSave = false;
        checkPointToLoad = -1;



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
            if (levelDifficulty >= 1)
                statDisplay.CreateSelfNew(this, 0, new Vector2Int(data.StartingLives, data.StartingLives), data.StartingAmmos, true);
            else
            {
                statDisplay.CreateSelfNew(this, 0, new Vector2Int(data.easyStartingLives, data.easyStartingLives), data.easyStartingAmmos, true);
            }
        }

        else
        {
            var checkpointData = LevelDataConverter.instance.ReturnAllCheckPointDataForLevel();
            int maxLives = 0;

            short[] ammos;

            if (levelDifficulty == 0)
            {
                maxLives = data.easyStartingLives;
                ammos = data.easyStartingAmmos;

            }
            else
            {
                ammos = data.StartingAmmos;
                maxLives = data.StartingLives;
            }

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
                        statDisplay.CreateSelfNew(this, i, lives, ammos, true);
                    }
                    else if (i == amount - 2)
                    {
                        t.position = topStatDisplayPos.position;
                        t.localScale = Vector3.one * topPosScale;
                        statDisplay.CreateSelfNew(this, i, lives, ammos, false);

                    }
                    else
                    {
                        t.position = hiddenTopStatDisplayPos.position;
                        t.localScale = Vector3.one * topPosScale;
                        statDisplay.CreateSelfNew(this, i, lives, ammos, false);

                    }

                }
            }


        }


        if (data.levelWorldAndNumber != Vector3Int.zero)
        {
            if (redo)
            {

            }
            else
            {
                // if (isChallenge)
                // {

                //     var o = Instantiate(difficultyPrefab, difficultyPanelsParent);
                //     var script = o.GetComponent<ButtonTouchByIndex>();
                //     script.SetData(3, this, false, false);
                //     script.CheckIfIndex(3);
                //     script.SetText(difficultyText[3]);
                //     levelDifficultyText.text = difficultyText[3];
                //     difficultyPanel.gameObject.GetComponent<Button>().interactable = false;
                //     lockedDifficultyObject.SetActive(true);

                // }
                // else
                // {
                //     List<ButtonTouchByIndex> difficultyButtonList = new List<ButtonTouchByIndex>();
                //     baseDifficultyHeight = difficultyPanel.rect.height;
                //     addedDifficultyHeightPerPanel = (addedDifficultyHeightPerPanel * 3) + baseDifficultyHeight;
                //     for (int i = 0; i < 3; i++)
                //     {
                //         var o = Instantiate(difficultyPrefab, difficultyPanelsParent);
                //         var script = o.GetComponent<ButtonTouchByIndex>();
                //         bool showMasterDifficulty = levelSavedData != null && levelSavedData.CompletedLevel;
                //         if (i == 2 && showMasterDifficulty)
                //             script.SetData(i, this, false);
                //         else if (i == 2)
                //             script.SetData(i, this, true);
                //         else
                //             script.SetData(i, this, false);

                //         script.SetText(difficultyText[i]);
                //         script.CheckIfIndex(currentSelectedLevelDifficulty);
                //         difficultyButtonList.Add(script);

                //     }
                //     difficultyButtons = difficultyButtonList.ToArray();



                // }
                // difficultyPanelsParent.gameObject.SetActive(false);

                // if (LevelDataConverter.currentChallengeType != 0)
                //     levelNumberText.text = $" {data.levelWorldAndNumber.x}-{data.levelWorldAndNumber.y}-<sprite name=\"{LevelDataConverter.currentChallengeType}\">";
                // else
                //     levelNumberText.text = $"{data.levelWorldAndNumber.x}-{data.levelWorldAndNumber.y}";


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
            }



            for (int i = 0; i < buttons.Count; i++)
            {

                bool canPress = i - 1 < validCheckPoints;

                buttons[i].SetData(i - 1, this, !canPress, redo);
            }
            CheckCheckPoints(checkPointToLoad);
        }
        // else
        //     levelNumberText.gameObject.SetActive(false);

    }

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


    private float GetXOnProgressBar(float percent)
    {
        return (progressBar.rect.width * percent) - progressBar.rect.width * .5f;
    }

    private float GetPercent(ushort step)
    {
        return (float)step / (float)levelData.finalSpawnStep;
    }

    public void Press(int index, ButtonTouchByIndex.ButtonType buttonType)
    {
        if (buttonType == ButtonTouchByIndex.ButtonType.LevelStart)
        {
            MoveStatItems(index + 1);
            checkPointToLoad = index;
            CheckCheckPoints(index);
        }
      

    }

    public void GetText(string text, ButtonTouchByIndex.ButtonType buttonType)
    {
        throw new System.NotImplementedException();
    }

    private void CheckCheckPoints(int index)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].CheckIfIndex(index);
        }
        DOTween.Kill(chickenObject);
        chickenObject.DOLocalMoveX(buttons[index + 1].GetLocalX(), .7f).SetUpdate(true).SetEase(Ease.InOutSine);

        TemporaryLevelCheckPointData progressData = null;

        if (index >= 0 && checkpointData != null)
        {
            progressData = checkpointData.LevelCheckPointData[index];
        }

        // challengeManager?.ShowChallengesForLevelPicker(levelData.GetLevelChallenges(true, progressData), LevelDataConverter.instance.ReturnLevelSavedData(), false);

    }
}
