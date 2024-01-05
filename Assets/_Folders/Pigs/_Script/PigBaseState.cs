using UnityEngine;

public abstract class PigBaseState 
{
    public abstract void EnterState(PigStateManager pig);
    public abstract void UpdateState(PigStateManager pig);
    public abstract void FixedUpdateState(PigStateManager pig);
    
}
