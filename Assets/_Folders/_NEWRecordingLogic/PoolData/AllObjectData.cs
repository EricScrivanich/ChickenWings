using UnityEngine;

[CreateAssetMenu(fileName = "AllObjectData", menuName = "ScriptableObjects/AllObjectData")]
public class AllObjectData : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [field: SerializeField] public RecordableObjectPool[] pools { get; private set; }

    [Header("Pools, Pigs 0, AI 1, Buildings 2, Collectables 3, Positioners 4, Rings 5")]

    [field: SerializeField] public RecordableObjectPool[] pigPools { get; private set; }
    [field: SerializeField] public RecordableObjectPool[] aiPools { get; private set; }
    [field: SerializeField] public RecordableObjectPool[] builidngPools { get; private set; }
    [field: SerializeField] public RecordableObjectPool[] collectablePools { get; private set; }
    [field: SerializeField] public RecordableObjectPool[] positionerPools { get; private set; }
    [field: SerializeField] public RingPool ringPool { get; private set; }

    [Header("Prefabs")]
    [field: SerializeField] public GameObject cageObject { get; private set; }
    [field: SerializeField] public CheckPointFlag checkPointFlag { get; private set; }
    [field: SerializeField] public GameObject finishLine { get; private set; }
    [field: SerializeField] public QPool[] QPools { get; private set; }

    public void InitializeQPools()
    {
        foreach (var qPool in QPools)
        {
            qPool.Initialize();
        }
    }

    public RecordableObjectPool[] GetPoolArrayByObjectType(int objectType)
    {
        return objectType switch
        {
            0 => pigPools,
            1 => aiPools,
            2 => builidngPools,
            3 => collectablePools,
            4 => positionerPools,
            _ => pools // fallback or shared/global pool list
        };
    }
    public void SpawnCheckPoint(bool hasCollected, ushort checkPointNumber, LevelData lvlData)
    {
        var o = Instantiate(checkPointFlag, new Vector2(BoundariesManager.rightBoundary, -.49f), Quaternion.Euler(new Vector3(0, 0, 90)));
        o.Initialize(hasCollected, checkPointNumber, lvlData);
    }
    public void SpawnFinishLine()
    {
        Instantiate(finishLine, new Vector2(BoundariesManager.rightBoundary, 0f), Quaternion.identity);
    }

}
