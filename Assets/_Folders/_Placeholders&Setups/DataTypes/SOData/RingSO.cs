using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RingSO : CollectableData
{
    public RingData[] data;

    
    public override void InitializeCollectable(SpawnStateManager manager,bool finalTrigger)
    {
        
        for (int i = 0; i < data.Length; i++)
        {
            if (finalTrigger && i == data.Length - 1)
            {
                manager.GetRing(data[i].position, data[i].scale, data[i].rotation, data[i].speed, true);
            }
            else 
            manager.GetRing(data[i].position, data[i].scale, data[i].rotation, data[i].speed,false);
        }
        
    }

    // Start is called before the first frame update
   
}
