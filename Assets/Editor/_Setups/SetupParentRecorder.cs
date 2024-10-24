using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(SetupParent))]
public class EnemySetupRecorderEditor : Editor
{
    public string[] options = new string[] {
    "Ring",
    "Normal Pig",
    "Big Pig",
    "JetPack Pig",
    "Tenderizer Pig",
    "Pilot Pig",
    "Missile Pig",      // New option for Missile Pig
    "Silo",             // New option for Silo
    "Wind Mill",        // New option for Wind Mill
    "Gas Pig",          // New option for Gas Pig
    "Hot Air Balloon",  // New option for Hot Air Balloon
    "Flappy Pig",       // New option for Flappy Pig
    "Bomber Plane"      // New option for Bomber Plane
};
    public int indexDropDown = 0;
    private int selectedTriggerIndex = -1;
    private int selectedTriggerIndexEnemy = -1;
    private int selectedTriggerIndexCollectable = -1;
    private List<int> selctedEnemyTriggers = new List<int>();
    private List<int> selctedCollTriggers = new List<int>();
    private GameObject recorderEnemy;
    private GameObject recorderCollectable;
    private SetupPlacer placer;
    private SetupParent script;
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
                RecordRings(tempListColl, script.XTriggerForRecording);
                break;
            case 1:
                RecordNormalPigs(tempListEnemy, script.XTriggerForRecording);

                break;
            case 2:
                RecordBigPigs(tempListEnemy, script.XTriggerForRecording);

                break;

            case 3:
                RecordJetPackPigs(tempListEnemy, script.XTriggerForRecording);

                break;

            case 4:
                RecordTenderizerPigs(tempListEnemy, script.XTriggerForRecording);

                break;

            case 5:
                RecordPilotPigs(tempListEnemy, script.XTriggerForRecording);  // New case for Pilot Pigs
                break;

            case 6:
                RecordMissilePigs(tempListEnemy, script.XTriggerForRecording);  // New case for Missile Pigs
                break;
            case 7:
                RecordSilos(tempListEnemy, script.XTriggerForRecording);  // New case for Silos
                break;
            case 8:
                RecordWindMills(tempListEnemy, script.XTriggerForRecording);  // New case for WindMills
                break;
            case 9:
                RecordGasPigs(tempListEnemy, script.XTriggerForRecording);  // New case for Gas Pigs
                break;
            case 10:
                RecordHotAirBalloons(tempListEnemy, script.XTriggerForRecording);  // New case for Hot Air Balloons
                break;
            case 11:
                RecordFlappyPigs(tempListEnemy, script.XTriggerForRecording);  // New case for Flappy Pigs
                break;
            case 12:
                RecordBomberPlanes(tempListEnemy, script.XTriggerForRecording);  // New case for Bomber Planes
                break;

