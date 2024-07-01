using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : PlayerSystem
{
    private int rotationLerpSpeed = 20;
    private int jumpRotSpeed = 200;
    private int frozenRotSpeed = 350;
    private int maxRotUp = 15;
    private int maxRotDown = -30;
    private float rotZ;
    private float frozenRotZ;
    private float totalRotation = 0.0f; // Total rotation done so far
    private float targetRotation = 0.0f;
    private float currentRotation = 0;
    private int remainingLeftFlips = 0;
    private int remainingRightFlips = 0;
    private float currentZRotation;

    // degrees per second
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float fastLerpSpeed = 1000f;
    private float rotationSpeedVar;




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        JumpRotation();

    }


    void JumpRotation()
    {
        if (player.isDropping)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (player.isDashing)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * fastLerpSpeed);
        }

        else if (player.isFlipping)
        {

            currentRotation += rotationSpeedVar * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);

            // if (currentRotation >= 360 || currentRotation <= -360)
            // {
            //     player.isFlipping = false;
            //     currentRotation
            // }




        }
        else
        {
            // If the object is moving upwards, rotate it upwards
            if (rb.velocity.y > 0 && rotZ < maxRotUp)
            {
                // Calculate the new rotation
                rotZ += jumpRotSpeed * Time.deltaTime;
            }
            // If the object is moving downwards, rotate it downwards
            else if (rb.velocity.y < 0 && rotZ > maxRotDown)
            {
                // Calculate the new rotation
                rotZ -= jumpRotSpeed * Time.deltaTime;
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rotZ), Time.deltaTime * rotationLerpSpeed);
        }

    }

    private void FlipRight(bool holding)
    {
        if (holding)
        {
            currentRotation = transform.rotation.eulerAngles.z;

            rotationSpeedVar = -rotationSpeed;
        }


    }

    private void FlipLeft(bool holding)
    {
        if (holding)
        {
            currentRotation = transform.rotation.eulerAngles.z;

            rotationSpeedVar = rotationSpeed;
        }

    }

    void OnEnable()
    {
        player.ID.events.OnFlipRight += FlipRight;
        player.ID.events.OnFlipLeft += FlipLeft;
    }
    void OnDisable()
    {
        player.ID.events.OnFlipRight -= FlipRight;
        player.ID.events.OnFlipLeft -= FlipLeft;
    }
}
