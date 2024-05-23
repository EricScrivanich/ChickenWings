using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlemishScript : MonoBehaviour
{
    [SerializeField] private PlaneManagerID ID;
    private SpriteRenderer sprite;
    private float time;
    private float duration = 3;
    // Start is called before the first frame update

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

      
            transform.Translate(Vector2.left * BoundariesManager.GroundSpeed * Time.deltaTime);


      




        // if (time < duration)
        // {
        //     float perc = time / duration;
        //     float alpha = Mathf.Lerp(0.9f, 0f, perc);
        //     sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
        // }






    }
    private void OnEnable()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, .9f);
    }

    private void OnDisable()
    {
        time = 0;

    }
}
