using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounceState : PlayerBaseState
{
    private bool bounce = false;
    private float bounceTime;
    private float bounceDuration = .5f;
    public override void EnterState(PlayerStateManager player)
    {
        bounceTime = 0;
        player.maxFallSpeed = player.ID.MaxFallSpeed;

        player.anim.SetTrigger("BounceTrigger");





    }
    public override void ExitState(PlayerStateManager player)

    {
        Debug.Log("BOujce");

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {

    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {

    // }

    public override void RotateState(PlayerStateManager player)
    {
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

    }

    public override void UpdateState(PlayerStateManager player)
    {
        bounceTime += Time.deltaTime;
        if (bounceTime > bounceDuration)
        {
            // player.ID.events.OnBounce.Invoke();
            AudioManager.instance.PlayBounceSound();
            player.rb.velocity = new Vector2(1, 9.5f);
            player.disableButtons = false;
            // player.ID.events.FloorCollsion.Invoke(true);
            player.isDropping = false;
            player.CheckIfIsTryingToParachute();

        }

    }
    private IEnumerator ApplyBounceAfterDelay()
    {
        yield return new WaitForSeconds(.1f);
        bounce = true;


    }
}
