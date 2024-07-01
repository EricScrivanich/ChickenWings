using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollectable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {

        ICollectible collectibleEntity = collider.gameObject.GetComponent<ICollectible>();
        if (collectibleEntity != null)
        {
            collectibleEntity.Collected();
        }

    }
}
