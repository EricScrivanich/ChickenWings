using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlashState : PlayerBaseState
{
    
    private float slashDuration = .4f;
    private float slashTime;
   
    
    [SerializeField] private float attackCooldown;

    
    public override void EnterState(PlayerStateManager player)
    {
        slashTime = 0;
        player.anim.SetTrigger("Slash");
       
        player.rb.velocity = new Vector2(0,0);
         player.rb.gravityScale = 0;
        
        
        player.slashBox.enabled = true;
        
    }

    public override void UpdateState(PlayerStateManager player)
    {
        slashTime += Time.deltaTime;
        if (slashTime > slashDuration)
        {
            player.rb.gravityScale = player.originalGravityScale;
            
            player.slashBox.enabled = false;
            player.SwitchState(player.IdleState);
        }
       
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
       
    }

    public override void RotateState(PlayerStateManager player)
    {
       
    }

    public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    {
       
    }
}
