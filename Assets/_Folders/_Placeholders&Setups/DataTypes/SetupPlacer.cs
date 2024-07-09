using System.Collections.Generic;
using UnityEngine;

public class SetupPlacer: MonoBehaviour
{
    public GameObject JetPackPigPrefab;
    public GameObject NormalPigPrefab;
    public GameObject TenderizerPigPrefab;

    public void PlaceSetup(EnemyData[] dataArray)
    {
        foreach (var item in dataArray)
        {
            PlaceEnemies(item);
        }
    }

    public void PlaceEnemies(EnemyData data)
    {
        if (data is NormalPigSO normalPigData)
        {
            foreach (var pigData in normalPigData.data)
            {
                var obj = Object.Instantiate(NormalPigPrefab, pigData.position, Quaternion.identity);
                obj.transform.parent = this.transform;
                var script = obj.GetComponent<PigMovementBasic>();
                script.xSpeed = pigData.speed;
                obj.transform.localScale = pigData.scale;
            }
        }
        else if (data is JetPackPigSO jetPackPigData)
        {
            foreach (var pigData in jetPackPigData.data)
            {
                var obj = Object.Instantiate(JetPackPigPrefab, pigData.position, Quaternion.identity);
                obj.transform.parent = this.transform;

                var script = obj.GetComponent<JetPackPigMovement>();
                script.speed = pigData.speed;
                obj.transform.localScale = pigData.scale;
            }
        }
        else if (data is TenderizerPigSO tenderizerPigData)
        {
            foreach (var pigData in tenderizerPigData.data)
            {
                var obj = Object.Instantiate(TenderizerPigPrefab, pigData.position, Quaternion.identity);
                obj.transform.parent = this.transform;

                var script = obj.GetComponent<TenderizerPig>();
                script.speed = pigData.speed;
                script.hasHammer = pigData.hasHammer;
                obj.transform.localScale = pigData.scale;
            }
        }
    }
}