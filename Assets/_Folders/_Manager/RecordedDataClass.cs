using UnityEngine;

public class RecordedDataClass
{
    public int ID;

    
    public int SpawnStep;

    public int EndStep;
    public bool[] dataBool;

    public byte[] dataByte;
    public int[] dataInt;
    public float[] dataFloat;

    public Vector2[] dataVector2;
    


    public RecordedDataClass(int id, Vector2 position, int spawnStep, int endStep,
                               bool[] boolData, byte[] byteData, int[] intData, float[] floatData, Vector2[] vectorData)
    {
        this.ID = id;
        this.SpawnStep = spawnStep;
        this.EndStep = endStep;

        // Ensure arrays are initialized properly
        this.dataBool = (boolData != null) ? (bool[])boolData.Clone() : new bool[0];
        this.dataByte = (byteData != null) ? (byte[])byteData.Clone() : new byte[0];
        this.dataInt = (intData != null) ? (int[])intData.Clone() : new int[0];
        this.dataFloat = (floatData != null) ? (float[])floatData.Clone() : new float[0];
        this.dataVector2 = (vectorData != null) ? (Vector2[])vectorData.Clone() : new Vector2[0];
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created


}
