using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnAndEggSpawner : MonoBehaviour
{
    public PlayerID player;

    [Header("Egg")]
    [SerializeField] private GameObject eggCollectablePrefab;
    [SerializeField] private int eggCollectableCount;
    private int currentEggCollectableIndex;

    [SerializeField] private Vector2 eggTimeRange;
    private float eggOffsetTime = 0;
    [SerializeField] private float baseEggThreeChance;
    private float eggThreeChance;

    [Header("Barn")]
    [SerializeField] private GameObject barnPrefab;
    [SerializeField] private Vector2 barnTimeRange;
    private float barnTimeOffset;


    [SerializeField] private float barnBigChance;
    [SerializeField] private float barnMidChance;

    private Vector2 barnSpawnPos = new Vector2(13.5f, -4.64f);
    private EggCollectableMovement[] eggCollectables;
    private int currentBarnIndex = 0;
    private BarnMovement[] barnScript = new BarnMovement[2];

    // Start is called before the first frame update
    void Start()
    {
        eggThreeChance = baseEggThreeChance;

        eggCollectables = new EggCollectableMovement[eggCollectableCount];

        for (int i = 0; i < eggCollectableCount; i++)
        {
            var obj = Instantiate(eggCollectablePrefab);
            var script = obj.GetComponent<EggCollectableMovement>();
            eggCollectables[i] = script;
            script.gameObject.SetActive(false);
        }

        for (int i = 0; i < 2; i++)
        {
            var barnObj = Instantiate(barnPrefab);
            barnScript[i] = barnObj.GetComponent<BarnMovement>();
            barnObj.SetActive(false);
        }




        StartCoroutine(SpawnEggs());
        StartCoroutine(SpawnBarn());


    }

    // Update is called once per frame
    public void GetEggCollectable(Vector2 pos, bool isThree, float speed)
    {
        if (currentEggCollectableIndex !< eggCollectableCount) currentEggCollectableIndex = 0;
        var obj = eggCollectables[currentEggCollectableIndex];
        obj.transform.position = pos;
        obj.EnableAmmo(isThree, speed);

    }

    public void GetBarn()
    {
        if (currentBarnIndex >= 2) currentBarnIndex = 0;
        float chance = Random.Range(0f, 1f);
        int side = 0;
        if (chance < barnBigChance) side = 0;
        else if (chance < barnMidChance) side = Random.Range(1, 3);
        else side = 3;

        var script = barnScript[currentBarnIndex];

        script.transform.position = barnSpawnPos;
        Debug.Log("Barn Index Should Be: " + side);
        script.Initilaize(side);
        currentBarnIndex++;

    }
    private IEnumerator SpawnEggs()
    {
        yield return new WaitForSeconds(Random.Range(2f, 6f));

        while (true)
        {

            Vector2 pos = new Vector2(13, Random.Range(-2.2f, 4.2f));
            bool three = Random.Range(0f, 1f) < eggThreeChance;

            GetEggCollectable(pos, three, 0);
            yield return new WaitForSeconds(Random.Range(eggTimeRange.x, eggTimeRange.y) + eggOffsetTime);
        }


    }
    private IEnumerator SpawnBarn()
    {
        yield return new WaitForSeconds(Random.Range(4f, 7f));
        while (true)
        {

            GetBarn();
            yield return new WaitForSeconds(Random.Range(barnTimeRange.x, barnTimeRange.y) + barnTimeOffset);
        }

    }
    private void AdjustTimeAndChance()
    {
        if (player.Ammo == 0)
        {
            eggOffsetTime = -5;
            eggThreeChance = baseEggThreeChance + .3f;
            barnTimeOffset = 6f;
        }
        else if (player.Ammo < 3)
        {
            eggOffsetTime = -4;
            eggThreeChance = baseEggThreeChance + .2f;
            barnTimeOffset = 3f;

        }
        else if (player.Ammo < 6)
        {
            eggOffsetTime = 0;
            eggThreeChance = baseEggThreeChance;
            barnTimeOffset = 0;


        }
        else if (player.Ammo < 9)
        {
            eggOffsetTime = 4;
            eggThreeChance = baseEggThreeChance - .1f;
            barnTimeOffset = -2.5f;


        }
        else if (player.Ammo < 12)
        {
            eggOffsetTime = 9;
            eggThreeChance = 0;
            barnTimeOffset = -4.5f;


        }
        else if (player.Ammo < 15)
        {
            eggOffsetTime = 15;
            eggThreeChance = 0;
            barnTimeOffset = -5.5f;


        }


    }

    private void OnEnable()
    {
        player.globalEvents.OnUpdateAmmo += AdjustTimeAndChance;
    }

    private void OnDisable()
    {
        player.globalEvents.OnUpdateAmmo -= AdjustTimeAndChance;
    }
}
