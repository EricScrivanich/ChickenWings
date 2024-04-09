using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MountainTriggers))]
public class CameraBoundsGizmoDrawer : Editor
{
    void OnSceneGUI()
    {
        MountainTriggers trigger = (MountainTriggers)target;

        if (trigger.gameObject.name == "TriggerObject")
        {
            MountainTriggers nearestTriggerForZoom = FindNearestTriggerWithActiveBool(trigger, true);
            MountainTriggers nearestTriggerForCamera = FindNearestTriggerWithActiveBool(trigger, false);

            float halfCameraHeight = nearestTriggerForZoom != null ? nearestTriggerForZoom.TargetZoomAmount : trigger.TargetZoomAmount;
            Vector2 cameraPosition = nearestTriggerForCamera != null ? nearestTriggerForCamera.TargetCameraPosition : trigger.TargetCameraPosition;

            // Calculate top and bottom positions based on the camera's height
            Vector3 topPosition = new Vector3(cameraPosition.x, cameraPosition.y + halfCameraHeight, 0);
            Vector3 bottomPosition = new Vector3(cameraPosition.x, cameraPosition.y - halfCameraHeight, 0);

            // Draw the lines using Handles
            Handles.color = Color.red;
            Handles.DrawLine(new Vector3(-10000, topPosition.y, 0), new Vector3(10000, topPosition.y, 0));
            Handles.DrawLine(new Vector3(-10000, bottomPosition.y, 0), new Vector3(10000, bottomPosition.y, 0));
        }
    }

    private MountainTriggers FindNearestTriggerWithActiveBool(MountainTriggers currentTrigger, bool searchingForZoom)
    {
        MountainTriggers[] allTriggers = FindObjectsOfType<MountainTriggers>();
        MountainTriggers nearestTrigger = null;
        float nearestDistance = float.MaxValue;

        foreach (MountainTriggers trigger in allTriggers)
        {
            if (trigger == currentTrigger) continue;

            Vector3 directionToTrigger = trigger.transform.position - currentTrigger.transform.position;
            if (directionToTrigger.x < 0) // Trigger is to the left
            {
                // Check if the bool we are interested in is true for this trigger
                bool isActive = searchingForZoom ? trigger.ChangeZoomEnabled : trigger.ChangeCameraEnabled;
                if (!isActive) continue; // Skip this trigger if the bool is not active

                float distance = directionToTrigger.magnitude;
                if (distance < nearestDistance)
                {
                    nearestTrigger = trigger;
                    nearestDistance = distance;
                }
            }
        }

        // If we found a trigger with the active bool, return it
        if (nearestTrigger != null)
        {
            return nearestTrigger;
        }

        // If no trigger was found with the active bool, search again excluding the currentTrigger from the list
        foreach (MountainTriggers trigger in allTriggers)
        {
            if (trigger == currentTrigger || trigger == nearestTrigger) continue;

            Vector3 directionToTrigger = trigger.transform.position - currentTrigger.transform.position;
            if (directionToTrigger.x < 0) // Trigger is to the left
            {
                // This time, don't check if the bool is active. Just find the nearest to the left.
                float distance = directionToTrigger.magnitude;
                if (distance < nearestDistance)
                {
                    nearestTrigger = trigger;
                    nearestDistance = distance;
                }
            }
        }

        // Return the nearest trigger to the left, regardless of the bool status
        return nearestTrigger;
    }

}
