using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    private bool canDash;
    private bool isDashing;


    private Vector2 dropVector;
    [SerializeField] private float dashPower = 10.5f;
    private float dashingTime;
    private float dashingDuration = .38f;
    [SerializeField] private float dashCoolDown = 1.5f;
    [SerializeField] private float dropCoolDown = 2f;
    private float bounceHeight = 10f;
    public override void EnterState(PlayerStateManager player)
    {
        player.disableButtons = true;
        player.anim.SetTrigger("DashTrigger");
        AudioManager.instance.PlayDashSound();
        dashingTime = 0;
        player.maxFallSpeed = 0;


    }

    public override void FixedUpdateState(PlayerStateManager player)
    {


        player.rb.velocity = new Vector2(dashPower, 0);


    }

    public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    {

    }

    public override void RotateState(PlayerStateManager player)
    {
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 900);

    }

    public override void UpdateState(PlayerStateManager player)
    {
        dashingTime += Time.deltaTime;
        if (dashingTime > dashingDuration)
        {
            player.disableButtons = false;
            player.rb.velocity = new Vector2(2, 0);
            player.maxFallSpeed = player.ID.MaxFallSpeed;
            
            player.CheckIfIsTryingToParachute();
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
