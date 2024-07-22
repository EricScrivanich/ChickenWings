using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class MissilePigScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private ParticleSystem ps;
    private Transform player;
    [SerializeField] private float shootTime;
    [SerializeField] private float frontRange;
    [SerializeField] private float backRange;
    [SerializeField] private float aimTime;
    private float missileTimer;


    [SerializeField] private Transform launchAim;
    [SerializeField] private Transform missileSpawnPos;

    [SerializeField] private SpriteRenderer missileImage;

    private Pool pool; // Drag your pool reference here in the inspector
    // Start is called before the first frame update
    void Start()
    {
        pool = PoolKit.GetPool("ExplosionPool");

        player = GameObject.Find("Player").GetComponent<Transform>();


    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        missileTimer += Time.deltaTime;
        if (player != null)
        {
            Vector3 distance = player.transform.position - transform.position;
            bool inRange = distance.x > frontRange && distance.x < backRange;

            if (missileTimer > shootTime && inRange)
            {
                StartCoroutine(LaunchMissile());
                missileTimer = 0;
            }
        }
    }

    private IEnumerator LaunchMissile()
    {
        float elapsedTime = 0f;
        while (elapsedTime < aimTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 direction = player.transform.position - launchAim.position;
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            launchAim.rotation = Quaternion.Slerp(launchAim.rotation, targetRotation, elapsedTime / aimTime);

            yield return null;
        }
        ps.Play();
        yield return new WaitForSeconds(.5f);
        // elapsedTime = 0;
        // while (elapsedTime < .4f)
        // {
        //     elapsedTime += Time.deltaTime;
        //     missileImage.transform.localPosition = new Vector2(.19f, ((elapsedTime / .4f) * .8f) + .98f);


        //     yield return null;
        // }




        // Spawn the missile from the pool with the final rotation
        pool.Spawn("missile", missileImage.transform.position, launchAim.rotation);
        missileImage.enabled = false;
    }

    private void OnEnable()
    {
        missileImage.enabled = true;

        missileTimer = shootTime;
    }

}

