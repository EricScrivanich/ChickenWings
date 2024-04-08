using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "PlaceholderDataCollection", menuName = "PlaceholderScriptableObjects/PlaceholderDataCollection", order = 1)]
public class PlaceholderDataCollection : ScriptableObject
{

    public bool TestSpecifiedTrigger;
    [SerializeField] private GameObject placeholderRingPrefab;
    [SerializeField] private GameObject planeAreaPrefab;
    public List<PlaceholderTriggerGroup> triggerGroups = new List<PlaceholderTriggerGroup>();

    [Space(20)]
    public int SpecifiedTrigger;

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
        // foreach (var group in triggerGroups)
        // {
        //     group.placeholderRingDataList = group.placeholderRingDataList.OrderBy(group => group.position.x).ToList();
        // }

        foreach (var group in triggerGroups)
        {
            group.placeholderRingDataList = group.placeholderRingDataList
                .OrderBy(data => Mathf.Abs(data.position.x))
                .ToList();
        }

        foreach (var placeholder in ringRecorder.GetComponentsInChildren<PlaceholderPlane>())
        {
            var data = new PlaceHolderPlaneData(placeholder);
            int triggerValue = placeholder.getsTriggeredInt; // Assuming PlaceholderPlane now has a getsTriggeredInt property

            // Find the existing group for this trigger value (groups should already exist from the PlaceholderRing loop)
            PlaceholderTriggerGroup group = triggerGroups.FirstOrDefault(g => g.triggerValue == triggerValue);
            if (group == null)
            {
                group = new PlaceholderTriggerGroup { triggerValue = triggerValue };
                triggerGroups.Add(group);
            }

            group.placeholderPlaneDataList.Add(data);
        }

        // Optionally, sort plane data within the groups, if needed
        foreach (var group in triggerGroups)
        {
            group.placeholderPlaneDataList = group.placeholderPlaneDataList.OrderBy(data => data.position.x).ToList();
        }

