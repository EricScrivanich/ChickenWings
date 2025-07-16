using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class LevelDataSave
{
    public string levelName;
    public short[] objectTypes;
    public short[] idList;
    public ushort[] spawnSteps;
    public ushort finalSpawnStep;
    public ushort[] typeList;

    public Vector2[] posList;
    public short[] dataTypes;

    public float[] floatList;
    public ushort[] poolSizes;
    public ushort[] pigPoolSizes;
    public ushort[] aiPoolSizes;
    public ushort[] buildingPoolSizes;
    public ushort[] collectablePoolSizes;
    public ushort[] positonerPoolSizes;
    public ushort[] ringPoolSizes;




    public ushort[] cageAttachments;

    




    public short[] startingAmmos;
    public short StartingLives;

    public RecordedObjectPositionerDataSave[] postionerData = null;

    public LevelDataSave(string levelName, short[] objectTypes, short[] idList, ushort[] spawnSteps, ushort[] typeList, short[] dataTypeList, ushort finalStep, Vector2[] posList, float[] floats, List<ushort[]> plList, short[] ammos, short lives, ushort[] plSizes = null)
    {
        this.levelName = levelName;
        this.objectTypes = objectTypes;
        this.idList = idList;
        this.typeList = typeList;
        this.dataTypes = dataTypeList;
        this.posList = posList;
        this.spawnSteps = spawnSteps;
        this.finalSpawnStep = finalStep;
        this.floatList = floats;
        this.poolSizes = plSizes;
        this.pigPoolSizes = plList[0];
        this.aiPoolSizes = plList[1];
        this.buildingPoolSizes = plList[2];
        this.collectablePoolSizes = plList[3];
        this.positonerPoolSizes = plList[4];
        this.ringPoolSizes = plList[5];
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
