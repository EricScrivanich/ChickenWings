using UnityEngine;
using DG.Tweening;
using System.Collections;


public class AimRotation : MonoBehaviour
{
    private float targetAngle;
    [SerializeField] private DOTweenPath rotatePath;




    [SerializeField] private Vector3 rotateAroundPosition;
    private Rigidbody2D rb;
    [SerializeField] private float normalOffset;
    [SerializeField] private float rotateAroundOffset;
    private bool isFlipped;
    [SerializeField] private float baseSmoothRotationTime;
    private float smoothRotationTime;
    [SerializeField] private float baseMaxRotationSpeed;

    private float maxRotationSpeed;
    private float rotationRef;

    private int frameSkips = -1; // reset this to -1
    private int currentFrameSkips;
    [SerializeField] private Transform target;

    [Header("Rotate Around")]
    [SerializeField] private float smoothMove;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float rotSpeed = 1f;
    private bool doingRotateAround = false;
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private Transform rotateRef;

    private float maxRotation = 360;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        smoothRotationTime = baseSmoothRotationTime;
        maxRotationSpeed = baseMaxRotationSpeed;




    }

    public void SetConstantRotation(int skips, Transform t)
    {
        frameSkips = skips;
        currentFrameSkips = 0;
        if (t != null)
            target = t;
    }

    public void SetRotateAround(bool rotate)
    {
        doingRotateAround = rotate;
        if (isFlipped && rotatePath.fullWps[0].x > 0)
        {
            for (int i = 0; i < rotatePath.fullWps.Count; i++)
            {
                rotatePath.fullWps[i] = new Vector3(rotatePath.fullWps[i].x * -1, rotatePath.fullWps[i].y, rotatePath.fullWps[i].z);

            }
        }
        else if (!isFlipped && rotatePath.fullWps[0].x < 0)
        {
            for (int i = 0; i < rotatePath.wps.Count; i++)
            {
                rotatePath.fullWps[i] = new Vector3(rotatePath.fullWps[i].x * -1, rotatePath.fullWps[i].y, rotatePath.fullWps[i].z);

            }
        }




        // frameSkips = 0;
        rotatePath.DOPlay();

    }

    private IEnumerator RotateAround(Vector2 start, Vector2 end, float time)
    {
        while (doingRotateAround)
        {
            yield return null;
        }
    }
    public Vector3 GetRotatePoint()
    {
        Vector3 pos = rotatePath.fullWps[0];
        if ((isFlipped && pos.x > 0) || (!isFlipped && pos.x < 0)) pos.x *= -1;

        return pos;
    }

    public void EditRotationValues(float speedMult, float smoothMult)
    {
        maxRotationSpeed = baseMaxRotationSpeed * speedMult;
        smoothRotationTime = baseSmoothRotationTime * smoothMult;


    }
    public void SetIsFlipped(bool flipped)
    {
        isFlipped = flipped;

        if (flipped && maxRotation > 0)
            maxRotation *= -1;
        else if (!flipped && maxRotation < 0)
            maxRotation *= -1;

    }


    public void SetMaxRotation(float r)
    {
        maxRotation = r;
        if (isFlipped) maxRotation *= -1;
    }


    // Update is called once per frame

    public void SetTargetAngle(Vector2 targetPos)
    {
        Vector2 directionToPlayer = targetPos - rb.position;
        float o = normalOffset;
        if (doingRotateAround) o = rotateAroundOffset;
        if (isFlipped) o -= 180;

        targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + o;

    }

    public void SetPathAngleAndAllowedOffset(float pathAngle, float globalOffset, float allowedOffset)
    {
        frameSkips = -1;
        Vector2 directionToPlayer = (Vector2)target.position - rb.position;
        float o = globalOffset;
        Debug.LogError("PATH ANGLE: " + pathAngle);

        // if (isFlipped) o -= 180;

        pathAngle += o;


        float exactAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + o;
        if (exactAngle > pathAngle + allowedOffset)
            targetAngle = pathAngle + allowedOffset;
        else if (exactAngle < pathAngle - allowedOffset)
            targetAngle = pathAngle - allowedOffset;
        else
            targetAngle = exactAngle;

    }

    public float ReturnTargetAngle(float additionOffset)
    {
        Vector2 directionToPlayer = (Vector2)target.position - rb.position;
        float o = normalOffset + additionOffset;
        // if (doingRotateAround) o = rotateAroundOffset;
        if (isFlipped) o -= 180;

        return Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + o;
    }
    public void SetExactAngle(float a, float maxSpeed, float smooth)
    {
        maxRotationSpeed = maxSpeed;
        smoothRotationTime = smooth;
        frameSkips = -1;
        // float o = normalOffset;
        if (isFlipped) a -= 180;

        targetAngle = a;

    }


    private Vector2 idk2;
    void FixedUpdate()
    {



        // if (doingRotateAround)
        // {
        //     float amount = rotSpeed * Time.fixedDeltaTime;
        //     rotateRef.RotateAround(rotatePoint.position, Vector3.forward, amount);
        //     Vector2 pos = Vector2.SmoothDamp(rb.position, rotateRef.position, ref idk2, smoothMove, maxMoveSpeed);
        //     targetAngle += amount;
        //     // if (frameSkips >= 0)
        //     // {
        //     //     if (currentFrameSkips >= frameSkips)
        //     //     {

        //     //         currentFrameSkips = 0;
        //     //     }

        //     //     else
        //     //         currentFrameSkips++;
        //     // }
        //     rb.MovePosition(pos);
        // }
        if (frameSkips >= 0)
        {
            if (currentFrameSkips >= frameSkips)
            {
                SetTargetAngle(target.position);
                currentFrameSkips = 0;
            }

            else
                currentFrameSkips++;
        }

        float angle = Mathf.SmoothDampAngle(rb.rotation, targetAngle, ref rotationRef, smoothRotationTime, maxRotationSpeed);
        if (isFlipped && angle < maxRotation)
            angle = maxRotation;
        else if (!isFlipped && angle > maxRotation)
            angle = maxRotation;

        rb.MoveRotation(angle);

    }
}
