using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlipLeftState : PlayerBaseState
{
    private float currentRotation;
    private float totalRotation;
    private float flipForceX = -6.9f;
    private float flipForceY = 9.5f;
    private float rotationSpeedVar = 370;
    private float rotationDelay = .1f;
    private float rotationTime = 0;
    private float flipThreshold;

    public override void EnterState(PlayerStateManager player)
    {
        rotationTime = 0;
        totalRotation = 0;
        player.anim.SetTrigger("FlipTrigger");

        currentRotation = player.transform.rotation.eulerAngles.z;
        player.rb.velocity = new Vector2(player.flipLeftForceX, player.flipLeftForceY);
        AudioManager.instance.PlayCluck();
        float rotationCheck = 360 - currentRotation;
        
        
       
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
        if (rotationTime < rotationDelay)
        {
            rotationTime += Time.deltaTime;
        }
        else{
            currentRotation += rotationSpeedVar * Time.deltaTime;
            totalRotation += rotationSpeedVar * Time.deltaTime;
            player.transform.rotation = Quaternion.Euler(0, 0, currentRotation);

        }
       
        

        
       
       
    }

    public override void UpdateState(PlayerStateManager player)
    {
      
        if (totalRotation > 370)
        {
           

            player.SwitchState(player.IdleState);
        }
    }
}
