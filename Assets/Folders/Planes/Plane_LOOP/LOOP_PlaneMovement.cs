using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOP_PlaneMovement : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed;
    [SerializeField] private float speedOffset;  //Randomizes speed by either adding or subtracting this variable
    private float speedOffsetVar;  
    [SerializeField] private float planeSinFrequency;
    [SerializeField] private float planeSinAmplitude;

    

    [SerializeField] private float minSpawn;
    [SerializeField] private float maxSpawn;

    private float planeHeightRandom;

    private bool resetBool;
    private bool stopSpawn;

    [SerializeField] private float planeTime;
    private float planeTimeVar;
    [SerializeField] private float planeTimeOffset;
    private float planeTimeOffsetVar;
    private float planeTimer;

    private float planeSpeed;

    private bool planeHitBool;

    [SerializeField] private int lives;

    

    

    

   
   void Start()
   {
     
   }

    void Awake()
    {
        resetBool = false;
        stopSpawn = false;
        
        planeHeightRandom = Random.Range(minSpawn,maxSpawn);
        speedOffsetVar = Random.Range(-speedOffset,speedOffset);
        planeTimeOffsetVar = Random.Range(-planeTimeOffset,planeTimeOffset);
        planeTimeVar = planeTime + planeTimeOffsetVar;
        planeSpeed = speed + speedOffsetVar;
        planeHitBool = false;
       
        
    }
    void Update()
    {
    
        transform.position += Vector3.left * planeSpeed * Time.deltaTime;
        float x = transform.position.x;
        float y = Mathf.Sin(x * planeSinFrequency) * planeSinAmplitude + planeHeightRandom; // Calculate y position using sine function
        transform.position = new Vector3(x, y, 0); // Update position

    

        Restart();

        Timer();

        
        
        

    }

    void Restart()
    {
        if ((transform.position.x < -13 || planeHitBool))
        {
            resetBool = true;
            planeHeightRandom = Random.Range(minSpawn,maxSpawn);
            transform.position = new Vector3 (BoundariesManager.rightBoundary,planeHeightRandom,0);
            planeSpeed = 0;
            planeHitBool = false;
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

    void Timer()
    {
        if (resetBool)
        {
            planeTimer += Time.deltaTime;
        }

        if (planeTimer > planeTimeVar)
        {
            resetBool = false;
            speedOffsetVar = Random.Range(-speedOffset,speedOffset);
            planeSpeed = speed + speedOffsetVar;
            planeTimeOffsetVar = Random.Range(-planeTimeOffset,planeTimeOffset);
            planeTimeVar = planeTime + planeTimeOffsetVar;
            planeTimer = 0;
            
        }
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
