using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/AmmoData")]
public class PlayerStartingStatsForLevels : ScriptableObject
{
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private GameObject shotgunPrefab;
    [SerializeField] private bool startHidden = false;

    [SerializeField] private ShotgunParticleID shotgunParticleData;
    [SerializeField] private QPool shotgunParticlePool;

    private Egg_Regular[] eggPool;

    [SerializeField] private QPool eggPoolNew;
    private ShotgunBlast[] shotgunPool;

    public short[] startingAmmos;
    public short StartingLives;

    public int numberOfWeapons { get; private set; }

    [SerializeField] private int eggPoolSize;
    [SerializeField] private int shotgunPoolSize;

    private int currentEggIndex;
    private int currentShotgunIndex;
    private int eggID;
    private int shotgunID;

    public int startingNormalEggAmmo;
    public int startingShotgunAmmo;

    public int[] AvailableAmmos;
    public int[] ChangedAvailableAmmos;

    private int startingAmmo = -1;
    public void SetData(short startingLives, short[] ammos, int shownAmmo, int maxLive = -1)
    {
        startingAmmos = ammos;

        this.StartingLives = startingLives;
        startingAmmo = shownAmmo;
        // shotgunParticleData.SetTransformData();
        Debug.Log("Starting Lives in stat ID: " + StartingLives);



    }

    public void Initialize()
    {
        numberOfWeapons = 0;


        currentEggIndex = 0;
        currentShotgunIndex = 0;
        eggID = 0;
        shotgunID = 0;
        if (startingAmmos != null && startingAmmos.Length > 0)
        {
            startingNormalEggAmmo = startingAmmos[0];
            startingShotgunAmmo = startingAmmos[1];
        }


        // eggPool = new Egg_Regular[eggPoolSize];
        // for (int i = 0; i < eggPoolSize; i++)
        // {
        //     var obj = Instantiate(eggPrefab).GetComponent<Egg_Regular>();
        //     eggPool[i] = obj;
        //     obj.gameObject.SetActive(false);

        // }

        // shotgunPool = new ShotgunBlast[shotgunPoolSize];
        // for (int i = 0; i < shotgunPoolSize; i++)
        // {
        //     var obj = Instantiate(shotgunPrefab).GetComponent<ShotgunBlast>();
        //     shotgunPool[i] = obj;
        //     obj.gameObject.SetActive(false);

        // }

        if (eggPoolSize > 0) numberOfWeapons++;
        if (shotgunPoolSize > 0) numberOfWeapons++;
    }

    public int ReturnStartingEquipedAmmo()
    {
        if (startingAmmo > -1) return startingAmmo;

        if ((startingNormalEggAmmo <= 0 && startingShotgunAmmo <= 0) || startHidden) return 0;
        else if (startingNormalEggAmmo > startingShotgunAmmo) return 0;
        else if (startingShotgunAmmo >= startingNormalEggAmmo) return 1;
        else return 0;
    }
    // public int ReturnNextAva

    public void GetShotgunBlast(Vector2 pos, float rot, bool chained)
    {
        shotgunParticlePool.SpawnTransformOverride(pos, rot, shotgunID, chained);


        // if (currentShotgunIndex >= shotgunPoolSize) currentShotgunIndex = 0;
        // if (shotgunPool[currentShotgunIndex].gameObject.activeInHierarchy)
        // {
        //     var s = Instantiate(shotgunPrefab, pos, rotation).GetComponent<ShotgunBlast>();
        //     // s.transform.position = pos;
        //     // s.transform.rotation = rotation;
        //     s.Initialize(chained, shotgunID);


        // }
        // else
        // {
        //     var s = shotgunPool[currentShotgunIndex];
        //     s.transform.position = pos;
        //     s.transform.rotation = rotation;
        //     s.Initialize(chained, shotgunID);
        //     s.gameObject.SetActive(true);
        //     currentShotgunIndex++;
        // }

        shotgunID++;


    }

    public void GetEgg(Vector2 pos, Vector2 force)
    {
        // if (currentEggIndex >= eggPoolSize) currentEggIndex = 0;

        // if (eggPool[currentEggIndex].gameObject.activeInHierarchy)
        // {
        //     var s = Instantiate(eggPrefab, pos, Quaternion.identity).GetComponent<Egg_Regular>();
        //     s.Initialize(force);

        // }
        // else
        // {
        //     var s = eggPool[currentEggIndex];
        //     s.transform.position = pos;
        //     s.gameObject.SetActive(true);
        //     s.Initialize(force); 

        //     currentEggIndex++;
        // }
        // Vector2 n = force.normalized;

        // eggPoolNew.SpawnWithVelocityAndRotation(pos, force, Mathf.Atan2(n.x, -n.y) * Mathf.Rad2Deg * .5f);
        eggPoolNew.SpawnWithVelocity(pos, force);



    }


}
