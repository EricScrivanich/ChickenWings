using UnityEngine;

public class SideCloudManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] cloudPrefabs; // Drag your cloud prefabs here
    [SerializeField]
    private float speed = 1.0f; // The speed at which the clouds will move
    [SerializeField]
    private float spawnYPoint = 10.2f; // The Y position to spawn clouds
    [SerializeField]
    private Vector3 rotation = new Vector3(0, 0, 90); // The rotation of the clouds

    private GameObject[] clouds;
    private float cloudHeight;

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

        float screenTopBound = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        float screenBottomBound = Camera.main.ScreenToWorldPoint(Vector3.zero).y + cloudHeight;

        for (int i = 0; i < clouds.Length; i++)
        {
            Vector3 newPosition = clouds[i].transform.position + new Vector3(0, speed * Time.deltaTime, 0);
            clouds[i].transform.position = newPosition;
        }

        if (clouds[0].transform.position.y >= screenTopBound + cloudHeight)
        {
            Vector3 newPosition = new Vector3(spawnYPoint, clouds[clouds.Length - 1].transform.position.y - cloudHeight, 0);
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
        cloudHeight = cloudPrefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
        float screenBottomBound = Camera.main.ScreenToWorldPoint(Vector3.zero).y;
        float screenTopBound = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        int numberOfClouds = Mathf.CeilToInt((screenTopBound - screenBottomBound) / cloudHeight) + 1; // Additional cloud for the topmost

        clouds = new GameObject[numberOfClouds];
        Vector3 position = new Vector3(spawnYPoint, screenBottomBound - cloudHeight, 0);

        for (int i = 0; i < numberOfClouds; i++)
        {
            int randomCloudIndex = Random.Range(0, cloudPrefabs.Length);
            GameObject cloudInstance = Instantiate(cloudPrefabs[randomCloudIndex], position, Quaternion.Euler(rotation));
            clouds[i] = cloudInstance;
            DontDestroyOnLoad(clouds[i]);
            position.y += cloudHeight;
        }
    }
}
