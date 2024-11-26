using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigIdleState : PigBaseState
{

    private float time;
    private bool isLow = true;
    private bool justJumped = false;
    // Start is called before the first frame update

    public override void EnterState(PigStateManager pig)
    {

    }

    public override void FixedUpdateState(PigStateManager pig)
    {

    }


    public override void UpdateState(PigStateManager pig)
    {

        if (pig.transform.position.y < pig.ID.jumpPoint && !justJumped)
        {
            if (isLow)
            {
                pig.anim.SetTrigger("FlapTrigger");
                isLow = false;

            }

            time += Time.deltaTime;

            if (time >= pig.ID.waitTime)
            {
                pig.rb.linearVelocity = new Vector2(0, pig.ID.jumpForce);
                justJumped = true;
                time = 0;

            }


        }
        else if (justJumped && pig.transform.position.y > pig.ID.jumpPoint)
        {
            justJumped = false;
            isLow = true;

        }
    }
}
