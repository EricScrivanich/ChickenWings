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
    public short[] dataTypes;

    public float[] floatList;
    public ushort[] poolSizes;
    public ushort[] cageAttachments;

   


    public short[] startingAmmos;
    public short StartingLives;

    public RecordedObjectPositionerDataSave[] postionerData = null;

    public LevelDataSave(string levelName, short[] idList, ushort[] spawnSteps, ushort[] typeList, short[] dataTypeList, ushort finalStep, Vector2[] posList, float[] floats, ushort[] plSizes, short[] ammos, short lives)
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
        this.cageAttachments = null;
        this.postionerData = null;

    }
    public void SetPositionerData(RecordedObjectPositionerDataSave[] data)
    {
        if (data == null || data.Length == 0)
        {
            Debug.LogError("Positioner data is null or empty, not setting it.");
            return;
        }
        this.postionerData = data;
    }
    public void SetCageAttachments(ushort[] cageAttachments)
    {
        if (cageAttachments == null || cageAttachments.Length == 0)
        {
            Debug.LogError("Cage attachments data is null or empty, not setting it.");
            return;
        }
        this.cageAttachments = cageAttachments;
    }



}
