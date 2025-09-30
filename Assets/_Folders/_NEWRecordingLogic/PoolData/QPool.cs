using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "RecordableObjectPool", menuName = "ScriptableObjects/QPool")]
public class QPool : ScriptableObject
{
    private Queue<SpawnedQueuedObject> q;
    [SerializeField] private GameObject prefab;
    private Vector3 initialSpawnPosition = new Vector3(0, BoundariesManager.GroundPosition - 10, 0);

    public void Initialize()
    {


        Debug.Log("Initializing QPool: " + prefab.name);
        q = new Queue<SpawnedQueuedObject>();
        var obj = Instantiate(prefab, initialSpawnPosition, Quaternion.identity).GetComponent<SpawnedQueuedObject>();
        obj.Initialize(this);
        obj.gameObject.SetActive(false);

    }
    public SpawnedQueuedObject GetObject()
    {
       
        var obj = q.Dequeue();
        
        if (q.Count <= 0)
        {
            Debug.LogWarning("QPool is empty, creating a new object.");
            var o = Instantiate(prefab, initialSpawnPosition, Quaternion.identity).GetComponent<SpawnedQueuedObject>();
            o.Initialize(this);
            o.gameObject.SetActive(false);

        }
        return obj;

    }

    public void ReturnObject(SpawnedQueuedObject obj)
    {
        q.Enqueue(obj);
    }
    public void Spawn(Vector2 pos)
    {


        var obj = GetObject();
        obj.ResetRB();
        obj.Spawn(pos);
    }
    public void SpawnWithRotationAndSpeed(Vector2 pos, Quaternion rot, float speed = 0, ushort side = 5)
    {
        var obj = GetObject();
        obj.ResetRB();
        obj.SpawnWithRotationAndSpeed(pos, rot, speed, side);
    }
    public void SpawnWithVelocity(Vector2 pos, Vector2 velocity)
    {
        var obj = GetObject();
        obj.ResetRB();
        obj.SpawnWithVelocity(pos, velocity);
    }

    public void SpawnSpecial(Vector2 pos, float speed, bool flipped)
    {
        var obj = GetObject();
        obj.ResetRB();
        obj.SpawnSpecial(pos, speed, flipped);

    }

    public void SpawnWithVelocityAndRotation(Vector2 pos, Vector2 velocity, float rotation, float angularVelocity = 0)
    {
        var obj = GetObject();
        obj.ResetRB();
        obj.SpawnWithVelocityAndRotation(pos, velocity, rotation, angularVelocity);
    }
    public void SpawnWithVelocityAndRotationAndScale(Vector2 pos, Vector2 velocity, float rotation, float scale)
    {
        var obj = GetObject();
        obj.ResetRB();
        obj.SpawnWithVelocityAndRotationAndScale(pos, velocity, rotation, scale);
    }

}

// Start is called once before the first execution of Update after the MonoBehaviour is created


