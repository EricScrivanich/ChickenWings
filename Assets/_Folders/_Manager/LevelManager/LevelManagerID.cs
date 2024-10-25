using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelManagerID : ScriptableObject
{
    public InputLvlEvent inputEvent;
    public OutputLvlEvent outputEvent;

    [SerializeField] private GameObject blobBurstPrefab;
    private GameObject blobBurst;

    private bool hasCreatedBlobBurst = false;

    public bool usingCheckPoint;

    [Header("RingPass")]
    public bool areRingsRequired;
    public bool PauseSpawning;
    public int ringsNeeded;


    public int LevelIntensity { get; private set; }
    public int barnsNeeded;
    public int currentRingsPassed;

    public int RingsPassed { get; private set; }

    public int bucketsNeeded;



    private void AddRingPass()
    {

    }

    public void CreateBlobBurst()
    {
        if (!hasCreatedBlobBurst)
        {
            hasCreatedBlobBurst = true;
            blobBurst = Instantiate(blobBurstPrefab);
            blobBurst.SetActive(false);

        }
    }

    public void GetBlobBurst(Vector2 pos, float zRot)
    {
        Debug.LogError("Getting blob butst");
        if (blobBurst.activeInHierarchy) blobBurst.SetActive(false);
        blobBurst.transform.position = pos;
        blobBurst.transform.eulerAngles = new Vector3(0, 0, zRot);
        blobBurst.SetActive(true);

    }



    public void ResetLevel(bool ringsReq, int ringAmountNeeded, int barnsNeed, int bucketNeed)
    {
        hasCreatedBlobBurst = false;
        areRingsRequired = ringsReq;
        PauseSpawning = false;
        ringsNeeded = ringAmountNeeded;
        LevelIntensity = 0;
        currentRingsPassed = 0;
        barnsNeeded = barnsNeed;
        bucketsNeeded = bucketNeed;


    }

    public void SetNewIntensity(int newVal)
    {
        LevelIntensity = newVal;

    }


}
