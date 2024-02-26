using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketCollisionState : PlayerBaseState
{
    private float xForce;
    private float yForce;
    public override void EnterState(PlayerStateManager player)
    {


        player.disableButtons = true;
        player.rb.velocity = new Vector2(0, 0);
        player.maxFallSpeed = player.ID.MaxFallSpeed;
        player.rb.simulated = false;
        player.isDropping = false;
        // player.anim.SetTrigger("ResetTrigger");
        player.anim.SetTrigger("IdleTrigger");


    }

    public override void FixedUpdateState(PlayerStateManager player)
    {



    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {

    // }

    public override void RotateState(PlayerStateManager player)
    {

    }
    public override void ExitState(PlayerStateManager player)

    {

    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (!player.bucketIsExploded)
        {
            player.transform.position = player.bucket.baseTransform.position;
        }
        else
        {
            if (player.transform.position.y > 4)
            {
                yForce = 1.5f;
            }
            else if (player.transform.position.y > 3)
            {
                yForce = 3f;
            }
            else
            {
                yForce = 9f;
            }

            player.rb.simulated = true;
            player.rb.velocity = new Vector2(player.transform.position.x * -.25f, yForce);
            player.disableButtons = false;
            player.bucketIsExploded = false;
            player.SwitchState(player.IdleState);


        }

        // // Debug.Log("Bukcet Transform" +player.bucket.transform.position);
        // Debug.Log("Player Transform" +player.transform.position);

    }
}
