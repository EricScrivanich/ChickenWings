using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{


    private bool switchSlash;

    private int rotationSpeed = 100;
    private float rotZ;
    private bool passedTime;
    private float dashingTime;
    private float ableToDashSlashDuration = .6f;
    // private readonly Vector2 dashForce = new Vector2(11f, 0);
    // private float dragAmount = 2.5f;
    // private float dragDuration = .4f;
    // private float dashDurationMin = .35f;
    // private float dashDurationMax = .69f;

    private Vector2 dashForce = new Vector2(11.6f, 0);
    private float dragAmount = 2.2f;
    private float initialDragAmount = .35f;
    private float dashDurationMin = .32f;
    private float dashDurationMax = .53f;
    private float dragDuration = .35f;

    private float dragTime;



    private bool hasFinishedDash;
    public void CacheVariables(float dashPower, float dashDurationMinInput, float dashDurationMaxInput)
    {
        dashForce = new Vector2(dashPower, 0);
        // dashForce = new Vector2(dashPower, 0);
        dashDurationMin = dashDurationMinInput;
        dashDurationMax = dashDurationMaxInput;

    }


    public override void EnterState(PlayerStateManager player)
    {

        player.rb.angularVelocity = 0;
        player.SetHingeTargetAngle(270);

        player.isDashing = true;
        // player.rb.freezeRotation = true;

        hasFinishedDash = false;
        player.stillDashing = true;
        switchSlash = false;
        dragTime = 0;
        // if (player.canDashSlash)
        // {
        //     player.ID.globalEvents.CanDashSlash?.Invoke(true);
        // }
        player.rb.linearDamping = .35f;

        rotZ = 0;
        passedTime = false;
        player.ChangeCollider(1);
        player.AdjustForce(dashForce);
        // player.rb.velocity = Vector2.zero;
        // player.disableButtons = true;
        player.rb.gravityScale = 0;
        player.rb.MoveRotation(0);
        // player.anim.SetTrigger("DashTrigger");
        AudioManager.instance.PlayDashSound();
        dashingTime = 0;
        // player.rb.velocity = new Vector2(dashPower, 0);



    }
    public override void ExitState(PlayerStateManager player)
    {

        if (player.isDashing)
            player.ID.events.OnDash?.Invoke(false);

        player.stillDashing = false;
        player.ID.globalEvents.CanDashSlash?.Invoke(false);

        if (!hasFinishedDash)
        {
            hasFinishedDash = true;
            player.DoDashCooldown();
            player.ID.UiEvents.OnDashUI?.Invoke(false);

            player.anim.SetTrigger(player.FinishDashTrigger);
            // player.rb.freezeRotation = false;

        }
        player.rb.gravityScale = player.originalGravityScale;
        player.rb.linearDamping = 0f;
        player.ChangeCollider(0);

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {

    }

    private void FinishDash()
    {


    }


    public override void RotateState(PlayerStateManager player)
    {
        if (passedTime && rotZ > -85)
        {
            rotZ -= rotationSpeed * Time.fixedDeltaTime;
            float newRotation = Mathf.LerpAngle(player.rb.rotation, rotZ, Time.deltaTime * rotationSpeed);

            // Apply the new rotation
            player.rb.MoveRotation(newRotation);

        }


    }

    public void SwitchSlash()
    {
        switchSlash = true;


    }


    public override void UpdateState(PlayerStateManager player)
    {

        dashingTime += Time.deltaTime;
        if (!passedTime && (dashingTime > dashDurationMin && !player.isDashing || player.isDashing && dashingTime > dashDurationMax))
        {


            player.rb.gravityScale = player.originalGravityScale;
            // player.ID.globalEvents.CanDash?.Invoke(false);
            if (!hasFinishedDash)
            {
                hasFinishedDash = true;
                player.DoDashCooldown();
                player.anim.SetTrigger(player.FinishDashTrigger);

                player.ID.UiEvents.OnDashUI?.Invoke(false);

                if (player.isDashing)
                    player.ID.events.OnDash?.Invoke(false);

            }
            // player.rb.freezeRotation = false;

            passedTime = true;


            player.rb.linearDamping = dragAmount;
            // player.rb.gravityScale = player.originalGravityScale;
            // player.rb.velocity = new Vector2(2, player.rb.velocity.y);


            // player.CheckIfIsTryingToParachute();
        }



        if (passedTime && player.stillDashing)
        {
            if (switchSlash)
            {

                if (!hasFinishedDash)
                {
                    hasFinishedDash = true;
                    player.anim.SetTrigger(player.FinishDashTrigger);

                }
                player.SwitchState(player.DashSlash);



                return;
            }
            dragTime += Time.deltaTime;




            if (dragTime > dragDuration)
            {
                player.rb.linearDamping = 0;

            }

            // if (dragTime > ableToDashSlashDuration)
            // {
            //     player.stillDashing = false;
            //     if (player.canDashSlash)
            //     {
            //         player.ID.globalEvents.CanDashSlash?.Invoke(false);


            //     }
            // }

        }

    }
}
// private IEnumerator Dash()
// {

//     isDashing = true;
//     // transform.rotation = Quaternion.Euler(0, 0, 0);
//     DisableButtons = true;



//     AudioManager.instance.PlayDashSound();
//     yield return new WaitForSeconds(dashingTime);
//     canDash = false;
//     rb.gravityScale = originalGravityScale;
//     DisableButtons = false;
//     rb.velocity = new Vector2 (2f, 0);
//     isDashing = false;
//     yield return new WaitForSeconds(dashCoolDown);
//     canDash = true;
// }
