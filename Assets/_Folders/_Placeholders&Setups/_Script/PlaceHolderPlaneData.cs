
using UnityEngine;
[System.Serializable]

public class PlaceHolderPlaneData 
{
    public PlaneData planeType;
    public float speed;
    public Vector2 position;

    public PlaceHolderPlaneData(PlaceholderPlane placeholder)
    {
        planeType = placeholder.planeType;
        speed = placeholder.speed;
        position = placeholder.transform.position;
    }

}
