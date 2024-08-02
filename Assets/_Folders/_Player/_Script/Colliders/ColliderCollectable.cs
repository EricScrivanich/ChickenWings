using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollectable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        collider.gameObject.GetComponent<ICollectible>().Collected();

    }
}
