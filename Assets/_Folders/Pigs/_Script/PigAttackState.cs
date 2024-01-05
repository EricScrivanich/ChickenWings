using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PigAttackState : PigBaseState
{
    private bool flipped;
    // Reference to the player's transform

    public override void EnterState(PigStateManager pig)
    {
         // Assuming player has tag "Player"
    }

    public override void FixedUpdateState(PigStateManager pig)
    {
        // Move towards player on X-axis
        MoveTowardsPlayer(pig);

        // Adjust vertical position to match player
        MatchPlayerHeight(pig);
    }

    public override void UpdateState(PigStateManager pig)
    {
        // Add any Update logic if needed
    }

    private void MoveTowardsPlayer(PigStateManager pig)
    {
        float direction = Mathf.Sign(pig.playerTransform.position.x - pig.transform.position.x);
        pig.rb.AddForce(new Vector2(direction * pig.ID.moveForce, 0), ForceMode2D.Force);


        if (pig.transform.position.x < pig.playerTransform.position.x && !flipped)
        {
          

        }
    }

    private void MatchPlayerHeight(PigStateManager pig)
    {
        // if (Mathf.Abs(pig.transform.position.y - pig.playerTransform.position.y) > pig.ID.followThreshold)
        // {
        //     pig.rb.velocity = new Vector2(pig.rb.velocity.x, pig.ID.jumpForce);
        // }

        if (pig.transform.position.y < pig.playerTransform.position.y || pig.transform.position.y < pig.ID.jumpPoint)
        {
            pig.anim.SetTrigger("FlapTrigger");
            pig.rb.velocity = new Vector2(pig.rb.velocity.x, pig.ID.jumpForce);
        }
    }
}
