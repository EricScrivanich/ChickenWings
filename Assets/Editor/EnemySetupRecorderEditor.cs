using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(SetupParent))]
public class EnemySetupRecorderEditor : Editor
{
    private int selectedTriggerIndex = -1;
    private enum ShowButtons
    {
        None,
        ShowHere1,
        ShowHere2
    }

    private enum ParentButtons
    {
        None,
        ParentButton1,
        ParentButton2
    }

    private ShowButtons currentShowButtons = ShowButtons.None;
    private ParentButtons currentParentButtons = ParentButtons.None;

    private Color recordButtonColor = new Color(0.9528f, 0.4179f, 0.4179f, 1f);

    private Color lastSelectedTriggerColor = new Color(.591f, 1f, .542f, 1f);

    public override void OnInspectorGUI()
    {
        SetupParent script = (SetupParent)target;

        base.OnInspectorGUI(); // Draw the default inspector
        if (script.enemySetup == null)
        {
            Debug.LogError("EnemySetup list is null BUDDDDYYYY.");
        }

        if (GUILayout.Button("Clear Record Area"))
        {
            ClearRecordArea();
        }

        // if (GUILayout.Button("Record For Trigger"))
        // {
        //     RecordForTrigger(script);
        // }

        if (GUILayout.Button("Place Setup In Scene"))
        {
            // PlaceSetupInScene(script);
        }

        DrawParentButtons(script); // Draw the parent buttons
                                   // Draw the show buttons based on parent button selection
    }


