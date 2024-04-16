using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MountainTriggers : MonoBehaviour
{
    [SerializeField] private bool ChangeCamera;
    [SerializeField] private bool ChangeZoom;
    [SerializeField] private bool ChangeSpeed;
    [SerializeField] private bool EventBool;
    [SerializeField] private Vector2 CameraPosition;
    [SerializeField] private float CameraTime;
    [SerializeField] private float ZoomAmount;
    [SerializeField] private float ZoomTime;
    [SerializeField] private float NewSpeed;
    [SerializeField] private float SpeedTime;


    public bool ChangeCameraEnabled => ChangeCamera;
    public bool ChangeZoomEnabled => ChangeZoom;
    public Vector2 TargetCameraPosition => CameraPosition;
    public float TargetZoomAmount => ZoomAmount;
    public GameEvent eventTrigger;

    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.CompareTag("Player"))
        {
            if (ChangeCamera)
            {
                MountainController.OnMoveCamera?.Invoke(CameraPosition, CameraTime);
            }

            if (ChangeZoom)
            {
                MountainController.OnChangeZoom?.Invoke(ZoomAmount, ZoomTime);

            }

            if (ChangeSpeed)
            {
                MountainController.OnChangeSpeed?.Invoke(NewSpeed, SpeedTime);
            }

            if (EventBool)
            {
                eventTrigger.TriggerEvent();
               
            }


        }

    }
}
