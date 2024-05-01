using UnityEngine;

public abstract class HeliBaseState
{
    public abstract void EnterState(HeliStateManager heli);
    public abstract void ExitState(HeliStateManager heli);
    public abstract void UpdateState(HeliStateManager heli);
    public abstract void OnTriggerEnter2D(HeliStateManager heli, Collider2D collider); 
}