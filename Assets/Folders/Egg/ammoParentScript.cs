using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoParentScript : MonoBehaviour
{
    private Transform topBoundryTransform;
    private Transform bottomBoundryTransform;

    public GameObject eggAmmoPrefab;
    private GameObject newEgg;

    private float eggTimer;
    private float eggTime;
    private Vector3 position;

    public float maxY;
    public float minY;
    private float yPos;

    public bool ammoBool;
    public float speed;
    public bool makeEggBool;
    private bool instatiate;
    // Start is called before the first frame update
    void Awake()
    {
        ammoBool = true;
        makeEggBool = true;
        
        eggTime = 10f;
        eggTimer = 0;
        instatiate = false;
    }
    void Start()
    {
        
        topBoundryTransform = GetComponentInChildren<Transform>().Find("topSpawn");
        bottomBoundryTransform = GetComponentInChildren<Transform>().Find("bottomSpawn");
        maxY = topBoundryTransform.position.y;
        minY = bottomBoundryTransform.position.y;
        
    }

    // Update is called once per frame
    void Update()
    {
        
       MakeEgg();

    
       
        
    }
    
    void MakeEgg()
    {
        if (makeEggBool)
        {
            yPos = Random.Range(minY, maxY);

            position = new Vector3 (transform.position.x,yPos,0);
            GameObject newEgg = Instantiate(eggAmmoPrefab, position, Quaternion.identity);
            makeEggBool = false;
            instatiate = true;

            

        }
    }

    

    void Restart()
    {
        if (instatiate)
        {
        
        if (newEgg.transform.position.x < -11 && ammoBool)
        {
            speed = 0;
            yPos = Random.Range(minY, maxY);
            newEgg.transform.position = new Vector3(transform.position.x,yPos, 0);
            ammoBool = false;
        }
        if (!ammoBool)
        {
            eggTimer += 1 * Time.deltaTime;

            if (eggTimer >= eggTime)
            {
                ammoBool = true;
            }

        }

        if (ammoBool && speed == 0)
        {
            speed = Random.Range(6f,8.5f);
        }
        }
    }
}
