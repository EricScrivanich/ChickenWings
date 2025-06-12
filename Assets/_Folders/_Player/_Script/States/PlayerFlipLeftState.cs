

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerFlipLeftState : PlayerBaseState
{
    private Sequence flipLeftSeq;

    private RectTransform flipImage;
    private Vector3 flipImageTargetRotation = new Vector3(0, 0, 45);
    Vector3[] rotations = new Vector3[]
       {
        new Vector3(0, 0, 40),
        new Vector3(0, 0, -35),
        new Vector3(0, 0, 20),
        new Vector3(0, 0, -15),
        new Vector3(0, 0, 5),
        new Vector3(0, 0, -3),
        Vector3.zero
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
    private float rotationSpeed;
    private float rotationSlowDownTime;
    private float time;
    private float rotationTimer;
    private bool hitSlowTarget;
    private bool prolongRotation;

    private bool resetRot;


    private int jumpAirIndex;

    private bool finishedLerpRot;

    private float angDrag = .45f;
    private float angForce = 390;

    private Sequence angVelSequence;

    private float angVel;
    private bool tweeningAng;
    private bool hasFinishedEndOfTweenLogic;


    public override void EnterState(PlayerStateManager player)
    {
        player.AdjustForce(JumpForce);
        player.SetHingeTargetAngle(50);
        // player.transform.eulerAngles = Vector3.zero;
        finishedLerpRot = false;
        tweeningAng = true;
        hasFadedJumpAir = false;
        jumpAirIndex = player.CurrentJumpAirIndex;
        hitSlowTarget = false;
        addForceDownTimer = 0;
        rotationSlowDownTime = .6f;
        time = 0;
        rotationTimer = 0;
        float added = 0;

        if (player.shotgunEquipped)
            angDrag = .5f;
        else angDrag = .45f;
        // if (player.justFlippedLeft)
        if (player.rb.angularVelocity > 10)
        {
            hasFinishedEndOfTweenLogic = true;
            added = player.rb.angularVelocity * .1f;

            if (added > 35)
            {

                added = 35;

            }

            player.rb.angularDamping = angDrag;
            player.rb.angularVelocity = angForce + added;
        }

        else
        {
            hasFinishedEndOfTweenLogic = false;
            float dur = .2f;

            if (player.shotgunEquipped) dur = .35f;
            TweenAngVel(dur, 5);
        }
        player.SetFlipDirection(false);
        // player.rb.angularVelocity = angForce;
        // player.rb.angularDrag = angDrag;

        // player.anim.SetTrigger("FlipTrigger");
        // if (player.transform.rotation.eulerAngles.z > 195f)
        // {
        //     prolongRotation = true;
        // }
        // if (player.transform.rotation.eulerAngles.z > 30f)
        // {
        //     prolongRotation = true;
        //     resetRot = true;
        // }
        // else
        // {
        //     resetRot = false;

        //     prolongRotation = false;
        // }
        // currentRotation = player.transform.rotation.eulerAngles.z;

        // if (player.shotgunEquipped)
        // {
        //     rotationSpeed = 300;
        //     rotationSlowDownTime = .8f;
        //     // addForceDownMultiplier = 1.25f;

        // }
        // else
        // {
        //     rotationSpeed = 340;
        //     rotationSlowDownTime = .6f;
        //     // addForceDownMultiplier = 1f;

        // }
        // rotationSpeedVar = rotationSpeed;

        AudioManager.instance.PlayCluck();
        Tween();
    }
    private void TweenAngVel(float dur, float start)
    {
        if (angVelSequence != null && angVelSequence.IsPlaying())
        {
            angVelSequence.Kill();
        }
        angVelSequence = DOTween.Sequence();
        angVelSequence.Append(DOTween.To(() => start, x => angVel = x, angForce, dur));
        // <-- Added this
        angVelSequence.Play().OnComplete(() => tweeningAng = false);
    }
    private void Tween()
    {
        if (flipLeftSeq != null && flipLeftSeq.IsPlaying())
            flipLeftSeq.Kill();

        flipLeftSeq = DOTween.Sequence();
        // <-- Added this

        flipLeftSeq.Append(flipImage.DORotate(flipImageTargetRotation, addForceTime).SetEase(Ease.InSine))
                .Join(flipImage.DOScale(1.2f, .35f));
        flipLeftSeq.Play();
    }

    public void StopTweens()
    {
        if (flipLeftSeq != null && flipLeftSeq.IsPlaying())
        {
            flipLeftSeq.Kill();
        }

        if (angVelSequence != null && angVelSequence.IsPlaying())
        {
            angVelSequence.Kill();
        }
        hasFinishedEndOfTweenLogic = true;
        tweeningAng = false;
        hasFadedJumpAir = true;
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
        if (!hasFinishedEndOfTweenLogic)
        {
            player.rb.angularVelocity = angVel;

            if (!tweeningAng)
            {
                player.rb.angularVelocity = angForce;
                player.rb.angularDamping = angDrag;
                hasFinishedEndOfTweenLogic = true;
            }
        }

        time += Time.fixedDeltaTime;
        if (time > .2f)
        {
            if (player.holdingLeftFlip)
            {
                player.rb.AddForce(AddForceVector);
                rotationSlowDownTime += .45f * Time.fixedDeltaTime;
                player.rb.AddTorque(.1f);

                if (time > addForceTime)
                {

                    player.holdingLeftFlip = false;

                }
            }
            else if (!player.holdingLeftFlip && addForceDownTimer < addForceDownTime)
            {
                addForceDownTimer += Time.fixedDeltaTime;
                player.rb.AddForce(AddForceDownVector);
            }
        }
    }
    // public override void RotateState(PlayerStateManager player)
    // {
    //     currentRotation += rotationSpeedVar * Time.fixedDeltaTime;
    //     float targetRotation = Mathf.LerpAngle(player.rb.rotation, currentRotation, Time.fixedDeltaTime * rotationSpeedVar);
    //     player.rb.MoveRotation(targetRotation);
    //     if (hitSlowTarget && !player.holdingLeftFlip)
    //     {
    //         rotationTimer += Time.fixedDeltaTime;
    //         rotationSpeedVar = Mathf.Lerp(rotationSpeed, 0, rotationTimer / rotationSlowDownTime);
    //     }
    // }

    public override void RotateState(PlayerStateManager player)
    {

        // if (resetRot)
        // {
        //     player.BaseRotationLogic();
        //     if (Mathf.Abs(player.transform.eulerAngles.z) > 10)
        //     {
        //         resetRot = false;
        //         player.rb.angularVelocity = angForce;
        //         player.rb.angularDrag = angDrag;

        //     }
        // }

    }


    public override void UpdateState(PlayerStateManager player)
    {
        if (Mathf.Abs(player.transform.rotation.eulerAngles.z - 270f) < 5 && !prolongRotation)
        {
            hitSlowTarget = true;
        }

        else if (prolongRotation)
        {
            if (player.transform.rotation.eulerAngles.z < 30)
            {
                prolongRotation = false;

            }
        }

        if (!hasFadedJumpAir && !player.holdingLeftFlip)
        {

            hasFadedJumpAir = true;

            player.ID.events.OnStopJumpAir?.Invoke(jumpAirIndex);

            SwingJumpImage();

        }

    }

    private void SwingJumpImage()
    {
        if (flipLeftSeq != null && flipLeftSeq.IsPlaying())
        {
            flipLeftSeq.Kill();
        }

        flipLeftSeq = DOTween.Sequence();
        // <-- Added this

        flipImage.DOScale(1, .2f);
        foreach (var rotation in rotations)
        {
            flipLeftSeq.Append(flipImage.DORotate(rotation, 0.15f).SetEase(Ease.OutSine));
        }

        flipLeftSeq.Play();
    }
    public void CachVariables(Vector2 jf, Vector2 afv, Vector2 afdv, float at, float adt)
    {

        JumpForce = jf;
        AddForceVector = afv;
        AddForceDownVector = afdv;
        // addForceTime = .8f;
        // addForceDownTime = .3f;
        addForceTime = at;
        addForceDownTime = adt;
        if (GameObject.Find("FlipLeftIMG") != null)
            flipImage = GameObject.Find("FlipLeftIMG").GetComponent<RectTransform>();

    }
}


#region OldCode
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerFlipLeftState : PlayerBaseState
// {
//     private Vector2 AddForceVector = new Vector2(-9f, 8);
//     private float currentRotation;
//     private float totalRotation;
//     private float flipForceX = -6.9f;

//     private float flipForceY = 9.5f;
//     // private float rotationSpeed = 420;
//     private float rotationSpeed = 100;
//     private float doesTiggerInt;
//     private float rotationSpeedVar;
//     private float rotationDelay = .15f;
//     private float rotationTime = 0;
//     private float flipThreshold;
//     private float time;
//     private bool hitSlowTarget;
//     private bool hitHeightTarget;
//     private bool prolongRotation;

//     public override void EnterState(PlayerStateManager player) 
//     {
//         hitSlowTarget = false;
//         hitHeightTarget = false;

//         time = 0;
//         player.SetFlipDirection(false);
//         rotationSpeedVar = 300;
//         player.anim.SetTrigger("FlipTrigger");
//         if (player.ID.testingNewGravity)
//         {
//             player.rb.gravityScale = player.ID.flipGravity;

//         }

//         totalRotation = 0;
//         currentRotation = player.transform.rotation.eulerAngles.z;

//         if (player.ID.UsingClocker)
//         {
//             rotationSpeed = 300;
//         }
//         else if (currentRotation > 140)
//         {
//             // rotationSpeed += (360 - currentRotation) / 3;
//             prolongRotation = true;
//         }
//         else
//         {
//             // rotationSpeed = 420;
//             prolongRotation = false;
//         }
//         // player.rb.velocity = new Vector2(player.flipLeftForceX, player.flipLeftForceY);
//         player.AdjustForce(player.flipLeftForceX, player.flipLeftForceY);
//         AudioManager.instance.PlayCluck();


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

//     public void ReEnterState()
//     {
//         rotationSpeedVar = 300;

//     }

//     public override void RotateState(PlayerStateManager player)
//     {
//         currentRotation += rotationSpeedVar * Time.fixedDeltaTime;
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
//         if (!hitHeightTarget && player.rb.velocity.y < -1)
//         {

//             hitHeightTarget = true;


//         }
//         if (player.transform.rotation.eulerAngles.z > 330 && !hitSlowTarget && !prolongRotation)
//         {
//             hitSlowTarget = true;

//             if (player.transform.rotation.eulerAngles.z > 30)
//             {
//                 prolongRotation = false;

//             }
//         }
//     }



// }
#endregion


