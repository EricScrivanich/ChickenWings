using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainTrigger : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("yer");
        // Check if the object's x position is less than the trigger's x position
        if (other.transform.position.x < transform.position.x)
        {
            // The object exited on the left side of the trigger
            other.gameObject.SetActive(false);
        }
    }
}
