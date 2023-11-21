using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{

    public GameObject missilePrefab;

    private GameObject BulletSpawnPoint;

    private GameObject player;

    

    private float missileTimer;

    private bool inRange;

    private int shotAmount;

    public float frontRange = 3;
    public float backRange = -3;

    public float shootTime = 2;

  

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
       

       
       

    }

   

    // Update is called once per frame
    void Update()
    {
     

        
        missileTimer += Time.deltaTime;
        Shoot();

    }

    void Shoot()
    {

        // if (PlayerManager.dead)
        // {
        //     return;
        // }
        
        if (player != null)
        {
        
         Vector3 distance = player.transform.position - transform.position;
         if (distance.x > frontRange && distance.x < backRange)
         {
            inRange = true;
         }
         else
         {
            inRange = false;
         }

       
        if (missileTimer > shootTime && shotAmount <= 10 && inRange)
        {
     GameObject BulletSpawnPoint = transform.Find("BulletSpawnPoint").gameObject;

    // Use the position of the eggDropSpawn GameObject as the position for the new egg
    GameObject missile = Instantiate(missilePrefab, BulletSpawnPoint.transform.position, Quaternion.identity);

    shotAmount += 1;
    missileTimer = 0;
        }
        

        }
        
   
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Fireball"))
        {
            Destroy(gameObject);
        }
}
}
