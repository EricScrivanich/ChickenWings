using UnityEngine;
using System.Collections;

public class LaserParent : MonoBehaviour, IPositionerObject
{


    private int laserCount;
    [SerializeField] private Laser laserPrefab;
    private float laserDistance;

    private float[] laserShootingDurations;
    private float[] laserShootingIntervals;

    private ushort colorSwitchTypeForRecording = 0;
    private int currentLaserShootingIndex = 0;

    private bool stopUpdate = false;


    private Color laserCooldownColor = new Color(.85f, .85f, .85f, .9f);
    private Laser[] lasers;

    [SerializeField] private AudioSource laserAudioSource;
    [SerializeField] private AudioClip laserBlastAudioClip;
    [SerializeField] private float laserBlastAudioClipVolume;
    [SerializeField] private AudioSource laserChargeAudioSource;
    private float laserChargeUpDuration;

    private float laserChargeUpVolume = .55f;
    private float laserShootingVolume = 1f;

    private float baseLaserShootingVolume = .9f;
    private float baseLaserChargingVolume = .45f;


    [SerializeField] private Material timerBaseMaterial;
    private Material timerMaterial;
    [SerializeField] private SpriteRenderer timerSprite;
    [SerializeField] private Material baseLaserMaterial;
    private Material laserMaterial;
    [SerializeField] private float laserLength;
    private float laserSpeed;
    private float initialLaserSpeed = 4f;
    private float finalLaserSpeed = 6.5f;
    [SerializeField] private Transform zeroRotation;
    private float currentLaserOffset = 0;

    [SerializeField] private float shootDuration;
    public static readonly float cooldownDuration = 1.44f;
    [SerializeField] private float initialLaserShootDelay;
    [SerializeField] private float laserStartFade;
    [SerializeField] private float laserInbetweenFade;



    public float currentLaserFadeAmount = 1;
    public float currentTimeLeft = 0;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        if (Time.timeScale == 0)
        {
            laserAudioSource.Stop();
            stopUpdate = true;
            return;
        }

        laserChargeUpVolume = baseLaserChargingVolume * AudioManager.instance.SfxVolume;
        laserShootingVolume = baseLaserShootingVolume * AudioManager.instance.SfxVolume;
        laserChargeAudioSource.volume = laserChargeUpVolume;
        laserAudioSource.volume = laserShootingVolume;
        // audioSource.volume = laserShootingVolume;






        // SetLaserAmount(laserCount, laserDistance);



