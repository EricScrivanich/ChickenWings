using UnityEngine;

public class SteroidButtonLevelPicker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer image;
    [SerializeField] private SpriteRenderer liquid;
    [SerializeField] private SteroidSO steroidData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (steroidData != null)
        {
            image.color = steroidData.ImageColor;
            image.sprite = steroidData.Icon;
            liquid.color = steroidData.LiquidColor;
        }

    }
}
