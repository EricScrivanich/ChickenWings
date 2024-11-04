using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnMovement : MonoBehaviour
{
    private int barnType;
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

    }
    public void Initilaize(int barnTypeVar)
    {
        Debug.Log("Barn Index IS: " + barnTypeVar);
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

    private void SetCollider(Vector2 offset, Vector2 size)
    {
        coll.offset = offset;
        coll.size = size;


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
}
