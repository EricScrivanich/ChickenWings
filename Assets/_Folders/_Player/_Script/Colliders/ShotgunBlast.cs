using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShotgunBlast : SpawnedQueuedObject
{

    [SerializeField] private float forceAmount;
    [SerializeField] private float xForceMultiplier;
    // private Rigidbody2D rb;
    private Sequence shotgunBlastSeq;
    [SerializeField] private AnimationDataSO animData;
    private SpriteRenderer sr;


    [SerializeField] private Vector3[] countAndVelocitys;



    private BoxCollider2D col;








    private float time;
    private int currentSpriteDelayIndex;

    private bool finished = false;
    private bool isChained;


    [SerializeField] private ShotgunParticleID data;
    [SerializeField] private GameObject shotgunParticlePrefab;
    [SerializeField] private AnimationDataSO particleSprites;
    [SerializeField] private AnimationDataSO muzzleFlashSprites;
    private ShotgunParticleNew[] bulletParticles;

    private readonly Vector3 baseScale = new Vector3(1.2f, 1, 1);

    private readonly float startFlashScale = 1.1f;
    private readonly float endFlashScale = 1.3f;

    private void Awake()
    {
        // rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        lifeTime = new WaitForSeconds(data.lifeTime);
        bulletParticles = new ShotgunParticleNew[data.TotalBulletCount()];
        for (int i = 0; i < data.TotalBulletCount(); i++)
        {
            var obj = Instantiate(shotgunParticlePrefab, this.transform);
            bulletParticles[i] = obj.GetComponent<ShotgunParticleNew>();
            if (obj == null) Debug.LogError("No ShotgunParticleNew script found on prefab!");
            if (data == null) Debug.LogError("No ShotgunParticleID data assigned!");
            obj.transform.localEulerAngles = new Vector3(0, 0, data.bulletRotationsAndYPositions[i].y);
            obj.SetActive(false);

        }


    }



    private void OnEnable()
    {






        // rb.linearVelocity = finalForce;

        // ScaleAndOpacity();

        // for (int i = 0; i < countAndVelocitys.Length; i++)
        // {
        //     for (int n = (int)countAndVelocitys[i].x; n < (int)countAndVelocitys[i].y; n++)
        //     {
        //         bulletParticles[n].linearVelocity = bulletParticles[n].transform.right * countAndVelocitys[i].z;
        //     }
        // }


    }

    private WaitForSeconds bulletLayerDelay = new WaitForSeconds(.07f);

    private WaitForSeconds lifeTime;

    private IEnumerator Shoot()
    {
        for (int i = 0; i < data.outerBulletCount; i++)
        {
            bulletParticles[i].transform.localPosition = data.bulletRotationsAndYPositions[i].x * Vector2.up + (Vector2.right * data.xOffsetBullets[i]);
            bulletParticles[i].gameObject.SetActive(true);
            bulletParticles[i].SetVelocity(data.outerBulletVel, id);

        }
        yield return bulletLayerDelay;
        for (int i = data.outerBulletCount; i < data.TotalBulletCount(); i++)
        {
            bulletParticles[i].transform.localPosition = data.bulletRotationsAndYPositions[i].x * Vector2.up + (Vector2.right * data.xOffsetBullets[i]);
            bulletParticles[i].gameObject.SetActive(true);
            bulletParticles[i].SetVelocity(data.innerBulletVel, id);


        }
        yield return lifeTime;


        for (int i = 1; i < particleSprites.sprites.Length; i++)
        {

            foreach (var p in bulletParticles)
            {
                p.SetSprite(particleSprites.sprites[i], i);
            }
            yield return bulletLayerDelay;

        }
        foreach (var p in bulletParticles)
        {
            p.SetSprite(particleSprites.sprites[0], -1);
        }



        gameObject.SetActive(false);

    }


    private void OnDisable()
    {

        finished = false;
        sr.enabled = true;
        sr.sprite = muzzleFlashSprites.sprites[0];
        sr.transform.localScale = baseScale * startFlashScale;

        ReturnToPool();
    }

    private void Update()
    {

        if (finished) return;
        else time += Time.deltaTime;
        if (time > muzzleFlashSprites.constantSwitchTime)
        {
            currentSpriteDelayIndex++;
            if (currentSpriteDelayIndex > muzzleFlashSprites.sprites.Length - 1)
            {
                currentSpriteDelayIndex = 0;
                finished = true;
                time = 0;
                sr.enabled = false;
                return;


            }
            float p = (float)currentSpriteDelayIndex / (float)(muzzleFlashSprites.sprites.Length - 1);
            sr.transform.localScale = baseScale * Mathf.Lerp(startFlashScale, endFlashScale, p);
            sr.color = Color.Lerp(data.startFlashColor, data.endFlashColor, p);
            sr.sprite = muzzleFlashSprites.sprites[currentSpriteDelayIndex];
            time = 0;

        }
    }

    public override void SpawnTransformOverride(Vector2 pos, float rotation, int ID, bool other)
    {
        transform.position = pos;
        transform.eulerAngles = new Vector3(0, 0, rotation);
        gameObject.SetActive(true);

        StartCoroutine(Shoot());
        // col.enabled = true;
        // transform.localScale = StartScale;
        sr.color = data.startFlashColor;
        time = 0;

        Vector2 force = transform.right * forceAmount;
        hasBeenBlocked = false;

        float xVelRatio = force.x / forceAmount;
        float addedX = 0;

        // if (yVelRatio > 0) addedY = yVelRatio * yForceMultiplier;
        // else if (yVelRatio < 0) addedY = yVelRatio * yForceMultiplier * -1;
        addedX = Mathf.Abs(xVelRatio * xForceMultiplier);

        Vector2 finalForce = new Vector2(force.x - addedX, force.y);


    }


    private bool hasBeenBlocked;
    // private void OnTriggerEnter2D(Collider2D collider)
    // {
    //     if (collider.gameObject.CompareTag("Block"))
    //     {
    //         hasBeenBlocked = true;
    //         AudioManager.instance.PlayParrySound();
    //         rb.linearVelocity *= .4f;
    //         DisableCollider();
    //         Debug.Log("Attack was blocked boi");
    //         return;
    //     }

    //     IDamageable damageableEntity = collider.gameObject.GetComponent<IDamageable>();
    //     if (damageableEntity != null && !hasBeenBlocked)
    //     {
    //         int type = 1;
    //         if (isChained) type = 2;
    //         damageableEntity.Damage(1, type, id);
    //         return;

    //     }
    //     IExplodable explodable = collider.gameObject.GetComponent<IExplodable>();
    //     if (explodable != null && !hasBeenBlocked)
    //     {

    //         explodable.Explode(false);
    //         return;

    //     }

    // }

    // private void DisableCollider()
    // {
    //     col.enabled = false;
    // }


}
