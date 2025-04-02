using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBombPool : MonoBehaviour
{
    [SerializeField] private ExplosivesPool pool;


    // Start is called before the first frame update
    void Start()
    {
        pool.MakePools();



    }

    // Update is called once per frame

}
