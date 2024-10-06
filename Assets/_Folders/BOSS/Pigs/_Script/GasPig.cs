using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPig : MonoBehaviour
{
  

    [SerializeField] private GameObject cloud;

    [SerializeField] private Transform cloudSpawn;

    private Animator anim;

    public float speed;
    public float delay;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(CloudRoutine());

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

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
}
