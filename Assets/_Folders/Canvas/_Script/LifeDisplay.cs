using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LifeDisplay : MonoBehaviour
{
    [SerializeField] private Transform livesPanel; // Reference to the LivesPanel
    public PlayerID player;
    private int lives;
    private List<Animator> eggAnimators; // List to store the Animator components of the eggs

    private StatsManager statsMan; // Reference to StatsManager

    private void Awake() 
    {
        lives = player.Lives;
        
    }   
    void Start()
    {
        statsMan = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();
        InitializeEggAnimators();

        // Subscribe to the OnLivesChanged event
        // statsMan.OnLivesChanged += UpdateLives;
    }

    private void InitializeEggAnimators()
    {
        eggAnimators = new List<Animator>();

        if (livesPanel == null)
        {
            
            return;
        }

        // Iterate through the children of the LivesPanel and find the Animator components
        for (int i = 0; i < livesPanel.childCount; i++)
        {
            Transform childTransform = livesPanel.GetChild(i);
            Animator eggAnimator = childTransform.GetComponent<Animator>();

            if (eggAnimator != null)
            {
                eggAnimators.Add(eggAnimator);
                
            }
            
        }
    }

    void UpdateLives(int lives)
    {
        
        int livesLost = eggAnimators.Count - lives;
        if (livesLost > 0 && livesLost <= eggAnimators.Count)
        {
            // Get the rightmost egg's Animator component that is not yet dead
            Animator eggAnimator = eggAnimators[eggAnimators.Count - livesLost];

            // Trigger the "Break" animation
            eggAnimator.SetTrigger("Break");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnLivesChanged event
        // statsMan.OnLivesChanged -= UpdateLives;
    }
    private void OnEnable() {
     player.globalEvents.LoseLife += UpdateLives;
    }
    private void OnDisable() {
        player.globalEvents.LoseLife -= UpdateLives;
    }
}
