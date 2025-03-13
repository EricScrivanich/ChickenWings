using UnityEngine;


[CreateAssetMenu]
public class PigsScriptableObject : ScriptableObject
{
    [SerializeField] private Sprite[] NormalPigHeads;
    [SerializeField] private Sprite[] OnPigDeathFace;
    [SerializeField] private float setMagPercent;

    [Header("Indexs For Each Value Type: Bool, Byte, Int, Float, Vector2")]
    [SerializeField] private byte[] IndexsPerValue;

    public string ObjectTitle;

    public Vector2Int[] ParameterByOrder;

    public string[] parameterTitles;




    public float baseScale;

    public Vector2 MinMaxSpeed;
    public Vector2 MinMaxAmp;
    public Vector2 MinMaxFreq;

    public Vector2 MinMaxMagDiff;
    public float maxHeightAtMinSpeed;
    public float maxFreqAtMinMag;

    // public Vector2 MinMaxAmp;

    public void ReturnSineWaveLogic(float speed, float mag, out float magR, out float freqR, out float magDif)
    {
        bool neg = false;
        if (speed < 0)
        {
            speed = Mathf.Abs(speed);
            neg = true;
        }
        float l = Mathf.InverseLerp(MinMaxSpeed.x, MinMaxSpeed.y, speed);
        float magAccountedForSpeed = Mathf.Lerp(maxHeightAtMinSpeed, 0, l);

        if (setMagPercent != 0) mag = setMagPercent;

        magR = Mathf.Lerp(MinMaxAmp.x, MinMaxAmp.y, mag) + magAccountedForSpeed;

        // if (neg) magR *= -1;

        freqR = Mathf.Lerp(MinMaxFreq.y, MinMaxFreq.x, l) + Mathf.Lerp(maxFreqAtMinMag, 0, mag);
        magDif = Mathf.Lerp(MinMaxMagDiff.x, MinMaxMagDiff.y, mag);

    }






    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Sprite ReturnStartingHead()
    {
        // switch (type)
        // {
        //     case 0:
        //         if (NormalPigHeads != null && NormalPigHeads.Length > 0)
        //         {
        //             int r = Random.Range(0, NormalPigHeads.Length);
        //             return NormalPigHeads[r];

        //         }
        //         else break;

        // }

        // return null;

        if (NormalPigHeads != null && NormalPigHeads.Length > 0)
        {
            int r = Random.Range(0, NormalPigHeads.Length);
            return NormalPigHeads[r];

        }
        else return null;
    }


    public Sprite ReturnDeathHead(int type)
    {
        if (OnPigDeathFace == null || type >= OnPigDeathFace.Length)
        {
            Debug.Log("NO PIG FACE");
            return null;
        }

        else
        {
            Debug.Log("Returning sprite of type: " + type);
            return OnPigDeathFace[type];
        }
    }

}
