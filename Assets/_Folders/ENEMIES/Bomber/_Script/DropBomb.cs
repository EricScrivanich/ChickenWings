using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBomb : MonoBehaviour
{
    [SerializeField] private GameObject bombSpawn;
    private Transform spawnTransform;
    [SerializeField] private GameObject dropZonePrefab;
    private GameObject dropZone;

    [SerializeField] private float speed;

    private Queue<GameObject> pooledBombs = new Queue<GameObject>();
    [SerializeField] private GameObject bombPrefab;

    public PlaneManagerID ID;
    private int amountToPool = 10;

    private float minX = -5.5f;
    private float maxX = 5.5f;
    private float xPos;

    private bool bomberGoing;
    private bool makeZoneBool;
    private bool dropping;

    public float dropTime;
    private float dropTimer;
    public float spawnTime;
    private float spawnTimer;

    void Start()
    {
        spawnTransform = bombSpawn.GetComponent<Transform>();
        xPos = Random.Range(minX, maxX);

        // for (int i = 0; i < amountToPool; i++)
        // {
        //     GameObject tmp = Instantiate(bombPrefab);
        //     tmp.SetActive(false);
        //     pooledBombs.Enqueue(tmp);
        // }
    }

    void Update()
    {
        if (bomberGoing)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        Drop();
        SpawnDropZone();
        Restart();
    }

    //     public GameObject GetPooledObject()
    //     {
    //         if (pooledBombs.Count > 0 && !pooledBombs.Peek().activeInHierarchy)
    //         {
    //             return pooledBombs.Dequeue();
    //         }
    //         return null;
    //     }
    //     public void ReturnBombToPool(GameObject bomb)
    // {
    //     bomb.SetActive(false);
    //     pooledBombs.Enqueue(bomb);
    // }

    void Drop()
    {
        dropTimer += Time.deltaTime;
        if (dropping && dropTimer >= dropTime)
        {
            ID.GetBomb(spawnTransform.position);
            // if (bomb != null)
            // {
            //     bomb.transform.position = spawnTransform.position;
            //     bomb.SetActive(true);
            // }
            dropTimer = 0;
        }
    }

    void SpawnDropZone()
    {
        if (makeZoneBool && bomberGoing)
        {
            if (dropZone == null)
            {
                dropZone = Instantiate(dropZonePrefab, (new Vector3(xPos, .3f, 0f)), Quaternion.identity);
            }
            dropZone.SetActive(true);
        }
        makeZoneBool = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "DropZone")
        {
            dropping = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "DropZone")
        {
            dropping = false;
            dropZone.SetActive(false);
        }
    }

    void Restart()
    {
        if (transform.position.x < -12)
        {
            bomberGoing = false;
            transform.position = new Vector3(BoundariesManager.rightBoundary, 4.15f, 1f);
        }
        if (!bomberGoing)
        {
            spawnTimer += Time.deltaTime;
            xPos = Random.Range(minX, maxX);
        }
        if (spawnTimer >= spawnTime)
        {
            bomberGoing = true;
            makeZoneBool = true;
            spawnTimer = 0;
        }
    }
}
