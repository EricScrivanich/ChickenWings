using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneAreaSpawn : MonoBehaviour
{
    [SerializeField] private BoxCollider2D spawnArea;
    public int getsTriggeredInt;
    public int maxPlanes;
    public int minPlanes;

    public float cropChance;
    public float jetChance;
    public float cargoChance;


    public float GetMinX() => spawnArea.bounds.min.x;
    public float GetMaxX() => spawnArea.bounds.max.x;

    // Function to get the minimum and maximum Y values
    public float GetMinY() => spawnArea.bounds.min.y;
    public float GetMaxY() => spawnArea.bounds.max.y;
    // Start is called before the first frame update
  

}

