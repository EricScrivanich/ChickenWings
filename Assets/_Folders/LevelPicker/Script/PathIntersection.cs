using UnityEngine;
using System.Collections.Generic;

public class PathIntersection : MonoBehaviour
{

    [SerializeField] private List<int> connectedPathsIndices;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public bool CheckIfConnected(int pathIndex)
    {
        return connectedPathsIndices.Contains(pathIndex);
    }

    // Update is called once per frame
   
}
