using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFrozenState : PlayerBaseState
{

    private float waitAtTopDuration = .35f;
    private float time;
    private bool rotate;
    // Start is called before the first frame update
    public override void EnterState(PlayerStateManager player)
    {
        time = 0;
        rotate = false;
        player.ID.globalEvents.Frozen.Invoke();
        AudioManager.instance.PlayFrozenSound();
        player.ID.globalEvents.OnPlayerFrozen?.Invoke(true);
        player.anim.SetBool(player.FrozenBool, true);
        // player.disableButtons = true;
        player.rb.velocity = new Vector2(0, 0);
        player.maxFallSpeed = 0;
        player.rb.freezeRotation = true;


    }
    public override void ExitState(PlayerStateManager player)
    {

        player.rb.freezeRotation = false;
        player.maxFallSpeed = player.originalMaxFallSpeed;

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {

    }



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
            player.maxFallSpeed = -4.7f;
            rotate = true;

        }
        if (player.transform.position.y < 1.7f)
        {
            // player.disableButtons = false;
            player.ID.events.EnableButtons?.Invoke(true);
            player.ID.globalEvents.OnPlayerFrozen?.Invoke(false);


            player.anim.SetBool(player.FrozenBool, false);
            player.maxFallSpeed = player.originalMaxFallSpeed;

            player.isFrozen = false;



            // player.CheckIfIsTryingToParachute();
        }

    }
}
