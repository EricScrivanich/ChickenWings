using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOP_PlaneMovement : MonoBehaviour, IDamageable
{
    public PlaneData Data;
    private float planeHeightRandom;
    private bool stopSpawn;
    private float planeTimeVar;
    private float planeTimeOffsetVar;
    private float planeSpeed;
    private bool planeHitBool;
    private int lives;

    void Awake()
    {
        
        stopSpawn = false;
        
        planeHeightRandom = Random.Range(Data.minSpawn,Data.maxSpawn);
        
        planeTimeOffsetVar = Random.Range(-Data.planeTimeOffset,Data.planeTimeOffset);
        planeTimeVar = Data.planeTime + planeTimeOffsetVar;
        planeSpeed = Data.speed + Random.Range(-Data.speedOffset,Data.speedOffset);
        planeHitBool = false;
       
        
    }
    void Update()
    {
    
        transform.position += Vector3.left * planeSpeed * Time.deltaTime;
        float x = transform.position.x;
        float y = Mathf.Sin(x * Data.planeSinFrequency) * Data.planeSinAmplitude + planeHeightRandom; 
        transform.position = new Vector3(x, y, 0); // Update position
        Restart();
    }

    void Restart()
    {
        if ((transform.position.x < -13 || planeHitBool))
        {
            planeHeightRandom = Random.Range(Data.minSpawn,Data.maxSpawn);
            transform.position = new Vector3 (BoundariesManager.rightBoundary,planeHeightRandom,0);
            planeSpeed = 0;
            planeHitBool = false;
            StartCoroutine(ResetTimer());
        }
    }

    void StopSpawn()
    {
        if (planeSpeed == 0)
        {
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        PlayerManager.onPlayerDeath += StopSpawn;
    }

     void OnDisable()
    {
        PlayerManager.onPlayerDeath -= StopSpawn;
    }

   

    private IEnumerator ResetTimer()
    {
        
        yield return new WaitForSeconds(planeTimeVar);
        planeSpeed = Data.speed + Random.Range(-Data.speedOffset,Data.speedOffset);
            
        planeTimeOffsetVar = Random.Range(-Data.planeTimeOffset,Data.planeTimeOffset);
        planeTimeVar = Data.planeTime + planeTimeOffsetVar;
    }

    public void Hit(int damageAmount)
    {
        lives -= damageAmount;

        if (lives <= 0)
        {
            planeHitBool = true;
        }
    }

    public void Damage(int damageAmount)
    {
        Hit(damageAmount);
    }
}
