using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpawnerActive : MonoBehaviour
{
    [SerializeField] private GameObject spawner;
    // Start is called before the first frame update
   public void SetSpawnActive()
   {
        spawner.SetActive(true);
    }
}
