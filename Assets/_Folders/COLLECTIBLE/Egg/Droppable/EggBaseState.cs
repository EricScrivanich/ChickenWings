
using UnityEngine;

public abstract class EggBaseState
{
    public abstract void EnterState(EggStateManager egg);
    public abstract void UpdateState(EggStateManager egg);
    public abstract void OnTriggerEnter2D(EggStateManager egg, Collider2D collider); 
}
