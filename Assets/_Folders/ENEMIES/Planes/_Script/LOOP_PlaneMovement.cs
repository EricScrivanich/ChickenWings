using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOP_PlaneMovement : MonoBehaviour, IDamageable
{
    public PlaneData Data;
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
    public float speed;
    private Animator anim;


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

        if (initialized)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            if ((transform.position.x < BoundariesManager.leftBoundary))
            {
                gameObject.SetActive(false);

            }
            // transform.position += Vector3.left * planeSpeed * Time.deltaTime;

            float x = transform.position.x;
            float y = Mathf.Sin(x * Data.planeSinFrequency) * Data.planeSinAmplitude + initialY;
            transform.position = new Vector2(x, y);

        }




        // Restart();
    }

    void Restart()
    {

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
        planeTimeOffsetVar = Random.Range(-Data.planeTimeOffset, Data.planeTimeOffset);
        planeTimeVar = Data.planeTime + planeTimeOffsetVar;
        planeSpeed = Data.speed + Random.Range(-Data.speedOffset, Data.speedOffset);
        // planeHitBool = false;
        coll.enabled = true;
        initialY = transform.position.y;
        initialized = true;


    }

    void OnDisable()
    {
        initialized = false;
    }



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
