using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CannonManager : MonoBehaviour
{
    [SerializeField] private GameObject topCannon;
    [SerializeField] private GameObject leftCannon;
    [SerializeField] private GameObject bottomCannon;
    [SerializeField] private GameObject rightCannon;
    [SerializeField] private GameObject cannonBallPrefab;

    public float fireTime = 3f;
    public float speed = 5f;

    private enum Direction { Top, Left, Bottom, Right }

    private void Start()
    {
        // Start the firing coroutine when the game starts
        StartCoroutine(FireCannons());
    }

    private IEnumerator FireCannons()
    {
        while (true)
        {
            FireCannon(Direction.Top);
            yield return new WaitForSeconds(fireTime);
            FireCannon(Direction.Left);
            yield return new WaitForSeconds(fireTime);
            FireCannon(Direction.Bottom);
            yield return new WaitForSeconds(fireTime);
            FireCannon(Direction.Right);
            yield return new WaitForSeconds(fireTime);
        }
    }

    private void FireCannon(Direction direction)
    {
        GameObject cannon = null;
        Vector2 fireDirection = Vector2.zero;

        switch (direction)
        {
            case Direction.Top:
                cannon = topCannon;
                fireDirection = Vector2.up;
                break;
            case Direction.Left:
                cannon = leftCannon;
                fireDirection = Vector2.left;
                break;
            case Direction.Bottom:
                cannon = bottomCannon;
                fireDirection = Vector2.down;
                break;
            case Direction.Right:
                cannon = rightCannon;
                fireDirection = Vector2.right;
                break;
        }

        if (cannon != null)
        {
            var spawnPoint = cannon.transform.Find("CannonBallSpawn");
            if (spawnPoint != null)
            {
                GameObject newCannonBall = Instantiate(cannonBallPrefab, spawnPoint.position, Quaternion.identity);
                Rigidbody2D rb = newCannonBall.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.linearVelocity = fireDirection * speed;
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
}
