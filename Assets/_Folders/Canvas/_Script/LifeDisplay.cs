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
        InitializeEggAnimators();
        
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

    void UpdateLives(int newLives)
    {
        // Check if gained a life
        if (newLives > lives)
        {
            if (lives <= 2)
            {
                
                eggAnimators[lives].SetBool("IsBrokenBool",false);
              


            }
            else{
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

    private void OnDestroy()
    {
        // Unsubscribe from the OnLivesChanged event
        // statsMan.OnLivesChanged -= UpdateLives;
    }
    private void OnEnable() {
     player.globalEvents.OnUpdateLives += UpdateLives;
    }
    private void OnDisable() {
        player.globalEvents.OnUpdateLives -= UpdateLives;
    }
}
