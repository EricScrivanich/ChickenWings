using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private float jumpForce = 11.3f;
  
     
    public override void EnterState(PlayerStateManager player)
    {
        player.rb.velocity = new Vector2(0, jumpForce);
        AudioManager.instance.PlayCluck();
        player.anim.SetTrigger("JumpTrigger");
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
      
    }

    public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    {
      
    }

    public override void RotateState(PlayerStateManager player)
    {
       player.BaseRotationLogic();
       
    }

    public override void UpdateState(PlayerStateManager player) 
    {
        if (player.jumpHeld)
        {
            player.SwitchState(player.HoldJumpState);
        }
        
    }
}
