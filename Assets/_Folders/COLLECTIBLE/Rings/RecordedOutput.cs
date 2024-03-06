using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordedOutput : MonoBehaviour
{
    private void OnEnable() {
        Debug.Log("ehat");
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeInHierarchy)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
