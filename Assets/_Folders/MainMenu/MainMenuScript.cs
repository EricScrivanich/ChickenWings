using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuScript : MonoBehaviour
{
    public SceneManagerSO smSO;
    private int currentMenu;
    [SerializeField] private RectTransform Main;
    [SerializeField] private RectTransform Bosses;
    [SerializeField] private RectTransform Levels;
    [SerializeField] private float fallDuration = 1f;
    [SerializeField] private float minGravity = 500f;
    [SerializeField] private float maxGravity = 2000f;
    [SerializeField] private float minRotationSpeed = 10f;
    [SerializeField] private float maxRotationSpeed = 30f;
    [SerializeField] private GameObject coverPanel;
    [SerializeField] private GameObject instanceMat;
    [SerializeField] private Color dayTop;
    [SerializeField] private Color dayBot;


    [SerializeField] private Color eveningTop;
    [SerializeField] private Color eveningBot;

    [SerializeField] private Material skyMaterial;

    [SerializeField] private List<GameObject> environmentLayers;
    [SerializeField] private List<Material> environmentMaterials;

    [SerializeField] private Material enviromentMaterial;



    [SerializeField] private Color Sunset;
    [SerializeField] private Color DayTime;


    private bool isDay;


    [SerializeField] private Image sunGlasses;
    private RectTransform sunGlassCache;
    [SerializeField] private RectTransform sunGlassesStartPos;
    [SerializeField] private RectTransform sunGlassesTargetPos;

    [SerializeField] private RectTransform[] buttonRects;
    [SerializeField] private RectTransform titleRect;
    [SerializeField] private Button[] buttons;

    public bool buttonPressed;

    void Awake()
    {
        isDay = BoundariesManager.isDay;





        buttonPressed = false;
    }

    private void IntitialMenuTween(RectTransform target, float pos, bool doX)
    {
        if (doX)
            target.DOAnchorPosX(pos, 1.5f).SetEase(Ease.OutSine);

        else
            target.DOAnchorPosY(pos, 1.5f).SetEase(Ease.OutSine);



    }

    private IEnumerator InitialTweenCourintine()
    {
        coverPanel.SetActive(true);
        yield return new WaitForSeconds(.1f);
        IntitialMenuTween(titleRect, 0, false);
        foreach (var rect in buttonRects)
        {
            IntitialMenuTween(rect, -373, true);
            yield return new WaitForSeconds(.33f);

        }
        coverPanel.SetActive(false);
    }

    private void Start()
    {


        Main.anchoredPosition = new Vector2(0, 0);
        currentMenu = 0;
        Bosses.anchoredPosition = new Vector2(2700, 0);
        Levels.anchoredPosition = new Vector2(2700, 0);
        titleRect.anchoredPosition = new Vector2(titleRect.anchoredPosition.x, 740);

        foreach (var rect in buttonRects)
        {
            rect.anchoredPosition = new Vector2(-1000, rect.anchoredPosition.y);
        }


        sunGlassCache = sunGlasses.GetComponent<RectTransform>();

        StartCoroutine(InitialTweenCourintine());



        environmentMaterials = new List<Material>();

        if (isDay)
        {
            skyMaterial.SetColor("_GradTopLeftCol", dayTop);
            skyMaterial.SetColor("_GradBotLeftCol", dayBot);
            enviromentMaterial.SetColor("_Color", DayTime);
            foreach (var obj in environmentLayers)
            {
                Material mat = obj.GetComponent<SpriteRenderer>().material;
                environmentMaterials.Add(mat);
                mat.SetColor("_Color", DayTime);
            }
            sunGlassCache.localPosition = sunGlassesTargetPos.localPosition;
        }
        else
        {
            skyMaterial.SetColor("_GradTopLeftCol", eveningTop);
            skyMaterial.SetColor("_GradBotLeftCol", eveningBot);
            enviromentMaterial.SetColor("_Color", Sunset);
            foreach (var obj in environmentLayers)
            {
                Material mat = obj.GetComponent<SpriteRenderer>().material;
                environmentMaterials.Add(mat);
                mat.SetColor("_Color", Sunset);
            }
            sunGlassCache.localPosition = sunGlassesStartPos.localPosition;


        }






    }

    public void SwitchMenu(int switchTo)
    {
        currentMenu = switchTo;
        if (switchTo == 0)
        {
            float initialRightShift = 50f;
            float durationRightShift = 0.6f;  // Duration of the initial right shift
            float finalLeftShiftDuration = 1.6f;  // Duration for moving to the final position

            // First move to the right
            Main.DOAnchorPos(new Vector2(Main.anchoredPosition.x + initialRightShift, Main.anchoredPosition.y), durationRightShift).SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    Levels.DOAnchorPos(new Vector2(0, Levels.anchoredPosition.y), finalLeftShiftDuration).SetEase(Ease.OutSine);
                    // After completing the move to the right, move to the final position to the left
                    Main.DOAnchorPos(new Vector2(-2700, Main.anchoredPosition.y), finalLeftShiftDuration);
                });
        }
        else if (switchTo == 1)
        {
            float initialLeftShift = 50f;
            float durationLeftShift = 0.6f;  // Duration of the initial Left shift
            float finalRightShiftDuration = 1.6f;  // Duration for moving to the final position

            // First move to the Left
            Levels.DOAnchorPos(new Vector2(Levels.anchoredPosition.x - initialLeftShift, Levels.anchoredPosition.y), durationLeftShift).SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    Main.DOAnchorPos(new Vector2(0, Main.anchoredPosition.y), finalRightShiftDuration).SetEase(Ease.OutSine);
                    // After completing the move to the right, move to the final position to the left
                    Levels.DOAnchorPos(new Vector2(2700, Levels.anchoredPosition.y), finalRightShiftDuration);
                });
        }
        else if (switchTo == 2)
        {
            float initialLeftShift = 50f;
            float durationLeftShift = 0.4f;  // Duration of the initial Left shift
            float finalRightShiftDuration = 1.3f;  // Duration for moving to the final position

            // First move to the Left
            Bosses.DOAnchorPos(new Vector2(Bosses.anchoredPosition.x - initialLeftShift, Main.anchoredPosition.y), durationLeftShift)
                .OnComplete(() =>
                {
                    Main.DOAnchorPos(new Vector2(0, Main.anchoredPosition.y), finalRightShiftDuration);
                    // After completing the move to the right, move to the final position to the left
                    Bosses.DOAnchorPos(new Vector2(2700, Bosses.anchoredPosition.y), finalRightShiftDuration);
                });
        }
    }

    public void ChangeTime()
    {
        if (!isDay)
        {
            sunGlassCache.DOLocalMove(sunGlassesTargetPos.localPosition, 3.0f)
        .SetEase(Ease.OutSine);
            StartCoroutine(TransitionEnvironmentColor(Sunset, DayTime, 3));
            StartCoroutine(TransitionSkyColors(eveningTop, eveningBot, dayTop, dayBot, 3));
            isDay = true;
            BoundariesManager.isDay = true;

        }
        else
        {
            sunGlassCache.DOLocalMove(sunGlassesStartPos.localPosition, 3.0f)
        .SetEase(Ease.InSine);
            StartCoroutine(TransitionEnvironmentColor(DayTime, Sunset, 3));
            StartCoroutine(TransitionSkyColors(dayTop, dayBot, eveningTop, eveningBot, 3));
            isDay = false;
            BoundariesManager.isDay = false;



        }
    }

    private IEnumerator TransitionEnvironmentColor(Color startColor, Color endColor, float duration)
    {
        float time = 0;
        coverPanel.SetActive(true);

        while (time < duration)
        {
            // Calculate the interpolated color once per frame
            Color currentColor = Color.Lerp(startColor, endColor, time / duration);
            enviromentMaterial.SetColor("_Color", currentColor);


            foreach (var mat in environmentMaterials)
            {
                // Apply the pre-calculated color to each material
                mat.SetColor("_Color", currentColor);
            }

            // Increment the time by the time elapsed since the last frame
            time += Time.deltaTime;

            // Yield until the next frame
            yield return null;
        }
        enviromentMaterial.SetColor("_Color", endColor);

        // Ensure the final color is set for each material
        foreach (var mat in environmentMaterials)
        {
            mat.SetColor("_Color", endColor);
        }

        yield return new WaitForSeconds(0.4f);
        coverPanel.SetActive(false);
    }


    private IEnumerator TransitionSkyColors(Color startTop, Color startBot, Color endTop, Color endBot, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            // Interpolate top and bottom colors based on the elapsed time
            skyMaterial.SetColor("_GradTopLeftCol", Color.Lerp(startTop, endTop, time / duration));
            skyMaterial.SetColor("_GradBotLeftCol", Color.Lerp(startBot, endBot, time / duration));

            // Increment the time by the time elapsed since the last frame
            time += Time.deltaTime;

            // Yield until the next frame
            yield return null;
        }

        // Ensure the final colors are set
        skyMaterial.SetColor("_GradTopLeftCol", endTop);
        skyMaterial.SetColor("_GradBotLeftCol", endBot);
    }
    public void LoadSceneWithIndex(int sceneIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    public void ImmediateLoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }


    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        buttonPressed = true;
        // Disable button interaction
        foreach (var button in buttons)
        {
            button.interactable = false;
        }

        float timer = 0f;
        Vector3[] originalPositions = new Vector3[buttons.Length];
        float[] gravityValues = new float[buttons.Length];
        float[] rotationSpeeds = new float[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            originalPositions[i] = buttons[i].transform.position;
            gravityValues[i] = Random.Range(minGravity, maxGravity);
            rotationSpeeds[i] = Random.Range(minRotationSpeed, maxRotationSpeed) * (Random.value > 0.5f ? 1f : -1f);
        }

        while (timer < fallDuration)
        {
            timer += Time.deltaTime;

            // Apply gravity and rotation to buttons
            for (int i = 0; i < buttons.Length; i++)
            {
                Vector3 position = originalPositions[i];
                position.y -= gravityValues[i] * Mathf.Pow(timer / fallDuration, 2f);
                buttons[i].transform.position = position;

                buttons[i].transform.Rotate(Vector3.forward, rotationSpeeds[i] * Time.deltaTime);
            }


            yield return null;
        }

        // Load the next scene
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadSceneFromSO(int levelIndex)
    {
        smSO.LoadLevel(levelIndex);
    }

    public void LoadGamemodeFromSO(int levelIndex)
    {
        smSO.LoadGamemode(levelIndex);
    }


    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {

            // When the game is paused by the system (going to background).
            Time.timeScale = 0;
        }
        else
        {

            // When the game returns from background, keep it paused.
            Time.timeScale = FrameRateManager.TargetTimeScale;

        }
    }
}

