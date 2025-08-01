using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;
using DG.Tweening;

public class DropBomb : MonoBehaviour, IRecordableObject
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
    private Sequence movePlaneSeq;

    private Vector3 rotateMidTarget = new Vector3(0, 0, 7);



    private Rigidbody2D rb;

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

    private float xEnter;
    private float xExit;

    private bool hasInitialized = false;

    private Vector3 nomralScale;
    private bool hasSpawned = false;

    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        nomralScale = transform.localScale;
        dropZone = Instantiate(dropZonePrefab).GetComponent<DropZone>();
        dropZone.gameObject.SetActive(false);

    }
    void Start()
    {
        hasSpawned = true;






        // pool = PoolKit.GetPool("ExplosionPool");

        // xPos = Random.Range(minX, maxX);
        // if (!hasInitialized && !ignoreAll)
        // {
        //     if (dropAreaScaleMultiplier == 0) dropAreaScaleMultiplier = 1;
        //     dropZone.Initilaize(xDropPosition, dropAreaScaleMultiplier * sideSwitchInteger, planeStartX, sideSwitchInteger);
        //     float u = dropZone.AddedUnits();
        //     xEnter = xDropPosition + ((u + 3) * sideSwitchInteger);
        //     xExit = xDropPosition - ((u + 4.7f) * sideSwitchInteger);
        //     Debug.Log("Added Units is: " + u + " side switch is: " + sideSwitchInteger);
        //     Debug.Log("Bomber initialized with xEnter: " + xEnter + " and xExit: " + xExit);

        //     StartCoroutine(BomberStart());
        //     hasInitialized = true;
        // }







        // for (int i = 0; i < amountToPool; i++)
        // {
        //     GameObject tmp = Instantiate(bombPrefab);
        //     tmp.SetActive(false);
        //     pooledBombs.Enqueue(tmp);
        // }
    }

    private bool logstart = false;
    void Update()
    {
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
        if (!hasEnteredTriggerArea && ((sideSwitchInteger == 1 && transform.position.x < xEnter) || (sideSwitchInteger == -1 && transform.position.x > xEnter)))
        {
            Debug.Log("Entered area");
            // Debug.Log("Bomber entered trigger: " + (Time.time - spawnTimeTest) + " seconds, with scale of: " + dropAreaScaleMultiplier);
            hasEnteredTriggerArea = true;
            dropping = true;
            if (!ignoreAll)
                StartCoroutine(DropBombs());
        }

        if (dropping && ((sideSwitchInteger == 1 && transform.position.x < xExit) || (sideSwitchInteger == -1 && transform.position.x > xExit)))
        {
            Debug.Log("Exit area");

            dropping = false;
        }

        //9.3 1.92


        // if (bomberGoing)
        // {
        //     // if (!logstart)
        //     // {
        //     //     Debug.Log("Bomber started at: " + (transform.position.x - xDropPosition) + " at time: " + (Time.time - spawnTimeTest));
        //     //     logstart = true;
        //     // }
        //     // transform.position += Vector3.left * speed * Time.deltaTime;



        // }


        // Drop();

        // Restart();
    }

    // private void FixedUpdate()
    // {
    //     if (bomberGoing)
    //         rb.MovePosition(rb.position + Vector2.left * speed * Time.fixedDeltaTime);

    // }

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

            transform.eulerAngles = new Vector3(0, 0, StartRot * sideSwitchInteger);
            Vector3 endRot = new Vector3(0, 0, 0);
            transform.position = new Vector2(xDropPosition + (xDistanceFromZone * sideSwitchInteger), StartY);

            movePlaneSeq = DOTween.Sequence();
            movePlaneSeq.Append(transform.DOLocalMoveY(EndY, entraceDuration).SetEase(Ease.OutBack));
            movePlaneSeq.Join(transform.DORotate(endRot, entraceDuration));
            movePlaneSeq.Play();


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



    private void OnDisable()
    {
        Debug.Log("Bomber disabled");

        StopAllCoroutines();

        if (!hasFadedDropZone && hasSpawned)
        {
            hasFadedDropZone = true;
            dropZone.FadeOut();
        }
        if (ignoreAll && dropZone != null) dropZone.gameObject.SetActive(false);

        if (droppingTweenSeq != null && droppingTweenSeq.IsPlaying())
            droppingTweenSeq.Kill();
        if (movePlaneSeq != null && movePlaneSeq.IsPlaying())
            movePlaneSeq.Kill();


    }

    private void OnDestroy()
    {

    }

    public void Stop()
    {
        if (droppingTweenSeq != null && droppingTweenSeq.IsPlaying())
            droppingTweenSeq.Kill();

        if (!hasFadedDropZone)
        {
            hasFadedDropZone = true;
            dropZone.FadeOut();
        }

        StopAllCoroutines();
    }


    private void OnEnable()
    {
        if (ignoreAll) return;

    }
    private void OnSpawned()
    {

        enteredZone = false;
        hasFadedDropZone = false;


        bomberGoing = false;
        hasEnteredTriggerArea = false;

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
        transform.position = new Vector2(planeStartX, StartY + 1);


        if (dropAreaScaleMultiplier == 0) dropAreaScaleMultiplier = 1;

        dropZone.Initilaize(xDropPosition, dropAreaScaleMultiplier * sideSwitchInteger, planeStartX, sideSwitchInteger);
        float u = dropZone.AddedUnits();
        xEnter = xDropPosition + ((u + 3) * sideSwitchInteger);
        xExit = xDropPosition - ((u + 4.7f) * sideSwitchInteger);

        StartCoroutine(BomberStart());

    }
    private WaitForSeconds wait1 = new WaitForSeconds(1.5f);
    private WaitForSeconds wait2 = new WaitForSeconds(.4f);

    private IEnumerator BomberStart()
    {

        AudioManager.instance.PlayAirRaidSiren();

        yield return wait1;
        AudioManager.instance.PlayFlyOver();
        yield return wait2;
        EnterOrExitTween(true);


        ChangeDropAreaSprite();
        bomberGoing = true;



    }
    private WaitForSeconds wait = new WaitForSeconds(.08f);

    private IEnumerator DropBombs()
    {
        int dropCount = 2;

        if (sideSwitchInteger == -1) dropCount = 0;


        while (dropping)
        {
            // pool.Spawn("bomberBomb", dropArea.transform.position, bombRotation);


            if (!dropping) break;

            pool.GetBomb(dropArea.transform.position, bombRotation * sideSwitchInteger, dropCount, true);
            yield return wait;
            if (sideSwitchInteger == 1) dropCount--;

            AudioManager.instance.PlayBombDroppedSound();
            yield return wait;


        }
        StartCoroutine(SwitchDropAreaSprite());
        yield return new WaitForSeconds(.2f);
        // TweenWhileDropping(false);
        Vector3 endRot = new Vector3(0, 0, -StartRot * sideSwitchInteger);
        hasFadedDropZone = true;
        dropZone.FadeOut();

        movePlaneSeq = DOTween.Sequence();
        movePlaneSeq.Append(transform.DOLocalMoveY(StartY, exitDuration).SetEase(Ease.InBack));
        movePlaneSeq.Join(transform.DORotate(endRot, exitDuration).OnComplete(() => gameObject.SetActive(false)));
        movePlaneSeq.Play();



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


    public void ApplyFloatOneData(DataStructFloatOne data)
    {
        ignoreAll = false;
        xDropPosition = data.startPos.x;
        dropAreaScaleMultiplier = data.float1;
        if (data.type == 0)
        {

            speedTarget = 8;
        }
        else
        {

            speedTarget = -7;

        }
        gameObject.SetActive(true);
        OnSpawned();

    }
    public void ApplyFloatTwoData(DataStructFloatTwo data)
    {

    }
    public void ApplyFloatThreeData(DataStructFloatThree data)
    {

    }
    public void ApplyFloatFourData(DataStructFloatFour data)
    {


    }
    public void ApplyFloatFiveData(DataStructFloatFive data)
    {

    }

    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        hasEnteredTriggerArea = true;
        dropping = false;
        Debug.Log("Applying Data");

        xDropPosition = data.startPos.x;
        if (data.type == 0)
        {
            sideSwitchInteger = 1;
            transform.localScale = nomralScale;
            speedTarget = 8;
        }
        else
        {
            transform.localScale = new Vector3(-nomralScale.x, nomralScale.y, nomralScale.z);
            speedTarget = -7;
            sideSwitchInteger = -1;
        }

        planeStartX = xDropPosition + ((xDistanceFromZone - 1) * sideSwitchInteger);

        dropZone.ApplyCustomizedData(data.startPos.x, data.float1, planeStartX, sideSwitchInteger);
        float u = dropZone.AddedUnits();
        xEnter = xDropPosition + ((u + 3) * sideSwitchInteger);
        xExit = xDropPosition - ((u + 4.7f) * sideSwitchInteger);
        ignoreAll = true;


    }
    private bool ignoreAll = false;

    public bool ShowLine()
    {
        return false;
    }

    public float TimeAtCreateObject(int index)
    {
        throw new System.NotImplementedException();
    }

    public Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset)
    {
        if (time < 1.92f)
        {
            return new Vector2(xDropPosition + (xDistanceFromZone * sideSwitchInteger), StartY);

        }
        else
        {
            return new Vector2(xDropPosition + (9.3f * sideSwitchInteger) - ((time - 1.92f) * speedTarget), EndY);
        }
    }

    public float ReturnPhaseOffset(float x)
    {
        Debug.Log("Returning Data");

        if ((sideSwitchInteger == 1 && x < xExit - 5.5f) || (sideSwitchInteger == -1 && x > xExit + 5.5f))
        {
            Debug.Log("Returning Data 2 with side " + sideSwitchInteger);
            return -1;
        }

        else return 1;
    }
}
