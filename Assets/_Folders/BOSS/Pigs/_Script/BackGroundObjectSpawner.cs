using UnityEngine;

public class BackGroundObjectSpawner : MonoBehaviour
{
    [SerializeField] private BackGroundObject[] backgroundObjects;
    [SerializeField] private Material backgroundMaterial;
    [SerializeField] private Vector2 minMaxY;
    [SerializeField] private Vector2 minMaxX;
    [SerializeField] private float maxXDistance;
    [SerializeField] private Vector2Int minMaxObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var backgroundObject in backgroundObjects)
        {
            backgroundObject.ApplyMaterialToAllSprites(backgroundMaterial, null);
        }
        int r = Random.Range(minMaxObjects.x, minMaxObjects.y + 1);

        for (int i = 0; i < r; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-maxXDistance, maxXDistance), Random.Range(minMaxY.x, minMaxY.y));
            BackGroundObject bgObj = Instantiate(backgroundObjects[Random.Range(0, backgroundObjects.Length)], pos, Quaternion.identity);

            // bgObj.Initialize(pos, Random.Range(0f, 1f), maxXDistance);
        }


    }

    // Update is called once per frame
    
}
