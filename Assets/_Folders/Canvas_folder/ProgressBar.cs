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
    private Color fillColor = new Color(.6f, .6f, .6f, .55f);

    [SerializeField] private RectTransform chicken;
    [SerializeField] private Image progress;
    private Image fill;
    [SerializeField] private RectTransform chickenEndPos;

    [SerializeField] private GameObject FinishLineEgg;
    [SerializeField] private Vector3 FinishLineEggSpawnPostion;
    private Image outline;

    private Sequence finishSeq;
    private float actualTime;

    private float timeStamp;
    private float time;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool stopUpdate;
    private bool showFinish = true;

    // Start is called before the first frame update
    void Start()
    {
        fill = GetComponent<Image>();
        outline = gameObject.transform.Find("Outline").GetComponent<Image>();
        progress.fillAmount = 0;
        startPosition = chicken.localPosition;
        endPosition = new Vector3(chickenEndPos.localPosition.x + 20, chickenEndPos.localPosition.y, 0);

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
        progress.color = colorSO.normalButtonColorFull;
        fill.color = fillColor;
        outline.color = colorSO.OutLineColor;

    }
    private void Awake()
    {
        levelManagerID.outputEvent.OnGetLevelTime += SetDuration;
        levelManagerID.outputEvent.OnGetLevelTimeNew += SetDurationNew;
        ResetManager.GameOverEvent += OnGameOver;

    }

    private void OnGameOver()
    {
        stopUpdate = true;
    }

    void SetDuration(float d)
    {
        duration = d;
    }
    void SetDurationNew(float total, float starting)
    {
        duration = total;
        actualTime = starting;
        showFinish = false;
    }

    void FinishLevel()
    {
        chicken.GetComponent<Image>().DOFade(0, .4f);
        if (showFinish)
            Instantiate(FinishLineEgg, FinishLineEggSpawnPostion, Quaternion.identity);

        fill.DOColor(colorSO.normalButtonColorFull, .3f).OnComplete(finishedTween);





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
        finishSeq.Append(fill.DOColor(fillColor, .5f).SetEase(Ease.InSine).From(colorSO.normalButtonColorFull));
        finishSeq.Append(fill.DOColor(colorSO.normalButtonColorFull, .6f).SetEase(Ease.OutSine));
        finishSeq.Play().SetLoops(-1);
    }

    private void OnDisable()
    {
        levelManagerID.outputEvent.OnGetLevelTime -= SetDuration;
        levelManagerID.outputEvent.OnGetLevelTimeNew -= SetDurationNew;

        ResetManager.GameOverEvent -= OnGameOver;

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
