using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "QPoolEffect", menuName = "ScriptableObjects/QPoolEffect")]
public class QEffectPool : ScriptableObject
{
    private Queue<SpawnedQueuedEffect> q;
    [SerializeField] private bool instantiateOnly;
    [SerializeField] private GameObject prefab;
    private Vector3 initialSpawnPosition = new Vector3(0, BoundariesManager.GroundPosition - 10, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {

        if (instantiateOnly) return;

        q = new Queue<SpawnedQueuedEffect>();
        var obj = Instantiate(prefab, initialSpawnPosition, Quaternion.identity).GetComponent<SpawnedQueuedEffect>();
        obj.Initialize(this);
        obj.gameObject.SetActive(false);
        // if (q.Count <= 0)
        //     q.Enqueue(obj);

    }
    public SpawnedQueuedEffect GetObject()
    {

        var obj = q.Dequeue();

        if (q.Count <= 0)
        {
            Debug.LogWarning("QPool is empty, creating a new object.");
            var o = Instantiate(prefab, initialSpawnPosition, Quaternion.identity).GetComponent<SpawnedQueuedEffect>();
            o.Initialize(this);
            o.gameObject.SetActive(false);

        }
        return obj;

    }

    public void ReturnObject(SpawnedQueuedEffect obj)
    {
        q.Enqueue(obj);

    }
    public void Spawn(Vector2 pos)
    {
        if (instantiateOnly)
        {
            var o = Instantiate(prefab, pos, Quaternion.identity);
            return;
        }
        var obj = GetObject();
        obj.Spawn(pos);
    }
    public void SpawnUniformScale(Vector2 pos, float scale)
    {


        var obj = GetObject();

        obj.Spawn(pos, scale);
    }
    public void SpawnSpecificScale(Vector2 pos, Vector3 scale)
    {


        var obj = GetObject();

        obj.Spawn(pos, scale);
    }
}
