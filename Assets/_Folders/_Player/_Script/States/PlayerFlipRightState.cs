
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerFlipRightState : PlayerBaseState
{
    private Sequence sequence;
    private RectTransform flipImage;
    private Vector3 flipImageTargetRotation = new Vector3(0, 180, 45);
    Vector3[] rotations = new Vector3[]
       {
        new Vector3(0, 180, 40),
        new Vector3(0, 180, -35),
        new Vector3(0, 180, 20),
        new Vector3(0, 180, -15),
        new Vector3(0, 180, 5),
        new Vector3(0, 180, -3), 
       new Vector3(0,180,0)
};
    private Vector2 JumpForce;
    private bool hasFadedJumpAir;
    private Vector2 AddForceVector;
    private Vector2 AddForceDownVector;
    private float addForceTime;
    private float addForceDownTime;
    private float addForceDownTimer;
    private float currentRotation = 0;
    private float rotationSpeedVar;
    private float rotationSpeed = 370;
    private float rotationSlowDownTime;
    private float time;
    private float rotationTimer;
    private bool hitSlowTarget;
    private bool prolongRotation;
    private int jumpAirIndex;

    public override void EnterState(PlayerStateManager player)
    {
        if (sequence != null && sequence.IsPlaying())
            sequence.Kill();

        sequence = DOTween.Sequence();

        sequence.Append(flipImage.DORotate(flipImageTargetRotation, addForceTime)).SetEase(Ease.InSine);
        // sequence.Join(flipImage.DOScale(flipImageTargetScale * 1.1f, .6f)).SetEase(Ease.InSine);
        sequence.Play();
        hasFadedJumpAir = false;
        jumpAirIndex = player.CurrentJumpAirIndex;
        hitSlowTarget = false;
        addForceDownTimer = 0;
        rotationSlowDownTime = .6f;
        time = 0;
        rotationTimer = 0;
        player.SetFlipDirection(true);
        // player.anim.SetTrigger("FlipTrigger");
        if (player.transform.rotation.eulerAngles.z < 175f)
        {
            prolongRotation = true;
        }
        else
        {
            prolongRotation = false;
        }
        currentRotation = player.transform.rotation.eulerAngles.z;
        rotationSpeedVar = rotationSpeed;
        player.AdjustForce(JumpForce);
        AudioManager.instance.PlayCluck();
    }
    public void ReEnterState()
    {
        rotationSpeedVar = 300;
    }
    public override void ExitState(PlayerStateManager player)
    {


        if (!hasFadedJumpAir)
        {

            player.ID.events.OnStopJumpAir?.Invoke(jumpAirIndex);
            SwingJumpImage();
        }
    }
    public override void FixedUpdateState(PlayerStateManager player)
    {
        time += Time.fixedDeltaTime;
        if (time > .2f)
        {
            if (player.holdingRightFlip)
            {
                player.rb.AddForce(AddForceVector);
                rotationSlowDownTime += .55f * Time.fixedDeltaTime;

                if (time > addForceTime)
                {
                    player.holdingRightFlip = false;

                }
            }
            else if (!player.holdingRightFlip && addForceDownTimer < addForceDownTime)
            {
                addForceDownTimer += Time.fixedDeltaTime; 
                player.rb.AddForce(AddForceDownVector);
            }
        }
    }
    public override void RotateState(PlayerStateManager player)
    {
        currentRotation -= rotationSpeedVar * Time.fixedDeltaTime;
        float targetRotation = Mathf.LerpAngle(player.rb.rotation, currentRotation, Time.fixedDeltaTime * rotationSpeedVar);
        player.rb.MoveRotation(targetRotation);
        if (hitSlowTarget && !player.holdingRightFlip)
        {
            rotationTimer += Time.fixedDeltaTime;
            rotationSpeedVar = Mathf.Lerp(rotationSpeed, 0, rotationTimer / rotationSlowDownTime);
        }
    }
    public override void UpdateState(PlayerStateManager player)
    {
        if (Mathf.Abs(player.transform.rotation.eulerAngles.z - 90f) < 5 && !prolongRotation)
        {
            hitSlowTarget = true;
        }

        else if (prolongRotation)
        {
            if (player.transform.rotation.eulerAngles.z > 330)
            {
                prolongRotation = false;

            }
        }

        if (!hasFadedJumpAir && !player.holdingRightFlip)
        {
            hasFadedJumpAir = true;
            player.ID.events.OnStopJumpAir?.Invoke(jumpAirIndex);
            SwingJumpImage();
        }

    }

    private void SwingJumpImage()
    {
        if (sequence != null && sequence.IsPlaying())
        {
            sequence.Kill();
        }

        sequence = DOTween.Sequence();

        foreach (var rotation in rotations)
        {
            sequence.Append(flipImage.DORotate(rotation, 0.15f).SetEase(Ease.OutSine));

        }

        sequence.Play();
    }

    public void CachVariables(Vector2 jf, Vector2 afv, Vector2 afdv, float at, float adt)
    {

        JumpForce = jf;
        AddForceVector = afv;
        AddForceDownVector = afdv;
        addForceTime = at;
        addForceDownTime = adt;
        if (GameObject.Find("FlipRightIMG") != null)
            flipImage = GameObject.Find("FlipRightIMG").GetComponent<RectTransform>();

    }
}

