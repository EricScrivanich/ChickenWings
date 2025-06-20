using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    private Sequence loadingTextSequence;
    private CanvasGroup canvasGroup;



    public void StartLoadingTextSeq()
    {
        gameObject.SetActive(true);
        canvasGroup = GetComponent<CanvasGroup>();
        loadingTextSequence = DOTween.Sequence();
        loadingTextSequence.Append(loadingText.DOFade(1, 0.4f))
            .AppendInterval(0.4f)
            .Append(loadingText.DOFade(0, 0.4f));


        loadingTextSequence.Play().SetUpdate(true).SetLoops(-1, LoopType.Restart);
    }

    public void DestroyImmediately()
    {
        if (loadingTextSequence != null && loadingTextSequence.IsActive())
        {
            loadingTextSequence.Kill();
        }
        Destroy(gameObject);
    }

    public void SetLoadingBarValue(float value)
    {
        Debug.Log("Setting loading bar value to: " + value);
        loadingBar.fillAmount = value;
    }

    public void StopLoad()
    {
        GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetUpdate(true).OnComplete(() =>
                {

                    if (loadingTextSequence != null && loadingTextSequence.IsActive())
                    {
                        loadingTextSequence.Kill();
                    }

                    Destroy(gameObject);
                });
    }

    private void OnDestroy()
    {
        if (loadingTextSequence != null && loadingTextSequence.IsActive())
        {
            loadingTextSequence.Kill();
        }
        if (canvasGroup != null)
            DOTween.Kill(canvasGroup);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
