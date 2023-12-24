using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{    
    [SerializeField]
    private GameObject[] cloudPrefabs;
    [SerializeField]
    private float speed = 1.0f;
    
    private Transform rectangleTransform; // Drag your rectangle object here

    private GameObject[] clouds;
    private float cloudWidth;

    private void Start()
    {
        rectangleTransform = GetComponent<Transform>();
        DontDestroyOnLoad(gameObject);
        InitializeClouds();
    }

    private void Update()
    {
        if (clouds == null || clouds[0] == null)
        {
            InitializeClouds();
        }

        for (int i = 0; i < clouds.Length; i++)
        {
            Vector3 newPosition = clouds[i].transform.localPosition - new Vector3(speed * Time.deltaTime, 0, 0);
            clouds[i].transform.localPosition = newPosition;
        }

       if (clouds[0].transform.localPosition.x <= -cloudWidth)
    {
        Vector3 newPosition = new Vector3(clouds[clouds.Length - 1].transform.localPosition.x + cloudWidth, 0, 0);
        clouds[0].transform.localPosition = newPosition;

        GameObject temp = clouds[0];
        for (int i = 0; i < clouds.Length - 1; i++)
        {
            clouds[i] = clouds[i + 1];
        }
        clouds[clouds.Length - 1] = temp;
    }
    }

    private void InitializeClouds()
{
    cloudWidth = cloudPrefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
    float rectLength = GetComponent<SpriteRenderer>().bounds.size.x;
    int numberOfClouds = Mathf.CeilToInt(rectLength / cloudWidth) + 2; // Extra clouds for smooth looping

    clouds = new GameObject[numberOfClouds];
    Vector3 position = Vector3.zero;

    for (int i = 0; i < numberOfClouds; i++)
    {
        int randomCloudIndex = Random.Range(0, cloudPrefabs.Length);
        clouds[i] = Instantiate(cloudPrefabs[randomCloudIndex], rectangleTransform);
        clouds[i].transform.localPosition = position;
        DontDestroyOnLoad(clouds[i]);
        position.x += cloudWidth;
    }
}

}