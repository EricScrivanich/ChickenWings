using UnityEngine;
[CreateAssetMenu(fileName = "Level Data Arrays", menuName = "Setups/LevelDataArrays")]
public class LevelDataArrays : ScriptableObject
{
    [field: SerializeField] public short[] StartingAmmos { get; private set; }
    [field: SerializeField] public short StartingLives { get; private set; }

    [field: SerializeField] public short[] easyStartingAmmos { get; private set; }
    [field: SerializeField] public short easyStartingLives { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ushort[] pigPoolSizes;
    public ushort[] aiPoolSizes;
    public ushort[] buildingPoolSizes;
    public ushort[] collectablePoolSizes;
    public ushort[] positionerPoolSizes;
    public ushort[] ringPoolSizes;
    public ushort[] cageAttachments;
    public Vector2[] posList;
    public float[] floatList;
    public ushort[] typeList;

    public void LoadLevelSaveData(LevelDataSave lds, PlayerStartingStatsForLevels startingStats)
    {
        if (lds == null)
        {
            Debug.LogError("LevelDataSave is null");
           
            posList = new Vector2[0];
           
            floatList = new float[0];
            StartingAmmos = new short[5];
            StartingAmmos[0] = 3;
            StartingAmmos[1] = 3;
            StartingLives = 3;
           
            pigPoolSizes = new ushort[0];
            aiPoolSizes = new ushort[0];
            buildingPoolSizes = new ushort[0];
            collectablePoolSizes = new ushort[0];
            positionerPoolSizes = new ushort[0];
            ringPoolSizes = new ushort[0];



            return;
        }
        Debug.LogError("Loading Level Data: " + lds.levelName + " with " + lds.spawnSteps.Length + " spawn steps.");
        
        // levelWorldAndNumber = lds.levelWorldAndNumber;
        
        floatList = lds.floatList;
       
        cageAttachments = lds.cageAttachments;
        pigPoolSizes = lds.pigPoolSizes;
        aiPoolSizes = lds.aiPoolSizes;
        buildingPoolSizes = lds.buildingPoolSizes;
        collectablePoolSizes = lds.collectablePoolSizes;
        positionerPoolSizes = lds.positonerPoolSizes;
        ringPoolSizes = lds.ringPoolSizes;

        posList = lds.posList;
        
        // speedList = lds.speeds;
        // scaleList = lds.scaleList;
        // magList = lds.magList;
        // timeList = lds.timeList;
        // delayList = lds.delayList;
        typeList = lds.typeList;
        
       

        if (lds.startingAmmos == null || lds.startingAmmos.Length <= 0)
        {
            Debug.LogError("Starting Ammos is null or empty, setting default values.");
            StartingAmmos = new short[5];
            StartingAmmos[0] = 3;
            StartingAmmos[1] = 3;

            startingStats.startingAmmos = new short[5];
            startingStats.startingAmmos[0] = 3;
            startingStats.startingAmmos[1] = 3;

        }
        else
            StartingAmmos = lds.startingAmmos;

        if (lds.StartingLives <= 0)
            StartingLives = 3;
        else
            StartingLives = lds.StartingLives;

        startingStats.SetData(StartingLives, StartingAmmos, -1);


       
    }


}
