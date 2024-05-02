using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Cannon : MonoBehaviour
{
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private Transform spawnPoint;
    private GameObject cannonBall;
    private Rigidbody2D ballRB;

    public float speed = 5f;

    private void Start()
    {
        cannonBall = Instantiate(cannonBallPrefab);
        ballRB = cannonBall.GetComponent<Rigidbody2D>();

        cannonBall.SetActive(false);
    }

    public void FireCannonBall()
    {
        cannonBall.SetActive(false);

        if (spawnPoint != null)
        {

            cannonBall.transform.position = spawnPoint.position;
            cannonBall.SetActive(true);
            Vector2 fireDirection = transform.up; // Use the cannon's upward direction
            ballRB.velocity = fireDirection * speed;


        }
        else
        {
            Debug.LogError("Cannon does not have a child object named 'CannonBallSpawn'.");
        }
    }

    private void OnDisable()
    {
        if (cannonBall != null)
        {
            cannonBall.SetActive(false);
        }
    }
}

