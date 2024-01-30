using System;

public struct RingEvents
{
    public float ringSuccessSpawnDelay;
    public float ringFailureSpawnDelay;
    public Action<int> OnPassRing;
    public Action OnRingLoss;
    public Action<int> OnRingTrigger;

    public Action OnCheckOrder;


    public Action<bool> OnSpawnRings;

    public Action<bool> OnCreateNewSequence;


}
