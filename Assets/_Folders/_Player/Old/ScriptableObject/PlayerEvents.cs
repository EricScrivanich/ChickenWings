
using System;

public struct PlayerEvents
{
    public Action OnJump;
    public Action OnFlipRight;
    public Action OnFlipLeft;
    public Action OnDash;
    public Action OnDrop;
    public Action OnBounce;
    public Action OnJumpHeld;
    public Action OnJumpReleased;
    public Action<bool> FloorCollsion;
    public Action OnEggDrop;
    public Action OnAttack;
}
