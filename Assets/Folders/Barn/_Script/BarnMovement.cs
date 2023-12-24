using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnMovement : MonoBehaviour
{
    public float speed = 5;
    [SerializeField] private bool nextBarnBool;


    private float spawnTimer;
    [SerializeField] private float spawnTime;
    private float spawnTimeVar;
    [SerializeField] private float offsetSpawnTime;

    

    
    void Awake()
    {
        spawnTimeVar = spawnTime += Random.Range(-offsetSpawnTime,offsetSpawnTime);
    }
    // Start is called before the first frame update
    void Start()
    {
        
  
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Restart();
   
    }
private void Restart()
{
   if (transform.position.x < -13f)
   {
       nextBarnBool = false;
       
   }
   
   if (!nextBarnBool)
   {
       spawnTimer += Time.deltaTime;
   }

   if (!nextBarnBool && spawnTimer >+ spawnTimeVar)
   {
    transform.position = new Vector2(BoundariesManager.rightBoundary, transform.position.y);
    nextBarnBool = true;
    spawnTimeVar = spawnTime += Random.Range(-offsetSpawnTime,offsetSpawnTime);
    spawnTimer = 0;
    
   }
  
}

private void Move()
{
    if (nextBarnBool)
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
   
}
    
}
