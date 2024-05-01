using UnityEngine;
using System.Collections;

public class HeliBullet : MonoBehaviour
{
    public float speed = 10f;

    private float speedVar;

    public float lifetime = 3f;
    [SerializeField] private HelicopterID heli;

    private void OnEnable()
    {
        if (heli.isFlipped)
        {
            speedVar = -speed;
        }
        else
        {
            speedVar = speed;
        }

        // Start the bullet's movement and lifetime countdown
        StartCoroutine(MoveAndDeactivate());
    }

    IEnumerator MoveAndDeactivate()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lifetime)
        {
            // Move the bullet
            transform.Translate(Vector2.left * speedVar * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Deactivate the bullet
        gameObject.SetActive(false);
    }
}
