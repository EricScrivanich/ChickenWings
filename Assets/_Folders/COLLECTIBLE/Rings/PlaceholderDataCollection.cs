using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "PlaceholderRingDataCollection", menuName = "ScriptableObjects/PlaceholderRingDataCollection", order = 1)]
public class PlaceholderRingDataCollection : ScriptableObject
{
    

    [SerializeField] private GameObject placeholderRingPrefab;
    public List<PlaceholderTriggerGroup> triggerGroups = new List<PlaceholderTriggerGroup>(); 

    public void RecordPlaceholders(GameObject ringRecorder)
    {
        
        triggerGroups.Clear(); // Clear existing groups
       

        foreach (var placeholder in ringRecorder.GetComponentsInChildren<PlaceholderRing>())
        {
            var data = new PlaceholderRingData(placeholder);
            int triggerValue = placeholder.getsTriggeredInt;

            // Find or create a group for this trigger
            PlaceholderTriggerGroup group = triggerGroups.FirstOrDefault(g => g.triggerValue == triggerValue);
            if (group == null)
            {
                group = new PlaceholderTriggerGroup { triggerValue = triggerValue };
                triggerGroups.Add(group);
            }

            group.placeholderRingDataList.Add(data);
            
        }

        // Optionally, sort each list within the groups by some criteria, such as the x-position
        foreach (var group in triggerGroups)
        {
            group.placeholderRingDataList = group.placeholderRingDataList.OrderBy(group => group.position.x).ToList();
        }

        foreach (var placeholder in ringRecorder.GetComponentsInChildren<PlaceholderPlane>())
        {
            var data = new PlaceHolderPlaneData(placeholder);
            int triggerValue = placeholder.getsTriggeredInt; // Assuming PlaceholderPlane now has a getsTriggeredInt property

            // Find the existing group for this trigger value (groups should already exist from the PlaceholderRing loop)
            PlaceholderTriggerGroup group = triggerGroups.FirstOrDefault(g => g.triggerValue == triggerValue);
            if (group != null) // No need to create a new group, as they should already exist from PlaceholderRing handling
            {
                group.placeholderPlaneDataList.Add(data);
            }
            else
            {
                Debug.LogWarning($"No trigger group found for trigger value {triggerValue} from PlaceholderPlane. This should not happen if rings and planes share trigger values.");
            }
        }

        // Optionally, sort plane data within the groups, if needed
        foreach (var group in triggerGroups)
        {
            group.placeholderPlaneDataList = group.placeholderPlaneDataList.OrderBy(data => data.position.x).ToList();
        }
    }


    public void PlacePrefabsInScene()
    {
        GameObject recordedOutput = GameObject.Find("RecordedOutput") ?? new GameObject("RecordedOutput");

        for (int i = recordedOutput.transform.childCount - 1; i >= 0; i--)
        {
            // Use DestroyImmediate in the editor, Destroy if this needs to run at runtime
            GameObject.DestroyImmediate(recordedOutput.transform.GetChild(i).gameObject);
        }

        foreach (var group in triggerGroups)
        {
            foreach (var placeholderData in group.placeholderRingDataList)
            {
                GameObject prefabInstance = Instantiate(placeholderRingPrefab, placeholderData.position, placeholderData.rotation, recordedOutput.transform);
                prefabInstance.transform.localScale = placeholderData.scale;

                PlaceholderRing placeholderScript = prefabInstance.GetComponent<PlaceholderRing>();
                if (placeholderScript != null)
                {
                    // Apply data from PlaceholderRingData to PlaceholderRing script
                }
            }


            foreach (var planeData in group.placeholderPlaneDataList)
            {
                if (planeData.planeType != null && planeData.planeType.fillObject != null)
                {
                    // Use the fillObject as the prefab to instantiate for planes
                    GameObject planeInstance = Instantiate(planeData.planeType.fillObject, planeData.position, Quaternion.identity, recordedOutput.transform);
                    // Set properties like scale and speed if needed. Assuming fillObject is a GameObject that has a PlaceholderPlane component or similar where you can set speed.
                    PlaceholderPlane placeholderPlaneScript = planeInstance.GetComponent<PlaceholderPlane>();
                    if (placeholderPlaneScript != null)
                    {
                        placeholderPlaneScript.speed = planeData.speed;
                    }
                }
                else
                {
                    Debug.LogWarning("PlaneType or FillObject is null. Skipping plane instantiation.");
                }
            }
        }


        
    }
}




