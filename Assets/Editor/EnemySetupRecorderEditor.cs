using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine;
[CustomEditor(typeof(SpawnSetup))]
public class EnemySetupRecorderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector

        SpawnSetup script = (SpawnSetup)target;
        if (GUILayout.Button("Record For Trigger"))
        {
            script.enemySetups = new EnemyData[0];
            // Automatically find the GameObject named "SetupRecorder"
            GameObject recorder = GameObject.Find("SetupRecorder");

            if (recorder != null)
            {
                // Use the found GameObject to record placeholders
                var jetPackPigsRecorder = recorder.GetComponentsInChildren<JetPackPigMovement>();
                if (jetPackPigsRecorder.Length > 0)
                {
                    JetPackPigSO newJetPackPigSO = CreateInstance<JetPackPigSO>();
                    newJetPackPigSO.data = new JetPackPigData[jetPackPigsRecorder.Length];

                    for (int i = 0; i < jetPackPigsRecorder.Length; i++)
                    {
                        newJetPackPigSO.data[i] = new JetPackPigData
                        {
                            position = jetPackPigsRecorder[i].transform.position,
                            scale = jetPackPigsRecorder[i].transform.localScale,
                            speed = jetPackPigsRecorder[i].speed
                        };
                    }

                    // Add the new instance to the enemySetups array
                    ArrayUtility.Add(ref script.enemySetups, newJetPackPigSO);
                    AssetDatabase.AddObjectToAsset(newJetPackPigSO, script); // Add the new instance to the SpawnSetup asset
                    EditorUtility.SetDirty(script); // Marks the object as dirty to ensure the changes are saved
                }
                else
                {
                    Debug.LogWarning("No JetPackPigMovement components found under SetupRecorder.");
                }

                var tenderizerPigsRecorder = recorder.GetComponentsInChildren<TenderizerPig>();
                if (tenderizerPigsRecorder.Length > 0)
                {
                    TenderizerPigSO newTenderizerPigSO = CreateInstance<TenderizerPigSO>();
                    newTenderizerPigSO.data = new TenderizerPigData[tenderizerPigsRecorder.Length];

                    for (int i = 0; i < tenderizerPigsRecorder.Length; i++)
                    {
                        newTenderizerPigSO.data[i] = new TenderizerPigData
                        {
                            position = tenderizerPigsRecorder[i].transform.position,
                            scale = tenderizerPigsRecorder[i].transform.localScale,
                            speed = tenderizerPigsRecorder[i].speed,
                            hasHammer= tenderizerPigsRecorder[i].hasHammer
                        };
                    }

                    // Add the new instance to the enemySetups array
                    ArrayUtility.Add(ref script.enemySetups, newTenderizerPigSO);
                    AssetDatabase.AddObjectToAsset(newTenderizerPigSO, script); // Add the new instance to the SpawnSetup asset
                    EditorUtility.SetDirty(script); // Marks the object as dirty to ensure the changes are saved
                }
                else
                {
                    Debug.LogWarning("No JetPackPigMovement components found under SetupRecorder.");
                }
            }
            else
            {
                Debug.LogWarning("SetupRecorder GameObject not found. Please ensure it exists in the current scene.");
            }
        }
    }

       
}
