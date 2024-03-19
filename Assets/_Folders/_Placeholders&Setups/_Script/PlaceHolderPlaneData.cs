
using UnityEngine;
[System.Serializable]

public class PlaceHolderPlaneData 
{
    public PlaneData planeType;
    public int doesTiggerInt;
    public float xCordinateTrigger;
    public float speed;
    public Vector2 position;
    

    public PlaceHolderPlaneData(PlaceholderPlane placeholder)
    {
        planeType = placeholder.planeType;
        doesTiggerInt = placeholder.doesTriggerInt;
        xCordinateTrigger = placeholder.xCordinateTrigger;
        speed = placeholder.speed;
        position = placeholder.transform.position;
    }

}
