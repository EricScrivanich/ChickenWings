using UnityEngine;

public class Bombs : MonoBehaviour
{
    public PlaneManagerID ID;
    private DropBomb dropBombController;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();

    }
    void Start()
    {
        // Find the DropBomb script in the scene
        dropBombController = FindObjectOfType<DropBomb>();
    }

    public void ResetBomb()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            ID.GetExplosion(transform.position, ID.droppedBombExplosionScale);

            AudioManager.instance.PlayBombExplosionSound();
            gameObject.SetActive(false);

            // gameObject.SetActive(false);
            // // Return the bomb to the pool
            // dropBombController.ReturnBombToPool(gameObject);
        }

        else if (collider.gameObject.tag == "Floor")
        {
            anim.SetTrigger("ExplodeTrigger");

            AudioManager.instance.PlayBombExplosionSound();

            // gameObject.SetActive(false);
            // // Return the bomb to the pool
            // dropBombController.ReturnBombToPool(gameObject);
        }
    }
}
