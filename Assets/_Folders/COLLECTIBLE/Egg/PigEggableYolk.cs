using UnityEngine;
using DG.Tweening;

public class PigEggableYolk : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void Initialize()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        float targetY = BoundariesManager.GroundPosition + .25f;
        transform.DOMoveY(targetY, 0.3f).SetEase(Ease.OutSine);
        transform.DOScale(Vector3.Scale(transform.localScale, new Vector3(1.25f, .75f, 1.2f)), 0.3f).SetEase(Ease.OutSine);
        transform.DORotate(new Vector3(0, 0,0), 0.3f).SetEase(Ease.OutSine);
    }


    // Update is called once per frame
    void Update()
    {
        // move to left regardless of rotation
        transform.position = new Vector2(transform.position.x - (BoundariesManager.GroundSpeed * Time.deltaTime), BoundariesManager.GroundPosition + .25f);

       
        if (transform.position.x < BoundariesManager.leftBoundary)
        {
            Destroy(gameObject);
        }

    }
}
