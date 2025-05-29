using UnityEngine;
using System.Collections;

public class LaserParent : MonoBehaviour
{


    [SerializeField] private int laserCount;
    [SerializeField] private Laser laserPrefab;
    [SerializeField] private float laserDistance;

    [SerializeField] private float[] laserShootingDurations;

    private ushort colorSwitchTypeForRecording = 0;
    private int currentLaserShootingIndex = 0;

    private bool stopUpdate = false;



    private Laser[] lasers;

    private AudioSource audioSource;
    [SerializeField] private AudioClip laserChargeUpSound;
    [SerializeField] private float laserChargeUpVolume = .7f;
    private float laserShootingVolume = .55f;

    [SerializeField] private Material timerBaseMaterial;
    private Material timerMaterial;
    [SerializeField] private SpriteRenderer timerSprite;
    [SerializeField] private Material baseLaserMaterial;
    private Material laserMaterial;
    [SerializeField] private float laserLength;
    private float laserSpeed;
    [SerializeField] private float initialLaserSpeed = 4f;
    [SerializeField] private float finalLaserSpeed = 6f;
    [SerializeField] private Transform zeroRotation;
    private float currentLaserOffset = 0;

    [SerializeField] private float shootDuration;
    public static float cooldownDuration = 2.7f;
    [SerializeField] private float initialLaserShootDelay;
    [SerializeField] private float laserStartFade;
    [SerializeField] private float laserInbetweenFade;



    public float currentLaserFadeAmount = 1;
    public float currentTimeLeft = 0;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (Time.timeScale == 0)
        {
            audioSource.Stop();
            stopUpdate = true;
            return;
        }
        audioSource.volume = laserShootingVolume;





        timerSprite.material = timerMaterial;
        SetLaserAmount(laserCount, laserDistance);

        laserSpeed = finalLaserSpeed;

