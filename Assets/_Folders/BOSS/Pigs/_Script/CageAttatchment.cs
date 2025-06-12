using UnityEngine;

public class CageAttatchment : MonoBehaviour
{
    public Vector2 offset;
    [field: SerializeField] public Transform transformInsteadOfOffset { get; private set; } = null;
    [field: SerializeField] public bool useAdditionalTransform = false;
    [HideInInspector] public bool showGizmo = false;

    [field: SerializeField] public bool useRotation { get; private set; }
    [SerializeField] private short rotationOffset = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector2 ReturnPosition()
    {
        if (useAdditionalTransform)
        {
            return (Vector2)transformInsteadOfOffset.position + offset;
        }

        return offset + (Vector2)transform.position;
    }

    public void SetCageParent(Transform cage, out bool useManualOffset)
    {

        if (useAdditionalTransform)
        {
            // cage.SetParent(transformInsteadOfOffset);
            // cage.localPosition = offset;
            useManualOffset = true;
        }
        else
        {
            useManualOffset = false;

            cage.SetParent(this.transform);
            cage.localPosition = offset;
        }


    }

    public int ReturnRotation()
    {
        if (useRotation && useAdditionalTransform)
        {
            return (int)(transform.eulerAngles.z + rotationOffset);
        }

        return 0;
    }
}
