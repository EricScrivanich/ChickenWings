using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelManagerID : ScriptableObject
{
    public InputLvlEvent inputEvent;
    public OutputLvlEvent outputEvent;

    [Header("RingPass")]
    public bool areRingsRequired;
    public int ringsNeeded;

    public int barnsNeeded;
    public int currentRingsPassed;

    public int RingsPassed { get; private set; }

    public int bucketsNeeded;



private void AddRingPass()
{

}

public void ResetLevel(bool ringsReq,int ringAmountNeeded, int barnsNeed, int bucketNeed)
{
        areRingsRequired = ringsReq;
        ringsNeeded = ringAmountNeeded;
        currentRingsPassed = 0;
        barnsNeeded = barnsNeed;
        bucketsNeeded = bucketNeed;


    }


}
