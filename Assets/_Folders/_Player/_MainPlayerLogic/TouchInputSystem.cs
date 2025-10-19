using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for event system interfaces
using DG.Tweening;



public class TouchInputSystem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    private int touchCount;
    private float lastTouchTime;
    private bool trackedDrag = false;
    private bool waitingForCallback;
    private Sequence stuckSeq;
    private Sequence flashStuckSeq;
    private Image joystickImage;

    [SerializeField] private CanvasGroup joystickGroup;


    [SerializeField] private float flashDur;
    [SerializeField] private float stuckTweenDur;
    [SerializeField] private float centerEndScale;
    [SerializeField] private Vector3 lineScale;
    private Vector2 initialPressPos;


    [SerializeField] private RectTransform[] scytheLines;
    [SerializeField] private RectTransform scytheCenter;
    public ButtonColorsSO colorSO;
    [SerializeField] private Image chickenImage;

    [SerializeField] private PlayerID player;

    [SerializeField] private RectTransform scytheSwipeRect;

    private bool didTwoFingerTap = false;
    private Coroutine inputDelay;

    private void Start()
    {
        if (scytheSwipeRect != null)
            player.events.OnSetScythePos?.Invoke(scytheSwipeRect.position);
        chickenImage.color = colorSO.normalButtonColorFull;
        joystickImage = scytheCenter.GetComponent<Image>();
        joystickImage.raycastTarget = false;
        joystickGroup.gameObject.SetActive(false);
        // joystickGroup.interactable = false;
        joystickGroup.alpha = 0;
    }

    private void HandleStuckScythe(bool stuck)
    {
        joystickImage.raycastTarget = stuck;
        StuckInitialTween(stuck);
        FlashTween(stuck);




    }
    private void FlashTween(bool stuck)
    {
        if (flashStuckSeq != null && flashStuckSeq.IsPlaying())
            flashStuckSeq.Kill();
        flashStuckSeq = DOTween.Sequence();

        if (stuck)
        {
            flashStuckSeq.Append(chickenImage.DOColor(colorSO.WeaponColor, flashDur).From(colorSO.normalButtonColorFull));
            flashStuckSeq.Append(chickenImage.DOColor(colorSO.normalButtonColorFull, flashDur));
            flashStuckSeq.Play().SetLoops(-1);
        }
        else
        {
            flashStuckSeq.Append(chickenImage.DOColor(colorSO.normalButtonColorFull, flashDur));
            flashStuckSeq.Play();
        }
    }
    private void ShowScytheLines(bool show)
    {
        scytheSwipeRect.gameObject.SetActive(show);
    }
    private void StuckInitialTween(bool stuck)
    {
        if (stuckSeq != null && stuckSeq.IsPlaying())
            stuckSeq.Complete();
        stuckSeq = DOTween.Sequence();
        if (stuck)
        {
            joystickGroup.gameObject.SetActive(true);
            stuckSeq.Append(joystickGroup.DOFade(1, stuckTweenDur));
            // stuckSeq.Append(scytheCenter.DOScale(centerEndScale, stuckTweenDur));

            // foreach (var l in scytheLines)
            // {
            //     stuckSeq.Join(l.DOScale(lineScale, stuckTweenDur));
            // }
            stuckSeq.Play();
        }
        else
        {
            // joystickGroup.gameObject.SetActive(true);
            stuckSeq.Append(joystickGroup.DOFade(0, stuckTweenDur));
            // stuckSeq.Append(scytheCenter.DOScale(1, stuckTweenDur));

            // foreach (var l in scytheLines)
            // {
            //     stuckSeq.Join(l.DOScale(1, stuckTweenDur));
            // }
            stuckSeq.Play().OnComplete(() => joystickGroup.gameObject.SetActive(false));

        }
    }
    private void OnEnable()
    {
        player.UiEvents.StuckPigScythe += HandleStuckScythe;
        player.UiEvents.OnShowSyctheLine += ShowScytheLines;

    }
    private void OnDisable()
    {
        player.UiEvents.StuckPigScythe -= HandleStuckScythe;
        player.UiEvents.OnShowSyctheLine -= ShowScytheLines;


    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchCount++;
        Debug.Log("Touch Count is: " + touchCount);
        initialPressPos = eventData.position;


        if (touchCount == 2 && Time.time - lastTouchTime < .06f)
        {
            player.events.TwoFingerCenterTouch?.Invoke();
            didTwoFingerTap = true;
            return;

        }
        else
        {
            lastTouchTime = Time.time;
        }
        if (waitingForCallback) StopCoroutine(inputDelay);


        inputDelay = StartCoroutine(CallTouchCenter(eventData.position));




    }
    public void OnPointerUp(PointerEventData eventData)
    {
        touchCount--;
        if (touchCount == 0 && waitingForCallback)
        {
            waitingForCallback = false;

            StopCoroutine(inputDelay);

            StartCoroutine(CallReleaseCenter(eventData.position));

        }







    }
    WaitForEndOfFrame awaiter = new WaitForEndOfFrame();
    WaitForSecondsRealtime twoTapAwaiter = new WaitForSecondsRealtime(.12f);

    private IEnumerator CallTouchCenter(Vector2 pos)
    {
        waitingForCallback = true;
        yield return twoTapAwaiter;

        waitingForCallback = false;

        if (didTwoFingerTap)
            didTwoFingerTap = false;
        else
            player.events.OnPointerCenter?.Invoke(false, pos);


    }
    private IEnumerator CallReleaseCenter(Vector2 pos)
    {
        yield return awaiter;
        player.events.OnPointerCenter?.Invoke(false, pos);
        yield return awaiter;
        player.events.QuickCenterRelease?.Invoke();

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (waitingForCallback)
        {
            waitingForCallback = false;

            StopCoroutine(inputDelay);
            player.events.OnPointerCenter?.Invoke(false, initialPressPos);



        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }

    // private void OnEnable()
    // {
    //     // EnhancedTouchSupport.Enable();

    // }

    // private void OnDisable()
    // {
    //     // EnhancedTouchSupport.Disable();

    // }
}
