using UnityEngine;

public class TypeOverride : MonoBehaviour
{
    [SerializeField] private GameObject[] Prefabs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 private void OverridePrefab(int type)
 {

 }

 private void Awake() {
        var s = GetComponent<RecordableObjectPlacer>();

        

    }
}
