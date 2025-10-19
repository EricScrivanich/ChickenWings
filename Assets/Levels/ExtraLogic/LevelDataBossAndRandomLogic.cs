using UnityEngine;
[CreateAssetMenu(fileName = "Level Data Boss And Random Logic", menuName = "Setups/LevelDataBossAndRandomLogic")]
public class LevelDataBossAndRandomLogic : ScriptableObject
{
    [SerializeField] private ushort spawnStep;
    private LevelData levelData;
    public bool stopWaveTime;
    [SerializeField] private CollectableSpawnData collectableSpawnData;
    [SerializeField] private int amountOfBosses;

    [SerializeField] private short finishCondition;
    private int bossesDefeated;

    public void Initialize(LevelData data)
    {
        levelData = data;
        bossesDefeated = 0;
    }
    public void CheckIfSpawnStep(ushort step)
    {
        if (step == spawnStep)
        {
            levelData.spawner.HandleWaveTime(!stopWaveTime);
            levelData.spawner.HandleRandomCollectableSpawning(collectableSpawnData);
        }

    }

    public void DefeatBoss()
    {
        bossesDefeated++;
        if (bossesDefeated >= amountOfBosses)
        {
            if (finishCondition > 0)
            {
                levelData.spawner.SetCurrentStep((ushort)finishCondition);
                return;
            }
            switch (finishCondition)
            {
                case -1:
                    levelData.spawner.HandleWaveTime(true);
                    return;

            }





        }
    }

    public CollectableSpawnData ReturnCollectableSpawnData()
    {
        return collectableSpawnData;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
