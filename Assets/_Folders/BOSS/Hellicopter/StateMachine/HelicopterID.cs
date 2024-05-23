
using UnityEngine;

[CreateAssetMenu]
public class HelicopterID : ScriptableObject
{
    public Vector2 leftPosition;
    public Vector2 normalPosition;
    public float normalSpeed;
    public bool isFlipped;
    public float xSwayAmount;
    public float ySwayAmount;
    public float shootingCooldown;

    public float minSwitchTime;
    public float maxSwitchTime;
    public int bulletAmount;
    public int Lives;
    public float shotDuration;
    public float rotationToPlayerSpeed;

    public float dipYTarget;
    public float riseYTarget;
    public float dippingYSpeed;
    public float risingYSpeed;
    public float dippingXSpeed;
    public float risingXSpeed;
    public int missileCount;

    public float dippedXSpeed;

    public HelicopterEvents events;


    public float GetOffset()
    {
        if (missileCount == 1)
        {
            return -3.5f;
        }
        else if (missileCount == 2)
        {
            return 0;

        }
        else if (missileCount == 3)
        {
            return 3.5f;
        }
        else
        {
            return 0;
        }

    }


}

