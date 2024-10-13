using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;
using DG.Tweening;

public class DropBomb : MonoBehaviour
{
    // private Pool pool;

    public float xDropPosition;
    public float dropAreaScaleMultiplier;
    public float speedTarget;





    [SerializeField] private ExplosivesPool pool;

    [SerializeField] private GameObject dropZonePrefab;
    private DropZone dropZone;
    [SerializeField] private float delayToFlyOverSound;
    [SerializeField] private float delayToBomberGoing;
    [SerializeField] private float startSpeedMultiplier;
    [SerializeField] private float speedChangeDuration;
    [SerializeField] private float StartY;
    [SerializeField] private float EndY;
    [SerializeField] private float StartRot;
    [SerializeField] private float entraceDuration;
    [SerializeField] private float exitDuration;
    [SerializeField] private float xDistanceFromZone;

    private Sequence droppingTweenSeq;

    private Vector3 rotateMidTarget = new Vector3(0, 0, 7);






    [SerializeField] private SpriteRenderer propellar;
    [SerializeField] private SpriteRenderer dropArea;
    private Transform spawnTransform;


    [SerializeField] private Sprite[] propSprites;
    [SerializeField] private Sprite[] dropAreaSprites;

    private readonly int spriteSize = 2;

    private int currentProp = 0;
    private int currentDropArea = 0;

    private float propTime = 0;





    private float speed;

    private Queue<GameObject> pooledBombs = new Queue<GameObject>();
    [SerializeField] private GameObject bombPrefab;



    public PlaneManagerID ID;
    private int amountToPool = 10;
    private Vector3 bombRotation = new Vector3(0, 0, 120);

    private float minX = -5.5f;
    private float maxX = 5.5f;
    private float xPos;

    private bool bomberGoing = false;
    private bool makeZoneBool;
    private bool dropping;

    public float dropTime;
    private float dropTimer;
    public float spawnTime;
    private float spawnTimer;

    private bool enteredZone = false;

    private float speedStart;
    private float speedEnd;

    private bool hasInitialized = false;

    void Start()
    {

        ID.finsihedDrop = false;


        dropZone = Instantiate(dropZonePrefab).GetComponent<DropZone>();
        dropZone.gameObject.SetActive(false);


        // pool = PoolKit.GetPool("ExplosionPool");

        xPos = Random.Range(minX, maxX);



        dropZone.Initilaize(xDropPosition, dropAreaScaleMultiplier);

        StartCoroutine(BomberStart());
        hasInitialized = true;





        // for (int i = 0; i < amountToPool; i++)
        // {
        //     GameObject tmp = Instantiate(bombPrefab);
        //     tmp.SetActive(false);
        //     pooledBombs.Enqueue(tmp);
        // }
    }

    void Update()
    {
        // if (bomberGoing)
        // {
        //     transform.position += Vector3.left * speed * Time.deltaTime;
        // }

        if (bomberGoing)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;


            propTime += Time.deltaTime;

            if (propTime > .07f)
            {
                currentProp++;
                if (currentProp > spriteSize) currentProp = 0;

                propellar.sprite = propSprites[currentProp];
                propTime = 0;
            }
        }


        // Drop();

