using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPig : MonoBehaviour
{
    public float speed;
    public float delay = 0;
    public float initialDelay;


    [HideInInspector]
    public int id;

    private bool hasCrossedBoundary;

    private bool flipped;



    [SerializeField] private Transform cloudSpawn;



    private Animator anim;



    private float testTimer = 0;

    [SerializeField] private bool flying;


    // Start is called before the first frame update

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void Initialize(float s, float d, bool f, float startDelay)
    {
        speed = s;
        delay = d;
        flipped = f;
        initialDelay = startDelay;
        hasCrossedBoundary = false;

        gameObject.SetActive(true);

        if (flying)
        {
            transform.localScale = BoundariesManager.vectorThree1 * .8f;
            anim.SetTrigger("FlyFart");

        }
        else
        {
            transform.localScale = BoundariesManager.vectorThree1;
            anim.SetTrigger("Walk");
            StartCoroutine(CloudRoutine());

        }
    }





    // Update is called once per frame
    void Update()
    {
        testTimer += Time.deltaTime;
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (flying && !hasCrossedBoundary)
        {
            if (Mathf.Abs(transform.position.x) < BoundariesManager.rightBoundary)
            {
                AudioManager.instance.PlayLongFarts();
                SmokeTrailPool.GetGasSmokeTrail?.Invoke(transform.position, speed, delay, id);
                hasCrossedBoundary = true;
            }
        }
        // else if (!flying && !hasCrossedBoundary)
        // {
        //     if (Mathf.Abs(transform.position.x) < BoundariesManager.rightBoundary)
        //     {
        //         StartCoroutine(CloudRoutine());
        //         hasCrossedBoundary = true;
        //     }

        // }



    }

    private IEnumerator CloudRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {

            // if (Mathf.Abs(transform.position.x) > BoundariesManager.rightBoundary)
            // {
            //     yield return new WaitForSeconds(.2f + delay);


            // }
            // else
            // {
            //     anim.SetTrigger("FartTrigger");
            //     yield return new WaitForSeconds(.1f);
            //     AudioManager.instance.PlayFartSound();
            //     yield return new WaitForSeconds(.1f);


            //     // Instantiate(cloud, cloudSpawn.position, Quaternion.identity, transform);
            //     SmokeTrailPool.GetGasCloud?.Invoke(cloudSpawn.position, -speed, flipped);

            //     yield return new WaitForSeconds(delay);

            // }

            anim.SetTrigger("FartTrigger");
            yield return new WaitForSeconds(.1f);
            if (Mathf.Abs(transform.position.x) < BoundariesManager.rightBoundary)
                AudioManager.instance.PlayFartSound();
            yield return new WaitForSeconds(.1f);


            // Instantiate(cloud, cloudSpawn.position, Quaternion.identity, transform);
            SmokeTrailPool.GetGasCloud?.Invoke(cloudSpawn.position, -speed, flipped);

            yield return new WaitForSeconds(delay);


        }



    }

    private void OnDisable()
    {
        if (flying)
            SmokeTrailPool.OnDisableGasTrail?.Invoke(id);
    }
}
