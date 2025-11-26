using UnityEngine;
[CreateAssetMenu(fileName = "Level Data Boss And Random Logic", menuName = "Setups/LevelDataBossAndRandomLogic")]
public class LevelDataBossAndRandomLogic : ScriptableObject
{
    [SerializeField] private ushort spawnStep;
    private LevelData levelData;
    public bool stopWaveTime;
    [SerializeField] private CollectableSpawnData collectableSpawnData;
    [SerializeField] private int amountOfBosses;

    [SerializeField] private int stepsPerEnemySpawn;

    [SerializeField] private short finishCondition;
    private int bossesDefeated;

    public void Initialize(LevelData data)
    {
        levelData = data;
        bossesDefeated = 0;
    }
    public void CheckIfSpawnStep(ushort step)
    {
        Debug.Log("Checking spawn step of: " + step);
        if (step == spawnStep)
        {
            Debug.Log("Correct Spawn Step: " + step);
            levelData.spawner.HandleWaveTime(!stopWaveTime, true);
            levelData.spawner.HandleRandomCollectableSpawning(collectableSpawnData);
            levelData.SetRandomEnemySpawnSteps(stepsPerEnemySpawn);
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

            }
            else
            {
                switch (finishCondition)
                {
                    case -1:
                        levelData.spawner.HandleWaveTime(true, true);
                        break;
                }
            }
            levelData.spawner.HandleRandomCollectableSpawning(null);
            levelData.SetRandomEnemySpawnSteps(0);
        }

    }

    public CollectableSpawnData ReturnCollectableSpawnData()
    {
        return collectableSpawnData;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
