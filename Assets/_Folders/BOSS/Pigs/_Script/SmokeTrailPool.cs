using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrailPool : MonoBehaviour
{

    private SmokeTrailLineNew[] jetPackSmokeTrails;
    private SmokeTrailLineNew[] gasSmokeTrails = new SmokeTrailLineNew[2];


    [SerializeField] private GameObject jetPackSmokeTrailPrefab;
    [SerializeField] private GameObject gasSmokeTrailPrefab;
    [SerializeField] private GameObject gasParticleSystemPrefab;
    [SerializeField] private GameObject playerParticleColliderPrefab;
    [SerializeField] private GameObject eggParticleColliderPrefab;

    private GasCloudParticleCollider particleCollider;
    private GasCloudParticleCollider[] eggParticleColliders = new GasCloudParticleCollider[2];



    private ParticleSystem[] gasParticleSystems = new ParticleSystem[2];



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
    private int currentGasSmokeIndex = 0;
    private int currentEggParticleColliderIndex = 0;


    private int gasPigFlyingPoolSize = 0;
    // Start is called before the first frame update

    public void SetGasPigFlyingPoolSize(int s)
    {
        gasPigFlyingPoolSize = s;
    }
    void Start()
    {
        jetPackSmokeTrails = new SmokeTrailLineNew[10];




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
                var obj = Instantiate(eggParticleColliderPrefab);
                eggParticleColliders[i] = obj.GetComponent<GasCloudParticleCollider>();
                obj.SetActive(false);
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




        // for (int i = 0; i < 10; i++)
        // {
        //     var obj = Instantiate(particleSystemPrefab);

        //     particleSystems[i] = obj.GetComponent<ParticleSystem>();
        //     obj.SetActive(false);

        // }


    }

    private void Update()
    {



    }

    private void GetPlayerParticleColldierFunc(Vector2 pos, Vector2 vel)
    {
        particleCollider.Initialize(pos, vel);
    }

    private void GetEggParticleColldierFunc(Vector2 pos, Vector2 vel)
    {
        eggParticleColliders[currentEggParticleColliderIndex].Initialize(pos, vel);
        currentEggParticleColliderIndex++;
        if (currentEggParticleColliderIndex >= 2)
            currentEggParticleColliderIndex = 0;
    }
    private void GetGasSmokeTrailFunc(Vector2 pos, float speed, float delay, int id)
    {

        gasSmokeTrails[id].Initialize(pos, speed);

        gasParticleSystems[id].transform.position = new Vector2(pos.x, pos.y);
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

    private IEnumerator GasCloudCourintine(int id, float delay)
    {
        yield return new WaitForSeconds(delay);
        gasParticleSystems[id].Stop();
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




    }

    private void OnDisable()
    {
        SmokeTrailPool.GetJetpackSmokeTrail -= GetJetPackSmoke;
        SmokeTrailPool.OnDisableSmokeTrail -= DisableSmokeTrail;
        SmokeTrailPool.GetGasSmokeTrail -= GetGasSmokeTrailFunc;
        SmokeTrailPool.OnDisableGasTrail -= DisableGasTrail;
        SmokeTrailPool.GetPlayerParticleCollider -= GetPlayerParticleColldierFunc;
        SmokeTrailPool.GetEggParticleCollider -= GetEggParticleColldierFunc;




    }



    // Update is called once per frame
    // void Update()
    // {
    //     textureOffset.x -= smokeTrailOffsetSpeed * Time.deltaTime;
    //     jetPackSmokeMaterial.SetTextureOffset("_MainTex", textureOffset);

    // }
}
