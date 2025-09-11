using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScreenPositions : MonoBehaviour
{
    [SerializeField] private Transform goldBucketBallTarget;
    [SerializeField] private LifeDisplay lives;

    [SerializeField] private ParticleSystem heartParticles;
    [SerializeField] private ParticleSystem goldParticles;
    // Start is called before the first frame update
    // void Start()
    // {
    //     GetComponent<Canvas>().worldCamera = Camera.main;

    // }
    public Vector2 ReturnGoldPosition()
    {
        if (goldBucketBallTarget == null) return Vector2.zero;
        return goldBucketBallTarget.position;
    }
    public Vector2 ReturnPinkPosition()
    {
        return lives.ReturnEggPosition();
    }


}
