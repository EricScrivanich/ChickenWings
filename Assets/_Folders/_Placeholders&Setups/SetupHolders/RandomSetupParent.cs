using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RandomSpawnSetupParent", menuName = "Setups/RandomSetupParent")]
public class RandomSetupParent : ScriptableObject
{
    [SerializeField] public List<NormalPigSO> pigNormalArray;
    [SerializeField] public List<JetPackPigSO> pigJetPackArray = new List<JetPackPigSO>();
    [SerializeField] public List<TenderizerPigSO> pigTenderizerArray = new List<TenderizerPigSO>();
    [SerializeField] public List<BigPigSO> pigBigArray = new List<BigPigSO>();
    [SerializeField] public List<RingSO> ringArray = new List<RingSO>();

    // Start is called before the first frame update
    private void EnsureListSize<T>(List<T> list, int index) where T : new()
    {
        while (list.Count <= index)
        {
            list.Add(new T());
        }
    }

    public void FillEnemyArrays(EnemyData data, int index)
    {
        EnsureListSize(pigNormalArray, index);
        EnsureListSize(pigBigArray, index);
        EnsureListSize(pigJetPackArray, index);
        EnsureListSize(pigTenderizerArray, index);

        if (data is NormalPigSO so)
        {
            if (pigNormalArray == null)
            {
                pigNormalArray = new List<NormalPigSO>();
                pigNormalArray.Add(so);
            }
            else pigNormalArray[index] = so;
        }
        else if (data is BigPigSO so1)
        {
            pigBigArray[index] = so1;
        }
        else if (data is JetPackPigSO so2)
        {
            pigJetPackArray[index] = so2;
        }
        else if (data is TenderizerPigSO so3)
        {
            pigTenderizerArray[index] = so3;
        }
    }

    public void FillCollectableArrays(CollectableData data, int index)
    {
        EnsureListSize(ringArray, index);

        if (data is RingSO so)
        {
            ringArray[index] = so;
        }
    }
}
