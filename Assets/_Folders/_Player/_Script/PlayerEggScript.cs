using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class PlayerEggScript : MonoBehaviour
{
    public PlayerID ID;
    private bool rotationIsFrozen;
    private Rigidbody2D rb;
    private bool canShootBool;
    private Animator anim;
    [SerializeField] private int eggPoolAmount = 10;
    private Pool pool;
    private int ammo;

    // Start is called before the first frame update
    void Start()
    {
        pool = PoolKit.GetPool("EggPool");
        //
        rotationIsFrozen = false;
        canShootBool = true;
        anim = GetComponent<Animator>();


        ammo = ID.Ammo;
        rb = GetComponent<Rigidbody2D>();
    }

    private void EggDrop()
    {
        if (ID.UsingClocker)
        {
            if (canShootBool)
            {
                anim.SetTrigger("ShootGunTrigger");


                canShootBool = false;

            }


        }
        else if (ID.Ammo > 0)
        {
            GameObject egg = pool.SpawnGO("Egg_Regular", transform.position, Vector3.zero, null);
            AudioManager.instance.PlayEggDrop();

            if (rb.velocity.x < 0)
            {
                egg.GetComponent<Rigidbody2D>().AddForce(new Vector2(rb.velocity.x / 1.6f, -1), ForceMode2D.Impulse);
            }
            else
            {
                egg.GetComponent<Rigidbody2D>().AddForce(new Vector2(rb.velocity.x / 2, -1), ForceMode2D.Impulse);
            }

            ID.Ammo -= 1;


        }
    }
    void OnEnable()
    {
        ID.events.OnEggDrop += EggDrop;
        // ID.globalEvents.OnAddAmmo += AddAmmo;
        // ID.globalEvents.OnUpdateAmmo?.Invoke(ammo);
    }

    void OnDisable()
    {
        ID.events.OnEggDrop -= EggDrop;
        // ID.globalEvents.OnAddAmmo -= AddAmmo;
    }
}


