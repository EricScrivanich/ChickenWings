using UnityEngine;

public class RigidboyCircularRotater : MonoBehaviour
{
    [ExposedScriptableObject]
    [SerializeField] private AnimationCurveSO completionCurve;
    [ExposedScriptableObject]
    [SerializeField] private AnimationCurveSO localXDistanceCurve;
    [SerializeField] private float maxLocalXDistance;
    private float timer;
    private float duration;
    private float finalRotation;
    private float startRotation;
    private Rigidbody2D rb;
    private Rigidbody2D targetrRb;
    [SerializeField] private Transform targetPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        this.enabled = false;
    }

    // Update is called once per frame
    public void DoRotation(Rigidbody2D r, float duration, float rotateAmount, float startRotation, int flipped)
    {
        Debug.Log("Doing rotation");
        targetrRb = r;
        this.duration = duration;
        this.startRotation = startRotation;
        finalRotation = startRotation * flipped;
        rb.SetRotation(startRotation);
        float startOffset = localXDistanceCurve.ReturnValue(0);
        targetPoint.localPosition = new Vector2(startOffset, 0);
        rb.position = new Vector2(r.position.x - startOffset, r.position.y);
        finalRotation = rotateAmount * flipped;
        timer = 0;
        this.enabled = true;

    }
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        float completionCurveValue = completionCurve.ReturnValue(timer / duration);
        float localXDistanceCurveValue = localXDistanceCurve.ReturnValue(completionCurveValue);
        float rotateAmount = Mathf.Lerp(startRotation, finalRotation, completionCurveValue);
        rb.SetRotation(rotateAmount);
        targetPoint.localPosition = new Vector2(localXDistanceCurveValue, 0);
        targetrRb.MovePosition(targetPoint.position);

    }
}
