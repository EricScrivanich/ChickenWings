using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeZoom : MonoBehaviour
{
    public CameraID cam;

    [SerializeField] private float zoom;
    [SerializeField] private float time;
    // Start is called before the first frame upda

    private void Start()
    {

    }
    public void TriggerAction()
    {
        cam.events.OnChangeZoom?.Invoke(zoom, time);

    }
}
