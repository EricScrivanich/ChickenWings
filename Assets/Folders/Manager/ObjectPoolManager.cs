using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance; // Static instance for easy access
    
    [SerializeField] private GameObject normalEggPrefab;
    [SerializeField] private GameObject yolkPrefab;
    [SerializeField] private int eggPoolAmount;
    [SerializeField] private int yolkPoolAmount;

    private static List<GameObject> eggPool;
    private static List<GameObject> yolkPool;

    private void Awake()
    {
        // Singleton pattern to ensure there's only one ObjectPoolManager
     Instance = this;
    //  DontDestroyOnLoad(Instance);
    }

    // Use this for initialization
    void Start()
    {
        eggPool = new List<GameObject>();
        yolkPool = new List<GameObject>();
        GameObject egg;
        GameObject yolk;
       
        // Initialize pools, for example, create 10 egg and yolk objects initially
        for (int i = 0; i < eggPoolAmount; i++)
        {
            egg = Instantiate(normalEggPrefab);
            egg.SetActive(false);
            eggPool.Add(egg);
        }
         for (int i = 0; i < yolkPoolAmount; i++)
         {
            yolk = Instantiate(yolkPrefab);
            yolk.SetActive(false);
            yolkPool.Add(yolk);
         }
        
    }

   public GameObject GetEgg()
{
    // Check if eggPool has been initialized
    if (eggPool == null)
    {
        Debug.LogError("eggPool is null. Initialization might have failed or not yet occurred.");
        return null;
    }

    for (int i = 0; i < eggPoolAmount; i++)
    {
        // Check if the egg object is null
        if (eggPool[i] == null)
        {
            Debug.LogError("An object in eggPool is null.");
            continue;
        }

        if (!eggPool[i].activeInHierarchy)
        {
            return eggPool[i];
        }
    }

    Debug.LogError("No available eggs in the pool.");
    return null;
}

    public GameObject GetYolk()
    {
       for(int i = 0; i < yolkPoolAmount; i++)
    {
        if(!yolkPool[i].activeInHierarchy)
        {
            return yolkPool[i];
        }
    }
    return null;
    }

    public void ResetPools()
{
    for (int i = 0; i < eggPool.Count; i++)
    {
        eggPool[i].SetActive(false);
    }

    for (int i = 0; i < yolkPool.Count; i++)
    {
        yolkPool[i].SetActive(false);
    }
}

   private void OnEnable()
{
    ResetManager.onRestartGame += ResetPools;
}

private void OnDisable()
{
    ResetManager.onRestartGame -= ResetPools;
}
}
