using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class LifeDisplay : MonoBehaviour
{
    [SerializeField] private bool infiniteLives;
    [SerializeField] private RectTransform infinitySymbol;
    public PlayerID player;
    [SerializeField] private float spriteSwitchDelay;

    [SerializeField] private GameObject eggPrefab;

    private Coroutine loseLifeRoutine;
    private Coroutine gainLifeRoutine;
    private int lives;
    private Canvas canvas;
    private bool isOverlay = false;
    private GameObject lastBrokenEgg;


    private Image[] eggImages;
    [SerializeField] private Sprite[] eggSprites;
    [SerializeField] private float baseWidth;
    [SerializeField] private float addedWidthPerEgg;

    [SerializeField] private float baseSpacing = -100;
    [SerializeField] private float spacingPerLife = 15;









    public Vector2 ReturnEggPosition()
    {
        return lastBrokenEgg.transform.position;

        // if (isOverlay)
        // {
        //     // Assuming you want to get the world position for non-UI interaction
        //     // Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, lastBrokenEgg.transform.position);
        //     // return canvas.worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, canvas.worldCamera.nearClipPlane));

        //     Vector2 pos = Camera.main.ScreenToWorldPoint(lastBrokenEgg.transform.position);
        //     return pos;
        // }
        // else
        // {
        //     return lastBrokenEgg.transform.position;

        // }

    }

    // private void Start()
    // {
    //     canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

    //     if (canvas != null)
    //     {
    //         if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
    //         {
    //             isOverlay = true;
    //         }
    //         else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
    //         {
    //             isOverlay = false;
    //         }
    //         else
    //         {
    //             Debug.Log("Canvas render mode is not Screen Space - Overlay or Screen Space - Camera");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("Canvas component not found on GameObject 'Canvas'");
    //     }



    //     // InitializeEggs(startingLives);
    // }



    public void SetInfiniteLives(bool isInfinite)
    {
        infiniteLives = isInfinite;
        player.infiniteLives = infiniteLives;

    }

    private void OnDestroy()
    {


    }



    private void InitializeEggs(int l)
    {
        player.infiniteLives = infiniteLives;
        lives = l;

        // player.Lives = startingLives;


        float w = baseWidth + (lives * addedWidthPerEgg);
        float s = baseSpacing + (lives * spacingPerLife);
        eggImages = new Image[lives];
        GetComponent<RectTransform>().sizeDelta = new Vector2(w, GetComponent<RectTransform>().sizeDelta.y);

        GetComponent<HorizontalLayoutGroup>().spacing = s;

        for (int i = 0; i < lives; i++)
        {
            var o = Instantiate(eggPrefab, transform).GetComponent<Image>();
            eggImages[i] = o;

        }

    }


    void UpdateLives(int newLives)
    {
        Debug.LogError("Updating lives: " + newLives + " current: " + lives);
        if (infiniteLives)
        {
            Debug.Log("Infinite");
            return;
        }
        // Check if gained a life
        if (newLives > lives)
        {
            if (lives < player.startingLives)
            {

                if (loseLifeRoutine != null)
                    StopCoroutine(loseLifeRoutine);
                gainLifeRoutine = StartCoroutine(UpdateSpritesOnGainLife(lives));

            }
            else
            {
                return;

            }
            // Find the most recently broken egg (if any)

        }
        else if (newLives < lives && lives > 0) // Check if lost a life
        {
            Debug.Log("Life should be lost");
            int livesLost = lives - newLives;
            // eggAnimators[newLives].SetBool("IsBrokenBool", true);
            if (gainLifeRoutine != null)
                StopCoroutine(gainLifeRoutine);
            loseLifeRoutine = StartCoroutine(UpdateSpritesOnLoseLife(newLives));
            eggImages[newLives].rectTransform.DOShakeRotation(.5f, 70, 15, 50, true, ShakeRandomnessMode.Harmonic);
            lastBrokenEgg = eggImages[newLives].gameObject;


        }

        lives = newLives; // Update the current lives count
    }


    private IEnumerator UpdateSpritesOnLoseLife(int lifeNumber)
    {

        Debug.LogError("Updating sprites on lose life: " + lifeNumber);
        for (int i = 0; i < eggSprites.Length; i++)
        {
            eggImages[lifeNumber].sprite = eggSprites[i];
            yield return new WaitForSeconds(spriteSwitchDelay);

        }
    }

    private IEnumerator UpdateSpritesOnGainLife(int lifeNumber)
    {

        Debug.LogError("Updating sprites on gain life: " + lifeNumber);


        for (int i = eggSprites.Length - 1; i >= 0; i--)
        {
            eggImages[lifeNumber].sprite = eggSprites[i];
            yield return new WaitForSeconds(spriteSwitchDelay);

        }




    }




    void InfiniteLivesAnim()
    {

        if (infinitySymbol != null)
        {

            Sequence sequence = DOTween.Sequence();
            sequence.Append(infinitySymbol.DOAnchorPosY(infinitySymbol.anchoredPosition.y + 20, .2f).SetEase(Ease.OutSine));

            sequence.Append(infinitySymbol.DORotate(new Vector3(0, 0, 10), 0.15f)
          .SetEase(Ease.InOutSine)
          .SetLoops(5, LoopType.Yoyo)); // Loops the rotation back and forth

            sequence.Append(infinitySymbol.DOAnchorPosY(infinitySymbol.anchoredPosition.y, .2f).SetEase(Ease.OutSine));
            sequence.Join(infinitySymbol.DORotate(new Vector3(0, 0, 0), 0.2f));

            // Play the sequence
            sequence.Play();
        }

    }

    private void OnEnable()
    {
        player.globalEvents.OnInfiniteLives += InfiniteLivesAnim;
        player.globalEvents.OnUpdateLives += UpdateLives;
        player.UiEvents.OnSetStartingLives += InitializeEggs;


    }

    private void OnDisable()
    {
        player.globalEvents.OnInfiniteLives -= InfiniteLivesAnim;
        player.globalEvents.OnUpdateLives -= UpdateLives;
        player.UiEvents.OnSetStartingLives -= InitializeEggs;

    }


}
