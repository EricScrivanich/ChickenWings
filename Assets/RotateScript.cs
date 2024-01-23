using UnityEngine;

public class RotateScript : MonoBehaviour
{
    public float scaleTime = 2f; // Time to scale from 0 to 1
    public AnimationCurve rotationSpeedCurve; // Curve to control rotation speed
    public float totalRotationDuration = 5f; // Total time for all rotations
    public int maxRotations = 3; // Maximum number of rotations

    private float currentScaleTime;
    private float rotationTimer;
    private bool isScalingUp = true;
    private bool isRotating = false;


}



