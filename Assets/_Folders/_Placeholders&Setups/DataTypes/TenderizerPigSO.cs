using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenderizerPigSO : EnemyData
{
    public TenderizerPigData[] data;
    // Start is called before the first frame update
    public override void InitializeEnemy()
    {
        var script = GameObject.Find("EnemyManager").GetComponent<EnemyPoolManager>();

        for (int i = 0; i < data.Length; i++)
        {
            script.GetTenderizerPig(data[i].position, data[i].scale, data[i].speed,data[i].hasHammer);
        }

    }
}
