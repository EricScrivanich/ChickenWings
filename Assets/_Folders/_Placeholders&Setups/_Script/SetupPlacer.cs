using System.Collections.Generic;
using UnityEngine;

public class SetupPlacer : MonoBehaviour
{
    [SerializeField] private Transform CollectableSetupTransform;
    [SerializeField] private Transform EnemySetupTransform;
    public GameObject JetPackPigPrefab;
    public GameObject NormalPigPrefab;
    public GameObject TenderizerPigPrefab;
    public GameObject BigPigPrefab;
    public GameObject PilotPigPrefab;
    public GameObject RingPrefab;

    public GameObject SquareArea;

    [SerializeField] private SpriteRenderer[] borderLines;

    private void Start()
    {
        ShowBorderLines(false);
    }

    public void PlaceColllectableSetup(CollectableData[] dataArray)
    {
        foreach (var item in dataArray)
        {
            PlaceColletables(item);
        }
    }

    public void PlaceColletables(CollectableData data)
    {
        if (data is RingSO dataVar)
        {
            foreach (var collData in dataVar.data)
            {
                var obj = Object.Instantiate(RingPrefab, collData.position, Quaternion.identity);
                obj.transform.parent = CollectableSetupTransform;
                var script = obj.GetComponent<RingMovement>();
                script.speed = collData.speed;
                obj.transform.localScale = collData.scale;
                obj.transform.rotation = collData.rotation;
            }
        }

    }
    public void PlaceEnemySetup(EnemyData[] dataArray)
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
                obj.transform.parent = EnemySetupTransform;
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
                obj.transform.parent = EnemySetupTransform;

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
                obj.transform.parent = EnemySetupTransform;

                var script = obj.GetComponent<TenderizerPig>();
                script.speed = pigData.speed;
                script.hasHammer = pigData.hasHammer;
                obj.transform.localScale = pigData.scale;
            }
        }

        else if (data is BigPigSO dataType)
        {
            foreach (var pigData in dataType.data)
            {
                var obj = Object.Instantiate(BigPigPrefab, pigData.position, Quaternion.identity);
                obj.transform.parent = EnemySetupTransform;

                var script = obj.GetComponent<BigPigMovement>();
                script.speed = pigData.speed;
                script.yForce = pigData.yForce;
                script.distanceToFlap = pigData.distanceToFlap;
                script.startingFallSpot = pigData.startingFallSpot;

                obj.transform.localScale = pigData.scale;

            }
        }
        else if (data is PilotPigSO pilotPigData)
        {
            foreach (var pigData in pilotPigData.data)
            {
                var obj = Object.Instantiate(PilotPigPrefab, pigData.position, Quaternion.identity);
                obj.transform.parent = EnemySetupTransform;

                var script = obj.GetComponent<PilotPig>();
                script.initialSpeed = pigData.speed;
                script.flightMode = pigData.flightMode;
                script.minY = pigData.minY;
                script.maxY = pigData.maxY;
                script.addForceY = pigData.yForce;
                script.maxYSpeed = pigData.maxYSpeed;
                script.xTrigger = pigData.xTrigger;

                obj.transform.localScale = pigData.scale;
            }
        }
    }

    public void ShowBorderLines(bool show)
    {
        foreach (var border in borderLines)
        {
            border.enabled = show;
        }
    }
}