using UnityEngine;
using System;

public class HeliFlipState : HeliBaseState
{
    private Vector2 targetPos;
    private bool flipDirectionToFlipped;
    private bool hitFlipThreshold;


    public override void EnterState(HeliStateManager heli)
    {
        hitFlipThreshold = false;
        heli.finishedFlipping = false;
        if (heli.targetRight)
        {
            heli.transform.rotation = Quaternion.Euler(0, 0, 0);
            targetPos = heli.ID.normalPosition;
            flipDirectionToFlipped = false;
        }
        else
        {
            heli.transform.rotation = Quaternion.Euler(0, 0, 20);

            targetPos = heli.ID.leftPosition;
            flipDirectionToFlipped = true;

        }

    }

    public override void ExitState(HeliStateManager heli)
    {

    }

    public override void OnTriggerEnter2D(HeliStateManager heli, Collider2D collider)
    {

    }

    public override void UpdateState(HeliStateManager heli)
    {
        heli.transform.position = Vector2.MoveTowards(heli.transform.position, targetPos, heli.ID.normalSpeed * Time.deltaTime);

        // Check if the helicopter is close enough to consider it has reached the target
        if (Vector2.Distance(heli.transform.position, targetPos) < 0.8f && !hitFlipThreshold)
        {
            heli.FlipRotation(flipDirectionToFlipped);
            hitFlipThreshold = true;
            // Trigger any event or state change as necessary
            // Example: heli.ChangeState(heli.SomeOtherState);
        }
        if (Vector2.Distance(heli.transform.position, targetPos) < 0.2f && heli.finishedFlipping)
        {
            heli.SwitchState(heli.NormalState);
            // Trigger any event or state change as necessary
            // Example: heli.ChangeState(heli.SomeOtherState);
        }




    }
}