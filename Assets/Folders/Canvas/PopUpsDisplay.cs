using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopUpsDisplay : MonoBehaviour
{
    public PlayerID ID;
    [SerializeField] private GameObject Frozen;
    [SerializeField] private GameObject FrozenOverlay;
    private PlayerMovement playerMove;
    [SerializeField] private float lerpTime = 1f;
    private float currentLerpTime = 0f;
    private GameObject player;
    [SerializeField] private Image gameoverImage;
    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private float positionArrivalThreshold = 1f;
    private PlayerManager playerMan;
    private ScoreManager scoreMan;
    [SerializeField] private Material frozenMaterial;
    [SerializeField] private Material frozenOverlayMaterial;

    [SerializeField] private float fadeInDuration = .9f;
    [SerializeField] private float fadeOutDuration = .2f;
    [SerializeField] private float frozenTime = .2f;

    // New variable for fade amount of frozenOverlayMaterial
    [SerializeField] private float frozenOverlayFadeAmount = 0.5f;

    void Start()
    {
        frozenOverlayMaterial.SetFloat("_FadeAmount", 1);
        gameoverImage.gameObject.SetActive(false);
        Frozen.SetActive(false);
        FrozenOverlay.SetActive(false);
        initialPosition = gameoverImage.transform.position;
    }

    void Update()
    {
        // Existing update logic
    }

    void OnEnable()
    {
        ID.globalEvents.Frozen += FrozenEvent;
    }

    void OnDisable()
    {
        ID.globalEvents.Frozen -= FrozenEvent;
    }

    public void GameOver()
    {
        initialPosition = new Vector2(initialPosition.x, Screen.height * 1.5f);
        gameoverImage.transform.position = initialPosition;
        gameoverImage.gameObject.SetActive(true);
        targetPosition = new Vector2(Screen.width / 2, Screen.height / 2);

        currentLerpTime = 0f;
        StartCoroutine(MoveGameOverImage());
    }

    private IEnumerator MoveGameOverImage()
    {
        while (currentLerpTime < lerpTime)
        {
            currentLerpTime += Time.deltaTime;
            float perc = currentLerpTime / lerpTime;
            float smoothT = Mathf.SmoothStep(0, 1, perc);

            gameoverImage.transform.position = Vector2.Lerp(initialPosition, targetPosition, smoothT);

            yield return null;
        }
    }

    private void FrozenEvent()
    {
        StartCoroutine(FadeInFrozenUI());
    }

    private IEnumerator FadeInFrozenUI()
    {
        Frozen.SetActive(true);
        FrozenOverlay.SetActive(true);
        float elapsedTime = 0;
        while (elapsedTime < fadeInDuration)
        {
            float fadeAmount = Mathf.Lerp(1, 0, elapsedTime / fadeInDuration);
            frozenMaterial.SetFloat("_FadeAmount", fadeAmount);
            frozenOverlayMaterial.SetFloat("_FadeAmount", Mathf.Lerp(frozenOverlayFadeAmount, 0, elapsedTime / fadeInDuration));
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
            frozenOverlayMaterial.SetFloat("_FadeAmount", Mathf.Lerp(0, frozenOverlayFadeAmount, elapsedTime / fadeOutDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Frozen.SetActive(false);
    }
}
