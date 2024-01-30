
using UnityEngine;

public class PlayerHoldJumpState : PlayerBaseState
{
    private float rotZ = 0;
    private bool holdTimeBool;
    private float holdDuration = 1.2f;
    private float holdTime;
    public override void EnterState(PlayerStateManager player)
    {
        
        holdTime = 0;
        holdTimeBool = false;
        player.anim.SetBool("JumpHeld", true);
        // player.rb.gravityScale -= .2f;
        player.maxFallSpeed = 2;


    }

    public override void FixedUpdateState(PlayerStateManager player)
    {

    }

    public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    {

    }

    public override void RotateState(PlayerStateManager player)
    {

        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.Euler(0, 0, 15), Time.deltaTime * 5);


    }

    public override void UpdateState(PlayerStateManager player)
    {
        
            player.UseStamina(60);

        

        //     holdTime += Time.deltaTime;
        //     if (holdTime >= holdDuration)
        //     {
        //         holdTimeBool = true;
        //     }
        //     if (!player.jumpHeld || holdTimeBool)
        //     {

        //         player.SwitchState(player.IdleState);


        //     }
        // }
    }

}