#region OldCode

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerFlipRightState : PlayerBaseState
// {

//     private Vector2 AddForceVector = new Vector2(7.5f, 12.5f);
//     // private Vector2 AddForceVector = new Vector2(11.5f, 13.5f);
//     private float currentRotation = 0;
//     private bool flippingRight;
//     private float totalRotation;
//     private float flipForceX = 6.5f;
//     private float flipForceY = 10f;
//     private float rotationSpeedVar;
//     private float rotationSpeed = 450;
//     private float rotationDelay = .15f;
//     private float rotationTime = 0;
//     private float time;
//     private bool hitSlowTarget;
//     private bool prolongRotation;
//     private bool hitHeightTarget;


//     public override void EnterState(PlayerStateManager player)
//     {
//         hitHeightTarget = false;
//         hitSlowTarget = false;
//         time = 0;
//         player.SetFlipDirection(true);
//         player.anim.SetTrigger("FlipTrigger");
//         // if (player.ID.testingNewGravity)
//         // {
//         //     player.rb.gravityScale = player.ID.flipGravity;

//         // }
//         totalRotation = 0;
//         // if (player.justFlipped && player.justFlippedRight)
//         // {
//         //     player.rb.velocity = new Vector2(player.flipRightForceX - .5f, player.flipRightForceY + .8f);

//         // }
//         // else
//         // {


//         // }
//         currentRotation = player.transform.rotation.eulerAngles.z;

//         if (player.ID.UsingClocker)
//         {
//             rotationSpeed = 300;
//         }
//         else if (currentRotation < 220)
//         {

//             prolongRotation = true;
//             rotationSpeed += currentRotation / 3;
//         }
//         else
//         {

//             prolongRotation = false;
//             rotationSpeed = 450;
//         }
//         rotationSpeedVar = rotationSpeed;
//         // player.rb.velocity = new Vector2(player.flipRightForceX, player.flipRightForceY);

//         player.AdjustForce(player.flipRightForceX, player.flipRightForceY);
//         AudioManager.instance.PlayCluck();





//     }
//     public void ReEnterState()
//     {
//         rotationSpeedVar = 300;

//     }
//     public override void ExitState(PlayerStateManager player)
//     {
//         player.rb.gravityScale = player.originalGravityScale;

//     }

//     public override void FixedUpdateState(PlayerStateManager player)
//     {
//         if (player.holdingFlip && !hitHeightTarget)
//         {
//             player.rb.AddForce(AddForceVector);
//         }

//     }

//     public override void RotateState(PlayerStateManager player)
//     {

//         currentRotation -= rotationSpeedVar * Time.fixedDeltaTime;
//         float targetRotation = Mathf.LerpAngle(player.rb.rotation, currentRotation, Time.fixedDeltaTime * rotationSpeedVar);
//         player.rb.MoveRotation(targetRotation);
//         if (hitSlowTarget)
//         {
//             time += Time.fixedDeltaTime;
//             rotationSpeedVar = Mathf.Lerp(rotationSpeed, 10, time / .4f);
//         }

//     }

//     public override void UpdateState(PlayerStateManager player)
//     {
//         if (player.ID.testingNewGravity && !hitHeightTarget && player.rb.velocity.y < .5)
//         {
//             player.rb.gravityScale = 2.1f;
//             hitHeightTarget = true;


//         }
//         if (player.transform.rotation.eulerAngles.z < player.ID.startLerp && !hitSlowTarget && !prolongRotation)
//         {
//             hitSlowTarget = true;
//         }
//         if (player.transform.rotation.eulerAngles.z > 330 && prolongRotation)
//         {
//             prolongRotation = false;
//         }
//     }
// }

#endregion

