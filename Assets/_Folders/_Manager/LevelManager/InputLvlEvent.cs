using System;
using UnityEngine;

public struct InputLvlEvent
{
    public Action<int> RingParentPass;
    public Action<int,float> StartSpawnerInput;
    public Action<int> SetCheckPoint;
    public Action<TriggerNextSection, bool, GameObject> ActivateObjFromEvent;
    public Action<string, int> OnUpdateObjective;
    public Action finishedOverrideStateLogic;
    public Action<SpawnStateTransitionLogic> switchTransitionLogic;

    public Action OnEggFinishLine;
}
