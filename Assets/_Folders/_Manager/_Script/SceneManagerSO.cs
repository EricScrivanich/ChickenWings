using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Levels/SceneHolder")]
public class SceneManagerSO : ScriptableObject
{
    [SerializeField] private List<string> Levels_Scene;
    [SerializeField] private List<string> Levels_Name;
    [SerializeField] private List<LevelChallenges> Level_Challenges;
    [SerializeField] private List<string> OtherGameModes_Scene;
    [SerializeField] private List<string> OtherGameModes_Name;
    [SerializeField] private List<Image> Levels_UI;
    [SerializeField] private List<string> Gamemodes_Scene;
    [SerializeField] private List<Scene> Gamemodes_UI;

    [SerializeField] private List<int> levelsByAdditionalNeededChallengesToUnlock;
    [SerializeField] private List<string> additionalNeededUnlockText;

    [Header("0 - Stars, 1 - Badges, 2 - No More Levels")]
    [SerializeField] private List<Vector2Int> additonalChallengeTypeAndAmount;
    private int levelNumber;


    public void LoadLevel(int levelNumber)
    {
        if (levelNumber == 0 || levelNumber >= Levels_Scene.Count) return;

        if (Time.timeScale == 0) Time.timeScale = FrameRateManager.TargetTimeScale;

        if (levelNumber == -1)
            SceneManager.LoadScene("BasicsPig");
        else
            SceneManager.LoadScene(Levels_Scene[levelNumber]);

        SaveManager.instance.PreWarmShaders();


    }

    public int ReturnChallengeCountByLevel(int level)
    {
        Debug.Log("TRYNA REUTRN: " + level);
        if (Level_Challenges[level] != null)
        {
            Debug.Log("Challenge Count Return: " + level);
            return Level_Challenges[level].GetAmountOfChallenges();
        }

        else
            return 0;
    }

    public int ReturnNumberOfLevels()
    {
        return Levels_Name.Count - 1;
    }

    public Vector3Int NeedsAddtionalUnlock(int l)
    {
        if (l == -1) l = levelNumber + 1;




        for (int i = 0; i < levelsByAdditionalNeededChallengesToUnlock.Count; i++)
        {
            if (l == levelsByAdditionalNeededChallengesToUnlock[i])
            {
                return new Vector3Int(i, additonalChallengeTypeAndAmount[i].x, additonalChallengeTypeAndAmount[i].y);

            }
        }
        return Vector3Int.zero;
    }

    public string RetrunAdditionalChallengeText(int index)
    {
        if (additionalNeededUnlockText == null || index >= additionalNeededUnlockText.Count) return null;
        return additionalNeededUnlockText[index];
    }



    public LevelChallenges ReturnLevelChallenges()
    {
        if (levelNumber == -1 || Level_Challenges[levelNumber] == null || levelNumber >= Level_Challenges.Count)
            return null;
        return Level_Challenges[levelNumber];
    }

    public void SetLevelNumber(int num)
    {
        levelNumber = num;

    }

    public int ReturnLevelNumber()
    {
        return levelNumber;
    }

    public void LoadGamemode(int type)
    {
        if (type >= OtherGameModes_Scene.Count) return;
        if (Time.timeScale == 0) Time.timeScale = FrameRateManager.TargetTimeScale;

        SceneManager.LoadScene(OtherGameModes_Scene[type]);
        SaveManager.instance.PreWarmShaders();




    }

    public string ReturnLevelName(int index)
    {
        return Levels_Name[index];
    }
    public int LevelsCount()
    {
        return Levels_Scene.Count;
    }

    public string ReturnGameModeName(int index)
    {
        return OtherGameModes_Name[index];
    }
    public int GameModesCount()
    {
        return OtherGameModes_Scene.Count;
    }
    public string ReturnSceneNameGameMode(int index)
    {
        if (OtherGameModes_Scene.Count > index && OtherGameModes_Scene[index] != null)
            return OtherGameModes_Scene[index];
        else
            return "none";
    }

    public string ReturnSceneNameLevel(int index)
    {
        if (Levels_Scene[index] != null)
            return Levels_Scene[index];
        else
            return "none";
    }

}
