
using UnityEngine;

[CreateAssetMenu]

public class PlayerID : ScriptableObject
{

    public int Lives;
    public bool IsTwoTouchPoints;
    public float StaminaUsed;
    public float MaxStamina = 100;
    [SerializeField] private int startingAmmo;

    public int Ammo;
    public float MaxFallSpeed;
    public float AddEggVelocity;
    public PlayerEvents events;
    public GlobalPlayerEvents globalEvents;
    public float parachuteXOffset;
    public float parachuteYOffset;

    public void ResetValues()
    {
        Ammo = startingAmmo;
        globalEvents.OnUpdateAmmo?.Invoke();
    }


}
