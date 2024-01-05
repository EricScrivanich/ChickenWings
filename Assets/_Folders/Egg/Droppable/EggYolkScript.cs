
using UnityEngine;



public class EggYolkScript : MonoBehaviour
{

    private float speed = 5;



    private void Update()
    {
        transform.Translate(Vector3.left * speed* Time.deltaTime);
        if (transform.position.x < BoundariesManager.leftBoundary)
        {
            gameObject.SetActive(false);
        }

    }

  
}
