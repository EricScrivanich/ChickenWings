using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EggDropScript : MonoBehaviour
{
    
    // private Rigidbody2D playerRb;
    // private Rigidbody2D rb;
    // private PlayerEggDrop yolkPool;
    
    
    // [SerializeField] private float dropSpeed;
    // [SerializeField] private ManualParticleSpawner particleSpawner;
    // [SerializeField] private GameObject player;

    // private float eggX;

    // [SerializeField] private GameObject eggYolkPrefab;

    // void Start()
    // {
    //     playerRb = player.GetComponent<Rigidbody2D>();
    //     yolkPool = player.GetComponent<PlayerEggDrop>();
    //     rb = GetComponent<Rigidbody2D>();

    //     eggX = (playerRb.velocity.x / 2);
    //     rb.velocity = new Vector2(eggX, dropSpeed);

        
    //     particleSpawner = GameObject.Find("EggShellParticles").GetComponent<ManualParticleSpawner>();
    // }

    //  private void OnTriggerEnter2D(Collider2D collider)
    //  {
   
    //     if (collider.gameObject.CompareTag("Floor"))
    //     {
    //         // Vector2 yolkSpawn = new Vector2 (transform.position.x, transform.position.y - .1f);

    //         //  GameObject yolk = yolkPool.GetPooledObject(yolkPool.yolkPool); // Use the GetPooledObject method from PlayerEggDrop

    //         // if (yolk != null) 
    //         // {
    //         //     yolk.transform.position = new Vector2(transform.position.x, transform.position.y - .15f);
    //         //     yolk.SetActive(true);
    //         // }

    //         // gameObject.SetActive(false);
    //         AudioManager.instance.PlayCrackSound();

    //         // GameObject eggYolk = Instantiate(eggYolkPrefab, new Vector3(transform.position.x ,transform.position.y - .1f , 0), Quaternion.identity);
           
  

    //         Vector3 spawnPos = new Vector3(transform.position.x - .3f, transform.position.y - .1f, transform.position.z);
    //         particleSpawner.SetSpawnPosition(spawnPos);
    //     }
    //     else if (collider.gameObject.CompareTag("Barn"))
    //     {
    //        gameObject.SetActive(false);

    //     }
    // }
}
