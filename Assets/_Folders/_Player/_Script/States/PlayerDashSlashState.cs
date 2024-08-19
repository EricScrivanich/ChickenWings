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

    private bool hasExited;
    private bool buttonsEnabled;
    private float buttonsDisabledTimer;
    private bool ignoreForce;

    public override void EnterState(PlayerStateManager player)
    {
        ignoreForce = false;


        time = 0;
        buttonsEnabled = false;
        buttonsDisabledTimer = 0;
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


        player.anim.SetTrigger(player.DashSlashTrigger);
        AudioManager.instance.PlaySwordSlashSound();


    }

    public override void ExitState(PlayerStateManager player)

    {
        player.ChangeCollider(0);

        player.maxFallSpeed = player.originalMaxFallSpeed;

        if (!hasSlashed)
        {
            player.anim.SetTrigger(player.DashSlashFinishTrigger);
        }

    }

    public void IgnoreForce(bool ignore)
    {
        ignoreForce = ignore;

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        // if (!player.rotateSlash && player.rb.velocity.x > 1)
        // {
        //     currentXVelocity -= 11.5f * Time.deltaTime;
        //     player.rb.velocity = new Vector2(currentXVelocity, 0);
        // }

        if (ignoreForce)
        {
            player.AdjustForce(new Vector2(-3, .1f));
            if (hasSlashed)
            {
                player.SwitchState(player.IdleState);
            }

        }
        if (!player.rotateSlash && !ignoreForce)
        {

            // player.rb.velocity = new Vector2(5, 0);
            player.AdjustForce(new Vector2(4, 0));
        }
        else if (!hasSlashed && !ignoreForce)
        {
            // player.rb.velocity = new Vector2(1, -.1f);
            player.AdjustForce(new Vector2(1, -.1f));


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


                // player.Sword.SetActive(false);
                player.ChangeCollider(0);
                player.anim.SetTrigger(player.DashSlashFinishTrigger);
                hasExited = true;

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
        if (!buttonsEnabled)
        {
            buttonsDisabledTimer += Time.deltaTime;
            if (buttonsDisabledTimer > .3f)
            {
                player.ID.events.EnableButtons(true);
            }

        }




    }


}
