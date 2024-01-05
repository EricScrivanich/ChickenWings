using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Cannon : MonoBehaviour
{
    [SerializeField] private GameObject cannonBallPrefab;
    public float speed = 5f;

    public void FireCannonBall()
    {
        var spawnPoint = transform.Find("CannonBallSpawn");
        if (spawnPoint != null)
        {
            GameObject newCannonBall = Instantiate(cannonBallPrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = newCannonBall.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 fireDirection = transform.up; // Use the cannon's upward direction
                rb.velocity = fireDirection * speed;
            }
            else
            {
                Debug.LogError("Cannon ball prefab does not have a Rigidbody2D component.");
            }
        }
        else
        {
            Debug.LogError("Cannon does not have a child object named 'CannonBallSpawn'.");
        }
    }
}

