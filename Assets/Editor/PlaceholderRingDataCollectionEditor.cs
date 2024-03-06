using UnityEditor;
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
    }
    
}
