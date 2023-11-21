using System.Collections.Generic;
using UnityEngine;

public class RingSpawner : MonoBehaviour
{
    public static RingSpawner Instance;
    public static int currentRing;
    private int correctRing;

    [SerializeField] private GameObject ringPrefab;
    [SerializeField] private List<GameObject> ringSetups;
    private List<GameObject> ringPool;
    private int ringAmount = 0;

    private const int ringPoolSize = 20;
    [SerializeField] private float spawnInterval = 5f;

    private void Awake()
    {
        Instance = this;
        correctRing = 1;
    }

    private void Start()
    {
        ringPool = new List<GameObject>();
        for (int i = 0; i < ringPoolSize; i++)
        {
            GameObject ring = Instantiate(ringPrefab);
            ring.SetActive(false);
            ringPool.Add(ring);
        }

        InvokeRepeating("SpawnRandomRingSetup", 0f, spawnInterval);
    }
    private void OnEnable() 
    {
        RingMovement.OnRingPassed += CheckOrder;
 
    }
    private void OnDisable() 
    {
         RingMovement.OnRingPassed -= CheckOrder;
    
    }
   private void CheckOrder(int ringOrder)
    {
        if (correctRing == ringAmount)
        {
            print("you did it");
            correctRing = 1;

        }
        else if (ringOrder == correctRing)
        {
            print("yes");
            correctRing++;
        }
        else
        {
            print("sorry");
            correctRing = 1;
        }
        
    }
    public void ReturnRingToPool(GameObject ring)
    {
        ring.SetActive(false);
        ringPool.Add(ring);
    }

    private void SpawnRandomRingSetup()
    {
        int randomIndex = Random.Range(0, ringSetups.Count);
        GameObject ringSetupPrefab = ringSetups[randomIndex];
        GameObject ringSetupInstance = Instantiate(ringSetupPrefab, new Vector3(BoundariesManager.rightBoundary, 0, 0), Quaternion.identity);

        int orderCounter = 0;
        ringAmount = 0;

        List<Transform> sortedChildren = new List<Transform>();
        foreach (Transform child in ringSetupInstance.transform)
        {
            sortedChildren.Add(child);
            ringAmount +=1;
        }
       sortedChildren.Sort((a, b) => a.position.x.CompareTo(b.position.x));

        foreach (Transform placeholder in sortedChildren)
        {
            GameObject ring = GetRingFromPool();
            if (ring != null)
            {
                ring.transform.position = placeholder.position;
                ring.transform.rotation = placeholder.rotation;

                RingMovement ringScript = ring.GetComponent<RingMovement>();
                ringScript.order = ++orderCounter;
                

                ring.SetActive(true);
                Destroy(placeholder.gameObject);
            }
        }
    }

    private GameObject GetRingFromPool()
    {
        foreach (GameObject ring in ringPool)
        {
            if (!ring.activeInHierarchy)
            {
                return ring;
            }
        }

        return null;
    }
}
