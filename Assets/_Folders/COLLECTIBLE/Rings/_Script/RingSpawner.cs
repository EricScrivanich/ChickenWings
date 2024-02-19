using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;


public class RingSpawner : MonoBehaviour
{
    public static RingSpawner Instance;
    [ExposedScriptableObject]
    public RingPool Pool;
    private int tracker = 0; 
    private RingID ID;
    public int Index;
    public static int currentRing;
    private int correctRing;
    private int correctTrigger = 0;
    private List<List<GameObject>> PlaceholderIndex;

    private int currentMaxOrder = 0;
    [SerializeField] private GameObject BallMaterial;
   
  
    [SerializeField] private float fadeRingsDuration;

    [SerializeField] private GameObject ringPrefab;

    private List<GameObject> ringPool;
    private List<RingMovement> ringScripts;

    [SerializeField] private GameObject Bucket;
    private int ringAmount = 0;

    // private const int ringPoolSize = 20;
    [SerializeField] private float spawnInterval = 5f;

    private Dictionary<Transform, PlaceholderRing> placeholderReferences = new Dictionary<Transform, PlaceholderRing>();
    private GameObject currentRingSetupInstance;

    private void Awake()
    {
        Instance = this;
        ID = Pool.RingType[Index];

        correctRing = 1;
        PlaceholderIndex = new List<List<GameObject>>();

        for (int i = 0; i < Pool.RingType.Count; i++)
        {
            PlaceholderIndex.Add(new List<GameObject>());
        }

        Pool.SpawnBallPool();
        Pool.SpawnBucketPool();
        Pool.SpawnRingPool();
    }

    private void Start()
    {
        // ResetRingMaterialFade();
        Pool.ResetAllMaterials();



        
        SpawnRings(.5f);

    }

    private void AssignOrderByPosition(int correctTrig)
    {

        // Create a local list of placeholder Transforms from the dictionary
        List<Transform> sortedPlaceholders = new List<Transform>(placeholderReferences.Keys);


        // Sort the placeholders by their x position (left to right)
        sortedPlaceholders.Sort((transform1, transform2) => transform1.position.x.CompareTo(transform2.position.x));

        // Assign order based on sorted positions
        for (int i = 0; i < sortedPlaceholders.Count; i++)
        {
            // The order is assigned starting from 1, going left to right

            if (placeholderReferences[sortedPlaceholders[i]].getsTriggeredInt == correctTrig)
            {

                ID.triggeredRingOrder++;
                placeholderReferences[sortedPlaceholders[i]].order = ID.triggeredRingOrder;

            }

        }
    }

    private void PopulatePlaceholderReferences(Transform parentTransform)
    {

        placeholderReferences.Clear();
        ID.placeholderCount = 0;

        PopulateRecursive(parentTransform); 




    }

