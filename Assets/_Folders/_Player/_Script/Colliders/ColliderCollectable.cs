using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollectable : MonoBehaviour
{
    private int times = 0;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collider)
    {
        times++;
        Debug.Log("HSHHDH");
        Debug.Log(times + ": " + collider.gameObject);
        ICollectible collectibleEntity = collider.gameObject.GetComponent<ICollectible>();
        if (collectibleEntity != null)
        {
            collectibleEntity.Collected();
        }

    }
}
