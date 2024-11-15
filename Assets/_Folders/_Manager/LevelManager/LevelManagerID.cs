using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelManagerID : ScriptableObject
{
    public InputLvlEvent inputEvent;
    public OutputLvlEvent outputEvent;

    public bool LevelCompleted { get; private set; }

    [SerializeField] private GameObject blobBurstPrefab;
    private GameObject blobBurst;

    private bool hasCreatedBlobBurst = false;

    public bool usingCheckPoint;

    [Header("RingPass")]
    public bool areRingsRequired;
    public bool PauseSpawning;
    public int ringsNeeded;

    public string LevelTitle;




    public int LevelIntensity { get; private set; }
    public int barnsNeeded;
    public int currentRingsPassed;

    public int RingsPassed { get; private set; }

    public int bucketsNeeded;

    [SerializeField] private PlayerID player;



    public int ReturnPlayerLives()
    {
        return player.Lives;
    }

    public List<int> ReturnPlayerInputs()
    {
        return player.PlayerInputs;
    }
    public List<Vector3Int> ReturnKilledPigs()
    {
        return player.KilledPigs;
    }

    public Vector2Int ReturnAmmoAmounts()
    {
        Vector2Int a = new Vector2Int(player.Ammo, player.ShotgunAmmo);
        return a;
    }




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
        LevelCompleted = false;
        areRingsRequired = ringsReq;
        PauseSpawning = false;
        ringsNeeded = ringAmountNeeded;
        LevelIntensity = 0;
        currentRingsPassed = 0;
        barnsNeeded = barnsNeed;
        bucketsNeeded = bucketNeed;



    }

    public void SetLevelComplete()
    {
        Debug.LogError("Setting Level Complete");
        LevelCompleted = true;
    }

    public void SetNewIntensity(int newVal)
    {
        LevelIntensity = newVal;

    }


}
