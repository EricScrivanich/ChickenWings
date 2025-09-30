using UnityEngine;

public class AimRotationTransform : MonoBehaviour
{
    private float targetAngle;


    [SerializeField] private float normalOffset;
    private bool isFlipped;
    [SerializeField] private float baseSmoothRotationTime;
    private float smoothRotationTime;
    [SerializeField] private float baseMaxRotationSpeed;

    private float maxRotationSpeed;
    private float rotationRef;

    private int frameSkips = -1;
    private int currentFrameSkips;
    private Transform target;
    private Vector2 offset = Vector2.zero;
    private Vector2 baseOffset = Vector2.zero;
    private Transform parentTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {




        smoothRotationTime = baseSmoothRotationTime;
        parentTransform = transform.parent;
        maxRotationSpeed = baseMaxRotationSpeed;

    }

    public void SetConstantRotation(int skips, Transform t)
    {
        frameSkips = skips;

        currentFrameSkips = 0;
        target = t;
    }
    public void SetOffset(float x, float y)
    {
        if (isFlipped) x *= -1;
        baseOffset.x = x;
        baseOffset.y = y;
        offset = baseOffset;

    }
    private float maxRotation = 360;
    public void SetMaxRotation(float maxRot)
    {
        maxRotation = maxRot;
    }
    public void RecalculateOffset(float angle)
    {
        // Make sure direction is normalized


        // Calculate the angle from Vector2.right to the direction to the target

        // Rotate the baseOffset by this angle
        if (!isFlipped) angle += 180;
        // else angle += 180;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        offset = rotation * baseOffset;
    }
    public void EditRotationValues(float speedMult, float smoothMult)
    {
        maxRotationSpeed = baseMaxRotationSpeed * speedMult;
        smoothRotationTime = baseSmoothRotationTime * smoothMult;

    }
    public void SetIsFlipped(bool flipped)
    {
        isFlipped = flipped;
        baseOffset.x *= -1;
        offset = baseOffset;
    }

    // Update is called once per frame

    public void SetTargetAngle(Vector2 targetPos)
    {
        Vector2 directionToPlayer = targetPos - (Vector2)transform.position;
        float o = normalOffset;
        // if (isFlipped) o -= 180;

        targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + o;



        if (maxRotation < 360)
        {
            float parentAngle = parentTransform.eulerAngles.z;

            // Convert desired global into local (relative to parent)
            float desiredLocalAngle = Mathf.DeltaAngle(parentAngle, targetAngle);

            // Clamp local angle
            float clampedLocalAngle = Mathf.Clamp(desiredLocalAngle, -maxRotation, maxRotation);

            // Convert back to global
            targetAngle = parentAngle + clampedLocalAngle;
        }


    }


    void FixedUpdate()
    {
        if (frameSkips >= 0)
        {
            if (currentFrameSkips >= frameSkips)
            {
                SetTargetAngle((Vector2)target.position + offset);
                currentFrameSkips = 0;
            }

            else
                currentFrameSkips++;
        }

        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref rotationRef, smoothRotationTime, maxRotationSpeed);

        // if (maxRotation < 360 && Mathf.Abs(angle - transform.localEulerAngles.z) > maxRotation)
        // {
        //     transform.localEulerAngles = Vector3.forward * Mathf.Clamp(angle - transform.localEulerAngles.z, -maxRotation, maxRotation);

        // }

        transform.eulerAngles = Vector3.forward * angle;
    }


}
