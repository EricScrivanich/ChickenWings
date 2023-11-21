using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Transform playerTransform;
    private Vector2 leftPosition;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        leftPosition = new Vector2 (-8,3f);
         playerTransform = player.transform;
        
    }

    // Update is called once per frame
    void Update()
    {
       
        if (playerTransform.position.x > 2)
        {
            Vector2 pos = Vector2.MoveTowards(transform.position, leftPosition, 9 * Time.deltaTime);
transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0, 10), Time.deltaTime* 10);
            rb.MovePosition(pos);

        }

        if (rb.position == leftPosition)
        {
            transform.rotation = Quaternion.Euler(0,180,0);
           
        }
        
        
    }

    void Flip()
    {
        
    }
}
