using UnityEngine;

public class SpawnedPigObject : SpawnedObject, IDamageable, IEggable
{
    protected bool canAttack = true;


    public bool CanPerfectScythe => throw new System.NotImplementedException();

    public Vector2 GetPosition()
    {
        Debug.Log("GetPosition: " + rb.position);
        return rb.position;
    }

    public virtual void EggPig(int type, Vector2 vector, float offset)
    {

        this.enabled = false;
        canAttack = false;

        //set rb to dynamic
        rb.gravityScale = 1.25f;
        rb.linearDamping = .25f;
        rb.angularDamping = .3f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = vector;
        rb.angularVelocity = offset * 10f;
        // if (GetComponent<PigMaterialHandler>() != null)
        // {
        //     GetComponent<PigMaterialHandler>().DoPigHitActivation();
        // }
        GetComponent<PigMaterialHandler>().DoPigHitActivation();


    }



    public void KillOnGround()
    {
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        rb.linearDamping = 0;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.rotation = 0;
        rb.angularVelocity = 0;

        GetComponent<PigMaterialHandler>().KillEnemy(1, 0, -1);
        // if (GetComponent<PigMaterialHandler>() != null)
        // {
        //     rb.gravityScale = 0;
        //     rb.linearVelocity = Vector2.zero;

        //     rb.linearDamping = 0;
        //     rb.bodyType = RigidbodyType2D.Kinematic;
        //     rb.rotation = 0;
        //     rb.angularVelocity = 0;

        //     GetComponent<PigMaterialHandler>().Damage(1, 0, -1);
        // }
    }
    private bool isHit = false;
    public void Damage(int damageAmount = 1, int type = -1, int id = -1)
    {
        canAttack = false;
        GetComponent<PigMaterialHandler>().KillEnemy(damageAmount, type, id);

    }

    public void OnEgged()
    {
        GetComponent<PigMaterialHandler>().OnEgged();
    }

}
