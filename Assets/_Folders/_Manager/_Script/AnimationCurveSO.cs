using UnityEngine;

[CreateAssetMenu(fileName = "AnimationCurveSO", menuName = "ScriptableObjects/AnimationCurveSO")]
public class AnimationCurveSO : ScriptableObject
{
    [SerializeField] private AnimationCurve curve;
    [field: SerializeField] public float time { get; private set; }

    public float ReturnValue(float percent)
    {
        return curve.Evaluate(percent);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
