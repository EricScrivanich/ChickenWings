using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform transform1;
        public Transform transform2;
        public float speed;
        public float startPosition1;
        public float startPosition2;
    }

    public ParallaxLayer skyLayer;
    public ParallaxLayer mountainsLayer;
    public ParallaxLayer groundLayer;

    private const float ResetPosition = -51f;
    private const float Offset = 80f;

    private void Start()
    {
        skyLayer.startPosition1 = skyLayer.transform1.position.x;
        skyLayer.startPosition2 = skyLayer.transform2.position.x;

        mountainsLayer.startPosition1 = mountainsLayer.transform1.position.x;
        mountainsLayer.startPosition2 = mountainsLayer.transform2.position.x;

        groundLayer.startPosition1 = groundLayer.transform1.position.x;
        groundLayer.startPosition2 = groundLayer.transform2.position.x;
    }

    private void FixedUpdate()
    {
        MoveLayer(skyLayer);
        MoveLayer(mountainsLayer);
        MoveLayer(groundLayer);
    }

    private void MoveLayer(ParallaxLayer layer)
    {
        // Calculate the new positions based on the speed and time
        float newPosition1 = layer.startPosition1 + layer.speed * Time.time;
        float newPosition2 = layer.startPosition2 + layer.speed * Time.time;

        // Apply the new positions to the transforms
        layer.transform1.position = new Vector3(newPosition1, layer.transform1.position.y, layer.transform1.position.z);
        layer.transform2.position = new Vector3(newPosition2, layer.transform2.position.y, layer.transform2.position.z);

        Reset(layer);
    }

    private void Reset(ParallaxLayer layer)
    {
        if (layer.transform1.position.x <= ResetPosition)
        {
            float newPosition = layer.transform2.position.x + Offset;
            layer.transform1.position = new Vector3(newPosition, layer.transform1.position.y, layer.transform1.position.z);
            layer.startPosition1 = newPosition - layer.speed * Time.time;
        }

        if (layer.transform2.position.x <= ResetPosition)
        {
            float newPosition2 = layer.transform1.position.x + Offset;
            layer.transform2.position = new Vector3(newPosition2, layer.transform2.position.y, layer.transform2.position.z);
            layer.startPosition2 = newPosition2 - layer.speed * Time.time;
        }
    }

}
       
