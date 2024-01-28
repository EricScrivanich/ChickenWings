
using UnityEngine;

[CreateAssetMenu]

public class PlayerID : ScriptableObject
{
    
    public int Lives;
    public float StaminaUsed;
    public float MaxStamina;
    public int Ammo;
    public float MaxFallSpeed;
    public float AddEggVelocity;
    public PlayerEvents events;
    public GlobalPlayerEvents globalEvents;
    public float parachuteXOffset;
    public float parachuteYOffset;


}
