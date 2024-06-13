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
        player.disableButtons = true;
        player.ChangeCollider(-1);

        // player.anim.SetBool("AttackBool",true);
        player.attackObject.SetActive(true);


        player.rb.velocity = new Vector2(0,0);
        //  player.rb.gravityScale = 0;
        player.maxFallSpeed = .6f;



    }
    public override void ExitState(PlayerStateManager player)
    {
        player.ChangeCollider(0);

    }

    public override void UpdateState(PlayerStateManager player)
    {
        // slashTime += Time.deltaTime;
        // if (slashTime > slashDuration)
        // {
        //     player.rb.gravityScale = player.originalGravityScale;
            
        //     player.slashBox.enabled = false;
        //     player.maxFallSpeed = player.ID.MaxFallSpeed;
        //     player.SwitchState(player.IdleState);
        // }
       
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
       
    }

    public override void RotateState(PlayerStateManager player)
    {
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 900);


    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {
       
    // }
}
