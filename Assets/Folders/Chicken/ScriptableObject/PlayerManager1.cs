using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager1 : PlayerSystem
{
    [SerializeField] private GameObject featherParticles;
    [SerializeField] private GameObject smokeParticles;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Kill()
    {
        
        
            // Instantiate smoke particles at the player's position if the player GameObject exists
        
                Instantiate(featherParticles, _transform.position, Quaternion.identity);
                Instantiate(smokeParticles, _transform.position, Quaternion.identity);
                AudioManager.instance.PlayDeathSound(); 
                
                gameObject.SetActive(false);
               
            

            // Instantiate feather particles at the player's position

        }
}
