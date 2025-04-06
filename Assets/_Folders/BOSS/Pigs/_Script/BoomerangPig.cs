using UnityEngine;
using System.Collections;

public class BoomerangPig : MonoBehaviour
{
    [SerializeField] private Transform boomeranngSpawnPoint;
    [SerializeField] private Pigarang boomerangPrefab;
    [SerializeField] private float throwInterval = 2f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float arcChange;
    private Transform player;
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
        while (true)
        {
            yield return new WaitForSeconds(throwInterval);
            float arc = arcChange;
            if (transform.position.y > 0) arc = -arcChange;

            Pigarang boomerang = Instantiate(boomerangPrefab, boomeranngSpawnPoint.position, boomeranngSpawnPoint.rotation);
            Vector2 direction = (player.position - boomeranngSpawnPoint.position).normalized;
            boomerang.Throw(direction, throwForce, arc, false);
        }

    }
}
