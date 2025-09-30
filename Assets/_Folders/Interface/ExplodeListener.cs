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

    [SerializeField] private QEffectPool groundExplosionPool;
    [SerializeField] private QEffectPool normalExplosionPool;

    // private Pool pool;

    public bool CanPerfectScythe => false;

    // private void Start()
    // {
    //     pool = PoolKit.GetPool("ExplosionPool");



    // }


    public void Explode(bool isGround)
    {
        if (isGround)
        {
            // pool.Spawn("GroundExplosion", transform.position, Vector3.zero, groundExplosionScale, null);
            // pool.Spawn("ExplosionBlemish", transform.position, Vector3.zero, groundExplosionScale, null);
            groundExplosionPool.SpawnSpecificScale((Vector2)transform.position + (Vector2.up * yOffsetGround), groundExplosionScale);

            AudioManager.instance.PlayBombExplosionSound();


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
        gameObject.SetActive(false);

    }

    private Vector2 CalculateOffset()
    {
        float offset = (ID.bombsDropped * -.25f) + 1;
        return new Vector2(transform.position.x + offset, transform.position.y);

    }

    // Start is called before the first frame update


    // Update is called once per frame

}
