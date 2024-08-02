using System;
using UnityEngine;

[System.Serializable]
public abstract class EnemyData 
{

    public float TimeToTrigger;
    // Define any common methods or properties that all enemies will have
    public abstract void InitializeEnemy(EnemyPoolManager manger);
    // public abstract void InitializeEnemyRandom(EnemyPoolManager manger, Vector2 minMaxSpeed, Vector2 minMaxY, float xOffset);

   

}