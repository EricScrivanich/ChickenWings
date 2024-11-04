using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggCollectableMovement : MonoBehaviour, ICollectible
{

    public PlayerID ID;
    private bool bursted;
    private bool isCollected;

    [SerializeField] private SpriteRenderer blur;

    private Color whiteColorStart = new Color(1, 1, 1, 1);
    private Color whiteColorEnd = new Color(1, 1, 1, 0);
    private Color blueColorStart = new Color(.809f, .05f, .05f, 1);
    private Color blueColorEnd = new Color(.809f, .05f, .05f, 0);
    private Color startColor;
    private Color endColor;



    private Vector3 startScale = new Vector3(1f, 1f, 1f);
    private Vector3 endScale;

    public BarnAndEggSpawner spawner;
    private float speed;
    private int ammoAmount;

    [SerializeField] private float eggTime;
    private float minY = -2.5f;
    private float maxY = 4.5f;
    private float yPos;

    private bool isPostiveXSpeed;


    private float minAmmoSpeed = 6f;
    private float maxAmmoSpeed = 8.5f;

    private float minEggSinFrequency = .5f;
    private float maxEggSinFrequency = .8f;
    private float eggSinFrequency;


    private float minEggSinAmplitude = .5f;
    private float maxEggSinAmplitude = .9f;
    private float eggSinAmplitude;


    public bool isThreeAmmo;
    public bool isMana;
    private SpriteRenderer mainSprite;

    [SerializeField] private Sprite[] blurImage;
    [SerializeField] private SpriteRenderer threeImage;
    [SerializeField] private SpriteRenderer shotgunImage;
    [SerializeField] private SpriteRenderer threeImageShotgun;

    void Awake()
    {
        mainSprite = GetComponent<SpriteRenderer>();
    }
    public void EnableAmmo(Sprite mainImage, Sprite ThreeImage, bool mana, float speedVar)
    {
        if (this.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }

        mainSprite.sprite = mainImage;
        if (ThreeImage != null)
        {
            threeImage.sprite = ThreeImage;
            threeImage.enabled = true;
            ammoAmount = 3;
            isThreeAmmo = true;


        }

        else
        {
            ammoAmount = 1;

            threeImage.enabled = false;
            isThreeAmmo = false;
        }



        isMana = mana;

        if (isMana)
        {

            blur.sprite = blurImage[1];
            startColor = blueColorStart;
            endColor = blueColorEnd;

        }

        else if (!isThreeAmmo)
        {

            blur.sprite = blurImage[0];


            startColor = whiteColorStart;
            endColor = whiteColorEnd;


        }
        else if (isThreeAmmo)
        {

            blur.sprite = blurImage[0];


            startColor = whiteColorStart;
            endColor = whiteColorEnd;


        }

        if (speedVar == 0)
        {
            eggSinFrequency = Random.Range(.4f, .75f);
            eggSinAmplitude = Random.Range(.5f, .9f);
            speed = Random.Range(5.5f, 7.6f);
        }
        else
        {
            speed = speedVar;
            eggSinFrequency = .4f;
            eggSinAmplitude = .5f;
        }

        yPos = transform.position.y;
        mainSprite.enabled = true;
        isCollected = false;

        blur.color = endColor;
        blur.enabled = false;
        isPostiveXSpeed = speed > 0;
        transform.localScale = startScale;
        endScale = startScale * 1.4f;

        this.gameObject.SetActive(true);





    }
    // private void OnEnable()
    // {

    //     if (!isThreeAmmo)
    //     {
    //         eggSinFrequency = Random.Range(.1f, .5f);
    //         eggSinAmplitude = Random.Range(.3f, .7f);
    //         ammoAmount = 1;
    //     }
    //     else if (isThreeAmmo)
    //     {
    //         eggSinFrequency = Random.Range(.4f, .75f);
    //         eggSinAmplitude = Random.Range(.5f, .9f);
    //         ammoAmount = 3;
    //     }

    //     if (speed == 0)
    //     {
    //         speed = Random.Range(5.5f, 8.5f);
    //     }
    //     yPos = transform.position.y;
    //     isPostiveXSpeed = speed > 0;
    //     threeImage.enabled = isThreeAmmo;


    // }


    void Update()
    {

        transform.Translate(Vector3.left * speed * Time.deltaTime);
        float x = transform.position.x;
        float y = Mathf.Sin(x * eggSinFrequency) * eggSinAmplitude + yPos; // Calculate y position using sine function
        transform.position = new Vector3(x, y, 0); // Update position

        if (isPostiveXSpeed)
        {
            if (transform.position.x < -13) gameObject.SetActive(false);
        }
        else
        {
            if (transform.position.x > 13) gameObject.SetActive(false);
        }





    }


    private IEnumerator Burst()
    {
        float burstTimer = 0;
        float duration = .15f;
        blur.enabled = true;
        float startSpeed = speed;
        float endSpeed = startSpeed * .2f;

        while (burstTimer < duration)
        {
            burstTimer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, burstTimer / duration);
            blur.color = Color.Lerp(endColor, startColor, burstTimer / duration);
            speed = Mathf.Lerp(startSpeed, endSpeed, burstTimer / duration);

            yield return null;

        }
        spawner.GetParticles(!isMana, transform.position);
        burstTimer = 0;
        duration = .2f;
        mainSprite.enabled = false;

        threeImage.enabled = false;
        while (burstTimer < duration)
        {
            burstTimer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(endScale, endScale * 1.1f, burstTimer / duration);
            blur.color = Color.Lerp(startColor, endColor, burstTimer / duration);

            yield return null;

        }

        gameObject.SetActive(false);


    }



    void Movement()
    {

        transform.Translate(Vector3.left * speed * Time.deltaTime);
        float x = transform.position.x;
        float y = Mathf.Sin(x * eggSinFrequency) * eggSinAmplitude + yPos; // Calculate y position using sine function
        transform.position = new Vector3(x, y, 0); // Update position

        if (isPostiveXSpeed)
        {
            if (transform.position.x < -13) gameObject.SetActive(false);
        }
        else
        {
            if (transform.position.x > 13) gameObject.SetActive(false);
        }


    }




    public void Collected()
    {
        if (isCollected)
            return;

        isCollected = true;
        if (isMana)
        {
            // ID.globalEvents.OnGetMana?.Invoke();
            ID.ShotgunAmmo += ammoAmount;

        }
        else
        {
            ID.Ammo += ammoAmount;

        }
        StartCoroutine(Burst());
        bursted = true;



    }
}
