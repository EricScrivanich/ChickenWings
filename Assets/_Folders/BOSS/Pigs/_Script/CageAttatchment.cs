using UnityEngine;

public class CageAttatchment : MonoBehaviour
{
    public Vector2 offset;
    [SerializeField] private Transform transformInsteadOfOffset = null;
    [SerializeField] private bool useTransformInsteadOfOffset = false;
    [HideInInspector] public bool showGizmo = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector2 ReturnPosition()
    {
        if (useTransformInsteadOfOffset)
        {
            return transformInsteadOfOffset.position;
        }

        return offset + (Vector2)transform.position;
    }
}
