using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(RandomSetupParent))]
public class RandomSetupParentRecorder : Editor
{
    public string[] options = new string[] { "Ring", "pNormal", "pBig", "pJetPack", "pTenderizer" };
    public int indexDropDown = 0;
    private int selectedTriggerIndex = -1;
    private int selectedTriggerIndexEnemy = -1;
    private int selectedTriggerIndexCollectable = -1;
    private List<int> selctedEnemyTriggers = new List<int>();
    private List<int> selctedCollTriggers = new List<int>();
    private GameObject recorderEnemy;
    private GameObject recorderCollectable;
    private SetupPlacer placer;
    private RandomSetupParent script;
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
    void RecordForNewType()
    {
        var tempListColl = new List<CollectableData>();
        var tempListEnemy = new List<EnemyData>();
        switch (indexDropDown)
        {
            case 0:
                RecordRings(tempListColl);
                break;
            case 1:
                RecordNormalPigs(tempListEnemy);

                break;
            case 2:
                RecordBigPigs(tempListEnemy);

                break;

            case 3:
                RecordJetPackPigs(tempListEnemy);

                break;

            case 4:
                RecordTenderizerPigs(tempListEnemy);

                break;

            default:
                Debug.LogError("Unrecognized Option");
                break;
        }

        if (indexDropDown == 0 && selectedTriggerIndexCollectable >= 0)
        {
            script.DuplicateOrRemoveCollectable(selectedTriggerIndexCollectable, script.collectableSetup[selectedTriggerIndexCollectable].dataArray.Length - 1, true, tempListColl[0]);
        }
        else if (indexDropDown > 0 && selectedTriggerIndexEnemy >= 0)
        {
            script.DuplicateOrRemoveEnemy(selectedTriggerIndexEnemy, script.enemySetup[selectedTriggerIndexEnemy].dataArray.Length - 1, true, tempListEnemy[0]);
        }





    }
    public override void OnInspectorGUI()
    {
        script = (RandomSetupParent)target;

        base.OnInspectorGUI(); // Draw the default inspector

        if (GameObject.Find("SetupRecorderParent") == null) return;

        recorderEnemy = GameObject.Find("SetupRecorderEnemy");
        recorderCollectable = GameObject.Find("SetupRecorderCollectable");
        placer = GameObject.Find("SetupRecorderParent").GetComponent<SetupPlacer>();

        if (EditorApplication.isPlaying && !EditorApplication.isPaused)
        {
            placer.ShowBorderLines(false);
        }
        else
        {
            placer.ShowBorderLines(true);
        }
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

        GUILayout.Space(10);

        indexDropDown = EditorGUILayout.Popup(indexDropDown, options);
        if (GUILayout.Button("Create New"))
            RecordForNewType();

        // if (GUILayout.Button("Record For Trigger"))
        // {
        //     RecordForTrigger(script);
        // }



        DrawParentButtons(); // Draw the parent buttons
                             // Draw the show buttons based on parent button selection
    }


    private void RecordForEnemyTrigger(int trigger)
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

        RecordJetPackPigs(newEnemyData);
        RecordNormalPigs(newEnemyData);
        RecordTenderizerPigs(newEnemyData);
        RecordBigPigs(newEnemyData);

        // Convert the list to an array and add to SetupParent
        script.RecordForEnemyTrigger(newEnemyData, trigger);
        EditorUtility.SetDirty(script);

