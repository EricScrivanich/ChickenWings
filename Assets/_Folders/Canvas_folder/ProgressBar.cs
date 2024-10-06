using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    // public Slider slider;
    [SerializeField] private float duration;
    [SerializeField] private LevelManagerID levelManagerID;
    public ButtonColorsSO colorSO;

    [SerializeField] private RectTransform chicken;
    [SerializeField] private Image progress;
    private Image fill;
    [SerializeField] private RectTransform chickenEndPos;

    [SerializeField] private GameObject FinishLineEgg;
    [SerializeField] private Vector3 FinishLineEggSpawnPostion;

    private Sequence finishSeq;
    private float actualTime;

    private float timeStamp;
    private float time;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool stopUpdate;

    // Start is called before the first frame update
    void Start()
    {
        fill = GetComponent<Image>();
        progress.fillAmount = 0;
        startPosition = chicken.localPosition;
        endPosition = new Vector3(chickenEndPos.localPosition.x, chickenEndPos.localPosition.y, 0);
        if (duration > 100)
            timeStamp = .12f;
        else if (duration > 80)
            timeStamp = .09f;
        else if (duration > 60)
            timeStamp = .06f;
        else if (duration > 40)
            timeStamp = .04f;
        // slider = GetComponent<Slider>();
        // slider.value = 0;
        // StartCoroutine(DoSlider());

        // progress.DOFillAmount(1, duration).From(0);
        // chicken.DOLocalMoveX(chickenEndPos.localPosition.x, duration).OnComplete(FinishLevel);
        // StartCoroutine(LerpTimer());
        progress.color = colorSO.DashImageManaHighlight;

    }
    private void Awake()
    {
        levelManagerID.outputEvent.OnGetLevelTime += SetDuration;

    }

    void SetDuration(float d)
    {
        duration = d;
    }

    void FinishLevel()
    {
        chicken.GetComponent<Image>().DOFade(0, .4f);
        Instantiate(FinishLineEgg, FinishLineEggSpawnPostion, Quaternion.identity);

        fill.DOColor(colorSO.DashImageManaHighlight, .3f).OnComplete(finishedTween);





    }

    private void Update()
    {
        if (stopUpdate) return;
        actualTime += Time.deltaTime;
        time += Time.deltaTime;

        if (actualTime > duration && !stopUpdate)
        {

            FinishLevel();
            stopUpdate = true;


        }

        if (time > timeStamp)
        {
            chicken.localPosition = Vector3.Lerp(startPosition, endPosition, actualTime / duration);
            progress.fillAmount = Mathf.Lerp(0, 1, actualTime / duration);
            time = 0;
        }




    }



    IEnumerator LerpTimer()
    {
        float time = 0;
        float timeStamp = .02f;




        progress.fillAmount = 0;
        Vector3 startPosition = chicken.localPosition;
        Vector3 endPosition = new Vector3(chickenEndPos.localPosition.x, chickenEndPos.localPosition.y, 0);

        while (time < duration)
        {

            chicken.localPosition = Vector3.Lerp(startPosition, endPosition, time / duration);
            progress.fillAmount = Mathf.Lerp(0, 1, time / duration);
            yield return new WaitForSeconds(timeStamp);
            time += timeStamp;



        }
        chicken.localPosition = endPosition; // Ensure it reaches the final position
        FinishLevel();
    }

    private void finishedTween()
    {
        if (finishSeq != null && finishSeq.IsPlaying())
            finishSeq.Kill();

        finishSeq = DOTween.Sequence();
        finishSeq.Append(fill.DOColor(colorSO.normalButtonColor, .5f).SetEase(Ease.InSine).From(colorSO.DashImageManaHighlight));
        finishSeq.Append(fill.DOColor(colorSO.DashImageManaHighlight, .6f).SetEase(Ease.OutSine));
        finishSeq.Play().SetLoops(-1);
    }

    private void OnDisable()
    {
        levelManagerID.outputEvent.OnGetLevelTime -= SetDuration;
        if (finishSeq != null && finishSeq.IsPlaying())
            finishSeq.Kill();

        DOTween.Kill(this);
    }

    // Update is called once per frame



    // private IEnumerator DoSlider()
    // {
    //     float time = 0;

    //     while (time < duration)
    //     {
    //         time += Time.deltaTime;
    //         slider.value = Mathf.Lerp(0, 1, time / duration);
    //         yield return null;
    //     }

    // }
}
