using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public abstract class AmmoBaseState
{

   
    public abstract void EnterState(PlayerStateManager player, int direction);

    public abstract void ExitState(PlayerStateManager player);
    public abstract void PressButton(PlayerStateManager player, Vector2 startPos);
    public abstract void ReleaseButton(PlayerStateManager player);

    public abstract void CollectAmmo(PlayerStateManager player, int type);
    public abstract void SwipeButton(PlayerStateManager player, Vector2 currentPos, bool isJoystick = false);



    



}
