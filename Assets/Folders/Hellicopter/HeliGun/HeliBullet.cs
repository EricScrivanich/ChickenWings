using UnityEngine;
using System.Collections;

public class HeliBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    private void OnEnable()
    {
        // Start the bullet's movement and lifetime countdown
        StartCoroutine(MoveAndDeactivate());
    }

    IEnumerator MoveAndDeactivate()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lifetime)
        {
            // Move the bullet
            transform.Translate(Vector3.left * speed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Deactivate the bullet
        gameObject.SetActive(false);
    }
}