// Add any other methods needed for manipulating the placeholder data




// using UnityEngine;
// using System.Collections;
// using System;
// using System.Collections.Generic;
// using System.Linq;

// [CreateAssetMenu(fileName = "PlaceholderRingDataCollection", menuName = "ScriptableObjects/PlaceholderRingDataCollection", order = 1)]
// public class PlaceholderRingDataCollection : ScriptableObject
// {
//     public Dictionary<int, List<PlaceholderRingData>> placeholdersByTrigger = new Dictionary<int, List<PlaceholderRingData>>();
//     public bool record;
//     public bool placeInNonActiveScene; // New boolean for placing prefabs
//     public GameObject placeholderRingPrefab; // Assign your PlaceholderRing prefab in the inspector

//     private void OnValidate()
//     {
//         if (record)
//         {
//             RecordPlaceholders();
//             record = false; // Reset the flag
//         }

//         if (placeInNonActiveScene)
//         {
//             PlacePrefabsInNonActiveScene();
//             placeInNonActiveScene = false; // Reset the flag
//         }
//     }

//     private void PlacePrefabsInNonActiveScene()
//     {
//         GameObject recordedOutput = GameObject.Find("RecordedOutput") ?? new GameObject("RecordedOutput");

//         // Check if the GameObject already has the RecordedOutput component
//         if (!recordedOutput.GetComponent<RecordedOutput>())
//         {
//             recordedOutput.AddComponent<RecordedOutput>(); // Add the RecordedOutput script as a component
//         }
//         foreach (Transform child in recordedOutput.transform)
//         {
//             child.gameObject.SetActive(false);
//         }



//         foreach (var triggerList in placeholdersByTrigger.Values)
//         {
//             foreach (var placeholderData in triggerList)
//             {
//                 GameObject instantiatedPrefab = Instantiate(placeholderRingPrefab, placeholderData.position, Quaternion.identity, recordedOutput.transform);

//                 // Set PlaceholderRing values
//                 PlaceholderRing placeholderComponent = instantiatedPrefab.GetComponent<PlaceholderRing>();
//                 if (placeholderComponent != null)
//                 {
//                     placeholderComponent.ringIndex = placeholderData.ringIndex;
//                     placeholderComponent.order = placeholderData.order;
//                     placeholderComponent.getsTriggeredInt = placeholderData.getsTriggeredInt;
//                     placeholderComponent.doesTriggerInt = placeholderData.doesTriggerInt;
//                     placeholderComponent.xCordinateTrigger = placeholderData.xCordinateTrigger;
//                     placeholderComponent.speed = placeholderData.speed;
//                     // Set any other properties as needed
//                 }
//             }
//         }
//     }

//     private void RecordPlaceholders()
//     {
//         var ringRecorder = GameObject.Find("RingRecorder");
//         if (ringRecorder == null) return;

//         foreach (var placeholder in ringRecorder.GetComponentsInChildren<PlaceholderRing>())
//         {
//             var data = new PlaceholderRingData(placeholder, placeholder.transform);
//             int triggerValue = placeholder.getsTriggeredInt;

//             if (!placeholdersByTrigger.ContainsKey(triggerValue))
//             {
//                 placeholdersByTrigger[triggerValue] = new List<PlaceholderRingData>();
//             }

//             placeholdersByTrigger[triggerValue].Add(data);
//         }

//         // Order each list by x-position
//         foreach (var key in placeholdersByTrigger.Keys.ToList())
//         {
//             placeholdersByTrigger[key] = placeholdersByTrigger[key].OrderBy(p => p.position.x).ToList();
//         }
//     }
// }