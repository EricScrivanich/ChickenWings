using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class RingParentSpawner : MonoBehaviour
{
    private Pool ringPool;
    [SerializeField] private int Index;
    public int currentRingParentType;
    public float waitTime;
    private Coroutine currentCourintine;

    [SerializeField] private List<GameObject> ringParentTypes;
    [SerializeField] private List<Vector2> minMax;
    // Start is called before the first frame update
    void Start()
    {
        ringPool = PoolKit.GetPool("RingPool");
        // SpawnRings(Index);

    }

    

    public void SpawnRings(int index)
    {

        currentCourintine = StartCoroutine(SpawnRingsCourintine(index));

    }

    private IEnumerator SpawnRingsCourintine(int index)
    {
        while (true)
        {
            Vector2 position = new Vector2(BoundariesManager.rightBoundary, Random.Range(minMax[index].x, minMax[index].y));
            ringPool.Spawn(ringParentTypes[index], position, Vector3.zero);
            yield return new WaitForSeconds(waitTime);
        }


    }

    // Update is called once per frame
    void Update()
    {

    }
}
