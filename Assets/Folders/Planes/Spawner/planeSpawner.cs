using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeSpawner : MonoBehaviour
{
    
    public GameObject childPrefab;
    private List<GameObject> newChildren;

    [SerializeField] private int maxPlanes;
    private int planeAmount;
    
  
    private GameObject child;
    private GameObject newChild;

    
    
    [SerializeField] private bool addPlaneSwitch;

    private float minY;
    private float maxY;
    private float yPos;
    private float planeTimer;

    public float spawnTimer = 10f;

    private Transform topBoundryTransform;
    private Transform bottomBoundryTransform;
    

 
 

void Awake()
    {
        
        
        
        planeAmount = 0;
        

         
        

        
        
    }

void Start()
    {
        // Generate a rand
        
        topBoundryTransform = GetComponentInChildren<Transform>().Find("topBoundry");
        bottomBoundryTransform = GetComponentInChildren<Transform>().Find("bottomBoundry");
        maxY = topBoundryTransform.position.y;
        minY = bottomBoundryTransform.position.y;
       
        yPos = Random.Range(minY, maxY);
        newChildren = new List<GameObject>();
        



        
        

        

        // Set minY and maxY to be the same size as the main camera
        
       
        
        

       
    
        
    }

void Update()
    {

        planeTimer += Time.deltaTime;
       
       addPlane();
       
       
       
       

       
      
    }

// void restart()
// {
    
//     for (int i = 0; i < newChildren.Count; i++)
//         {
//             // Check if the current new child has gone off screen
//             if (newChildren[i].transform.position.x < -30)
//             {
//                 // Reset the new child's x position to the starting x position
//                 newChildren[i].transform.position = new Vector3(transform.position.x, newChildren[i].transform.position.y, 0);

//                 // Set the y position of the new child to a new random value within the bounds of the camera's view
//                 newChildren[i].transform.position = new Vector3(newChildren[i].transform.position.x, Random.Range(minY, maxY), 0);
//             }

//         }
// }

void addPlane()
{
    PlaneSwitch();
    // Check if the current value of planeTimer is greater than or equal to the interval
    if (addPlaneSwitch)
    {
        // Generate a new y position for the child object
        yPos = Random.Range(minY, maxY);

        // Instantiate the new child object
        GameObject newChild = Instantiate(childPrefab, new Vector3(transform.position.x, yPos, transform.position.z), Quaternion.identity);

        

        // Add the new child to the list of new children
        newChildren.Add(newChild);

        // Reset the planeTimer to zero
        planeTimer = 0;
        planeAmount ++;
        addPlaneSwitch = false;

    }
}

void PlaneSwitch()
{
    if (planeTimer > spawnTimer & planeAmount <= maxPlanes)
    addPlaneSwitch = true;
    

}


}
