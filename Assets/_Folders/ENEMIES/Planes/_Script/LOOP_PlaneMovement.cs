using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOP_PlaneMovement : MonoBehaviour, IDamageable
{
    public PlaneData Data;
    public PlaneManagerID ID;
    public int doesTiggerInt;
    public float xCordinateTrigger;
    private float planeHeightRandom;

    private Collider2D coll;
    private bool stopSpawn;
    private float planeTimeVar;
    private float planeTimeOffsetVar;
    private float planeSpeed;
    private bool planeHitBool;
    private int lives;
    private float initialY;
    private bool initialized;
    public float speed = 0;
    private Animator anim;
    private bool positiveSpeed;
    private bool hasTrigger;


    // void Awake()
    // {

    //     stopSpawn = false;

    //     planeHeightRandom = Random.Range(Data.minSpawn,Data.maxSpawn);

    //     planeTimeOffsetVar = Random.Range(-Data.planeTimeOffset,Data.planeTimeOffset);
    //     planeTimeVar = Data.planeTime + planeTimeOffsetVar;
    //     planeSpeed = Data.speed + Random.Range(-Data.speedOffset,Data.speedOffset);
    //     planeHitBool = false;


    // }

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {


        transform.position += Vector3.left * speed * Time.deltaTime;

        if (positiveSpeed)
        {
            if (hasTrigger)
            {
                if (transform.position.x < xCordinateTrigger)
                {
                    hasTrigger = false;
                    ID.events.TriggeredSpawn?.Invoke(doesTiggerInt);
                    Debug.Log("PlaneTrigger");
                    return;

                }

            }

            else
            {
                if (transform.position.x < BoundariesManager.leftBoundary)
                {
                    gameObject.SetActive(false);
                }
            }

        }
        else
        {
            if (hasTrigger)
            {
                if (transform.position.x > xCordinateTrigger)
                {
                    ID.events.TriggeredSpawn(doesTiggerInt);
                    hasTrigger = false;
                }

            }

            else
            {
                if (transform.position.x > BoundariesManager.rightBoundary)
                {
                    gameObject.SetActive(false);
                }
            }


        }



        // transform.position += Vector3.left * planeSpeed * Time.deltaTime;

        float x = transform.position.x;
        float y = Mathf.Sin(x * Data.planeSinFrequency) * Data.planeSinAmplitude + initialY;
        transform.position = new Vector2(x, y);

        // Restart();
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
        if (speed > 0)
        {
            positiveSpeed = true;
            Vector3 currentRotation = transform.eulerAngles;
            transform.eulerAngles = new Vector3(currentRotation.x, 0, currentRotation.z);
        }
        else
        {
            Debug.Log("Negative Speed: " + speed);
            positiveSpeed = false;
            Vector3 currentRotation = transform.eulerAngles;
            transform.eulerAngles = new Vector3(currentRotation.x, 180, currentRotation.z);
            // flip x rotation here

        }

        if (doesTiggerInt != 0)
        {
            hasTrigger = true;
        }
        else
        {
            hasTrigger = false;
        }
        planeTimeOffsetVar = Random.Range(-Data.planeTimeOffset, Data.planeTimeOffset);
        planeTimeVar = Data.planeTime + planeTimeOffsetVar;
        planeSpeed = Data.speed + Random.Range(-Data.speedOffset, Data.speedOffset);
        // planeHitBool = false;
        coll.enabled = true;
        initialY = transform.position.y;



    }

    // void OnDisable()
    // {

    // }



    // private IEnumerator ResetTimer()
    // {

    //     yield return new WaitForSeconds(planeTimeVar);
    //     planeSpeed = Data.speed + Random.Range(-Data.speedOffset,Data.speedOffset);

    //     planeTimeOffsetVar = Random.Range(-Data.planeTimeOffset,Data.planeTimeOffset);
    //     planeTimeVar = Data.planeTime + planeTimeOffsetVar;
    // }
    public void GetExplosion()
    {
        speed = 0;
        Data.GetExplosion(transform.position);
        AudioManager.instance.PlayPlaneExplosionSound();

        Debug.Log("Stopped" + this.gameObject);
    }



    public void SetUnactive()
    {
        gameObject.SetActive(false);
    }
    public void Hit(int damageAmount)
    {
        // lives -= damageAmount;

        // if (lives <= 0)
        // {
        //     planeHitBool = true;
        //     coll.enabled = false;
        //     anim.SetTrigger("ExplodeTrigger");
        // }
        anim.SetTrigger("ExplodeTrigger");
        planeHitBool = true;
        coll.enabled = false;


    }

    public void Damage(int damageAmount)
    {
        Hit(damageAmount);
    }
}
