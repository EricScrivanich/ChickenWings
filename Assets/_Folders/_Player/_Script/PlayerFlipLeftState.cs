using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlipLeftState : PlayerBaseState
{
    private Vector2 AddForceVector = new Vector2(-9f, 8);
    private float currentRotation;
    private float totalRotation;
    private float flipForceX = -6.9f;

    private float flipForceY = 9.5f;
    // private float rotationSpeed = 420;
    private float rotationSpeed = 100;
    private float doesTiggerInt;
    private float rotationSpeedVar;
    private float rotationDelay = .15f;
    private float rotationTime = 0;
    private float flipThreshold;
    private float time;
    private bool hitSlowTarget;
    private bool hitHeightTarget;
    private bool prolongRotation;

    public override void EnterState(PlayerStateManager player)
    {
        hitSlowTarget = false;
        hitHeightTarget = false;

        time = 0;
        player.SetFlipDirection(false);
        rotationSpeedVar = 300;
        player.anim.SetTrigger("FlipTrigger");
        if (player.ID.testingNewGravity)
        {
            player.rb.gravityScale = player.ID.flipGravity;

        }

        totalRotation = 0;
        currentRotation = player.transform.rotation.eulerAngles.z;

        if (player.ID.UsingClocker)
        {
            rotationSpeed = 300;
        }
        else if (currentRotation > 140)
        {
            // rotationSpeed += (360 - currentRotation) / 3;
            prolongRotation = true;
        }
        else
        {
            // rotationSpeed = 420;
            prolongRotation = false;
        }
        // player.rb.velocity = new Vector2(player.flipLeftForceX, player.flipLeftForceY);
        player.AdjustForce(player.flipLeftForceX, player.flipLeftForceY);
        AudioManager.instance.PlayCluck();


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

    public void ReEnterState()
    {
        rotationSpeedVar = 300;

    }

    public override void RotateState(PlayerStateManager player)
    {
        currentRotation += rotationSpeedVar * Time.fixedDeltaTime;
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
        if (!hitHeightTarget && player.rb.velocity.y < -1)
        {

            hitHeightTarget = true;


        }
        if (player.transform.rotation.eulerAngles.z > 330 && !hitSlowTarget && !prolongRotation)
        {
            hitSlowTarget = true;

            if (player.transform.rotation.eulerAngles.z > 30)
            {
                prolongRotation = false;

            }
        }
    }
}
