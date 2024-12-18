using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(RandomSpawnIntensity))]
public class RandomSpawnIntensityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RandomSpawnIntensity script = (RandomSpawnIntensity)target;

        GUILayout.Space(10);
        GUILayout.Label("Main Controls", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create All", GUILayout.Height(40)))
        {
            CreateBoundingBoxes(script);
            EditorUtility.SetDirty(script);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Remove All", GUILayout.Height(40)))
        {
            RemoveBoundingBoxes(script);
            EditorUtility.SetDirty(script);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20); // Space between main controls and pig type controls

        DrawPigControls("Normal Pig", script, "NormalPig_BB", "BB_Normal.asset");
        GUILayout.Space(10); // Space between pig type rows
        DrawPigControls("Jetpack Pig", script, "JetpackPig_BB", "BB_Jetpack.asset");
        GUILayout.Space(10); // Space between pig type rows
        DrawPigControls("Big Pig", script, "BigPig_BB", "BB_BigPig.asset");
        GUILayout.Space(10); // Space between pig type rows
        DrawPigControls("Tenderizer Pig", script, "TenderizerPig_BB", "BB_Tenderizer.asset");


    }

    private void DrawPigControls(string title, RandomSpawnIntensity script, string fieldName, string baseAssetName)
    {
        GUILayout.Label(title, EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset"))
        {
            ResetBoundingBox(script, fieldName, baseAssetName);
            EditorUtility.SetDirty(script);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Remove"))
        {
            RemoveBoundingBox(script, fieldName);
            EditorUtility.SetDirty(script);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void CreateBoundingBoxes(RandomSpawnIntensity script)
    {
        CreateOrReplaceBoundingBox(script, "NormalPig_BB", "BB_Normal.asset");
        CreateOrReplaceBoundingBox(script, "JetpackPig_BB", "BB_Jetpack.asset");
        CreateOrReplaceBoundingBox(script, "BigPig_BB", "BB_BigPig.asset");
        CreateOrReplaceBoundingBox(script, "TenderizerPig_BB", "BB_Tenderizer.asset");
    }

    private void RemoveBoundingBoxes(RandomSpawnIntensity script)
    {
        RemoveBoundingBox(script, "NormalPig_BB");
        RemoveBoundingBox(script, "JetpackPig_BB");
        RemoveBoundingBox(script, "BigPig_BB");
        RemoveBoundingBox(script, "TenderizerPig_BB");
    }

    private void ResetBoundingBox(RandomSpawnIntensity script, string fieldName, string baseAssetName)
    {
        CreateOrReplaceBoundingBox(script, fieldName, baseAssetName);
    }

    private void RemoveBoundingBox(RandomSpawnIntensity script, string fieldName)
    {
        var field = script.GetType().GetField(fieldName);
        if (field == null) return;

        EnemyBoundingBox boundingBox = field.GetValue(script) as EnemyBoundingBox;
        if (boundingBox != null && boundingBox.name.StartsWith($"BB_{script.name}"))
        {
            string assetPath = AssetDatabase.GetAssetPath(boundingBox);
            AssetDatabase.DeleteAsset(assetPath);
        }

        field.SetValue(script, null);
    }

    private void CreateOrReplaceBoundingBox(RandomSpawnIntensity script, string fieldName, string baseAssetName)
    {
        var field = script.GetType().GetField(fieldName);
        if (field == null) return;

        string basePath = $"Assets/_Folders/_Placeholders&Setups/RandomSpawning/Base/{baseAssetName}";
        string destinationFolder = "Assets/_Folders/_Placeholders&Setups/RandomSpawning/LevelBoundingBoxes/";

        string intensityName = script.name;
        bool isValidName = script.name.StartsWith("INT_");

        if (!isValidName)
        {
            Debug.LogError($"Naming convention for {script.name} is off. Bounding boxes will be named with '_NULL_BB-' prefix.");
            intensityName = $"_NULL_BB-{script.name}";
        }

        string newAssetName = $"BB_{intensityName}-{fieldName.Replace("_BB", "")}";
        string newAssetPath = Path.Combine(destinationFolder, $"{newAssetName}.asset");

        EnemyBoundingBox existingBox = field.GetValue(script) as EnemyBoundingBox;
        if (existingBox != null && !existingBox.name.StartsWith($"BB_{script.name}"))
        {
            Debug.Log($"Skipping creation for {fieldName} because it's not generated by this ScriptableObject.");
            return;
        }

        EnemyBoundingBox baseBoundingBox = AssetDatabase.LoadAssetAtPath<EnemyBoundingBox>(basePath);
        if (baseBoundingBox == null)
        {
            Debug.LogError($"Base Bounding Box not found at path: {basePath}");
            return;
        }

        if (AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(baseBoundingBox), newAssetPath))
        {
            EnemyBoundingBox newBoundingBox = AssetDatabase.LoadAssetAtPath<EnemyBoundingBox>(newAssetPath);
            field.SetValue(script, newBoundingBox);
        }
        else
        {
            Debug.LogError($"Failed to copy asset for {fieldName}.");
        }
    }
}