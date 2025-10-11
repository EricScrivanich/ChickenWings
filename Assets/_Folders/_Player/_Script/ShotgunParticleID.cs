using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunParticleID", menuName = "ScriptableObjects/ShotgunParticleID")]
public class ShotgunParticleID : ScriptableObject
{

    [SerializeField] private int bulletCount;

    public float speed;


    public float lifeTime = 1;
    [field: SerializeField] public Color startFlashColor { get; private set; }
    [field: SerializeField] public Color endFlashColor { get; private set; }



    [Header("New Data")]


    [field: SerializeField] public int outerBulletCount { get; private set; }
    [field: SerializeField] public int innerBulletCount { get; private set; }

    [field: SerializeField] public float outerBulletVel { get; private set; }
    [field: SerializeField] public float innerBulletVel { get; private set; }

    [field: SerializeField] public Vector2[] bulletRotationsAndYPositions { get; private set; }
    [field: SerializeField] public float[] xOffsetBullets { get; private set; }

    public int TotalBulletCount()
    {
        return outerBulletCount + innerBulletCount;
    }

    public void SetData(Vector2[] d)
    {
        bulletRotationsAndYPositions = d;
    }






    // Start is called once before the first execution of Update after the MonoBehaviour is created




}




// Update is called once per frame


