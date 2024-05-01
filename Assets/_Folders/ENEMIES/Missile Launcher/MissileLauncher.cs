using System.Collections;
using UnityEngine;
using HellTap.PoolKit;  // Include PoolKit namespace

public class MissileLauncher : MonoBehaviour
{
    private GameObject bulletSpawnPoint;
    private GameObject player;

    private float missileTimer;
    private bool inRange;
    private int shotAmount;

    public float frontRange = 3;
    public float backRange = -3;
    public float shootTime = 2;
   
    
    public Pool missilePool; // Drag your pool reference here in the inspector

    void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
        bulletSpawnPoint = transform.Find("BulletSpawnPoint").gameObject;
        
       
    }
    private void OnEnable() {
        missileTimer = shootTime;
    }

    void Update()
    {
        missileTimer += Time.deltaTime;
        Shoot();
    }

    void Shoot()
    {
        if (player != null)
        {
            Vector3 distance = player.transform.position - transform.position;
            inRange = distance.x > frontRange && distance.x < backRange;

            if (missileTimer > shootTime && shotAmount <= 10 && inRange)
            {
                Vector3 direction = player.transform.position - bulletSpawnPoint.transform.position;
                Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);

                // Spawn the missile from the pool with the calculated rotation
                Transform missile = missilePool.Spawn("missile",bulletSpawnPoint.transform.position, rotation);
                
                shotAmount++;
                missileTimer = 0;
            }
        }
    }
}

