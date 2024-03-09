using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashSlashState : PlayerBaseState
{

    
    private bool startRotation;
    private float currentRotation;
    private float rotationSpeedVar = 580;
    private float totalRotation;
    private bool hasSlashed;

    public override void EnterState(PlayerStateManager player)
    {
        player.ChangeCollider(-1);
        player.Sword.SetActive(true);
        currentRotation = 0;
        hasSlashed = false;
        player.maxFallSpeed = -9f;
        totalRotation = 0;
        startRotation = false;
       
        player.disableButtons = true;
        player.anim.SetTrigger("IdleTrigger");






    }
    public override void ExitState(PlayerStateManager player)

    {
        player.ChangeCollider(0);

        player.maxFallSpeed = player.ID.MaxFallSpeed;

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        if (!hasSlashed)
        {
            player.rb.velocity = new Vector2(2.1f, 0);
        }



    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {

    // }

    public override void RotateState(PlayerStateManager player)
    {
        if (hasSlashed)
        {

            player.BaseRotationLogic();

        }
        else 
        {
            currentRotation -= rotationSpeedVar * Time.deltaTime;
            totalRotation -= rotationSpeedVar * Time.deltaTime;
            player.transform.rotation = Quaternion.Euler(0, 0, currentRotation);

            if (totalRotation <= -360)
            {
                
                player.disableButtons = false;
                player.Sword.SetActive(false);
                player.ChangeCollider(0);
                startRotation = false;
                hasSlashed = true;

            }

        }


    }

    public override void UpdateState(PlayerStateManager player)
    {

        // time += Time.deltaTime;
        // if (time > .1f)
        // {
        //     startRotation = true;

        // }
        // else
        // {

        // }

    }


}
