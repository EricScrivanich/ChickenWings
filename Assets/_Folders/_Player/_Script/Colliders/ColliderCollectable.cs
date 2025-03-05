using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollectable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<ICollectible>() == null)
        {

            return;
        }
        collider.gameObject.GetComponent<ICollectible>().Collected();

    }
}
