using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public struct CameraEvents 
{
    
    public Action<List<GameObject>, float> OnChangePosition;
    public Action<float, float> OnShakeCamera;

    public Action<bool, float> OnMoveEnviroment;
    public Action<float, float> OnChangeYPosition;
    public Action<float, float> OnChangeXPosition;
    public Action<float, bool, float> AdjustCameraX;
    public Action<float, bool, float> AdjustCameraY;
    public Action<Vector2, int> OnSendPosiition;
    public Action<float, float> OnChangeZoom;
    public Action<float,float> OnChangeSpeed;
    public Action<int,bool> OnChangeGameObject;
}
