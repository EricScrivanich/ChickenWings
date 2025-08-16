
#if UNITY_EDITOR
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

public class PickLevelEditorOnly : MonoBehaviour
{
    [SerializeField] private AllObjectData objData;
    [SerializeField] private PlayerStartingStatsForLevels startingStats;
    [SerializeField] private PlayerID playerID;
    private LevelData selectedLevelData;
    private LevelData tempSelectedLevelData;
    private static LevelData[] sortedLevels;
    [SerializeField] private Transform[] levelButtonParents;
    [SerializeField] private GameObject addLevelWindow;
    [SerializeField] private Button chooseLevelButton;

    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private int maxLevelPerSection = 5;

    private List<PickLevelLevelButton> levelButtons = new List<PickLevelLevelButton>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Load all LevelData assets in "Assets/Levels/Main"

    }
    public static string ReturnLevelName(string levelName, Vector3Int numbers, bool display)
    {

        // check if any levels already use numbers
        if (string.IsNullOrEmpty(levelName) || numbers == null || numbers.x <= 0 || numbers.y <= 0 || numbers.z < 0)
        {
            Debug.LogWarning("Invalid level name");
            return null;
        }
        foreach (var level in sortedLevels)
        {
            if (level.levelWorldAndNumber == numbers)
            {
                Debug.LogWarning($"Level with name {levelName} and numbers {numbers} already exists.");
                return null;
            }
        }
        string numberString = "";

        if (numbers.z <= 0)
            numberString = $"{numbers.x:00}-{numbers.y:00}_";
        else
            numberString = $"{numbers.x:00}-{numbers.y:00}-{numbers.z:00}_";

        if (display)
        {
            // replaceUnderscore with space in numberString as well as remove the tenth place if zero
            numberString = numberString.Replace("_", " ");
            if (numbers.z <= 0)
            {
                numberString = numberString.Substring(0, numberString.Length - 3);
            }
            else
            {
                numberString = numberString.Substring(0, numberString.Length - 4);
            }
        }

        return numberString + levelName;



    }

    public void ChooseLevel(string levelName)
    {
        var chosen = sortedLevels.FirstOrDefault(l => l.LevelName == levelName);
        if (chosen != null)
        {
            Debug.Log("Chosen Level: " + chosen.LevelName);
            tempSelectedLevelData = chosen;
            chooseLevelButton.interactable = true;
            foreach (var button in levelButtons)
            {
                button.CheckIfSelected(levelName);
            }
            // Do something with the level if needed
        }
        else
        {
            Debug.LogWarning("Level not found: " + levelName);
        }
    }

    public void AddNewLevel(Vector3Int numbers, string name)
    {
        LevelData asset = ScriptableObject.CreateInstance<LevelData>();
        LevelChallenges challenges = ScriptableObject.CreateInstance<LevelChallenges>();

        string numberString = "";

        if (numbers.z <= 0)
            numberString = $"{numbers.x:00}-{numbers.y:00}_";
        else
            numberString = $"{numbers.x:00}-{numbers.y:00}-{numbers.z:00}_";

        string n = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/Main/" + numberString + name + ".asset");
        string challengePath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/Challenges/" + numberString + name + "Challenge" + ".asset");
        AssetDatabase.CreateAsset(asset, n);
        AssetDatabase.CreateAsset(challenges, challengePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();



        asset.SetDefaults(objData, startingStats, playerID, challenges);
        asset.LoadLevelSaveData(null);
        asset.LevelName = name;
        asset.levelWorldAndNumber = numbers;
        UnityEditor.EditorUtility.SetDirty(asset);
        UnityEditor.EditorUtility.SetDirty(challenges);

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        LevelDataConverter.instance.AddLevel(asset); // This will also reorder the levels
        LevelRecordManager.instance.LoadNewLevel(asset);

    }

    public void OpenAddLevelWindow(bool open)
    {
        if (open)
        {
            addLevelWindow.SetActive(true);
            foreach (var parent in levelButtonParents)
            {
                parent.gameObject.SetActive(false);
            }

        }
        else
        {
            addLevelWindow.SetActive(false);
            foreach (var parent in levelButtonParents)
            {
                parent.gameObject.SetActive(true);
            }
        }

    }

    public void ConfirmLevelChoice()
    {
        if (tempSelectedLevelData != null)
        {
            selectedLevelData = tempSelectedLevelData;
            Debug.Log("Confirmed Level: " + selectedLevelData.LevelName);
            LevelRecordManager.instance.LoadNewLevel(selectedLevelData);
            // Here you can add logic to handle the confirmed level, like saving it or starting the game
        }
        else
        {
            Debug.LogWarning("No level selected to confirm.");
        }
    }

    private void OnEnable()
    {
        addLevelWindow.SetActive(false);
        chooseLevelButton.interactable = false;
        foreach (var parent in levelButtonParents)
        {
            parent.gameObject.SetActive(true);
        }
        levelButtons.Clear();
        for (int n = 0; n < levelButtonParents.Length; n++)
        {
            for (int i = levelButtonParents[n].childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(levelButtonParents[n].GetChild(i).gameObject);
            }
        }

        string[] guids = AssetDatabase.FindAssets("t:LevelData", new[] { "Assets/Levels/Main" });
        int currentLevelCount = 0;
        int currentLevelParent = 0;
        sortedLevels = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<LevelData>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(ld => ld != null)
            .OrderBy(ld => ld.levelWorldAndNumber.x)
            .ThenBy(ld => ld.levelWorldAndNumber.y)
            .ThenBy(ld => ld.levelWorldAndNumber.z)
            .ThenBy(ld => ld.LevelName) // Assuming you want to sort by title/levelName
            .ToArray();

        foreach (var level in sortedLevels)
        {
            Debug.Log("Loaded Level: " + level.LevelName);
            GameObject buttonObject = Instantiate(levelButtonPrefab, levelButtonParents[currentLevelParent]);
            levelButtons.Add(buttonObject.GetComponent<PickLevelLevelButton>());
            string numberString = "";
            if (level.levelWorldAndNumber.z <= 0)
                numberString = $"{level.levelWorldAndNumber.x:00}-{level.levelWorldAndNumber.y:00}_";
            else
                numberString = $"{level.levelWorldAndNumber.x:00}-{level.levelWorldAndNumber.y:00}-{level.levelWorldAndNumber.z:00}_";

            buttonObject.GetComponent<PickLevelLevelButton>().SetLevelName(this, numberString + level.LevelName, level.LevelName);
            currentLevelCount++;
            if (currentLevelCount >= maxLevelPerSection)
            {
                currentLevelCount = 0;
                currentLevelParent++;

            }
        }

    }
}
#endif

