using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectableData: ScriptableObject
{
    public abstract void InitializeCollectable(CollectablePoolManager manger,bool finalTrigger);
}
