using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketCollisionState : PlayerBaseState
{
    private float xForce;
    private float yForce;
    private float time;
    private float waitDuration = .15f;
    private Transform targetPos;
    public override void EnterState(PlayerStateManager player)
    {

        time = 0;
        player.rb.angularVelocity = 0;


        player.rb.velocity = new Vector2(0, 0);
        player.maxFallSpeed = player.originalMaxFallSpeed;
        player.rb.simulated = false;
        player.ID.events.EnableButtons?.Invoke(false);


        // player.anim.SetTrigger("ResetTrigger");
        if (player.isDropping)
        {

            // player.anim.SetTrigger("BounceTrigger");
            player.isDropping = false;
        }


    }

    public void SetValues(Transform target)
    {
        targetPos = target;

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {



    }

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
            player.transform.position = targetPos.position;
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

            player.AdjustForce(new Vector2(0, yForce));

            player.ID.events.EnableButtons?.Invoke(true);
            player.bucketIsExploded = false;
            player.StartCoroutine(player.SetUndamagableCourintine(true, 1.4f));
            
            player.SwitchState(player.IdleState);

            // }



        }

        // // Debug.Log("Bukcet Transform" +player.bucket.transform.position);
        // Debug.Log("Player Transform" +player.transform.position);

    }
}
