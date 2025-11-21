using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDropState : PlayerBaseState
{

    private readonly Vector2 dropForce = new Vector2(0, -13);
    private bool switchedToBounce;
    public override void EnterState(PlayerStateManager player)
    {
        player.DoDropCooldown();

        player.ChangeCollider(2);
        player.SetHingeTargetAngle(180);
        player.rb.angularVelocity = 0;
        player.anim.SetBool(player.BounceBool, false);
        switchedToBounce = false;

        player.rb.rotation = 0;
        // player.ID.events.FloorCollsion.Invoke(false); 
        AudioManager.instance.PlayDownJumpSound();
        player.maxFallSpeed = -50;
        // player.anim.SetTrigger("DropTrigger");

        // player.rb.velocity = new Vector2 (0,dropPower);
        player.AdjustForce(dropForce);
        player.rb.freezeRotation = true;



    }

   

    public override void ExitState(PlayerStateManager player)

    {
        if (!switchedToBounce)
        {
            player.isDropping = false;

        }

        // player.anim.SetTrigger(player.FinishDashTrigger);

        player.rb.freezeRotation = false;
        player.maxFallSpeed = player.originalMaxFallSpeed;
        player.anim.SetTrigger(player.FinishDropTrigger);

    }

    public void SwitchToBounce()
    {
        switchedToBounce = true;

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