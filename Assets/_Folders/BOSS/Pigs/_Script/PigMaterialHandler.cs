using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;
using DG.Tweening;

public class PigMaterialHandler : MonoBehaviour, IDamageable
{
    // 0 normal, 1 jetPack, 2 bigPig, 3 tenderizer

    [SerializeField] private PlayerID player;
    // [SerializeField] private SpriteRenderer headSprite;
    [SerializeField] private GameObject enableObjectOnDeath;
    [SerializeField] private GameObject disableObjectOnDeath;
    [SerializeField] private bool ignoreBoundaries = false;


    [SerializeField] private int health = 1;
    [SerializeField] private float totalDamageTime;

    private int currentHealth;
    private bool isStuck;


    [Header("0-Normal; 1-Jetpack; 2-BigPig; 3-Tenderizer; 4-Pilot; 5- Missile; 6-BomberPlane; 7-Flappy; 8-Gas; 9-Balloon")]
    [SerializeField] private int pigType;
    [SerializeField] private bool isScalable;
    [SerializeField] private int pigTypeAudio;
    [SerializeField] private Vector3 explosionScale;



    private Pool explosionPool;

    [SerializeField] private Collider2D[] colls;
    private float initialScale;
    private bool isHit;
    [SerializeField] private SpriteRenderer[] sprites;
    [SerializeField] private Material pigMaterial;
    [SerializeField] private Material defaultMat;

    private Material instanceMaterial;

    private bool hasCrossedScreen;
    private bool canPerfectScythe;

    public bool CanPerfectScythe => !isHit;

    private void Awake()
    {
        isHit = false;



    }
    private void Start()
    {
        explosionPool = PoolKit.GetPool("ExplosionPool");

        if (health > 1)
        {
            instanceMaterial = new Material(pigMaterial);
            instanceMaterial.SetFloat("_HitEffectBlend", 0);


            foreach (var pig in sprites)
            {
                pig.material = instanceMaterial;
            }

        }








    }

    public void Tick()
    {
        float x = transform.position.x;
        if (!hasCrossedScreen && x > BoundariesManager.leftBoundary && x < BoundariesManager.rightBoundary)
        {
            hasCrossedScreen = true;
        }
        else if (hasCrossedScreen && (x < BoundariesManager.leftBoundary || x > BoundariesManager.rightBoundary))
        {
            gameObject.SetActive(false);


        }


    }

    void RandomColorChange()
    {

    }

    private void OnEnable()
    {
        if (!ignoreBoundaries)
            Ticker.OnTickAction015 += Tick;



        currentHealth = health;
        initialScale = transform.localScale.x;

        hasCrossedScreen = false;
        if (isHit)
        {
            foreach (var pig in sprites)
            {
                pig.material = defaultMat;
            }
            foreach (var col in colls)
            {
                col.enabled = true;
            }
            isHit = false;
            isStuck = false;

            if (enableObjectOnDeath != null) enableObjectOnDeath.SetActive(false);
            if (disableObjectOnDeath != null) disableObjectOnDeath.SetActive(true);

        }

        // if (isScalable)
        // {
        //     float subtract = .75f - transform.localScale.x + 1;

        //     body.localScale = new Vector3(subtract, 1, 1);

        //     BackLegs.position = BackLegsPosition.position;
        //     Wings.position = WingsPosition.position;
        //     Tail.position = TailPosition.position;
        // }






        // Color newColor = new Color(
        //     Random.Range(250f, 255f) / 255f,
        //     Random.Range(80f, 170f) / 255f,
        //     Random.Range(160f, 180f) / 255f,
        //     1f
        // );
        // Debug.Log(newColor);
        // instanceMaterial.SetColor("_ColorChangeNewCol", newColor);

    }

    private void OnDisable()
    {
        if (!ignoreBoundaries)
            Ticker.OnTickAction015 -= Tick;

    }

