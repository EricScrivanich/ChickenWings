using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PlayerMovementData : ScriptableObject
{
    [Header("Other")]

    [SerializeField] private float maxFallSpeed;
    public float MaxFallSpeed 
    {
        get
        {
            return maxFallSpeed;
        }
        set
        {
            return;
        }
    }


    [Header("Jumping")]
    public Vector2 JumpForce;
    public Vector2 RemoveJumpForce;
    public float addJumpForce;
    [Header("Flip Right")]
    public Vector2 FlipRightInitialForceVector;
    public Vector2 FlipRightAddForce;
    public Vector2 FlipRightDownForce;
    public float flipRightAddForceTime;
    public float flipRightDownForceTime;

    [Header("Flip Left")]
    public Vector2 FlipLeftInitialForceVector;
    public Vector2 FlipLeftAddForce;
    public Vector2 FlipLeftDownForce;
    public float flipLeftAddForceTime;
    public float flipLeftDownForceTime;


    [Header("Whirlpool Variables")]

    public float maxAmpRadRatio = 0.5f;
    public float outerAmp = 0;
    public float innerAmp = 3.14f;
    public float innerCutoff = .05f;
    public float drag = .05f;



    // Start is called before the first frame update

}
