using UnityEngine;

[CreateAssetMenu(fileName = "AllObjectData", menuName = "ScriptableObjects/AllObjectData")]
public class AllObjectData : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [field: SerializeField] public RecordableObjectPool[] pools { get; private set; }
    [field: SerializeField] public GameObject cageObject { get; private set; }

    [field: SerializeField] public RingPool ringPool { get; private set; }
}
