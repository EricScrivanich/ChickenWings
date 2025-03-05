using UnityEngine;

public class PlayerScytheState : PlayerBaseState
{

    // private float rotationTarget;
    private float timer;
    private float floatingTime = .26f;
    private float returnTime = .25f;

    private bool hitRotTarget;
    private float resetValuesTime;
    private bool hitMaxTime;
    private float rotationTime = .07f;
    private float startRot;

    private int rotDirection;

    public override void EnterState(PlayerStateManager player)
    {
        if (player.usingScythePower) player.UseScythePower(14);
        // player.anim.SetTrigger("IdleTrigger");
        player.rb.angularDamping = .65f;
        player.rb.angularVelocity = 720 * rotDirection;
        startRot = player.rb.rotation;
        hitMaxTime = false;
        timer = 0;
        hitRotTarget = false;
        // player.StartCoroutine(player.SwitchToIdleTimer(.32f));


        player.AdjustForce(player.rb.linearVelocity * .25f);
        player.rb.linearDamping = .25f;
        player.rb.gravityScale = .25f;



    }

    public void SetTargets(int d)
    {

        rotDirection = d;
    }
    public override void ExitState(PlayerStateManager player)
    {

        player.rb.gravityScale = player.originalGravityScale;
        player.rb.linearDamping = 0f;



    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        timer += Time.fixedDeltaTime;

        if (!hitMaxTime && timer > floatingTime)
        {

            hitMaxTime = true;
            timer = 0;
        }
        else if (hitMaxTime && timer < returnTime)
        {
            player.rb.linearDamping = Mathf.Lerp(.25f, 0, timer / returnTime);
            player.rb.gravityScale = Mathf.Lerp(.25f, player.originalGravityScale, timer / returnTime);
        }



    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {

    // }

    public override void RotateState(PlayerStateManager player)
    {
        // if (!hitRotTarget)
        // {
        //     float r = Mathf.LerpAngle(startRot, rotationTarget, timer / rotationTime);
        //     player.rb.MoveRotation(r);

        //     if (timer >= rotationTime)
        //     {
        //         hitRotTarget = true;
        //         player.rb.angularVelocity = 650 * rotDirection;
        //     }
        // }



    }

    public override void UpdateState(PlayerStateManager player)
    {


    }
}
