using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliDipState : HeliBaseState
{
    private bool onRightSide;
    private float dippingXSpeed;
    private float risingXSpeed;
    private float dippedXSpeed;
    private Vector2 dippingSpeed;
    private Vector2 risingSpeed;
    private float minimumDippedTime = .3f;
    private float maxX;

    private float time;
    private bool moveUp;
    public override void EnterState(HeliStateManager heli)
    {
        time = 0;
        moveUp = false;
        if (heli.transform.position.x > .5f)
        {
            onRightSide = true;
            maxX = -6;



            heli.transform.rotation = Quaternion.Euler(0, 0, 5);

            heli.targetRight = false;
            dippedXSpeed = -heli.ID.dippedXSpeed;
            dippingXSpeed = -heli.ID.dippingXSpeed;
            risingXSpeed = -heli.ID.risingXSpeed;
        }
        else
        {
            heli.transform.rotation = Quaternion.Euler(0, 0, -5);

            heli.targetRight = true;
            maxX = 6;


            onRightSide = false;
            dippedXSpeed = heli.ID.dippedXSpeed;
            dippingXSpeed = heli.ID.dippingXSpeed;
            risingXSpeed = heli.ID.risingXSpeed;

        }
        dippingSpeed = new Vector2(dippingXSpeed, -heli.ID.dippingYSpeed);
        risingSpeed = new Vector2(risingXSpeed, heli.ID.risingYSpeed);

    }

    public override void ExitState(HeliStateManager heli)
    {

    }

    public override void OnTriggerEnter2D(HeliStateManager heli, Collider2D collider)
    {

    }

    public override void UpdateState(HeliStateManager heli)
    {
        if (heli.transform.position.y > heli.ID.dipYTarget && !moveUp)
        {
            heli.transform.Translate(dippingSpeed * Time.deltaTime);
        }
        else if (time < minimumDippedTime)
        {
            time += Time.deltaTime;
            heli.transform.Translate(Vector2.right * dippedXSpeed * Time.deltaTime);
        }
        else if (!moveUp)
        {
            heli.transform.Translate(Vector2.right * dippedXSpeed * Time.deltaTime);

            if (Mathf.Abs(heli.transform.position.x - heli.player.position.x) < 1.2f || Mathf.Abs(heli.transform.position.x - maxX) < .5f)
            {
                moveUp = true;
            }

        }

        else if (heli.transform.position.y < heli.ID.riseYTarget)
        {
            heli.transform.Translate(risingSpeed * Time.deltaTime);

        }
        else
        {
            heli.SwitchState(heli.FlipState);
        }

    }

    // Start is called before the first frame update

}
