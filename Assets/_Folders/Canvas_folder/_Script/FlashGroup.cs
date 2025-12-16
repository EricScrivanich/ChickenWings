using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;


public class FlashGroup : MonoBehaviour
{

    private Coroutine flashCor;

    private Sequence fadeOutSeq;
    private Sequence enlargeSeq;
    [SerializeField] private TutorialData lvlID;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float fullAlphaAmount;
    [SerializeField] private float lowAlphaAmount;
    [SerializeField] private float fullAlphaTime;
    [SerializeField] private float lowAlphaTime;
    [SerializeField] private float fullAlphaTransitionTime;
    [SerializeField] private float lowAlphaTransitionTime;
    private int addedHeightPerLine = 40;
    private CanvasGroup group;
    private Vector3 startScale = new Vector3(1.55f, 1.55f, 1.55f);

    // Start is called before the first frame update
    private void Awake()
    {
        // lvlID.SetPressButtonText += SetText;

        group = GetComponent<CanvasGroup>();
    }

    public void Initialize(TutorialData t)
    {
        lvlID = t;
        lvlID.SetPressButtonText += SetText;
    }
    private void OnDestroy()
    {
        lvlID.SetPressButtonText -= SetText;
    }

    private void OnEnable()
    {

    }

    public void ShowMessage(string s)
    {

        group.alpha = 0;
        transform.localScale = startScale * 1.4f;
        text.text = s;

        flashCor = StartCoroutine(Flash());
    }


    public void SetText(bool show, int type, string s)
    {
        // if (fadeOutSeq != null && fadeOutSeq.IsPlaying())
        //     fadeOutSeq.Kill();

        // if (enlargeSeq != null && enlargeSeq.IsPlaying())
        //     enlargeSeq.Kill();
        DOTween.Kill(transform);
        DOTween.Kill(group);
        if (show)
        {
            // if (fadeOutSeq != null && fadeOutSeq.IsPlaying())
            //     fadeOutSeq.Kill();

            // if (enlargeSeq != null && enlargeSeq.IsPlaying())
            //     enlargeSeq.Kill();
            Debug.Log("CALLEEDDD");
            transform.localScale = startScale;
            Debug.LogError("Local scale is: " + transform.localScale);
            if (type == 0) text.text = ("Press any button to continue.");
            else if (type == 1) text.text = ("Press " + s + " button to continue.");
            else if (type == 2) text.text = ("Hold the " + s + " button to continue.");
            else if (type == 3) text.text = ("Continue holding the " + s + " button to continue.");

            gameObject.SetActive(true);


            group.DOFade(lowAlphaAmount, .3f).SetEase(Ease.InSine).SetUpdate(true).From(0).OnComplete(() => flashCor = StartCoroutine(Flash()));
            // flashCor = StartCoroutine(Flash());

        }
        else
        {
            FadeOut();

        }



    }


    private void OnDisable()
    {
        if (flashCor != null)
        {
            StopCoroutine(flashCor);
        }

        if (fadeOutSeq != null && fadeOutSeq.IsPlaying())
            fadeOutSeq.Kill();

        if (enlargeSeq != null && enlargeSeq.IsPlaying())
            enlargeSeq.Kill();
    }

    public void FadeOut()
    {
        if (flashCor != null)
        {
            StopCoroutine(flashCor);
        }
        fadeOutSeq = DOTween.Sequence();
        enlargeSeq = DOTween.Sequence();



        // var rect = group.GetComponent<RectTransform
        enlargeSeq.Append(transform.DOScale(startScale * 1.4f, .5f).SetEase(Ease.InOutSine));

        fadeOutSeq.Append(group.DOFade(fullAlphaAmount - .2f, .3f));
        fadeOutSeq.Append(group.DOFade(0, .4f).SetEase(Ease.OutSine));
        enlargeSeq.Play().SetUpdate(true);
        fadeOutSeq.Play().SetUpdate(true).OnComplete(() =>
        {
            Destroy(gameObject);
        });
        // fadeOutSeq.OnComplete(() => gameObject.SetActive(false));
    }

    // Update is called once per frame
    private IEnumerator Flash()
    {
        yield return new WaitForSecondsRealtime(1.3f);
        var r = group.GetComponent<RectTransform>();
        Debug.Log("Line count is: " + text.textInfo.lineCount);
        r.sizeDelta = new Vector2(r.sizeDelta.x, (text.textInfo.lineCount * addedHeightPerLine) + 70);
        float entracneDur = fullAlphaTransitionTime + 1.1f;
        group.DOFade(fullAlphaAmount, entracneDur).SetEase(Ease.InOutSine).SetUpdate(true);
        transform.DOScale(startScale, entracneDur).SetEase(Ease.InOutSine).SetUpdate(true); ;
        yield return new WaitForSecondsRealtime(entracneDur + .1f);

        while (true)
        {
            group.DOFade(lowAlphaAmount, lowAlphaTransitionTime).SetEase(Ease.InSine).SetUpdate(true);
            yield return new WaitForSecondsRealtime(lowAlphaTransitionTime + lowAlphaTime);

            group.DOFade(fullAlphaAmount, fullAlphaTransitionTime).SetEase(Ease.OutSine).SetUpdate(true);
            // group.gameObject.transform.DOScale(startScale * 1.04f, fullAlphaTime + fullAlphaTransitionTime).SetEase(Ease.InOutSine).SetUpdate(true);
            yield return new WaitForSecondsRealtime(fullAlphaTime + fullAlphaTransitionTime);
            // group.gameObject.transform.DOScale(startScale * .96f, lowAlphaTime + lowAlphaTransitionTime).SetEase(Ease.InOutSine).SetUpdate(true);




        }
    }
}
