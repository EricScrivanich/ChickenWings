using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HellTap.PoolKit;

[CreateAssetMenu]
public class PlaneData : ScriptableObject
{
    [SerializeField] private GameObject pigVersion;
    private Pool pigPool;
    public PlaneManagerID ID;
    private Pool explosionPool;

    public int PlanesAvailable;
    [SerializeField] private float startingMinOffsetTime;
    [SerializeField] private float startingMaxOffsetTime;
    [SerializeField] private float startingMinIterationDelay;
    [SerializeField] private float startingMaxIterationDelay;

    public float minOffsetTime;
    public float maxOffsetTime;
    public float minIterationDelay;
    public float maxIterationDelay;

    public GameObject fillObject;
    public int PlaneIndex;
    public Vector3 explosionScale;
    public float speed;
    public int poolSize;
    public float speedOffset;
    public float planeSinFrequency;
    public float planeSinAmplitude;
    public float minSpawn;
    public float maxSpawn;
    public float planeTime;
    public float planeTimeOffset;
    private Transform parent;
    private List<LOOP_PlaneMovement> planePool;
    public int lives;
    public GameObject PlaneObject;


    public void ResetValues()
    {
        explosionPool = PoolKit.GetPool("ExplosionPool");
        pigPool = PoolKit.GetPool("PigPool");
        PlanesAvailable = 1;
        minOffsetTime = startingMinOffsetTime;
        maxOffsetTime = startingMaxOffsetTime;
        minIterationDelay = startingMinIterationDelay;
        maxIterationDelay = startingMaxIterationDelay;
    }
    public void SpawnPlanePool()
    {

        planePool = new List<LOOP_PlaneMovement>();

        if (!parent)
        {
            parent = new GameObject(name).transform;
        }

        while (planePool.Count < poolSize)
        {
            GameObject obj = Instantiate(PlaneObject, parent);
            obj.gameObject.SetActive(false);
            LOOP_PlaneMovement planeScript = obj.GetComponent<LOOP_PlaneMovement>();
            planePool.Add(planeScript);
        }

    }

    public void GetPlane(int trigger, float xCorTrigger, float speedVar, Vector2 spawnPosition)
    {
        // if (planePool == null || planePool.Count == 0)
        // {
        //     SpawnPlanePool();
        //     Debug.LogWarning($"{name} spawned mid-game. Consider spawning it at the start of the game");
        // }

        // foreach (LOOP_PlaneMovement planeScript in planePool)
        // {
        //     if (!planeScript.gameObject.activeInHierarchy)
        //     {
        //         if (speedVar == 0)
        //         {
        //             speedVar = speed + Random.Range(-speedOffset, speedOffset);
        //         }
        //         planeScript.doesTiggerInt = trigger;
        //         planeScript.xCordinateTrigger = xCorTrigger;
        //         planeScript.speed = speedVar;
        //         planeScript.transform.position = spawnPosition;
        //         planeScript.gameObject.SetActive(true);
        //         break;

        //     }

        // }

        pigPool.Spawn(pigVersion, spawnPosition, Vector3.zero);
    }

    public void GetExplosion(Vector2 position)
    {
        // ID.GetExplosion(position, explosionScale);

        explosionPool.Spawn("NormalExplosion", position, Vector3.zero, explosionScale, null);


    }

}
