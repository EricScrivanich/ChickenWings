using System;

public struct OutputLvlEvent 
{
    public Action<int> RingParentPass;

    public Action FinishedLevel;

    public Action<int> BoostLevelIntensity;
}
