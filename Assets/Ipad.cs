using UnityEngine;

public class Ipad : MonoBehaviour
{
    void Start()
    {
        Camera camera = GetComponent<Camera>();

        // Adjust the camera's field of view or orthographic size based on the aspect ratio
        if (camera.aspect < 1.5f) // For screens with an aspect ratio less than 1.5 (like iPads)
        {
            camera.orthographicSize *= 1.5f;
        }
    }
}