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
    [SerializeField] private bool useOldSizeLogic = false;
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
        text.ForceMeshUpdate();

        if (!gameObject.activeInHierarchy)
        {
            Debug.Log("SetText called with: " + t);
            gameObject.SetActive(true);
            if (useOldSizeLogic) // corutine wont start since object unactive
            {
                Invoke("DoDelay", .08f);
            }
            else
                DoDelay();

        }
        else
        {
            Debug.Log("SetText called active with: " + t);
            DoDelay();
        }





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

        FinishFadeOut();
    }

    public void FadeOut()
    {
        if (flashSeq != null && flashSeq.IsPlaying())
            flashSeq.Kill();

        if (doFade)
            group.DOFade(0, fadeDur).SetUpdate(true).OnComplete(() => FinishFadeOut());
        else
            FinishFadeOut();
    }

    private void FinishTyping()
    {
        SetAnim(false);
    }

    public void FinishFadeOut()
    {
        group.alpha = 0;
        textAnimator.enabled = false;
        gameObject.SetActive(false);
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
        text.ForceMeshUpdate();

        if (useOldSizeLogic)
        {
            int lineCount = text.textInfo.lineCount;
            float newHeight = baseHeight + (Mathf.Max(0, lineCount) * addedHeightPerLine);
            var rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);
        }




        if (doFade)
        {
            yield return new WaitForSecondsRealtime(.6f);
            group.DOFade(1, fadeDur).SetUpdate(true).OnComplete(() => Flash());
        }

        else
        {
            group.alpha = 1;
            Debug.Log("Flash called directly");
        }


    }
}
