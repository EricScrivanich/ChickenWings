using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggCollectableMovement : SpawnedObject, ICollectible, IRecordableObject
{

    public PlayerID ID;
    private bool bursted;
    [SerializeField] private int type;
    [SerializeField] private ParticleSystem ps;
    private bool isCollected;

    // private bool hasCrossedScreen = false;

    [SerializeField] private SpriteRenderer blur;

    private Color whiteColorStart = new Color(1, 1, 1, .8f);
    private Color whiteColorEnd = new Color(1, 1, 1, 0);
    private Color blueColorStart = new Color(.809f, .05f, .05f, .8f);
    private Color blueColorEnd = new Color(.809f, .05f, .05f, 0);
    private Color startColor;
    private Color endColor;






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

    private float _sineMagnitude;
    private float _sineFrequency;
    private float magDiff;
    [SerializeField] private float phaseOffset;
    private float startX;

    private float speedDiff;

    [SerializeField] private float magPercent;


    [ExposedScriptableObject]
    [SerializeField] private PigsScriptableObject pigID;
    Vector2 _position;



    private readonly Vector2 minMaxAmp = new Vector2(.2f, 4f);
    private readonly Vector2 maxFreqBasedOnAmp = new Vector2(.1f, 3.4f);

    private readonly float minFreq = .05f;




    void Awake()
    {
        mainSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (type == 0)
        {

            startColor = whiteColorStart;
            endColor = whiteColorEnd;

        }
        else if (type == 1)
        {


            startColor = blueColorStart;
            endColor = blueColorEnd;

        }
    }

    private void CheckPosition()
    {
        if (transform.position.x < BoundariesManager.leftPlayerBoundary && speed > 0 || transform.position.x > BoundariesManager.rightPlayerBoundary && speed < 0)
        {
            gameObject.SetActive(false);
        }
    }
    public void EnableAmmo(Sprite mainImage, Sprite ThreeImage, bool mana, float speedVar)
    {
        // hasCrossedScreen = false;
        _position = transform.position;

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
            _sineFrequency = Random.Range(.4f, .75f);
            _sineMagnitude = Random.Range(.5f, .9f);
            speed = Random.Range(5.5f, 7.6f);
        }
        else
        {
            speed = speedVar;
            _sineFrequency = .4f;
            _sineMagnitude = .5f;
        }

        yPos = transform.position.y;
        mainSprite.enabled = true;
        isCollected = false;

        blur.color = endColor;
        blur.enabled = false;
        isPostiveXSpeed = speed > 0;
        transform.localScale = Vector3.one;


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


    // void Update()
    // {


    //     transform.Translate(Vector3.left * speed * Time.deltaTime);
    //     float x = transform.position.x;
    //     float y = Mathf.Sin(x * eggSinFrequency) * eggSinAmplitude + yPos; // Calculate y position using sine function
    //     transform.position = new Vector3(x, y, 0); // Update position

    //     if (isPostiveXSpeed)
    //     {
    //         if (transform.position.x < -13) gameObject.SetActive(false);
    //     }
    //     else
    //     {
    //         if (transform.position.x > 13) gameObject.SetActive(false);
    //     }





    // }
    private float startY;
    private void FixedUpdate()
    {
        _position += Vector2.left * Time.fixedDeltaTime * speed;

        float period = Mathf.Sin((_position.x - startX) * _sineFrequency);


        // Dynamically adjust sine magnitude using Lerp
        // speed = Mathf.Lerp(minSpeed, maxSpeed, Mathf.Abs(period));



        // Apply movement with dynamic sine magnitude
        rb.MovePosition(_position + Vector2.up * period * _sineMagnitude);

        // Animation trigger logic

    }
    private void OnEnable()
    {
        Ticker.OnTickAction015 += CheckPosition;
    }
    private void OnDisable()
    {



        ID.globalEvents.OnAmmoEvent?.Invoke(type);
        blur.enabled = false;
        mainSprite.enabled = true;
        bursted = false;
        isCollected = false;
        Ticker.OnTickAction015 -= CheckPosition;
        // hasCrossedScreen = false;

    }

    // public void Tick()
    // {
    //     float x = transform.position.x;
    //     if (!hasCrossedScreen && x > BoundariesManager.leftBoundary && x < BoundariesManager.rightBoundary)
    //     {
    //         hasCrossedScreen = true;
    //     }
    //     else if (hasCrossedScreen && (x < BoundariesManager.leftBoundary || x > BoundariesManager.rightBoundary))
    //     {
    //         gameObject.SetActive(false);


    //     }


    // }


    private IEnumerator Burst()
    {
        float burstTimer = 0;
        float duration = .15f;
        blur.enabled = true;
        float startSpeed = speed;
        float endSpeed = startSpeed * .2f;
        Vector3 endScale = transform.localScale * 1.3f;

        while (burstTimer < duration)
        {
            burstTimer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, endScale, burstTimer / duration);
            blur.color = Color.Lerp(endColor, startColor, burstTimer / duration);
            speed = Mathf.Lerp(startSpeed, endSpeed, burstTimer / duration);

            yield return null;

        }
        // if (spawner != null)
        //     spawner.GetParticles(!isMana, transform.position);
        if (ps != null)
        {

            ps.Play();
        }
        burstTimer = 0;
        duration = .2f;
        mainSprite.enabled = false;

        threeImage.enabled = false;
        while (burstTimer < duration)
        {
            burstTimer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(endScale, endScale * 1.2f, burstTimer / duration);
            blur.color = Color.Lerp(startColor, endColor, burstTimer / duration);

            yield return null;

        }
        yield return new WaitForSeconds(.3f);

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

        switch (type)
        {
            case 0:
                ID.Ammo += ammoAmount;
                break;
            case 1:
                ID.ShotgunAmmo += ammoAmount;
                break;
                // case 2:
                //     ID.globalEvents.OnGetMana?.Invoke();
                //     break;
        }

        // if (isMana)
        // {
        //     // ID.globalEvents.OnGetMana?.Invoke();
        //     ID.ShotgunAmmo += ammoAmount;

        // }
        // else
        // {
        //     ID.Ammo += ammoAmount;

        // }
        StartCoroutine(Burst());
        bursted = true;
        // ID.globalEvents.OnAmmoEvent?.Invoke(1);



    }




    public override void ApplyFloatFiveData(DataStructFloatFive data)
    {
        _position = data.startPos;

        transform.position = data.startPos;
        speed = data.float1;
        transform.localScale = Vector3.one * data.float2;
        _sineMagnitude = Mathf.Lerp(minMaxAmp.x, minMaxAmp.y, data.float3);
        float maxFreq = Mathf.Lerp(maxFreqBasedOnAmp.y, maxFreqBasedOnAmp.x, data.float3);
        _sineFrequency = Mathf.Lerp(minFreq, maxFreq, data.float4);
        phaseOffset = data.float5;
        startX = ReturnPhaseOffset(data.startPos.x);
        if (data.type == 0)
        {
            threeImage.enabled = false;
            ammoAmount = 1;
            isThreeAmmo = false;
        }
        else
        {
            threeImage.enabled = true;
            ammoAmount = 3;
            isThreeAmmo = true;

        }

        gameObject.SetActive(true);
    }


    public void ApplyCustomizedData(RecordedDataStructDynamic data)
    {
        speed = data.float1;
        transform.localScale = Vector3.one * data.float2;
        _sineMagnitude = Mathf.Lerp(minMaxAmp.x, minMaxAmp.y, data.float3);
        float maxFreq = Mathf.Lerp(maxFreqBasedOnAmp.y, maxFreqBasedOnAmp.x, data.float3);
        _sineFrequency = Mathf.Lerp(minFreq, maxFreq, data.float4);
        phaseOffset = data.float5;
        if (data.type == 0)
        {
            threeImage.enabled = false;
            ammoAmount = 1;
            isThreeAmmo = false;
        }
        else
        {
            threeImage.enabled = true;
            ammoAmount = 3;
            isThreeAmmo = true;

        }

    }

    public bool ShowLine()
    {
        return true;
    }

    public float TimeAtCreateObject(int index)
    {
        throw new System.NotImplementedException();
    }

    public Vector2 PositionAtRelativeTime(float time, Vector2 currPos, float phaseOffset)
    {
        float xPos = currPos.x + (-speed * time);

        // Apply phase offset correction


        // Compute sine wave period at this x position
        float period = Mathf.Sin((xPos - phaseOffset) * _sineFrequency);

        float yPos = currPos.y + (period * _sineMagnitude);




        // Dynamically adjust sine magnitude using Lerp


        // Compute final vertical position


        return new Vector2(xPos, yPos);
    }

    public float ReturnPhaseOffset(float x)
    {
        return x + (((Mathf.PI) / _sineFrequency) * phaseOffset);
    }
}

