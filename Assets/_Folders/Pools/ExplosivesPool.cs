using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ExplosivesPool : ScriptableObject
{
    private readonly int bombPoolSize = 9;
    private readonly int balloonBombPoolSize = 8;

    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject balloonBombPrefab;

    private int currentBombIndex;
    private int currentBalloonBombIndex;

    private Bombs[] bombPool;
    private BalloonBomb[] balloonBombPool;


    public Sprite[] bombLaunchSmoke;


    public void MakePools()
    {
        bombPool = new Bombs[bombPoolSize];
        balloonBombPool = new BalloonBomb[balloonBombPoolSize];

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
    }

    public void GetBomb(Vector2 pos, Vector3 rot, float force, bool dropped)
    {
        bombPool[currentBombIndex].transform.position = pos;
        bombPool[currentBombIndex].transform.eulerAngles = rot;
        bombPool[currentBombIndex].GetBomb(force, dropped);
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
}
