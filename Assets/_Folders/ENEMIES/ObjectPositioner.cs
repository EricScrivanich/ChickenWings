using UnityEngine;
using DG.Tweening;

public class ObjectPositioner : MonoBehaviour
{
    private Rigidbody2D rb;
    private float currentRotation;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private LaserParent laser;
    [SerializeField] private float startRotation;


    [SerializeField] private Transform directionArrowParent;

    [SerializeField] private SpriteRenderer[] directionArrows;

    [SerializeField] private Vector2[] positionList;

    [SerializeField] private float[] positionTimeList;
    [SerializeField] private float[] positionDelayList;

    private int currentPostionIndex;

    private Sequence MoveSeq;
    private Sequence ArrowSeq;
    private bool moving;
    [SerializeField] private float fadeArrowDur;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

            MoveSeq.Append(transform.DOMove(positionList[index], positionTimeList[index])
                .SetEase(Ease.InOutSine)
                .OnComplete(() => StopArrows()));

            MoveSeq.AppendInterval(positionDelayList[index]);

            MoveSeq.AppendCallback(() => DoHandleArrows(index + 1));
        }

        MoveSeq.Play().OnComplete(() => moving = false);
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

        if (rotationSpeed == 0) return;
        rb.MoveRotation(currentRotation);
        currentRotation += rotationSpeed * Time.fixedDeltaTime;



    }
}
