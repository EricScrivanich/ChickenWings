using UnityEngine;
[System.Serializable]
public class LevelDataSave
{
    public string levelName;
    public short[] idList;
    public ushort[] spawnSteps;
    public ushort finalSpawnStep;
    public ushort[] typeList;

    public Vector2[] posList;
    public ushort[] dataTypes;

    public float[] floatList;
    public ushort[] poolSizes;

   


    public short[] startingAmmos;
    public short StartingLives;

    public RecordedObjectPositionerDataSave[] postionerData = null;

    public LevelDataSave(string levelName, short[] idList, ushort[] spawnSteps, ushort[] typeList, ushort[] dataTypeList, ushort finalStep, Vector2[] posList, float[] floats, ushort[] plSizes, short[] ammos, short lives)
    {
        this.levelName = levelName;
        this.idList = idList;
        this.typeList = typeList;
        this.dataTypes = dataTypeList;
        this.posList = posList;
        this.spawnSteps = spawnSteps;
        this.finalSpawnStep = finalStep;
        this.floatList = floats;
        this.poolSizes = plSizes;
        this.startingAmmos = ammos;
        this.StartingLives = lives;
    }
    public void SetPositionerData(RecordedObjectPositionerDataSave[] data)
    {
        this.postionerData = data;
    }


}
