using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashSlashState : PlayerBaseState
{


    private bool startRotation;
    private float currentRotation;
    private float currentRotationBefore;
    private float rotationSpeedVar;
    private float totalRotation;
    private float currentRotationVar;
    private float startingRotationSpeed = 900f;
    private bool hasSlashed;
    private float time;
    private float currentXVelocity;

    public override void EnterState(PlayerStateManager player)
    {

       
        time = 0;
        rotationSpeedVar = startingRotationSpeed;
        player.rotateSlash = false;
        player.ChangeCollider(-1);


        // player.Sword.SetActive(true);
        currentRotation = 0;
        currentRotationBefore = 0;
        hasSlashed = false;
        player.maxFallSpeed = -9f;
        totalRotation = 0;
        startRotation = false;


        player.anim.SetTrigger("DashSlashTrigger");
        AudioManager.instance.PlaySwordSlashSound();







    }
    public override void ExitState(PlayerStateManager player)

    {
        player.ChangeCollider(0);

        player.maxFallSpeed = player.ID.MaxFallSpeed;

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        // if (!player.rotateSlash && player.rb.velocity.x > 1)
        // {
        //     currentXVelocity -= 11.5f * Time.deltaTime;
        //     player.rb.velocity = new Vector2(currentXVelocity, 0);
        // }
        if (!player.rotateSlash)
        {

            // player.rb.velocity = new Vector2(5, 0);
            player.AdjustForce(4, 0);
        }
        else if (!hasSlashed)
        {
            // player.rb.velocity = new Vector2(1, -.1f);
            player.AdjustForce(1, -.1f);


        }



    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {

    // }

    public override void RotateState(PlayerStateManager player) 
    {
        if (hasSlashed)
        {
            return;
            // player.BaseRotationLogic();

        }
        else if (player.rotateSlash)
        {
            time += Time.fixedDeltaTime;

            currentRotation -= rotationSpeedVar * Time.fixedDeltaTime;
            // totalRotation -= rotationSpeedVar * Time.deltaTime;



            rotationSpeedVar = Mathf.Lerp(startingRotationSpeed, 100, time / .9f);

            float newRotation = Mathf.LerpAngle(player.rb.rotation, currentRotation, Time.deltaTime * rotationSpeedVar);

            // Apply the new rotation
            player.rb.MoveRotation(newRotation);
            // player.transform.rotation = Quaternion.Euler(0, 0, currentRotation);


            if (currentRotation <= -400)
            {

                player.disableButtons = false;
                // player.Sword.SetActive(false);
                player.ChangeCollider(0);
                player.anim.SetTrigger("DashSlashFinishTrigger");

                startRotation = false;
                hasSlashed = true;

            }

        }
        else
        {
            currentRotation += 120 * Time.deltaTime;
            player.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
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
