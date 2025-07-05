using UnityEngine;

public class LevelPickerPathData : MonoBehaviour
{

    [field: SerializeField] public AnimationCurve ScalePlayerCurve { get; private set; }
    [field: SerializeField] public AnimationCurve PathDistanceCurve { get; private set; }

    [field: SerializeField] public float totalTime { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 ReturnScaleAndDistance(float p)
    {
        return new Vector2(ScalePlayerCurve.Evaluate(p), PathDistanceCurve.Evaluate(p));
    }
}
