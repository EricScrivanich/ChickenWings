using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFrozenState : PlayerBaseState
{
    private float frozenRotZ;
    private float frozenRotationSpeed = 400f;
    // Start is called before the first frame update
    public override void EnterState(PlayerStateManager player)
    {
        player.ID.globalEvents.Frozen.Invoke();
        AudioManager.instance.PlayFrozenSound();
        player.anim.SetBool("FrozenBool",true);
        player.disableButtons = true;
        player.rb.velocity = new Vector2 (0,0);
        frozenRotZ = 0;
        player.maxFallSpeed = -5.2f;
        
       
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        
    }

    public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    {
        
    }

    public override void RotateState(PlayerStateManager player)
    {
        frozenRotZ += frozenRotationSpeed * Time.deltaTime;
        player.transform.rotation = Quaternion.Euler(0,0, frozenRotZ);
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (player.transform.position.y < 1.2f)
        {
            player.disableButtons = false;
            player.anim.SetBool("FrozenBool",false);
            player.maxFallSpeed = player.ID.MaxFallSpeed;
            
            player.SwitchState(player.IdleState);
        }
        
    }
}
