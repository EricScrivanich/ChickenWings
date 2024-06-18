
using System;

public struct GlobalPlayerEvents
{
    public Action<int> OnUpdateLives;
    public Action OnInfiniteLives;
    public Action<float> OnAdjustConstantSpeed;
    public Action OnUpdateAmmo;
    public Action<int> OnAddAmmo;  
    public Action<int> OnUpdateScore;
    public Action<int> OnAddScore;
    public Action<float> EggVelocity;
    public Action Frozen;

    public Action<int> OnBucketExplosion;
    public Action<bool> OnUseStamina;
    
    public Action OnZeroStamina;
    public Action AddMana; 
    public Action<float> AddPowerUse;

    public Action UsePower;

    public Action<bool> CanDash;
    public Action<bool> CanDrop;

    public Action<bool> CanDashSlash;
    public Action<bool> SetCanDashSlash;

    public Action<float, float> ShakeCamera;

    public Action<bool> HighlightDash;
    public Action<bool> HighlightDrop;
    public Action<bool> HighlightEgg;

    public Action OnOffScreen;


}
