using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParachuteState : PlayerBaseState
{
    private float rotZ = 0;


    private float holdTime;
    private float rotationSpeed = 50f;
    public override void EnterState(PlayerStateManager player)
    {

        player.disableButtons = true;
        player.anim.SetBool("ParachuteBool", true);
        // player.transform.position = new Vector2(player.transform.position.x - player.ID.parachuteXOffset, player.transform.position.y + player.ID.parachuteYOffset);
        // player.rb.gravityScale -= .2f;
        player.maxFallSpeed = -1;


    }

    public override void FixedUpdateState(PlayerStateManager player)
    {

    }

    public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    {

    }

    public override void RotateState(PlayerStateManager player)
    {
        player.transform.RotateAround(player.ParachuteObject.transform.position, new Vector3(0, 0, 1), rotationSpeed * Time.deltaTime);

        // player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.Euler(0, 0, 15), Time.deltaTime * 5);


    }

    public override void UpdateState(PlayerStateManager player)
    {

        // player.UseStamina(0);



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
