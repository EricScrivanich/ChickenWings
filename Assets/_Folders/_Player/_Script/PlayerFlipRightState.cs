using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlipRightState : PlayerBaseState
{
     private float currentRotation = 0;
    private float totalRotation;
    private float flipForceX = 6.5f;
    private float flipForceY = 10f; 
    private float rotationSpeedVar = -400;
  
    
    public override void EnterState(PlayerStateManager player)
    {
        player.anim.SetTrigger("FlipTrigger");
        totalRotation = 0;
        
        currentRotation = player.transform.rotation.eulerAngles.z;
        player.rb.velocity = new Vector2(player.flipRightForceX, player.flipRightForceY);
        AudioManager.instance.PlayCluck();
        
      
       
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
        currentRotation += rotationSpeedVar * Time.deltaTime;
        totalRotation -= rotationSpeedVar * Time.deltaTime;
        player.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        // Debug.Log("right: " + currentRotation);
       
    }

    public override void UpdateState(PlayerStateManager player)
    {
       
         if (totalRotation > 420)
        {
            

            player.SwitchState(player.IdleState);
        }
        
        
    }
}

