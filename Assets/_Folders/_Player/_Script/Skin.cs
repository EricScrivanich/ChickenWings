using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer eyes;
    public PlayerParts parts;
    // Start is called before the first frame update
    void Start()
    {
        if (BoundariesManager.isDay)
        {
            eyes.sprite = parts.glasses;
        }
        else
        {
            eyes.sprite = parts.eyes;

        }


    }

    // Update is called once per frame
    void Update()
    {

    }
}
