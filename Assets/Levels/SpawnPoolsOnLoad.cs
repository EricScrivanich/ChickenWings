using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpawnPoolsOnLoad : MonoBehaviour
{
    [SerializeField] private GameObject[] SetActiveOnLoaded;
    [SerializeField] private Canvas Overlay;
    [SerializeField] private RectTransform[] UiElementOverlay;
    [SerializeField] private Canvas Screen;

    [SerializeField] private RectTransform[] UiElementScreen;
    private bool doUiSeq = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        // TransitionDirector.instance.OnLoadNewScene += Initialize;
        // TransitionDirector.instance.OnHandleGameUI += HandleUI;
        // TransitionDirector.instance.OnDoUITween += HandleUITween;
        // for (int i = 0; i < UiElementOverlay.Length; i++)
        // {
        //     UiElementOverlay[i].gameObject.SetActive(false);
        // }
        // for (int i = 0; i < UiElementScreen.Length; i++)
        // {
        //     UiElementScreen[i].gameObject.SetActive(false);
        // }
    }
    void OnDisable()
    {
        // TransitionDirector.instance.OnLoadNewScene -= Initialize;
        // TransitionDirector.instance.OnHandleGameUI -= HandleUI;
        // TransitionDirector.instance.OnDoUITween -= HandleUITween;

    }
    void HandleUI(bool hide)
    {
        if (hide)
        {
            doUiSeq = true;
            for (int i = 0; i < UiElementOverlay.Length; i++)
            {
                UiElementOverlay[i].gameObject.SetActive(false);
            }
        }

    }
    void Initialize()
    {
        GetComponent<SmokeTrailPool>().enabled = true;
        GetComponent<SpawnStateManager>().enabled = true;

        for (int i = 0; i < SetActiveOnLoaded.Length; i++)
            SetActiveOnLoaded[i].SetActive(true);

    }
    void HandleUITween(float dur, Ease ease, float amount)
    {
        float a1 = -WorldToScreenDeltaY(amount, Overlay.scaleFactor);
        float a2 = -WorldToScreenDeltaY(amount, Screen.scaleFactor);
        for (int i = 0; i < UiElementOverlay.Length; i++)
        {
            var rect = UiElementOverlay[i];
            float originalY = rect.anchoredPosition.y;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, originalY - a1);
            rect.gameObject.SetActive(true);
            rect.DOAnchorPosY(originalY, dur).SetEase(ease);

        }
        for (int i = 0; i < UiElementScreen.Length; i++)
        {
            var rect = UiElementScreen[i];
            float originalY = rect.anchoredPosition.y;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, originalY - a2);
            rect.gameObject.SetActive(true);
            rect.DOAnchorPosY(originalY, dur).SetEase(ease);

        }
    }

    float WorldToScreenDeltaY(float worldDeltaY, float canvasScale = 1f)
    {
        var cam = Camera.main;
        var a = cam.WorldToScreenPoint(new Vector3(0f, 0f, 0f));
        var b = cam.WorldToScreenPoint(new Vector3(0f, worldDeltaY, 0f));

        return -(b.y - a.y) / canvasScale;

    }

    // Update is called once per frame

}
