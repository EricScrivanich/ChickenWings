using System.Collections.Generic;
[System.Serializable]
public class PlaceholderTriggerGroup
{
    public int triggerValue; 
    public List<PlaceholderRingData> placeholderRingDataList = new List<PlaceholderRingData>();
    public List<PlaceHolderPlaneData> placeholderPlaneDataList = new List<PlaceHolderPlaneData>();
    public List<PlaneAreaSpawnData> planeAreaSpawnDataList = new List<PlaneAreaSpawnData>();

}