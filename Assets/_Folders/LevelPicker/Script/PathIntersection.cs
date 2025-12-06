using UnityEngine;
using System.Collections.Generic;

public class PathIntersection : MonoBehaviour
{

    [field: SerializeField] public int[] connectedPathsIndices { get; private set; }
    [field: SerializeField] public int[] connectedPathsDistances { get; private set; } //-1 for postion of connecttion, 0 for start, 1 for end 

    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }




}
