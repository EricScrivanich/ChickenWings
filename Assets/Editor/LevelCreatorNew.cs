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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private string newLevelName;
    private Vector3Int newLevelNumbers;
    private bool dropdownOpen = false;
    public override void OnInspectorGUI()
    {
        Parent = (LevelData)target;
        recorder = GameObject.Find("SetupRecorderParent");

        base.OnInspectorGUI(); // Draw 

        // Make this a dropdown that be shown or hidden

        if (Parent.tutorialData == null)
            if (GUILayout.Button("Add Tutorial Data", GUILayout.Height(40)))
            {
                TutorialData asset = ScriptableObject.CreateInstance<TutorialData>();
                asset.bubblePrefab = baseTutorialData.bubblePrefab;
                asset.messagePrefab = baseTutorialData.messagePrefab;

                string numberString = "";
                Vector3Int numbers = Parent.levelWorldAndNumber;

                if (numbers.z <= 0)
                    numberString = $"{numbers.x:00}-{numbers.y:00}_";
                else
                    numberString = $"{numbers.x:00}-{numbers.y:00}-{numbers.z:00}_";

                string path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/Levels/TutorialData/" + "TutuorialData" + numberString + Parent.LevelName + ".asset");
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
                string name = newLevelName;
                Vector3 numbers = newLevelNumbers;

                if (PickLevelEditorOnly.ReturnLevelName(newLevelName, newLevelNumbers, false) != null)
                {




                    string newFileName = PickLevelEditorOnly.ReturnLevelName(newLevelName, newLevelNumbers, false);
                    // rename the asset file
                    string currentPath = AssetDatabase.GetAssetPath(Parent.GetInstanceID());
                    if (string.IsNullOrEmpty(currentPath))
                    {
                        Debug.LogError("Could not find asset path for the ScriptableObject.");
                        return;
                    }
                    Parent.LevelName = name;
                    Parent.levelWorldAndNumber = new Vector3Int((int)numbers.x, (int)numbers.y, (int)numbers.z);
                    // LevelDataConverter.instance.AddLevel(null); // adding null just so it gets reordered

                    AssetDatabase.RenameAsset(currentPath, newFileName);
                    UnityEditor.EditorUtility.SetDirty(Parent);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogWarning("Invalid level name or numbers. Please ensure they are set correctly.");
                }




            }
            EditorGUILayout.EndVertical();
        }


        // add a string field and vector3 field for level name and numbers


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
    }
}


