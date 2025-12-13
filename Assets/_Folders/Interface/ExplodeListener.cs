using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class ExplodeListener : MonoBehaviour, IExplodable
{

    [SerializeField] private Vector3 groundExplosionScale;
    [SerializeField] private float yOffsetGround;
    [SerializeField] private float airExplosionScale;
    [SerializeField] private bool isBomberBomb;
    [SerializeField] private PlaneManagerID ID;

    [SerializeField] private byte waitbeforeDespawnTime;

    [SerializeField] private QEffectPool groundExplosionPool;
    [SerializeField] private QEffectPool normalExplosionPool;

    // private Pool pool;

    public bool CanPerfectScythe => false;

    // private void Start()
    // {
    //     pool = PoolKit.GetPool("ExplosionPool");



    // }

    private void Awake()
    {
        if (waitbeforeDespawnTime > 0)
        {
            waitDespawn = new WaitForSecondsRealtime((float)waitbeforeDespawnTime / 100);
        }
    }


    public void Explode(bool isGround)
    {
        if (isGround)
        {
            // pool.Spawn("GroundExplosion", transform.position, Vector3.zero, groundExplosionScale, null);
            // pool.Spawn("ExplosionBlemish", transform.position, Vector3.zero, groundExplosionScale, null);
            groundExplosionPool.SpawnSpecificScale((Vector2)transform.position + (Vector2.up * yOffsetGround), groundExplosionScale);

            AudioManager.instance.PlayBombExplosionSound(1 - (Mathf.Abs(transform.position.x) / BoundariesManager.rightBoundary));


        }
        // else if (isGround && isBomberBomb)
        // {
        //     pool.Spawn("GroundExplosion", transform.position, Vector3.zero, groundExplosionScale, null);
        //     pool.Spawn("ExplosionBlemish", CalculateOffset(), Vector3.zero, groundExplosionScale, null);

        //     pool.Despawn(this.gameObject);

        // }
        else
        {
            AudioManager.instance.PlayBombExplosionSound();
            // pool.Spawn("NormalExplosion", transform.position, Vector3.zero, normalExplosionScale, null);
            normalExplosionPool.SpawnUniformScale(transform.position, airExplosionScale);

            // if (isBomberBomb) gameObject.SetActive(false);
            // else pool.Despawn(this.gameObject);


        }

        if (waitbeforeDespawnTime > 0)
        {
            GetComponent<Rigidbody2D>().simulated = false;
            GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(DespawnAfterTime());
        }
        else
            gameObject.SetActive(false);

    }

    private Vector2 CalculateOffset()
    {
        float offset = (ID.bombsDropped * -.25f) + 1;
        return new Vector2(transform.position.x + offset, transform.position.y);

    }

    private WaitForSecondsRealtime waitDespawn;
    private IEnumerator DespawnAfterTime()
    {
        yield return waitDespawn;

        gameObject.SetActive(false);
        GetComponent<Rigidbody2D>().simulated = true;
        GetComponent<SpriteRenderer>().enabled = true;

    }

    // Start is called before the first frame update


    // Update is called once per frame

}
