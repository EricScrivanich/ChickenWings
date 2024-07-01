using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownTIme : MonoBehaviour
{
    private IEnumerator SmoothTimeScaleTransition(float targetTimeScale, float duration)
    {
        float start = Time.timeScale;
        float elapsed = 0f;

       


        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(start, targetTimeScale, elapsed / duration);
            yield return null;
        }

        Time.timeScale = targetTimeScale;

    }

    private void OnTriggerEnter2D(Collider2D other) {
        StartCoroutine(SmoothTimeScaleTransition(.5f, .25f));
    }

    private void OnTriggerExit2D(Collider2D other) {
        StartCoroutine(SmoothTimeScaleTransition(1, .2f));
        
    }
}