    private void RecordForTrigger(SetupParent script, int trigger)
    {
        if (script == null)
        {
            Debug.LogError("SetupParent script is null.");
            return;
        }

        if (script.enemySetup == null)
        {
            Debug.LogError("EnemySetup list is null.");
            script.enemySetup = new List<EnemyDataArray>();
        }

        // Automatically find the GameObject named "SetupRecorder"
        GameObject recorder = GameObject.Find("SetupRecorderEnemy");

        if (recorder == null)
        {
            Debug.LogWarning("SetupRecorder GameObject not found. Please ensure it exists in the current scene.");
            return;
        }


        // Temporary list to hold new data
        List<EnemyData> newEnemyData = new List<EnemyData>();

        RecordJetPackPigs(recorder, newEnemyData);
        RecordNormalPigs(recorder, newEnemyData);
        RecordTenderizerPigs(recorder, newEnemyData);

        // Convert the list to an array and add to SetupParent
        script.RecordForEnemyTrigger(newEnemyData, trigger);
        EditorUtility.SetDirty(script);

        selectedTriggerIndex = trigger;


    }
    private void RecordJetPackPigs(GameObject recorder, List<EnemyData> newEnemyData)
    {
        var jetPackPigsRecorder = recorder.GetComponentsInChildren<JetPackPigMovement>();
        if (jetPackPigsRecorder.Length > 0)
        {
            JetPackPigSO newJetPackPigSO = new JetPackPigSO
            {
                data = new JetPackPigData[jetPackPigsRecorder.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < jetPackPigsRecorder.Length; i++)
            {
                newJetPackPigSO.data[i] = new JetPackPigData
                {
                    position = jetPackPigsRecorder[i].transform.position,
                    scale = jetPackPigsRecorder[i].transform.localScale,
                    speed = jetPackPigsRecorder[i].speed
                };

                // Calculate time to reach x = 7 or x = -7
                float distanceToTarget = jetPackPigsRecorder[i].speed > 0 ? 7f - jetPackPigsRecorder[i].transform.position.x : -7f - jetPackPigsRecorder[i].transform.position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / jetPackPigsRecorder[i].speed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger
            newJetPackPigSO.TimeToTrigger = timesToTrigger.Max();

            // Add the new instance to the temporary list
            newEnemyData.Add(newJetPackPigSO);
        }
    }

    private void RecordNormalPigs(GameObject recorder, List<EnemyData> newEnemyData)
    {
        var normalPigsRecorder = recorder.GetComponentsInChildren<PigMovementBasic>();
        if (normalPigsRecorder.Length > 0)
        {
            NormalPigSO newNormalPigSO = new NormalPigSO
            {
                data = new NormalPigData[normalPigsRecorder.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < normalPigsRecorder.Length; i++)
            {
                newNormalPigSO.data[i] = new NormalPigData
                {
                    position = normalPigsRecorder[i].transform.position,
                    scale = normalPigsRecorder[i].transform.localScale,
                    speed = normalPigsRecorder[i].xSpeed
                };

                // Calculate time to reach x = 7 or x = -7
                float distanceToTarget = newNormalPigSO.data[i].speed > 0 ? 7f - newNormalPigSO.data[i].position.x : -7f - newNormalPigSO.data[i].position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / newNormalPigSO.data[i].speed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger
            newNormalPigSO.TimeToTrigger = timesToTrigger.Max();

            // Add the new instance to the temporary list
            newEnemyData.Add(newNormalPigSO);
        }
    }

    private void RecordTenderizerPigs(GameObject recorder, List<EnemyData> newEnemyData)
    {
        var tenderizerPigsRecorder = recorder.GetComponentsInChildren<TenderizerPig>();
        if (tenderizerPigsRecorder.Length > 0)
        {
            TenderizerPigSO newTenderizerPigSO = new TenderizerPigSO
            {
                data = new TenderizerPigData[tenderizerPigsRecorder.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < tenderizerPigsRecorder.Length; i++)
            {
                newTenderizerPigSO.data[i] = new TenderizerPigData
                {
                    position = tenderizerPigsRecorder[i].transform.position,
                    scale = tenderizerPigsRecorder[i].transform.localScale,
                    speed = tenderizerPigsRecorder[i].speed,
                    hasHammer = tenderizerPigsRecorder[i].hasHammer
                };

                // Calculate time to reach x = 7 or x = -7
                float distanceToTarget = newTenderizerPigSO.data[i].speed > 0 ? 7f - newTenderizerPigSO.data[i].position.x : -7f - newTenderizerPigSO.data[i].position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / newTenderizerPigSO.data[i].speed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger
            newTenderizerPigSO.TimeToTrigger = timesToTrigger.Max();

            // Add the new instance to the temporary list
            newEnemyData.Add(newTenderizerPigSO);
        }
    }


    private void ClearRecordArea()
    {
        GameObject recordedOutput = GameObject.Find("SetupRecorderEnemy");
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
    // private void PlaceSetupInScene(SetupParent script)
    // {
    //     GameObject recorderOutput = GameObject.Find("SetupRecorderEnemy");

    //     if (recorderOutput != null && script != null)
    //     {
    //         Transform recorderTransform = recorderOutput.transform;
    //         script.PlaceSetup(recorderTransform);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Recorder Output GameObject or SpawnSetupCollectable script not found!");
    //     }
    // }
    private void DrawShowButtons(EnemyData[] enemyDataArray, int trigger, SetupParent script)
    {
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        var originalColor = GUI.backgroundColor;
        GUI.backgroundColor = recordButtonColor;

        if (GUILayout.Button("Record Trigger"))
        {
            RecordForTrigger(script, trigger);
        }
        GUI.backgroundColor = originalColor;
        if (GUILayout.Button("Place Trigger"))
        {
            PlaceSetup(enemyDataArray);
        }

        // Reset button color

        EditorGUILayout.EndHorizontal();

        foreach (var enemyData in enemyDataArray)
        {
            int index = -1;
            GUILayout.Space(5);
            string buttonTitle = "";

            if (enemyData is JetPackPigSO)
            {
                index = 1;
                buttonTitle = "JetPackPigs";
            }
            else if (enemyData is NormalPigSO)
            {
                index = 0;
                buttonTitle = "NormalPigs";
            }
            else if (enemyData is TenderizerPigSO)
            {
                index = 2;
                buttonTitle = "TenderizerPigs";
            }

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = recordButtonColor;
            if (GUILayout.Button("R", GUILayout.Width(75))) // Adjust width as needed
            {
                RecordSpecificEnemy(index, trigger, script);
            }

            float newTimeToTrigger = EditorGUILayout.FloatField(enemyData.TimeToTrigger, GUILayout.Width(100)); // Adjust width as needed
            if (newTimeToTrigger != enemyData.TimeToTrigger)
            {
                enemyData.TimeToTrigger = newTimeToTrigger;
                EditorUtility.SetDirty(script); // Mark scriptable object as dirty to ensure changes are saved
            }

            GUI.backgroundColor = originalColor;

            GUILayout.FlexibleSpace();
            if (GUILayout.Button(buttonTitle, GUILayout.Width(100))) // Adjust width as needed
            {
                if (currentShowButtons.ToString() == buttonTitle)
                {
                    currentShowButtons = ShowButtons.None; // Close if already open
                }
                else
                {

                    PlaceEnemies(enemyData);
                }
            }
            GUILayout.EndHorizontal();

            if (currentShowButtons.ToString() == buttonTitle)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("testBut1", GUILayout.Width(100)))
                {
                    Debug.Log($"testBut1 clicked under {buttonTitle}");
                }
                if (GUILayout.Button("testBut2", GUILayout.Width(100)))
                {
                    Debug.Log($"testBut2 clicked under {buttonTitle}");
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        GUILayout.Space(15);
    }

    private void DrawParentButtons(SetupParent script)
    {
        var originalColor = GUI.backgroundColor;
        for (int i = 0; i < script.enemySetup.Count; i++)
        {
            GUILayout.Space(15);
            if (selectedTriggerIndex == i)
            {

                GUI.backgroundColor = lastSelectedTriggerColor;
            }
            else
            {
                GUI.backgroundColor = originalColor;
            }
            if (GUILayout.Button($"Enemy Trigger: {i}", GUILayout.Height(50)))
            {
                if (selectedTriggerIndex == i)
                {
                    selectedTriggerIndex = -1; // Deselect if already selected
                }
                else
                {
                    selectedTriggerIndex = i;
                }
            }

            if (selectedTriggerIndex == i)
            {
                GUI.backgroundColor = originalColor;


                DrawShowButtons(script.enemySetup[i].dataArray, i, script); // Draw ShowHere buttons under the corresponding trigger
            }
        }
    }


    public void RecordSpecificEnemy(int index, int trigger, SetupParent script)
    {
        List<EnemyData> tempList = new List<EnemyData>();
        GameObject recorder = GameObject.Find("SetupRecorderEnemy");

        if (index == 0)
        {
            RecordNormalPigs(recorder, tempList);
            script.RecordSpecificEnemy(tempList[0], trigger);

        }
        else if (index == 1)
        {
            RecordJetPackPigs(recorder, tempList);
            script.RecordSpecificEnemy(tempList[0], trigger);

        }
        else if (index == 2)
        {
            RecordTenderizerPigs(recorder, tempList);
            script.RecordSpecificEnemy(tempList[0], trigger);

        }

    }
    private void PlaceSetup(EnemyData[] data)
    {
        SetupPlacer output = GameObject.Find("SetupRecorderEnemy").GetComponent<SetupPlacer>();
        ClearRecordArea();
        output.PlaceSetup(data);
        EditorSceneManager.MarkSceneDirty(output.gameObject.scene);
    }
    private void PlaceEnemies(EnemyData data)
    {
        SetupPlacer output = GameObject.Find("SetupRecorderEnemy").GetComponent<SetupPlacer>();
        ClearRecordArea();
        output.PlaceEnemies(data);
        EditorSceneManager.MarkSceneDirty(output.gameObject.scene);
    }
}