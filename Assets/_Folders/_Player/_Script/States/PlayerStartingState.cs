
using UnityEngine;

public class PlayerStartingState : PlayerBaseState
{
    
    private bool switchBool = false;
    private float time;
    public override void EnterState(PlayerStateManager player)
    {
        player.rb.gravityScale = .8f;
        // player.rb.velocity = new Vector2(0, 5);
        player.AdjustForce(0, 5);

        time = 0;
        AudioManager.instance.PlayStartSound(); 
       
    }
    public override void ExitState(PlayerStateManager player)
    {
        player.rb.gravityScale = player.originalGravityScale;

    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
     
    }

    // public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    // {
      
    // }

    public override void RotateState(PlayerStateManager player)
    {
       player.BaseRotationLogic();
    }

    public override void UpdateState(PlayerStateManager player)
    {
        
        time += Time.deltaTime;
        if (time > .5f)
        {
            player.rb.gravityScale = player.originalGravityScale;
            
        }
        else 
        {
            
        }

    }

   
}
