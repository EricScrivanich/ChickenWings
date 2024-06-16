using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveToPosition : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private float duration;
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 endPosition;
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
        rectTransform.anchoredPosition = startPosition; // Set the start position directly
        rectTransform.DOAnchorPos(endPosition, duration).SetEase(Ease.OutSine).SetUpdate(true);
    }

    // Update is called once per frame

}
