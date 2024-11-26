using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounceState : PlayerBaseState
{
    private bool bounce = false;
    private Vector2 position;

    private float bounceTime;
    private float bounceDuration = .5f;
    private float afterBounceDuration = .55f;
    private Vector2 afterBounceForce = new Vector2(1, 9.5f);
    private bool hasBounced = false;
    private bool hasEnabledColliders;
    public override void EnterState(PlayerStateManager player)
    {
        player.rb.gravityScale = 0;
        player.anim.SetBool(player.BounceBool, true);

        position = player.transform.position;
        player.rb.linearVelocity = Vector2.zero;
        hasEnabledColliders = false;
        hasBounced = false;
        player.ChangeCollider(-1);
        bounceTime = 0;

    }
    public override void ExitState(PlayerStateManager player)

    {
        player.ChangeCollider(0);
        player.rb.gravityScale = player.originalGravityScale;
        player.maxFallSpeed = player.originalMaxFallSpeed;
    }
    public override void FixedUpdateState(PlayerStateManager player)
    {
        if (!hasBounced && !player.ID.constantPlayerForceBool)
        {
            player.rb.linearVelocity = Vector2.left * (BoundariesManager.GroundSpeed - .2f);
        }
        if (!player.isDropping && !hasEnabledColliders)
        {
            player.ChangeCollider(0);
            hasEnabledColliders = true;


        }

    }

    public override void RotateState(PlayerStateManager player)
    {
        if (hasBounced)
        {
            player.BaseRotationLogic();
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        bounceTime += Time.deltaTime;
        if (bounceTime > afterBounceDuration && hasBounced)
        {
            player.isDropping = false;
        }
        else if (bounceTime > bounceDuration && !hasBounced)
        {
            hasBounced = true;
            player.Dust.Play();

            player.rb.gravityScale = player.originalGravityScale;
            // player.ID.events.OnBounce.Invoke();
            AudioManager.instance.PlayBounceSound();

            // player.rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            // player.rb.gravityScale = player.originalGravityScale;

            player.AdjustForce(afterBounceForce);
            player.ID.events.EnableButtons?.Invoke(true);


            player.maxFallSpeed = -3f;
            // player.ID.events.FloorCollsion.Invoke(true);
        }



    }

}
