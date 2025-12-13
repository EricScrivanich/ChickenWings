using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TutorialHand : MonoBehaviour
{

    [SerializeField] private int handID;

    [SerializeField] private Transform parent;
    [SerializeField] private GameObject handPrefab;
    private GameObject handInstance;
    [SerializeField] private float moveAmount;
    [SerializeField] private float moveDuration;
    [SerializeField] private float scale;
    private Sequence handSeq;
    private bool isShowing = false;

    void Awake()
    {
        Destroy(GetComponent<Image>());
    }

    void Start()
    {
        SteroidTutorial.instance.OnShowTutorialHand += ShowHand;
    }
    void OnDisable()
    {
        SteroidTutorial.instance.OnShowTutorialHand -= ShowHand;
    }

    public void Initialize(bool destroy, int id)
    {
        if (destroy)
        {
            Destroy(gameObject);
        }
        else
        {
            handID = id;
           
        }

    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ShowHand(int id, bool destroy)
    {
        if (id == -1)
        {
            Destroy(gameObject);
        }
        else if (id != handID)
        {
            if (isShowing && destroy)
            {
                handSeq.Kill();
                Destroy(handInstance);
                Destroy(gameObject);
            }
            return;
        }

        if (destroy) return;
        isShowing = true;
        handInstance = Instantiate(handPrefab, parent);
        handInstance.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
        handInstance.transform.localRotation = transform.localRotation;
        // handInstance.transform.localScale = Vector3.one * scale;
        var hand = handInstance.GetComponentInChildren<Image>();
        hand.enabled = true;
        handSeq = DOTween.Sequence();
        handSeq.AppendInterval(0.9f);
        handSeq.Append(hand.DOFade(1, 0.7f));
        handSeq.Play().OnComplete(() =>
        {
            handSeq = DOTween.Sequence();
            handSeq.Append(hand.rectTransform.DOLocalMoveY(moveAmount, moveDuration).SetEase(Ease.InOutSine));
            handSeq.Append(hand.rectTransform.DOLocalMoveY(0, moveDuration).SetEase(Ease.InOutSine));
            handSeq.Play().SetLoops(-1);
        }

        );


    }

    void LoopAnim()
    { }
}