        // Restart();
    }

    private void TweenWhileDropping(bool doTween)
    {
        if (droppingTweenSeq != null && droppingTweenSeq.IsPlaying())
            droppingTweenSeq.Kill();


        if (doTween)
        {
            droppingTweenSeq = DOTween.Sequence();

            droppingTweenSeq.Append(transform.DORotate(rotateMidTarget, 1));
            droppingTweenSeq.Join(transform.DOMoveY(transform.position.y - 1, 1));
        }


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
            // pool.Spawn("bomberBomb", dropArea.transform.position, bombRotation);

            // pool.GetBomb(dropArea.transform.position,bombRotation,)
            // if (bomb != null)
            // {
            //     bomb.transform.position = spawnTransform.position;
            //     bomb.SetActive(true);
            // }
            dropTimer = 0;
        }
    }

    public void Initilaize()
    {

    }

    private void EnterOrExitTween(bool enter)
    {


        if (enter)
        {
            transform.eulerAngles = new Vector3(0, transform.rotation.y, StartRot);
            Vector3 endRot = new Vector3(0, transform.rotation.y, 0);
            transform.position = new Vector2(xDropPosition + xDistanceFromZone, StartY);



            transform.DOLocalMoveY(EndY, entraceDuration).SetEase(Ease.OutBack);
            // transform.DORotate(endRot, entraceDuration).OnComplete(() => TweenWhileDropping(true));
            transform.DORotate(endRot, entraceDuration);
        }
    }

    private void ChangeDropAreaSprite()
    {
        StartCoroutine(SwitchDropAreaSprite());

    }

    private IEnumerator SwitchDropAreaSprite()
    {
        float time = 0;

        if (!enteredZone)
        {

            while (time < speedChangeDuration)
            {
                time += Time.deltaTime;

                speed = Mathf.Lerp(speedStart, speedEnd, time / speedChangeDuration);
            }
            for (int i = 0; i < spriteSize; i++)
            {
                yield return new WaitForSeconds(.2f);

                dropArea.sprite = dropAreaSprites[i];
            }
            enteredZone = true;
            yield return new WaitForSeconds(.2f);
            dropArea.sprite = dropAreaSprites[spriteSize];
            yield break;
        }
        else
        {
            for (int i = spriteSize; i > -1; i--)
            {
                yield return new WaitForSeconds(.25f);

                dropArea.sprite = dropAreaSprites[i];
            }

        }
    }

    void SpawnDropZone()
    {

        // if (dropZone == null)
        // {
        //     dropZone = Instantiate(dropZonePrefab, (new Vector3(xPos, .3f, 0f)), Quaternion.identity);
        // }
        // dropZone.SetActive(true);

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("DropZone"))
        {
            ID.finsihedDrop = false;
            dropping = true;
            StartCoroutine(DropBombs());

        }
    }

    private void Testwait()
    {
        ID.finsihedDrop = true;


    }

    private void OnEnable()
    {


        speedEnd = speedTarget;
        speedStart = speedTarget * startSpeedMultiplier;
        speed = speedStart;


        if (hasInitialized)
        {
            ID.finsihedDrop = false;


            // dropZone = Instantiate(dropZonePrefab).GetComponent<DropZone>();
            // dropZone.gameObject.SetActive(false);


            // pool = PoolKit.GetPool("ExplosionPool");





            dropZone.Initilaize(xDropPosition, dropAreaScaleMultiplier);

            StartCoroutine(BomberStart());

        }

    }


    private IEnumerator BomberStart()
    {
        AudioManager.instance.PlayAirRaidSiren();

        yield return new WaitForSeconds(delayToFlyOverSound);
        AudioManager.instance.PlayFlyOver();
        yield return new WaitForSeconds(delayToBomberGoing);
        EnterOrExitTween(true);
        ChangeDropAreaSprite();
        bomberGoing = true;



    }

    private IEnumerator DropBombs()
    {
        int dropCount = 2;
        while (dropping)
        {
            // pool.Spawn("bomberBomb", dropArea.transform.position, bombRotation);

            pool.GetBomb(dropArea.transform.position, bombRotation, dropCount, true);
            dropCount--;

            yield return new WaitForSeconds(.1f);
            AudioManager.instance.PlayBombDroppedSound();
            yield return new WaitForSeconds(dropTime);

        }
        StartCoroutine(SwitchDropAreaSprite());
        yield return new WaitForSeconds(.2f);
        // TweenWhileDropping(false);
        Vector3 endRot = new Vector3(0, transform.rotation.y, -StartRot);
        dropZone.FadeOut();

        transform.DOLocalMoveY(StartY, exitDuration).SetEase(Ease.InBack);
        transform.DORotate(endRot, exitDuration);

    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("DropZone"))
        {

            // Invoke("Testwait", .7f);
            dropping = false;

            // dropZone.SetActive(false);
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
