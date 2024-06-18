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
    [SerializeField] private List<Animator> eggAnimators; // List to store the Animator components of the eggs



    private void Awake()
    {

        player.infiniteLives = infiniteLives;
        Debug.Log("InfiniteLives?" + player.infiniteLives);





        // InitializeEggAnimators();

    }

    private void OnDestroy()
    {


    }
    void Start()
    {

        // InitializeEggAnimators();

        // Subscribe to the OnLivesChanged event
        // statsMan.OnLivesChanged += UpdateLives;
    }

    // private void InitializeEggAnimators()
    // {
    //     eggAnimators = new List<Animator>();

    //     if (livesPanel == null)
    //     {

    //         return;
    //     }

    //     // Iterate through the children of the LivesPanel and find the Animator components
    //     for (int i = 0; i < livesPanel.childCount; i++)
    //     {
    //         Transform childTransform = livesPanel.GetChild(i);
    //         Animator eggAnimator = childTransform.GetComponent<Animator>();

    //         if (eggAnimator != null)
    //         {
    //             eggAnimators.Add(eggAnimator);

    //         }

    //     }
    // }

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
            for (int i = 2; i > newLives - 1; i--)
            {
                // Assuming eggs are lost from right to left

                eggAnimators[i].SetBool("IsBrokenBool", true);

            }
        }

        lives = newLives; // Update the current lives count
    }


    void InfiniteLivesAnim()
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
