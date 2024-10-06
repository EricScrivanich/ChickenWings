using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationDataSO", menuName = "ScriptableObjects/SpriteAnimationData")]
public class AnimationDataSO : ScriptableObject
{
    public Sprite[] sprites;
    public Vector2 RandomDelaySpriteSwitch;
    public float constantSwitchTime;
    public float[] opacityTargets;

    public Vector3 startScale;
    public Vector3 endScale;
}
