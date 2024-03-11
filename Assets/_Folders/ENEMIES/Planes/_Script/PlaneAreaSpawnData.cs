
using UnityEngine;

[System.Serializable]
public class PlaneAreaSpawnData 
{
    public int getsTriggeredInt;
    public int minPlanes;
    public int maxPlanes;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public float cropChance;
    public float jetChance;
    public float cargoChance;

    public PlaneAreaSpawnData (PlaneAreaSpawn area)
    {
        getsTriggeredInt = area.getsTriggeredInt;
        minPlanes = area.minPlanes;
        maxPlanes = area.maxPlanes;
        minX = area.GetMinX();
        maxX = area.GetMaxX();
        minY = area.GetMinY();
        maxY = area.GetMaxY();
        cropChance = area.cropChance;
        jetChance = area.jetChance;
        cargoChance = area.cargoChance;

    }
}
