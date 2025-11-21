using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class ButtonTweenFromSide : MonoBehaviour
{

    [SerializeField] private bool moveX = true;
    [SerializeField] private float moveAmount;
    [SerializeField] private Vector2 normalAnchoredPosition;
    [SerializeField] public PlayerID player;
    [SerializeField] private int type;
    [SerializeField] private float duration;
    private Vector2 normalPosition;


    private bool hasTransitioned = false;

    // 

    void Awake()
    {
        player.UiEvents.OnShowPlayerUI += DoMove;
        var rect = GetComponent<RectTransform>();
        normalPosition = rect.anchoredPosition;

        if (moveX) rect.anchoredPosition = new Vector2(rect.anchoredPosition.x - moveAmount, rect.anchoredPosition.y);
        else rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - moveAmount);
        // gameObject.SetActive(false);

    }
    void OnDestroy()
    {
        player.UiEvents.OnShowPlayerUI -= DoMove;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // public void DoMove(bool show, int t, float mult)
    // {
    //     if (hasTransitioned) return;
    //     if (type <= t)
    //     {
    //         float dur = duration;
    //         if (mult != 0) dur *= mult;
    //         hasTransitioned = true;
    //         var rect = GetComponent<RectTransform>();
    //         Vector2 target = normalAnchoredPosition;


    //         Debug.LogError("Doing Move for: " + gameObject.name + " to position: " + target);
    //         normalAnchoredPosition = rect.anchoredPosition;

    //         rect.DOAnchorPos(target, dur).SetEase(Ease.OutBack);

    //     }

    // }

    public void DoMove(bool show, int t, float mult)
    {
        if (hasTransitioned) return;
        if (type <= t)
        {
            float dur = duration;
            if (mult != 0) dur *= mult;
            else dur *= 1.2f;
            hasTransitioned = true;
            var rect = GetComponent<RectTransform>();

            gameObject.SetActive(true);

            Sequence seq = Sequence.Create(useUnscaledTime: true)
            .ChainDelay(.9f)
            .Chain(Tween.UIAnchoredPosition(rect, normalPosition, dur, Ease.OutBack));

            // rect.DOAnchorPos(normalPosition, dur).SetEase(Ease.OutBack).SetUpdate(true);

        }

    }

    // #if UNITY_EDITOR
    //     public bool setInitialPos;
    //     public bool resetToInitial;
    //     public float adjustXStartAmount;
    //     public float adjustYStartAmount;
    //     public bool adjustStart;
    //     public bool adjustCurrent;

    //     void OnValidate()
    //     {
    //         if (setInitialPos)
    //         {
    //             normalAnchoredPosition = GetComponent<RectTransform>().anchoredPosition;
    //             setInitialPos = false;
    //         }
    //         if (resetToInitial)
    //         {
    //             GetComponent<RectTransform>().anchoredPosition = normalAnchoredPosition;
    //             resetToInitial = false;
    //         }
    //         if (adjustStart)
    //         {
    //             var rect = GetComponent<RectTransform>();
    //             if (rect.anchoredPosition.x < 0) adjustXStartAmount = -adjustXStartAmount;
    //             normalAnchoredPosition = new Vector2(normalAnchoredPosition.x + adjustXStartAmount, normalAnchoredPosition.y + adjustYStartAmount);
    //             adjustStart = false;
    //             adjustXStartAmount = 0;
    //             adjustYStartAmount = 0;
    //         }
    //         if (adjustCurrent)
    //         {
    //             var rect = GetComponent<RectTransform>();
    //             if (rect.anchoredPosition.x < 0) adjustXStartAmount = -adjustXStartAmount;
    //             rect.anchoredPosition = new Vector2(normalAnchoredPosition.x + adjustXStartAmount, normalAnchoredPosition.y + adjustYStartAmount);
    //             adjustCurrent = false;
    //             adjustXStartAmount = 0;
    //             adjustYStartAmount = 0;
    //         }
    //     }

    // #endif

    // Update is called once per frame

}
