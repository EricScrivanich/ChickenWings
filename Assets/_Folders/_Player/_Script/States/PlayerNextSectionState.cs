using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNextSectionState : PlayerBaseState
{
    private float time;
    private float rotateTime;
    private float duration;
    private bool isClockwise;
    private Transform targetTransform;
    private Vector3 startPosition;
    private float initialRadius;
    private Vector3 initialOffset;
    private float centerDuration;

    private Vector2 targetPosition;
    private bool reachedTargetPosition;
    private bool tweenPlayer;
    private float upDownSpeed = 2;
    private float upDownAmplitude = .1f;
    private bool doPlayerTween;

    private float maxAmpRadRatio;
    private float outerAmp;
    private float innerAmp;
    private float innerCutoff;
    private float coeff1;
    private float coeff2;
    private float newY;

    private float dragAmount;

    private float moveTime;

    private float currentRotation;
    public override void EnterState(PlayerStateManager player)
    {
        time = 0;
        moveTime = 0;
        player.rb.angularVelocity = 0;

        currentRotation = player.transform.rotation.eulerAngles.z;
        reachedTargetPosition = false;
        player.rb.gravityScale = 0;
        player.ChangeCollider(-1);
        player.AdjustForce(Vector2.zero);
        player.ID.events.EnableButtons?.Invoke(false);
        newY = targetPosition.y;

        // player.rb.simulated = false;

        if (player.isDropping)
        {
            player.isDropping = false;
        }

        if (duration > 0)
            player.anim.SetTrigger(player.FlipTrigger);

        // Save the initial position and radius
        startPosition = player.transform.position;
        initialRadius = Vector3.Distance(startPosition, targetTransform.position);
        initialOffset = startPosition - targetTransform.position;
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        // No implementation needed for FixedUpdateState in this context
    }

    public void CacheVariables(float mA, float outA, float inA, float inCut, float drag)
    {
        maxAmpRadRatio = mA;
        outerAmp = outA;
        innerAmp = inA;
        innerCutoff = inCut;
        dragAmount = drag;
    }

    public void SetValues(float durationVar, float centerDurationVar, bool clockwise, Transform trans, Vector2 targetPos, bool doTween)
    {
        duration = durationVar;
        centerDuration = centerDurationVar;
        isClockwise = clockwise;
        targetTransform = trans;
        targetPosition = targetPos;
        doPlayerTween = doTween;
    }

    public override void RotateState(PlayerStateManager player)
    {
        if (doPlayerTween)
        {
            if (duration < 0)
            {
                player.rb.SetRotation(0);

            }
            else if (rotateTime < duration)
            {


                rotateTime += Time.fixedUnscaledDeltaTime;
                // Use SmoothStep to ease out the rotation near the end
                float easedTime = Mathf.SmoothStep(0, 1, time / duration);

                // Ensure always clockwise rotation
                float targetRotation = Mathf.LerpAngle(currentRotation, 360, easedTime);
                targetRotation = Mathf.Repeat(targetRotation, 360); // Normalize the angle to keep it within 0-360

                if (targetRotation < currentRotation)
                {
                    targetRotation = Mathf.LerpAngle(currentRotation, 360, easedTime); // Continue rotating clockwise


                }

                player.rb.MoveRotation(targetRotation);
            }
        }

        // No implementation needed for RotateState in this context
    }

    public override void ExitState(PlayerStateManager player)
    {
        player.rb.gravityScale = player.originalGravityScale;
        player.ChangeCollider(0);

    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (time < duration && duration > 0)
        {
            time += Time.unscaledDeltaTime;

            // Calculate the angle based on the elapsed time and direction
            float angle = (isClockwise ? -1 : 1) * (time / duration) * 2 * Mathf.PI;

            // Interpolate the radius to move the player closer to the center
            float radius = Mathf.Lerp(initialRadius, 0, time / duration);

            // Calculate the new position
            Vector3 newPosition = targetTransform.position + new Vector3(
                initialOffset.x * Mathf.Cos(angle) - initialOffset.y * Mathf.Sin(angle),
                initialOffset.x * Mathf.Sin(angle) + initialOffset.y * Mathf.Cos(angle),
                0
            ) * (radius / initialRadius);

            // Update the player's position
            player.transform.position = newPosition;
        }
        else
        {
            moveTime += Time.unscaledDeltaTime / centerDuration;
            if (doPlayerTween)
                newY = targetPosition.y + Mathf.Sin(Time.unscaledTime * upDownSpeed) * upDownAmplitude;
            if (!reachedTargetPosition)
            {


                // Lerp player to target position
                player.transform.position = Vector3.Lerp(player.transform.position, new Vector2(targetPosition.x, newY), moveTime);



                if (Vector3.Distance(player.transform.position, targetPosition) < 0.01f)
                {
                    reachedTargetPosition = true;
                    moveTime = 0; // Reset move time for up and down movement
                }
            }
            else if (doPlayerTween)
            {
                // Ensure up and down movement starts from the target position

                player.transform.position = new Vector3(targetPosition.x, newY, player.transform.position.z);
            }
        }
    }
}