using System;

public struct OutputLvlEvent 
{
    public Action<int> RingParentPass;

    public Action FinishedLevel;

    public Action<int> BoostLevelIntensity;

    public Action triggerFinshed;
    public Action<bool,int> ringSequenceFinished;
    public Action addBucketPass;
    public Action<int> setBucketPass;
}
