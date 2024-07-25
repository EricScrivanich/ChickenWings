using System;
using UnityEngine;

public struct RingEvents
{
   
    public Action<int> OnRingTrigger;
    public Action OnCheckOrder;
    public Action<Vector2> OnGetBall;
    public Action OnBallFinished;
    public Action<bool, int> OnSpawnRings;
    public Action<bool,int> OnCreateNewSequence;
    public Action tutorialRingPass;
    public Action<int> DisableRings;


}
