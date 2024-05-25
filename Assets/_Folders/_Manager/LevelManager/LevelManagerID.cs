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
    public int currentRingsPassed;

    public int RingsPassed { get; private set; }

private void AddRingPass()
{

}

public void ResetLevel(bool ringsReq,int ringAmountNeeded)
{
        areRingsRequired = ringsReq;
        ringsNeeded = ringAmountNeeded;
        currentRingsPassed = 0;


    }


}