using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelImporterWindow : EditorWindow
{
    private string jsonFilePath = "";
    private string levelName = "";
    private int worldNumber = 1;
    private int levelNumber = 1;
    private int subLevelNumber = 0;

    private AllObjectData objData;
    private PlayerStartingStatsForLevels startingStats;
    private PlayerID playerID;

    private Vector2 scrollPosition;
    private LevelDataSave previewData;

    [MenuItem("Tools/Level Importer")]
    public static void ShowWindow()
    {
        var window = GetWindow<LevelImporterWindow>("Level Importer");
        window.minSize = new Vector2(400, 500);
    }

    private void OnEnable()
    {
        // Try to find default references
        if (objData == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:AllObjectData");
            if (guids.Length > 0)
                objData = AssetDatabase.LoadAssetAtPath<AllObjectData>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        if (startingStats == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:PlayerStartingStatsForLevels");
            if (guids.Length > 0)
                startingStats = AssetDatabase.LoadAssetAtPath<PlayerStartingStatsForLevels>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        if (playerID == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:PlayerID");
            if (guids.Length > 0)
                playerID = AssetDatabase.LoadAssetAtPath<PlayerID>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("Level Importer", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        // JSON File Selection
        EditorGUILayout.LabelField("JSON File", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        jsonFilePath = EditorGUILayout.TextField("File Path", jsonFilePath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFilePanel("Select Level JSON", "", "json");
            if (!string.IsNullOrEmpty(path))
            {
                jsonFilePath = path;
                PreviewJsonFile();
            }
        }
        EditorGUILayout.EndHorizontal();

        // Drag and drop area
        EditorGUILayout.Space(5);
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag & Drop JSON File Here");

        Event evt = Event.current;
        if (dropArea.Contains(evt.mousePosition))
        {
            switch (evt.type)
            {
                case EventType.DragUpdated:
                    if (DragAndDrop.paths.Length > 0 && DragAndDrop.paths[0].EndsWith(".json"))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                    evt.Use();
                    break;

                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();
                    if (DragAndDrop.paths.Length > 0)
                    {
                        jsonFilePath = DragAndDrop.paths[0];
                        PreviewJsonFile();
                    }
                    evt.Use();
                    break;
            }
        }

        EditorGUILayout.Space(10);

        // Level Info
        EditorGUILayout.LabelField("Level Info", EditorStyles.boldLabel);
        levelName = EditorGUILayout.TextField("Level Name", levelName);

        EditorGUILayout.BeginHorizontal();
        worldNumber = EditorGUILayout.IntField("World", worldNumber);
        levelNumber = EditorGUILayout.IntField("Level", levelNumber);
        subLevelNumber = EditorGUILayout.IntField("Sub-Level", subLevelNumber);
        EditorGUILayout.EndHorizontal();

        // Preview level number format
        string previewFormat = GetLevelNumberString();
        EditorGUILayout.LabelField("Preview: " + previewFormat + levelName, EditorStyles.miniLabel);

        EditorGUILayout.Space(10);

        // Required References
        EditorGUILayout.LabelField("Required References", EditorStyles.boldLabel);
        objData = (AllObjectData)EditorGUILayout.ObjectField("All Object Data", objData, typeof(AllObjectData), false);
        startingStats = (PlayerStartingStatsForLevels)EditorGUILayout.ObjectField("Starting Stats", startingStats, typeof(PlayerStartingStatsForLevels), false);
        playerID = (PlayerID)EditorGUILayout.ObjectField("Player ID", playerID, typeof(PlayerID), false);

        EditorGUILayout.Space(10);

        // Preview Section
        if (previewData != null)
        {
            EditorGUILayout.LabelField("JSON Preview", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Original Name: " + previewData.levelName);
            EditorGUILayout.LabelField("Spawn Steps: " + (previewData.spawnSteps?.Length ?? 0));
            EditorGUILayout.LabelField("Objects: " + (previewData.idList?.Length ?? 0));
            EditorGUILayout.LabelField("Final Step: " + previewData.finalSpawnStep);
            EditorGUILayout.Space(5);
        }

        EditorGUILayout.Space(10);

        // Validation
        bool canCreate = ValidateInput();

        EditorGUI.BeginDisabledGroup(!canCreate);
        if (GUILayout.Button("Create Level", GUILayout.Height(40)))
        {
            CreateLevel();
        }
        EditorGUI.EndDisabledGroup();

        if (!canCreate)
        {
            EditorGUILayout.HelpBox(GetValidationMessage(), MessageType.Warning);
        }

        EditorGUILayout.EndScrollView();
    }

    private void PreviewJsonFile()
    {
        if (string.IsNullOrEmpty(jsonFilePath) || !File.Exists(jsonFilePath))
        {
            previewData = null;
            return;
        }

        try
        {
            string jsonContent = File.ReadAllText(jsonFilePath);
            previewData = JsonUtility.FromJson<LevelDataSave>(jsonContent);

            // Auto-fill level name from JSON if empty
            if (string.IsNullOrEmpty(levelName) && previewData != null && !string.IsNullOrEmpty(previewData.levelName))
            {
                levelName = previewData.levelName;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse JSON: " + e.Message);
            previewData = null;
        }

        Repaint();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrEmpty(jsonFilePath) || !File.Exists(jsonFilePath))
            return false;
        if (string.IsNullOrEmpty(levelName))
            return false;
        if (worldNumber <= 0 || levelNumber <= 0)
            return false;
        if (objData == null || startingStats == null || playerID == null)
            return false;
        if (previewData == null)
            return false;

        return true;
    }

    private string GetValidationMessage()
    {
        if (string.IsNullOrEmpty(jsonFilePath) || !File.Exists(jsonFilePath))
            return "Please select a valid JSON file.";
        if (previewData == null)
            return "Failed to parse JSON file. Make sure it's a valid level file.";
        if (string.IsNullOrEmpty(levelName))
            return "Please enter a level name.";
        if (worldNumber <= 0 || levelNumber <= 0)
            return "World and Level numbers must be greater than 0.";
        if (objData == null)
            return "Please assign All Object Data reference.";
        if (startingStats == null)
            return "Please assign Starting Stats reference.";
        if (playerID == null)
            return "Please assign Player ID reference.";

        return "";
    }

    private string GetLevelNumberString()
    {
        if (subLevelNumber <= 0)
            return $"{worldNumber:00}-{levelNumber:00}_";
        else
            return $"{worldNumber:00}-{levelNumber:00}-{subLevelNumber:00}_";
    }

    private void CreateLevel()
    {
        Vector3Int numbers = new Vector3Int(worldNumber, levelNumber, subLevelNumber);

        // Read and parse the JSON file
        string jsonContent = File.ReadAllText(jsonFilePath);
        LevelDataSave lds = JsonUtility.FromJson<LevelDataSave>(jsonContent);

        if (lds == null)
        {
            EditorUtility.DisplayDialog("Error", "Failed to parse JSON file.", "OK");
            return;
        }

        // Update the level name in the save data
        lds.levelName = levelName;

        // Create ScriptableObjects
        LevelData asset = ScriptableObject.CreateInstance<LevelData>();
        LevelChallenges challenges = ScriptableObject.CreateInstance<LevelChallenges>();
        challenges.Editor_SetChallenges();

        string numberString = LevelDataConverter.GetLevelNumberStringFormat(numbers);

        LevelDataArrays arrayAsset = ScriptableObject.CreateInstance<LevelDataArrays>();
        string arrayPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/Resources/" + numberString + levelName + "_DataArrays.asset");
        AssetDatabase.CreateAsset(arrayAsset, arrayPath);

        string levelPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/Main/" + numberString + levelName + ".asset");
        string challengePath = AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/Challenges/" + numberString + levelName + "_Challenge.asset");
        AssetDatabase.CreateAsset(asset, levelPath);
        AssetDatabase.CreateAsset(challenges, challengePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Load the data
        asset.SetDefaults(objData, startingStats, playerID, challenges);
        asset.LoadLevelSaveData(lds);
        arrayAsset.LoadLevelSaveData(lds);
        asset.LevelName = levelName;
        asset.levelWorldAndNumber = numbers;

        EditorUtility.SetDirty(asset);
        EditorUtility.SetDirty(challenges);
        EditorUtility.SetDirty(arrayAsset);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Show success message
        EditorUtility.DisplayDialog("Success",
            $"Level '{levelName}' created successfully!\n\n" +
            $"LevelData: {levelPath}\n" +
            $"LevelDataArrays: {arrayPath}\n" +
            $"LevelChallenges: {challengePath}",
            "OK");

        // Select the created asset
        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);

        // Clear the form for next import
        jsonFilePath = "";
        levelName = "";
        previewData = null;
        levelNumber++;
    }
}