    public void Damage(int damageAmount, int type, int id)
    {
        if (!isHit && (!isStuck || type == 4))
        {

            if (damageAmount == -1)
            {
                AudioManager.instance.PlayScytheHitNoise(true);
                isStuck = true;
                player.globalEvents.OnScythePig?.Invoke(this, pigType);
                return;
            }

            if (type == 3) AudioManager.instance.PlayScytheHitNoise(false);
            isHit = true;

            if (enableObjectOnDeath != null) enableObjectOnDeath.SetActive(true);
            if (disableObjectOnDeath != null) disableObjectOnDeath.SetActive(false);


            AudioManager.instance.PlayPigDeathSound(pigTypeAudio);

            currentHealth -= damageAmount;




            if (currentHealth <= 0)
            {

                player.AddKillPig(pigType, type, id);




                foreach (var col in colls)
                {
                    col.enabled = false;
                }

                if (health <= 1)
                {
                    instanceMaterial = new Material(pigMaterial);

                    foreach (var pig in sprites)
                    {
                        pig.material = instanceMaterial;
                    }

                }


                StartCoroutine(Explode(.45f));

            }
            else
            {
                foreach (var col in colls)
                {
                    col.enabled = false;
                }
                StartCoroutine(DamageCor(totalDamageTime));
            }


        }
        // instanceMaterial.SetColor("_ColorChangeNewCol", new Color(Random.Range(250, 255), Random.Range(80, 170), Random.Range(160, 180), 1));
    }

    private IEnumerator Explode(float time)
    {
        float elapsedTime = 0.0f;
        float endScale = initialScale * 1.2f;
        float hitEffectBlendStart = 0f;
        float hitEffectBlendEnd = 0.1f;
        float fadeAmountStart = 0.0f;
        float fadeAmountEnd = .7f;
        bool hasExploded = false;

        transform.DOScale(transform.localScale * 1.25f, .65f);
        while (elapsedTime < .15f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / time;

            // Interpolate _HitEffectBlend and _FadeAmount
            float currentHitEffectBlend = Mathf.Lerp(hitEffectBlendStart, hitEffectBlendEnd, t);


            instanceMaterial.SetFloat("_HitEffectBlend", currentHitEffectBlend);


            yield return null;
        }
        elapsedTime = 0;
        explosionPool.Spawn("NormalExplosion", transform.position, Vector3.zero, explosionScale);

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / time;
            if (elapsedTime > time * .5f && !hasExploded)
            {
                hasExploded = true;

            }

            // Interpolate _HitEffectBlend and _FadeAmount
            float currentHitEffectBlend = Mathf.Lerp(hitEffectBlendEnd, hitEffectBlendStart, t);
            float alpha = Mathf.Lerp(1, .2f, t);
            // float scale = Mathf.Lerp(initialScale, endScale, t);
            float currentFadeAmount = Mathf.Lerp(fadeAmountStart, fadeAmountEnd, t);
            // transform.localScale = new Vector3(scale, scale, scale);
            instanceMaterial.SetFloat("_HitEffectBlend", currentHitEffectBlend);
            instanceMaterial.SetFloat("_Alpha", alpha);
            instanceMaterial.SetFloat("_FadeAmount", currentFadeAmount);

            yield return null;
        }
        player.globalEvents.OnKillPig?.Invoke(pigType);
        isStuck = false;

        gameObject.SetActive(false);
        // Ensure the final values are set
        // transform.localScale = new Vector3(endScale, endScale, endScale);


        Destroy(instanceMaterial);


    }

    private IEnumerator DamageCor(float time)
    {
        float elapsedTime = 0.0f;
        float cycleTime = time / 3f; // Divide the total time into 3 cycles
        float hitEffectBlendStart = 0f;
        float hitEffectBlendEnd = 0.1f;

        for (int i = 0; i < 3; i++) // Loop 3 times for 3 cycles
        {
            elapsedTime = 0;

            // Blend towards End
            while (elapsedTime < cycleTime / 2f) // First half of the cycle
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / (cycleTime / 2f);
                float currentHitEffectBlend = Mathf.Lerp(hitEffectBlendStart, hitEffectBlendEnd, t);

                instanceMaterial.SetFloat("_HitEffectBlend", currentHitEffectBlend);
                yield return null;
            }

            elapsedTime = 0;

            // Blend back to Start
            while (elapsedTime < cycleTime / 2f) // Second half of the cycle
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / (cycleTime / 2f);
                float currentHitEffectBlend = Mathf.Lerp(hitEffectBlendEnd, hitEffectBlendStart, t);

                instanceMaterial.SetFloat("_HitEffectBlend", currentHitEffectBlend);
                yield return null;
            }
        }
        foreach (var col in colls)
        {
            col.enabled = true;
        }
        isStuck = false;
        isHit = false;
    }
}

