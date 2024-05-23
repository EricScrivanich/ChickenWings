using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedLevel : MonoBehaviour
{
    public CameraID cam;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit");
        if (other.gameObject.CompareTag("Manager"))
        {
            Debug.Log("Invoked");

            cam.events.OnChangeSpeedFinished?.Invoke(0, 1.5f);

        }
    }
}
