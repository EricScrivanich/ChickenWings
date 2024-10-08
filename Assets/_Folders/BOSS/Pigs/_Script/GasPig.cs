using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPig : MonoBehaviour
{


    [SerializeField] private GameObject cloud;

    public int id;

    private bool hasCrossedBoundary;



    [SerializeField] private Transform cloudSpawn;



    private Animator anim;

    public float speed;
    public float delay = 0;

    private float testTimer = 0;

    [SerializeField] private bool flying;


    // Start is called before the first frame update

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void Initialize(float s, float d)
    {
        speed = s;
        delay = d;
        hasCrossedBoundary = false;

        gameObject.SetActive(true);

        if (flying)
        {
            anim.SetTrigger("FlyFart");

        }
        else
        {
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



    }

    private IEnumerator CloudRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            anim.SetTrigger("FartTrigger");
            yield return new WaitForSeconds(.1f);
            AudioManager.instance.PlayFartSound();
            yield return new WaitForSeconds(.1f);


            Instantiate(cloud, cloudSpawn.position, Quaternion.identity, transform);

        }



    }

    private void OnDisable()
    {
        if (flying)
            SmokeTrailPool.OnDisableGasTrail?.Invoke(id);
    }
}
