using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/AmmoData")]
public class PlayerStartingStatsForLevels : ScriptableObject
{
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private GameObject shotgunPrefab;

    private Egg_Regular[] eggPool;
    private ShotgunBlast[] shotgunPool;

    [SerializeField] private int eggPoolSize;
    [SerializeField] private int shotgunPoolSize;

    private int currentEggIndex;
    private int currentShotgunIndex;
    private int eggID;
    private int shotgunID;

    public int startingNormalEggAmmo;
    public int startingShotgunAmmo;

    public void Initialize()
    {
        currentEggIndex = 0;
        currentShotgunIndex = 0;
        eggID = 0;
        shotgunID = 0;

        eggPool = new Egg_Regular[eggPoolSize];
        for (int i = 0; i < eggPoolSize; i++)
        {
            var obj = Instantiate(eggPrefab).GetComponent<Egg_Regular>();
            eggPool[i] = obj;
            obj.gameObject.SetActive(false);

        }

        shotgunPool = new ShotgunBlast[shotgunPoolSize];
        for (int i = 0; i < shotgunPoolSize; i++)
        {
            var obj = Instantiate(shotgunPrefab).GetComponent<ShotgunBlast>();
            shotgunPool[i] = obj;
            obj.gameObject.SetActive(false);

        }
    }

    public void GetShotgunBlast(Vector2 pos, Quaternion rotation, bool chained)
    {
        if (currentShotgunIndex >= shotgunPoolSize) currentShotgunIndex = 0;
        if (shotgunPool[currentShotgunIndex].gameObject.activeInHierarchy)
        {
            var s = Instantiate(shotgunPrefab).GetComponent<ShotgunBlast>();
            s.transform.position = pos;
            s.transform.rotation = rotation;
            s.Initialize(chained, shotgunID);
            s.gameObject.SetActive(true);

        }
        else
        {
            var s = shotgunPool[currentShotgunIndex];
            s.transform.position = pos;
            s.transform.rotation = rotation;
            s.Initialize(chained, shotgunID);
            s.gameObject.SetActive(true);
            currentShotgunIndex++;
        }

        shotgunID++;


    }

    public void GetEgg()
    {

    }


}
