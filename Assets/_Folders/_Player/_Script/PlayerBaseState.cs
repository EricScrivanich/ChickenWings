
using UnityEngine;

public abstract class PlayerBaseState 
{
    public abstract void EnterState(PlayerStateManager player);
    public abstract void ExitState(PlayerStateManager player);
    public abstract void UpdateState(PlayerStateManager player);
    public abstract void FixedUpdateState(PlayerStateManager player);
    public abstract void RotateState(PlayerStateManager player);
    public abstract void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision);
}
