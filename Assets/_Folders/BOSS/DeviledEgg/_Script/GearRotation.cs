using System.Collections;
using UnityEngine;

public class GearRotation : MonoBehaviour
{
    public GameObject smallGear;
    public GameObject targetObject;
    public Cannon topCannon;
    public Cannon leftCannon;
    public Cannon bottomCannon;
    public Cannon rightCannon;
    public int largeGearTeeth = 24;
    public int smallGearTeeth = 6;
    public float maxRotationSpeed = 30f;
    public float rotationSpeed = 10f;
    public float fireTime = 3f;
    public bool fireAllAtOnce = false;
    public bool shouldRotateContinuously = false;
    public bool rotateClockwise = true;

    public enum ActiveCannon
    {
        Top,
        Left,
        Bottom,
        Right
    }

    public ActiveCannon activeCannon;

    private float speedRatio;
    private Quaternion previousRotation;

    void Start()
    {
        speedRatio = GetSpeedRatio(largeGearTeeth, smallGearTeeth);
        previousRotation = transform.rotation;

        StartCoroutine(FireCannons());
    }

    void Update()
    {
      if (targetObject != null)
      {
        if (shouldRotateContinuously)
        {
            RotateContinuously();
        }
        else
        {
            RotateGears();
        }
      }
      else
      {
        return;
      }
        
    }

    private float GetSpeedRatio(int largeGearTeeth, int smallGearTeeth)
    {
        return (float)largeGearTeeth / smallGearTeeth;
    }

    private void RotateGears()
    {
        // Calculate the angle required for the active cannon to face the target object
        Vector3 difference = targetObject.transform.position - transform.position;
        float targetRotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 90f;

        // Modify the target rotation depending on the active cannon
        switch (activeCannon)
        {
            case ActiveCannon.Top:
                break;
            case ActiveCannon.Left:
                targetRotationZ -= 90f;
                break;
            case ActiveCannon.Bottom:
                targetRotationZ -= 180f;
                break;
            case ActiveCannon.Right:
                targetRotationZ -= 270f;
                break;
        }

        RotateGear(targetRotationZ);
    }

    private void RotateGear(float targetRotationZ)
    {
        float rotationAngle = Mathf.DeltaAngle(previousRotation.eulerAngles.z, targetRotationZ);

        rotationAngle = Mathf.Clamp(rotationAngle, -maxRotationSpeed * Time.deltaTime, maxRotationSpeed * Time.deltaTime);

        transform.Rotate(0, 0, rotationAngle);

        previousRotation = transform.rotation;

        float smallGearRotationAngle = rotationAngle * speedRatio;

        smallGear.transform.Rotate(0, 0, -smallGearRotationAngle);
    }

    private void RotateContinuously()
    {
        float rotationAngle = rotateClockwise ? rotationSpeed * Time.deltaTime : -rotationSpeed * Time.deltaTime;
        transform.Rotate(0, 0, rotationAngle);
        smallGear.transform.Rotate(0, 0, -rotationAngle * speedRatio);
    }

    private IEnumerator FireCannons()
    {
        while (true)
        {
            if (fireAllAtOnce)
            {
                topCannon.FireCannonBall();
                leftCannon.FireCannonBall();
                bottomCannon.FireCannonBall();
                rightCannon.FireCannonBall();

                yield return new WaitForSeconds(fireTime);
            }
            else
            {
                topCannon.FireCannonBall();
                yield return new WaitForSeconds(fireTime);
                leftCannon.FireCannonBall();
                yield return new WaitForSeconds(fireTime);
                bottomCannon.FireCannonBall();
                yield return new WaitForSeconds(fireTime);
                rightCannon.FireCannonBall();
                yield return new WaitForSeconds(fireTime);
            }
        }
    }
}
