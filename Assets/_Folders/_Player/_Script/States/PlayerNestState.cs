using UnityEngine;

public class PlayerNestState : PlayerBaseState
{
    private bool holdingG;
    private PlayerStateManager playerCache;
    private bool justHit;
    private bool onWater;
    private float justHitTimer;
    private float justHitDelay = .1f;

    private float addedX;
    public override void EnterState(PlayerStateManager player)
    {
        onWater = false;
        player.AdjustForce(new Vector2(0, 5));
        player.rb.gravityScale = .5f;
        playerCache = player;



    }
    public override void ExitState(PlayerStateManager player)
    {

    }

    public void OnHitWater(bool hit)
    {
        // addedX = playerCache.rb.linearVelocity.y;
        justHit = true;
        onWater = hit;

    }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (other.gameObject.CompareTag("Manager"))
    //     {
    //         onWater = true;

    //     }
    // }
    // private void OnCollisionExit2D(Collision2D other)
    // {
    //     if (other.gameObject.CompareTag("Manager"))
    //     {
    //         onWater = false;

    //     }
    // }
    public override void UpdateState(PlayerStateManager player)
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            holdingG = true;
        }
        else if (Input.GetKeyUp(KeyCode.G) && holdingG)
        {
            holdingG = true;
        }

    }
    public override void FixedUpdateState(PlayerStateManager player)
    {

        if (justHit)
        {
            justHitTimer += Time.fixedDeltaTime;
            float final = 0;

            if (justHitTimer >= justHitDelay)
            {
                if (player.rb.linearVelocity.y < 0)
                {
                    final = player.rb.linearVelocity.y;
                }
                player.AdjustForce(new Vector2(-final * 1.5f, 0));
                justHit = false;
                justHitTimer = 0;
            }

        }
        if (holdingG)
        {
            player.rb.AddForce(new Vector2(0, -6));


        }
        if (onWater && playerCache.rb.linearVelocityY < 0)
        {
            Debug.Log("Floating");
            playerCache.rb.AddForce(new Vector2(7, 0));
        }


    }


    public override void RotateState(PlayerStateManager player)
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
