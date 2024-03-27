using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class PlaneData : ScriptableObject
{
    public PlaneManagerID ID;
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
        if (planePool == null || planePool.Count == 0)
        {
            SpawnPlanePool();
            Debug.LogWarning($"{name} spawned mid-game. Consider spawning it at the start of the game");
        }

        foreach (LOOP_PlaneMovement planeScript in planePool)
        {
            if (!planeScript.gameObject.activeInHierarchy)
            {
                if (speedVar == 0)
                {
                    speedVar = speed + Random.Range(-speedOffset, speedOffset);
                }
planeScript.doesTiggerInt = trigger;
planeScript.xCordinateTrigger = xCorTrigger;
                planeScript.speed = speedVar;
                planeScript.transform.position = spawnPosition;
                planeScript.gameObject.SetActive(true);
                break;

            }

        }
    }

    public void GetExplosion(Vector2 position)
    {
        ID.GetExplosion(position,explosionScale); 

        
    }

}