    private void PopulateRecursive(Transform currentTransform)
    {

        foreach (Transform child in currentTransform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                var placeholderScript = child.GetComponent<PlaceholderRing>();
                if (placeholderScript != null)
                {
                    placeholderReferences[child] = placeholderScript;
                    ID.placeholderCount++;
                }
            }

            // Recursively search through nested children
            if (child.childCount > 0)
            {
                PopulateRecursive(child);

            }
            PlaceholderIndex[Index].Add(child.gameObject);
        }


    }

    public void TriggeredSpawn(int triggerValue)
    {
        Debug.Log("Triggered");


        AssignOrderByPosition(triggerValue);

        foreach (var placeholder in placeholderReferences)
        {
            if (placeholder.Value.getsTriggeredInt == triggerValue)
            {
                SpawnRingAtPlaceholder(placeholder.Key, placeholder.Value);
            }
        }

        ID.ringEvent.OnCheckOrder?.Invoke();
    }

    private void SpawnRingAtPlaceholder(Transform placeholderTransform, PlaceholderRing placeholderScript)
    {

        // RingMovement ring = GetRingFromPool();
        if (placeholderScript.order != ID.placeholderCount || Pool.isTutorial)
        {

            ID.GetRing(placeholderScript.transform, placeholderScript.order, placeholderScript.speed, placeholderScript.doesTriggerInt, placeholderScript.xCordinateTrigger);

        }
        else
        {
            Debug.Log("Starting Bucket Get");
            // BucketScript bucketScript = ID.GetBucket(placeholderTransform, placeholderScript.order, placeholderScript.speed);
            ID.GetBucket(placeholderTransform, placeholderScript.order, placeholderScript.speed);

            // bucketScript.gameObject.SetActive(true);

        }



    }


    #region Losing and Fading
    public void SequenceFinished(bool correctSequence, int index)
    {
        if (!correctSequence)
        {

            switch (index)
            {
                case 0:
                    StartCoroutine(Pool.FadeOutRed());
                    break;
                case 1:
                    StartCoroutine(Pool.FadeOutPink());

                    break;
                case 2:

                    StartCoroutine(Pool.FadeOutGold());
                    break;
                default:
                    break;
            }



            SpawnRings(3.4f);
        }
        else
        {
            SpawnRings(4f);


        }
        // RemovePlaceholders();

    }


    // 

   
    #endregion

    private void SpawnRings(float time)
    {
        tracker += 1;
        Debug.Log("SpawningRings: " + tracker);
        ID.triggeredRingOrder = 0;
        ID.CorrectRing = 1;
        StartCoroutine(SpawnRingsCourintine(time));
    }
    private IEnumerator SpawnRingsCourintine(float time)
    {


        PopulatePlaceholderReferences(transform);
        ID.CorrectRing = 1;
        AudioManager.instance.ResetRingPassPitch();
        // currentRingSetupInstance = InstantiateRandomSetup();

        yield return new WaitForSeconds(time);
        TriggeredSpawn(0);
    }



    IEnumerator TimePassed()
    {
        yield return new WaitForSeconds(4f);
    }
    // public void ReturnRingToPool(GameObject ring)
    // {
    //     ring.SetActive(false);
    //     ringPool.Add(ring);
    // }

    private void RemovePlaceholders()
    {
        foreach (GameObject placeholder in PlaceholderIndex[Index])
        {
            placeholder.SetActive(false);

        }

    }

    private void OnEnable()
    {

        foreach (var ringId in Pool.RingType)
        {
            ringId.ringEvent.OnSpawnRings += SequenceFinished;
            ringId.ringEvent.OnRingTrigger += TriggeredSpawn;
            ringId.ringEvent.OnCreateNewSequence += SequenceFinished;

            // Subscribe to other events as needed
        }




    }
    private void OnDisable()
    {
        

        foreach (var ringId in Pool.RingType)
        {

            ringId.ringEvent.OnSpawnRings -= SequenceFinished;
            ringId.ringEvent.OnRingTrigger -= TriggeredSpawn;
            ringId.ringEvent.OnCreateNewSequence -= SequenceFinished;
            // Subscribe to other events as needed
        }

        // ID.ringEvent.OnPassRing -= CheckOrder;




    }

    private void CheckScore(int newScore)
    {
        if (newScore == 4 || newScore == 9 || newScore == 13)
        {
            // SpawnRandomRingSetup();

        }

    }

}
#region comments

// private void DeactivateAllRings()
// {
//     foreach (var ring in ringPool) // Assuming 'allRings' is a collection of all ring instances
//     {
//         ring.gameObject.SetActive(false);
//     }
//     ResetRingMaterialFade();
// }

//     SpriteRenderer ringRenderer = ring.GetComponent<SpriteRenderer>();
// if (ringRenderer != null)
// {
//     ringRenderer.material = ID.defaultMaterial;
// }


// public void StartBall(Vector2 startPos, float startRot)
// {

//     foreach (RingMovement ring in ringScripts)
//     {

//         if (ring != null && ring.order == correctRing)
//         {
//             BallObject.transform.position = startPos;
//             BallObject.SetActive(true);// Activate the ball
//             ballMovement.SetParameters(ring.gameObject);


//             break;


//         }

//     }
// }


