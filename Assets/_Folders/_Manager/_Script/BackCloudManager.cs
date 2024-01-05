using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackCloudManager : MonoBehaviour
{
    // Start is called before the first frame update
   [SerializeField]
    private GameObject[] cloudPrefabs; // Drag your 6 cloud prefabs here
    [SerializeField]
    private float speed = 1.0f; // The speed at which the clouds will move

    private GameObject[] clouds;
    private float cloudWidth;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        InitializeClouds();
    }

    private void Update()
    {
        if (clouds == null || clouds[0] == null)
        {
            InitializeClouds(); // Recreate clouds if they are destroyed
        }

        float screenRightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        float screenLeftBound = Camera.main.ScreenToWorldPoint(Vector3.zero).x - cloudWidth;

        for (int i = 0; i < clouds.Length; i++)
        {
            Vector3 newPosition = clouds[i].transform.position - new Vector3(speed * Time.deltaTime, 0, 0);
            clouds[i].transform.position = newPosition;
        }

        if (clouds[clouds.Length - 1].transform.position.x <= screenRightBound)
        {
            Vector3 newPosition = new Vector3(clouds[clouds.Length - 1].transform.position.x + cloudWidth, 5, 0);
            clouds[0].transform.position = newPosition;

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
        float screenLeftBound = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        float screenRightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        int numberOfClouds = Mathf.CeilToInt((screenRightBound - screenLeftBound) / cloudWidth) + 1; // Additional cloud for the rightmost

        clouds = new GameObject[numberOfClouds];
        Vector3 position = new Vector3(screenLeftBound, 5, 0);

        for (int i = 0; i < numberOfClouds; i++)
        {
            int randomCloudIndex = Random.Range(0, cloudPrefabs.Length);
            clouds[i] = Instantiate(cloudPrefabs[randomCloudIndex], position, Quaternion.identity);
            DontDestroyOnLoad(clouds[i]); // Add this line
            position.x += cloudWidth;
        }
    }
}

