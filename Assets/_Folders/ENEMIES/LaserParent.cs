using UnityEngine;
using System.Collections;

public class LaserParent : MonoBehaviour
{
    [SerializeField] private int laserCount;
    [SerializeField] private Laser laserPrefab;
    [SerializeField] private float laserDistance;
    private Laser[] lasers;

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
    [SerializeField] private float cooldownDuration;
    [SerializeField] private float initialLaserShootDelay;
    [SerializeField] private float laserStartFade;
    [SerializeField] private float laserInbetweenFade;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        lasers = new Laser[laserCount];

        float angleDiff = (360f / laserCount) * laserDistance;
        float currentAngle = angleDiff;
        laserMaterial = new Material(baseLaserMaterial);
        timerMaterial = new Material(timerBaseMaterial);
        timerSprite.material = timerMaterial;

        lasers[0] = laserPrefab;
        lasers[0].SetLaserMaterial(laserMaterial);
        for (int i = 1; i < laserCount; i++)
        {
            lasers[i] = Instantiate(laserPrefab);
            lasers[i].transform.SetParent(transform);
            lasers[i].transform.localPosition = new Vector2(0, 0);
            lasers[i].transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
            lasers[i].SetLaserMaterial(laserMaterial);



            currentAngle += angleDiff;
            Debug.Log("current angle is: " + currentAngle);
        }
        laserSpeed = finalLaserSpeed;

        StartCoroutine(LaserShoot());


    }



    private void Update()
    {

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
        while (time < cooldownDuration)
        {


            time += Time.deltaTime;
            timerMaterial.SetFloat("_RadialClip", Mathf.Lerp(360, 0, time / cooldownDuration));
            laserMaterial.SetFloat("_FadeAmount", Mathf.Lerp(laserStartFade, laserInbetweenFade, time / cooldownDuration));
            laserSpeed = Mathf.Lerp(initialLaserSpeed, finalLaserSpeed, time / cooldownDuration);
            if (!useParticles && time > cooldownDuration * .3f)
            {
                for (int i = 0; i < laserCount; i++)
                {
                    useParticles = true;
                    lasers[i].SetUseParticles(true);
                }
            }
            yield return null;
        }
        timerMaterial.SetFloat("_RadialClip", 0);
        laserSpeed = finalLaserSpeed;


        StartCoroutine(LaserShoot());



    }

    private IEnumerator LaserShoot()
    {
        float time = 0;
        while (time < initialLaserShootDelay)
        {
            time += Time.deltaTime;
            laserMaterial.SetFloat("_FadeAmount", Mathf.Lerp(laserInbetweenFade, 0, time / initialLaserShootDelay));
            yield return null;
        }

        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].HandleShooting(true);
        }

        time = 0;
        while (time < shootDuration)
        {
            time += Time.deltaTime;
            timerMaterial.SetFloat("_RadialClip", Mathf.Lerp(0, 358, time / shootDuration));
            yield return null;
        }
        timerMaterial.SetFloat("_RadialClip", 360);

        for (int i = 0; i < laserCount; i++)
        {
            lasers[i].HandleShooting(false);
        }
        time = 0;

        while (time < .3f)
        {
            time += Time.deltaTime;
            laserMaterial.SetFloat("_FadeAmount", Mathf.Lerp(0, .8f, time / .3f));
            yield return null;
        }
        laserMaterial.SetFloat("_FadeAmount", 1);



        StartCoroutine(LaserCooldown());


    }

    // Update is called once per frame

}
