using UnityEngine;

public class MoveOnGround : SpawnedQueuedObject
{
    [SerializeField] private float addedBoundaryCheck;
    [SerializeField] private bool destroyOnFinish;
    private float checkX;
    [SerializeField] private GameObject specialCaseObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame

    void Update()
    {
        transform.Translate(Vector2.left * BoundariesManager.GroundSpeed * Time.deltaTime);
        if (transform.position.x < checkX)
        {
            if (destroyOnFinish) Destroy(gameObject);
            else gameObject.SetActive(false);
        }
    }
    void OnEnable()
    {
        checkX = BoundariesManager.leftBoundary - addedBoundaryCheck;

        if (specialCaseObject != null)
        {
            specialCaseObject.SetActive(true);
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    void OnDisable()
    {
        base.ReturnToPool();
    }
}
