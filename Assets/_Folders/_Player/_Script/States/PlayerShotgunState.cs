using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotgunState : PlayerBaseState
{

    public override void EnterState(PlayerStateManager player)
    {
        // player.anim.SetTrigger("IdleTrigger");

        // if (player.justFlipped)
        // {
        //     if (player.justFlippedLeft)
        //     {
        //         player.rb.angularVelocity = 150;

        //     }
        //     else if (player.justFlippedRight)
        //     {
        //         player.rb.angularVelocity = -150;


        //     }
        // }
        player.maxFallSpeed = -6;

        if (player.transform.rotation.eulerAngles.z > 270 || player.transform.rotation.eulerAngles.z < 90)
        {
            player.rb.angularVelocity = 500;
        }
        else
        {
            player.rb.angularVelocity = -500;
        }


    }
    public override void ExitState(PlayerStateManager player)
    {
        player.maxFallSpeed = player.originalMaxFallSpeed;
        player.rb.angularVelocity = 0;


    }


    public override void FixedUpdateState(PlayerStateManager player)
    {


    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {

    // }

    public override void RotateState(PlayerStateManager player)
    {



    }

    public override void UpdateState(PlayerStateManager player)
    {

    }


}