            default:
                Debug.LogError("Unrecognized Option");
                break;
        }
        if (script.isRandomSetup)
        {
            if (indexDropDown == 0 && selectedTriggerIndexCollectable >= 0)
            {
                script.DuplicateOrRemoveCollectable(selectedTriggerIndexCollectable, script.collectableSetup[selectedTriggerIndexCollectable].dataArray.Length - 1, true, tempListColl[0]);
            }
            else if (indexDropDown > 0 && selectedTriggerIndexEnemy >= 0)
            {
                script.DuplicateOrRemoveEnemy(selectedTriggerIndexEnemy, script.enemySetup[selectedTriggerIndexEnemy].dataArray.Length - 1, true, tempListEnemy[0]);
            }

        }

        else
        {
            if (indexDropDown == 0 && selectedTriggerIndex >= 0)
            {
                script.DuplicateOrRemoveCollectable(selectedTriggerIndex, script.collectableSetup[selectedTriggerIndex].dataArray.Length - 1, true, tempListColl[0]);
            }
            else if (indexDropDown > 0 && selectedTriggerIndex >= 0)
            {
                script.DuplicateOrRemoveEnemy(selectedTriggerIndex, script.enemySetup[selectedTriggerIndex].dataArray.Length - 1, true, tempListEnemy[0]);
            }

        }

    }
    public override void OnInspectorGUI()
    {
        script = (SetupParent)target;

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

        RecordJetPackPigs(newEnemyData, script.XTriggerForRecording);
        RecordNormalPigs(newEnemyData, script.XTriggerForRecording);
        RecordTenderizerPigs(newEnemyData, script.XTriggerForRecording);
        RecordBigPigs(newEnemyData, script.XTriggerForRecording);
        RecordPilotPigs(newEnemyData, script.XTriggerForRecording);
        RecordMissilePigs(newEnemyData, script.XTriggerForRecording);
        RecordSilos(newEnemyData, script.XTriggerForRecording);
        RecordWindMills(newEnemyData, script.XTriggerForRecording);
        RecordGasPigs(newEnemyData, script.XTriggerForRecording);
        RecordHotAirBalloons(newEnemyData, script.XTriggerForRecording);
        RecordFlappyPigs(newEnemyData, script.XTriggerForRecording);
        RecordBomberPlanes(newEnemyData, script.XTriggerForRecording);


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

        RecordRings(newCollectableData, script.XTriggerForRecording);

        // Convert the list to an array and add to SetupParent
        script.RecordForColletableTrigger(newCollectableData, trigger);
        EditorUtility.SetDirty(script);

        selectedTriggerIndex = trigger;


    }
    private void RecordJetPackPigs(List<EnemyData> newEnemyData, float xTrigger)
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
                float distanceToTarget = jetPackPigsRecorder[i].speed > 0 ? xTrigger - jetPackPigsRecorder[i].transform.position.x : -xTrigger - jetPackPigsRecorder[i].transform.position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / jetPackPigsRecorder[i].speed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger
            if (!script.ignoreXTriggetTime)
                newJetPackPigSO.TimeToTrigger = timesToTrigger.Max();

            // Add the new instance to the temporary list
            newEnemyData.Add(newJetPackPigSO);
        }
    }

    private void RecordNormalPigs(List<EnemyData> newEnemyData, float xTrigger)
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
                float distanceToTarget = newNormalPigSO.data[i].speed > 0 ? xTrigger - newNormalPigSO.data[i].position.x : -xTrigger - newNormalPigSO.data[i].position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / newNormalPigSO.data[i].speed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger
            if (!script.ignoreXTriggetTime)
                newNormalPigSO.TimeToTrigger = timesToTrigger.Max();

            // Add the new instance to the temporary list
            newEnemyData.Add(newNormalPigSO);
        }
    }

    private void RecordBigPigs(List<EnemyData> newEnemyData, float xTrigger)
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
                    distanceToFlap = targets[i].distanceToFlap,
                    startingFallSpot = targets[i].startingFallSpot
                };

                // Calculate time to reach x = 7 or x = -7

                float distanceToTarget = newClass.data[i].speed > 0 ? xTrigger - newClass.data[i].position.x : -xTrigger - newClass.data[i].position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / newClass.data[i].speed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger

            if (!script.ignoreXTriggetTime)
                newClass.TimeToTrigger = timesToTrigger.Max();



            // Add the new instance to the temporary list
            newEnemyData.Add(newClass);
        }
    }
    private void RecordPilotPigs(List<EnemyData> newEnemyData, float xTrigger)
    {
        var pilotPigsRecorder = recorderEnemy.GetComponentsInChildren<PilotPig>();
        if (pilotPigsRecorder.Length > 0)
        {
            PilotPigSO newPilotPigSO = new PilotPigSO
            {
                data = new PilotPigData[pilotPigsRecorder.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < pilotPigsRecorder.Length; i++)
            {
                newPilotPigSO.data[i] = new PilotPigData
                {
                    position = pilotPigsRecorder[i].transform.position,
                    scale = pilotPigsRecorder[i].transform.localScale,
                    speed = pilotPigsRecorder[i].initialSpeed,
                    flightMode = pilotPigsRecorder[i].flightMode,
                    minY = pilotPigsRecorder[i].minY,
                    maxY = pilotPigsRecorder[i].maxY,
                    yForce = pilotPigsRecorder[i].addForceY,
                    maxYSpeed = pilotPigsRecorder[i].maxYSpeed,
                    xTrigger = pilotPigsRecorder[i].xTrigger
                };

                // Calculate time to reach x = 7 or x = -7
                float distanceToTarget = pilotPigsRecorder[i].initialSpeed > 0 ? xTrigger - pilotPigsRecorder[i].transform.position.x : -xTrigger - pilotPigsRecorder[i].transform.position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / pilotPigsRecorder[i].initialSpeed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger
            if (!script.ignoreXTriggetTime)
                newPilotPigSO.TimeToTrigger = timesToTrigger.Max();

            // Add the new instance to the temporary list
            newEnemyData.Add(newPilotPigSO);
        }
    }

    private void RecordTenderizerPigs(List<EnemyData> newEnemyData, float xTrigger)
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
                float distanceToTarget = newClass.data[i].speed > 0 ? xTrigger - newClass.data[i].position.x : -xTrigger - newClass.data[i].position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / newClass.data[i].speed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger
            if (!script.ignoreXTriggetTime)
                newClass.TimeToTrigger = timesToTrigger.Max();

            // Add the new instance to the temporary list
            newEnemyData.Add(newClass);
        }
    }

    private void RecordSilos(List<EnemyData> newEnemyData, float xTrigger)
    {
        var targets = recorderEnemy.GetComponentsInChildren<SiloMovement>();
        if (targets.Length > 0)
        {
            SiloSO newClass = new SiloSO
            {
                data = new SiloData[targets.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < targets.Length; i++)
            {
                newClass.data[i] = new SiloData
                {
                    position = targets[i].transform.position,
                    type = targets[i].type,
                    baseHeightMultiplier = targets[i].baseHeightMultiplier
                };

                float distanceToTarget = Mathf.Abs(xTrigger - targets[i].transform.position.x);
                float timeToTrigger = distanceToTarget / 4.7f; // Assuming moveSpeed exists
                timesToTrigger.Add(timeToTrigger);
            }
            if (!script.ignoreXTriggetTime)
                newClass.TimeToTrigger = timesToTrigger.Max();
            newEnemyData.Add(newClass);
        }
    }

    private void RecordWindMills(List<EnemyData> newEnemyData, float xTrigger)
    {
        var targets = recorderEnemy.GetComponentsInChildren<Windmill>();
        if (targets.Length > 0)
        {
            WindMillSO newClass = new WindMillSO
            {
                data = new WindMillData[targets.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < targets.Length; i++)
            {
                newClass.data[i] = new WindMillData
                {
                    position = targets[i].transform.position,
                    bladeAmount = targets[i].bladeAmount,
                    bladeScaleMultiplier = targets[i].bladeScaleMultiplier,
                    bladeSpeed = targets[i].bladeSpeed,
                    startRot = targets[i].startRot
                };

                float distanceToTarget = Mathf.Abs(xTrigger - targets[i].transform.position.x);
                float timeToTrigger = distanceToTarget / 4.7f; // Assuming moveSpeed exists
                timesToTrigger.Add(timeToTrigger);
            }
            if (!script.ignoreXTriggetTime)

                newClass.TimeToTrigger = timesToTrigger.Max();
            newEnemyData.Add(newClass);
        }
    }

    private void RecordGasPigs(List<EnemyData> newEnemyData, float xTrigger)
    {
        var targets = recorderEnemy.GetComponentsInChildren<GasPig>();
        if (targets.Length > 0)
        {
            GasPigSO newClass = new GasPigSO
            {
                data = new GasPigData[targets.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < targets.Length; i++)
            {
                newClass.data[i] = new GasPigData
                {
                    position = targets[i].transform.position,
                    speed = targets[i].speed,
                    delay = targets[i].delay,
                    initialDelay = targets[i].initialDelay

                };

                float distanceToTarget = Mathf.Abs(xTrigger - targets[i].transform.position.x);
                float timeToTrigger = distanceToTarget / targets[i].speed; // Assuming moveSpeed exists
                timesToTrigger.Add(timeToTrigger);
            }
            if (!script.ignoreXTriggetTime)

                newClass.TimeToTrigger = timesToTrigger.Max();
            newEnemyData.Add(newClass);
        }
    }

    private void RecordMissilePigs(List<EnemyData> newEnemyData, float xTrigger)
    {
        var targets = recorderEnemy.GetComponentsInChildren<MissilePigScript>();
        if (targets.Length > 0)
        {
            MissilePigSO newClass = new MissilePigSO
            {
                data = new MissilePigData[targets.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < targets.Length; i++)
            {
                newClass.data[i] = new MissilePigData
                {
                    position = targets[i].transform.position,
                    missileType = targets[i].missileType,
                    movementType = targets[i].movementType
                };
                float speedToCheck = 4.9f;
                if (newClass.data[i].movementType == 1 || newClass.data[i].movementType == 3)
                    speedToCheck = 3.2f;

                float distanceToTarget = Mathf.Abs(xTrigger - targets[i].transform.position.x);
                float timeToTrigger = distanceToTarget / speedToCheck; // Assuming moveSpeed exists
                timesToTrigger.Add(timeToTrigger);
            }
            if (!script.ignoreXTriggetTime)
                newClass.TimeToTrigger = timesToTrigger.Max();
            newEnemyData.Add(newClass);
        }
    }

    private void RecordFlappyPigs(List<EnemyData> newEnemyData, float xTrigger)
    {
        var targets = recorderEnemy.GetComponentsInChildren<FlappyPigMovement>();
        if (targets.Length > 0)
        {
            FlappyPigSO newClass = new FlappyPigSO
            {
                data = new FlappyPigData[targets.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < targets.Length; i++)
            {
                newClass.data[i] = new FlappyPigData
                {
                    position = targets[i].transform.position,
                    scaleFactor = targets[i].scaleFactor
                };

                float distanceToTarget = Mathf.Abs(xTrigger - targets[i].transform.position.x);
                float timeToTrigger = 0; // Assume moveSpeed is defined
                timesToTrigger.Add(timeToTrigger);
            }
            if (!script.ignoreXTriggetTime)
                newClass.TimeToTrigger = timesToTrigger.Max();
            newEnemyData.Add(newClass);
        }
    }

    private void RecordBomberPlanes(List<EnemyData> newEnemyData, float xTrigger)
    {
        var targets = recorderEnemy.GetComponentsInChildren<DropBomb>();
        if (targets.Length > 0)
        {
            BomberPlaneSO newClass = new BomberPlaneSO
            {
                data = new BomberPlaneData[targets.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < targets.Length; i++)
            {
                newClass.data[i] = new BomberPlaneData
                {
                    xDropPosition = targets[i].xDropPosition,
                    dropAreaScaleMultiplier = targets[i].dropAreaScaleMultiplier,
                    speedTarget = targets[i].speedTarget
                };

                float distanceToTarget = Mathf.Abs(xTrigger - targets[i].transform.position.x);
                float timeToTrigger = 0; // Assume moveSpeed is defined
                timesToTrigger.Add(timeToTrigger);
            }
            if (!script.ignoreXTriggetTime)
                newClass.TimeToTrigger = timesToTrigger.Max();
            newEnemyData.Add(newClass);
        }
    }

    private void RecordHotAirBalloons(List<EnemyData> newEnemyData, float xTrigger)
    {
        var targets = recorderEnemy.GetComponentsInChildren<HotAirBalloon>();
        if (targets.Length > 0)
        {
            HotAirBalloonSO newClass = new HotAirBalloonSO
            {
                data = new HotAirBalloonData[targets.Length]
            };

            List<float> timesToTrigger = new List<float>();

            for (int i = 0; i < targets.Length; i++)
            {
                newClass.data[i] = new HotAirBalloonData
                {
                    position = targets[i].transform.position,
                    type = targets[i].type,
                    xTrigger = targets[i].xTrigger,
                    yTarget = targets[i].yTarget,
                    speed = targets[i].speed,
                    delay = targets[i].delay
                };

                float distanceToTarget = Mathf.Abs(xTrigger - targets[i].transform.position.x);
                float timeToTrigger = distanceToTarget / targets[i].speed; // Assume moveSpeed is defined
                timesToTrigger.Add(timeToTrigger);
            }
            if (!script.ignoreXTriggetTime)
                newClass.TimeToTrigger = timesToTrigger.Max();
            newEnemyData.Add(newClass);
        }
    }




    private void RecordRings(List<CollectableData> newCollectableData, float xTrigger)
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
                float distanceToTarget = newClass.data[i].speed > 0 ? xTrigger - newClass.data[i].position.x : -xTrigger - newClass.data[i].position.x;
                float timeToTrigger = Mathf.Abs(distanceToTarget / newClass.data[i].speed);
                timesToTrigger.Add(timeToTrigger);
            }

            // Set the highest time to trigger
            if (!script.ignoreXTriggetTime)
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

        if (script.isRandomSetup)
        {
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
                        RecordForEnemyTrigger(script, i);
                    }

                    GUI.backgroundColor = originalColor;

                    if (GUILayout.Button("Place Trigger", GUILayout.Height(40)))
                    {
                        PlaceEnemySetup(script.enemySetup[i].dataArray);
                    }

                    EditorGUILayout.EndHorizontal();

                    DrawShowButtons(script.enemySetup[i].dataArray, null, i, script);
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
                        RecordForCollectableTrigger(script, i);
                    }

                    GUI.backgroundColor = originalColor;

                    if (GUILayout.Button("Place Trigger", GUILayout.Height(40)))
                    {
                        PlaceCollectableSetup(script.collectableSetup[i].dataArray);
                    }

                    EditorGUILayout.EndHorizontal();

                    DrawShowButtons(null, script.collectableSetup[i].dataArray, i, script);
                }
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
        else
        {
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

                    if (i < script.enemySetup.Count)
                    {
                        GUILayout.Label($"Enemy Trigger: {i} -  (Count {script.enemySetup[i].dataArray.Length})");
                        DrawShowButtons(script.enemySetup[i].dataArray, null, i, script); // Draw ShowHere buttons under the corresponding trigger
                    }
                    else
                    {
                        GUILayout.Label($"Empty Enemy Trigger: {i}");
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth / 2 - 10));

                    if (i < script.collectableSetup.Count)
                    {
                        GUILayout.Label($"Collectable Trigger: {i} -  (Count {script.collectableSetup[i].dataArray.Length})");
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
                else if (enemyData is PilotPigSO pilotPig)
                {
                    buttonTitle = $"PilotPigs ({pilotPig.data.Length})";
                }
                else if (enemyData is SiloSO siloData)
                {
                    buttonTitle = $"Silos ({siloData.data.Length})";
                }
                else if (enemyData is WindMillSO windMillData)
                {
                    buttonTitle = $"WindMills ({windMillData.data.Length})";
                }
                else if (enemyData is GasPigSO gasPigData)
                {
                    buttonTitle = $"GasPigs ({gasPigData.data.Length})";
                }
                else if (enemyData is HotAirBalloonSO hotAirBalloonData)
                {
                    buttonTitle = $"HotAirBalloons ({hotAirBalloonData.data.Length})";
                }
                else if (enemyData is FlappyPigSO flappyPigData)
                {
                    buttonTitle = $"FlappyPigs ({flappyPigData.data.Length})";
                }
                else if (enemyData is BomberPlaneSO bomberPlaneData)
                {
                    buttonTitle = $"BomberPlanes ({bomberPlaneData.data.Length})";
                }
                else if (enemyData is MissilePigSO missilePigData)
                {
                    buttonTitle = $"MissilePigs ({missilePigData.data.Length})";
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
            RecordNormalPigs(tempList, script.XTriggerForRecording);


        }
        else if (dataTypeEnemy is JetPackPigSO so1)
        {
            RecordJetPackPigs(tempList, script.XTriggerForRecording);


        }
        else if (dataTypeEnemy is TenderizerPigSO so2)
        {
            RecordTenderizerPigs(tempList, script.XTriggerForRecording);


        }

        else if (dataTypeEnemy is BigPigSO so3)
        {
            RecordBigPigs(tempList, script.XTriggerForRecording);


        }
        else if (dataTypeEnemy is PilotPigSO so4)
        {
            RecordPilotPigs(tempList, script.XTriggerForRecording);


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
        else if (data is PilotPigSO)
        {
            ClearSpecificEnemies<PilotPig>();
        }
        else if (data is SiloSO)
        {
            ClearSpecificEnemies<SiloMovement>();
        }
        else if (data is WindMillSO)
        {
            ClearSpecificEnemies<Windmill>();
        }
        else if (data is GasPigSO)
        {
            ClearSpecificEnemies<GasPig>();
        }
        else if (data is HotAirBalloonSO)
        {
            ClearSpecificEnemies<HotAirBalloon>();
        }
        else if (data is FlappyPigSO)
        {
            ClearSpecificEnemies<FlappyPigMovement>();
        }
        else if (data is BomberPlaneSO)
        {
            ClearSpecificEnemies<DropBomb>();
        }
        else if (data is MissilePigSO)
        {
            ClearSpecificEnemies<MissilePigScript>();
        }

        // Mark scene as dirty to ensure changes are saved
        EditorSceneManager.MarkSceneDirty(recorderEnemy.scene);
    }





    public void RecordOrDuplicateSpecificCollectable(CollectableData dataTypeCollectable, int trigger, int index, bool isDuplicate)
    {
        List<CollectableData> tempList = new List<CollectableData>();


        if (dataTypeCollectable is RingSO so)
        {
            RecordRings(tempList, script.XTriggerForRecording);


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