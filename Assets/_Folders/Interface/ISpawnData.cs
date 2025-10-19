using UnityEngine;
public interface ISpawnData
{
   
    void ApplyTo(SpawnedObject obj);

    Vector2 GetStartPos();
    ushort GetType();
}