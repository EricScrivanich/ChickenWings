using UnityEngine;

public class EnemyBossTrigger : MonoBehaviour
{
    private LevelDataBossAndRandomLogic data;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(LevelDataBossAndRandomLogic levelBossData)
    {
        data = levelBossData;
    }

    public void OnDeathTrigger()
    {
        if (data != null)
        {
            data.DefeatBoss();
        }

    }

    void OnDisable()
    {
        Destroy(this);

    }






}
