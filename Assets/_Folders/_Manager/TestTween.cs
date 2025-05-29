using UnityEngine;
using DG.Tweening;

public class TestTween : MonoBehaviour
{
    private Sequence testSequence;

    [SerializeField] private float duration1;
    [SerializeField] private float duration2;
    [SerializeField] private Ease easeType;
    [SerializeField] private Vector2 targetPos1;
    [SerializeField] private Vector2 targetPos2;
    [SerializeField] private float testTime;
    [SerializeField] private float testDelay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testSequence = DOTween.Sequence();
        testSequence.Append(transform.DOMove(targetPos1, duration1).SetEase(easeType));
        testSequence.AppendInterval(testDelay);
        testSequence.Append(transform.DOMove(targetPos2, duration2).SetEase(easeType));
        testSequence.SetUpdate(UpdateType.Manual);


       

    }

    void OnValidate()
    {
        Vector2 val = DOVirtual.EasedValue(targetPos1, targetPos2, testTime / duration1, easeType);
        Debug.Log("val is: " + val);



    }

    // Update is called once per frame
    void Update()
    {

    }
}
