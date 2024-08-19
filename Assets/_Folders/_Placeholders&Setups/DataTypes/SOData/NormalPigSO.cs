using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class NormalPigSO : EnemyData
{
    public NormalPigData[] data;
    public override void InitializeEnemy(SpawnStateManager manager)
    {

        for (int i = 0; i < data.Length; i++)
        {
            manager.GetNormalPig(data[i].position, data[i].scale, data[i].speed);
        }

    }

}
