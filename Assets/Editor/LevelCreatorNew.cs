using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


[CustomEditor(typeof(LevelData))]
public class LevelCreatorNew : Editor
{
    private LevelData Parent;
    public GameObject recorder;
    [SerializeField] private TutorialData baseTutorialData;

    private bool[] challengeCompletionTest;
    private bool masterLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private string newLevelName;
    private Vector3Int newLevelNumbers;
    private bool dropdownOpen = false;

    private bool randomWaveDropdownOpen = false;
    private bool testCompletionDropdownOpen = false;
    public override void OnInspectorGUI()
    {
        Parent = (LevelData)target;
        recorder = GameObject.Find("SetupRecorderParent");

        base.OnInspectorGUI(); // Draw 


        randomWaveDropdownOpen = EditorGUILayout.Foldout(randomWaveDropdownOpen, "Edit Random Wave Data");
        if (randomWaveDropdownOpen)
        {
            if (GUILayout.Button("Add Random Wave Data", GUILayout.Height(20)))
            {
                string randomWaveNumber = Parent.randomWaveDataArray.Length.ToString("00");
                RandomWaveData randWaveData = ScriptableObject.CreateInstance<RandomWaveData>();
                string p = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/RandomWaveLogic/" + LevelDataConverter.GetLevelNumberStringFormat(Parent.levelWorldAndNumber) + Parent.LevelName + "_RandWave-" + randomWaveNumber + ".asset");
                AssetDatabase.CreateAsset(randWaveData, p);




                UnityEditor.EditorUtility.SetDirty(randWaveData);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();

                List<RandomWaveData> randWaveList = Parent.randomWaveDataArray.ToList();
                randWaveList.Add(randWaveData);
                Parent.randomWaveDataArray = randWaveList.ToArray();

                UnityEditor.EditorUtility.SetDirty(Parent);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();


            }

            if (GUILayout.Button("Copy Random Data", GUILayout.Height(20)))
            {
                Parent.randomWaveDataArray[0].CopyFromDataArray(Parent.ReturnDataArrays());
                UnityEditor.EditorUtility.SetDirty(Parent.randomWaveDataArray[0]);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
            if (GUILayout.Button("Clear Random Data", GUILayout.Height(20)))
            {
                Parent.randomWaveDataArray[0].CopyFromDataArray(null);
                UnityEditor.EditorUtility.SetDirty(Parent.randomWaveDataArray[0]);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }




        }

        // Make this a dropdown that be shown or hidden

        if (Parent.tutorialData == null)
            if (GUILayout.Button("Add Tutorial Data", GUILayout.Height(40)))
            {
                TutorialData asset = ScriptableObject.CreateInstance<TutorialData>();
                asset.bubblePrefab = baseTutorialData.bubblePrefab;
                asset.messagePrefab = baseTutorialData.messagePrefab;




                string numberString = LevelDataConverter.GetLevelNumberStringFormat(Parent.levelWorldAndNumber);

                string path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/TutorialData/" + numberString + Parent.LevelName + "_TutuorialData" + ".asset");
                AssetDatabase.CreateAsset(asset, path);
                UnityEditor.EditorUtility.SetDirty(asset);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
                Parent.tutorialData = asset;
                UnityEditor.EditorUtility.SetDirty(Parent);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();

            }

        dropdownOpen = EditorGUILayout.Foldout(dropdownOpen, "Edit Level Name and Numbers");
        if (dropdownOpen)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("New Level Name and Numbers", EditorStyles.boldLabel);
            newLevelName = EditorGUILayout.TextField("Level Name", newLevelName);
            newLevelNumbers = EditorGUILayout.Vector3IntField("Level Numbers (World, Level, Special)", newLevelNumbers);

            GUILayout.Space(20);
            if (GUILayout.Button("Rename Asset", GUILayout.Height(40)))
            {


                DoAction(false);


            }
            if (GUILayout.Button("Delete Asset", GUILayout.Height(40)))
            {


                DoAction(true);


            }

            EditorGUILayout.EndVertical();
        }
        testCompletionDropdownOpen = EditorGUILayout.Foldout(testCompletionDropdownOpen, "Edit Test Completion For Levels");
        if (testCompletionDropdownOpen)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Set Level Test Completion", EditorStyles.boldLabel);

            var challengeData = Parent.GetLevelChallenges(false, null);

            if (challengeCompletionTest == null)
                challengeCompletionTest = new bool[challengeData.challengeTexts.Length];

            for (int i = 0; i < challengeData.challengeTexts.Length; i++)
            {
                challengeCompletionTest[i] = EditorGUILayout.Toggle(challengeData.challengeTexts[i], challengeCompletionTest[i]);
            }

            masterLevel = EditorGUILayout.Toggle("Is Master Level", masterLevel);


            if (GUILayout.Button("Submit", GUILayout.Height(20)))
            {
                LevelDataConverter.instance.SetLevelCompletionManual(Parent, false, masterLevel, challengeCompletionTest.Clone() as bool[]);
            }
            if (GUILayout.Button("Clear", GUILayout.Height(20)))
            {
                LevelDataConverter.instance.SetLevelCompletionManual(Parent, true, false, null);
            }

            EditorGUILayout.EndVertical();

        }

