
using System;
using UnityEngine;

public struct PlayerEvents
{
    public Action OnJump;
    public Action OnFlipRight;
    public Action OnFlipLeft;
    public Action<bool> OnDash; 
    public Action OnDashSlash;
    public Action OnDrop;
    public Action OnBounce;
    public Action<bool> OnJumpHeld;  
    public Action OnJumpReleased;
    public Action<bool> OnParachute;
    // public Action<bool> FloorCollsion;
    public Action OnEggDrop;
    public Action<bool> OnAttack;
    public Action<Transform> OnCompletedRingSequence;

    public Action HitBoss;

    public Action LoseLife;
    public Action HitGround;

    public Action<bool> OnHoldFlip;
}
