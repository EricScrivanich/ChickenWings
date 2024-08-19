using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCustomSpawnBoundary : MonoBehaviour
{
    private SpawnStateManager manager;
    [SerializeField] private Vector2[] boxDataXRange;

    [SerializeField] private Vector2[] boxDataYRange;

    [SerializeField] private int StartWithBox = -1;
    [SerializeField] private float startWithBoxDelay;
    // Start is called before the first frame update
    void Start()
    {
        manager = gameObject.GetComponent<SpawnStateManager>();
        if (manager == null)
        {
            Debug.LogError("SpawnStateManager component not found on this GameObject!");
        }


        if (StartWithBox > -1)
        {
            Invoke("AddBoudingBoxAtStart", startWithBoxDelay);
        }

    }

    public void AddBoudingBoxAtStart()
    {
        bool allX = false;
        if (boxDataXRange[StartWithBox].x == 0 && boxDataXRange[StartWithBox].y == 0)
            allX = true;

        if (!allX)
            manager.CreateCustomBoundingBox(boxDataYRange[StartWithBox], boxDataXRange[StartWithBox]);
        else
            manager.CreateCustomBoundingBox(boxDataYRange[StartWithBox], Vector2.zero);

    }

    public void AddBoudingBox(int index)
    {
        bool allX = false;
        if (boxDataXRange[index].x == 0 && boxDataXRange[index].y == 0)
            allX = true;

        if (!allX)
            manager.CreateCustomBoundingBox(boxDataYRange[index], boxDataXRange[index]);
        else
            manager.CreateCustomBoundingBox(boxDataYRange[index], Vector2.zero);

    }

    public void RemoveBoundingBox()
    {
        manager.RemoveCustomBoundingBox();
    }

    // Update is called once per frame

}
