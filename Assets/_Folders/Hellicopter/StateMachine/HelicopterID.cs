
using UnityEngine;

[CreateAssetMenu]
public class HelicopterID : ScriptableObject 
{
    public Vector2 normalPosition;
    public float xSwayAmount;
    public float ySwayAmount;
    public float waitToShootTime;
    public float rotationToPlayerSpeed;

    public HelicopterEvents events;
  
    
}

