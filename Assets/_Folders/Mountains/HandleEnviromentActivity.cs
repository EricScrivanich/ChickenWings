using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class HandleEnviromentActivity : MonoBehaviour
{

    [SerializeField] private GameObject[] activate;
    [SerializeField] private GameObject[] deActivate;
    // Start is called before the first frame update
    public void TriggerAction()
    {
        foreach(GameObject obj in activate)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in deActivate)
        {
            obj.SetActive(false);
        }
    }
}
