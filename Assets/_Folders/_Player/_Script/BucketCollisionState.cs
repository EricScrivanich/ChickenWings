using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketCollisionState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.disableButtons = true;
        player.rb.velocity = new Vector2(0, 0);
        player.rb.simulated = false;

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {


    }

    public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    {

    }

    public override void RotateState(PlayerStateManager player)
    {
        
    }

    public override void UpdateState(PlayerStateManager player)
    {
        player.transform.position = player.bucket.transform.position;
        // // Debug.Log("Bukcet Transform" +player.bucket.transform.position);
        // Debug.Log("Player Transform" +player.transform.position);

    }
}
