using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class GasCloud : MonoBehaviour
{

    [SerializeField] private AnimationDataSO animData;

    private readonly int spriteCount = 3;

    private int currentSpriteIndex = 0;

    private readonly float spriteSwitchTime = .1f;
    private float time = 0;
    private readonly float tweenTime = .7f;



    private SpriteRenderer sr;
    // Start is called before the first frame update
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        float startX = transform.localPosition.x;
        transform.DOScale(animData.endScale, tweenTime).From(animData.startScale).SetEase(Ease.OutSine);
        transform.DOLocalMoveX(startX + 1.7f, .5f).SetEase(Ease.OutSine);
        transform.DOMoveY(7f, 6.5f);
    }

    // Update is called once per frame
    void Update()
    {

        time += Time.deltaTime;

        if (time > spriteSwitchTime)
        {
            currentSpriteIndex++;

            if (currentSpriteIndex > spriteCount)
            {
                currentSpriteIndex = 0;
            }
            sr.sprite = animData.sprites[currentSpriteIndex];
            time = 0;

        }



    }
}
