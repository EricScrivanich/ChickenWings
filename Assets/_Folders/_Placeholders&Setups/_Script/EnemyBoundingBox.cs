using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyBoundingBox", menuName = "Randomspawning/EnemyBoundingBox", order = 2)]
public class EnemyBoundingBox : ScriptableObject
{
    [Header("Base Settings")]
    public Vector3 baseScale = Vector3.one;
    public Vector2 scaleMinMax;

    public float baseSpeed = 1f;
    public Vector2 speedMinMax;

    [Header("Spawn Settings")]
    public Vector2 ySpawnRange;
    public Vector2 PreferredDistanceRange;

    [Header("Bounding Box Dimensions")]
    public float leftOffset = 0.5f;
    public float rightOffset = 0.5f;
    public float topOffset = 0.5f;
    public float bottomOffset = 0.5f;

    [Header("Influence Settings")]
    public float scaleInfluence = 1f;
    public float leftSpeedInfluence = 1f;
    public float rightSpeedInfluence = 1f;

    [Header("YForce Influence Settings (For Big Pig Only)")]
    public bool useYForceInfluence = false;
    public float baseYForce = 5f;
    public Vector2 yForceRange = new Vector2(3f, 10f);
    public float baseDistanceToFlap = 2f;
    public float yForceToDistanceFlapRatio = 1f;
    public float yForceInfluence = 0.5f; // Influence of yForce on top and bottom bounds

    [Header("Bounding Box Active Duration")]
    public float activeDuration = 2f;

    public Rect GetBoundingBox(Vector3 scale, float speed, Vector2 allowedOverlap, float yForce = 0f)
    {
        float scaleY = scale.y / baseScale.y;

        float speedDelta = speed - baseSpeed;
        float leftAdjustment = leftSpeedInfluence * (speedDelta / 0.5f);
        float rightAdjustment = rightSpeedInfluence * (speedDelta / 0.5f);

        float topAdjustment = scaleInfluence * (scaleY - 1f);
        float bottomAdjustment = scaleInfluence * (scaleY - 1f);

        if (useYForceInfluence)
        {
            float yForceAdjustment = yForceInfluence * (yForce - baseYForce);
            topAdjustment += yForceAdjustment;
            bottomAdjustment += yForceAdjustment;
        }

        return new Rect(
            -leftOffset - leftAdjustment + allowedOverlap.x,  // Left side adjusted by allowed overlap
            -bottomOffset - bottomAdjustment + allowedOverlap.y, // Bottom side adjusted by allowed overlap
            leftOffset + rightOffset + leftAdjustment + rightAdjustment + 2 * -allowedOverlap.x, // Total width adjusted by overlap
            topOffset + bottomOffset + topAdjustment + bottomAdjustment + 2 * -allowedOverlap.y  // Total height adjusted by overlap
        );
    }

    public float CalculateDistanceToFlap(float yForce)
    {
        return baseDistanceToFlap + (yForce - baseYForce) * yForceToDistanceFlapRatio;
    }
}