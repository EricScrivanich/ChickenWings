using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeInUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup FadeGroup;
    
    [SerializeField] private List<GameObject> SetActiveObjects;

    [SerializeField] private float startAlpha;
    [SerializeField] private float endAlpha;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float setObjectActiveDelay;


    [SerializeField] private Image flashImage;
    [SerializeField] private Color flashImageColor1;
    [SerializeField] private Color flashImageColor2;
    [SerializeField] private float flashImageDurataion;



    private Sequence sequence;

    // Start is called before the first frame update
    void Start()
    {


    }

    private IEnumerator SetObjectsActive()
    {
        yield return new WaitForSecondsRealtime(setObjectActiveDelay);
        foreach (var item in SetActiveObjects)
        {
            item.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDisable() {

        
    }



    private void OnEnable()
    {

        FadeGroup.DOFade(endAlpha, fadeDuration).SetEase(Ease.InOutSine).From(startAlpha).SetUpdate(true);
        flashImage.CrossFadeColor(flashImageColor1, flashImageDurataion,true,false);
        StartCoroutine(SetObjectsActive());
    }
}
