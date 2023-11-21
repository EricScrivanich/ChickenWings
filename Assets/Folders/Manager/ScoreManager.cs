using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // public int score;

    // [SerializeField] private int initialScore;
    // [SerializeField] private int initialAmmunition;
    // public int ammunition;
    

    // public event Action<int> OnScoreChanged; // New event for score changes

    // private TextMeshProUGUI scoreText;
    // private TextMeshProUGUI ammoText;

    // private void Awake()
    // {
    //     score = initialScore;
    //     ammunition = initialAmmunition;
    // }

    // private void Start()
    // {
    //     scoreText = transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
    //     ammoText = transform.Find("AmmoText")?.GetComponent<TextMeshProUGUI>();
        
    //     Reset();
    // }

    // private void Update()
    // {
    //     if (scoreText != null)
    //         scoreText.text = "Score: " + score.ToString();

    //     if (ammoText != null)
    //         ammoText.text = "Ammo: " + ammunition.ToString();
    // }

    // public void Reset()
    // {
    //     score = initialScore;
    //     ammunition = initialAmmunition;

    //     if (scoreText != null)
    //         scoreText.text = "Score: " + score.ToString();

    //     if (ammoText != null)
    //         ammoText.text = "Ammo: " + ammunition.ToString();
    // }

    // public void IncreaseScore()
    // {
    //     score++;
    //     OnScoreChanged?.Invoke(score); // Raise the event with the new score value
    // }
}
