using UnityEngine;

public class Bombs : MonoBehaviour
{
    private DropBomb dropBombController;

    void Start()
    {
        // Find the DropBomb script in the scene
        dropBombController = FindObjectOfType<DropBomb>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Floor")
        {
            gameObject.SetActive(false);
            // Return the bomb to the pool
            dropBombController.ReturnBombToPool(gameObject);
        }
    }
}
