using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliGun : MonoBehaviour
{
    public HelicopterID helicopterID;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform spawnPoint;
    private Animator anim;
    private int poolSize = 10;
    private int shootTriggerHash;

    private Queue<GameObject> bulletPool;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {


        shootTriggerHash = Animator.StringToHash("Shoot");


        // Initialize the bullet pool
        bulletPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    private void Shoot(float totalShootTime, int bulletCount)
    {
        StartCoroutine(ShootBullets(totalShootTime, bulletCount));
    }

    IEnumerator ShootBullets(float totalShootTime, int bulletCount)
    {
        float timeBetweenShots = helicopterID.shotDuration / helicopterID.bulletAmount;

        for (int i = 0; i < helicopterID.bulletAmount; i++)
        {
            // Get a bullet from the pool
            GameObject bullet = bulletPool.Dequeue();

            // Set bullet position and direction
            bullet.transform.position = spawnPoint.position;
            bullet.transform.rotation = transform.rotation;
            anim.SetTrigger(shootTriggerHash);

            bullet.SetActive(true);

            // Return the bullet to the pool after some time
            // For example, you could do this in a 'DisableBullet' coroutine
            // StartCoroutine(DisableBullet(bullet, bulletLifetime));

            // Re-enqueue the bullet for future use
            bulletPool.Enqueue(bullet);

            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private void OnEnable()
    {
        helicopterID.events.shoot += Shoot;
        anim.SetTrigger("Reset");


    }

    private void OnDisable()
    {
        helicopterID.events.shoot -= Shoot;

    }
}