        // StartCoroutine(LaserShoot());


    }
    void Awake()
    {

        laserMaterial = new Material(baseLaserMaterial);
        timerMaterial = new Material(timerBaseMaterial);
        timerSprite.material = timerMaterial;
        laserSpeed = finalLaserSpeed;
        laserChargeUpDuration = laserChargeAudioSource.clip.length;


    }

    private void OnEnable()
    {
        currentLaserFadeAmount = 0;
        currentTimeLeft = cooldownDuration;
        AudioManager.instance.OnSetAudioPitch += ChangeAudioPitch;
        laserAudioSource.pitch = AudioManager.instance.SfxPitch;
        laserChargeAudioSource.pitch = AudioManager.instance.SfxPitch;


    }


    private void OnDisable()
    {

        AudioManager.instance.OnSetAudioPitch -= ChangeAudioPitch;
    }
    public void ChangeAudioPitch(float pitch)
    {

        laserAudioSource.pitch = pitch;
        laserChargeAudioSource.pitch = pitch;
    }



    public void SetLaserAmount(int amount, float space = 1)
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
        laserCount = amount;

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

    private IEnumerator HandleLaserTimer(float duration)
    {
        float time = 0;
        Debug.Log("HandleLaserTimer called with duration: " + duration);

        while (time < duration)
        {
            time += Time.deltaTime;
            timerMaterial.SetFloat("_RadialClip", Mathf.Lerp(360, 0, time / duration));



            // laserMaterial.SetFloat("_ZoomUvAmount", Mathf.Lerp(1.5f, 1.2f, time / duration));
            yield return null;
        }


    }

    private IEnumerator LaserCooldown()
    {

        float interval = 0;
        if (laserShootingDurations != null && currentLaserShootingIndex < laserShootingDurations.Length)
            interval = laserShootingIntervals[currentLaserShootingIndex];
        else
            yield break;
        laserMaterial.SetColor("_Color", laserCooldownColor);
        timerMaterial.SetColor("_Color", laserCooldownColor);
        float duration = 0;
        Debug.Log("LaserCooldown called with interval: " + interval);
        if (interval > laserChargeUpDuration - cooldownDuration)
        {
            duration = laserChargeUpDuration;
            StartCoroutine(HandleLaserTimer(interval + cooldownDuration));

            yield return new WaitForSeconds(interval + cooldownDuration - laserChargeUpDuration);
            laserChargeAudioSource.Play();



        }
        else
        {

            duration = interval + cooldownDuration;


            laserChargeAudioSource.time = laserChargeUpDuration - duration;
            laserChargeAudioSource.Play();
            StartCoroutine(HandleLaserTimer(duration));


        }



        // yield return new WaitForSeconds(interval);

        float time = 0;


        bool useParticles = false;
        laserSpeed = initialLaserSpeed;
        currentLaserFadeAmount = 0;
        currentTimeLeft = duration;

        for (int i = 0; i < laserCount; i++)
        {

            lasers[i].SetUseParticles(true);
        }
        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].SetLaserGroundHit();
        }

        // AudioManager.instance.PlayLaserChargeUpSound();
        while (time < duration)
        {


            time += Time.deltaTime;
            // timerMaterial.SetFloat("_RadialClip", Mathf.Lerp(360, 0, time / duration));
            laserMaterial.SetFloat("_FadeAmount", Mathf.Lerp(laserStartFade, laserInbetweenFade, time / duration));

            for (int i = 0; i < laserCount; i++)
            {
                lasers[i].SetLaserScale(Mathf.Lerp(.3f, .7f, time / duration));
            }
            // laserMaterial.SetFloat("_ZoomUvAmount", Mathf.Lerp(1.5f, 1.2f, time / duration));

            laserSpeed = Mathf.Lerp(initialLaserSpeed, 5, time / duration);
            currentLaserFadeAmount = Mathf.Lerp(0, 1, time / duration);
            currentTimeLeft = duration - time;
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
        laserChargeAudioSource.Stop();
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
        laserAudioSource.volume = laserShootingVolume;
        laserAudioSource.Play();
        laserAudioSource.PlayOneShot(laserBlastAudioClip, laserBlastAudioClipVolume);
        while (time < initialLaserShootDelay)
        {
            time += Time.deltaTime;
            laserMaterial.SetFloat("_FadeAmount", Mathf.Lerp(laserInbetweenFade, 0, time / initialLaserShootDelay));
            Color c = Color.Lerp(laserCooldownColor, Color.white, time / initialLaserShootDelay);
            laserMaterial.SetColor("_Color", c);
            timerMaterial.SetColor("_Color", c);

            for (int i = 0; i < laserCount; i++)
            {
                lasers[i].SetLaserScale(Mathf.Lerp(.7f, .9f, time / initialLaserShootDelay));
            }

            // laserMaterial.SetFloat("_ZoomUvAmount", Mathf.Lerp(1.2f, 1, time / initialLaserShootDelay));
            yield return null;
        }
        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].SetLaserScale(1);
        }
        laserMaterial.SetColor("_Color", Color.white);
        timerMaterial.SetColor("_Color", Color.white);

        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].HandleShooting(true);
        }

        time = 0;
        float shootTime = 0;
        if (laserShootingDurations != null && currentLaserShootingIndex < laserShootingDurations.Length)
            shootTime = laserShootingDurations[currentLaserShootingIndex] - cooldownDuration - initialLaserShootDelay;


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
            // laserAudioSource.volume = Mathf.Lerp(laserShootingVolume, 0, time / .3f);
            yield return null;
        }
        laserAudioSource.Stop();
        laserMaterial.SetFloat("_FadeAmount", 1);
        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].StopParticles();
        }



        StartCoroutine(LaserCooldown());


    }

    public void SetData(int type, float f, float[] intervals = null, float[] times = null)
    {
        Debug.Log("Setting data for laser parent: " + type + " " + f);
        SetLaserAmount(type, f);

        if (times != null)
        {
            laserShootingIntervals = intervals;
            laserShootingDurations = times;
            StartCoroutine(LaserCooldown());
        }

    }

    public void SetPercent(float percent)
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

    public void DoUpdateTransform()
    {
        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].SetLaserGroundHit();
        }
    }

    // Update is called once per frame

}
