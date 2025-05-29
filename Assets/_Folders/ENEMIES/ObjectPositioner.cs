using UnityEngine;
using DG.Tweening;

public class ObjectPositioner : MonoBehaviour, IRecordableObject
{

    [SerializeField] private float testPathTime = 1f;

    [SerializeField] private Ease testPathEase;
    private Rigidbody2D rb;
    private float currentRotation;
    [SerializeField] private float rotationSpeed = 1f;
    private LaserParent laser;
    [SerializeField] private float startRotation;

    public enum ObjectType
    {
        RedLaser,

        Bomb
    }
    [SerializeField] private ObjectType objectType;


    [SerializeField] private Transform directionArrowParent;

    [SerializeField] private SpriteRenderer[] directionArrows;


    [Header("Posioning Settings")]
    [SerializeField] private Vector2[] positionList;
    [SerializeField] private float[] positionTimeList;
    [SerializeField] private float[] positionDelayList;

    [SerializeField] private int[] rotationList;
    [SerializeField] private float[] rotationTimeList;
    [SerializeField] private float[] rotationDelayList;










    private int currentPostionIndex;
    private int currentRotationIndex;

    private Sequence MoveSeq;
    private Sequence RotationSeq;
    private Sequence ArrowSeq;
    private bool moving;

    private bool rotatingTween;
    [SerializeField] private float fadeArrowDur;




    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        laser = GetComponent<LaserParent>();

    }
    void Start()
    {


        transform.rotation = Quaternion.Euler(0, 0, startRotation);



        if (directionArrows != null && directionArrows.Length > 0)
        {
            foreach (var s in directionArrows)
            {
                s.DOFade(0, 0);
            }

            directionArrowParent.gameObject.SetActive(false);
        }

        DoMoveSequence();
        DoRotateSequence();


    }

    private void DoRotateSequence()
    {

        if (rotationList == null || rotationList.Length <= 0) return;

        rotatingTween = true;
        currentRotationIndex = 0;

        RotationSeq = DOTween.Sequence();

        for (int i = 0; i < rotationList.Length; i++)
        {
            int index = i; // capture local index for closure

            RotationSeq.Append(rb.DORotate(rotationList[index], rotationTimeList[index]).SetEase(Ease.Linear));



            RotationSeq.AppendInterval(rotationDelayList[index]);


        }

        RotationSeq.Play().SetUpdate(UpdateType.Fixed).OnComplete(() => rotatingTween = false);







    }

    private void DoMoveSequence()
    {
        if (positionList == null || positionList.Length <= 0) return;
        moving = true;
        MoveSeq = DOTween.Sequence();

        currentPostionIndex = -1; // reset so it starts clean
        DoHandleArrows(0); // show the first arrow right away

        for (int i = 0; i < positionList.Length; i++)
        {
            int index = i; // capture local index for closure

            MoveSeq.Append(rb.DOMove(positionList[index], positionTimeList[index])
                .SetEase(testPathEase)
                .OnComplete(() => StopArrows()));
            Debug.Log("rotation at time is" + MoveSeq.PathGetPoint(testPathTime));

            MoveSeq.AppendInterval(positionDelayList[index]);

            MoveSeq.AppendCallback(() => DoHandleArrows(index + 1));
        }



        MoveSeq.Play().SetUpdate(UpdateType.Fixed).OnComplete(() => moving = false);
    }

    private void DoHandleArrows(int arrowIndex)
    {
        if (arrowIndex >= positionList.Length) return;


        currentPostionIndex = arrowIndex;

        if (directionArrowParent != null)
        {
            if (ArrowSeq != null && ArrowSeq.IsPlaying()) ArrowSeq.Kill();

            Vector2 direction = positionList[arrowIndex] - (Vector2)transform.position;
            direction.Normalize();

            float zRot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            directionArrowParent.eulerAngles = new Vector3(0, 0, zRot + 90);
            directionArrowParent.gameObject.SetActive(true);

            ArrowSeq = DOTween.Sequence();
            ArrowSeq.Append(directionArrows[2].DOFade(0, fadeArrowDur));
            ArrowSeq.Join(directionArrows[0].DOFade(1, fadeArrowDur));
            ArrowSeq.Append(directionArrows[0].DOFade(0, fadeArrowDur));
            ArrowSeq.Join(directionArrows[1].DOFade(1, fadeArrowDur));
            ArrowSeq.Append(directionArrows[1].DOFade(0, fadeArrowDur));
            ArrowSeq.Join(directionArrows[2].DOFade(1, fadeArrowDur));
            ArrowSeq.SetLoops(-1).Play();
        }
    }
    private void StopArrows()
    {
        if (ArrowSeq != null && ArrowSeq.IsPlaying()) ArrowSeq.Kill();
        foreach (var s in directionArrows)
        {
            s.DOFade(0, 0);
        }
        directionArrowParent.gameObject.SetActive(false);


    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (laser != null && (rotationSpeed != 0 || moving))
        {
            laser.UpdateLaserPostions();
        }

        // if (rotationSpeed == 0) return;
        // rb.MoveRotation(currentRotation);
        // currentRotation += rotationSpeed * Time.fixedDeltaTime;



    }

    public void ApplyFloatOneData(DataStructFloatOne data)
    {
        throw new System.NotImplementedException();
    }

    public void ApplyFloatTwoData(DataStructFloatTwo data)
    {
        throw new System.NotImplementedException();
    }

    public void ApplyFloatThreeData(DataStructFloatThree data)
    {
        throw new System.NotImplementedException();
    }

    public void ApplyFloatFourData(DataStructFloatFour data)
    {
        throw new System.NotImplementedException();
    }

    public void ApplyFloatFiveData(DataStructFloatFive data)
    {
        throw new System.NotImplementedException();
    }

    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        laser.SetLaserAmount(data.type + 1, data.float2);



    }

    public void SetPercentForLaserShooting(float percent)
    {
        laser.SetLaserPercentForRecording(percent);
    }

    public void ApplyTransformData(Vector2 pos, float rot)
    {

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
        return Vector2.zero;
    }

    public float ReturnPhaseOffset(float x)
    {
        laser.SetLaserPercentForRecording(x);
        return 0;
    }
}
