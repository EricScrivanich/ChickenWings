using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    private bool canDash;

    private bool switchSlash;

    [SerializeField] private float dashPower = 10.4f;
    // [SerializeField] private float dashPower = 12.5f;

    private int rotationSpeed = 100;
    private float rotZ;
    private bool passedTime;
    private float dashingTime;
    private float dragDuration = .4f;
    private float dragAmount = 2.5f;
    private float dragTime;
    private float dashDurationMin = .35f;
    private float dashDurationMax = .52f;
    private float currentSpeed;
    private float currentGravity;
    private float slowdownSpeed = 16.2f;
    private float addGravitySpeed = 3.3f;
    private bool hasFinishedDash;


    public override void EnterState(PlayerStateManager player)
    {
        player.isDashing = true;
        hasFinishedDash = false;
        player.stillDashing = true;
        switchSlash = false;
        dragTime = 0;
        if (player.canDashSlash)
        {
            player.ID.globalEvents.CanDashSlash?.Invoke(true);
        }

        rotZ = 0;
        passedTime = false;
        player.ChangeCollider(1);
        // player.rb.velocity = Vector2.zero;
        // player.disableButtons = true;
        player.rb.gravityScale = 0;
        player.rb.MoveRotation(0);
        // player.anim.SetTrigger("DashTrigger");
        AudioManager.instance.PlayDashSound();
        dashingTime = 0;
        // player.rb.velocity = new Vector2(dashPower, 0);
        player.AdjustForce(dashPower, 0);
        currentSpeed = dashPower;

    }
    public override void ExitState(PlayerStateManager player)

    {
        if (player.canDashSlash)
        {
            player.ID.globalEvents.CanDashSlash?.Invoke(false);

        }

        if (!hasFinishedDash)
        {
            hasFinishedDash = true;
            player.anim.SetTrigger(player.FinishDashTrigger);

        }

        player.rb.gravityScale = player.originalGravityScale;
        player.stillDashing = false;
        player.rb.drag = 0f;
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
        if ((dashingTime > dashDurationMin && !player.isDashing || player.isDashing && dashingTime > dashDurationMax) && !passedTime)
        {


            player.rb.gravityScale = player.originalGravityScale;
            player.ID.globalEvents.CanDash?.Invoke(false);
            if (!hasFinishedDash)
            {
                hasFinishedDash = true;
                player.anim.SetTrigger(player.FinishDashTrigger);

            }

            passedTime = true;
            player.rb.drag = dragAmount;
            // player.rb.gravityScale = player.originalGravityScale;
            // player.rb.velocity = new Vector2(2, player.rb.velocity.y);


            // player.CheckIfIsTryingToParachute();
        }

        if (passedTime && player.stillDashing)
        {
            if (switchSlash)
            {
                player.SwitchState(player.DashSlash);
                if (!hasFinishedDash)
                {
                    hasFinishedDash = true;
                    player.anim.SetTrigger(player.FinishDashTrigger);

                }


                player.ID.globalEvents.SetCanDashSlash?.Invoke(false);
                return;
            }
            dragTime += Time.deltaTime;




            if (dragTime > dragDuration)
            {
                player.rb.drag = 0;
                player.stillDashing = false;
                if (player.canDashSlash)
                {
                    player.ID.globalEvents.CanDashSlash?.Invoke(false);


                }
            }

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
