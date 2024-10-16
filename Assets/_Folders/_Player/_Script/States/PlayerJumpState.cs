using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
  
    private bool hasFadedJumpAir;

    private int jumpAirIndex;
    // private float slightUpwardsForce = 12f;
    private float slightUpwardsForce = 15f;

    private Vector2 jumpForce;
    // private Vector2 removerAddedJumpForce;
    private float addJumpForce;

    private bool startHoldJumpAnimation;


    public override void EnterState(PlayerStateManager player)
    {
        // player.anim.SetTrigger("JumpTrigger"); 
        hasFadedJumpAir = false;

        jumpAirIndex = player.CurrentJumpAirIndex;

        // player.rb.velocity = new Vector2(0, player.jumpForce);
        player.AdjustForce(jumpForce);
        // player.rb.AddForce(new Vector2(0, player.jumpForce),ForceMode2D.Impulse);
        AudioManager.instance.PlayCluck();

        // player.anim.SetBool("FlipRightBool", false);
        // player.anim.SetBool("FlipLeftBool", false);

    }
    public override void ExitState(PlayerStateManager player)

    {
        if (!hasFadedJumpAir)
        {

            player.ID.events.OnStopJumpAir?.Invoke(jumpAirIndex);
        }

    }

    public void CacheVaraibles(Vector2 initialForce,float addForce)
    {
        jumpForce = initialForce;
        
        addJumpForce = addForce;

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        if (player.ID.isHolding)
        {

            player.rb.AddForce(new Vector2(0, addJumpForce - Mathf.Abs(player.rb.velocity.y)));
            startHoldJumpAnimation = true;

            if (player.rb.velocity.y < -5)
            {
                player.ID.isHolding = false;
            }


        }
       
        // else if (player.rb.velocity.y < -.5f && player.rb.velocity.y > -2)
        // {
        //     // Apply a small upwards force
        //     // You can adjust this value as needed
        //     player.rb.AddForce(removerAddedJumpForce);


        // }

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
        // if (player.jumpHeld)
        // {
        //     player.SwitchState(player.HoldJumpState);
        // }

        if (!player.ID.isHolding && !hasFadedJumpAir)
        {
            hasFadedJumpAir = true;
        }


    }
}
