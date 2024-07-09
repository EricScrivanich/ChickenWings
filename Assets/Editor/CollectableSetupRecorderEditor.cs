using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;


[CustomEditor(typeof(SpawnSetupCollectable))]
public class CollectableSetupRecorderEditor : Editor
{
    // Start is called before the first frame update

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector

        SpawnSetupCollectable script = (SpawnSetupCollectable)target;

        if (GUILayout.Button("Clear Record Area"))
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

        if (GUILayout.Button("Record For Trigger"))
        {
           
            // Automatically find the GameObject named "SetupRecorder"
            GameObject recorder = GameObject.Find("SetupRecorderCollectable");
            script.collectableSetups = new CollectableData[0];


            if (recorder != null)
            {
                var triggerObjectRecorder = recorder.GetComponentInChildren<TriggerSetupObject>();

                if (triggerObjectRecorder != null)
                {
                    script.triggerObjectPosition = triggerObjectRecorder.transform.position;
                    script.triggerObjectSpeed = triggerObjectRecorder.speed;
                    script.triggerObjectXCordinateTrigger = triggerObjectRecorder.xCordinateTrigger;
                }
                else
                {
                    Debug.Log("No Trigger Object Found");
                    script.triggerObjectPosition = Vector2.zero;
                    script.triggerObjectSpeed = 0;
                    script.triggerObjectXCordinateTrigger = 0;

                }
                // Use the found GameObject to record placeholders
                var ringRecorder = recorder.GetComponentsInChildren<RingMovement>();

                if (ringRecorder.Length > 0)
                {
                    // Sort the ringRecorder array by distance from 0 on the X axis
                    Array.Sort(ringRecorder, (a, b) => Mathf.Abs(a.transform.position.x).CompareTo(Mathf.Abs(b.transform.position.x)));

                    RingSO newRingSO = CreateInstance<RingSO>();
                    newRingSO.data = new RingData[ringRecorder.Length];

                    for (int i = 0; i < ringRecorder.Length; i++)
                    {
                        newRingSO.data[i] = new RingData
                        {
                            position = ringRecorder[i].transform.position,
                            rotation = ringRecorder[i].transform.rotation,
                            scale = ringRecorder[i].transform.localScale,
                            speed = ringRecorder[i].speed
                        };
                    }

                    // Add the new instance to the enemySetups array
                    ArrayUtility.Add(ref script.collectableSetups, newRingSO);
                    AssetDatabase.AddObjectToAsset(newRingSO, script); // Add the new instance to the SpawnSetup asset
                    EditorUtility.SetDirty(script); // Marks the object as dirty to ensure the changes are saved
                }
            }


           

        }
        if (GUILayout.Button("Place Setup In Scene"))
        {

            GameObject recorderOutput = GameObject.Find("SetupRecorderCollectable");


            if (recorderOutput != null && script != null)
            {
                Transform recorderTransform = recorderOutput.transform;
                script.PlaceSetup(recorderTransform);
            }
            else
            {
                Debug.LogWarning("Recorder Output GameObject or SpawnSetupCollectable script not found!");
            }
        }
    }
    // if (GUILayout.Button("Record For Trigger"))
    // {
    //     script.collectableSetups = new CollectableData[0];
    //     // Automatically find the GameObject named "SetupRecorder"
    //     GameObject recorder = GameObject.Find("SetupRecorderCollectable");

    //     if (recorder != null)
    //     {
    //         // Use the found GameObject to record placeholders
    //         var ringRecorder = recorder.GetComponentsInChildren<RingMovement>();
    //         if (ringRecorder.Length > 0)
    //         {
    //             JetPackPigSO newJetPackPigSO = CreateInstance<JetPackPigSO>();
    //             newJetPackPigSO.data = new JetPackPigData[ringRecorder.Length];

    //             for (int i = 0; i < ringRecorder.Length; i++)
    //             {
    //                 newJetPackPigSO.data[i] = new JetPackPigData
    //                 {
    //                     position = ringRecorder[i].transform.position,
    //                     scale = ringRecorder[i].transform.localScale,
    //                     speed = ringRecorder[i].speed
    //                 };
    //             }

    //             // Add the new instance to the enemySetups array
    //             ArrayUtility.Add(ref script.enemySetups, newJetPackPigSO);
    //             AssetDatabase.AddObjectToAsset(newJetPackPigSO, script); // Add the new instance to the SpawnSetup asset
    //             EditorUtility.SetDirty(script); // Marks the object as dirty to ensure the changes are saved
    //         }
    //         else
    //         {
    //             Debug.LogWarning("No JetPackPigMovement components found under SetupRecorder.");
    //         }
    //     }
    // }
}
