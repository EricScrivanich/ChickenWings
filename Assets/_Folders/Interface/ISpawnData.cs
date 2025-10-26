using UnityEngine;
public interface ISpawnData
{
   
    void ApplyTo(SpawnedObject obj);

    Vector2 GetStartPos();
    short GetID();
    ushort GetType();

    float[] GetFloatData();
}