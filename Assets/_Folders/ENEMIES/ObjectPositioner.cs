using UnityEngine;
using DG.Tweening;

public class ObjectPositioner : MonoBehaviour, IRecordableObject
{

    [SerializeField] private float testPathTime = 1f;

    [SerializeField] private Ease testPathEase;
    private Rigidbody2D rb;
    private float currentRotation;
    [SerializeField] private float rotationSpeed = 1f;
    private IPositionerObject addedComponent;
    [SerializeField] private float startRotation;
    private Ease[] eases = new Ease[] { Ease.Linear, Ease.InSine, Ease.OutSine, Ease.InOutSine };

    public enum ObjectType
    {
        RedLaser,

        Bomb
    }
    [SerializeField] private ObjectType objectType;


    [SerializeField] private Transform directionArrowParent;

    [SerializeField] private SpriteRenderer[] directionArrows;


    [Header("Posioning Settings")]
    private Vector2[] positionList;
    private float[] positionTimeList;
    private float[] positionDelayList;
    private ushort[] positionEases;

    private float[] rotationList;
    private float[] rotationTimeList;
    private float[] rotationDelayList;
    private ushort[] rotationEases;










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
        addedComponent = GetComponent<IPositionerObject>();


    }
    void Start()
    {

        // directionArrowParent.gameObject.SetActive(false);
        // transform.rotation = Quaternion.Euler(0, 0, startRotation);






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

            RotationSeq.AppendInterval(rotationDelayList[index]);
            RotationSeq.Append(rb.DORotate(rotationList[index], rotationTimeList[index]).SetEase(eases[rotationEases[index]]));






        }

        RotationSeq.Play().SetUpdate(UpdateType.Fixed).OnComplete(() => rotatingTween = false);







    }

    private void DoMoveSequence()
    {
        if (positionList == null || positionList.Length <= 0) return;

        MoveSeq = DOTween.Sequence();

        currentPostionIndex = -1; // reset so it starts clean
        // DoHandleArrows(0); // show the first arrow right away

        for (int i = 0; i < positionList.Length; i++)
        {
            int index = i; // capture local index for closure

            Debug.Log("Adding move sequence for index: " + index + " with position: " + positionList[index] + " and time: " + positionTimeList[index] + " and delay: " + positionDelayList[index] + " and ease: " + eases[positionEases[index]]);
            MoveSeq.AppendInterval(positionDelayList[index]);

            MoveSeq.AppendCallback(() => DoHandleArrows(index));

            MoveSeq.Append(rb.DOMove(positionList[index], positionTimeList[index])
                .SetEase(eases[positionEases[index]])
                .OnComplete(() => StopArrows()));



        }



        MoveSeq.Play().SetUpdate(UpdateType.Fixed);
    }
    int arrowIndex = 0;
    private void DoHandleArrows(int arrowIndex)
    {
        if (arrowIndex >= positionList.Length) return;

        moving = true;
        currentPostionIndex = arrowIndex;

        if (directionArrowParent != null)
        {
            if (ArrowSeq != null && ArrowSeq.IsPlaying()) ArrowSeq.Kill();

            Vector2 direction = positionList[arrowIndex] - (Vector2)transform.position;
            direction.Normalize();

            float zRot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            directionArrowParent.eulerAngles = new Vector3(0, 0, zRot + 90);
            directionArrowParent.gameObject.SetActive(true);
            Debug.LogError("Showing arrows for index: " + arrowIndex + " with direction: " + direction + " and rotation: " + zRot);


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
        moving = false;
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
        if (addedComponent != null && (rotationSpeed != 0 || moving))
        {
            addedComponent.DoUpdateTransform();
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
        addedComponent.SetData(data.type + 1, data.float2);



    }

    public void ApplyPositonerData(RecordedObjectPositionerDataSave data)
    {
        transform.SetPositionAndRotation(data.startPos, Quaternion.Euler(0, 0, data.startRot));
        Debug.LogError("Saved Time Per Step is: " + data.savedTimePerStep);
        data.ReturnIntervalAndDuration(0, out positionDelayList, out positionTimeList, out positionEases);
        positionList = data.positions;
        data.ReturnIntervalAndDuration(1, out rotationDelayList, out rotationTimeList, out rotationEases);
        rotationList = data.values;
        float[] interval2;
        float[] duration2;
        ushort[] nullEases;
        data.ReturnIntervalAndDuration(2, out interval2, out duration2, out nullEases);
        gameObject.SetActive(true);
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
        addedComponent.SetData(data.type + 1, data.perecnt, interval2, duration2);

    }

    public void SetPercentForLaserShooting(float percent)
    {
        addedComponent.SetPercent(percent);
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

        return 0;
    }
}
