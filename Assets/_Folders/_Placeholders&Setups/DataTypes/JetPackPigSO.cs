using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class JetPackPigSO : EnemyData
{
    public JetPackPigData[] data;
    public override void InitializeEnemy()
    {
        var script = GameObject.Find("EnemyManager").GetComponent<EnemyPoolManager>();

        for (int i = 0; i < data.Length; i++)
        {
            script.GetJetPackPig(data[i].position, data[i].scale, data[i].speed);
        }

    }

   

    // Start is called before the first frame update

}
