using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PlayerMovementData : ScriptableObject
{
    [Header("Jumping")]
    public float JumpForce;
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
    


    // Start is called before the first frame update

}
