using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BarnID : ScriptableObject
{
    public Sprite[] haySmall;
    public Sprite[] hayBig;
    public Sprite barnSide;


    public Sprite GetRandomSmallHay()
    {
        int randomIndex = Random.Range(0, haySmall.Length + 1);

        if (randomIndex == haySmall.Length)
        {
            return null;
        }

        return haySmall[randomIndex];
    }

    public Sprite GetRandomBigHay()
    {
        int randomIndex = Random.Range(0, hayBig.Length + 1);

        if (randomIndex == hayBig.Length)
        {
            return null;
        }

        return hayBig[randomIndex];
    }

}
