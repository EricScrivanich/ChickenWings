using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsManager : MonoBehaviour
{
    public static CloudsManager Instance;
    [SerializeField] private List<GameObject> Prefabs;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        

        foreach(var prefab in Prefabs)
        {
            if (!prefab.activeInHierarchy)
            {
                prefab.SetActive(true);
            }
        }
    }
}
