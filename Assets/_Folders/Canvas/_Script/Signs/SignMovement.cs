using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SignMovement : MonoBehaviour
{
    [SerializeField] private FadeInUI uiFade;
    [SerializeField] private bool doesDrop;
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
    private bool hasRetracted = false;

    private LevelManager LM;
    private Sequence sequence;

    void Start()
    {

        LM = GameObject.Find("LevelManager").GetComponent<LevelManager>();

    }

    private void DropSignTween()
    {

        hasRetracted = false;
        target.anchoredPosition = startPosition;
        sequence = DOTween.Sequence();
        // Create the main drop tween

        sequence.Append(target.DOAnchorPos(endPosition, mainDropDuration).SetEase(Ease.OutBack));
        // sequence.Append(target.DOAnchorPos(endPosition, overshootDuration).SetEase(Ease.OutBounce));



        // Create the overshoot tween


        // Chain the tweens together

        sequence.Play().SetUpdate(true).OnComplete(EnableButtons);

    }

    private void OnEnable()
    {
        DisableButtons();
        hasRetracted = false;

        if (doesDrop)
        {
            DropSignTween();
        }


        // Ensure the target starts at the startPosition


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
        if (NextPrevButtons == null || NextPrevButtons.Length == 0) return;
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
        if (LM == null || hasRetracted || !this.gameObject.activeInHierarchy)
        {
            return;
        }
        else
        {
            hasRetracted = true;
        }

        Debug.Log("Retracting with this: " + this.gameObject);
        DisableButtons();
        if (uiFade != null)
        {
            uiFade.FadeOut();
        }




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

    public void RetractToNextUILocal(bool isNext)
    {
        if (LM == null || hasRetracted || !this.gameObject.activeInHierarchy)
        {
            return;
        }
        else
        {
            hasRetracted = true;
        }

        Debug.Log("Retracting with this: " + this.gameObject);
        DisableButtons();
        if (uiFade != null)
        {
            uiFade.FadeOut();
        }







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