using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;


public class RingSpawner : MonoBehaviour
{

    [ExposedScriptableObject]
    public RingPool Pool;
    public PlaneManagerID PlaneID;
    private bool hasSpawnedLives;

    private bool SpawningRandomSetup;

    [SerializeField] private bool bossExample;

    [SerializeField] private PlaneData crop;
    [SerializeField] private PlaneData jet;
    [SerializeField] private PlaneData cargo;

    [SerializeField] private float totalSpawnDelay;
    [SerializeField] private float totalIterationSpawnDelay;

    [SerializeField] private PlayerID player;

    public PlaceholderDataCollection placeholderDataCollection;
    [SerializeField] private PlaceholderDataCollection lifeSupport;

    public SetupHolder setupHolder;
    private int tracker = 0;

    private RingID ringType;
    public int Index;
    public static int currentRing;
    private int correctRing;
    private int correctTrigger = 0;
    private List<List<GameObject>> PlaceholderIndex;


    // red = 0
    // pink = 1
    // gold = 2
    // purple = 3


    private void Awake()
    {
        hasSpawnedLives = false;
        SpawningRandomSetup = false;


        ringType = Pool.RingType[Index];

        correctRing = 1;
        PlaceholderIndex = new List<List<GameObject>>();

        foreach (var ringId in Pool.RingType)
        {

            ringId.InitializeEffectsPool();
        }
        for (int i = 0; i < Pool.RingType.Count; i++)
        {
            PlaceholderIndex.Add(new List<GameObject>());
        }

        Pool.SpawnBallPool();
        Pool.SpawnBucketPool();
        Pool.SpawnRingPool();
        ringType.newSetup = true;
    }

    private void Start()
    {
        crop.SpawnPlanePool();
        jet.SpawnPlanePool();
        cargo.SpawnPlanePool();
        crop.ResetValues();
        jet.ResetValues();
        cargo.ResetValues();
        PlaneID.SpawnExplosionQueue();

        if (bossExample)
        {
            StartCoroutine(BossSpawner());
            PlaneID.bomberTime = 30;
            PlaneID.numberOfSpawnsToDeactivate = 3;
            PlaneID.SpawnExplosionQueue();
            PlaneID.spawnRandomPlanesBool = true;

            return;

        }


        PlaneID.bomberTime = 27;
        PlaneID.numberOfSpawnsToDeactivate = 3;



        if (Pool.Testing)
        {
            StartCoroutine(SpawnSpecificSetup());
        }
        else
        {
            StartCoroutine(MainSpawner());

        }



        // ResetRingMaterialFade();
        Pool.ResetAllMaterials();
        foreach (var ringId in Pool.RingType)
        {
            ringId.ResetVariables();

        }




        PlaneID.spawnRandomPlanesBool = true;

    }


    public void NewTriggeredSpawn(int triggerValue)
    {


        if (triggerValue == -1)
        {
            if (Pool.Testing)
            {
                StartCoroutine(SpawnSpecificSetup());

            }
            else
            {
                SpawningRandomSetup = false;
                PlaneID.spawnRandomPlanesBool = true;

            }

            return;

        }

        if (placeholderDataCollection != null)
        {
            if (placeholderDataCollection.TestSpecifiedTrigger)
            {
                triggerValue = placeholderDataCollection.SpecifiedTrigger;


            }
            int totalRingPlaceholders = placeholderDataCollection.triggerGroups.Sum(g => g.placeholderRingDataList.Count);
            int totalPlanePlaceholders = placeholderDataCollection.triggerGroups.Sum(g => g.placeholderPlaneDataList.Count);

            int planesSpawned = 0;
            SpawningRandomSetup = true;



            // Find the PlaceholderTriggerGroup for the given trigger value
            PlaceholderTriggerGroup triggerGroup = placeholderDataCollection.triggerGroups.FirstOrDefault(g => g.triggerValue == triggerValue);

            if (triggerGroup == null)
            {
                Debug.LogError($"No placeholders found for trigger {triggerValue}");
                return;
            }
            Debug.Log(tracker + ": Triggered: " + triggerGroup);


            // Iterate through the placeholders for the specific trigger, already sorted by their x position
            foreach (var placeholderData in triggerGroup.placeholderRingDataList)
            {
                ringType.triggeredRingOrder++; // Increment the order for each spawned ring

                if (ringType.triggeredRingOrder == totalRingPlaceholders)
                {
                    ringType.GetBucket(placeholderData.position, placeholderData.rotation, placeholderData.scale, ringType.triggeredRingOrder, placeholderData.speed);
                }
                else
                {
                    ringType.GetRing(placeholderData.position, placeholderData.rotation, placeholderData.scale, ringType.triggeredRingOrder, placeholderData.speed, placeholderData.doesTriggerInt, placeholderData.xCordinateTrigger);
                    // If you have special logic for the last ring or a "BucketScript", you can include that here
                }

                // Spawn the ring using the placeholder's transform and other data

                // For example, if placeholderData represents the last item in the list, you might call ID.GetBucket or similar
            }

            foreach (var placeholderData in triggerGroup.placeholderPlaneDataList)
            {
                planesSpawned++;
                if (planesSpawned == totalPlanePlaceholders && placeholderDataCollection.TestSpecifiedTrigger)
                {
                    placeholderData.planeType.GetPlane(-1, 0, placeholderData.speed, placeholderData.position);

                }
                else
                {
                    placeholderData.planeType.GetPlane(placeholderData.doesTiggerInt, placeholderData.xCordinateTrigger, placeholderData.speed, placeholderData.position);

                }
            }

            foreach (var placeholder in triggerGroup.planeAreaSpawnDataList)
            {
                PlaneID.SpawnInArea(placeholder.minPlanes, placeholder.maxPlanes, placeholder.minX, placeholder.maxX, placeholder.minY, placeholder.maxY, placeholder.cropChance, placeholder.jetChance, placeholder.cargoChance);
            }
        }




        // Consider whether you need to reset ID.triggeredRingOrder here, depending on whether it should continue across different triggers
    }






