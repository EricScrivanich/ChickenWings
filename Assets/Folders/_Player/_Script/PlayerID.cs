
using UnityEngine;

[CreateAssetMenu]

public class PlayerID : ScriptableObject
{
    
    public int Lives;
    public int Ammo;
    public float MaxFallSpeed;
    public float AddEggVelocity;
    public PlayerEvents events;
    public GlobalPlayerEvents globalEvents;

}
