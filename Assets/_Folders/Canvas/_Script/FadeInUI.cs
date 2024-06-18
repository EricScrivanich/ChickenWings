using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeInUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup FadeGroup;

    [SerializeField] private List<GameObject> SetActiveObjects;
    [SerializeField] private List<GameObject> SetUnactiveObjects;

    [SerializeField] private float startAlpha;
    [SerializeField] private float endAlpha;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float setObjectActiveDelay;

    private bool hasFaded;


    [SerializeField] private Image flashImage;
    [SerializeField] private Color flashImageColor1;
    [SerializeField] private Color flashImageColor2;
    [SerializeField] private float flashImageDurataion;



    private Sequence sequence;

    // Start is called before the first frame update


    private IEnumerator SetObjectsActive()
    {
        yield return new WaitForSecondsRealtime(setObjectActiveDelay);
        foreach (var item in SetActiveObjects)
        {
            item.SetActive(true);
        }
    }

    private void SetObjectUnactive()
    {

    }

    // Update is called once per frame


    private void OnEnable()
    {
        hasFaded = false;
        foreach (var item in SetActiveObjects)
        {
            item.SetActive(false);
        }
        foreach (var item in SetUnactiveObjects)
        {
            item.SetActive(false);
        }

        FadeGroup.DOFade(endAlpha, fadeDuration).SetEase(Ease.InOutSine).From(startAlpha).SetUpdate(true);

        StartCoroutine(SetObjectsActive());
    }

    public void FadeOut()
    {
        if (!hasFaded)
        {
            hasFaded = true;
            FadeGroup.DOFade(startAlpha, fadeDuration / 2).SetEase(Ease.InOutSine).From(endAlpha).SetUpdate(true).SetUpdate(true);

        }

    }


}