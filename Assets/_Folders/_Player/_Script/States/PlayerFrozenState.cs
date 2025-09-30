using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFrozenState : PlayerBaseState
{

    private float waitAtTopDuration = .15f;
    private float maxFallSpeed = -7.5f;
    private Vector2 addDownForce = new Vector2(0, -1f);
    private float time;
    private bool rotate;
    // Start is called before the first frame update
    public override void EnterState(PlayerStateManager player)
    {
        time = 0;
        rotate = false;
        player.rb.angularVelocity = 0;
        // player.ID.globalEvents.Frozen?.Invoke();
        AudioManager.instance.PlayFrozenSound();
        player.ID.globalEvents.OnPlayerFrozen?.Invoke(true);
        player.anim.SetBool(player.FrozenBool, true);
        // player.disableButtons = true;
        addDownForce.x = player.rb.linearVelocity.x * .2f;
        player.rb.linearVelocity = new Vector2(0, 0);
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
        if (!rotate && time > waitAtTopDuration)
        {
            player.maxFallSpeed = maxFallSpeed;
            player.rb.linearVelocity = addDownForce;
            rotate = true;

        }
        if (player.transform.position.y < 1f)
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
