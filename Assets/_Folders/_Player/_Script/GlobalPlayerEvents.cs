
using System;

public struct GlobalPlayerEvents 
{
   public Action<int> LoseLife;
   public Action OnUpdateAmmo;
   public Action<int> OnAddAmmo;
   public Action<float> EggVelocity;
   public Action Frozen;

    public Action OnBucketExplosion;
    public Action<bool> OnUseStamina;
    public Action OnZeroStamina;


}
