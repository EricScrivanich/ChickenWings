using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlemishScript : SpawnedQueuedEffect
{

    private SpriteRenderer sprite;
    private float timer;

    // private float time;
    // private float duration = 3;

    // private int spriteLength;

    // private int currentSmokeIndex = 0;
    // private bool finishedSpriteAnim;


    // Start is called before the first frame update

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

    }
    void CheckForDespawn()
    {
        if (transform.position.x < BoundariesManager.leftBoundary)
        {
            gameObject.SetActive(false);

        }

    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * BoundariesManager.GroundSpeed * Time.deltaTime);

        if (timer < .25f)
        {
            timer += Time.deltaTime;
            sprite.color = Color.black * timer * 4;


        }

        // if (!finishedSpriteAnim)
        //     time += Time.deltaTime;


        // if (time > animData.constantSwitchTime && !finishedSpriteAnim)
        // {
        //     time = 0;
        //     currentSmokeIndex++;

        //     if (currentSmokeIndex >= animData.sprites.Length)
        //     {
        //         smoke.DOFade(0, .2f).SetEase(Ease.OutSine);
        //         finishedSpriteAnim = true;
        //         return;
        //     }
        //     smoke.sprite = animData.sprites[currentSmokeIndex];

        // }





    }
    void OnDisable()
    {
        Ticker.OnTickAction015 -= CheckForDespawn;
        sprite.color = Color.clear;
        ReturnToPool();
    }
    private void OnEnable()
    {
        Ticker.OnTickAction015 += CheckForDespawn;

        timer = 0;

        // finishedSpriteAnim = false;
        // currentSmokeIndex = 0;
        // smoke.sprite = animData.sprites[0];
        // smoke.DOFade(1, 0);

        // smoke.transform.DOScale(animData.endScale, .5f).SetEase(Ease.OutSine);
    }



}
