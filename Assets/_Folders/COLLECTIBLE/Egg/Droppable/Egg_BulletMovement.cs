using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg_BulletMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 startScale = new Vector2(.5f, .5f);
    private Vector2 endScale = new Vector2(1.2f, 1f);
    [SerializeField] private float force;
    // Start is called before the first frame update

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();


    }
    void Start()
    {
        // rb.AddForce(transform.up * force, ForceMode2D.Impulse);
      


    }
    void OnEnable()
    {
        StartCoroutine(ScaleOverTime(.3f));



    }
    private void FixedUpdate()
    {
        rb.linearVelocity = (transform.up * force);

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator ScaleOverTime(float duration)
    {
        float currentTime = 0f;
        transform.localScale = startScale;
        while (currentTime < duration)
        {
            float t = currentTime / duration; // Normalized time
            transform.localScale = Vector2.Lerp(startScale, endScale, t);
            currentTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.localScale = endScale; // Ensure it ends at the target scale
    }
}
