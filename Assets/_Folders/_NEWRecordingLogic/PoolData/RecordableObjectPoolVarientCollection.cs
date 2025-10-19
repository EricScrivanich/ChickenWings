using UnityEngine;
[CreateAssetMenu(fileName = "RecordableObjectPoolVarientCollection", menuName = "ScriptableObjects/RecordableObjectPoolVarientCollection")]
public class RecordableObjectPoolVarientCollection : RecordableObjectPool
{
    [SerializeField] private GameObject[] prefabVarients;
    [SerializeField] private bool instantiateVarientsOnly = false;

    public override void Spawn<T>(T data)
    {
        var type = data.GetType();

        if (type <= 0)
        {
            base.Spawn(data);

        }
        else
        {
            var obj = Instantiate(prefabVarients[type - 1], data.GetStartPos(), Quaternion.identity).GetComponent<SpawnedObject>();
            obj.InitialSpawnCheck(spawnNumber, instantiateOnly);

            data.ApplyTo(obj);
        }

       
    }



}
