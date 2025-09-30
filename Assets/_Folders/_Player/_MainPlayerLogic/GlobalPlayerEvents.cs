
using System;
using UnityEngine;

public struct GlobalPlayerEvents
{
    public Action<int> OnUpdateLives;

    public Action<int> OnUpdateChainedShotgunAmmo;
    public Action<float> OnUpdateChainedShotgunMaxWait;
    public Action<float> OnUpdateScytheAmmo;
    public Action<float> OnUpdateBoomerangAmmo;

    public Action<bool> OnHideEggButton;
    public Action<PigMaterialHandler, int> OnScythePig;

    public Action<bool> OnUseChainedAmmo;
    public Action OnInfiniteLives;
    public Action<float> OnAdjustConstantSpeed;
    public Action<int> OnUpdateAmmo;
    public Action<int> OnUpdateShotgunAmmo;

    public Action<bool> OnSwitchAmmo;
    public Action<PlayerMovementData> OnSetNewPlayerMovementData;

    public Action<bool> OnEggButton;

    public Action<int> OnEmptyAmmo;

    public Action<Vector2, float> OnPlayerVelocityChange;

    public Action<int> OnAmmoEvent;

    public Action FillPlayerMana;

    public Action OnGetMana;
    public Action<int> OnAddAmmo;
    public Action<int> OnUpdateScore;
    public Action<int> OnAddScore;
    public Action<float> EggVelocity;
   

    public Action<int> OnBucketExplosion;
    public Action<bool> OnUseStamina;

    public Action<int> OnKillPig;
    // float durationVar, float centerDurationVar, bool clockwise, Transform trans, Vector2 targetPos
    public Action<float, float, bool, Transform, Vector2, bool> OnEnterNextSectionTrigger;

    public Action ExitSectionTrigger;

    public Action<bool, string[], float, float, bool, bool> OnSetInputs;
    public Action<bool> OnEnterBubble;
    public Action OnInputWithSpecialEnableButtons;
    // public Action<bool> OnPlayerDamaged;

    public Action<bool> OnPlayerFrozen;

    public Action OnReleaseCage;
    public Action<int> SetCageAngle;

    public Action OnZeroStamina;
    public Action AddMana;
    public Action<float> AddPowerUse;

    public Action UsePower;

    public Action<bool> CanDash;
    public Action<bool> CanDrop;

    public Action<bool> CanDashSlash;
    public Action<bool> SetCanDashSlash;
    public Action<float> timeLeftOfMinDash;


    public Action<float, float> ShakeCamera;

    public Action<bool> HighlightDash;
    public Action<bool> HighlightDrop;
    public Action<bool> HighlightEgg;

    public Action OnOffScreen;

    public Action OnFinishedLevel;
    public Action<Vector2, float> ThrowItem;
    public Action<int, bool> EquipItem;

    public Action KillPlayer;


}
