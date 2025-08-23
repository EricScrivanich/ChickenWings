using UnityEngine;

public class SpawnedObject : MonoBehaviour
{
    public Rigidbody2D rb { get; protected set; }
    protected bool canAttack = true;
    protected ushort objectID;

    public void InitialSpawnCheck(ushort id)
    {
        if (this.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
        if (!this.enabled)
            this.enabled = true;

        objectID = id;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void ApplyFloatOneData(DataStructFloatOne data)
    {
    }
    public virtual void ApplyFloatTwoData(DataStructFloatTwo data)
    {
    }
    public virtual void ApplyFloatThreeData(DataStructFloatThree data)
    {
    }
    public virtual void ApplyFloatFourData(DataStructFloatFour data)
    {
    }
    public virtual void ApplyFloatFiveData(DataStructFloatFive data)
    {
    }
    public virtual void ApplyPositionerData(RecordedObjectPositionerDataSave data)
    {

    }
    public Vector2 GetPosition()
    {
        Debug.Log("GetPosition: " + rb.position);
        return rb.position;
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

}
