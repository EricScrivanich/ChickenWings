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
    public Vector2 ReturnGoldPosition()
    {
        return goldBucketBallTarget.position;
    }
    public Vector2 ReturnPinkPosition()
    {
        return lives.ReturnEggPosition();
    }

  
}
