using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonScript : MonoBehaviour
{
    
    [SerializeField] private float speed;
    [SerializeField] private float moveDelay;

    // Start is called before the first frame update
    void Start()
    {
       
        StartCoroutine(MoveAfterDelay(moveDelay));
    }

    // Coroutine to delay the movement of the object
    IEnumerator MoveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        while (true)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            yield return null;
        }
    }
}
