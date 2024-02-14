using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParachuteState : PlayerBaseState
{
    private float amplitude = 300f; // Oscillation amplitude in degrees
    private float frequency = 0.5f; // Oscillations per second
    private float lerpSpeed = 30f; // Speed for lerp to 0
    private bool startOscillation = false; // Flag to start oscillation
    private float initialRotation;
    private float time;
    private float rotationSpeed = 360;

    public override void EnterState(PlayerStateManager player)
    {
        time = 0;
        player.rb.velocity = new Vector2(player.rb.velocity.x / 3, player.rb.velocity.y);
        initialRotation = player.transform.rotation.z;
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

       
        player.disableButtons = true;
        // player.anim.SetBool("ParachuteBool", true);
        player.anim.SetTrigger("ParachuteTrigger");

        player.maxFallSpeed = -1;

        startOscillation = false; // Reset the flag on state entry
    }
    public override void ExitState(PlayerStateManager player)

    {
        player.isParachuting = false;
        // player.anim.SetBool("ParachuteBool", false);
        player.anim.SetTrigger("IdleTrigger");
        player.StartFillStaminaCoroutine();
        player.maxFallSpeed = player.ID.MaxFallSpeed;
        player.disableButtons = false;

    }

    public override void RotateState(PlayerStateManager player)
    {


        // // Rotate the player around the pivot by the angle difference
        // player.transform.RotateAround(player.ParachuteObject.transform.position, Vector3.forward, -initialRotation * rotationSpeed * Time.fixedDeltaTime);
        // if (Mathf.Abs(initialRotation + player.transform.rotation.z) < Mathf.Abs(initialRotation / 2))
        // {


        //     if (rotationSpeed < 0)
        //     {

        //         initialRotation = player.transform.rotation.z;
        //         Debug.Log("second" + initialRotation);
        //         rotationSpeed = 360;

        //     }
        // }
    }



    // Helper function to normalize angles to [-180, 180] range




    public override void FixedUpdateState(PlayerStateManager player)
    {
        // Implement any physics-related updates here
    }

    public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    {
        // Handle collision events here
    }

    public override void UpdateState(PlayerStateManager player)
    {
        player.UseStamina(65);
        // Implement any frame-based updates here

        // Example: Check for state transition conditions
        // if (conditionToTransition)
        // {
        //     player.SwitchState(player.SomeOtherState);
        // }
    }
}

