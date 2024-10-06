using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TouchGestureVideo : MonoBehaviour
{
    public PlayerID player;
    public float initialDelay;
    public int loopCount;

    public bool isLeft;
    public bool isDash;
    public bool isDashSlash;
    public bool isDrop;
    public bool isJump;
    public bool isEggDrop;

    private CanvasGroup fadeGroup;
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

    private bool isHolding;
    private bool hasCompleted;
    private bool hasStarted;

    private void Awake()
    {
        InitializeValues();


        fadeGroup = GetComponent<CanvasGroup>();
        fadeGroup.DOFade(0, .1f);
        if (isJump)
        {
            player.events.OnJump += InitialTap;
            player.events.OnJumpHeld += FlipListener;
        }
        else if (isEggDrop) player.events.OnEggDrop += InitialTap;
        else if (isDash) player.events.OnDash += FlipListener;
        else if (isDrop) player.events.OnDrop += InitialTap;
        else if (isDashSlash) player.events.OnDashSlash += InitialTap;
        else if (isLeft) player.events.OnFlipLeft += FlipListener;
        else player.events.OnFlipRight += FlipListener;
    }

    private void OnDestroy()

    {
        if (isJump)
        {
            player.events.OnJump -= InitialTap;
            player.events.OnJumpHeld -= FlipListener;
        }
        else if (isEggDrop) player.events.OnEggDrop -= InitialTap;
        else if (isDash) player.events.OnDash -= FlipListener;
        else if (isDrop) player.events.OnDrop -= InitialTap;
        else if (isLeft) player.events.OnFlipLeft -= FlipListener;
        else if (isDashSlash) player.events.OnDashSlash -= InitialTap;

        else player.events.OnFlipRight -= FlipListener;

    }

    // private void OnDisable()
    // {
    //     if (gestureSequence != null && gestureSequence.IsActive())
    //     {
    //         gestureSequence.Kill();
    //     }
    // }

    // private IEnumerator WaitForDelay()
    // {
    //     hand.gameObject.SetActive(false);
    //     circle.gameObject.SetActive(false);
    //     yield return new WaitForSecondsRealtime(initialDelay);
    //     hand.gameObject.SetActive(true);
    //     circle.gameObject.SetActive(true);
    //     if (isTap)
    //     {
    //         AnimateTapGesture();
    //     }
    //     else
    //     {
    //         AnimateHoldGesture();
    //     }
    // }

    private void InitializeValues()
    {
        // // Set initial values for hand
        // hand.localRotation = Quaternion.Euler(handStartRotation);
        // hand.localScale = handStartScale;

        // // Set initial values for circle
        // circle.localScale = circleStartScale;
        // Color color = circleImage.color;
        // color.a = circleStartAlpha;
        // circleImage.color = color;
    }
    private void InitialTap()
    {
        if (gestureSequence != null && gestureSequence.IsPlaying())
            gestureSequence.Kill();
        isHolding = true;
        hasCompleted = false;
        gestureSequence = DOTween.Sequence();
        gestureSequence.Append(hand.DOScale(handScaleFactor, .2f).SetEase(Ease.InOutSine).From(handStartScale));
        gestureSequence.Join(circle.DOScale(circleStartScale, 0f));
        gestureSequence.Join(circleImage.DOFade(circleStartAlpha, 0f));

        gestureSequence.Join(hand.DORotate(handRotationAngle, .2f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine));

        gestureSequence.Join(fadeGroup.DOFade(1, .2f).SetEase(Ease.InSine));

        gestureSequence.OnComplete(() =>
       {
           hasCompleted = true;
           if (isHolding && !isDrop && !isEggDrop && !isDashSlash)
           {
               if (isJump)
                   AnimateHoldingJump();

               else
                   AnimateHolding();
           }
           else
           {
               AnimateRelease();
           }
       });



        gestureSequence.Play();


    }

    private void InitialTapJump()
    {
        hasCompleted = false;
        if (gestureSequence != null && gestureSequence.IsPlaying())
            gestureSequence.Kill();

        // Ensure alpha values are set correctly
        fadeGroup.alpha = 1;
        Color color = circleImage.color;
        color.a = 1;
        circleImage.color = color;

        hasStarted = true;
        gestureSequence = DOTween.Sequence();
        gestureSequence.Append(hand.DOScale(handScaleFactor, .4f).SetEase(Ease.InOutSine).From(handStartScale));
        gestureSequence.Join(circle.DOScale(circleStartScale, 0f));
        gestureSequence.Join(circleImage.DOFade(circleStartAlpha, 0f));
        gestureSequence.Join(hand.DORotate(handRotationAngle, .4f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine));
        gestureSequence.OnComplete(() =>
        {
            hasCompleted = true;
            if (isHolding)
            {
                AnimateHolding();
            }
            else
            {
                AnimateRelease();
            }
        });
        gestureSequence.Play();


    }



    private void FlipListener(bool holding)
    {
        isHolding = holding;
        Debug.Log("Called");
        if (hasCompleted && !isHolding)
        {
            AnimateRelease();
        }
        else if (holding && !isJump)
        {
            InitialTap();
        }

    }

    private void AnimateHolding()
    {
        gestureSequence.Append(circle.DOScale(circleScaleFactor, .8f).SetEase(Ease.InOutSine).From(handStartScale));
        gestureSequence.Join(circleImage.DOFade(circleEndAlpha, .8f));


    }

    private void AnimateHoldingJump()
    {
        gestureSequence.Append(circle.DOScale(circleScaleFactor, 1.2f).SetEase(Ease.InOutSine).From(handStartScale));
        gestureSequence.Join(circleImage.DOFade(circleEndAlpha, 1.2f));


    }

    private void AnimateRelease()
    {
        hasCompleted = false;
        if (gestureSequence != null && gestureSequence.IsPlaying())
            gestureSequence.Kill();
        fadeGroup.alpha = 1;
        gestureSequence.Append(hand.DOScale(handStartScale, .4f).SetEase(Ease.InOutSine));


        gestureSequence.Join(hand.DORotate(handStartRotation, .4f).SetEase(Ease.InOutSine));

        // Circle animation: Scale and change opacity
        gestureSequence.Join(circle.DOScale(circleScaleFactor, .4f).SetEase(Ease.InOutSine));
        gestureSequence.Join(circleImage.DOFade(circleEndAlpha, .4f).SetEase(Ease.InOutSine));

        gestureSequence.Append(fadeGroup.DOFade(0, .9f));



        gestureSequence.Play();



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
        gestureSequence.SetLoops(loopCount);
        gestureSequence.OnComplete(() => EndGestureSequence());
    }

    private void EndGestureSequence()
    {
        gameObject.SetActive(false);
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