using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollectable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<ICollectible>() == null)
        {
            Debug.Log("Game object has not collectable interface, object is: " + collider.gameObject + " transfrom: " + collider.gameObject.transform.position);
            return;
        }
        collider.gameObject.GetComponent<ICollectible>().Collected();

    }
}
