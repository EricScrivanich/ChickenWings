using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireballMovement : MonoBehaviour
{
   
    private float speed;
    [SerializeField] private GameObject player;
    private PowerUps powerUps;
    [SerializeField] private float timeToReturn;
    

     public bool returnToPool;
    // Start is called before the first frame update
    void Awake()
    {
        speed = 10;
    }
    void Start()
    {
       

        powerUps = player.GetComponent<PowerUps>();



     
        
    }

     void OnEnable()
    {
       
        
    }

    // Called when the GameObject is disabled
    

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector3.right * speed* Time.deltaTime);
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        
    }

     private void OnTriggerEnter2D(Collider2D collider)
{
     if (collider.gameObject.tag == "Plane")
     {

       gameObject.SetActive(false);
        
     }      
    
}

// IEnumerator ReturnFireballAfterTime()
//     {
//         yield return new WaitForSeconds(timeToReturn);

//         returnToPool = true;
//         powerUps.pool.Release(gameObject);
//         print("yes");
//     }
        
}
