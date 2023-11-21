using UnityEngine;
using System.Collections;

public class HeliNormalState : HeliBaseState
{
    public override void EnterState(HeliStateManager heli)
    {
    Debug.Log("Entered");
    heli.transform.position = heli.ID.normalPosition;
    float rotationSpeed = 30f;  // Speed of rotation
    float waitTime = heli.ID.waitToShootTime;  // Time between each rotation and shoot action
    float rotationDuration = 3f;  // Time to spend rotating
    heli.StartCoroutine(RotateAndShoot(heli, rotationSpeed, waitTime, rotationDuration));
    }

    public override void OnTriggerEnter2D(HeliStateManager heli, Collider2D collider)
    {
        
    }

    public override void UpdateState(HeliStateManager heli)
    {
        float xSwayAmount = Mathf.Sin(Time.time * .5f) * heli.ID.xSwayAmount;
        float ySwayAmount = Mathf.Sin(Time.time * 2f ) * heli.ID.ySwayAmount; // Offset phase for different Y movement

        Vector3 currentPos = heli.transform.position;
        Vector3 sway = new Vector3(xSwayAmount, ySwayAmount, 0);
        heli.transform.position = currentPos + sway;

        // Vector2 playerPosition = heli.player.transform.position;
        // Vector2 direction = playerPosition - new Vector2(heli.transform.position.x, heli.transform.position.y);
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;  // Subtracting 90 degrees to align with the sprite
        // heli.transform.rotation = Quaternion.Euler(0, 0, angle);
        
    }

    IEnumerator RotateAndShoot(HeliStateManager heli, float rotationSpeed, float waitTime, float rotationDuration)
{
    float elapsedTime = 0f;
    

    // Calculate the initial and target rotation
    Quaternion initialRotation = heli.transform.rotation;
    Vector2 direction = heli.player.transform.position - heli.transform.position;
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
    Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

     heli.ID.events.shoot?.Invoke(1.2f,5);


    // Rotate over time
    while (elapsedTime < rotationDuration)
    {
        elapsedTime += Time.deltaTime;
       
        float t = elapsedTime / rotationDuration;
        heli.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
        yield return null;
    }

    // Call your shoot function here
    // Shoot();

    yield return new WaitForSeconds(waitTime);

    // Restart the coroutine to repeat the behavior
    heli.StartCoroutine(RotateAndShoot(heli, rotationSpeed, waitTime, rotationDuration));
}
}
