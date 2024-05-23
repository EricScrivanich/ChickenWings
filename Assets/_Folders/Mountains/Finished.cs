using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finished : MonoBehaviour
{
    public CameraID cam;
    // Start is called before the first frame update
    public void TriggerAction()
    {
        Debug.Log("HITTARGET");
        cam.events.OnStopTimer?.Invoke(true);

    }
}