        selectedTriggerIndex = trigger;


    }

    private void RecordForCollectableTrigger( int trigger)
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

        RecordRings(newCollectableData);

        // Convert the list to an array and add to SetupParent
        script.RecordForColletableTrigger(newCollectableData, trigger);
        EditorUtility.SetDirty(script);

        selectedTriggerIndex = trigger;


    }
    private void RecordJetPackPigs(List<EnemyData> newEnemyData)
    {
        var jetPackPigsRecorder = recorderEnemy.GetComponentsInChildren<JetPackPigMovement>();
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

    private void RecordNormalPigs(List<EnemyData> newEnemyData)
    {
        var normalPigsRecorder = recorderEnemy.GetComponentsInChildren<PigMovementBasic>();
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

    private void RecordBigPigs(List<EnemyData> newEnemyData)
    {
        var targets = recorderEnemy.GetComponentsInChildren<BigPigMovement>();
        if (targets.Length > 0)
        {
            BigPigSO newClass = new BigPigSO
            {
                data = new BigPigData[targets.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < targets.Length; i++)
            {
                newClass.data[i] = new BigPigData
                {
                    position = targets[i].transform.position,
                    scale = targets[i].transform.localScale,
                    speed = targets[i].speed,
                    yForce = targets[i].yForce,
                    distanceToFlap = targets[i].distanceToFlap
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

    private void RecordTenderizerPigs(List<EnemyData> newEnemyData)
    {
        var targets = recorderEnemy.GetComponentsInChildren<TenderizerPig>();
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


    private void RecordRings(List<CollectableData> newCollectableData)
    {
        var targets = recorderCollectable.GetComponentsInChildren<RingMovement>();

        if (targets.Length > 0)
        {
            // Sort targets based on their distance from 0 on the x-axis
            var sortedTargets = targets.OrderBy(target => Mathf.Abs(target.transform.position.x)).ToArray();

            RingSO newClass = new RingSO
            {
                data = new RingData[sortedTargets.Length],
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < sortedTargets.Length; i++)
            {
                newClass.data[i] = new RingData
                {
                    position = sortedTargets[i].transform.position,
                    rotation = sortedTargets[i].transform.rotation,
                    scale = sortedTargets[i].transform.localScale,
                    speed = sortedTargets[i].speed,
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

    #region ButtonDrawers
    private void DrawParentButtons()
    {
        var originalColor = GUI.backgroundColor;



        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        for (int i = 0; i < script.enemySetup.Count; i++)
        {
            if (selectedTriggerIndexEnemy == i)
            {
                GUI.backgroundColor = lastSelectedTriggerColor;
            }
            else
            {
                GUI.backgroundColor = originalColor;
            }
            if (GUILayout.Button($"Enemy Trigger: {i} -  (Count {script.enemySetup[i].dataArray.Length})", GUILayout.Height(50)))
            {
                if (selctedEnemyTriggers.Contains(i))
                {
                    selctedEnemyTriggers.Remove(i);
                }
                else
                {
                    selctedEnemyTriggers.Add(i);
                    selectedTriggerIndexEnemy = i;
                }

            }

            if (selctedEnemyTriggers.Contains(i))
            {
                EditorGUILayout.BeginHorizontal();

                GUI.backgroundColor = recordButtonColor;

                if (GUILayout.Button("Record Trigger", GUILayout.Height(40)))
                {
                    RecordForEnemyTrigger(i);
                }

                GUI.backgroundColor = originalColor;

                if (GUILayout.Button("Place Trigger", GUILayout.Height(40)))
                {
                    PlaceEnemySetup(script.enemySetup[i].dataArray);
                }

                EditorGUILayout.EndHorizontal();

                DrawShowButtons(script.enemySetup[i].dataArray, null, i);

            }
        }
        GUILayout.EndVertical();

            GUILayout.BeginVertical();
            for (int i = 0; i < script.collectableSetup.Count; i++)
            {
                if (selectedTriggerIndexCollectable == i)
                {
                    GUI.backgroundColor = lastSelectedTriggerColor;
                }
                else
                {
                    GUI.backgroundColor = originalColor;
                }
                if (GUILayout.Button($"Collectable Trigger: {i} -  (Count {script.collectableSetup[i].dataArray.Length})", GUILayout.Height(50)))
                {
                    if (selctedCollTriggers.Contains(i))
                    {
                        selctedCollTriggers.Remove(i);
                    }
                    else
                    {
                        selctedCollTriggers.Add(i);
                        selectedTriggerIndexCollectable = i;
                    }

                }

                if (selctedCollTriggers.Contains(i))
                {
                    EditorGUILayout.BeginHorizontal();

                    GUI.backgroundColor = recordButtonColor;

                    if (GUILayout.Button("Record Trigger", GUILayout.Height(40)))
                    {
                        RecordForCollectableTrigger(i);
                    }

                    GUI.backgroundColor = originalColor;

                    if (GUILayout.Button("Place Trigger", GUILayout.Height(40)))
                    {
                        PlaceCollectableSetup(script.collectableSetup[i].dataArray);
                    }

                    EditorGUILayout.EndHorizontal();

                    DrawShowButtons(null, script.collectableSetup[i].dataArray, i);
                }
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        

    }
    private void DrawShowButtons(EnemyData[] enemyDataArray, CollectableData[] collectableDataArray, int trigger)
    {
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        var originalColor = GUI.backgroundColor;
        GUI.backgroundColor = recordButtonColor;

        if (GUILayout.Button("Record Trigger", GUILayout.Width(100)))
        {
            if (enemyDataArray != null)
            {
                RecordForEnemyTrigger(trigger);
            }
            else if (collectableDataArray != null)
            {
                RecordForCollectableTrigger(trigger);
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

                GUILayout.Space(5);
                string buttonTitle = "";

                if (enemyData is JetPackPigSO jetPackPigData)
                {

                    buttonTitle = $"JetPackPigs ({jetPackPigData.data.Length})";
                }
                else if (enemyData is NormalPigSO normalPigData)
                {
                    buttonTitle = $"NormalPigs ({normalPigData.data.Length})";
                }
                else if (enemyData is TenderizerPigSO tenderizerPigData)
                {
                    buttonTitle = $"TenderizerPigs ({tenderizerPigData.data.Length})";
                }
                else if (enemyData is BigPigSO bigPig)
                {
                    buttonTitle = $"BigPigs ({bigPig.data.Length})";
                }




                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = recordButtonColor;
                if (GUILayout.Button("R", GUILayout.Width(25))) // Adjust width as needed
                {
                    RecordOrDuplicateSpecificEnemy(enemyData, trigger, System.Array.IndexOf(script.enemySetup[trigger].dataArray, enemyData), false);
                }
                GUI.backgroundColor = originalColor;

                if (GUILayout.Button("P", GUILayout.Width(25))) // Adjust width as needed
                {
                    ClearEnemyType(enemyData);
                    PlaceEnemies(enemyData, false);
                }

                if (GUILayout.Button("P&", GUILayout.Width(25))) // Adjust width as needed
                {
                    PlaceEnemies(enemyData, false);
                }

                if (GUILayout.Button("+", GUILayout.Width(25))) // Adjust width as needed
                {
                    ClearEnemyType(enemyData);
                    PlaceEnemies(enemyData, false);
                    RecordOrDuplicateSpecificEnemy(enemyData, trigger, System.Array.IndexOf(script.enemySetup[trigger].dataArray, enemyData), true);


                }
                if (GUILayout.Button("-", GUILayout.Width(25))) // Adjust width as needed
                {
                    script.DuplicateOrRemoveEnemy(trigger, System.Array.IndexOf(script.enemySetup[trigger].dataArray, enemyData), false, null);

                }



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
                        PlaceEnemies(enemyData, true);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        else if (collectableDataArray != null)
        {
            foreach (var collData in collectableDataArray)
            {

                GUILayout.Space(5);
                string buttonTitle = "";

                if (collData is RingSO data)
                {

                    buttonTitle = $"Rings ({data.data.Length})";
                }

                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = recordButtonColor;
                if (GUILayout.Button("R", GUILayout.Width(25))) // Adjust width as needed
                {
                    RecordOrDuplicateSpecificCollectable(collData, trigger, System.Array.IndexOf(script.collectableSetup[trigger].dataArray, collData), false);
                }
                GUI.backgroundColor = originalColor;
                if (GUILayout.Button("P", GUILayout.Width(25))) // Adjust width as needed
                {
                    ClearCollectableType(collData);
                    PlaceCollectables(collData, false);
                }
                if (GUILayout.Button("P&", GUILayout.Width(25))) // Adjust width as needed
                {

                    PlaceCollectables(collData, false);
                }

                if (GUILayout.Button("+", GUILayout.Width(25))) // Adjust width as needed
                {
                    ClearCollectableType(collData);
                    PlaceCollectables(collData, false);

                    RecordOrDuplicateSpecificCollectable(collData, trigger, System.Array.IndexOf(script.collectableSetup[trigger].dataArray, collData), true);



                }
                if (GUILayout.Button("-", GUILayout.Width(25))) // Adjust width as needed
                {


                    script.DuplicateOrRemoveCollectable(trigger, System.Array.IndexOf(script.collectableSetup[trigger].dataArray, collData), false, null);


                }

                if (GUILayout.Button("T", GUILayout.Width(25))) // Adjust width as needed
                {


                    Debug.Log("Index is: " + System.Array.IndexOf(script.collectableSetup[trigger].dataArray, collData));


                }

                float newTimeToTrigger = EditorGUILayout.FloatField(collData.TimeToTrigger, GUILayout.Width(60)); // Adjust width as needed
                if (newTimeToTrigger != collData.TimeToTrigger)
                {
                    collData.TimeToTrigger = newTimeToTrigger;
                    EditorUtility.SetDirty(script); // Mark scriptable object as dirty to ensure changes are saved
                }



                if (GUILayout.Button(buttonTitle, GUILayout.Width(90))) // Adjust width as needed
                {
                    if (currentShowButtons.ToString() == buttonTitle)
                    {
                        currentShowButtons = ShowButtons.None; // Close if already open
                    }
                    else
                    {
                        PlaceCollectables(collData, true);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(15);
    }

    #endregion

    public void RecordOrDuplicateSpecificEnemy(EnemyData dataTypeEnemy, int trigger, int index, bool isDuplicate)
    {
        List<EnemyData> tempList = new List<EnemyData>();


        if (dataTypeEnemy is NormalPigSO so0)
        {
            RecordNormalPigs(tempList);


        }
        else if (dataTypeEnemy is JetPackPigSO so1)
        {
            RecordJetPackPigs(tempList);


        }
        else if (dataTypeEnemy is TenderizerPigSO so2)
        {
            RecordTenderizerPigs(tempList);


        }

        else if (dataTypeEnemy is BigPigSO so3)
        {
            RecordBigPigs(tempList);


        }
        if (isDuplicate)
        {
            script.DuplicateOrRemoveEnemy(trigger, index, true, tempList[0]);
        }
        else
        {
            script.RecordSpecificEnemy(tempList[0], trigger, index);
        }



    }




    public void RecordOrDuplicateSpecificCollectable(CollectableData dataTypeCollectable, int trigger, int index, bool isDuplicate)
    {
        List<CollectableData> tempList = new List<CollectableData>();


        if (dataTypeCollectable is RingSO so)
        {
            RecordRings(tempList);


        }

        if (isDuplicate)
        {
            script.DuplicateOrRemoveCollectable(trigger, index, true, tempList[0]);
        }

        else
        {
            script.RecordSpecificCollectable(tempList[0], trigger, index);
        }

    }
    private void PlaceEnemySetup(EnemyData[] data)
    {
        SetupPlacer output = GameObject.Find("SetupRecorderParent").GetComponent<SetupPlacer>();
        ClearEnemyRecordArea();
        output.PlaceEnemySetup(data);
        EditorSceneManager.MarkSceneDirty(output.gameObject.scene);
    }
    private void PlaceEnemies(EnemyData data, bool clearAll)
    {
        if (clearAll)
            ClearEnemyRecordArea();

        placer.PlaceEnemies(data);
        EditorSceneManager.MarkSceneDirty(placer.gameObject.scene);
    }

    private void PlaceCollectableSetup(CollectableData[] data)
    {
        SetupPlacer output = GameObject.Find("SetupRecorderParent").GetComponent<SetupPlacer>();
        ClearCollectableRecordArea();
        output.PlaceColllectableSetup(data);
        EditorSceneManager.MarkSceneDirty(output.gameObject.scene);
    }
    private void PlaceCollectables(CollectableData data, bool clearAll)
    {
        if (clearAll)
            ClearCollectableRecordArea();


        placer.PlaceColletables(data);
        EditorSceneManager.MarkSceneDirty(placer.gameObject.scene);
    }

    public void ClearEnemyType(EnemyData data)
    {
        GameObject recorderEnemy = GameObject.Find("SetupRecorderEnemy");

        if (recorderEnemy == null)
        {
            Debug.LogWarning("SetupRecorderEnemy GameObject not found.");
            return;
        }

        if (data is NormalPigSO)
        {
            ClearSpecificEnemies<PigMovementBasic>();
        }
        else if (data is BigPigSO)
        {
            ClearSpecificEnemies<BigPigMovement>();
        }
        else if (data is JetPackPigSO)
        {
            ClearSpecificEnemies<JetPackPigMovement>();
        }
        else if (data is TenderizerPigSO)
        {
            ClearSpecificEnemies<TenderizerPig>();
        }

        // Mark scene as dirty to ensure changes are saved
        EditorSceneManager.MarkSceneDirty(recorderEnemy.scene);
    }

    private void ClearSpecificEnemies<T>() where T : MonoBehaviour
    {
        var targets = recorderEnemy.GetComponentsInChildren<T>();
        foreach (var target in targets)
        {
            GameObject.DestroyImmediate(target.gameObject);
        }
    }

    public void ClearCollectableType(CollectableData data)
    {

        if (recorderCollectable == null)
        {
            Debug.LogWarning("SetupRecorderCollectable GameObject not found.");
            return;
        }

        if (data is RingSO)
        {
            ClearSpecificCollectable<RingMovement>();
        }


        // Mark scene as dirty to ensure changes are saved
        EditorSceneManager.MarkSceneDirty(recorderEnemy.scene);
    }

    private void ClearSpecificCollectable<T>() where T : MonoBehaviour
    {
        var targets = recorderCollectable.GetComponentsInChildren<T>();
        foreach (var target in targets)
        {
            GameObject.DestroyImmediate(target.gameObject);
        }
    }

}