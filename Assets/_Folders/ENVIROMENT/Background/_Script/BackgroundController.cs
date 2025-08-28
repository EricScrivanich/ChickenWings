using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    // [System.Serializable]
    // public class ParallaxLayer
    // {
    //     public Transform transform1;
    //     public Transform transform2;
    //     public float speed;
    //     public float startPosition1;
    //     public float startPosition2;
    // }

    [SerializeField] private Transform topViewTransform;




    public List<GameObject> layers;
    public List<float> speeds;
    public List<float> MountainSpeeds;
    // public List<float> gradientBlends;

    // public float groundSpeed;
    // public float mountainSpeed;

    // public ParallaxLayer skyLayer;
    // public ParallaxLayer mountainsLayer;
    // public ParallaxLayer groundLayer;

    // private const float ResetPosition = -51f;
    // private const float Offset = 80f;
    [SerializeField] private bool levelCreator = false;


    private Material[] layerMaterials;
    private float[] textureScrollSpeeds; // Speed in texture coordinate space


    [Header("Cloud Settings")]
    [SerializeField] private Sprite[] cloudSprites;
    [SerializeField] private GameObject cloudPrefab;

    [SerializeField] private Vector2Int cloudCountRange = new Vector2Int(4, 8);
    [SerializeField] private Vector2 cloudSpeedRange = new Vector2(0.3f, 1.3f);
    [SerializeField] private Vector2 cloudScaleRange = new Vector2(0.5f, 1.5f);
    [SerializeField] private Color colorFar;
    [SerializeField] private Color colorClose;

    [SerializeField] private Vector2 cloudYRange;
    [SerializeField] private Vector2 cloudXRange;

    private Vector2 cloudSpawnInterval;
    private float cloudSpawnTimer = 0f;
    private bool spawningCloud;

    private List<Transform> activeClouds = new List<Transform>();
    private List<float> cloudSpeeds = new List<float>();

    private SpriteRenderer[] clouds;

    private int cloudCount;
    private int cloudIndex = 0;




    private void Awake()
    {
        BoundariesManager.GroundPosition = layers[0].transform.position.y;
        BoundariesManager.TopViewBoundary = topViewTransform.position.y;
        layerMaterials = new Material[layers.Count];
        // Initialize arrays
        if (levelCreator)
        {

            textureScrollSpeeds = new float[layers.Count];
        }


        for (int i = 0; i < layers.Count; i++)
        {
            // Get the renderer and sprite
            var renderer = layers[i].GetComponent<Renderer>();
            var sprite = layers[i].GetComponent<SpriteRenderer>();

            // Calculate the actual width of the sprite in world units
            float spriteWidth = sprite.bounds.size.x;

            // Get the material from the renderer


            // Convert world speed to texture coordinate speed
            float textureSpeed = speeds[i] / spriteWidth;

            if (i == 0)
                textureSpeed = BoundariesManager.GroundSpeed / spriteWidth;

            layerMaterials[i] = renderer.material;
            // Store texture scroll speed for later use
            if (levelCreator)
            {
                textureScrollSpeeds[i] = textureSpeed * 5;

            }


            // Set initial scroll speed
            layerMaterials[i].SetFloat("_TextureScrollXSpeed", textureSpeed);
        }

        if (levelCreator) return;
        clouds = new SpriteRenderer[cloudCountRange.y];
        for (int i = 0; i < cloudCountRange.y; i++)
        {
            // Set the initial texture offset to zero
            var c = Instantiate(cloudPrefab, transform).GetComponent<SpriteRenderer>();
            clouds[i] = c;
            c.gameObject.SetActive(false);

        }

        // cloudCount = Random.Range(cloudCountRange.x, cloudCountRange.y + 1);
        // for (int i = 0; i < cloudCount; i++)
        // {
        //     // Randomly select a cloud sprite

        //     float distance = Random.Range(0f, 1f);

        //     // Set the sprite and initial properties
        //     clouds[i].sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
        //     clouds[i].color = Color.Lerp(colorClose, colorFar, distance);
        //     float s = Mathf.Lerp(cloudScaleRange.x, cloudScaleRange.y, distance);
        //     float n = Random.Range(0, 1);
        //     int flip = n > 0.45f ? 1 : -1;

        //     clouds[i].transform.localScale = new Vector3(s * flip + Random.Range(-.2f, .2f), s + Random.Range(-.2f, .2f), 1f);
        //     activeClouds.Add(clouds[i].transform);

        //     // Set initial position
        //     Vector2 position = new Vector3(
        //         Random.Range(BoundariesManager.leftBoundary + .4f, BoundariesManager.rightBoundary - .4f),
        //         Random.Range(cloudYRange.x, cloudYRange.y)
        //     );
        //     activeClouds[i].position = position;

        //     // Store the speed for this cloud
        //     cloudSpeeds.Add(Mathf.Lerp(cloudSpeedRange.x, cloudSpeedRange.y, distance));
        //     activeClouds[i].gameObject.SetActive(true);
        // }
    }

    // void Update()
    // {

    //     if (levelCreator) return;
    //     for (int i = 0; i < activeClouds.Count; i++)
    //     {
    //         // Move each cloud based on its speed
    //         activeClouds[i].position += Vector3.left * cloudSpeeds[i] * Time.deltaTime;

    //         // Check if the cloud has moved out of bounds
    //         if (activeClouds[i].position.x < BoundariesManager.leftBoundary)
    //         {
    //             // Reset position to the right side
    //             activeClouds[i].position = new Vector2(
    //                 Random.Range(BoundariesManager.rightBoundary + .4f, BoundariesManager.rightBoundary + 1.4f),
    //                 Random.Range(cloudYRange.x, cloudYRange.y)
    //             );
    //         }
    //     }

    // }

    /// <summary>
    /// Sets all layer textures to the correct offset based on the given game time
    /// </summary>

    public void SetTextureOffsetsByTime(ushort gameStep, float realTime)
    {
        float gameTime = realTime;


        if (realTime == 0) gameTime = gameStep * LevelRecordManager.TimePerStep;
        for (int i = 0; i < layerMaterials.Length; i++)
        {
            // Calculate what the offset should be at this point in time
            // Offset = (speed * time) % 1.0 to keep it within 0-1 range
            float xOffset = (textureScrollSpeeds[i] * gameTime) % 1.0f;

            // Set the offset directly instead of relying on continuous scrolling
            layerMaterials[i].SetTextureOffset("_MainTex", new Vector2(xOffset, 0));
        }
    }

    private void SpawnCloud()
    {

    }

    private void OnEnable()
    {
        if (levelCreator)
        {
            LevelRecordManager.SetGlobalTime += SetTextureOffsetsByTime;

        }

    }
    private void OnDisable()
    {
        if (levelCreator)
            LevelRecordManager.SetGlobalTime -= SetTextureOffsetsByTime;
    }


    private void Start()
    {




        // skyLayer.startPosition1 = skyLayer.transform1.position.x;
        // skyLayer.startPosition2 = skyLayer.transform2.position.x;

        // mountainsLayer.startPosition1 = mountainsLayer.transform1.position.x;
        // mountainsLayer.startPosition2 = mountainsLayer.transform2.position.x;

        // groundLayer.startPosition1 = groundLayer.transform1.position.x;
        // groundLayer.startPosition2 = groundLayer.transform2.position.x;
    }

    // private void FixedUpdate()
    // {
    //     MoveLayer(skyLayer);
    //     MoveLayer(mountainsLayer);
    //     MoveLayer(groundLayer);
    // }

    // private void MoveLayer(ParallaxLayer layer)
    // {
    //     // Calculate the new positions based on the speed and time
    //     float newPosition1 = layer.startPosition1 + layer.speed * Time.time;
    //     float newPosition2 = layer.startPosition2 + layer.speed * Time.time;

    //     // Apply the new positions to the transforms
    //     layer.transform1.position = new Vector3(newPosition1, layer.transform1.position.y, layer.transform1.position.z);
    //     layer.transform2.position = new Vector3(newPosition2, layer.transform2.position.y, layer.transform2.position.z);

    //     Reset(layer);
    // }

    public void MoveWithMountains(bool turnOffSpeed)
    {
        for (int i = 0; i < layers.Count; i++)
        {
            Material mat = layers[i].GetComponent<Renderer>().material;

            mat.SetFloat("_TextureScrollXSpeed", MountainSpeeds[i]);

        }

    }



    // private void Reset(ParallaxLayer layer)
    // {
    //     if (layer.transform1.position.x <= ResetPosition)
    //     {
    //         float newPosition = layer.transform2.position.x + Offset;
    //         layer.transform1.position = new Vector3(newPosition, layer.transform1.position.y, layer.transform1.position.z);
    //         layer.startPosition1 = newPosition - layer.speed * Time.time;
    //     }

    //     if (layer.transform2.position.x <= ResetPosition)
    //     {
    //         float newPosition2 = layer.transform1.position.x + Offset;
    //         layer.transform2.position = new Vector3(newPosition2, layer.transform2.position.y, layer.transform2.position.z);
    //         layer.startPosition2 = newPosition2 - layer.speed * Time.time;
    //     }
    // }



}

