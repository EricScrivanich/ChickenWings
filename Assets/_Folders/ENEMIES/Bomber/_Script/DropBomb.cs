using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class DropBomb : MonoBehaviour
{
    private Pool pool;
    [SerializeField] private GameObject bombSpawn;
    private Transform spawnTransform;
    [SerializeField] private GameObject dropZonePrefab;

    private GameObject dropZone;

    [SerializeField] private float speed;

    private Queue<GameObject> pooledBombs = new Queue<GameObject>();
    [SerializeField] private GameObject bombPrefab;

    public PlaneManagerID ID;
    private int amountToPool = 10;
    private Vector3 bombRotation;

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
        ID.finsihedDrop = false;

        bombRotation = new Vector3(0, 0, -30);
        pool = PoolKit.GetPool("ExplosionPool");
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
            ID.bombsDropped++;

            // ID.GetBomb(spawnTransform.position);
            pool.Spawn("bomberBomb", spawnTransform.position, bombRotation);
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

        if (dropZone == null)
        {
            dropZone = Instantiate(dropZonePrefab, (new Vector3(xPos, .3f, 0f)), Quaternion.identity);
        }
        dropZone.SetActive(true);

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("DropZone"))
        {
            ID.finsihedDrop = false;
            dropping = true;

        }
    }

    private void Testwait()
    {
        ID.finsihedDrop = true;


    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("DropZone"))
        {

            Invoke("Testwait", .7f);
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
        if (spawnTimer > ID.bomberTime)
        // if (spawnTimer > 5)
        {
            bomberGoing = true;
            SpawnDropZone();
            ID.bombsDropped = 0;
            spawnTimer = 0;
        }
    }
}
