using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBombPool : MonoBehaviour
{
    [SerializeField] private ExplosivesPool pool;

    public PigMovementBasic obj;

    public RecordedDataStruct data;
    // Start is called before the first frame update
    void Start()
    {
        pool.MakePools();

        data = obj.RecordData();

        Debug.Log("Data speed is: " + data.speed);

    }

    // Update is called once per frame

}
