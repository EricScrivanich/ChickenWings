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
    public GameObject MissilePigPrefab;  // New addition for Missile Pig
    public GameObject SiloPrefab;        // New addition for Silo
    public GameObject WindMillPrefab;    // New addition for Wind Mill
    public GameObject GasPigPrefab;      // New addition for Gas Pig
    public GameObject HotAirBalloonPrefab;  // New addition for Hot Air Balloon
    public GameObject FlappyPigPrefab;   // New addition for Flappy Pig
    public GameObject BomberPlanePrefab;
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

        else if (data is MissilePigSO missilePigData)
        {
            foreach (var pigData in missilePigData.data)
            {
                var obj = Object.Instantiate(MissilePigPrefab, pigData.position, Quaternion.identity);
                obj.transform.parent = EnemySetupTransform;
                var script = obj.GetComponent<MissilePigScript>();
                script.missileType = pigData.missileType;
                script.movementType = pigData.movementType;

            }
        }
        else if (data is SiloSO siloData)
        {
            foreach (var siloDataItem in siloData.data)
            {
                var obj = Object.Instantiate(SiloPrefab, siloDataItem.position, Quaternion.identity);
                obj.transform.parent = EnemySetupTransform;
                var script = obj.GetComponent<SiloMovement>();
                script.type = siloDataItem.type;
                script.baseHeightMultiplier = siloDataItem.baseHeightMultiplier;
                obj.transform.localScale = new Vector3(1, 1, 1);  // Assuming uniform scaling
            }
        }
        else if (data is WindMillSO windMillData)
        {
            foreach (var windMillDataItem in windMillData.data)
            {
                var obj = Object.Instantiate(WindMillPrefab, windMillDataItem.position, Quaternion.identity);
                obj.transform.parent = EnemySetupTransform;
                var script = obj.GetComponent<Windmill>();
                script.bladeAmount = windMillDataItem.bladeAmount;
                script.bladeScaleMultiplier = windMillDataItem.bladeScaleMultiplier;
                script.bladeSpeed = windMillDataItem.bladeSpeed;
                script.heightMultiplier = windMillDataItem.heightMultiplier;
                // Assuming uniform scaling
            }
        }
        else if (data is GasPigSO gasPigData)
        {
            foreach (var gasPigDataItem in gasPigData.data)
            {
                var obj = Object.Instantiate(GasPigPrefab, gasPigDataItem.position, Quaternion.identity);
                obj.transform.parent = EnemySetupTransform;
                var script = obj.GetComponent<GasPig>();
                script.speed = gasPigDataItem.speed;
                script.delay = gasPigDataItem.delay;

            }
        }
        else if (data is HotAirBalloonSO hotAirBalloonData)
        {
            foreach (var hotAirBalloonDataItem in hotAirBalloonData.data)
            {
                var obj = Object.Instantiate(HotAirBalloonPrefab, hotAirBalloonDataItem.position, Quaternion.identity);
                obj.transform.parent = EnemySetupTransform;
                var script = obj.GetComponent<HotAirBalloon>();
                script.type = hotAirBalloonDataItem.type;
                script.xTrigger = hotAirBalloonDataItem.xTrigger;
                script.yTarget = hotAirBalloonDataItem.yTarget;
                script.speed = hotAirBalloonDataItem.speed;
                script.delay = hotAirBalloonDataItem.delay;

            }
        }
        else if (data is FlappyPigSO flappyPigData)
        {
            foreach (var flappyPigDataItem in flappyPigData.data)
            {
                var obj = Object.Instantiate(FlappyPigPrefab, flappyPigDataItem.position, Quaternion.identity);
                obj.transform.parent = EnemySetupTransform;
                var script = obj.GetComponent<FlappyPigMovement>();
                script.scaleFactor = flappyPigDataItem.scaleFactor;
                float s = flappyPigDataItem.scaleFactor;
                obj.transform.localScale = new Vector3(s, s, s);  // Assuming uniform scaling
            }
        }
        else if (data is BomberPlaneSO bomberPlaneData)
        {
            foreach (var bomberPlaneDataItem in bomberPlaneData.data)
            {
                var obj = Object.Instantiate(BomberPlanePrefab, new Vector2(12, 8), Quaternion.identity);
                obj.transform.parent = EnemySetupTransform;
                var script = obj.GetComponent<DropBomb>();
                script.dropAreaScaleMultiplier = bomberPlaneDataItem.dropAreaScaleMultiplier;
                script.xDropPosition = bomberPlaneDataItem.xDropPosition;
                script.speedTarget = bomberPlaneDataItem.speedTarget;

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