// public void SetSpecialRingEffect()
// {
//     foreach (RingMovement ring in ringScripts)
//     {

//         if (ring.gameObject.activeInHierarchy && ring.order == correctRing)
//         {
//             // Apply the special shader material to the correct ring


//             ring.SetHighlightedMaterial();

//             break;
//         }
//     }
// }


// private void CheckOrder(int ringOrder)
// {

//     if (ringOrder == 0 || ringOrder == ringAmount)
//     {
//         AudioManager.instance.PlayRingPassSound(false);
//         AudioManager.instance.PlayRingSuccessSound();



//         StartCoroutine(SpawnRings(3f));

//         print("won- start");

//     }
//     else if (ringOrder == correctRing)
//     {
//         if (ringOrder != 1)
//         {

//             AudioManager.instance.PlayRingPassSound(false);
//         }
//         else
//         {

//             AudioManager.instance.PlayRingPassSound(true);
//         }
//         // print("yes");
//         correctRing++;
//         // SetSpecialRingEffect();

//         // StartBall(startPos, startRot);
//     }
//     else
//     {
//         RingLoss();
//         Debug.Log("WrongRing");
//     }

// }



// private void SpawnRandomRingSetup()
// {
//     int randomIndex = Random.Range(0, ringSetups.Count);
//     GameObject ringSetupPrefab = ringSetups[randomIndex];
//     GameObject ringSetupInstance = Instantiate(ringSetupPrefab, new Vector3(BoundariesManager.rightBoundary, 0, 0), Quaternion.identity);

//     RingSetup ringSetupScript = ringSetupInstance.GetComponent<RingSetup>();
//     bool useSetOrder = ringSetupScript != null && ringSetupScript.setOrder;

//     int orderCounter = 0;
//     ringAmount = 0;

//     List<Transform> sortedChildren = new List<Transform>();
//     foreach (Transform child in ringSetupInstance.transform)
//     {
//         sortedChildren.Add(child);
//         ringAmount += 1;
//     }

//     if (!useSetOrder)
//     {

//         sortedChildren.Sort((a, b) => a.position.x.CompareTo(b.position.x));
//     }

//     // int scriptIndex = 0;

//     foreach (Transform placeholder in sortedChildren)
//     {
//         RingMovement ring = GetRingFromPool();
//         if (ring != null)
//         {
//             ring.transform.position = placeholder.position;
//             ring.transform.rotation = placeholder.rotation;
//             ring.transform.localScale = placeholder.localScale;


//             if (useSetOrder)
//             {
//                 print("YesI Used");
//                 // If setOrder is true, use a predefined order from the placeholder
//                 PlaceholderRing placeholderRingScript = placeholder.GetComponent<PlaceholderRing>();
//                 if (placeholderRingScript != null)
//                 {
//                     ring.order = placeholderRingScript.order;
//                 }
//             }
//             else
//             {
//                 // If setOrder is false, order based on x position
//                 ring.order = ++orderCounter;
//             }

//             ring.gameObject.SetActive(true);
//             Destroy(placeholder.gameObject);
//         }
//     }

//     // SetSpecialRingEffect();
// }

// private IEnumerator FadeOutRings()
// {


//     float time = 0;


//     while (time < fadeRingsDuration)
//     {
//         time += Time.deltaTime;
//         float fadeAmount = Mathf.Lerp(0, 1, time / fadeRingsDuration);
//         ID.defaultMaterial.SetFloat("_FadeAmount", fadeAmount);
//         ID.highlightedMaterial.SetFloat("_FadeAmount", fadeAmount);
//         ID.passedMaterial.SetFloat("_FadeAmount", fadeAmount);

//         yield return null;
//     }
//     Pool.DisableRings(Index);

//     ResetRingMaterialFade();


// }

// private void ResetRingMaterialFade()
    // {


//     // Reset fade effect
//     ID.defaultMaterial.SetFloat("_FadeAmount", 0);
//     ID.highlightedMaterial.SetFloat("_FadeAmount", 0);
//     ID.passedMaterial.SetFloat("_FadeAmount", 0);
// }


#endregion


