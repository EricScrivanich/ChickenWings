
using UnityEngine;

public class BallMaterialMovement : MonoBehaviour
{
    
    public float speed = 50f;
    public Vector2 startPosition;
    public GameObject targetRing;

    public void StartMoving()
    {
        gameObject.SetActive(true);
    }

    public void SetParameters(GameObject targetObject)
    {
        
        targetRing = targetObject;
    }

    private void Update()
    {
        if (targetRing != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetRing.transform.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == targetRing)
        {
            // Here, change the ring's material or trigger any effect
            gameObject.SetActive(false); // Deactivate the ball
            // RingSpawner.Instance.SetSpecialRingEffect();
        }
    }
  
    }

