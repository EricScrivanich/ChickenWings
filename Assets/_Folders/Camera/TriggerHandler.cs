using UnityEngine;

public class TriggerHandler : MonoBehaviour
{

    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider is the one you're interested in
        // You can use tags, names, or layers for filtering
        if (other.CompareTag("Player"))
        {
            // Call the function by name on every MonoBehaviour attached to this GameObject
            SendMessage("TriggerAction", SendMessageOptions.DontRequireReceiver);
        }

        gameObject.SetActive(false);
    }


}