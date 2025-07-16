using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
public class LevelPickerUIPopup : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [field: SerializeField] public Sprite[] ammoImages { get; private set; }

    [SerializeField] private GameObject statDisplayPrefab;
    [SerializeField] private RectTransform progressBar;
    [SerializeField] private Image progressBarFill;

    [SerializeField] private RectTransform flagImage;
    private LevelPickerManager levelPickerManager;



    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI levelNumberText;
    private Sequence moveStatSequence;
    [SerializeField] private RectTransform statDisplayParent;
    private RectTransform[] statDisplays;
    [SerializeField] private RectTransform mainStatDisplayPos;
    [SerializeField] private RectTransform topStatDisplayPos;
    [SerializeField] private RectTransform hiddenTopStatDisplayPos;
    [SerializeField] private RectTransform hiddenBottomStatDisplayPos;

    [SerializeField] private float topPosScale = .6f;
    [SerializeField] private float tweenDur;
    private int currentStatDisplayIndex = 0;


    public int amount;
    public int SetMainIndex = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ShowData(levelData);

    }
    private void OnValidate()
    {
        if (SetMainIndex >= 0)
        {
            MoveStatItems(SetMainIndex);
            SetMainIndex = -1;


        }
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
            moveStatSequence.Join(statDisplays[mainIndex].DOScale(1, tweenDur));

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



    public void ShowData(LevelData data, LevelPickerManager manager)
    {
        if (manager != null)
            levelPickerManager = manager;
        this.levelData = data;
        levelNameText.text = data.LevelName;

        if (data.levelWorldAndNumber != Vector3Int.zero)
        {
            levelNumberText.text = $"{data.levelWorldAndNumber.x}-{data.levelWorldAndNumber.y}";
            if (data.checkPointSteps != null && data.checkPointSteps.Length > 0)
                for (int i = 0; i < data.checkPointSteps.Length; i++)
                {
                    if (i == 0) flagImage.localPosition = new Vector2(GetXOnProgressBar(GetPercent(data.checkPointSteps[i])), flagImage.localPosition.y);
                    else
                    {
                        Instantiate(flagImage, flagImage.parent).localPosition = new Vector2(GetXOnProgressBar(GetPercent(data.checkPointSteps[i])), flagImage.localPosition.y);
                    }


                }
            else
                flagImage.gameObject.SetActive(false);



        }
        else
            levelNumberText.gameObject.SetActive(false);

        statDisplays = new RectTransform[amount];

        for (int i = 0; i < amount; i++)
        {

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
                    t.localScale = Vector3.one;
                    statDisplay.CreateSelf(this, new Vector2Int(data.StartingLives, data.StartingLives), data.StartingAmmos, true);
                }
                else if (i == amount - 2)
                {
                    t.position = topStatDisplayPos.position;
                    t.localScale = Vector3.one * topPosScale;
                    statDisplay.CreateSelf(this, new Vector2Int(data.StartingLives, data.StartingLives), data.StartingAmmos, false);

                }
                else
                {
                    t.position = hiddenTopStatDisplayPos.position;
                    t.localScale = Vector3.one * topPosScale;
                    statDisplay.CreateSelf(this, new Vector2Int(data.StartingLives, data.StartingLives), data.StartingAmmos, false);

                }

            }
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

    private float GetPercent(ushort step)
    {
        return (float)step / (float)levelData.finalSpawnStep;
    }

    private float GetXOnProgressBar(float percent)
    {
        return (progressBar.rect.width * percent) - progressBar.rect.width * .5f;
    }

    // Update is called once per frame

}
