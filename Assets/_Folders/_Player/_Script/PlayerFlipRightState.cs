using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlipRightState : PlayerBaseState
{

    private Vector2 AddForceVector = new Vector2(7.5f, 12.5f);
    private float currentRotation = 0;
    private float totalRotation;
    private float flipForceX = 6.5f;
    private float flipForceY = 10f;
    private float rotationSpeedVar;
    private float rotationSpeed = 450;
    private float rotationDelay = .15f;
    private float rotationTime = 0;
    private float time;
    private bool hitSlowTarget;
    private bool prolongRotation;
    private bool hitHeightTarget;


    public override void EnterState(PlayerStateManager player)
    {
        hitHeightTarget = false;
        hitSlowTarget = false;
        time = 0;
        player.SetFlipDirection(true);
        player.anim.SetTrigger("FlipTrigger");
        // if (player.ID.testingNewGravity)
        // {
        //     player.rb.gravityScale = player.ID.flipGravity;

        // }
        totalRotation = 0;
        // if (player.justFlipped && player.justFlippedRight)
        // {
        //     player.rb.velocity = new Vector2(player.flipRightForceX - .5f, player.flipRightForceY + .8f);

        // }
        // else
        // {


        // }
        currentRotation = player.transform.rotation.eulerAngles.z;

        if (player.ID.UsingClocker)
        {
            rotationSpeed = 300;
        }
        else if (currentRotation < 220)
        {

            prolongRotation = true;
            rotationSpeed += currentRotation / 3;
        }
        else
        {

            prolongRotation = false;
            rotationSpeed = 450;
        }
        rotationSpeedVar = rotationSpeed;
        player.rb.velocity = new Vector2(player.flipRightForceX, player.flipRightForceY);
        AudioManager.instance.PlayCluck();





    }
    public void ReEnterState()
    {
        rotationSpeedVar = 300;

    }
    public override void ExitState(PlayerStateManager player)
    {
        player.rb.gravityScale = player.originalGravityScale;

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        if (player.holdingFlip && !hitHeightTarget)
        {
            player.rb.AddForce(AddForceVector);
        }

    }

    public override void RotateState(PlayerStateManager player)
    {

        currentRotation -= rotationSpeedVar * Time.fixedDeltaTime;
        float targetRotation = Mathf.LerpAngle(player.rb.rotation, currentRotation, Time.fixedDeltaTime * rotationSpeedVar);
        player.rb.MoveRotation(targetRotation);
        if (hitSlowTarget)
        {
            time += Time.fixedDeltaTime;
            rotationSpeedVar = Mathf.Lerp(rotationSpeed, 10, time / .4f);
        }

    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (player.ID.testingNewGravity && !hitHeightTarget && player.rb.velocity.y < .5)
        {
            player.rb.gravityScale = 2.1f;
            hitHeightTarget = true;


        }
        if (player.transform.rotation.eulerAngles.z < player.ID.startLerp && !hitSlowTarget && !prolongRotation)
        {
            hitSlowTarget = true;
        }
        if (player.transform.rotation.eulerAngles.z > 330 && prolongRotation)
        {
            prolongRotation = false;
        }
    }
}

