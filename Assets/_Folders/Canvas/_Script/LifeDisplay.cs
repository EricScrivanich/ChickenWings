using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class LifeDisplay : MonoBehaviour
{
    [SerializeField] private bool infiniteLives;
    [SerializeField] private RectTransform infinitySymbol;
    [SerializeField] private Transform livesPanel; // Reference to the LivesPanel
    public PlayerID player;
    private int lives;
    private Canvas canvas;
    private bool isOverlay;
    private GameObject lastBrokenEgg;
    [SerializeField] private List<Animator> eggAnimators; // List to store the Animator components of the eggs



    private void Awake()
    {

        player.infiniteLives = infiniteLives;
        lives = player.Lives;






        // InitializeEggAnimators();

    }


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
    private void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        if (canvas != null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                isOverlay = true;
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                isOverlay = false;
            }
            else
            {
                Debug.Log("Canvas render mode is not Screen Space - Overlay or Screen Space - Camera");
            }
        }
        else
        {
            Debug.LogError("Canvas component not found on GameObject 'Canvas'");
        }
    }

    public void SetInfiniteLives(bool isInfinite)
    {
        infiniteLives = isInfinite;
        player.infiniteLives = infiniteLives;

    }

    private void OnDestroy()
    {


    }


    void UpdateLives(int newLives)
    {
        if (infiniteLives)
        {
            return;
        }
        // Check if gained a life
        if (newLives > lives)
        {
            if (lives <= 2)
            {

                eggAnimators[lives].SetBool("IsBrokenBool", false);

            }
            else
            {
                return;

            }
            // Find the most recently broken egg (if any)

        }
        else if (newLives < lives) // Check if lost a life
        {
            int livesLost = lives - newLives;
            eggAnimators[newLives].SetBool("IsBrokenBool", true);
            lastBrokenEgg = eggAnimators[newLives].gameObject;


        }

        lives = newLives; // Update the current lives count
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


    }

    private void OnDisable()
    {
        player.globalEvents.OnInfiniteLives -= InfiniteLivesAnim;
        player.globalEvents.OnUpdateLives -= UpdateLives;

    }


}
