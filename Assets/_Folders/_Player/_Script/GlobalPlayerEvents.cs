
using System;

public struct GlobalPlayerEvents 
{
   public Action<int> LoseLife;
   public Action<int> OnUpdateAmmo;
   public Action<int> OnAddAmmo;
   public Action<float> EggVelocity;
   public Action Frozen;
   
   
}