        StartCoroutine(LaserShoot());


    }
    void Awake()
    {
        laserMaterial = new Material(baseLaserMaterial);
        timerMaterial = new Material(timerBaseMaterial);

    }

    public void SetLaserPercentForRecording(float percent)
    {
        if (percent == 0 && colorSwitchTypeForRecording != 1)
        {
            colorSwitchTypeForRecording = 1;
            laserMaterial.SetFloat("_FadeAmount", 0);
            laserMaterial.SetColor("_Color", Color.white * .5f);
        }

        else if (percent > 0)
        {
            if (colorSwitchTypeForRecording != 2)
            {
                colorSwitchTypeForRecording = 2;
                laserMaterial.SetColor("_Color", Color.white * .9f);
            }
            timerMaterial.SetFloat("_RadialClip", Mathf.Lerp(360, 0, percent));
            laserMaterial.SetFloat("_FadeAmount", Mathf.Lerp(laserStartFade - .1f, laserInbetweenFade, percent));
        }
        else if (percent < 0)
        {
            if (colorSwitchTypeForRecording != 3)
            {
                laserMaterial.SetFloat("_FadeAmount", 0);
                colorSwitchTypeForRecording = 3;
                laserMaterial.SetColor("_Color", Color.white);
            }
            timerMaterial.SetFloat("_RadialClip", Mathf.Lerp(360, 0, -percent));

        }









    }

    public void SetLaserAmount(int amount, float space = 1, bool isLevelCreator = false)
    {
        if (space == 0) space = 1;


        float angleDiff = (360f / amount) * space;
        float currentAngle = angleDiff;
        if (lasers != null)
        {
            for (int i = 1; i < lasers.Length; i++)
            {
                Destroy(lasers[i].gameObject);
            }
        }

        lasers = new Laser[amount];

        lasers[0] = laserPrefab;
        lasers[0].SetLaserMaterial(laserMaterial);
        lasers[0].SetLaserParent(this);
        for (int i = 1; i < amount; i++)
        {
            lasers[i] = Instantiate(laserPrefab);
            lasers[i].transform.SetParent(transform);
            lasers[i].transform.localPosition = new Vector2(0, 0);
            lasers[i].transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
            lasers[i].SetLaserMaterial(laserMaterial);
            lasers[i].SetLaserParent(this);



            currentAngle += angleDiff;
            Debug.Log("current angle is: " + currentAngle);

        }


    }



    private void Update()
    {
        if (stopUpdate) return;
        laserMaterial.SetTextureOffset("_MainTex", Vector2.up * currentLaserOffset);
        currentLaserOffset += laserSpeed * Time.deltaTime;

        if (currentLaserOffset > laserLength)
        {
            currentLaserOffset = 0;
        }
        zeroRotation.eulerAngles = Vector3.zero;


    }

    public void UpdateLaserPostions()
    {
        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].SetLaserGroundHit();
        }
    }

    private IEnumerator LaserCooldown()
    {


        float time = 0;

        bool useParticles = false;
        laserSpeed = initialLaserSpeed;
        currentLaserFadeAmount = 0;
        currentTimeLeft = cooldownDuration;

        for (int i = 0; i < laserCount; i++)
        {

            lasers[i].SetUseParticles(true);
        }
        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].SetLaserGroundHit();
        }

        AudioManager.instance.PlayLaserChargeUpSound();
        while (time < cooldownDuration)
        {


            time += Time.deltaTime;
            timerMaterial.SetFloat("_RadialClip", Mathf.Lerp(360, 0, time / cooldownDuration));
            laserMaterial.SetFloat("_FadeAmount", Mathf.Lerp(laserStartFade, laserInbetweenFade, time / cooldownDuration));

            for (int i = 0; i < laserCount; i++)
            {
                lasers[i].SetLaserScale(Mathf.Lerp(.3f, .8f, time / cooldownDuration));
            }
            // laserMaterial.SetFloat("_ZoomUvAmount", Mathf.Lerp(1.5f, 1.2f, time / cooldownDuration));

            laserSpeed = Mathf.Lerp(initialLaserSpeed, 5, time / cooldownDuration);
            currentLaserFadeAmount = Mathf.Lerp(0, 1, time / cooldownDuration);
            currentTimeLeft = cooldownDuration - time;
            // if (!useParticles && time > cooldownDuration * .3f)
            // {
            //     for (int i = 0; i < laserCount; i++)
            //     {
            //         useParticles = true;
            //         lasers[i].SetUseParticles(true);
            //     }
            // }
            yield return null;
        }
        currentLaserFadeAmount = 1;
        currentTimeLeft = 0;

        timerMaterial.SetFloat("_RadialClip", 0);
        laserSpeed = finalLaserSpeed;


        StartCoroutine(LaserShoot());



    }

    private IEnumerator LaserShoot()
    {
        float time = 0;
        laserSpeed = finalLaserSpeed;
        audioSource.volume = laserShootingVolume;
        audioSource.Play();
        while (time < initialLaserShootDelay)
        {
            time += Time.deltaTime;
            laserMaterial.SetFloat("_FadeAmount", Mathf.Lerp(laserInbetweenFade, 0, time / initialLaserShootDelay));

            for (int i = 0; i < laserCount; i++)
            {
                lasers[i].SetLaserScale(Mathf.Lerp(.8f, 1, time / initialLaserShootDelay));
            }

            // laserMaterial.SetFloat("_ZoomUvAmount", Mathf.Lerp(1.2f, 1, time / initialLaserShootDelay));
            yield return null;
        }

        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].HandleShooting(true);
        }

        time = 0;
        float shootTime = 4;
        if (laserShootingDurations != null && currentLaserShootingIndex < laserShootingDurations.Length)
            shootTime = laserShootingDurations[currentLaserShootingIndex];

        while (time < shootTime)
        {
            time += Time.deltaTime;
            timerMaterial.SetFloat("_RadialClip", Mathf.Lerp(0, 358, time / shootTime));
            yield return null;
        }
        timerMaterial.SetFloat("_RadialClip", 360);
        currentLaserShootingIndex++;

        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].HandleShooting(false);
        }
        time = 0;

        while (time < .3f)
        {
            time += Time.deltaTime;
            laserMaterial.SetFloat("_FadeAmount", Mathf.Lerp(0, .8f, time / .3f));
            audioSource.volume = Mathf.Lerp(laserShootingVolume, 0, time / .3f);
            yield return null;
        }
        audioSource.Stop();
        laserMaterial.SetFloat("_FadeAmount", 1);
        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].StopParticles();
        }



        StartCoroutine(LaserCooldown());


    }

    // Update is called once per frame

}
