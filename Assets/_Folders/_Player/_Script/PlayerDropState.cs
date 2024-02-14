using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDropState : PlayerBaseState
{
    private float dropPower = -15f;
    public override void EnterState(PlayerStateManager player)
    {
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        player.ID.events.FloorCollsion.Invoke(false); 
        AudioManager.instance.PlayDownJumpSound();
        player.maxFallSpeed = -50;
        player.anim.SetTrigger("DropTrigger");
        player.disableButtons = true;
        player.rb.velocity = new Vector2 (0,dropPower);

       
    }
    public override void ExitState(PlayerStateManager player)

    {

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
      
     
    }

    public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
            {
                player.rb.velocity = new Vector2 (0,0);
                player.anim.SetBool("DropBool",false);
                player.maxFallSpeed = player.ID.MaxFallSpeed;

                player.SwitchState(player.BounceState);
            }

      
    }

   

    public override void RotateState(PlayerStateManager player)
    {
       
    }

    public override void UpdateState(PlayerStateManager player)
    {
        
    }
}