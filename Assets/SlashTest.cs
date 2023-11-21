using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashTest : MonoBehaviour
{
    [SerializeField] private GameObject slash;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(test());
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator test()
    {
        slash.SetActive(false);
        Debug.Log("start");
        yield return new WaitForSeconds(2.5f);
        slash.SetActive(true);
        yield return new WaitForSeconds(.5f);
        StartCoroutine(test());


    }
}
