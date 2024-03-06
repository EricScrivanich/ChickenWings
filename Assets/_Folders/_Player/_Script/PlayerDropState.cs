using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDropState : PlayerBaseState
{
    private float dropPower = -15f;
    public override void EnterState(PlayerStateManager player)
    {
        
        player.ChangeCollider(2);
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        // player.ID.events.FloorCollsion.Invoke(false); 
        AudioManager.instance.PlayDownJumpSound();
        player.maxFallSpeed = -50;
        player.anim.SetTrigger("DropTrigger");
        player.disableButtons = true;
        player.rb.velocity = new Vector2 (0,dropPower);

       
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Ring"))
    //     {
    //         foreach (ContactPoint2D contact in collision.contacts)
    //         {
    //             Vector2 normal = contact.normal;

    //             // Determine direction based on the normal
    //             // Assuming a simple scenario where a positive normal.x suggests a hit from the left
    //             if (normal.x > 0)
    //             {
                    
    //                 // Move player to the right
    //                 player.transform.position += Vector3.right * dodgeDistance;
    //             }
    //             else
    //             {
    //                 // Move player to the left
    //                 transform.position += Vector3.left * dodgeDistance;
    //             }

    //             break; // Assuming we only care about the first contact point
    //         }
    //     }
    // }
    public override void ExitState(PlayerStateManager player)

    {
        

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        // player.transform.rotation = Quaternion.Euler(0, 0, 0);
     
    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {
    //     if (collision.gameObject.tag == "Floor")
    //         {
    //             player.rb.velocity = new Vector2 (0,0);
    //             player.anim.SetBool("DropBool",false);
    //             player.maxFallSpeed = player.ID.MaxFallSpeed;

    //             player.SwitchState(player.BounceState);
    //         }

      
    // }

   

    public override void RotateState(PlayerStateManager player)
    {
       
    }

    public override void UpdateState(PlayerStateManager player)
    {
        
    }
}