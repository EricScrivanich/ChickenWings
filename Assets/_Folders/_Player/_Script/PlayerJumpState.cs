using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private float jumpForce = 11.3f;
    private float slightUpwardsForce = 12f; 


    public override void EnterState(PlayerStateManager player)
    {
        player.anim.SetTrigger("JumpTrigger");
        
        player.rb.velocity = new Vector2(0, player.jumpForce);
        // player.rb.AddForce(new Vector2(0, player.jumpForce),ForceMode2D.Impulse);
        AudioManager.instance.PlayCluck();

        // player.anim.SetBool("FlipRightBool", false);
        // player.anim.SetBool("FlipLeftBool", false);

    }
    public override void ExitState(PlayerStateManager player)

    {
        
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        if (player.ID.isHolding && player.rb.velocity.y > -5)
        {
            player.rb.AddForce(new Vector2(0, player.ID.addJumpForce - Mathf.Abs(player.rb.velocity.y)));
            Debug.Log("Adding");

        }
        else if (Mathf.Abs(player.rb.velocity.y) < 0.5f)
        {
            // Apply a small upwards force
            // You can adjust this value as needed
            player.rb.AddForce(new Vector2(0, slightUpwardsForce));
        }

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
        if (player.jumpHeld)
        {
            player.SwitchState(player.HoldJumpState);
        }

    }
}
