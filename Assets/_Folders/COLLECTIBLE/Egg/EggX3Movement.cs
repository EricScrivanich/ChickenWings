using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggX3Movement : MonoBehaviour
{
    
    public PlayerID player;
    private float speed;
    private int amount = 3;
    private bool ammoBool;
    public bool ammoCollected;
    [SerializeField] private float eggTime;
    private float eggTimer;
    private float minY;
    private float maxY;
    private float yPos;
    

    [SerializeField] private float minAmmoSpeed;
    [SerializeField] private float maxAmmoSpeed;

    [SerializeField] private float minEggSinFrequency;
    [SerializeField] private float maxEggSinFrequency;
    private float eggSinFrequency;


    [SerializeField] private float minEggSinAmplitude;
    [SerializeField] private float maxEggSinAmplitude;
    private float eggSinAmplitude;


    [SerializeField] private bool eggX3Bool;

  private StatsManager statsMan;
   





    void Awake()
    {
        
        speed = Random.Range(minAmmoSpeed,maxAmmoSpeed);
       
        ammoBool = false;
        ammoCollected = true;
        eggTimer = 0;
        

        minY = -2.5f;
        maxY = 4.2f;

        eggSinAmplitude = Random.Range(minEggSinAmplitude, maxEggSinAmplitude);
        eggSinFrequency = Random.Range(minEggSinFrequency, maxEggSinFrequency);


       

    }
    void Start()
    {
        
      
         statsMan = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();
        
        
    }


    void Update()
    {
        if (ammoBool)
        {
            Movement();
        }

        Restart();
             
   
        
    }
    void Movement()
    {
   
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        float x = transform.position.x;
        float y = Mathf.Sin(x * eggSinFrequency) * eggSinAmplitude + yPos; // Calculate y position using sine function
        transform.position = new Vector3(x, y, 0); // Update position

    
    }
    
    void Restart()
    {
     
        if ((transform.position.x < -13 && ammoBool) || ammoCollected)
        {
            ammoCollected = false;
            yPos = Random.Range(minY, maxY);
            speed = Random.Range(minAmmoSpeed,maxAmmoSpeed);
            eggSinAmplitude = Random.Range(minEggSinAmplitude, maxEggSinAmplitude);
            eggSinFrequency = Random.Range(minEggSinFrequency, maxEggSinFrequency);

            transform.position = new Vector3(BoundariesManager.rightBoundary,yPos, 0);
            ammoBool = false;
        }
        if (!ammoBool)
        {
            eggTimer += 1 * Time.deltaTime;

            if (eggTimer >= eggTime)
            {
                ammoBool = true;
                eggTimer = 0;
            }

        }

    }
    

     private void OnTriggerEnter2D(Collider2D collider)
{
    
    if (collider.gameObject.tag == "Player")
    {
        
        ammoCollected = true;
        statsMan.AddAmmo(3);

        player.globalEvents.OnAddAmmo?.Invoke(amount);
      
        
        
    }

    
}
}
