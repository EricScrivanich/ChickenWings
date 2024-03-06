using System.Collections.Generic;
using UnityEngine;

public class PlaneManager : MonoBehaviour
{
    public enum PlaneType
    {
        Jet,
        Cargo,
        Crop
    }

    [SerializeField] private GameObject jetPrefab;
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private GameObject cropPrefab;
    private Dictionary<PlaneType, List<GameObject>> objectPools;

    private const int objectPoolSize = 10;
    [SerializeField] private float spawnThreshold = 10f;
    private float speed = -5f;

    private void Start()
    {
        objectPools = new Dictionary<PlaneType, List<GameObject>>
        {
            { PlaneType.Jet, new List<GameObject>() },
            { PlaneType.Cargo, new List<GameObject>() },
            { PlaneType.Crop, new List<GameObject>() }
        };

        InitializeObjectPool(jetPrefab, PlaneType.Jet);
        InitializeObjectPool(cargoPrefab, PlaneType.Cargo);
        InitializeObjectPool(cropPrefab, PlaneType.Crop);
    }

    private void InitializeObjectPool(GameObject prefab, PlaneType type)
    {
        for (int i = 0; i < objectPoolSize; i++)
        {
            GameObject plane = Instantiate(prefab);
            plane.SetActive(false);
            objectPools[type].Add(plane);
            plane.transform.SetParent(transform);
        }
    }

    private void Update()
    {
        List<Transform> placeholdersToRemove = new List<Transform>();
        foreach (Transform parent in transform)
        {
            foreach (Transform child in parent)
            {
                if(child == null)
                {
                    Debug.LogError("A filler is null!");
                    continue;
                }

                PlanePlaceholder placeholder = child.GetComponent<PlanePlaceholder>();
                if(placeholder == null)
                {
                    Debug.LogError("A Placeholder component is missing on a filler!");
                    continue;
                }

                PlaneType type = placeholder.GetPlaneType();

                if (child.position.x <= spawnThreshold)
                {
                    SpawnPlane(type, child.position);
                    placeholdersToRemove.Add(child);
                }
                else
                {
                    child.position += Vector3.right * speed * Time.deltaTime;
                }
            }
        }

        foreach(Transform child in placeholdersToRemove)
        {
            Destroy(child.gameObject);
        }
    }

    private void SpawnPlane(PlaneType type, Vector3 position)
    {
        GameObject plane = GetObjectFromPool(type);
        if (plane != null)
        {
            plane.transform.position = position;
            plane.SetActive(true);
        }
    }

    private GameObject GetObjectFromPool(PlaneType type)
    {
        foreach (GameObject obj in objectPools[type])
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        return null;
    }

    public void ResetPlane(GameObject plane, PlaneType type)
    {
        plane.SetActive(false);
        objectPools[type].Add(plane);
    }

    
}
