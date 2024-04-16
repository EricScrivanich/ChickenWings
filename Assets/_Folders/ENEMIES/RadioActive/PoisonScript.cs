using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonScript : MonoBehaviour
{
    
   
    private bool ready;

    [SerializeField] private float speed;
    [SerializeField] private float moveDelay;
    [SerializeField] private Vector2 startScale;
    [SerializeField] private Vector2 endScale;
    [SerializeField] private float startOpacity;
    [SerializeField] private float endOpacity;
    [SerializeField] private float lerpDuration;
    private SpriteRenderer spriteRenderer;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        ready = false;
        StartCoroutine(LerpScaleAndOpacity());
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;



    }

    private void Update() {

        
        transform.Translate(Vector2.up * speed * Time.deltaTime);


        
    }

    private IEnumerator LerpScaleAndOpacity()
    {
        float timeElapsed = 0;

       
        while (timeElapsed < lerpDuration)
        {
            transform.localScale = Vector2.Lerp(startScale, endScale, timeElapsed / lerpDuration);
            if (spriteRenderer != null)
            {
                
                color.a = Mathf.Lerp(startOpacity, endOpacity, timeElapsed / lerpDuration);
                spriteRenderer.color = color;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = endOpacity;
            spriteRenderer.color = color;
        }
        
    }

   

    // Coroutine to delay the movement of the object
   
}
