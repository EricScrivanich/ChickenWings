using UnityEngine;

public class ConstantSpriteSwitch : MonoBehaviour
{

    [SerializeField] private AnimationDataSO animData;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private float time;
    private ushort currentFrameIndex;
    [SerializeField] private Vector2Int startFrame;

    private void OnEnable()
    {
        currentFrameIndex = (ushort)Random.Range(startFrame.x, startFrame.y);
        spriteRenderer.sprite = animData.sprites[currentFrameIndex];
        time = 0;
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= animData.constantSwitchTime)
        {

            currentFrameIndex++;
            if (currentFrameIndex >= animData.sprites.Length)
            {
                currentFrameIndex = 0;
            }
            spriteRenderer.sprite = animData.sprites[currentFrameIndex];

            time = 0f;





        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
