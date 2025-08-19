using UnityEngine;

public class MoveOnGround : SpawnedQueuedObject
{
    [SerializeField] private float addedBoundaryCheck;
    private float checkX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
  
    void Update()
    {
        transform.Translate(Vector2.left * BoundariesManager.GroundSpeed * Time.deltaTime);
        if (transform.position.x < checkX)
        {
            gameObject.SetActive(false);
        }
    }
    void OnEnable()
    {
        checkX = BoundariesManager.leftBoundary - addedBoundaryCheck;
    }
    void OnDisable()
    {
        base.ReturnToPool();
    }
}
