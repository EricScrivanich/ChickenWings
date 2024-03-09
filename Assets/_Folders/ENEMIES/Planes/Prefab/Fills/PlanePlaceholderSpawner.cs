using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePlaceholderSpawner : MonoBehaviour
{
    public PlaneData CropID;
    public PlaneData CargoID;
    public PlaneData JetID;
    [SerializeField] private List<GameObject> SpawnOptions;
    [SerializeField] private bool isRandom;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ChooseRandomSetup", 1f, 5f);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void ChooseRandomSetup()
    {
        int index;
        if (isRandom)
        {
            index = Random.Range(0, SpawnOptions.Count);

        }
        else
        {
            index = SpawnOptions.Count - 1;

        }

        var element = SpawnOptions[index];
        element.SetActive(true);
        SpawnPlanes(element.transform);

    }

    void SpawnPlanes(Transform prefab)
    {
        //     foreach (Transform child in prefab)
        //     {

        //             var script = child.gameObject.GetComponent<PlanePlaceholder>();
        //             script.SpawnPlane();
        //         prefab.gameObject.SetActive(false);



        //     }
        // }
    }
}
