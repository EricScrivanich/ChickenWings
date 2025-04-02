using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserCreatedLevels
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeReference] public List<string> UserCreatedLevelNames = new List<string>();
}
