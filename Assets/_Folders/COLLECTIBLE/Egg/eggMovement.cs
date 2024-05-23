using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggMovement : MonoBehaviour, ICollectible
{
    public PlayerID ID;
    private int amount = 1;
    private ammoParentScript parent;
    private float speed;
    private bool ammoBool;
    public bool ammoCollected;
    [SerializeField] private float eggTime;
    private float eggTimer;
    private float minY;
    private float maxY;
    private float yPos;
    

    // private StatsManager statsMan;

    [SerializeField] private float minAmmoSpeed;
    [SerializeField] private float maxAmmoSpeed;

   

   





    void Awake()
    {
        
        speed = Random.Range(minAmmoSpeed,maxAmmoSpeed);
        parent = GetComponentInParent<ammoParentScript>();
        ammoBool = false;
        ammoCollected = false;
        eggTimer = 0;
        

        minY = -2.5f;
        maxY = 4.5f;


       

    }
    void Start()
    {
        
       
        // statsMan = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();
       
        
        
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
    
        transform.Translate(Vector3.left * speed* Time.deltaTime);
   
    
 
    }
    public int GetAmmo()
    {
        return amount;
    }
    
    void Restart()
    {
     
        if ((transform.position.x < -13 && ammoBool) || ammoCollected)
        {
            ammoCollected = false;
            yPos = Random.Range(minY, maxY);
            speed = Random.Range(minAmmoSpeed,maxAmmoSpeed);
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

    public void Collected()
    {
        ammoCollected = true;

        ID.Ammo += 1;
        ID.globalEvents.OnUpdateAmmo?.Invoke();
        
    }

    

  
}
    

