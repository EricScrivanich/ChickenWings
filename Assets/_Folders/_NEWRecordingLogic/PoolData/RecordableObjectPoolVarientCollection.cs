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
            Debug.LogError("Spawning varient type: " + type + " at pos: " + data.GetStartPos());
            var obj = Instantiate(prefabVarients[type - 1], data.GetStartPos(), Quaternion.identity).GetComponent<SpawnedObject>();
            obj.InitialSpawnCheck(spawnNumber, true);

            data.ApplyTo(obj);
        }


    }

    public override void SpawnBoss<T>(T data, bool hasTrigger)
    {
        var type = data.GetType();
        SpawnedObject obj;
        if (type <= 0)
        {

            if (instantiateOnly)
            {
                Debug.Log("Instantiating object at position: " + data.GetStartPos());
                obj = Instantiate(prefab, data.GetStartPos(), Quaternion.identity).GetComponent<SpawnedObject>();


            }
            else if (pool != null || pool.Length > 0)
            {
                obj = pool[currentIndex];
                currentIndex = (currentIndex + 1) % pool.Length;

            }

            else
                return;



            obj.InitialSpawnCheck(spawnNumber, instantiateOnly);

            data.ApplyTo(obj);


            spawnNumber++;

        }
        else
        {
            Debug.Log("Instantiating object at position: " + data.GetStartPos());
            obj = Instantiate(prefabVarients[type - 1], data.GetStartPos(), Quaternion.identity).GetComponent<SpawnedObject>();






            obj.InitialSpawnCheck(spawnNumber, true);
        }



        var pigBoss = obj.GetComponent<SpawnedPigBossObject>();
        if (hasTrigger)
            pigBoss?.SetTriggerWhenDead(levelData.levelDataBossAndRandomLogic);
        pigBoss?.SetHealth(data.GetID());

        data.ApplyTo(obj);



    }



}
