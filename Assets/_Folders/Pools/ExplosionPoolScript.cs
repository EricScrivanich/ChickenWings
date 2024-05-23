using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class ExplosionPoolScript : MonoBehaviour
{
    private Pool pool;
    private void OnEnable() {
        pool = PoolKit.GetPool("ExplosionPool");

    }
    // Start is called before the first frame update
//    public static void GetGroundExplosion(Vector3 scale)
//    {
//     pool.Spawn("GroundExplosion",)

//    }
}
