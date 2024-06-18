using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class PigMaterialHandler : MonoBehaviour, IDamageable
{
    [SerializeField] private bool isScalable;
    [SerializeField] private Vector3 explosionScale;

    private Pool explosionPool;

    [SerializeField] private Collider2D[] colls;
    private float initialScale;
    private bool isHit;
    [SerializeField] private SpriteRenderer[] sprites;
    [SerializeField] private Material pigMaterial;
    [SerializeField] private Material defaultMat;
    [SerializeField] private Transform body;
    private Material instanceMaterial;


    [Header("SpriteObjects")]
    [SerializeField] private Transform BackLegs;
    [SerializeField] private Transform FrontLegs;
    [SerializeField] private Transform Wings;
    [SerializeField] private Transform Tail;
    [Header("SpritePositions")]
    [SerializeField] private Transform BackLegsPosition;
    [SerializeField] private Transform FrontLegsPosition;
    [SerializeField] private Transform WingsPosition;
    [SerializeField] private Transform TailPosition;


    private void Awake()
    {
        isHit = false;



    }
    private void Start()
    {
        explosionPool = PoolKit.GetPool("ExplosionPool");

        // float randomScale = Random.Range(.74f, .92f);
        // transform.localScale = new Vector3(randomScale, randomScale, randomScale);


        // float newBodyScale = (.83f - randomScale) + 1.02f;
        // body.localScale = new Vector3(newBodyScale, 1, 1);




        // Invoke("RandomColorChange", 3);




    }

    void RandomColorChange()
    {

    }

    private void OnEnable()
    {
        initialScale = transform.localScale.x;
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

    public void Damage(int damageAmount)
    {
        // instanceMaterial.SetColor("_ColorChangeNewCol", new Color(Random.Range(250, 255), Random.Range(80, 170), Random.Range(160, 180), 1));
        isHit = true;
        instanceMaterial = new Material(pigMaterial);
        foreach (var col in colls)
        {
            col.enabled = false;
        }

        foreach (var pig in sprites)
        {
            pig.material = instanceMaterial;
        }


        StartCoroutine(Explode(.45f));

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
            float scale = Mathf.Lerp(initialScale, endScale, t);
            float currentFadeAmount = Mathf.Lerp(fadeAmountStart, fadeAmountEnd, t);
            transform.localScale = new Vector3(scale, scale, scale);
            instanceMaterial.SetFloat("_HitEffectBlend", currentHitEffectBlend);
            instanceMaterial.SetFloat("_Alpha", alpha);
            instanceMaterial.SetFloat("_FadeAmount", currentFadeAmount);

            yield return null;
        }

        // Ensure the final values are set
        transform.localScale = new Vector3(endScale, endScale, endScale);
        instanceMaterial.SetFloat("_HitEffectBlend", 0);
        instanceMaterial.SetFloat("_FadeAmount", fadeAmountEnd);
        Destroy(instanceMaterial);
        gameObject.SetActive(false);
    }
}
