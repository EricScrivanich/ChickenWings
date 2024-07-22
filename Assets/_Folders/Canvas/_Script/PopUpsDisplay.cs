using System.Collections;
using UnityEngine;

using DG.Tweening;

public class PopUpsDisplay : MonoBehaviour
{
    public PlayerID ID;
    [SerializeField] private LevelManagerID LvlID;


    [SerializeField] private GameObject Frozen;
    private PlayerMovement playerMove;
    [SerializeField] private float lerpTime = 1f;
    private float currentLerpTime = 0f;
    private GameObject player;

    public RectTransform gameOver;
    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private float positionArrivalThreshold = 1f;
    private PlayerManager playerMan;
    private ScoreManager scoreMan;
    [SerializeField] private Material frozenMaterial;


    [SerializeField] private float fadeInDuration = .9f;
    [SerializeField] private float fadeOutDuration = .2f;
    [SerializeField] private float frozenTime = .2f;

    // New variable for fade amount of frozenOverlayMaterial


    void Start()
    {

        gameOver.gameObject.SetActive(false);
        Frozen.SetActive(false);


    }
    private void GameOver2()
    {
        gameOver.anchoredPosition = new Vector2(0, Screen.height + 30);
        gameOver.DOAnchorPos(new Vector2(0, 10), 1.5f);
    }

    void Update()
    {
        // Existing update logic
    }

    void OnEnable()
    {
        ID.globalEvents.Frozen += FrozenEvent;

        if (LvlID != null)
        {
            LvlID.outputEvent.FinishedLevel += FinishLevel;
        }

    }

    void OnDisable()
    {
        ID.globalEvents.Frozen -= FrozenEvent;
        if (LvlID != null)
        {
            LvlID.outputEvent.FinishedLevel -= FinishLevel;
        }

    }

    public void GameOver()
    {
        gameOver.anchoredPosition = new Vector2(0, Screen.height + 30);
        gameOver.gameObject.SetActive(true);
        gameOver.DOAnchorPos(new Vector2(0, 10), 1.5f);

        // gameoverImage.gameObject.SetActive(true);
        // targetPosition = new Vector2(0, 1);

        // currentLerpTime = 0f;
        // StartCoroutine(MoveGameOverImage());
    }

    // private IEnumerator MoveGameOverImage()
    // {
    //     while (currentLerpTime < lerpTime)
    //     {
    //         currentLerpTime += Time.deltaTime;
    //         float perc = currentLerpTime / lerpTime;
    //         float smoothT = Mathf.SmoothStep(0, 1, perc);

    //         gameoverImage.transform.position = Vector2.Lerp(initialPosition, targetPosition, smoothT);

    //         yield return null;
    //     }
    // }

    private void FrozenEvent()
    {
        StartCoroutine(FadeInFrozenUI());
    }

    private IEnumerator FadeInFrozenUI()
    {
        Frozen.SetActive(true);
        // FrozenOverlay.SetActive(true);
        float elapsedTime = 0;
        while (elapsedTime < fadeInDuration)
        {
            float fadeAmount = Mathf.Lerp(1, 0, elapsedTime / fadeInDuration);
            frozenMaterial.SetFloat("_FadeAmount", fadeAmount);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(frozenTime);
        StartCoroutine(FadeOutFrozenUI());
    }

    private IEnumerator FadeOutFrozenUI()
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeOutDuration)
        {
            float fadeAmount = Mathf.Lerp(0, 1, elapsedTime / fadeOutDuration);
            frozenMaterial.SetFloat("_FadeAmount", fadeAmount);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Frozen.SetActive(false);
    }

    private void FinishLevel()
    {
        Time.timeScale = 0;

    }
}
