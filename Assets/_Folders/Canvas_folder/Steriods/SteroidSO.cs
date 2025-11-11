using UnityEngine;

[CreateAssetMenu(fileName = "SteroidData", menuName = "ScriptableObjects/StedroidSO")]
public class SteroidSO : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public string Ingredients { get; private set; }

    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Color ImageColor { get; private set; }
    [field: SerializeField] public Color LiquidColor { get; private set; }
    [field: SerializeField] public ushort Spaces { get; private set; }
    [field: SerializeField] public ushort ID { get; private set; }



    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
