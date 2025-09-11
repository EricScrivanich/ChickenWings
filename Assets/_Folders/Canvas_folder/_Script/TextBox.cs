using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class TextBox : MonoBehaviour
{
    private CanvasGroup group;
    [SerializeField] private TextMeshProUGUI text;

    [Header("Box Size Settings")]
    [SerializeField] private float baseHeight;
    [SerializeField] private float addedHeightPerLine;

    [Header("Fade Settings")]
    [SerializeField] private bool doFade;
    [SerializeField] private float fadeDur;
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
            Invoke("DoDelay", .08f);
        }
        else
            DoDelay();




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

    private IEnumerator DelayToUpdateBoxSize()
    {
        yield return null;
        int lineCount = text.textInfo.lineCount;
        float newHeight = baseHeight + (Mathf.Max(0, lineCount) * addedHeightPerLine);
        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);



        if (doFade)
            group.DOFade(1, fadeDur);
        else
            group.alpha = 1;

    }
}
