using UnityEngine;
using System;


public class PlayerManager : MonoBehaviour
{
    
    // public static bool dead;
    private bool dead;
    private GameObject player;

    [SerializeField] private GameObject smokeParticles;
    [SerializeField] private GameObject featherParticles;


    public static event Action onPlayerDeath;
    private StatsManager statsMan;

    void Awake()
    {
     
       
    }

    void Start()
    {
        statsMan = GetComponent<StatsManager>();
        statsMan.OnLivesChanged += UpdateLives;
        
       
        
    }

    void UpdateLives(int lives)
    {
         if (lives <= 0)
         {
            onPlayerDeath?.Invoke();
            Kill();

         }
    }
    

    

    public void Kill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
            // Instantiate smoke particles at the player's position if the player GameObject exists
            if (player != null)
            {
                Instantiate(featherParticles, player.transform.position, Quaternion.identity);
                Instantiate(smokeParticles, player.transform.position, Quaternion.identity);
                AudioManager.instance.PlayDeathSound(); 
                
                Destroy(player);
               
            }

            // Instantiate feather particles at the player's position

        }

    public void KillEvent()
    {
       
        onPlayerDeath?.Invoke();
        
        
    }
}
