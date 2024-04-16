using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeed : MonoBehaviour
{
    public CameraID cam;

    [SerializeField] private float speedChange;
    [SerializeField] private float time;
    // Start is called before the first frame upda
private void Start() {
    
}
    public void TriggerAction()
    {
        cam.events.OnChangeSpeed?.Invoke(speedChange, time);

    }
}
