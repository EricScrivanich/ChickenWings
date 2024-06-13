using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketCollisionState : PlayerBaseState
{
    private float xForce;
    private float yForce;
    private float time;
    private float waitDuration = .15f;
    public override void EnterState(PlayerStateManager player)
    {

        time = 0;
        player.disableButtons = true;
        player.rb.velocity = new Vector2(0, 0);
        player.maxFallSpeed = player.ID.MaxFallSpeed;
        player.rb.simulated = false;

        // player.anim.SetTrigger("ResetTrigger");
        if (player.isDropping)
        {

            // player.anim.SetTrigger("BounceTrigger");
            player.isDropping = false;
        }


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
            player.transform.position = player.bucket.position;
        }
        else
        {
            // time += Time.deltaTime;
            // if (time > waitDuration)
            // {
                if (player.transform.position.y > 4)
                {
                    yForce = 1f;
                }
                else if (player.transform.position.y > 3) 
                {
                    yForce = 2.5f;
                }
                else
                {
                    yForce = 7f;
                }


                player.rb.simulated = true;
                
            player.AdjustForce(0, yForce);

            player.disableButtons = false;
                player.bucketIsExploded = false;
                player.SwitchState(player.IdleState);

            // }



        }

        // // Debug.Log("Bukcet Transform" +player.bucket.transform.position);
        // Debug.Log("Player Transform" +player.transform.position);

    }
}