    #region Losing and Fading
    public void SequenceFinished(bool correctSequence, int index)
    {

        if (!correctSequence)
        {

            StartCoroutine(SequenceFinishedCourintine(0, index));

            // SpawnRings(3.4f);
        }
        else
        {
            StartCoroutine(SequenceFinishedCourintine(1.5f, index));

            // Pool.RingType[index].ResetVariables();
            // SpawnRings(4f);


        }

    }

    private IEnumerator SequenceFinishedCourintine(float time, int index)
    {
        yield return new WaitForSeconds(time);

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
            case 3:

                StartCoroutine(Pool.FadeOutPurple());
                break;
            default:
                break;
        }


    }

    private void SpawnRandomSetup(int ringIndex, float time)
    {
        placeholderDataCollection = setupHolder.GetRandomSetup();
        PlaneID.spawnRandomPlanesBool = false;

        if (ringIndex >= 0)
        {
            ringType = Pool.RingType[ringIndex];
        }

        StartCoroutine(SpawnSetupCourintine(time));




    }

    private IEnumerator SpawnSpecificSetup()
    {

        yield return new WaitUntil(() => ringType.ReadyToSpawn == true);
        yield return new WaitUntil(() => SpawningRandomSetup == false);

        if (placeholderDataCollection.TestSpecifiedTrigger)
        {
            NewTriggeredSpawn(placeholderDataCollection.SpecifiedTrigger);
        }
        else
        {
            NewTriggeredSpawn(0);
        }

    }



    // 


    #endregion

    private void SpawnRings(float time)
    {

        AudioManager.instance.ResetRingPassPitch();

        tracker++;

        // Index++;


        StartCoroutine(SpawnRingsCourintine(.5f));
    }

    private IEnumerator SpawnSetupCourintine(float time)
    {
        yield return new WaitForSeconds(time);
        yield return new WaitUntil(() => ringType.ReadyToSpawn == true);
        NewTriggeredSpawn(0);


    }
    private IEnumerator SpawnRingsCourintine(float time)
    {

        if (!Pool.Testing)
        {
            PlaneID.spawnRandomPlanesBool = true;
            yield return new WaitForSeconds(time);
        }
        PlaneID.spawnRandomPlanesBool = false;
        ringType = Pool.RingType[Index];

        // currentRingSetupInstance = InstantiateRandomSetup();

        yield return new WaitForSeconds(2.5f);
        yield return new WaitUntil(() => ringType.ReadyToSpawn == true);
        Debug.Log("spawning Rings: " + tracker);

        NewTriggeredSpawn(0);
    }

    #region SpawnerMain
    private IEnumerator MainSpawner()
    {
        yield return new WaitForSeconds(6f);
        StartCoroutine(SpawnPlaneType(crop));
        yield return new WaitForSeconds(5f);
        crop.PlanesAvailable = 2;
        yield return new WaitForSeconds(6f);
        StartCoroutine(SpawnPlaneType(jet));
        yield return new WaitForSeconds(7f);
        SpawnRandomSetup(2, 2.5f);

        yield return new WaitUntil(() => PlaneID.spawnRandomPlanesBool == true);
        PlaneID.bomberTime = 22;

        yield return new WaitForSeconds(4f);
        PlaneID.numberOfSpawnsToDeactivate = 2;


        crop.PlanesAvailable = 3;
        yield return new WaitForSeconds(4f);
        jet.PlanesAvailable = 2;
        yield return new WaitForSeconds(5f);

        Debug.Log("Cargo");
        StartCoroutine(SpawnPlaneType(cargo));
        crop.minIterationDelay = 2;
        yield return new WaitForSeconds(8f);
        jet.minOffsetTime = 0;
        jet.maxOffsetTime = 2;
        jet.maxIterationDelay = 2;
        cargo.PlanesAvailable = 2;
        yield return new WaitForSeconds(6f);
        SpawnRandomSetup(0, 2.5f);

        yield return new WaitUntil(() => PlaneID.spawnRandomPlanesBool == true);
        PlaneID.bomberTime = 17;
        crop.PlanesAvailable = 4;

        crop.maxIterationDelay = 4;
        yield return new WaitForSeconds(3f);
        jet.PlanesAvailable = 3;
        crop.PlanesAvailable = 4;

        yield return new WaitForSeconds(4f);
        cargo.PlanesAvailable = 3;
        cargo.maxOffsetTime = 3;
        yield return new WaitForSeconds(6f);
        jet.PlanesAvailable = 4;
        yield return new WaitForSeconds(8f);
        crop.PlanesAvailable = 6;
        jet.minIterationDelay = 0;
        jet.maxIterationDelay = 1;
        PlaneID.bomberTime = 13;

        yield return new WaitForSeconds(6f);
        cargo.PlanesAvailable = 4;
        PlaneID.numberOfSpawnsToDeactivate = 1;

        cargo.minOffsetTime = 0;
        cargo.minOffsetTime = 1;










    }

    private IEnumerator SpawnLives()
    {
        float randomTime = Random.Range(4, 9);
        yield return new WaitUntil(() => SpawningRandomSetup == false);
        SpawningRandomSetup = true;
        yield return new WaitForSeconds(randomTime);
        PlaneID.spawnRandomPlanesBool = false;
        yield return new WaitForSeconds(2);

        placeholderDataCollection = lifeSupport;
        ringType = Pool.RingType[1];
        NewTriggeredSpawn(0);


    }

    private IEnumerator BossSpawner()
    {
        yield return new WaitForSeconds(6f);

        crop.PlanesAvailable = 1;
        StartCoroutine(SpawnPlaneType(crop));
        yield return new WaitForSeconds(9f);
        jet.PlanesAvailable = 1;
        StartCoroutine(SpawnPlaneType(jet));
        yield return new WaitForSeconds(3f);

        yield return new WaitForSeconds(10f);
        crop.PlanesAvailable = 2;








    }

    #endregion

    #region Plane


    IEnumerator SpawnPlaneType(PlaneData plane)
    {
        while (true) // Main game loop
        {
            for (int i = 0; i < plane.PlanesAvailable; i++)
            {
                Debug.Log(plane.PlanesAvailable);
                float SpawnOffsetVar = Random.Range(plane.minOffsetTime, plane.maxOffsetTime);
                yield return new WaitUntil(() => PlaneID.spawnRandomPlanesBool == true);
                yield return new WaitForSeconds(SpawnOffsetVar + totalSpawnDelay);
                yield return new WaitUntil(() => PlaneID.spawnRandomPlanesBool == true);
                float spawnPositionY = SpawnPointManager.GetRandomSpawnPointY(sp => sp.canSpawnCrop);
                Vector2 position = new Vector2(BoundariesManager.rightBoundary, spawnPositionY);
                plane.GetPlane(0, 0, 0, position);
                // Spawn a plane
                // Wait for offset
            }
            float iterationEndDelay = Random.Range(plane.minIterationDelay, plane.maxIterationDelay);
            yield return new WaitUntil(() => PlaneID.spawnRandomPlanesBool == true);
            yield return new WaitForSeconds(iterationEndDelay + totalIterationSpawnDelay); // End-of-iteration delay
                                                                                           // Optional: Adjust cropPlanesAvailable and delays based on game conditions
        }
    }




    #endregion





    private void OnEnable()
    {

        foreach (var ringId in Pool.RingType)
        {
            ringId.ringEvent.OnSpawnRings += SequenceFinished;
            ringId.ringEvent.OnRingTrigger += NewTriggeredSpawn;
            ringId.ringEvent.OnCreateNewSequence += SequenceFinished;

        }

        PlaneID.events.TriggeredSpawn += NewTriggeredSpawn;
        player.globalEvents.OnUpdateLives += CheckLives;

    }
    private void OnDisable()
    {
        foreach (var ringId in Pool.RingType)
        {

            ringId.ringEvent.OnSpawnRings -= SequenceFinished;
            ringId.ringEvent.OnRingTrigger -= NewTriggeredSpawn;
            ringId.ringEvent.OnCreateNewSequence -= SequenceFinished;
            // Subscribe to other events as needed
        }

        PlaneID.events.TriggeredSpawn -= NewTriggeredSpawn;
        player.globalEvents.OnUpdateLives -= CheckLives;


    }

    private void CheckLives(int lives)
    {
        if (lives == 1 && !hasSpawnedLives)
        {
            StartCoroutine(SpawnLives());
            hasSpawnedLives = true;

        }

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


// private void AssignOrderByPosition(int correctTrig)
// {

//     // Create a local list of placeholder Transforms from the dictionary
//     List<Transform> sortedPlaceholders = new List<Transform>(placeholderReferences.Keys);


//     // Sort the placeholders by their x position (left to right)
//     sortedPlaceholders.Sort((transform1, transform2) => transform1.position.x.CompareTo(transform2.position.x));

//     // Assign order based on sorted positions
//     for (int i = 0; i < sortedPlaceholders.Count; i++)
//     {
//         // The order is assigned starting from 1, going left to right

//         if (placeholderReferences[sortedPlaceholders[i]].getsTriggeredInt == correctTrig)
//         {

//             ID.triggeredRingOrder++;
//             placeholderReferences[sortedPlaceholders[i]].order = ID.triggeredRingOrder;

//         }

//     }
// }

// private void PopulatePlaceholderReferences(Transform parentTransform)
// {

//     placeholderReferences.Clear();
//     ID.placeholderCount = 0;

//     PopulateRecursive(parentTransform);




// }

// private void PopulateRecursive(Transform currentTransform)
// {

//     foreach (Transform child in currentTransform)
//     {
//         if (child.gameObject.activeInHierarchy)
//         {
//             var placeholderScript = child.GetComponent<PlaceholderRing>();
//             if (placeholderScript != null)
//             {
//                 placeholderReferences[child] = placeholderScript;
//                 ID.placeholderCount++;
//             }
//         }

//         // Recursively search through nested children
//         if (child.childCount > 0)
//         {
//             PopulateRecursive(child);

//         }
//         PlaceholderIndex[Index].Add(child.gameObject);
//     }


// }

// public void TriggeredSpawn(int triggerValue)
// {


//     // Assign order by position to each placeholder
//     AssignOrderByPosition(triggerValue);

//     // Create a list of placeholders that match the trigger value, sorted by their assigned order
//     var orderedPlaceholders = placeholderReferences
//         .Where(placeholder => placeholder.Value.getsTriggeredInt == triggerValue)
//         .OrderBy(placeholder => placeholder.Value.order)
//         .ToList();

//     // Iterate through the sorted placeholders and spawn rings accordingly
//     foreach (var placeholder in orderedPlaceholders)
//     {
//         SpawnRingAtPlaceholder(placeholder.Key, placeholder.Value);
//     }

//     // Add particle object after all rings have been spawned
//     // ID.AddParticleObject();
// }

// private void SpawnRingAtPlaceholder(Transform placeholderTransform, PlaceholderRing placeholderScript)
// {

//     // RingMovement ring = GetRingFromPool();
//     if (placeholderScript.order != ID.placeholderCount || Pool.isTutorial)
//     {

//         ID.GetRing(placeholderScript.transform.position, placeholderScript.transform.rotation, placeholderScript.transform.localScale, placeholderScript.order, placeholderScript.speed, placeholderScript.doesTriggerInt, placeholderScript.xCordinateTrigger);

//     }
//     else
//     {

//         // BucketScript bucketScript = ID.GetBucket(placeholderTransform, placeholderScript.order, placeholderScript.speed);
//         ID.GetBucket(placeholderScript.transform.position, placeholderScript.transform.rotation, placeholderScript.transform.localScale, placeholderScript.order, placeholderScript.speed);

//         // bucketScript.gameObject.SetActive(true);

//     }



// }

#endregion


