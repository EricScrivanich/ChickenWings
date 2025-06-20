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
    }

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

