using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenuAttribute(fileName = "TenderizerPigSetup", menuName = "IndividualSetups")]
[System.Serializable]
public class TenderizerPigSO : EnemyData
{
    public TenderizerPigData[] data;
    // Start is called before the first frame update
    public override void InitializeEnemy(EnemyPoolManager manager)
    {
        

        for (int i = 0; i < data.Length; i++)
        {
            manager.GetTenderizerPig(data[i].position, data[i].scale, data[i].speed,data[i].hasHammer);
        }

    }
}
