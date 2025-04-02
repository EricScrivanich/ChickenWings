using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ExplosivesPool : ScriptableObject
{
    private readonly int bombPoolSize = 14;
    private readonly int balloonBombPoolSize = 8;

    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject balloonBombPrefab;
    [SerializeField] private GameObject bulletPrefab;

    private int currentBombIndex;
    private int currentBalloonBombIndex;

    private Bombs[] bombPool;
    private BalloonBomb[] balloonBombPool;

    private Queue<Bullet> bullets;


    public Sprite[] bombLaunchSmoke;


    public void MakePools()
    {
        bombPool = new Bombs[bombPoolSize];
        balloonBombPool = new BalloonBomb[balloonBombPoolSize];
        bullets = new Queue<Bullet>();


        for (int i = 0; i < bombPoolSize; i++)
        {
            var obj = Instantiate(bombPrefab);

            bombPool[i] = obj.GetComponent<Bombs>();
            obj.SetActive(false);
        }

        for (int i = 0; i < balloonBombPoolSize; i++)
        {
            var obj = Instantiate(balloonBombPrefab);

            balloonBombPool[i] = obj.GetComponent<BalloonBomb>();
            obj.SetActive(false);
        }

        for (int i = 0; i < 10; i++)
        {
            var obj = Instantiate(bulletPrefab);
            var bullet = obj.GetComponent<Bullet>();
            bullets.Enqueue(bullet);
            obj.SetActive(false);

        }
    }

    public void GetBomb(Vector2 pos, Vector3 rot, float force, bool dropped)
    {
        bombPool[currentBombIndex].transform.position = pos;
        bombPool[currentBombIndex].transform.eulerAngles = rot;
        int side = 1;
        if (rot.z < 0) side = -1;
        bombPool[currentBombIndex].GetBomb(force, dropped, side);
        currentBombIndex++;

        if (currentBombIndex >= bombPoolSize)
            currentBombIndex = 0;
    }

    public void GetBalloonBomb(Vector2 pos, Vector2 force)
    {
        balloonBombPool[currentBalloonBombIndex].transform.position = pos;
        balloonBombPool[currentBalloonBombIndex].Initilaize(force);
        currentBalloonBombIndex++;

        if (currentBalloonBombIndex >= balloonBombPoolSize)
            currentBalloonBombIndex = 0;
    }

    public void GetBullet(Vector2 pos, float z, float speed, int flipInt)
    {
        if (bullets.Count > 0)
        {
            var bullet = bullets.Dequeue();

            if (bullet.gameObject.activeInHierarchy)
            {
                bullets.Enqueue(bullet);
                bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
                bullet.gameObject.SetActive(false);
            }
            bullet.transform.position = pos;
            bullet.Fire(pos, z, speed, flipInt);
            bullet.gameObject.SetActive(true);
            bullets.Enqueue(bullet);
        }
    }
}
