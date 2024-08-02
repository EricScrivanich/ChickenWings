using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDropState : PlayerBaseState
{
    private float dropPower = -14f;
    public override void EnterState(PlayerStateManager player)
    {

        player.ChangeCollider(2);

        player.rb.rotation = 0;
        // player.ID.events.FloorCollsion.Invoke(false); 
        AudioManager.instance.PlayDownJumpSound();
        player.maxFallSpeed = -50;
        // player.anim.SetTrigger("DropTrigger");
       
        // player.rb.velocity = new Vector2 (0,dropPower);
        player.AdjustForce(0, dropPower);
        player.rb.freezeRotation = true;



    }


    public override void ExitState(PlayerStateManager player)

    {
        // player.anim.SetTrigger(player.FinishDashTrigger);

        player.rb.freezeRotation = false;

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {


    }


    public override void RotateState(PlayerStateManager player)
    {

    }

    public override void UpdateState(PlayerStateManager player)
    {

    }
}