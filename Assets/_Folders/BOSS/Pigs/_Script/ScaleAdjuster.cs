using UnityEngine;

public class ScaleAdjuster : MonoBehaviour
{
    [Header("SpriteObjects")]
    [SerializeField] private Transform backLegs;
    [SerializeField] private Transform wings;
    [SerializeField] private Transform tail;
    [SerializeField] private Transform body;
    [Header("SpritePositions")]

    [SerializeField] private Transform backLegsPosition;
    [SerializeField] private Transform wingsPosition;
    [SerializeField] private Transform tailPosition;

    [SerializeField] private float bodyMinScaleX;
    [SerializeField] private float bodyMaxScaleX;

    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        SetScales();

    }

    public void SetScales()
    {
        if (body != null)
        {

            body.localScale = new Vector3(Mathf.Lerp(bodyMinScaleX, bodyMaxScaleX, Mathf.InverseLerp(maxScale, minScale, transform.localScale.y)), 1, 1);
            backLegs.position = backLegsPosition.position;
            wings.position = wingsPosition.position;
            tail.position = tailPosition.position;
        }

    }
    // Update is called once per frame

}
