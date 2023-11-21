using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreAdder : MonoBehaviour
{
    private StatsManager statsMan;
    
    // Start is called before the first frame update
    void Start()
    {
        statsMan = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("eggDrop"))
        {
            AudioManager.instance.PlayScoreSound();
            
            statsMan.AddScore(1); // Use the IncreaseScore method to increase score and invoke the OnScoreChanged event
        }
    }
}
