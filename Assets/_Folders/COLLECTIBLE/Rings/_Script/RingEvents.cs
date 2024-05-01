using System;
using UnityEngine;

public struct RingEvents
{
    public float ringSuccessSpawnDelay;
    public float ringFailureSpawnDelay;
    public Action<int> OnPassRing;
    public Action<int> OnRingTrigger;
    public Action OnCheckOrder;
    public Action OnPassedCorrectRing;
    public Action ResetQuueue;
    
    public Action<Vector2> OnGetBall;
    public Action OnBallFinished;
    public Action<bool, int> OnSpawnRings;
    public Action<bool,int> OnCreateNewSequence;

    public Action tutorialRingPass;


}
