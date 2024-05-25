using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelManagerID LvlID;
    [Header("RingPass")]
    [SerializeField] private bool areRingsRequired;
    private bool finishedRings;
    [SerializeField] private int ringsNeeded;
    [SerializeField] private int currentRingsPassed;



    // Start is called before the first frame update
    private void Awake() 
    {
        LvlID.ResetLevel(areRingsRequired, ringsNeeded);
        finishedRings = !areRingsRequired;

        if (areRingsRequired)
        {
            LvlID.inputEvent.RingParentPass += PassRing;

        }

    }
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PassRing(int inARow)
    {
        currentRingsPassed++;
        LvlID.outputEvent.RingParentPass(currentRingsPassed);

        if (currentRingsPassed == LvlID.ringsNeeded)
        {
            finishedRings = true;
            CheckGoals();
        }

    }

    private void OnDestroy() {
        LvlID.inputEvent.RingParentPass -= PassRing;
    }



    private void CheckGoals()
    {
        if (finishedRings == true)
        {
            LvlID.outputEvent.FinishedLevel?.Invoke();
        }
    }
}
