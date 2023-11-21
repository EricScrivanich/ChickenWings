
using UnityEngine;

public class PlayerStartingState : PlayerBaseState
{
    
    private bool switchBool = false;
    public override void EnterState(PlayerStateManager player)
    {
        player.rb.velocity = new Vector2 (0,5);
        AudioManager.instance.PlayStartSound();
       
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
     
    }

    public override void OnCollisionEnter2D(PlayerStateManager player, Collision2D collision)
    {
      
    }

    public override void RotateState(PlayerStateManager player)
    {
       player.BaseRotationLogic();
    }

    public override void UpdateState(PlayerStateManager player)
    {
       
    }

   
}
