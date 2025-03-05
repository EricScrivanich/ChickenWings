using UnityEngine;

public class PlayerParryState : PlayerBaseState
{
    private bool hasParried;
    private float parryTime = .2f;
    private float perfectParryTime = .06f;

    private float parryTimer;
    public bool canParry { get; private set; } = false;
    public bool canPerfectParry { get; private set; } = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void EnterState(PlayerStateManager player)
    {
        parryTimer = 0;
        canParry = true;
        player.SetParryType(true, true);
        canPerfectParry = true;

        // player.ID.events.EnableButtons(false);
        hasParried = false;
        player.rb.gravityScale = 0;

        player.rb.angularVelocity = 0;
        player.rb.SetRotation(0);

        // player.rb.linearVelocity *= .4f;
        // player.rb.linearDamping = .35f;
        player.AdjustForce(Vector2.zero);

        // player.DoParry(true);
        player.SetParry(true);
        player.anim.SetTrigger("Parry");
        AudioManager.instance.PlayParryNoise(false);


    }

    public override void ExitState(PlayerStateManager player)
    {
        // player.ID.events.EnableButtons(true);

        player.rb.gravityScale = player.originalGravityScale;
        player.rb.linearDamping = 0;
        if (player.ID.trackParrySwipe)
        {
            player.ID.trackParrySwipe = false;
            player.StopSuccesfulParry();
        }

        // player.ID.events.OnTrackParrySwipe?.Invoke(false);


    }

    public void Parried(PlayerStateManager player)
    {
        player.ID.trackParrySwipe = true;
        hasParried = true;
        player.rb.linearVelocity = Vector2.zero;
        player.rb.gravityScale = 0;



    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        parryTimer += Time.fixedDeltaTime;
        if (!hasParried)
        {

            if (player.CanPerectParry && parryTimer >= perfectParryTime)
                player.SetParryType(false, true);
            else if (parryTimer >= parryTime)
            {
                player.SetParryType(false, false);
                parryTimer = 0;
                hasParried = true;
            }


        }
        if (hasParried && parryTimer < .3f)
        {
            player.rb.gravityScale = Mathf.Lerp(.3f, player.originalGravityScale, parryTimer / .3f);
        }
        // else
        // player.SwitchState


    }

    public override void RotateState(PlayerStateManager player)
    {

    }

    public override void UpdateState(PlayerStateManager player)
    {

    }
}
