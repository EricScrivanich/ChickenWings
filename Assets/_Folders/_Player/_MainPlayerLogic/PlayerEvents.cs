
using System;
using UnityEngine;

public struct PlayerEvents
{
    public Action OnJump;

    public Action<bool> OnWater;

    public Action<bool> OnFlipRight;
    public Action<bool> OnFlipLeft;
    public Action<bool> OnDash;
    public Action OnDashSlash;
    public Action OnDrop;
    public Action OnBounce;
    public Action<bool> OnJumpHeld;
    public Action OnJumpReleased;
    public Action<bool> OnParachute;
    public Action<bool> OnPressAmmo;

    // public Action<bool> FloorCollsion;
    public Action OnEggDrop;

    public Action OnSuccesfulParry;
    public Action<bool> OnPerformParry;

    public Action<bool> OnAttack;
    public Action<Transform> OnCompletedRingSequence;

    public Action HitBoss;

    public Action LoseLife;
    public Action HitGround;

    public Action<bool> EnableButtons;
    public Action<bool> SpecialEnableButtons;

    public Action<int> OnStopJumpAir;
    public Action<int> OnSwitchAmmoType;

    public Action<Vector2> OnAimJoystick;

    public Action OnCollectCage;
    public Action<Vector2, float, float> OnSwipeButton;

    public Action<Vector2> SendParrySwipeData;
    public Action<Vector2, float> OnShowCursor;
    public Action ReleaseSwipe;

    public Action<byte> OnScytheAttack;

    public Action<float> OnSwipeScytheAttack;

    public Action<Vector2> OnStuckScytheSwipe;

    public Action<Vector2> OnTouchCenter;
    public Action OnReleaseCenter;

    public Action<Vector2, bool> OnDragCenter;
    public Action TwoFingerCenterTouch;

    public Action<bool, Vector2> OnPointerCenter;
    public Action QuickCenterRelease;
    public Action<Vector2> OnSetScythePos;
    public Action OnReleaseStick;
    public Action OnCollision;
    public Action OnStartPlayer;


}

public struct UiEvents
{
    public Action<bool, int, float> OnShowPlayerUI;
    public Action<bool> OnShowSyctheLine;
    public Action<bool> OnAllowSwipe;
    public Action<float> OnPressWeaponButton;
    public Action<int> OnCollectAmmo;
    public Action<int[], int> OnSendShownSidebarAmmos;
    public Action<int> OnResetSidebarAmmos;
    public Action<bool> OnClickAmmoSwitch;
    public Action<int, int> OnSwitchWeapon;
    public Action<int, int, int> OnSwitchDisplayedWeapon;
    public Action<bool> ReleaseScope;
    public Action<int> OnUseAmmo;
    public Action<bool> OnUseJoystick;
    public Action<bool> OnSetAmmoZero;
    public Action<bool> OnUseChainedAmmo;

    public Action<int> OnPressAmmoSideButton;
    public Action<bool> OnShowCage;

    public Action<bool> StuckPigScythe;

    public Action<float> UseScythePower;
    public Action<float> ChangeScythePowerPitch;
    public Action<bool, float> ShowScythePower;
    public Action<int> OnSetStartingLives;

    public Action<bool, bool> OnHandleFlip;
    public Action<bool> OnDashUI;

    public Action<bool, bool> OnFinishDashAndDropCooldown;
    public Action OnDropUI;
}
