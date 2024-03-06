
using UnityEngine;

[System.Serializable]
public class PlaceholderRingData
{


   
    public int getsTriggeredInt;
    public int doesTriggerInt;
    public float xCordinateTrigger;
    public float speed;
    public Vector2 position; // To store the position
    public Quaternion rotation; // To store the rotation
    public Vector2 scale;

    public PlaceholderRingData(PlaceholderRing placeholder)
    {
        
        getsTriggeredInt = placeholder.getsTriggeredInt;
        doesTriggerInt = placeholder.doesTriggerInt;
        xCordinateTrigger = placeholder.xCordinateTrigger;
        speed = placeholder.speed;
        position = placeholder.transform.position; // Capture the position from the transform
        rotation = placeholder.transform.rotation; // Capture the rotation from the transform
        scale = placeholder.transform.localScale; // Capture the scale from the transfo
    }
}