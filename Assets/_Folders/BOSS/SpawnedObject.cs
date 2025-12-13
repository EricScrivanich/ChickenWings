using UnityEngine;

public class SpawnedObject : MonoBehaviour
{
    public Rigidbody2D rb { get; protected set; }
    protected bool canAttack = true;


    protected ushort objectID;
    // private short triggerType;

    public void InitialSpawnCheck(ushort id, bool ignoreDisable = false)
    {
        if (!ignoreDisable && !this.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
        if (!this.enabled)
            this.enabled = true;

        objectID = id;
        canAttack = true;
        // triggerType = trigger;
    }

    public virtual void ApplySimpleData(DataStructSimple data)
    {
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








}
