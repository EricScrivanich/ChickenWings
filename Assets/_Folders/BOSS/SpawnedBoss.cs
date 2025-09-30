using UnityEngine;

public class SpawnedBoss : MonoBehaviour
{
    protected Rigidbody2D rb;
    [field: SerializeField] public int lives;
    [field: SerializeField] public int startingLives;


    public virtual void Hit(int type)
    {

    }

    public virtual void Parry(int type)
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
