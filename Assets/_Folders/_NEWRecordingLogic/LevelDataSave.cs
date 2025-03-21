using UnityEngine;
[System.Serializable]
public class LevelDataSave
{
    public string levelName;
    public string levelWorldAndNumber;
    public short[] idList;
    public ushort[] spawnSteps;
    public Vector2[] posList;
    public float[] speeds;
    public float[] scaleList;
    public float[] magList;
    public float[] timeList;
    public float[] delayList;
    public short[] typeList;

    public LevelDataSave(string levelName, string levelWorldAndNumber, short[] idList, ushort[] spawnSteps, Vector2[] posList, float[] speeds, float[] scaleList, float[] magList, float[] timeList, float[] delayList, short[] typeList)
    {
        this.levelName = levelName;
        this.levelWorldAndNumber = levelWorldAndNumber;
        this.idList = idList;
        this.spawnSteps = spawnSteps;
        this.posList = posList;
        this.speeds = speeds;
        this.scaleList = scaleList;
        this.magList = magList;
        this.timeList = timeList;
        this.delayList = delayList;
        this.typeList = typeList;
    }
    
    
}
