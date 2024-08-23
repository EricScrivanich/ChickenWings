using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLaucnher : MonoBehaviour
{

    [SerializeField] private float force;
    [SerializeField] private float killChance;


    [SerializeField] private Vector2 forceAmountXRange;
    [SerializeField] private Vector2 forceAmountYRange;
    [SerializeField] private Color KillColor;
    [SerializeField] private Color NormalColor;
    [SerializeField] private Vector2 scaleRange;
    private Rigidbody2D rb;
    [SerializeField] private float baseMass;
    [SerializeField] private float massToScaleRatio;
    private SpriteRenderer sprite;

    private float center;
    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        center = (scaleRange.x + scaleRange.y) * .5f;
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame

    private void OnEnable()
    {
        float r = Random.Range(0f, 1f);

        if (r < killChance)
        {
            sprite.color = KillColor;
            gameObject.tag = "Floor";

        }
        else
        {
            sprite.color = NormalColor;
            gameObject.tag = "Untagged";

        }

        float s = transform.localScale.x;
        rb.mass = ((s - center) * massToScaleRatio) + baseMass;
        rb.velocity = new Vector2(Random.Range(forceAmountXRange.x, forceAmountXRange.y), Random.Range(forceAmountYRange.x, forceAmountYRange.y));

    }
}
