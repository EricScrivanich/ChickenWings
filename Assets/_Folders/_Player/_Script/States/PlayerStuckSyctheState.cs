using UnityEngine;

public class PlayerStuckSyctheState : PlayerBaseState
{
    private Vector2 offset;
    private Vector2 lastPos;
    private int pigType;
    private float yCord;
    private float magnitude = 10.5f;
    private bool didSwipe;
    private float timer;
    private float maxStuckTime = 3.2f;
    private float recordPositionTimer;
    private float maxFloatTime = 3.5f;
    private float maxIdleTime = 2.5f;
    private float idlePowerUseSpeed = 15;
    private float glidePowerUseSpeed = 30;

    private Vector2 stickDirection;
    private float stickMagnitude;

    private float maxStickForce = 7.3f;
    private float minStickForce = 1f;
    // private float initialStickForce = 7f;
    private float maxMagnitude = 8.5f;
    private bool hitMaxTime;
    private Vector2 estimatedSpeed;
    private bool hasSwiped;

    private bool usingStick;

    public override void EnterState(PlayerStateManager player)
    {
        hitMaxTime = false;
        player.SetUseScythePowerSpeed(idlePowerUseSpeed,1);
        player.ActivateScythePower();
        hasSwiped = false;
        recordPositionTimer = 0;
        player.ID.scytheIsStuck = true;
        usingStick = false;
        player.ID.UiEvents.StuckPigScythe?.Invoke(true);
        stickMagnitude = 0;
        stickDirection = Vector2.zero;
        lastPos = player.transform.position;
        estimatedSpeed = Vector2.zero;
        timer = 0;
        didSwipe = false;
        player.AdjustForce(Vector2.zero);
        player.rb.angularVelocity = 0;
        player.rb.linearDamping = 0;
        player.rb.gravityScale = 0;
        player.rb.bodyType = RigidbodyType2D.Kinematic;
        offset = player.rb.position - (Vector2)player.stuckPig.transform.position;
        yCord = player.transform.position.y;



    }
    public void SetPigType(int t)
    {
        pigType = t;

    }

    public void Swipe(PlayerStateManager player, Vector2 dir)
    {
        stickDirection = dir.normalized;

        if (!usingStick)
        {
            // player.set

            usingStick = true;
            player.SetUseScythePowerSpeed(glidePowerUseSpeed,1.6f);



            player.rb.bodyType = RigidbodyType2D.Dynamic;
            didSwipe = true;
            player.rb.gravityScale = .3f;
            player.rb.linearDamping = .25f;
            player.stuckPig.Damage(1, 4, 0);
            // player.rb.linearVelocity = initialStickForce * dir.normalized;
            timer = 0;

        }
        stickMagnitude = Mathf.Lerp(minStickForce, maxStickForce, dir.magnitude);
        // stickDirection = dir.normalized;

        // player.ID.scytheIsStuck = false;


        // Debug.Log("Adding stuck swipe force of: " + dir * magnitude);
        // player.AdjustForce(dir * magnitude);
        // player.ID.UiEvents.StuckPigScythe?.Invoke(false);


    }
    public override void ExitState(PlayerStateManager player)
    {
        player.ID.UiEvents.StuckPigScythe?.Invoke(false);
        player.rb.linearDamping = 0;
        usingStick = false;
        player.ID.scytheIsStuck = false;
        // player.ResetDamagable();
        // player.SetTrailActive(false);



        player.rb.gravityScale = player.originalGravityScale;
        if (!didSwipe)
        {



            player.stuckPig.Damage(1, 4, 0);

            player.rb.bodyType = RigidbodyType2D.Dynamic;
            if (hitMaxTime)
                player.AdjustForce(estimatedSpeed * .6f);
            else
                player.AdjustForce(new Vector2((estimatedSpeed.x) * .75f, estimatedSpeed.y + 1));



        }


    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        if (usingStick && player.usingScythePower)
        {
            // player.rb.AddForce(stickDirection * stickMagnitude);

            player.AdjustForce(stickDirection * stickMagnitude);

            float angle = Mathf.Atan2(stickDirection.y, stickDirection.x) * Mathf.Rad2Deg;

            player.rb.MoveRotation(angle);

            // if (player.rb.linearVelocity.magnitude > maxMagnitude)
            // {
            //     player.rb.linearVelocity = stickDirection * maxMagnitude;
            // }

        }



    }


    public override void RotateState(PlayerStateManager player)
    {



    }
    public void ReleaseStick(PlayerStateManager player)
    {
        if (!hasSwiped)
        {
            player.SetUseScythePowerSpeed(idlePowerUseSpeed,1);
            timer = 0;
            hasSwiped = true;
            usingStick = false;

            player.rb.linearDamping = .3f;
        }


    }
    private bool countedPos;
    public override void UpdateState(PlayerStateManager player)
    {
       
        if (!didSwipe)
        {
            player.transform.position = (Vector2)player.stuckPig.transform.position + offset;
            timer += Time.deltaTime;
            recordPositionTimer += Time.deltaTime;
            if (timer >= maxStuckTime)
            {
                hitMaxTime = true;

                player.SwitchState(player.NullState);

            }
            else if (recordPositionTimer > .1f)
            {

                recordPositionTimer = 0;
                estimatedSpeed = ((Vector2)player.transform.position - lastPos) * 10f;
                Debug.Log("Estimated Speed is: " + estimatedSpeed);
                lastPos = player.transform.position;
            }

        }
        // else if (!hasSwiped)
        // {
        //     // timer += Time.deltaTime;

        //     if (timer >= maxFloatTime)
        //     {
        //         player.SetUseScythePowerSpeed(20);
        //         hasSwiped = true;
        //         player.ID.UiEvents.StuckPigScythe?.Invoke(false);
        //         timer = 0;
        //         player.rb.linearDamping = .4f;

        //         // player.SwitchState(player.NullState);
        //     }
        // }
        else if (hasSwiped && timer < maxIdleTime)
        {
            timer += Time.deltaTime;

            player.rb.gravityScale = Mathf.Lerp(.8f, player.originalGravityScale, timer / maxIdleTime);

            // if (timer >= maxIdleTime)
            // {
            //     player.SwitchState(player.NullState);
            // }

        }

        // if (pigType != 0)
        //     player.transform.position = (Vector2)player.stuckPig.transform.position + offset;
        // else
        //     player.transform.position = new Vector2(player.stuckPig.transform.position.x + offset.x, yCord);

    }


}