        if (GUILayout.Button("RedoName", GUILayout.Height(20)))
        {
            newLevelName = Parent.LevelName;
            newLevelNumbers = Parent.levelWorldAndNumber;
            DoAction(false);

        }

        if (GUILayout.Button("Create Data Arrays", GUILayout.Height(20)))
        {
            LevelDataArrays arrayAsset = ScriptableObject.CreateInstance<LevelDataArrays>();
            string path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/Resources/" + LevelDataConverter.GetLevelNumberStringFormat(Parent.levelWorldAndNumber) + Parent.LevelName + "_DataArrays" + ".asset");
            AssetDatabase.CreateAsset(arrayAsset, path);
            arrayAsset.SetData(Parent);
            UnityEditor.EditorUtility.SetDirty(arrayAsset);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();


        }


        // add a string field and vector3 field for level name and numbers


    }

    private string ReturnLevelName(string levelName, Vector3Int numbers, bool display)
    {

        // check if any levels already use numbers
        if (string.IsNullOrEmpty(levelName) || numbers == null || numbers.x <= 0 || numbers.y <= 0 || numbers.z < 0)
        {
            Debug.LogWarning("Invalid level name");
            return null;
        }

        string numberString = LevelDataConverter.GetLevelNumberStringFormat(numbers);



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

    private void DoAction(bool delete)
    {
        string name = newLevelName;
        if (string.IsNullOrEmpty(name))
        {
            newLevelName = Parent.LevelName;
            name = Parent.LevelName;
        }
        Vector3 numbers = newLevelNumbers;
        if (numbers == Vector3.zero)
        {
            newLevelNumbers = Parent.levelWorldAndNumber;
            numbers = Parent.levelWorldAndNumber;
        }

        if (ReturnLevelName(newLevelName, newLevelNumbers, false) != null || delete)
        {


            bool hasTutorialData = Parent.tutorialData != null;
            string newFileName = ReturnLevelName(newLevelName, newLevelNumbers, false);
            string newChallengeFileName = newFileName + "_Challenge";
            string newArrayFileName = newFileName + "_DataArrays";
            string newTutorialFileName = "";
            // rename the asset file
            string currentPath = AssetDatabase.GetAssetPath(Parent.GetInstanceID());
            string currentChallengePath = AssetDatabase.GetAssetPath(Parent.GetLevelChallenges(false, null).GetInstanceID());
            string currentArrayPath = AssetDatabase.GetAssetPath(Parent.ReturnDataArrays().GetInstanceID());
            string currentTutorialPath = "";

            if (string.IsNullOrEmpty(currentPath) || string.IsNullOrEmpty(currentChallengePath) || string.IsNullOrEmpty(currentArrayPath))
            {
                Debug.LogError("Could not find asset path for the ScriptableObject.");
                return;
            }
            if (hasTutorialData)
            {
                currentTutorialPath = AssetDatabase.GetAssetPath(Parent.tutorialData.GetInstanceID());
                if (string.IsNullOrEmpty(currentTutorialPath))
                {
                    Debug.LogError("Could not find asset path for the TutorialData.");
                    return;
                }
                newTutorialFileName = newFileName + "_TutorialData";
            }

            if (delete)
            {
                AssetDatabase.DeleteAsset(currentPath);
                AssetDatabase.DeleteAsset(currentChallengePath);
                AssetDatabase.DeleteAsset(currentArrayPath);
                if (hasTutorialData)
                {
                    AssetDatabase.DeleteAsset(currentTutorialPath);
                }
                return;
            }


            Parent.LevelName = name;
            Parent.levelWorldAndNumber = new Vector3Int((int)numbers.x, (int)numbers.y, (int)numbers.z);
            Debug.Log("Renaming Level: " + Parent.LevelName + " with numbers: " + numbers);
            UnityEditor.EditorUtility.SetDirty(Parent);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            // LevelDataConverter.instance.AddLevel(null); // adding null just so it gets reordered

            AssetDatabase.RenameAsset(currentPath, newFileName);
            AssetDatabase.RenameAsset(currentChallengePath, newChallengeFileName);
            AssetDatabase.RenameAsset(currentArrayPath, newArrayFileName);

            if (hasTutorialData)
            {
                AssetDatabase.RenameAsset(currentTutorialPath, newTutorialFileName);

            }


            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogWarning("Invalid level name or numbers. Please ensure they are set correctly.");
        }
    }

}



public static class JsonDataTools
{
    [MenuItem("Tools/ClearAllJsonData")]
    public static void ClearAllJsonData()
    {
        // Add your clear logic here
        string path = Application.persistentDataPath;

        if (Directory.Exists(path))
        {
            string[] jsonFiles = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
            foreach (string file in jsonFiles)
            {
                try
                {
                    File.Delete(file);
                    Debug.Log("Deleted: " + file);
                }
                catch (IOException e)
                {
                    Debug.LogError("Could not delete file: " + file + "\n" + e.Message);
                }
            }
        }
        else
        {
            Debug.Log("Persistent data directory does not exist.");
        }
        PlayerPrefs.DeleteAll();
    }
}


