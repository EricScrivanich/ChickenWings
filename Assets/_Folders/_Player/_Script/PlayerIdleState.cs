using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState :PlayerBaseState
{
   
    public override void EnterState(PlayerStateManager player)
    {
        // player.anim.SetTrigger("IdleTrigger");
        player.rb.gravityScale = player.originalGravityScale;
       
       
    }
    public override void ExitState(PlayerStateManager player)

    {

    }

    public override void FixedUpdateState(PlayerStateManager player)
    { 
        
     
    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {
      
    // }

    public override void RotateState(PlayerStateManager player)
    {
        player.BaseRotationLogic();
       
       
    }

    public override void UpdateState(PlayerStateManager player)
    {
       
    }

   
}
