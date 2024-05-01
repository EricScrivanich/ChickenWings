using UnityEngine;
using System.Collections;

public class HeliNormalState : HeliBaseState
{
    private Coroutine rotateShoot;
    public override void EnterState(HeliStateManager heli)
    {


        // heli.transform.position = heli.ID.normalPosition;

        heli.RotateAndShoot(true);
        if (heli.ID.Lives < 3)
        {
            float time = Random.Range(heli.ID.minSwitchTime, heli.ID.maxSwitchTime);
            heli.Invoke("TempSwitch", time);

        }
    }

    public override void OnTriggerEnter2D(HeliStateManager heli, Collider2D collider)
    {

    }

    public override void ExitState(HeliStateManager heli)
    {
        heli.RotateAndShoot(false);

    }

    public override void UpdateState(HeliStateManager heli)
    {
        float xSwayAmount = Mathf.Sin(Time.time * .5f) * heli.ID.xSwayAmount;
        float ySwayAmount = Mathf.Sin(Time.time * 2f) * heli.ID.ySwayAmount; // Offset phase for different Y movement

        Vector2 currentPos = heli.transform.position;
        Vector2 sway = new Vector3(xSwayAmount, ySwayAmount, 0);
        heli.transform.position = currentPos + sway;

        // Vector2 playerPosition = heli.player.transform.position;
        // Vector2 direction = playerPosition - new Vector2(heli.transform.position.x, heli.transform.position.y);
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;  // Subtracting 90 degrees to align with the sprite
        // heli.transform.rotation = Quaternion.Euler(0, 0, angle);

    }


}
