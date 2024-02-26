using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFrozenState : PlayerBaseState
{
    private float frozenRotZ;
    private float frozenRotationSpeed = 380f;
    private float waitAtTopDuration = .45f;
    private float time;
    private bool rotate;
    // Start is called before the first frame update
    public override void EnterState(PlayerStateManager player)
    {
        time = 0;
        rotate = false;
        player.ID.globalEvents.Frozen.Invoke();
        AudioManager.instance.PlayFrozenSound();
        player.anim.SetBool("FrozenBool",true);
        player.disableButtons = true;
        player.rb.velocity = new Vector2 (0,0);
        player.maxFallSpeed = 0;
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        frozenRotZ = 0;
       
    }
    public override void ExitState(PlayerStateManager player)

    {

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        
    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {
        
    // }
   
    public override void RotateState(PlayerStateManager player)
    {
        // if (rotate)
        // {
        //     frozenRotZ += frozenRotationSpeed * Time.deltaTime;
        //     player.transform.rotation = Quaternion.Euler(0,0, frozenRotZ);

        // }
        
    }

    public override void UpdateState(PlayerStateManager player)
    {
        time += Time.deltaTime;
        if (time > waitAtTopDuration)
        {
            player.maxFallSpeed = -4.5f;
            rotate = true; 

        }
        if (player.transform.position.y < 1)
        {
            player.disableButtons = false;
            player.anim.SetBool("FrozenBool",false);
            player.maxFallSpeed = player.ID.MaxFallSpeed;

            player.CheckIfIsTryingToParachute();
        }
        
    }
}
