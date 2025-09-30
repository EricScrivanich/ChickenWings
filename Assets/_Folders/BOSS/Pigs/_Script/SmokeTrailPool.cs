using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrailPool : MonoBehaviour
{
    [SerializeField] private float eggColliderYOffsetMultiplier;
    [SerializeField] private PlayerID player;
    [SerializeField] private int gasCloudPoolSize;
    private SmokeTrailLineNew[] jetPackSmokeTrails;
    private SmokeTrailLineNew[] gasSmokeTrails;

    // private GasCloud[] gasClouds;

    private GameObject[] gasCloudHitParticles = new GameObject[3];
    private Vector2 eggFieldOffset = new Vector2(.15f, .7f);




    [SerializeField] private GameObject jetPackSmokeTrailPrefab;
    [SerializeField] private GameObject gasSmokeTrailPrefab;
    [SerializeField] private GameObject gasParticleSystemPrefab;
    [SerializeField] private GameObject gasCloudPrefab;
    [SerializeField] private GameObject playerParticleColliderPrefab;
    [SerializeField] private GameObject eggParticleColliderCoverPrefab;
    [SerializeField] private GameObject eggParticleColliderPrefab;
    [SerializeField] private GameObject gasCloudHitParticlesPrefab;

    private GasCloudParticleCollider particleCollider;
    private GasCloudParticleCollider[] eggParticleColliders = new GasCloudParticleCollider[2];
    private GameObject[] eggParticleColliderCovers = new GameObject[2];



    private ParticleSystem[] gasParticleSystems;



    public static Action<Vector2, float, int> GetJetpackSmokeTrail;
   
    public static Action<Vector2, Vector2> GetPlayerParticleCollider;
    public static Action<Vector2, Vector2> GetEggParticleCollider;
    public static Action<Vector2, float, float, int> GetGasSmokeTrail;
    public static Action<int> OnDisableSmokeTrail;
    public static Action<int> OnDisableGasTrail;

    private int[] idList = new int[10];
    private float[] speedList = new float[10];

    private int particlesInUse = 0;


    private int currentSmokeIndex = 0;
    private int currentGasCloudIndex = 0;
    private int currentGasCloudHitParticlesIndex = 0;
    private int currentGasSmokeIndex = 0;
    private int currentEggParticleColliderIndex = 0;


    private int gasPigFlyingPoolSize = 0;
    // Start is called before the first frame update

    public void SetGasPigFlyingPoolSize(int s)
    {
        Debug.LogError("Gas Pig Flying Pool Size is " + s);
        gasPigFlyingPoolSize = s;
        gasSmokeTrails = new SmokeTrailLineNew[s];
        gasParticleSystems = new ParticleSystem[s];
    }
    void Start()
    {
        jetPackSmokeTrails = new SmokeTrailLineNew[10];
        // gasClouds = new GasCloud[gasCloudPoolSize];


        // for (int i = 0; i < gasCloudPoolSize; i++)
        // {
        //     var obj = Instantiate(gasCloudPrefab);

        //     gasClouds[i] = obj.GetComponent<GasCloud>();
        //     obj.SetActive(false);

        // }

        if (gasCloudPoolSize > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                var obj = Instantiate(gasCloudHitParticlesPrefab);

                gasCloudHitParticles[i] = obj;
                obj.SetActive(false);

            }

        }

        for (int i = 0; i < 10; i++)
        {
            var obj = Instantiate(jetPackSmokeTrailPrefab);

            jetPackSmokeTrails[i] = obj.GetComponent<SmokeTrailLineNew>();
            obj.SetActive(false);

        }

        if (gasPigFlyingPoolSize > 0)
        {
            particleCollider = Instantiate(playerParticleColliderPrefab).GetComponent<GasCloudParticleCollider>();
            particleCollider.gameObject.SetActive(false);

            for (int i = 0; i < 2; i++)
            {
                // var obj = Instantiate(eggParticleColliderPrefab);
                // eggParticleColliders[i] = obj.GetComponent<GasCloudParticleCollider>();
                // obj.SetActive(false);
                eggParticleColliderCovers[i] = Instantiate(eggParticleColliderCoverPrefab);
                eggParticleColliderCovers[i].SetActive(false);

            }
            for (int i = 0; i < gasPigFlyingPoolSize; i++)
            {
                var obj = Instantiate(gasSmokeTrailPrefab);
                gasSmokeTrails[i] = obj.GetComponent<SmokeTrailLineNew>();
                obj.SetActive(false);

            }
            for (int i = 0; i < gasPigFlyingPoolSize; i++)

            {
                var obj = Instantiate(gasParticleSystemPrefab);

                gasParticleSystems[i] = obj.GetComponent<ParticleSystem>();
                obj.SetActive(false);

            }

        }
    }

    // private void GetGasCloudFunc(Vector2 pos, float speed, bool flipped)
    // {
    //     var obj = gasClouds[currentGasCloudIndex];
    //     obj.transform.position = pos;
    //     obj.Eject(speed, flipped);
    //     currentGasCloudIndex++;
    //     if (currentGasCloudIndex >= gasCloudPoolSize) currentGasCloudIndex = 0;

    // }
   

    private void GetPlayerParticleColldierFunc(Vector2 pos, Vector2 vel)
    {
        particleCollider.Initialize(pos, vel);
    }

    private void GetEggParticleColldierFunc(Vector2 pos, Vector2 vel)
    {
        // eggParticleColliders[currentEggParticleColliderIndex].Initialize(pos - vel.normalized * 1.5f, vel);
        eggParticleColliderCovers[currentEggParticleColliderIndex].transform.position = pos + eggFieldOffset;
        eggParticleColliderCovers[currentEggParticleColliderIndex].SetActive(true);

        currentEggParticleColliderIndex++;
        if (currentEggParticleColliderIndex >= 2)
            currentEggParticleColliderIndex = 0;
    }
    private void GetGasSmokeTrailFunc(Vector2 pos, float speed, float delay, int id)
    {

        gasSmokeTrails[id].Initialize(pos, speed);
        if (pos.x < BoundariesManager.rightViewBoundary + 1.2f) pos.x = BoundariesManager.rightViewBoundary + 1.2f;

        gasParticleSystems[id].transform.position = new Vector2(pos.x, pos.y);
        player.NewGasParticles(pos.y, true);
        // Debug.LogError("Normal Postion is " + pos);

        // Debug.LogError("Gas Cloud Position is" + gasParticleSystems[id].transform.position);
        gasParticleSystems[id].gameObject.SetActive(false);
        gasParticleSystems[id].gameObject.SetActive(true);
        gasParticleSystems[id].Play();
        StartCoroutine(GasCloudCourintine(id, delay));

    }




    private void GetJetPackSmoke(Vector2 pos, float speed, int id)
    {
        jetPackSmokeTrails[currentSmokeIndex].Initialize(pos, speed);

        idList[currentSmokeIndex] = id;

        currentSmokeIndex++;


        if (currentSmokeIndex > 9)
            currentSmokeIndex = 0;



        // particleSystems[currentSmokeIndex].transform.position = pos;
        // speedList[currentSmokeIndex] = speed;

        // idList[currentSmokeIndex] = id;
        // particlesInUse++;

        // currentSmokeIndex++;


        // if (currentSmokeIndex > 9)
        //     currentSmokeIndex = 0;

    }
    private WaitForSeconds delay = new WaitForSeconds(13);
    private IEnumerator GasCloudCourintine(int id, float delay)
    {
        yield return new WaitForSeconds(delay);
        gasParticleSystems[id].Stop();
        yield return delay;
        player.NewGasParticles(gasParticleSystems[id].transform.position.y, false);

    }
    private void DisableGasTrail(int id)
    {
        gasSmokeTrails[id].FadeOut();

    }

    private void DisableSmokeTrail(int id)
    {
        for (int i = 0; i < 10; i++)
        {
            if (idList[i] == id)
            {
                jetPackSmokeTrails[i].FadeOut();
            }
            // jetPackSmokeTrails[id].gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        SmokeTrailPool.GetJetpackSmokeTrail += GetJetPackSmoke;
        SmokeTrailPool.OnDisableSmokeTrail += DisableSmokeTrail;

        SmokeTrailPool.GetGasSmokeTrail += GetGasSmokeTrailFunc;
        SmokeTrailPool.OnDisableGasTrail += DisableGasTrail;
        SmokeTrailPool.GetPlayerParticleCollider += GetPlayerParticleColldierFunc;
        SmokeTrailPool.GetEggParticleCollider += GetEggParticleColldierFunc;
        // SmokeTrailPool.GetGasCloud += GetGasCloudFunc;
        




    }

    private void OnDisable()
    {
        SmokeTrailPool.GetJetpackSmokeTrail -= GetJetPackSmoke;
        SmokeTrailPool.OnDisableSmokeTrail -= DisableSmokeTrail;
        SmokeTrailPool.GetGasSmokeTrail -= GetGasSmokeTrailFunc;
        SmokeTrailPool.OnDisableGasTrail -= DisableGasTrail;
        SmokeTrailPool.GetPlayerParticleCollider -= GetPlayerParticleColldierFunc;
        SmokeTrailPool.GetEggParticleCollider -= GetEggParticleColldierFunc;
        // SmokeTrailPool.GetGasCloud -= GetGasCloudFunc;
      






    }



    // Update is called once per frame
    // void Update()
    // {
    //     textureOffset.x -= smokeTrailOffsetSpeed * Time.deltaTime;
    //     jetPackSmokeMaterial.SetTextureOffset("_MainTex", textureOffset);

    // }
}
