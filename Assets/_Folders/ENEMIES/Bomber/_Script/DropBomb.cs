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
    private bool hasFadedDropZone = false;
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

    private BoxCollider2D col;



    private bool hasEnteredTriggerArea = false;


    [SerializeField] private SpriteRenderer propellar;
    [SerializeField] private SpriteRenderer dropArea;
    private Transform spawnTransform;

    private float planeStartX;
    [SerializeField] private Sprite[] propSprites;
    [SerializeField] private Sprite[] dropAreaSprites;

    private readonly int spriteSize = 2;

    private int currentProp = 0;
    private int currentDropArea = 0;

    private float propTime = 0;

    private int sideSwitchInteger = 1;





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

    private Vector3 nomralScale;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        nomralScale = transform.localScale;
        dropZone = Instantiate(dropZonePrefab).GetComponent<DropZone>();
        dropZone.gameObject.SetActive(false);

    }
    void Start()
    {







        // pool = PoolKit.GetPool("ExplosionPool");

        // xPos = Random.Range(minX, maxX);
        if (!hasInitialized)
        {
            if (dropAreaScaleMultiplier == 0) dropAreaScaleMultiplier = 1;
            dropZone.Initilaize(xDropPosition, dropAreaScaleMultiplier * sideSwitchInteger, planeStartX, sideSwitchInteger);

            StartCoroutine(BomberStart());
            hasInitialized = true;
        }







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
            Debug.LogError("Enter Tween started");
            transform.eulerAngles = new Vector3(0, 0, StartRot * sideSwitchInteger);
            Vector3 endRot = new Vector3(0, 0, 0);
            transform.position = new Vector2(xDropPosition + (xDistanceFromZone * sideSwitchInteger), StartY);



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
        if (collider.gameObject.CompareTag("DropZone") && !hasEnteredTriggerArea)
        {
            hasEnteredTriggerArea = true;
            dropping = true;
            StartCoroutine(DropBombs());
        }
    }

    private void OnDisable()
    {
        if (droppingTweenSeq != null && droppingTweenSeq.IsPlaying())
            droppingTweenSeq.Kill();

        if (!hasFadedDropZone)
        {
            hasFadedDropZone = true;
            dropZone.FadeOut();
        }
        DOTween.Kill(this);
        StopAllCoroutines();
    }


    private void OnEnable()
    {
        enteredZone = false;
        hasFadedDropZone = false;

        bomberGoing = false;
        hasEnteredTriggerArea = false;
        col.enabled = false;
        if (speedTarget < 0)
        {
            sideSwitchInteger = -1;
            transform.localScale = new Vector3(-nomralScale.x, nomralScale.y, nomralScale.z);
        }
        else
        {
            sideSwitchInteger = 1;
            transform.localScale = nomralScale;
        }

        planeStartX = xDropPosition + ((xDistanceFromZone - 1) * sideSwitchInteger);
        speedEnd = speedTarget;
        speedStart = speedTarget * startSpeedMultiplier;
        speed = speedStart;

        if (hasInitialized)
        {
            if (dropAreaScaleMultiplier == 0) dropAreaScaleMultiplier = 1;

            dropZone.Initilaize(xDropPosition, dropAreaScaleMultiplier * sideSwitchInteger, planeStartX, sideSwitchInteger);

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
        col.enabled = true;

        ChangeDropAreaSprite();
        bomberGoing = true;



    }

    private IEnumerator DropBombs()
    {
        int dropCount = 2;

        if (sideSwitchInteger == -1) dropCount = 0;
        while (dropping)
        {
            // pool.Spawn("bomberBomb", dropArea.transform.position, bombRotation);

            pool.GetBomb(dropArea.transform.position, bombRotation * sideSwitchInteger, dropCount, true);
            if (sideSwitchInteger == 1) dropCount--;

            yield return new WaitForSeconds(.1f);
            AudioManager.instance.PlayBombDroppedSound();
            yield return new WaitForSeconds(dropTime);

        }
        StartCoroutine(SwitchDropAreaSprite());
        yield return new WaitForSeconds(.2f);
        // TweenWhileDropping(false);
        Vector3 endRot = new Vector3(0, 0, -StartRot * sideSwitchInteger);
        hasFadedDropZone = true;
        dropZone.FadeOut();

        transform.DOLocalMoveY(StartY, exitDuration).SetEase(Ease.InBack);
        transform.DORotate(endRot, exitDuration).OnComplete(() => gameObject.SetActive(false));

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
