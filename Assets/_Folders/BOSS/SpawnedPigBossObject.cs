using UnityEngine;

public class SpawnedPigBossObject : SpawnedObject, IDamageable, IEggable
{
    protected bool canAttack = true;
    [SerializeField] protected int startingLives;
    protected int lives;
    [SerializeField] protected EnemyHeart heart;
    protected bool triggerWhenDead = false;




    public bool CanPerfectScythe => throw new System.NotImplementedException();

    public Vector2 GetPosition()
    {
        Debug.Log("GetPosition: " + rb.position);
        return rb.position;
    }
    public void SetTriggerWhenDead(LevelDataBossAndRandomLogic logic)
    {

        triggerWhenDead = true;
        Debug.LogError("Adding Trigger Component");
        var comp = this.gameObject.AddComponent<EnemyBossTrigger>();
        comp.Init(logic);

    }
    public void SetHealth(short health)
    {
        Debug.LogError("SetHealth for pig in Class: " + health);
        if (health < 1) health = 1;
        startingLives = health;
        lives = startingLives;
        if (heart != null) heart.SetHearts(startingLives);
    }

    public virtual void EggPig(int type, Vector2 vector, float offset)
    {

        this.enabled = false;

        //set rb to dynamic
        rb.gravityScale = 1.25f;
        rb.linearDamping = .25f;
        rb.angularDamping = .3f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = vector;
        rb.angularVelocity = offset * 10f;
        if (GetComponent<PigMaterialHandler>() != null)
        {
            GetComponent<PigMaterialHandler>().DoPigHitActivation();
        }



    }



    public void KillOnGround()
    {
        if (GetComponent<PigMaterialHandler>() != null)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;

            rb.linearDamping = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.rotation = 0;
            rb.angularVelocity = 0;

            GetComponent<PigMaterialHandler>().Damage(1, 0, -1);
        }
    }
    public bool isHit = false;
    public void Damage(int damageAmount = 1, int type = -1, int id = -1)
    {
        if (isHit) return;
        isHit = true;
        Debug.Log("Damage: " + damageAmount + " Is hit: " + isHit);

        // Hit(type);
        lives -= damageAmount;


        if (lives <= 0)
        {
            Kill(damageAmount, type, id);

        }
        else
            GetComponent<PigMaterialHandler>().DamageEnemy(damageAmount, type, id);


        if (heart != null) heart.Damage(lives);






    }

    public virtual void Kill(int damageAmount = 1, int type = -1, int id = -1)
    {
        GetComponent<PigMaterialHandler>().KillEnemy(damageAmount, type, id);

        if (triggerWhenDead)
            GetComponent<EnemyBossTrigger>()?.OnDeathTrigger();
    }


    public void OnEnable()
    {
        OnEnableLogic();
        lives = startingLives;

        isHit = false;
        canAttack = true;
    }

    public virtual void OnEnableLogic()
    {

    }
    public virtual void OnPigEgged()
    {

    }

    public void OnEgged()
    {

        OnPigEgged();

    }

    public virtual void Hit(int type)
    {


    }
}


// Start is called once before the first execution of Update after the MonoBehaviour is created


