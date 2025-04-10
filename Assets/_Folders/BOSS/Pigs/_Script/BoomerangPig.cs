using UnityEngine;
using System.Collections;
using PathCreation;
using DG.Tweening;

public class BoomerangPig : MonoBehaviour
{
    [SerializeField] private Transform boomeranngSpawnPoint;
    [SerializeField] private Pigarang boomerangPrefab;
    [SerializeField] private float throwInterval = 2f;
    [SerializeField] private float throwBreak;
    [SerializeField] private int throwAmount;
    private int throwCount;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float arcChange;
    private Transform player;
    [SerializeField] private PathCreator path;
    [SerializeField] private float totalTime;
    private bool usingReverseDirection = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").transform;
        StartCoroutine(ThrowBoomerang());

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator ThrowBoomerang()
    {
        throwCount = 0;
        while (true)
        {
            yield return new WaitForSeconds(throwInterval);
            float arc = arcChange;
            if (transform.position.y > 0) arc = -arcChange;

            Pigarang boomerang = Instantiate(boomerangPrefab, boomeranngSpawnPoint.position, boomeranngSpawnPoint.rotation);
            // Vector2 direction = (player.position - boomeranngSpawnPoint.position).normalized;
            // boomerang.Throw(direction, throwForce, arc, false);

            boomerang.ThrowUsingPath(path, totalTime, usingReverseDirection);
            throwCount++;
            if (throwCount >= throwAmount)
            {
                throwCount = 0;
                if (!usingReverseDirection)
                {
                    usingReverseDirection = true;
                    transform.DOMoveY(transform.position.y - 5, throwBreak).SetEase(Ease.OutSine);

                }
                else
                {
                    usingReverseDirection = false;
                    transform.DOMoveY(transform.position.y + 5, throwBreak).SetEase(Ease.OutSine);

                }
                yield return new WaitForSeconds(throwBreak + .4f);
            }
        }

    }
}
