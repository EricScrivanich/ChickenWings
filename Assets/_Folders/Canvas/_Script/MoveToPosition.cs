using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveToPosition : MonoBehaviour
{
    private RectTransform rectTransform;
    public Event yuh;

    [SerializeField] private bool isSpecial;
    [SerializeField] private bool waitForEvent;
    [SerializeField] private float duration;
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 amountToMoveFromEvent;
    [SerializeField] private Vector2 endPosition;
    private Sequence sequence;
    private bool hasMoved = false;
    // Start is called before the first frame update\

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void Start()
    {

    }

    private void OnEnable()
    {
        if (!isSpecial && !hasMoved && !waitForEvent)
        {
            rectTransform.anchoredPosition = startPosition; // Set the start position directly
            rectTransform.DOAnchorPos(endPosition, duration).SetEase(Ease.OutSine).SetUpdate(true);
            hasMoved = true;
        }
        else if (!waitForEvent)
        {
            if (!hasMoved)
                MoveLivesThenActivate();
        }

    }


    private void OnDisable()
    {
        if (sequence != null && sequence.IsPlaying())
        {
            sequence.Kill();
            rectTransform.position = endPosition;
        }
    }

    

    public void MoveFromEvent()
    {
        Vector2 orginalPos = rectTransform.anchoredPosition;

        rectTransform.DOAnchorPos(new Vector2(orginalPos.x + amountToMoveFromEvent.x, orginalPos.y + amountToMoveFromEvent.y), duration).SetEase(Ease.OutSine).SetUpdate(true);



    }


    public void MoveLivesThenActivate()
    {

        rectTransform.anchoredPosition = startPosition; // Set the start position directly
        sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOAnchorPos(endPosition, duration).SetEase(Ease.OutSine).SetUpdate(true));
        sequence.OnComplete(SetInfiniteLives).SetUpdate(true);

        sequence.Play().SetUpdate(true);


    }

    private void SetInfiniteLives()
    {
        rectTransform.gameObject.GetComponent<LifeDisplay>().SetInfiniteLives(false);

    }

    // Update is called once per frame

}
