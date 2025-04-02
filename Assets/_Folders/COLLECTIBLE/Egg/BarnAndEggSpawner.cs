
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnAndEggSpawner : MonoBehaviour
{
    public PlayerID player;

    public CollectableSpawnData SpawnData;

    private Queue<float> windmillSpawns;

    [SerializeField] private LevelManagerID lvlID;

    private bool checkEggs = false;
    private bool checkShotgun = false;

    private Coroutine EggRoutine;
    private Coroutine ShotgunRoutine;
    private Coroutine BarnRoutine;

    private Vector2 nextEggData;
    private Vector2 nextShotgunData;
    private Vector2 nextBarnData;

    [SerializeField] private bool usingEgg;
    [SerializeField] private bool usingShotgun;
    [SerializeField] private bool usingBarn;

    private bool spawningEgg;
    private bool spawningShotgun;
    private bool spawningBarn;
    [SerializeField] private bool spawnNormalRandom = true;
    [SerializeField] private bool spawnShotgunRandom = true;


    [SerializeField] private Sprite eggImage;
    [SerializeField] private Sprite eggThreeImage;
    [SerializeField] private Sprite shotgunImage;
    [SerializeField] private Sprite shotgunThreeImage;

    [Header("Egg")]
    [SerializeField] private GameObject eggCollectablePrefab;
    [SerializeField] private GameObject shotgunCollectablePrefab;
    [SerializeField] private int eggCollectableCount;
    private int currentEggCollectableIndex;
    private int currentShotgunCollectableIndex;

    [SerializeField] private Vector2 eggTimeRange;
    private float eggOffsetTime = 0;
    [SerializeField] private float baseEggThreeChance;
    private float eggThreeChance;
    [SerializeField] private float baseShotgunThreeChance;
    private float shotgunThreeChance;

    [Header("Barn")]
    [SerializeField] private GameObject barnPrefab;
    [SerializeField] private Vector2 barnTimeRange;
    private float barnTimeOffset;



    [SerializeField] private float barnBigChance;
    [SerializeField] private float barnMidChance;

    [Header("Mana")]
    [SerializeField] private Vector2 manaTimeRange;


    private Vector2 barnSpawnPos = new Vector2(14.5f, -5.17f);
    private EggCollectableMovement[] eggCollectables;
    private int currentBarnIndex = 0;
    private BarnMovement[] barnScript = new BarnMovement[2];

    private int currentAmmoParticle = 0;
    private int currentManaParticle = 0;
    [SerializeField] private ParticleSystem ammoParticlePrefab;
    [SerializeField] private ParticleSystem manaParticlePrefab;

    private ParticleSystem[] ammoParticles = new ParticleSystem[2];
    private ParticleSystem[] manaParticles = new ParticleSystem[2];

    private static System.Action<Vector2, int> OnGetAmmoParticles;

    private bool justSpawnedEnemies = false;

    // Start is called before the first frame update

    private void Awake()
    {
        spawningBarn = false;
        spawningEgg = false;
        spawningShotgun = false;

        if (usingEgg || usingShotgun)
        {
            eggCollectables = new EggCollectableMovement[eggCollectableCount];
            currentEggCollectableIndex = 0;
            for (int i = 0; i < eggCollectableCount; i++)
            {
                var obj = Instantiate(eggCollectablePrefab);
                var script = obj.GetComponent<EggCollectableMovement>();
                script.spawner = this;
                eggCollectables[i] = script;
                script.gameObject.SetActive(false);
            }
            if (usingEgg)
            {

                for (int i = 0; i < 2; i++)
                {
                    var ps = Instantiate(ammoParticlePrefab);
                    ammoParticles[i] = ps;

                }

            }


            if (usingShotgun)
            {
                for (int i = 0; i < 2; i++)
                {
                    var ps = Instantiate(manaParticlePrefab);
                    manaParticles[i] = ps;

                }
            }
        }
        if (usingBarn)
        {
            for (int i = 0; i < 2; i++)
            {
                var barnObj = Instantiate(barnPrefab);
                barnScript[i] = barnObj.GetComponent<BarnMovement>();
                barnObj.SetActive(false);
            }

        }


    }

    public void AddWindmillSpawn(float xPos)
    {
        if (windmillSpawns == null)
            windmillSpawns = new Queue<float>();

        windmillSpawns.Enqueue(xPos);

        // Start a coroutine to dequeue this entry after the calculated time
        StartCoroutine(RemoveWindmillSpawnAfterDelay(xPos));
    }

    private IEnumerator RemoveWindmillSpawnAfterDelay(float xPos)
    {
        // Base delay of 1.5 seconds
        float delay = 1.3f;

        // Add 0.5 seconds for every 0.5 units greater than barnSpawnPos.x
        if (xPos > barnSpawnPos.x)
        {
            float additionalTime = ((xPos - barnSpawnPos.x) * 1.2f) * 0.5f;
            delay += additionalTime;
        }

        // Wait for the calculated delay
        yield return new WaitForSeconds(delay);

        // Remove the xPos from the queue
        if (windmillSpawns != null && windmillSpawns.Count > 0 && windmillSpawns.Peek() == xPos)
        {
            windmillSpawns.Dequeue();
            Debug.Log($"Dequeued windmill spawn at {xPos} after {delay} seconds.");
        }
    }

    public void JustSpawnedEnemies()
    {
        justSpawnedEnemies = true;

    }
    void Start()
    {
        if (usingBarn)
            barnSpawnPos = new Vector2(13.5f, BoundariesManager.GroundPosition - .1f);

        if (SpawnData != null)
            SetNewData(SpawnData);




        // if (SpawnData != null)
        //     return;
        // if (usingEgg)
        // {


        //     eggThreeChance = baseEggThreeChance;


        //     for (int i = 0; i < 2; i++)
        //     {
        //         var ps = Instantiate(ammoParticlePrefab);
        //         ammoParticles[i] = ps;

        //     }

        //     if (spawnNormalRandom)
        //     {
        //         EggRoutine = StartCoroutine(SpawnEggs());
        //         BarnRoutine = StartCoroutine(SpawnBarn());
        //     }


        // }


        // if (usingShotgun)
        // {
        //     for (int i = 0; i < 2; i++)
        //     {
        //         var ps = Instantiate(manaParticlePrefab);
        //         manaParticles[i] = ps;

        //     }

        //     if (spawnShotgunRandom)
        //         ShotgunRoutine = StartCoroutine(SpawnShotgun());

        // }
    }

    public void GetParticles(bool isNormal, Vector2 pos)
    {
        if (isNormal)
        {
            if (currentAmmoParticle > 1)
            {
                currentAmmoParticle = 0;
            }

            ammoParticles[currentAmmoParticle].transform.position = pos;
            ammoParticles[currentAmmoParticle].Play();
            currentAmmoParticle++;

        }
        else
        {
            if (currentManaParticle > 1)
            {
                currentManaParticle = 0;
            }

            manaParticles[currentManaParticle].transform.position = pos;
            manaParticles[currentManaParticle].Play();
            currentManaParticle++;

        }

    }

    // Update is called once per frame
    public void GetEggCollectable(Vector2 pos, bool isShotgun, bool isThree, float speed)
    {

        if (currentEggCollectableIndex >= eggCollectableCount) currentEggCollectableIndex = 0;
        var obj = eggCollectables[currentEggCollectableIndex];
        obj.transform.position = pos;

        if (isShotgun)
        {
            if (isThree)
                obj.EnableAmmo(shotgunImage, shotgunThreeImage, isShotgun, speed);
            else
                obj.EnableAmmo(shotgunImage, null, isShotgun, speed);
        }
        else
        {
            if (isThree)
                obj.EnableAmmo(eggImage, eggThreeImage, isShotgun, speed);
            else
                obj.EnableAmmo(eggImage, null, isShotgun, speed);

        }


        currentEggCollectableIndex++;

    }

    // public void GetEggByData(RecordedDataClass data, int type)
    // {
    //     if (type)
    // }

    public void SpawnEggWithData(RecordedDataStruct data)
    {
        if (currentEggCollectableIndex >= eggCollectableCount) currentEggCollectableIndex = 0;
        var obj = eggCollectables[currentEggCollectableIndex];

    }

    public void GetBarn(int side)
    {
        if (currentBarnIndex >= 2) currentBarnIndex = 0;
        float chance = Random.Range(0f, 1f);
        // int side = 0;
        // if (chance < barnBigChance) side = 0;
        // else if (chance < barnMidChance) side = Random.Range(1, 3);
        // else side = 3;

        var script = barnScript[currentBarnIndex];

        script.transform.position = barnSpawnPos;

        script.Initilaize(side);
        currentBarnIndex++;

    }
    private IEnumerator SpawnEggs(float initialDelay)
    {
        Debug.LogError("Starting Egg ROutine");
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            checkEggs = false;

            Vector2 pos = new Vector2(13.5f, Random.Range(-2.2f, 4.2f));
            // bool three = Random.Range(0f, 1f) < eggThreeChance;

            Vector2 data = SpawnData.ReturnEggTime(player.Ammo);



            if (data == Vector2.zero)
            {
                Debug.LogError("On Max Egg Ammo, wiating for: " + SpawnData.RecheckEggIfMaxTime);
                yield return new WaitForSeconds(SpawnData.RecheckEggIfMaxTime);
            }

            else
            {
                Debug.LogError("SPawing an egg, waitng for " + data.x);
                yield return new WaitForSeconds(data.x);
                GetEggCollectable(pos, false, CheckIfEggIsThree(data.y), 0);
                yield return new WaitUntil(() => checkEggs == true);


            }


        }


    }



    private IEnumerator SpawnShotgun(float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {

            Vector2 pos = new Vector2(13.5f, Random.Range(-2.2f, 4.2f));
            // bool three = Random.Range(0f, 1f) < baseShotgunThreeChance;

            Vector2 data = SpawnData.ReturnShotgunTime(player.ShotgunAmmo);
            checkShotgun = false;

            if (data == Vector2.zero)
            {

                yield return new WaitForSeconds(SpawnData.RecheckShotgunIfMaxTime);
            }

            else
            {
                yield return new WaitForSeconds(data.x);
                GetEggCollectable(pos, true, CheckIfEggIsThree(data.y), 0);
                yield return new WaitUntil(() => checkShotgun == true);
            }

        }

    }


    private IEnumerator SpawnBarn(float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // Reset the spawn condition
            justSpawnedEnemies = false;

            // Wait until enemies have been spawned
            yield return new WaitUntil(() => justSpawnedEnemies);
            yield return new WaitForSeconds(.5f);

            // Ensure barnSpawnPos.x is at least 4.5f away from all windmill spawn positions
            bool isValidPosition = false;
            while (!isValidPosition)
            {
                isValidPosition = true; // Assume the position is valid initially

                if (windmillSpawns != null && windmillSpawns.Count > 0)
                {
                    foreach (float windmillX in windmillSpawns)
                    {
                        if (Mathf.Abs(barnSpawnPos.x - windmillX) < 4.5f)
                        {
                            isValidPosition = false; // Found a conflict
                            break;
                        }
                    }
                }

                if (!isValidPosition)
                {
                    // Wait for another second before rechecking
                    yield return new WaitForSeconds(1.7f);
                }
            }

            // Get barn spawn data and spawn the barn
            Vector2 data = SpawnData.ReturnBarnTime();
            int side = Mathf.RoundToInt(data.y);
            GetBarn(side);

            // Wait for the specified time before spawning the next barn
            yield return new WaitForSeconds(data.x);
        }
    }
    private bool CheckIfEggIsThree(float val)
    {
        if (val == 0)
            return false;
        else return true;

    }
    private void AdjustTimeAndChance(int amount)
    {
        if (amount == 0)
        {
            eggOffsetTime = -5;
            eggThreeChance = baseEggThreeChance + .3f;
            barnTimeOffset = 6f;
        }
        else if (amount < 3)
        {
            eggOffsetTime = -4;
            eggThreeChance = baseEggThreeChance + .2f;
            barnTimeOffset = 3f;

        }
        else if (amount < 6)
        {
            eggOffsetTime = 0;
            eggThreeChance = baseEggThreeChance;
            barnTimeOffset = 0;


        }
        else if (amount < 9)
        {
            eggOffsetTime = 4;
            eggThreeChance = baseEggThreeChance - .1f;
            barnTimeOffset = -2.5f;


        }
        else if (amount < 12)
        {
            eggOffsetTime = 9;
            eggThreeChance = 0;
            barnTimeOffset = -4.5f;


        }
        else if (amount < 15)
        {
            eggOffsetTime = 15;
            eggThreeChance = 0;
            barnTimeOffset = -5.5f;


        }


    }

    public void SetNewData(CollectableSpawnData data)
    {
        SpawnData = data;



        if (spawningEgg && !data.SpawnEggs)
            StopCoroutine(EggRoutine);
        else if (!spawningEgg && data.SpawnEggs)
        {
            EggRoutine = StartCoroutine(SpawnEggs(data.ReturnInitialDelay(0)));

        }

        if (spawningShotgun && !data.SpawnShotgun)
            StopCoroutine(ShotgunRoutine);
        else if (!spawningShotgun && data.SpawnShotgun)
            ShotgunRoutine = StartCoroutine(SpawnShotgun(data.ReturnInitialDelay(1)));



        if (spawningBarn && !data.SpawnBarn)
            StopCoroutine(BarnRoutine);
        else if (!spawningBarn && data.SpawnBarn)
        {
            BarnRoutine = StartCoroutine(SpawnBarn(data.ReturnInitialDelay(2)));


        }






        spawningEgg = data.SpawnEggs;
        spawningShotgun = data.SpawnShotgun;
        spawningBarn = data.SpawnBarn;

    }

    private void OnSetCheckAmmo(int type)
    {
        if (type == 0) checkEggs = true;
        else if (type == 1) checkShotgun = true;
    }

    private void OnEnable()
    {
        // player.globalEvents.OnUpdateAmmo += AdjustTimeAndChance;

        if (lvlID != null)
            lvlID.outputEvent.SetNewCollectableSpawnData += SetNewData;

        player.globalEvents.OnAmmoEvent += OnSetCheckAmmo;

    }

    private void OnDisable()
    {
        // player.globalEvents.OnUpdateAmmo -= AdjustTimeAndChance;

        if (lvlID != null)
            lvlID.outputEvent.SetNewCollectableSpawnData -= SetNewData;
        player.globalEvents.OnAmmoEvent -= OnSetCheckAmmo;

    }
}
