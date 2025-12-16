using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using Febucci.TextAnimatorForUnity.TextMeshPro;
using Febucci.UI;



public class TextBox : MonoBehaviour
{
    private CanvasGroup group;
    [SerializeField] private Animator anim;

    [SerializeField] private TextAnimator_TMP textAnimator;
    [SerializeField] private TextMeshProUGUI text;
    // [SerializeField] private TextAnimator text

    [Header("Box Size Settings")]
    [SerializeField] private float baseHeight;
    [SerializeField] private float addedHeightPerLine;

    [Header("Fade Settings")]
    [SerializeField] private bool doFade;
    [SerializeField] private bool doFlash;
    [SerializeField] private float flashAlpha;
    [SerializeField] private float flashDurIn;
    [SerializeField] private float flashDurOut;
    [SerializeField] private float flashDurInterval;
    [SerializeField] private float fadeDur;
    private Sequence flashSeq;

    [SerializeField] private bool updateText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
    }

    // Update is called once per frame


    public void SetText(string t)
    {
        text.text = t;
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            // Invoke("DoDelay", .08f);
            DoDelay();

        }
        else
            DoDelay();




    }

    public void SetAnim(bool play)
    {
        if (anim != null)
            anim.SetBool("Talking", play);

    }

    void OnValidate()
    {
        if (updateText)
        {
            updateText = false;
            SetText(text.text);
        }
    }

    private void DoDelay()
    {
        StartCoroutine(DelayToUpdateBoxSize());
    }
    void OnDisable()
    {
        if (doFade)
            DOTween.Kill(group);
    }

    public void FadeOut()
    {
        if (flashSeq != null && flashSeq.IsPlaying())
            flashSeq.Kill();

        if (doFade)
            group.DOFade(0, fadeDur).SetUpdate(true);
        else
            group.alpha = 0;
    }

    private void FinishTyping()
    {
        SetAnim(false);
    }

    private void Flash()
    {
        if (textAnimator != null)
        {
            textAnimator.enabled = true;


        }


        SetAnim(true);
        if (doFlash)
        {
            if (flashSeq != null && flashSeq.IsPlaying())
                flashSeq.Kill();

            flashSeq = DOTween.Sequence();
            flashSeq.AppendInterval(flashDurInterval);
            flashSeq.Append(group.DOFade(flashAlpha, flashDurIn));
            flashSeq.Append(group.DOFade(1, flashDurOut));

            flashSeq.Play().SetLoops(-1).SetUpdate(true);
        }

    }

    private IEnumerator DelayToUpdateBoxSize()
    {
        yield return null;
        int lineCount = text.textInfo.lineCount;
        float newHeight = baseHeight + (Mathf.Max(0, lineCount) * addedHeightPerLine);
        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);



        if (doFade)
        {
            yield return new WaitForSecondsRealtime(.8f);
            group.DOFade(1, fadeDur).SetUpdate(true).OnComplete(() => Flash());
        }

        else
            group.alpha = 1;

    }
}
