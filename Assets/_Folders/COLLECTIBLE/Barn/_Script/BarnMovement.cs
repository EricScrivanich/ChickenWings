using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnMovement : SpawnedObject, IRecordableObject
{
    private int barnType;
    private ParticleSystem ps;
    public PlayerID playerID;
    public BarnID ID;
    [SerializeField] private SpriteRenderer leftSide;
    [SerializeField] private SpriteRenderer rightSide;
    [SerializeField] private SpriteRenderer smallHay;
    private BoxCollider2D coll;
    private int speed;


    private float fullSize = 4.7f;
    private float middleSize = 3.85f;
    private float smallSize = 3;
    private float midOffset = .44f;
    private Vector2 noOffset = new Vector2(0, .5f);
    private Vector2 rightOffset = new Vector2(.45f, .5f);
    private Vector2 leftOffset = new Vector2(-.45f, .5f);

    private Vector2 smallCollSize = new Vector2(2.8f, 1);
    private Vector2 midCollSize = new Vector2(3.8f, 1);
    private Vector2 bigCollSize = new Vector2(4.8f, 1);



    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        ps = GetComponent<ParticleSystem>();

    }


    public void Initialize(int barnTypeVar)
    {

        switch (barnTypeVar)
        {

            case 0:

                //both sides activated

                leftSide.sprite = ID.barnSide;
                rightSide.sprite = ID.barnSide;


                SetCollider(noOffset, bigCollSize);
                smallHay.sprite = ID.GetRandomSmallHay();
                smallHay.transform.localPosition = new Vector2(Random.Range(-2f, 2f), 0);
                break;
            case 1:

                //left side activated

                leftSide.sprite = ID.barnSide;
                rightSide.sprite = ID.GetRandomBigHay();
                SetCollider(leftOffset, midCollSize);

                smallHay.transform.localPosition = new Vector2(Random.Range(-2f, -.7f), 0);
                smallHay.sprite = ID.GetRandomSmallHay();

                break;

            case 2:

                //right side activated


                rightSide.sprite = ID.barnSide;
                leftSide.sprite = ID.GetRandomBigHay();
                SetCollider(rightOffset, midCollSize);
                smallHay.sprite = ID.GetRandomSmallHay();
                smallHay.transform.localPosition = new Vector2(Random.Range(.7f, 2f), 0);

                break;

            case 3:

                //no sides activated

                int randomInt = Random.Range(0, 2);
                smallHay.sprite = ID.GetRandomSmallHay();

                SetCollider(noOffset, smallCollSize);


                if (randomInt == 0)
                {
                    rightSide.sprite = null;
                    leftSide.sprite = ID.GetRandomBigHay();
                    smallHay.transform.localPosition = new Vector2(Random.Range(.7f, 2f), 0);

                }
                else
                {
                    leftSide.sprite = null;
                    rightSide.sprite = ID.GetRandomBigHay();
                    smallHay.transform.localPosition = new Vector2(Random.Range(-2f, -.7f), 0);

                }


                break;

        }


        this.gameObject.SetActive(true);

    }

    public void CollectChicAnimation(Transform chic)
    {

    }

    private void SetCollider(Vector2 offset, Vector2 size)
    {
        coll.offset = offset;
        coll.size = size;


    }
    public void EggHit()
    {
        // if (ps.isPlaying)
        // {
        //     ps.Stop();
        //     ps.Play();
        // }
        ps.Play();
        AudioManager.instance.PlayScoreSound();
        playerID.AddBarnHit(objectID);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * BoundariesManager.GroundSpeed * Time.deltaTime);

        if (transform.position.x < BoundariesManager.leftBoundary)
        {
            gameObject.SetActive(false);
        }

    }

    public override void ApplyFloatOneData(DataStructFloatOne data)
    {
        transform.position = data.startPos;
        Initialize((int)data.type);
    }


    private int lastDataType;

    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        if (lastDataType == data.type) return;
        lastDataType = data.type;
        switch (data.type)
        {
            case 0:
                leftSide.gameObject.SetActive(true);
                rightSide.gameObject.SetActive(true);
                break;
            case 1:
                leftSide.gameObject.SetActive(true);
                rightSide.gameObject.SetActive(false);

                break;
            case 2:
                leftSide.gameObject.SetActive(false);
                rightSide.gameObject.SetActive(true);

                break;
            case 3:
                leftSide.gameObject.SetActive(false);
                rightSide.gameObject.SetActive(false);

                break;
        }


    }

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
        return new Vector2(currPos.x - (BoundariesManager.GroundSpeed * time), transform.position.y);
    }

    public float ReturnPhaseOffset(float x)
    {
        return 0;
    }
}
