using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SignMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Button[] NextPrevButtons;
    [SerializeField] private RectTransform target;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float mainDropDuration;
    [SerializeField] private float RetractDuration;
    [SerializeField] private float RetractBounceDuration;
    [SerializeField] private float RetractBounceAmount;
    [SerializeField] private float overshootDuration;
    [SerializeField] private float overshootAmount;

    private LevelManager LM;
    private Sequence sequence;

    void Start()
    {

        LM = GameObject.Find("LevelManager").GetComponent<LevelManager>();
       
    }

    private void OnEnable() {
        DisableButtons();


        // Ensure the target starts at the startPosition
        target.anchoredPosition = startPosition;

        // Create the main drop tween
        Tweener mainDropTween = target.DOAnchorPos(endPosition, mainDropDuration)
            .SetEase(Ease.Linear)
            .SetUpdate(true);

        // Create the overshoot tween
        Vector3 overshootPosition = endPosition - new Vector3(0, overshootAmount, 0);
        Tweener overshootTween = target.DOAnchorPos(overshootPosition, overshootDuration)
            .SetEase(Ease.OutElastic)
            .SetUpdate(true);

        // Chain the tweens together
        sequence = DOTween.Sequence();
        sequence.Append(mainDropTween)
                .Append(overshootTween)
                 .OnComplete(EnableButtons)
                .SetUpdate(true);
        
    }

    private void OnDisable()
    {
        // Kill the sequence when the GameObject is disabled
        if (sequence != null && sequence.IsActive())
        {
            sequence.Kill();
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

    private void SetUnactive()
    {
        gameObject.SetActive(false);
    }

    public void RetractToNextUI(bool isNext)
    {
        DisableButtons();
        LM.NextUI(isNext);
        Vector3 overshootPosition = endPosition - new Vector3(0, overshootAmount, 0) - new Vector3(0, RetractBounceAmount, 0);
        Tweener overshootTween = target.DOAnchorPos(overshootPosition, RetractBounceDuration)
            .SetEase(Ease.InSine)
            .SetUpdate(true);


        Tweener riseTween = target.DOAnchorPos(startPosition, RetractDuration)
           .SetEase(Ease.InSine)
           .SetUpdate(true);


        sequence = DOTween.Sequence();
        sequence.Append(overshootTween)
                .Append(riseTween)
                .OnComplete(SetUnactive)
                .SetUpdate(true);


    }
}