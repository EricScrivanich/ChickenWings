using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(RandomSetupParent))]
public class RandomSetupParentEditor : Editor
{
    public RandomSetupParent script;
    public GameObject recorderEnemy;
    public GameObject recorderCollectable;
    public SetupPlacer placer;
    private int selectedTriggerIndex = -1;

    private Color recordButtonColor = new Color(0.9528f, 0.4179f, 0.4179f, 1f);

    private Color lastSelectedTriggerColor = new Color(.591f, 1f, .542f, 1f);
    private Color emptyArrayColor = new Color(.3f, .3f, .3f, 1f);
    public override void OnInspectorGUI()
    {
        script = (RandomSetupParent)target;
        recorderEnemy = GameObject.Find("SetupRecorderEnemy");
        recorderCollectable = GameObject.Find("SetupRecorderCollectable");
        placer = GameObject.Find("SetupRecorderParent").GetComponent<SetupPlacer>();
        CreateLists();

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
        DrawParentButtons();


    }

    public void RecordNormalPig(int index)
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
            script.FillEnemyArrays(newNormalPigSO, index);

            // Add the new instance to the temporary list


        }
    }

    public void RecordBigPigs(int index)
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
            script.FillEnemyArrays(newClass, index);
        }
    }

    public void RecordTenderizerPigs(int index)
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
            script.FillEnemyArrays(newClass, index);
        }
    }

    public void RecordJetPackPigs(int index)
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
            script.FillEnemyArrays(newJetPackPigSO, index);
        }
    }

    public void RecordRings(int index)
    {
        var recordObj = recorderCollectable.GetComponentsInChildren<RingMovement>();
        if (recordObj.Length > 0)
        {
            RingSO newRingSO = new RingSO
            {
                data = new RingData[recordObj.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < recordObj.Length; i++)
            {
                newRingSO.data[i] = new RingData
                {
                    position = recordObj[i].transform.position,
                    scale = recordObj[i].transform.localScale,
                    speed = recordObj[i].speed
                };

                // Calculate time to reach x = 7 or x = -7
                float distanceToTarget = recordObj[i].speed > 0 ? 7f - recordObj[i].transform.position.x : -7f - recordObj[i].transform.position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / recordObj[i].speed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger
            newRingSO.TimeToTrigger = timesToTrigger.Max();
            script.FillCollectableArrays(newRingSO, index);
        }
    }


    private void DrawParentButtons()
    {
        var originalColor = GUI.backgroundColor;
        for (int i = 0; i < TriggerCount(); i++)
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


                GUI.backgroundColor = recordButtonColor;

                if (GUILayout.Button("Record Trigger", GUILayout.Height(40)))
                {
                    RecordForTrigger(i);
                }

                GUI.backgroundColor = originalColor;
                DrawButtons(null, script.ringArray[i], i);
                DrawButtons(script.pigNormalArray[i], null, i);
                DrawButtons(script.pigJetPackArray[i], null, i);
                DrawButtons(script.pigBigArray[i], null, i);
                DrawButtons(script.pigTenderizerArray[i], null, i);






            }
        }
    }

    private void DrawButtons(EnemyData dataEnemy, CollectableData dataColl, int index)
    {
        string buttonTitle = "";
        var originalColor = GUI.backgroundColor;
        bool isPresent = false;
        if (dataEnemy == null && dataColl != null)
        {
            if (dataColl is RingSO data)
            {

                if (script.ringArray != null && script.ringArray.Count > index)
                {
                    buttonTitle = $"Rings ({data.data.Length})";
                    isPresent = true;

                }
                else
                {
                    buttonTitle = "Rings Empty";
                    isPresent = false;

                }
            }

            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = recordButtonColor;
            if (GUILayout.Button("R", GUILayout.Width(25))) // Adjust width as needed
            {
                RecordRings(index);
            }
            GUI.backgroundColor = originalColor;
            if (GUILayout.Button("Clear", GUILayout.Width(50))) // Adjust width as needed
            {
                ClearCollectable(dataColl);
            }

            if (isPresent)
            {

                if (GUILayout.Button(buttonTitle)) // Adjust width as needed
                {
                    ClearCollectable(dataColl);
                    PlaceCollectables(dataColl);

                }
            }
            else
            {
                GUI.backgroundColor = emptyArrayColor;

                if (GUILayout.Button(buttonTitle)) // Adjust width as needed
                {
                    Debug.Log("This is empty");

                }
                GUI.backgroundColor = originalColor;

            }

            EditorGUILayout.EndHorizontal();

        }

        else if (dataEnemy != null && dataColl == null)
        {
            if (dataEnemy is NormalPigSO data)
            {
                if (script.pigNormalArray != null && script.pigNormalArray.Count > index)
                {
                    buttonTitle = $"Pig Normal ({data.data.Length})";
                    isPresent = true;

                }
                else
                {
                    buttonTitle = "Pigs Normal Empty";
                    isPresent = false;
                }
            }
            else if (dataEnemy is JetPackPigSO data1)
            {
                if (script.pigJetPackArray != null && script.pigJetPackArray.Count > index)
                {
                    buttonTitle = $"Pig JetPack ({data1.data.Length})";
                    isPresent = true;
                }
                else
                {
                    buttonTitle = "Pig JetPack Empty";
                    isPresent = false;
                }
            }
            else if (dataEnemy is TenderizerPigSO data2)
            {
                if (script.pigTenderizerArray != null && script.pigTenderizerArray.Count > index)
                {
                    buttonTitle = $"Pig Tenderizer ({data2.data.Length})";
                    isPresent = true;
                }
                else
                {
                    buttonTitle = "Pig Tenderizer Empty";
                    isPresent = false;
                }
            }
            else if (dataEnemy is BigPigSO data3)
            {
                if (script.pigBigArray != null && script.pigBigArray.Count > index)
                {
                    buttonTitle = $"Pig Big ({data3.data.Length})";
                    isPresent = true;
                }
                else
                {
                    buttonTitle = "Pig Big Empty";
                    isPresent = false;
                }
            }

            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = recordButtonColor;
            if (GUILayout.Button("R", GUILayout.Width(25))) // Adjust width as needed
            {
                RecordSpecificEnemy(dataEnemy, index);
            }
            GUI.backgroundColor = originalColor;
            if (GUILayout.Button("Clear", GUILayout.Width(50))) // Adjust width as needed
            {
                ClearEnemy(dataEnemy);
            }

            if (isPresent)
            {

                if (GUILayout.Button(buttonTitle)) // Adjust width as needed
                {
                    ClearEnemy(dataEnemy);
                    PlaceEnemies(dataEnemy);

                }
            }
            else
            {
                GUI.backgroundColor = emptyArrayColor;

                if (GUILayout.Button(buttonTitle)) // Adjust width as needed
                {
                    Debug.Log("This is empty");

                }
                GUI.backgroundColor = originalColor;

            }

            EditorGUILayout.EndHorizontal();






        }


    }


    private void ButtonLayout()
    {

    }

    private void RecordSpecificEnemy(EnemyData data, int index)
    {
        if (recorderEnemy == null)
        {
            Debug.LogWarning("SetupRecorderEnemy GameObject not found.");
            return;
        }

        if (data is NormalPigSO)
        {
            RecordNormalPig(index);
        }
        else if (data is BigPigSO)
        {
            RecordBigPigs(index);
        }
        else if (data is JetPackPigSO)
        {
            RecordJetPackPigs(index);
        }
        else if (data is TenderizerPigSO)
        {
            RecordTenderizerPigs(index);
        }

        // Mark scene as dirty to ensure changes are saved
        EditorUtility.SetDirty(script);

    }

    public void RecordForTrigger(int index)
    {
        RecordRings(index);
        RecordNormalPig(index);
        RecordBigPigs(index);
        RecordJetPackPigs(index);
        RecordTenderizerPigs(index);
    }

    public void ClearEnemy(EnemyData data)
    {

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

    public void ClearCollectable(CollectableData data)
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

    public int TriggerCount()
    {
        List<int> counts = new List<int>();
        int count = 0;

        counts.Add(script.pigNormalArray.Count);
        counts.Add(script.pigJetPackArray.Count);
        counts.Add(script.pigBigArray.Count);
        counts.Add(script.pigTenderizerArray.Count);
        counts.Add(script.ringArray.Count);

        foreach (int i in counts)
        {
            if (i > count) count = i;
        }

        if (count > 0) return count;
        else return 1;





    }


    private void CreateLists()
    {


        if (script.pigNormalArray == null)
        {
            script.pigNormalArray = new List<NormalPigSO>();
        }


        if (script.pigJetPackArray == null)
        {
            script.pigJetPackArray = new List<JetPackPigSO>();
        }


        if (script.pigBigArray == null)
        {
            script.pigBigArray = new List<BigPigSO>();
        }


        if (script.pigTenderizerArray == null)
        {
            script.pigTenderizerArray = new List<TenderizerPigSO>();
        }


        if (script.ringArray == null)
        {
            script.ringArray = new List<RingSO>();
        }
        EditorUtility.SetDirty(script);


    }


    private void PlaceEnemies(EnemyData data)
    {
        placer.PlaceEnemies(data);
    }

    private void PlaceCollectables(CollectableData data)
    {
        placer.PlaceColletables(data);
    }

    private void ClearEnemyRecordArea()
    {

        if (recorderEnemy != null)

        {
            // Clear children
            for (int i = recorderEnemy.transform.childCount - 1; i >= 0; i--)
            {
                // Use DestroyImmediate in the editor, Destroy if this needs to run at runtime
                GameObject.DestroyImmediate(recorderEnemy.transform.GetChild(i).gameObject);
            }
            // Mark scene as dirty to ensure changes are saved
            EditorSceneManager.MarkSceneDirty(recorderEnemy.scene);
        }
        else
        {
            Debug.LogWarning("recorderEnemy GameObject not found.");
        }
    }

    private void ClearCollectableRecordArea()
    {

        if (recorderCollectable != null)

        {
            // Clear children
            for (int i = recorderCollectable.transform.childCount - 1; i >= 0; i--)
            {
                // Use DestroyImmediate in the editor, Destroy if this needs to run at runtime
                GameObject.DestroyImmediate(recorderCollectable.transform.GetChild(i).gameObject);
            }
            // Mark scene as dirty to ensure changes are saved
            EditorSceneManager.MarkSceneDirty(recorderCollectable.scene);
        }
        else
        {
            Debug.LogWarning("recorderCollectable GameObject not found.");
        }
    }

}
