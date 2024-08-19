using UnityEngine;


public class ObjectRotator : MonoBehaviour
{
    [Header("Rotation Angles")]
    [SerializeField]
    private float rotationX;
    [SerializeField]
    private float rotationY;
    [SerializeField]
    private float rotationZ;

    private void OnValidate()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        // Create quaternions for each axis rotation
        Quaternion rotationXQuat = Quaternion.Euler(rotationX, 0, 0);
        Quaternion rotationYQuat = Quaternion.Euler(0, rotationY, 0);
        Quaternion rotationZQuat = Quaternion.Euler(0, 0, rotationZ);

        // Combine the rotations
        Quaternion finalRotation = rotationXQuat * rotationYQuat * rotationZQuat;

        // Apply the final rotation to the object
        transform.rotation = finalRotation;
    }
}