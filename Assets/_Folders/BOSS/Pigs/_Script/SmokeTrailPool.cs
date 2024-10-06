using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrailPool : MonoBehaviour
{

    private SmokeTrailLineNew[] jetPackSmokeTrails;
    [SerializeField] private GameObject jetPackSmokeTrailPrefab;
    [SerializeField] private Material jetPackSmokeMaterial;
    private Vector2 textureOffset;

    [SerializeField] private float smokeTrailOffsetSpeed;

    public static Action<Vector2, float> GetJetpackSmokeTrail;

    private int currentSmokeIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        jetPackSmokeTrails = new SmokeTrailLineNew[10];

        for (int i = 0; i < 10; i++)
        {
            var obj = Instantiate(jetPackSmokeTrailPrefab);

            jetPackSmokeTrails[i] = obj.GetComponent<SmokeTrailLineNew>();
            obj.SetActive(false);

        }


    }
    private void GetJetPackSmoke(Vector2 pos, float speed)
    {
        jetPackSmokeTrails[currentSmokeIndex].Initialize(pos, speed);

        currentSmokeIndex++;

        if (currentSmokeIndex > 9)
            currentSmokeIndex = 0;

    }

    private void OnEnable()
    {

        SmokeTrailPool.GetJetpackSmokeTrail += GetJetPackSmoke;

    }

    private void OnDisable()
    {
        SmokeTrailPool.GetJetpackSmokeTrail -= GetJetPackSmoke;


    }



    // Update is called once per frame
    void Update()
    {
        textureOffset.x -= smokeTrailOffsetSpeed * Time.deltaTime;
        jetPackSmokeMaterial.SetTextureOffset("_MainTex", textureOffset);

    }
}
