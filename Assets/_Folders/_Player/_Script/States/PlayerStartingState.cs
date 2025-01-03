
using UnityEngine;

public class PlayerStartingState : PlayerBaseState
{

    private bool switchBool = false;
    private float time;
    private float currentRotation;
    private float rotationSpeedVar = 350;
    private float rotateDuration = .9f;
    private float lockedDuration = .5f;
    private bool hasFinishedLocked;
    private float startMaxFallSpeed = -4f;
    private Vector2 startPosition = new Vector2(-12.5f, 2f);
    private Vector2 initialForce = new Vector2(8.3f, 8.9f);
    public override void EnterState(PlayerStateManager player)
    {

        hasFinishedLocked = false;
        player.maxFallSpeed = startMaxFallSpeed;
        currentRotation = 120;
        player.transform.position = startPosition;
        player.AdjustForce(initialForce);

        // player.transform.position = Vector2.zero;




        // player.rb.velocity = new Vector2(0, 5);


        time = 0;
        AudioManager.instance.PlayStartSound();

    }
    public override void ExitState(PlayerStateManager player)
    {

        player.maxFallSpeed = player.originalMaxFallSpeed;



        if (!hasFinishedLocked)
        {
            player.rb.gravityScale = player.originalGravityScale;

            if (!player.usingConstantForce)
            {
                player.SetAddForceAtBoundaries(true);
            }
            if (!player.DisableButtonsAtStart)
            {
                player.ID.events.EnableButtons?.Invoke(true);

            }

        }


    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        if (time < rotateDuration)
        {
            time += Time.fixedDeltaTime;

            // Use SmoothStep to ease out the rotation near the end
            float easedTime = Mathf.SmoothStep(0, 1, time / rotateDuration);

            // Ensure always clockwise rotation
            float targetRotation = Mathf.LerpAngle(currentRotation, -20, easedTime);
            targetRotation = Mathf.Repeat(targetRotation, 360); // Normalize the angle to keep it within 0-360

            if (targetRotation < currentRotation)
            {
                targetRotation = Mathf.LerpAngle(currentRotation, 360 + (-20), easedTime); // Continue rotating clockwise
                player.maxFallSpeed = Mathf.Lerp(startMaxFallSpeed, player.originalMaxFallSpeed, easedTime);

            }

            player.rb.MoveRotation(targetRotation);

            if (time > lockedDuration && !hasFinishedLocked)
            {
                hasFinishedLocked = true;
                player.rb.gravityScale = player.originalGravityScale;
                if (!player.usingConstantForce)
                {
                    player.SetAddForceAtBoundaries(true);
                }
                if (!player.DisableButtonsAtStart)
                {
                    player.ID.events.EnableButtons?.Invoke(true);
                }


            }
        }
    }

    public override void RotateState(PlayerStateManager player)
    {

    }

    public override void UpdateState(PlayerStateManager player)
    {



    }


}
