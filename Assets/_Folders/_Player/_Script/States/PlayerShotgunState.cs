using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotgunState : PlayerBaseState
{
    // private float ignoreParticleTimer = 0;
    // private float ignoreParticleDuration = .3f;
    // private bool ignoringTime = true;
    public override void EnterState(PlayerStateManager player)
    {
        // ignoringTime = true;

        // ignoreParticleTimer = 0;
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
        player.rb.angularDrag = .35f;
        player.rb.gravityScale *= .95f;

        if (player.transform.rotation.eulerAngles.z > 270 || player.transform.rotation.eulerAngles.z < 90)
        {
            player.rb.angularVelocity = 460;
        }
        else
        {
            player.rb.angularVelocity = -460;
        }


    }
    public override void ExitState(PlayerStateManager player)
    {
        player.maxFallSpeed = player.originalMaxFallSpeed;
        player.rb.gravityScale = player.originalGravityScale;
        // player.rb.angularVelocity = 0;

        // if (ignoringTime) player.ignoreParticleCollision = false;



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
        // if (ignoringTime)
        // {
        //     ignoreParticleTimer += Time.deltaTime;

        //     if (ignoreParticleTimer >= ignoreParticleDuration)
        //     {
        //         ignoringTime = false;
        //         player.ignoreParticleCollision = false;
        //     }
        // }


    }


}
