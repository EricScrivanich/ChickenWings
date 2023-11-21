
using UnityEngine;
using System;


public class RingMovement : MonoBehaviour
{
   public float speed;
   public int order;
   public static event Action<int> OnRingPassed; // Declare the event
  
    // Start is called before the first frame update


    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
  

    private void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.CompareTag("Player"))
        {
            OnRingPassed?.Invoke(order); // Trigger the event
        }
        else if (collision.CompareTag("ResetTag"))
        {
            // Return the ring to the pool
            RingSpawner.Instance.ReturnRingToPool(gameObject);
        }
    }
}
