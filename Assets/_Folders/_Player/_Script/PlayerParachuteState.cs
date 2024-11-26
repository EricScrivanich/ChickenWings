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
    private float duration = .7f;
    private float rotationSpeed = 360;

    private bool isPositive;
    private bool startRotation = false;

    private float rotationTarget;

    public float rotationFactor = 1f;
    public float smoothFactor = 10f;
    public float decelerationFactor; // This will be used to slow down the player
    public float minRotation = -45f;
    public float maxRotation = 45f;

    public PlayerBaseState lastState;




    public override void EnterState(PlayerStateManager player)
    {
        time = 0;

        // player.AdjustForce(Vector2.zero);
        player.anim.SetTrigger("NewSlash");
        player.ChangeCollider(-1);
        player.rb.gravityScale = .2f;
        player.rb.linearDamping = 1.9f;
        // player.transform.RotateAround(player.parchutePoint.transform.position, new Vector3(0, 0, 1), 100f);
        // if (player.rb.velocity.x > 7.2f)
        // {

        //     decelerationFactor = 6.7f;
        // }
        // else
        // {
        //     decelerationFactor = 3.2f;
        // }
        // time = 0;
        // startRotation = false;

        // // Adjust rotationTarget based on the sign of the x velocity
        // rotationTarget = Mathf.Sign(player.rb.velocity.x) * Mathf.Abs(player.rb.velocity.x * 8);
        // if (Mathf.Abs(player.transform.eulerAngles.z) < 5)
        // {

        // }

        // initialRotation = player.transform.rotation.z;


        // // player.anim.SetTrigger("ParachuteTrigger");
        // player.maxFallSpeed = -1;
        // startOscillation = false; // Reset the flag on state entry
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.ChangeCollider(0);

        player.rb.gravityScale = player.originalGravityScale;
        player.rb.linearDamping = 0;
        player.rotateWithLastState = false;

        // player.isParachuting = false;
        // player.ImageTransform.localRotation = Quaternion.Euler(0, 0, 0);
        // // player.anim.SetTrigger("IdleTrigger");
        // // player.StartFillStaminaCoroutine();
        // player.maxFallSpeed = player.originalMaxFallSpeed;



    }



    public override void FixedUpdateState(PlayerStateManager player)
    {
        // float decelerationDirection = player.rb.velocity.x > 0 ? -1 : 1;
        // player.rb.velocity = new Vector2(player.rb.velocity.x + decelerationDirection * decelerationFactor * Time.deltaTime, player.rb.velocity.y);

        // // Set velocity to 0 if it's near zero to prevent jitter
        // if (Mathf.Abs(player.rb.velocity.x) < 0.01f)
        // {
        //     player.rb.velocity = new Vector2(0, player.rb.velocity.y);
        // }
    }
    public override void RotateState(PlayerStateManager player)
    {



        // if (player.justFlipped)
        // {
        //     if (!startRotation)
        //     {
        //         // Calculate the current angle in degrees
        //         float currentAngle = player.transform.eulerAngles.z;

        //         // Ensure the angle is within the -180 to 180 range for consistency
        //         if (currentAngle > 180) currentAngle -= 360;

        //         // Adjust the current angle based on the x velocity and the completion of a flip
        //         if (player.rb.velocity.x > 0 && currentAngle < 0)
        //         {
        //             // Adjust for clockwise movement if flip hasn't completed
        //             currentAngle += 360;
        //         }
        //         else if (player.rb.velocity.x < 0 && currentAngle > 0)
        //         {
        //             // Adjust for counter-clockwise movement if flip hasn't completed
        //             currentAngle -= 360;
        //         }

        //         // Calculate the step based on constant speed and time delta
        //         float step = rotationSpeed * Time.deltaTime;

        //         // Calculate the new angle by moving towards the target by the step amount
        //         float newAngle = Mathf.MoveTowards(currentAngle, rotationTarget, step);

        //         // Apply the new rotation, ensuring the angle is wrapped correctly
        //         player.transform.rotation = Quaternion.Euler(0, 0, newAngle % 360);

        //         // Check if the rotation has reached the target (within a small threshold to account for floating-point inaccuracies)
        //         if (Mathf.Abs(newAngle - rotationTarget) < 0.1f || Mathf.Abs((newAngle % 360) - rotationTarget) < 0.1f)
        //         {
        //             startRotation = true;
        //         }
        //     }
        //     else
        //     {
        //         // Continue with your existing logic for the ImageTransform rotation
        //         player.ImageTransform.localRotation = Quaternion.Lerp(player.ImageTransform.localRotation, Quaternion.Euler(0, 0, -rotationTarget), Time.deltaTime * 1.5f);
        //     }

        // }

        // else
        // {
        //     rotationTarget = Mathf.Sign(player.rb.velocity.x) * Mathf.Abs(player.rb.velocity.x * 8);
        //     player.ImageTransform.localRotation = Quaternion.Lerp(player.ImageTransform.localRotation, Quaternion.Euler(0, 0, rotationTarget), Time.deltaTime *1.5f);
        // }

    }


    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {
    //     // Handle collision events here, if needed
    // }

    public override void UpdateState(PlayerStateManager player)
    {
        if (time < duration)
        {
            time += Time.deltaTime;

            player.rb.linearDamping = Mathf.Lerp(1.9f, 4.5f, duration / time);
        }
        else
        {
            player.SwitchState(player.IdleState);
        }
        // player.UseStamina(52);
    }
}

