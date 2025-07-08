using UnityEngine;

[CreateAssetMenu(fileName = "AllObjectData", menuName = "ScriptableObjects/AllObjectData")]
public class AllObjectData : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [field: SerializeField] public RecordableObjectPool[] pools { get; private set; }
    [field: SerializeField] public GameObject cageObject { get; private set; }

    [field: SerializeField] public RingPool ringPool { get; private set; }
    [field: SerializeField] public CheckPointFlag checkPointFlag { get; private set; }


    public void SpawnCheckPoint(bool hasCollected)
    {
        var o = Instantiate(checkPointFlag, new Vector2(BoundariesManager.rightBoundary, -.49f), Quaternion.Euler(new Vector3(0, 0, 90)));
        o.Initialize(hasCollected);
    }

}
