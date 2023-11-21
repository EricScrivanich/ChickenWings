using UnityEngine;

[CreateAssetMenu]
public class PlaneData : ScriptableObject
{
    public float speed;
    public float speedOffset;
    public float planeSinFrequency;
    public float planeSinAmplitude;
    public float minSpawn;
    public float maxSpawn;
    public float planeTime;
    public float planeTimeOffset;
    public int lives;
}
