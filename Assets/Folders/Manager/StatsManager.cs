using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatsManager : MonoBehaviour
{
    // Define events for changes in stats
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnAmmoChanged;
    public static event Action<int> OnLivesChanged;

    [SerializeField] private int score;
    private int initialScore;
    [SerializeField] private int ammo;
    private int initialAmmo;
    [SerializeField] private int lives;
    private int initialLives;

    
    void Start()
    {
        OnScoreChanged?.Invoke(score);
        OnAmmoChanged?.Invoke(ammo);
       
        initialLives = lives;
        initialAmmo = ammo;
        initialScore = score;

    }

    public void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score); // Notify subscribers
    }

    public void UseAmmo(int amount)
    {
        ammo -= amount;
        OnAmmoChanged?.Invoke(ammo); // Notify subscribers
    }
     public void AddAmmo(int amount)
    {
        ammo += amount;
        OnAmmoChanged?.Invoke(ammo); // Notify subscribers
    }

    public void LoseLife(int amount)
    {
        lives -= amount;
        OnLivesChanged?.Invoke(lives); // Notify subscribers
    }
    public void AddLife(int amount)
    {
     lives += amount;
     OnLivesChanged?.Invoke(lives);
    }
    private void SetStats()
    {
        lives = initialLives;
        score = initialScore;
        ammo = initialAmmo;
    }
    void OnEnable()
    {
        ResetManager.onRestartGame += SetStats;

    }

    void OnDisable()
    {
        ResetManager.onRestartGame -= SetStats;
    }


    // The getters
    public int GetScore() { return score; }
    public int GetAmmo() { return ammo; }
    public int GetLives() { return lives; }
}




