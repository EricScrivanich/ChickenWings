using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class TouchGesture : MonoBehaviour
{
    // public PlayerID player;
    [SerializeField] private bool isTap;
    [Header("Hand Animation Settings")]
    [SerializeField] private RectTransform hand;
    [SerializeField] private Vector3 handStartRotation;
    [SerializeField] private Vector3 handRotationAngle;
    [SerializeField] private Vector3 handStartScale;
    [SerializeField] private float handScaleFactor;
    [SerializeField] private float handAnimationDuration;

    [Header("Circle Animation Settings")]
    [SerializeField] private RectTransform circle;
    [SerializeField] private Vector3 circleStartScale;
    [SerializeField] private float circleScaleFactor;
    [SerializeField] private float circleAnimationDuration;
    [SerializeField] private Image circleImage; // To handle opacity
    [SerializeField] private float circleStartAlpha;
    [SerializeField] private float circleEndAlpha;

    private Sequence gestureSequence;


    private void OnEnable()
    {
        InitializeValues();

        if (isTap)
        {
            AnimateTapGesture();
        }
        else
        {
            AnimateHoldGesture();
        }

    }
    private void OnDisable()
    {
        if (gestureSequence != null && gestureSequence.IsActive())
        {
            gestureSequence.Kill();
        }
    }

    private void InitializeValues()
    {
        // Set initial values for hand
        hand.localRotation = Quaternion.Euler(handStartRotation);
        hand.localScale = handStartScale;

        // Set initial values for circle
        circle.localScale = circleStartScale;
        Color color = circleImage.color;
        color.a = circleStartAlpha;
        circleImage.color = color;
    }

    private void AnimateTapGesture()
    {
        // Create a sequence
        gestureSequence = DOTween.Sequence().SetUpdate(true);

        // Hand animation: Rotate and scale up
        gestureSequence.Append(hand.DOScale(handScaleFactor, handAnimationDuration).SetEase(Ease.InOutSine).From(handStartScale).SetUpdate(true));
        gestureSequence.Join(hand.DORotate(handRotationAngle, handAnimationDuration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine).From(handStartRotation).SetUpdate(true));

        // gestureSequence.AppendCallback(() => player.events.OnJump?.Invoke());
        // Hand animation: Scale and rotate back to original
        gestureSequence.Append(hand.DOScale(handStartScale, circleAnimationDuration).SetEase(Ease.InOutSine).SetUpdate(true));

        gestureSequence.Join(hand.DORotate(handStartRotation, circleAnimationDuration).SetEase(Ease.InOutSine).SetUpdate(true));

        // Circle animation: Scale and change opacity
        gestureSequence.Join(circle.DOScale(circleScaleFactor, circleAnimationDuration).SetEase(Ease.InOutSine).From(circleStartScale).SetUpdate(true));
        gestureSequence.Join(circleImage.DOFade(circleEndAlpha, circleAnimationDuration).SetEase(Ease.InOutSine).From(circleStartAlpha).SetUpdate(true));

        // Loop the sequence indefinitely
        gestureSequence.SetLoops(-1, LoopType.Restart).SetUpdate(true);
    }

    private void AnimateHoldGesture()
    {
        // Create a sequence
        gestureSequence = DOTween.Sequence().SetUpdate(true);

        // Hand animation: Rotate and scale up to the target
        gestureSequence.Append(hand.DOScale(handScaleFactor, handAnimationDuration).SetEase(Ease.InOutSine).From(handStartScale).SetUpdate(true));
        gestureSequence.Join(hand.DORotate(handRotationAngle, handAnimationDuration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine).From(handStartRotation).SetUpdate(true));

        // Circle animation: Scale and change opacity after hand animation completes
        // gestureSequence.AppendCallback(() =>
        // {
        //     Debug.Log("Calling HoldJumpFunc");
        //     HoldJumpFunc();
        // });
        gestureSequence.Append(circle.DOScale(circleScaleFactor, circleAnimationDuration).SetEase(Ease.InOutSine).From(circleStartScale).SetUpdate(true));
        gestureSequence.Join(circleImage.DOFade(circleEndAlpha, .1f).SetEase(Ease.InOutSine).From(circleStartAlpha).SetUpdate(true));

        // Hand animation: Scale and rotate back to original after circle animation completes
        gestureSequence.Append(hand.DOScale(handStartScale, handAnimationDuration).SetEase(Ease.InOutSine).SetUpdate(true));
        gestureSequence.Join(hand.DORotate(handStartRotation, handAnimationDuration).SetEase(Ease.InOutSine).SetUpdate(true));
        gestureSequence.Join(circleImage.DOFade(0, .2f).SetUpdate(true));

        // Loop the sequence indefinitely
        gestureSequence.SetLoops(-1, LoopType.Restart).SetUpdate(true);
    }

    // private void HoldJumpFunc()
    // {
    //     Debug.Log("Function HoldJumpFunc called");
    //     StartCoroutine(holdJump());
    // }

    // private IEnumerator holdJump()
    // {
    //     Debug.Log("Coroutine holdJump started");

    //     player.events.OnJump?.Invoke();
    //     Debug.Log("OnJump invoked");

    //     yield return new WaitForSecondsRealtime(.2f);

    //     player.events.OnJumpHeld?.Invoke(true);
    //     Debug.Log("OnJumpHeld invoked");
    // }
}