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

        if (GUILayout.Button("Clear Entire Record Area", GUILayout.Height(40)))
        {
            ClearCollectableRecordArea();
            ClearEnemyRecordArea();
        }
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear Enemy Record Area", GUILayout.Height(30)))
        {
            ClearEnemyRecordArea();
        }
        if (GUILayout.Button("Clear Collectable Record Area", GUILayout.Height(30)))
        {
            ClearCollectableRecordArea();
        }


        EditorGUILayout.EndHorizontal();

        // if (GUILayout.Button("Record For Trigger"))
        // {
        //     RecordForTrigger(script);
        // }



        DrawParentButtons(script); // Draw the parent buttons
                                   // Draw the show buttons based on parent button selection
    }


    private void RecordForEnemyTrigger(SetupParent script, int trigger)
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

    private void RecordForCollectableTrigger(SetupParent script, int trigger)
    {
        if (script == null)
        {
            Debug.LogError("SetupParent script is null.");
            return;
        }

        if (script.collectableSetup == null)
        {
            Debug.LogError("EnemySetup list is null.");
            script.collectableSetup = new List<CollectableDataArray>();
        }

        // Automatically find the GameObject named "SetupRecorder"
        GameObject recorder = GameObject.Find("SetupRecorderCollectable");

        if (recorder == null)
        {
            Debug.LogWarning("SetupRecorder GameObject not found. Please ensure it exists in the current scene.");
            return;
        }


        // Temporary list to hold new data
        List<CollectableData> newCollectableData = new List<CollectableData>();

        RecordRings(recorder, newCollectableData);

        // Convert the list to an array and add to SetupParent
        script.RecordForColletableTrigger(newCollectableData, trigger);
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
        var targets = recorder.GetComponentsInChildren<TenderizerPig>();
        if (targets.Length > 0)
        {
            TenderizerPigSO newClass = new TenderizerPigSO
            {
                data = new TenderizerPigData[targets.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < targets.Length; i++)
            {
                newClass.data[i] = new TenderizerPigData
                {
                    position = targets[i].transform.position,
                    scale = targets[i].transform.localScale,
                    speed = targets[i].speed,
                    hasHammer = targets[i].hasHammer
                };

                // Calculate time to reach x = 7 or x = -7
                float distanceToTarget = newClass.data[i].speed > 0 ? 7f - newClass.data[i].position.x : -7f - newClass.data[i].position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / newClass.data[i].speed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger
            newClass.TimeToTrigger = timesToTrigger.Max();

            // Add the new instance to the temporary list
            newEnemyData.Add(newClass);
        }
    }


    private void RecordRings(GameObject recorder, List<CollectableData> newCollectableData)
    {
        var targets = recorder.GetComponentsInChildren<RingMovement>();
        if (targets.Length > 0)
        {
            RingSO newClass = new RingSO
            {
                data = new RingData[targets.Length],

            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < targets.Length; i++)
            {
                newClass.data[i] = new RingData
                {
                    position = targets[i].transform.position,
                    rotation = targets[i].transform.rotation,
                    scale = targets[i].transform.localScale,
                    speed = targets[i].speed,

                };

                // Calculate time to reach x = 7 or x = -7
                float distanceToTarget = newClass.data[i].speed > 0 ? 7f - newClass.data[i].position.x : -7f - newClass.data[i].position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / newClass.data[i].speed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger
            newClass.TimeToTrigger = timesToTrigger.Max();
            newClass.isRings = true;

            // Add the new instance to the temporary list
            newCollectableData.Add(newClass);
        }
    }


    private void ClearEnemyRecordArea()
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

    private void ClearCollectableRecordArea()
    {
        GameObject recordedOutput = GameObject.Find("SetupRecorderCollectable");
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
    private void DrawParentButtons(SetupParent script)
    {
        var originalColor = GUI.backgroundColor;
        for (int i = 0; i < Mathf.Max(script.enemySetup.Count, script.collectableSetup.Count); i++)
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

            if (GUILayout.Button($"Trigger {i}", GUILayout.Height(50)))
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
                EditorGUILayout.BeginHorizontal();

                GUI.backgroundColor = recordButtonColor;

                if (GUILayout.Button("Record Trigger", GUILayout.Height(40)))
                {
                    RecordForEnemyTrigger(script, i);
                    RecordForCollectableTrigger(script, i);
                }

                GUI.backgroundColor = originalColor;

                if (GUILayout.Button("Place Trigger", GUILayout.Height(40)))
                {

                    PlaceCollectableSetup(script.collectableSetup[i].dataArray);
                    PlaceEnemySetup(script.enemySetup[i].dataArray);

                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth / 2 - 10));
                GUILayout.Label($"Enemy Trigger: {i} -  (Count {script.enemySetup[i].dataArray.Length})");
                if (i < script.enemySetup.Count)
                {
                    DrawShowButtons(script.enemySetup[i].dataArray, null, i, script); // Draw ShowHere buttons under the corresponding trigger
                }
                else
                {
                    GUILayout.Label($"Empty Enemy Trigger: {i}");
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth / 2 - 10));
                GUILayout.Label($"Collectable Trigger: {i} -  (Count {script.collectableSetup[i].dataArray.Length})");
                if (i < script.collectableSetup.Count)
                {
                    DrawShowButtons(null, script.collectableSetup[i].dataArray, i, script); // Draw ShowHere buttons under the corresponding trigger
                }
                else
                {
                    GUILayout.Label($"Empty Collectable Trigger: {i}");
                }
                GUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void DrawShowButtons(EnemyData[] enemyDataArray, CollectableData[] collectableDataArray, int trigger, SetupParent script)
    {
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        var originalColor = GUI.backgroundColor;
        GUI.backgroundColor = recordButtonColor;

        if (GUILayout.Button("Record Trigger", GUILayout.Width(100)))
        {
            if (enemyDataArray != null)
            {
                RecordForEnemyTrigger(script, trigger);
            }
            else if (collectableDataArray != null)
            {
                RecordForCollectableTrigger(script, trigger);
            }
        }

        GUI.backgroundColor = originalColor;
        if (GUILayout.Button("Place Trigger", GUILayout.Width(100)))
        {
            if (collectableDataArray != null)
            {
                PlaceCollectableSetup(collectableDataArray);

            }
            else if (enemyDataArray != null)
            {
                PlaceEnemySetup(enemyDataArray);
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(15);

        if (enemyDataArray != null)
        {
            foreach (var enemyData in enemyDataArray)
            {
                int index = -1;
                GUILayout.Space(5);
                string buttonTitle = "";

                if (enemyData is JetPackPigSO jetPackPigData)
                {
                    index = 1;
                    buttonTitle = $"JetPackPigs ({jetPackPigData.data.Length})";
                }
                else if (enemyData is NormalPigSO normalPigData)
                {
                    index = 0;
                    buttonTitle = $"NormalPigs ({normalPigData.data.Length})";
                }
                else if (enemyData is TenderizerPigSO tenderizerPigData)
                {
                    index = 2;
                    buttonTitle = $"TenderizerPigs ({tenderizerPigData.data.Length})";
                }

                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = recordButtonColor;
                if (GUILayout.Button("R", GUILayout.Width(25))) // Adjust width as needed
                {
                    RecordSpecificEnemy(index, trigger, script);
                }
                GUI.backgroundColor = originalColor;

                float newTimeToTrigger = EditorGUILayout.FloatField(enemyData.TimeToTrigger, GUILayout.Width(60)); // Adjust width as needed
                if (newTimeToTrigger != enemyData.TimeToTrigger)
                {
                    enemyData.TimeToTrigger = newTimeToTrigger;
                    EditorUtility.SetDirty(script); // Mark scriptable object as dirty to ensure changes are saved
                }



                GUILayout.Space(20);

                if (GUILayout.Button(buttonTitle, GUILayout.Width(120))) // Adjust width as needed
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
                EditorGUILayout.EndHorizontal();
            }
        }
        else if (collectableDataArray != null)
        {
            foreach (var collData in collectableDataArray)
            {
                int index = -1;
                GUILayout.Space(5);
                string buttonTitle = "";

                if (collData is RingSO data)
                {
                    index = 0;
                    buttonTitle = $"Rings ({data.data.Length})";
                }

                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = recordButtonColor;
                if (GUILayout.Button("R", GUILayout.Width(25))) // Adjust width as needed
                {
                    RecordSpecificCollectable(index, trigger, script);
                }

                float newTimeToTrigger = EditorGUILayout.FloatField(collData.TimeToTrigger, GUILayout.Width(60)); // Adjust width as needed
                if (newTimeToTrigger != collData.TimeToTrigger)
                {
                    collData.TimeToTrigger = newTimeToTrigger;
                    EditorUtility.SetDirty(script); // Mark scriptable object as dirty to ensure changes are saved
                }

                GUI.backgroundColor = originalColor;

                if (GUILayout.Button(buttonTitle, GUILayout.Width(90))) // Adjust width as needed
                {
                    if (currentShowButtons.ToString() == buttonTitle)
                    {
                        currentShowButtons = ShowButtons.None; // Close if already open
                    }
                    else
                    {
                        PlaceCollectables(collData);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        GUILayout.Space(15);
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

    public void RecordSpecificCollectable(int index, int trigger, SetupParent script)
    {
        List<CollectableData> tempList = new List<CollectableData>();
        GameObject recorder = GameObject.Find("SetupRecorderCollectable");

        if (index == 0)
        {
            RecordRings(recorder, tempList);
            script.RecordSpecificCollectable(tempList[0], trigger);

        }

    }
    private void PlaceEnemySetup(EnemyData[] data)
    {
        SetupPlacer output = GameObject.Find("SetupRecorderParent").GetComponent<SetupPlacer>();
        ClearEnemyRecordArea();
        output.PlaceEnemySetup(data);
        EditorSceneManager.MarkSceneDirty(output.gameObject.scene);
    }
    private void PlaceEnemies(EnemyData data)
    {
        SetupPlacer output = GameObject.Find("SetupRecorderParent").GetComponent<SetupPlacer>();
        ClearEnemyRecordArea();

        output.PlaceEnemies(data);
        EditorSceneManager.MarkSceneDirty(output.gameObject.scene);
    }

    private void PlaceCollectableSetup(CollectableData[] data)
    {
        SetupPlacer output = GameObject.Find("SetupRecorderParent").GetComponent<SetupPlacer>();
        ClearCollectableRecordArea();
        output.PlaceColllectableSetup(data);
        EditorSceneManager.MarkSceneDirty(output.gameObject.scene);
    }
    private void PlaceCollectables(CollectableData data)
    {
        SetupPlacer output = GameObject.Find("SetupRecorderParent").GetComponent<SetupPlacer>();
        ClearCollectableRecordArea();


        output.PlaceColletables(data);
        EditorSceneManager.MarkSceneDirty(output.gameObject.scene);
    }
}