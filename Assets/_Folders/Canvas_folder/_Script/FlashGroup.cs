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
    [SerializeField] private LevelManagerID lvlID;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float fullAlphaAmount;
    [SerializeField] private float lowAlphaAmount;
    [SerializeField] private float fullAlphaTime;
    [SerializeField] private float lowAlphaTime;
    [SerializeField] private float fullAlphaTransitionTime;
    [SerializeField] private float lowAlphaTransitionTime;
    private CanvasGroup group;
    private Vector3 startScale = new Vector3(1.55f, 1.55f, 1.55f);

    // Start is called before the first frame update
    private void Awake()
    {
        lvlID.outputEvent.SetPressButtonText += SetText;

        group = GetComponent<CanvasGroup>();
    }
    private void OnDestroy()
    {
        lvlID.outputEvent.SetPressButtonText -= SetText;
    }

    private void OnEnable()
    {

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
        fadeOutSeq.Play().SetUpdate(true);
        // fadeOutSeq.OnComplete(() => gameObject.SetActive(false));
    }

    // Update is called once per frame
    private IEnumerator Flash()
    {
        group.alpha = lowAlphaAmount;

        while (true)
        {

            group.DOFade(fullAlphaAmount, fullAlphaTransitionTime).SetEase(Ease.OutSine).SetUpdate(true);
            // group.gameObject.transform.DOScale(startScale * 1.04f, fullAlphaTime + fullAlphaTransitionTime).SetEase(Ease.InOutSine).SetUpdate(true);
            yield return new WaitForSecondsRealtime(fullAlphaTime + fullAlphaTransitionTime);
            // group.gameObject.transform.DOScale(startScale * .96f, lowAlphaTime + lowAlphaTransitionTime).SetEase(Ease.InOutSine).SetUpdate(true);
            group.DOFade(lowAlphaAmount, lowAlphaTransitionTime).SetEase(Ease.InSine).SetUpdate(true);
            yield return new WaitForSecondsRealtime(lowAlphaTransitionTime + lowAlphaTime);



        }
    }
}
