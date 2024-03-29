using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    private bool canDash;
    private bool isDashing;



    [SerializeField] private float dashPower = 10.5f;
    private int rotationSpeed = 100;
    private float rotZ;
    private bool passedTime;
    private float dashingTime;
    private float dashingDuration = .36f;
    private float currentSpeed;
    private float currentGravity;
    private float slowdownSpeed = 16.2f;
    private float addGravitySpeed = 3.3f;


    public override void EnterState(PlayerStateManager player)
    {
        rotZ = 0;
        passedTime = false;
        player.ChangeCollider(1);
        player.rb.velocity = Vector2.zero;
        // player.disableButtons = true;
        player.rb.gravityScale = 0;
        player.rb.MoveRotation(0);
        player.anim.SetTrigger("DashTrigger");
        AudioManager.instance.PlayDashSound();
        dashingTime = 0;
        player.rb.velocity = new Vector2(dashPower, 0);
        currentSpeed = dashPower;




    }
    public override void ExitState(PlayerStateManager player)

    {
        player.rb.gravityScale = player.originalGravityScale;

        player.ChangeCollider(0);

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {





    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {

    // }

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

    public override void UpdateState(PlayerStateManager player)
    {
        dashingTime += Time.deltaTime;
        if (dashingTime > dashingDuration && !passedTime)
        {
            player.disableButtons = false;
            player.rb.gravityScale = player.originalGravityScale;

            passedTime = true;
            // player.rb.gravityScale = player.originalGravityScale;
            // player.rb.velocity = new Vector2(2, player.rb.velocity.y);


            // player.CheckIfIsTryingToParachute();
        }

        if (passedTime)
        {
            if (player.rb.velocity.x > 0)
            {

                currentSpeed -= slowdownSpeed * Time.deltaTime;
            }
            // if (player.rb.gravityScale < player.originalGravityScale)
            // {
            //     currentGravity += addGravitySpeed * Time.deltaTime;

            // }
            player.rb.velocity = new Vector2(currentSpeed, player.rb.velocity.y);
            // player.rb.gravityScale = currentGravity;
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
