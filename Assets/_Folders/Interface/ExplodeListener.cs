using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class ExplodeListener : MonoBehaviour, IExplodable, IDamageable
{
    [SerializeField] private Vector3 normalExplosionScale;
    [SerializeField] private Vector3 groundExplosionScale;
    [SerializeField] private bool isBomberBomb;
    [SerializeField] private PlaneManagerID ID;

    private Pool pool;


    private void Start()
    {
        pool = PoolKit.GetPool("ExplosionPool");



    }
    public void Damage(int damageAmount)
    {
        pool.Spawn("NormalExplosion", transform.position, Vector3.zero, normalExplosionScale, null);
        if (isBomberBomb) gameObject.SetActive(false);
        else pool.Despawn(this.gameObject);

    }

    public void Explode(bool isGround)
    {
        if (isGround)
        {
            // pool.Spawn("GroundExplosion", transform.position, Vector3.zero, groundExplosionScale, null);
            pool.Spawn("ExplosionBlemish", transform.position, Vector3.zero, groundExplosionScale, null);

            AudioManager.instance.PlayBombExplosionSound();

            if (isBomberBomb) gameObject.SetActive(false);
            else pool.Despawn(this.gameObject);
        }
        // else if (isGround && isBomberBomb)
        // {
        //     pool.Spawn("GroundExplosion", transform.position, Vector3.zero, groundExplosionScale, null);
        //     pool.Spawn("ExplosionBlemish", CalculateOffset(), Vector3.zero, groundExplosionScale, null);

        //     pool.Despawn(this.gameObject);

        // }
        else
        {
            pool.Spawn("NormalExplosion", transform.position, Vector3.zero, normalExplosionScale, null);

            if (isBomberBomb) gameObject.SetActive(false);
            else pool.Despawn(this.gameObject);


        }

    }

    private Vector2 CalculateOffset()
    {
        float offset = (ID.bombsDropped * -.25f) + 1;
        return new Vector2(transform.position.x + offset, transform.position.y);

    }

    // Start is called before the first frame update


    // Update is called once per frame

}
