using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SidewaysSignMovement : MonoBehaviour
{
    [SerializeField] private RectTransform target;
    [SerializeField] private LevelManagerID lvlID;


    public int TipSignIndex;

    [SerializeField] private Button[] NextPrevButtons;

    [SerializeField] private GameObject NextSign;

    private readonly Vector3[] rotations = new Vector3[]
      {
        new Vector3(0, 0, 8f),
        new Vector3(0, 0, -6.5f),
        new Vector3(0, 0, 3f),
        new Vector3(0, 0, -1.7f),
        new Vector3(0, 0, .7f),
        new Vector3(0, 0, -.3f),
        new Vector3(0, 0, .15f),
        Vector3.zero
      };



    private Vector2 endPosition = new Vector2(0, -1110);
    [SerializeField] private float rotationAmount;
    [SerializeField] private float rotationDuration;
    private float swingDuration = .75f;
    [SerializeField] private float signMovementTime;
    [SerializeField] private Vector2 moveAmount; // Adjust as needed
    private float rotationAmountVar;
    private Vector2 moveAmountVar;

    private RectTransform rectTransform;
    private Sequence sequence;
    private Sequence signSequence;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();


    }

    public void AnimateSign(bool goingLeft, bool SetActive)
    {
        if (signSequence != null && signSequence.IsActive())
        {
            signSequence.Kill();
        }

        if (goingLeft)
        {
            moveAmountVar = moveAmount;
            rotationAmountVar = 1;
        }
        else
        {
            moveAmountVar = -moveAmount;
            rotationAmountVar = -1;

        }
        signSequence = DOTween.Sequence().SetUpdate(true);

        // Calculate half movement
        Vector2 halfMoveAmount = moveAmountVar / 2f;
        Vector2 finalPosition = rectTransform.anchoredPosition + moveAmountVar;

        // First part of the movement and initial rotation
        signSequence.Append(rectTransform.DORotate(rotations[0] * rotationAmountVar, signMovementTime / 2).SetEase(Ease.InSine)).SetUpdate(true);
        signSequence.Join(rectTransform.DOAnchorPos(rectTransform.anchoredPosition + halfMoveAmount, signMovementTime / 2).SetEase(Ease.InSine)).SetUpdate(true);

        // Second part of the movement and counter rotation
        signSequence.Append(rectTransform.DORotate(rotations[1] * rotationAmountVar, signMovementTime / 2).SetEase(Ease.OutSine)).SetUpdate(true);
        signSequence.Join(rectTransform.DOAnchorPos(finalPosition, signMovementTime / 2).SetEase(Ease.OutSine)).SetUpdate(true);

        // Swing back and forth to simulate stopping
        float halfRotationAmount = rotationAmountVar / 2f;
        float quarterRotationAmount = rotationAmountVar / 4f;
        float finalRotationAmount = -(quarterRotationAmount - 5);

        // Rotate to half of the initial rotation amount in the opposite direction
        for (int i = 2; i < rotations.Length; i++)
        {
            signSequence.Append(rectTransform.DORotate(rotations[i] * rotationAmountVar, swingDuration).SetEase(Ease.InOutSine).SetUpdate(true));

        }
        // signSequence.AppendCallback(() => SetUnactive(SetActive));
        // Start the sequence
        signSequence.Play().SetUpdate(true);
    }

    private void SetUnactive(bool b)
    {
        if (!b)
        {
            gameObject.SetActive(false);
        }

    }


    public void DropSign()
    {
        if (lvlID != null)
        {
            lvlID.outputEvent.setButtonsReadyToPress?.Invoke(false);
        }
        rectTransform = GetComponent<RectTransform>();
        if (signSequence != null && signSequence.IsActive())
        {
            signSequence.Kill();
        }
        if (sequence != null && sequence.IsActive())
        {
            sequence.Kill();
        }
        target.anchoredPosition = Vector2.zero;

        rectTransform.eulerAngles = Vector3.zero;

        // Create the main drop tween
        Tweener mainDropTween = target.DOAnchorPos(endPosition, .8f)
            .SetEase(Ease.Linear)
            .SetUpdate(true);

        // Create the overshoot tween
        Vector3 overshootPosition = endPosition - new Vector2(0, 40);
        Tweener overshootTween = target.DOAnchorPos(overshootPosition, 1)
            .SetEase(Ease.OutElastic)
            .SetUpdate(true);

        // Chain the tweens together
        sequence = DOTween.Sequence();
        sequence.Append(mainDropTween)
                .Append(overshootTween)
                 .OnComplete(EnableButtons)
                .SetUpdate(true);
    }

    public void RetractToNextUI(bool isNext)
    {
        if (signSequence != null && signSequence.IsActive())
        {
            signSequence.Kill();
        }

        DisableButtons();
        // LevelManager LM = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        // LM.NextUI(isNext);


        Vector3 overshootPosition = endPosition - new Vector2(0, 40) - new Vector2(0, 70);
        Tweener overshootTween = target.DOAnchorPos(overshootPosition, .4f)
            .SetEase(Ease.InSine)
            .SetUpdate(true);


        Tweener riseTween = target.DOAnchorPos(Vector2.zero, .7f)
           .SetEase(Ease.InSine)
           .SetUpdate(true);


        sequence = DOTween.Sequence();
        sequence.Append(overshootTween)
                .Append(riseTween)
                .OnComplete(SetUnactive)
                .SetUpdate(true);

        sequence.Play();


    }

    private void SetUnactive()
    {
        if (transform.parent != null)
        {
            if (lvlID != null)
            {
                Debug.Log("Showing press buttons");

                lvlID.outputEvent.setButtonsReadyToPress?.Invoke(true);
            }

            Debug.Log("Setting sign active: " + NextSign);

            NextSign.SetActive(true);

            transform.parent.gameObject.SetActive(false);
        }

        else
        {
            Debug.Log("Transform Parent is null");
        }
    }


    private void EnableButtons()
    {
        foreach (var button in NextPrevButtons)
        {
            button.enabled = true;
        }

    }
    private void DisableButtons()
    {
        foreach (var button in NextPrevButtons)
        {
            button.enabled = false;
        }

    }
}