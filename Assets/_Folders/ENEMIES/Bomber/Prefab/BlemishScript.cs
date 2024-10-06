using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlemishScript : MonoBehaviour
{
    [SerializeField] private PlaneManagerID ID;
    [SerializeField] private AnimationDataSO animData;
    [SerializeField] private SpriteRenderer smoke;
    private SpriteRenderer sprite;
    private float time;
    private float duration = 3;

    private int spriteLength;

    private int currentSmokeIndex = 0;
    private bool finishedSpriteAnim;


    // Start is called before the first frame update

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * BoundariesManager.GroundSpeed * Time.deltaTime);

        if (!finishedSpriteAnim)
            time += Time.deltaTime;


        if (time > animData.constantSwitchTime && !finishedSpriteAnim)
        {
            time = 0;
            currentSmokeIndex++;

            if (currentSmokeIndex >= animData.sprites.Length)
            {
                smoke.DOFade(0, .2f).SetEase(Ease.OutSine);
                finishedSpriteAnim = true;
                return;
            }
            smoke.sprite = animData.sprites[currentSmokeIndex];

        }





    }
    private void OnEnable()
    {
        finishedSpriteAnim = false;
        currentSmokeIndex = 0;
        smoke.sprite = animData.sprites[0];
        smoke.DOFade(1, 0);

        smoke.transform.DOScale(animData.endScale, .5f).SetEase(Ease.OutSine);
    }



}