        foreach (var placeholder in ringRecorder.GetComponentsInChildren<PlaneAreaSpawn>())
        {
            var data = new PlaneAreaSpawnData(placeholder);
            int triggerValue = placeholder.getsTriggeredInt; // Assuming PlaceholderPlane now has a getsTriggeredInt property

            // Find the existing group for this trigger value (groups should already exist from the PlaceholderRing loop)
            PlaceholderTriggerGroup group = triggerGroups.FirstOrDefault(g => g.triggerValue == triggerValue);
            if (group == null)
            {
                group = new PlaceholderTriggerGroup { triggerValue = triggerValue };
                triggerGroups.Add(group);
            }

            group.planeAreaSpawnDataList.Add(data);
        }
    }

    public void RecordPlaceholdersForSpecifiedTrigger(GameObject ringRecorder)
    {
        // Find the trigger group that matches SpecifiedTrigger
        PlaceholderTriggerGroup specifiedGroup = triggerGroups.FirstOrDefault(g => g.triggerValue == SpecifiedTrigger);

        // If the group exists, clear its data; otherwise, create a new group
        if (specifiedGroup != null)
        {
            specifiedGroup.placeholderRingDataList.Clear();
            specifiedGroup.placeholderPlaneDataList.Clear();
            specifiedGroup.planeAreaSpawnDataList.Clear();
        }
        else
        {
            specifiedGroup = new PlaceholderTriggerGroup { triggerValue = SpecifiedTrigger };
            triggerGroups.Add(specifiedGroup);
        }

        // Record all PlaceholderRing placeholders to the specified trigger group
        foreach (var placeholder in ringRecorder.GetComponentsInChildren<PlaceholderRing>())
        {
            var data = new PlaceholderRingData(placeholder);
            specifiedGroup.placeholderRingDataList.Add(data);
        }

        specifiedGroup.placeholderRingDataList = specifiedGroup.placeholderRingDataList
                .OrderBy(data => Mathf.Abs(data.position.x))
                .ToList();

        // Record all PlaceholderPlane placeholders to the specified trigger group
        foreach (var placeholder in ringRecorder.GetComponentsInChildren<PlaceholderPlane>())
        {
            var data = new PlaceHolderPlaneData(placeholder);
            specifiedGroup.placeholderPlaneDataList.Add(data);
        }

        specifiedGroup.placeholderPlaneDataList = specifiedGroup.placeholderPlaneDataList.OrderBy(data => data.position.x).ToList();

        // Record all PlaneAreaSpawn placeholders to the specified trigger group
        foreach (var placeholder in ringRecorder.GetComponentsInChildren<PlaneAreaSpawn>())
        {
            var data = new PlaneAreaSpawnData(placeholder);
            specifiedGroup.planeAreaSpawnDataList.Add(data);
        }

        // Optionally, you can sort the data within the group here, similar to your existing RecordPlaceholders method
    }



    public void PlacePrefabsInScene()
    {
        GameObject recordedOutput = GameObject.Find("PlaceholderRecorder") ?? new GameObject("PlaceholderRecorder");

        for (int i = recordedOutput.transform.childCount - 1; i >= 0; i--)
        {
            // Use DestroyImmediate in the editor, Destroy if this needs to run at runtime
            GameObject.DestroyImmediate(recordedOutput.transform.GetChild(i).gameObject);
        }

        foreach (var group in triggerGroups)
        {
            PlaceRelvantPrefabs(group, recordedOutput);
        }


    }

    public void PlaceSpecifiedTrigger()
    {
        GameObject recordedOutput = GameObject.Find("PlaceholderRecorder") ?? new GameObject("PlaceholderRecorder");
        for (int i = recordedOutput.transform.childCount - 1; i >= 0; i--)
        {
            // Use DestroyImmediate in the editor, Destroy if this needs to run at runtime
            GameObject.DestroyImmediate(recordedOutput.transform.GetChild(i).gameObject);
        }
        PlaceholderTriggerGroup group = triggerGroups.FirstOrDefault(g => g.triggerValue == SpecifiedTrigger);
        if (group == null)
        {
            group = new PlaceholderTriggerGroup { triggerValue = SpecifiedTrigger };
            triggerGroups.Add(group);
        }
        PlaceRelvantPrefabs(triggerGroups[SpecifiedTrigger], recordedOutput);
    }


    public void PlaceRelvantPrefabs(PlaceholderTriggerGroup group, GameObject recordedOutput)
    {
        foreach (var placeholderData in group.placeholderRingDataList)
        {
            GameObject prefabInstance = Instantiate(placeholderRingPrefab, placeholderData.position, placeholderData.rotation, recordedOutput.transform);
            prefabInstance.transform.localScale = placeholderData.scale;

            PlaceholderRing placeholderScript = prefabInstance.GetComponent<PlaceholderRing>();
            if (placeholderScript != null)
            {
                placeholderScript.getsTriggeredInt = placeholderData.getsTriggeredInt;
                placeholderScript.doesTriggerInt = placeholderData.doesTriggerInt;
                placeholderScript.xCordinateTrigger = placeholderData.xCordinateTrigger;
                placeholderScript.speed = placeholderData.speed;

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
                    placeholderPlaneScript.doesTriggerInt = planeData.doesTiggerInt;
                    placeholderPlaneScript.xCordinateTrigger = planeData.xCordinateTrigger;
                    placeholderPlaneScript.speed = planeData.speed;
                }
            }
            else
            {
                Debug.LogWarning("PlaneType or FillObject is null. Skipping plane instantiation.");
            }
        }


        // Your existing logic for placing rings and planes...

        // Instantiate plane area spawn representations
        foreach (var areaData in group.planeAreaSpawnDataList)
        {
            // Assume planeAreaPrefab is a GameObject that represents your spawn area
            GameObject areaInstance = Instantiate(planeAreaPrefab, recordedOutput.transform);
            var script = areaInstance.GetComponent<PlaneAreaSpawn>();

            // Set the position and scale to represent the areaData bounds
            Vector3 position = new Vector3((areaData.minX + areaData.maxX) / 2, (areaData.minY + areaData.maxY) / 2, 0);
            Vector3 scale = new Vector3(areaData.maxX - areaData.minX, areaData.maxY - areaData.minY, 1);

            areaInstance.transform.position = position;
            areaInstance.transform.localScale = scale;

            script.getsTriggeredInt = areaData.getsTriggeredInt;
            script.minPlanes = areaData.minPlanes;
            script.maxPlanes = areaData.maxPlanes;
            script.cropChance = areaData.cropChance;
            script.jetChance = areaData.jetChance;
            script.cargoChance = areaData.cargoChance;
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