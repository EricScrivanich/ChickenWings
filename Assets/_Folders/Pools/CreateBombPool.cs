using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBombPool : MonoBehaviour
{
    [SerializeField] private ExplosivesPool pool;

    [SerializeField] private PooledQueue[] PooledQs;


    // Start is called before the first frame update
    void Start()
    {
        pool.MakePools();

        foreach (var p in PooledQs)
        {
            p.CreatePool();
        }




    }

    // Update is called once per frame

}
