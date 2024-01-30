using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RingPool : ScriptableObject
{
    public bool Testing;
    [SerializeField] private int currentTriggerEdited;
    private int lastCurrentTrigger;
    [SerializeField] private bool showAllPlaceholders;
    private bool lastShowAllPlaceholders;
    // Start is called before the first frame update
    public GameObject ringPrefab;
    public GameObject bucketPrefab;

    private int ringAmount = 25;
    private int bucketAmount = 4;





    // private Queue<GameObject> ringPool;
    private List<BucketScript> bucketPool;
    private List<RingMovement> ringPool;

    private Transform parent;

    public List<RingID> RingType;

    public void SpawnRingPool()
    {
        ringPool = new List<RingMovement>();
        // if (ringPool == null || ringPool.Count == 0)
        // {

        // }

        // if (ringPool.Count >= ringAmount)
        // {
        //     return;
        // }

        if (!parent)
        {
            parent = new GameObject("RingPool").transform;
        }

        while (ringPool.Count < ringAmount)
        {
            // GameObject obj = Instantiate(prefab, parent);
            GameObject obj = Instantiate(ringPrefab, parent);
            obj.gameObject.SetActive(false);
            RingMovement ringScript = obj.GetComponent<RingMovement>();
            ringPool.Add(ringScript);
        }
    }


    public RingMovement GetRing(RingID ID)
    {
        if (ringPool == null || ringPool.Count == 0)
        {
            SpawnRingPool();
            Debug.LogWarning($"{name} spawned mid-game. Consider spawning it at the start of the game");
        }
        foreach (RingMovement ringScript in ringPool)
        {
            if (!ringScript.gameObject.activeInHierarchy)
            {
                ringScript.ID = ID;
                return ringScript;
            }
        }
        return null;
    }

    public void SpawnBucketPool()
    {
        Debug.Log("BucketSPawning");
        bucketPool = new List<BucketScript>();
        // if (bucketPool == null || bucketPool.Count == 0)
        // {
        //     bucketPool = new List<BucketScript>();
        // }
        // if (bucketPool.Count >= bucketAmount)
        // {
        //     return;
        // }
        if (!parent)
        {
            parent = new GameObject("BucketPool").transform;
        }
        while (bucketPool.Count < bucketAmount)
        {
            // GameObject obj = Instantiate(prefab, parent);
            GameObject obj = Instantiate(bucketPrefab, parent);
            obj.gameObject.SetActive(false);
            BucketScript bucketScript = obj.GetComponent<BucketScript>();
            bucketPool.Add(bucketScript);
        }
    }

    public BucketScript GetBucket(RingID ID)
    {
        if (bucketPool == null || bucketPool.Count == 0)
        {
            SpawnBucketPool();
            Debug.LogWarning($"{name} spawned mid-game. Consider spawning it at the start of the game");
        }
        foreach (BucketScript bucketScript in bucketPool)
        {
            if (!bucketScript.gameObject.activeInHierarchy)
            {
                bucketScript.ID = ID;
                return bucketScript;
            }
        }
        return null;
    }

    private void OnEnable()
    {

        if (showAllPlaceholders)
        {
            ShowAllPlaceholders();
            lastCurrentTrigger = currentTriggerEdited;
        }
        else
        {
            ShowCorrectPlaceholders();

        }


    }
    private void OnValidate()
    {
        if (lastCurrentTrigger != currentTriggerEdited)
        {
            ShowCorrectPlaceholders();

        }

        else if (showAllPlaceholders != lastShowAllPlaceholders)
        {
            if (showAllPlaceholders)
            {
                ShowAllPlaceholders();

            }

            else
            {
                ShowCorrectPlaceholders();
            }


        }

    }
    private void ShowAllPlaceholders()
    {
        foreach (PlaceholderRing placeholder in FindObjectsOfType<PlaceholderRing>())
        {
            placeholder.gameObject.layer = LayerMask.NameToLayer("PlayerEnemy");
        }
        lastShowAllPlaceholders = showAllPlaceholders;

    }

    private void ShowCorrectPlaceholders()
    {
        foreach (PlaceholderRing placeholder in FindObjectsOfType<PlaceholderRing>())
        {
            // Check and update the layer as needed
            if (placeholder.getsTriggeredInt == currentTriggerEdited)
            {
                placeholder.gameObject.layer = LayerMask.NameToLayer("PlayerEnemy");
            }
            else
            {
                placeholder.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }
        lastCurrentTrigger = currentTriggerEdited;
        showAllPlaceholders = false;
        lastShowAllPlaceholders = showAllPlaceholders;

    }
}
