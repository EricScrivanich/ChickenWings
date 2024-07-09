using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine;

[CustomEditor(typeof(PlaceholderDataCollection))]
public class PlaceholderDataCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector

        PlaceholderDataCollection script = (PlaceholderDataCollection)target;

        EditorGUILayout.PrefixLabel("Specified Trigger");
        EditorGUILayout.BeginHorizontal();
        // IntField for showing and editing the integer value
        // If the '-' button is pressed, decrement the value
        if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(18))) // Width and Height are optional for button size
        {
            script.SpecifiedTrigger = Mathf.Max(0, script.SpecifiedTrigger - 1); // Prevents the value from going below 0
            EditorUtility.SetDirty(target); // Marks the target object as dirty to ensure the change is saved
        }

        // If the '+' button is pressed, increment the value
        if (GUILayout.Button("+", GUILayout.Width(25), GUILayout.Height(18))) // Width and Height are optional for button size
        {
            script.SpecifiedTrigger += 1;
            EditorUtility.SetDirty(target); // Marks the target object as dirty to ensure the change is saved
        }

        script.SpecifiedTrigger = EditorGUILayout.IntField("", script.SpecifiedTrigger);


        // End the horizontal group
        EditorGUILayout.EndHorizontal();


        GUILayout.Space(10);



        if (GUILayout.Button("Place Specific Placeholders"))
        {
            script.PlaceSpecifiedTrigger();
            EditorUtility.SetDirty(target); // Ensure changes are saved
        }
        GUILayout.Space(5);


        if (GUILayout.Button("Place all Placeholders"))
        {
            script.PlacePrefabsInScene();
            EditorUtility.SetDirty(target); // Ensure changes are saved
        }



        GUILayout.Space(25);

        if (GUILayout.Button("Record Specified Placeholders"))
        {
            // Automatically find the GameObject named "PlaceholderRecorder"
            GameObject ringRecorder = GameObject.Find("PlaceholderRecorder");

            if (ringRecorder != null)
            {
                // Use the found GameObject to record placeholders
                script.RecordPlaceholdersForSpecifiedTrigger(ringRecorder);
                EditorUtility.SetDirty(target); // Marks the object as dirty to ensure the changes are saved
            }
            else
            {


                Debug.LogWarning("PlaceholderRecorder GameObject not found. Please ensure it exists in the current scene.");
            }
        }
        GUILayout.Space(5);


        if (GUILayout.Button("Record All Placeholders"))
        {

            // Automatically find the GameObject named "PlaceholderRecorder"
            GameObject ringRecorder = GameObject.Find("PlaceholderRecorder");


            if (ringRecorder != null)
            {
                // Use the found GameObject to record placeholders
                script.RecordPlaceholders(ringRecorder);
                EditorUtility.SetDirty(target); // Marks the object as dirty to ensure the changes are saved
            }
            else
            {
                Debug.LogWarning("PlaceholderRecorder GameObject not found. Please ensure it exists in the current scene.");
            }
        }




        GUILayout.Space(20);


        if (GUILayout.Button("Clear All Recorded Output Children"))
        {
            GameObject recordedOutput = GameObject.Find("PlaceholderRecorder");
            if (recordedOutput != null)
            {
                // Clear children
                for (int i = recordedOutput.transform.childCount - 1; i >= 0; i--)
                {
                    // Use DestroyImmediate in the editor, Destroy if this needs to run at runtime
                    GameObject.DestroyImmediate(recordedOutput.transform.GetChild(i).gameObject);
                }
                // Mark scene as dirty to ensure changes are saved
                EditorSceneManager.MarkSceneDirty(recordedOutput.scene);
            }
            else
            {
                Debug.LogWarning("RecordedOutput GameObject not found.");
            }
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Set As Active Prefab"))
        {
            GameObject spawnParent = GameObject.Find("-----------Other------------");
            SetSpawnerActive tempScript = spawnParent.GetComponent<SetSpawnerActive>();
            tempScript.SetSpawnActive();
            GameObject spawner = GameObject.Find("RingSpawner");
            if (spawner == null)
            {
                Debug.LogWarning("Spawner not found.");

            }
            if (!spawner.activeInHierarchy)
            {
                spawner.SetActive(true);

            }
            RingSpawner spawnerScript = spawner.GetComponent<RingSpawner>();
            if (spawnerScript != null)
            {
                // Clear children
                spawnerScript.placeholderDataCollection = script;
                EditorSceneManager.MarkSceneDirty(spawner.scene);
                EditorUtility.SetDirty(spawnerScript); // Marks the object as dirty to ensure the changes are saved


                // Mark scene as dirty to ensure changes are saved

            }
            else
            {
                Debug.LogWarning("Spawner Script not found.");
            }
        }


        GUILayout.Space(20);


        if (GUILayout.Button("Load LevelCreator Layout"))
        {
            string pathToLayout = Path.Combine(Application.dataPath, "Editor/Layouts/LevelCreator.wlt");
            if (File.Exists(pathToLayout))
            {
                EditorUtility.LoadWindowLayout(pathToLayout);
            }
            else
            {
                Debug.LogError("Could not find the LevelCreator layout at path: " + pathToLayout);
            }
        }

    }

}
