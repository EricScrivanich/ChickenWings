using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(PlaceholderRingDataCollection))]
public class PlaceholderRingDataCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector

        PlaceholderRingDataCollection script = (PlaceholderRingDataCollection)target;

        if (GUILayout.Button("Record Placeholders"))
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
        if (GUILayout.Button("Place Prefabs in Scene"))
        {
            script.PlacePrefabsInScene();
            EditorUtility.SetDirty(target); // Ensure changes are saved
        }


        if (GUILayout.Button("Clear RecordedOutput Children"))
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
    }

}
