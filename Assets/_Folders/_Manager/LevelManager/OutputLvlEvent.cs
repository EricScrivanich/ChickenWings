using System;
using UnityEngine;

public struct OutputLvlEvent
{
    public Action<int> RingParentPass;

    public Action StartCustomBoundingBoxWithDelay;

    public Action<CollectableSpawnData> SetNewCollectableSpawnData;

    public Action<int> SetCheckPoint;

    public Action<bool> setButtonsReadyToPress;

    public Action<int> StartSpawner;

    public Action<bool, int, string> SetPressButtonText;

    public Action<GameObject, float> SetObjectActiveWithDelay;

    public Action<float> SpecialRingFadeIn;
    public Action<int, Vector2, float> GetSpecialRing;

    public Action FinishedLevel;

    public Action<int, bool> ShowSection;

    public Action<int> BoostLevelIntensity;

    public Action triggerFinshed;
    public Action<bool, int> ringSequenceFinished;
    public Action addBucketPass;
    public Action<int> setBucketPass;

    public Action<int> nextSection;

    public Action<int, int> killedPig;

  

  

    public Action<float> SetLevelProgress;






    public Action<RandomSpawnIntensity, bool> OnSetNewIntensity;
    public Action<SpawnStateTransitionLogic, bool> OnSetNewTransitionLogic;
}
