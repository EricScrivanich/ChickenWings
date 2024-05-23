using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollectable : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collider)
    {

        ICollectible collectibleEntity = collider.gameObject.GetComponent<ICollectible>();
        if (collectibleEntity != null)
        {
            collectibleEntity.Collected();
        }

    }
}
