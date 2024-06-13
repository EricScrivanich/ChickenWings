using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAirBehavior : MonoBehaviour
{
    [SerializeField] private PlayerID ID;
    public int index { get; private set; }
    private SpriteRenderer spriteRen;
    private bool fadeBool;
    private float currentAlpha;
    private float fadeTimer;
    private float fadeDuration = .5f;
    private float opacityTime;


    [SerializeField] private float scaleTime;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 endScale;
    [SerializeField] private float waveSpeed;
    private float waveSpeedVar;



    // private Vector2 startPos = new Vector2(-.2f,-1);

    [SerializeField] private Sprite[] sprites;
    private Transform airWavePos;
    private SpriteRenderer airWaveSprite;
    private int currentSprite;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        opacityTime = scaleTime - .1f;

    }
    public void SetIndex(int i)
    {
        index = i;
    }
    private void Awake()
    {


        airWaveSprite = GetComponent<SpriteRenderer>();
        airWavePos = GetComponent<Transform>();

    }
    private void Update()
    {
        time += Time.deltaTime;
        airWavePos.Translate(Vector2.down * waveSpeedVar * Time.deltaTime, transform);



        if (time < scaleTime)
        {
            float xScale = Mathf.Lerp(startScale.x, endScale.x, time / scaleTime);
            float yScale = Mathf.Lerp(startScale.y, endScale.y, time / scaleTime);
            waveSpeedVar = Mathf.Lerp(waveSpeed, 1.5f, time / scaleTime);
            airWavePos.localScale = new Vector3(xScale, yScale, 1);
        }

        if (fadeBool && fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float a = Mathf.Lerp(currentAlpha, 0, fadeTimer / fadeDuration);
            airWaveSprite.color = new Color(1, 1, 1, a);



        }
        else if (fadeBool)
        {
            gameObject.SetActive(false);
        }

        else
        {
            currentAlpha = Mathf.Lerp(1f, .35f, time / scaleTime);

            airWaveSprite.color = new Color(1, 1, 1, opacityTime);
            if (time > opacityTime)
            {
                fadeBool = true;
            }

        }
        // if (time < .45f)
        // {
        //     float a = Mathf.Lerp(1f, .2f, time / .45f);

        //     airWaveSprite.color = new Color(1, 1, 1, a);
        // }
        // else
        // {
        //     gameObject.SetActive(false);
        // }



    }

    private void Fade(int i)
    {
        if (i == index && !fadeBool)
        {
            fadeBool = true;

        }

    }





    private void OnEnable()
    {
        airWaveSprite.color = Color.white;
        currentAlpha = 1;

        ID.events.OnStopJumpAir += Fade;
        fadeBool = false;
        fadeTimer = 0;
        time = 0;
        waveSpeedVar = waveSpeed;
        currentSprite = 0;



        airWavePos.localScale = startScale;

    }

    private void OnDisable()
    {
        ID.events.OnStopJumpAir -= Fade;
    }

    // Update is called once per frame

}
