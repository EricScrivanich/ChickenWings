using UnityEngine;


[CreateAssetMenu(fileName = "LevelCreatorColors", menuName = "LevelCreatorColors", order = 0)]
public class LevelCreatorColors : ScriptableObject
{
    public Vector3 arrowNormalScale;
    public Vector3 arrowSelectedScale;
    public Color SelectedPigOutlineColor;

    public Color SelectedLineColor;
    public Color NormalLineColor;
    public Color PassedLineColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Color ArrowXDefaultColor;
    public Color ArrowXSelectedColor;
    public Color ArrowYDefaultColor;
    public Color ArrowYSelectedColor;
    public Color ArrowNullColor;
    public float TimeStampTimeSpacing;
    public Color[] timeStampColors;
}
