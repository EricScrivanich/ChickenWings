
using System;

public struct GlobalPlayerEvents
{
    public Action<int> OnUpdateLives;
    public Action OnUpdateAmmo;
    public Action<int> OnAddAmmo; 
    public Action<int> OnUpdateScore;
    public Action<float> EggVelocity;
    public Action Frozen;

    public Action<int> OnBucketExplosion;
    public Action<bool> OnUseStamina;
    
    public Action OnZeroStamina;
    public Action AddMana;
    public Action<float> AddPowerUse;

    public Action UsePower;


}
