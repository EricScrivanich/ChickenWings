using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialRingSpawner : MonoBehaviour
{
    public PlaceholderDataCollection placeholderDataCollection;
    public RingID ringID;
    public RingPool Pool;
    private int correctRing;
    // Start is called before the first frame update

    private void Awake()
    {
        correctRing = 1;
        Pool.SpawnBallPool();
        Pool.SpawnBucketPool();
        Pool.SpawnRingPool();

    }

    private void Start()
    {
        ringID.InitializeEffectsPool();

        Pool.ResetAllMaterials();
        ringID.ResetVariables();
    }


    public void NewTriggeredSpawn(int triggerValue)
    {
        // Find the PlaceholderTriggerGroup for the given trigger value
        PlaceholderTriggerGroup triggerGroup = placeholderDataCollection.triggerGroups.FirstOrDefault(g => g.triggerValue == triggerValue);

        if (triggerGroup == null)
        {

            return;
        }

        int totalRingPlaceholders = placeholderDataCollection.triggerGroups.Sum(g => g.placeholderRingDataList.Count);

        // Iterate through the placeholders for the specific trigger, already sorted by their x position
        foreach (var placeholderData in triggerGroup.placeholderRingDataList)
        {
            ringID.triggeredRingOrder++; // Increment the order for each spawned ring

            if (ringID.triggeredRingOrder == totalRingPlaceholders)
            {
                ringID.GetBucket(placeholderData.position, placeholderData.rotation, placeholderData.scale, placeholderData.speed);
            }
            else
            {
                ringID.GetRing(placeholderData.position, placeholderData.rotation, placeholderData.scale,placeholderData.speed);
                // If you have special logic for the last ring or a "BucketScript", you can include that here
            }

            // Spawn the ring using the placeholder's transform and other data

            // For example, if placeholderData represents the last item in the list, you might call ID.GetBucket or similar
        }

        // Consider whether you need to reset ID.triggeredRingOrder here, depending on whether it should continue across different triggers
    }

    public void SequenceFinished(bool correctSequence, int index)
    {

        if (!correctSequence)
        {

            StartCoroutine(SequenceFinishedCourintine(0, index));

            // SpawnRings(3.4f);
        }
        else
        {
            Debug.Log("yeee");

            // Pool.RingType[index].ResetVariables();
            // SpawnRings(4f);


        }

    }


    public void StartFinalTest()
    {
        StartCoroutine(SpawnRingsCourintine(2.5f));

    }

    private IEnumerator SequenceFinishedCourintine(float time, int index)
    {
        yield return new WaitForSeconds(time);

        switch (index)
        {

            case 2:

                StartCoroutine(Pool.FadeOutGold());
                StartCoroutine(SpawnRingsCourintine(2.5f));
                break;

            default:
                break;
        }


    }


    private IEnumerator SpawnRingsCourintine(float time)
    {

        yield return new WaitForSeconds(2.5f);
        yield return new WaitUntil(() => ringID.ReadyToSpawn == true);


        NewTriggeredSpawn(0);
    }


    private void OnEnable()
    {
        ringID.ringEvent.OnSpawnRings += SequenceFinished;
        ringID.ringEvent.OnRingTrigger += NewTriggeredSpawn;
        ringID.ringEvent.OnCreateNewSequence += SequenceFinished;
    }

    private void OnDisable()
    {
        ringID.ringEvent.OnSpawnRings -= SequenceFinished;
        ringID.ringEvent.OnRingTrigger -= NewTriggeredSpawn;
        ringID.ringEvent.OnCreateNewSequence -= SequenceFinished;

    }

}
