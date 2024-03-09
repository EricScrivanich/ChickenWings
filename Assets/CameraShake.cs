using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public PlayerID player;
    private bool isShaking = false;
    // Start is called before the first frame update

    
    public void ShakeCamera(float duration, float magnitude)
    {
        if (!isShaking)
        {
            StartCoroutine(Shake(duration, magnitude));
            isShaking = true; 
        }

    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 orignalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(x, y,-10);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isShaking = false;
        transform.position = orignalPosition;
    }

    private void OnEnable() {
        player.globalEvents.ShakeCamera += ShakeCamera;
    }
    private void OnDisable() {
        player.globalEvents.ShakeCamera -= ShakeCamera;

    }
}
