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
    private float initialY;
    private bool initialized;

    
    // void Awake()
    // {
        
    //     stopSpawn = false;
        
    //     planeHeightRandom = Random.Range(Data.minSpawn,Data.maxSpawn);
        
    //     planeTimeOffsetVar = Random.Range(-Data.planeTimeOffset,Data.planeTimeOffset);
    //     planeTimeVar = Data.planeTime + planeTimeOffsetVar;
    //     planeSpeed = Data.speed + Random.Range(-Data.speedOffset,Data.speedOffset);
    //     planeHitBool = false;
       
        
    // }
    void Update()
    {
       
        if (initialized)
        {
        transform.position += Vector3.left * planeSpeed * Time.deltaTime;
        float x = transform.position.x;
        float y = Mathf.Sin(x * Data.planeSinFrequency) * Data.planeSinAmplitude + initialY;
        transform.position = new Vector2(x, y);

        }
       
     
        
      
        Restart();
    }

    void Restart()
    {
        if ((transform.position.x < -13 || planeHitBool))
        {
            gameObject.SetActive(false);
            // planeHeightRandom = Random.Range(Data.minSpawn,Data.maxSpawn);
            // transform.position = new Vector3 (BoundariesManager.rightBoundary,planeHeightRandom,0);
            // planeSpeed = 0;
            // planeHitBool = false;
            // StartCoroutine(ResetTimer());
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
        planeTimeOffsetVar = Random.Range(-Data.planeTimeOffset,Data.planeTimeOffset);
        planeTimeVar = Data.planeTime + planeTimeOffsetVar;
        planeSpeed = Data.speed + Random.Range(-Data.speedOffset,Data.speedOffset);
        planeHitBool = false;
        initialY = transform.position.y;
        initialized = true;
        
        
    }

     void OnDisable()
    {
        initialized = false;
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
