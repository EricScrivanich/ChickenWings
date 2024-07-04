using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpawnSetup : ScriptableObject
{
    public EnemyData[] enemySetups;
    public CollectableData[] collectableSetups;

    public void SpawnEnemies()
    {
        if (enemySetups.Length > 0)
        {
            foreach (var enemy in enemySetups)
            {
                enemy.InitializeEnemy();
            }
        }

    }

    public void SpawnCollectables()
    {
        if (collectableSetups.Length > 0)
        {
            foreach (var collectable in collectableSetups)
            {
                collectable.InitializeCollectable();
            }
        }
    }

}
