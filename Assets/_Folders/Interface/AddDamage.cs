using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDamage : MonoBehaviour
{
    public PlayerID ID;
    private bool blocked = false;
    [SerializeField] private int type;
    [SerializeField] private int damageAmount = 1;  // Amount of damage the slash does
    [SerializeField] private float raycastAngledCheckAmount;
    [SerializeField] private Transform origin;

    [SerializeField] private Vector2 raycastOffset = Vector2.zero;  // Offset from the object's position
    [SerializeField] private float raycastDistance = 2f;            // Distance to check for collisions
    [SerializeField] private float raycastAngle = 0f;
    private Animator anim;

    private bool justCreated = true;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Block"))
        {
            AudioManager.instance.PlayParrySound();
            Debug.Log("Attack was blocked boi");
            return;
        }

        IDamageable damageableEntity = collider.gameObject.GetComponent<IDamageable>();
        if (damageableEntity != null)
        {


            damageableEntity.Damage(damageAmount, type);
        }


    }
    private void OnEnable()
    {
        if (justCreated)
        {
            justCreated = false;
            return;
        }
        blocked = false;

        if (origin != null)
            PerformBlockRaycast();
    }

    public void AttackFinished()
    {
        Debug.Log("finished");
        // ID.events.OnAttack?.Invoke(false);
    }

    private bool RaycastHitBlock(RaycastHit2D[] hits)
    {


        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Plane"))
            {
                Debug.Log("Hit plane first, ignoring...");
                return false; // Ignore if "Plane" is hit before "Weapon"
            }

            if (hit.collider.CompareTag("Weapon"))
            {
                Debug.Log("Blocked Weapon!");
                return true;


            }
        }
        return false;

    }

    private void PerformBlockRaycast()
    {
        // Calculate adjusted direction based on parent scale

        Vector2 direction;
        Vector2 direction2;
        Vector2 direction3;
        // GetRaycastParams(out origin, out direction, 0);
        // GetRaycastParams(out origin, out direction2, raycastAngledCheckAmount);
        // GetRaycastParams(out origin, out direction3, raycastAngledCheckAmount);
        GetRaycastParams(out direction, out direction2, out direction3, raycastAngledCheckAmount);
        // Perform the raycast
        RaycastHit2D[] hits1 = Physics2D.RaycastAll(origin.position, direction, raycastDistance);
        RaycastHit2D[] hits2 = Physics2D.RaycastAll(origin.position, direction2, raycastDistance);
        RaycastHit2D[] hits3 = Physics2D.RaycastAll(origin.position, direction3, raycastDistance);



        if (RaycastHitBlock(hits1))
        {
            blocked = true;
            return;
        }

        if (RaycastHitBlock(hits2))
        {
            blocked = true;
            return;
        }
        if (RaycastHitBlock(hits3))
        {
            blocked = true;
            return;
        }



    }



    private void OnDrawGizmos()
    {
        if (origin != null)
        {
            Vector2 direction;
            Vector2 direction2;
            Vector2 direction3;
            GetRaycastParams(out direction, out direction2, out direction3, raycastAngledCheckAmount);

            // Draw the ray in the Scene view
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin.position, (Vector2)origin.position + direction * raycastDistance);



            // Draw the ray in the Scene view

            Gizmos.DrawLine(origin.position, (Vector2)origin.position + direction2 * raycastDistance);



            // Draw the ray in the Scene view

            Gizmos.DrawLine(origin.position, (Vector2)origin.position + direction3 * raycastDistance);

        }

    }

    /// <summary>
    /// Calculates the correct raycast origin and direction, considering object scale.
    /// </summary>
    private void GetRaycastParams(out Vector2 direction, out Vector2 direction2, out Vector2 direction3, float angle)
    {
        // Get parent scale to determine flipping
        Transform parent = transform.parent;
        float scaleX = parent ? parent.lossyScale.x : transform.lossyScale.x;
        float scaleY = parent ? parent.lossyScale.y : transform.lossyScale.y;
        float ra = raycastAngle;

        if (scaleX != scaleY) ra *= -1;

        // Calculate the base angle using global rotation
        float adjustedAngle = transform.eulerAngles.z + ra;

        // Handle single-axis flips correctly
        if (scaleX < 0 && scaleY > 0)
        {
            adjustedAngle = 180f + adjustedAngle;


        }  // Flipped X only
        // if (scaleY < 0 && scaleX > 0) ra *= -1;    // Flipped Y only
        if (scaleX < 0 && scaleY < 0) adjustedAngle += 180f;                // Both axes flipped (double flip cancels out, but rotates fully)

        // Convert the adjusted angle to direction
        direction = new Vector2(Mathf.Cos(adjustedAngle * Mathf.Deg2Rad), Mathf.Sin(adjustedAngle * Mathf.Deg2Rad));

        adjustedAngle = transform.eulerAngles.z + ra + angle;

        // Handle single-axis flips correctly
        if (scaleX < 0 && scaleY > 0) adjustedAngle = 180f + adjustedAngle; // Flipped X only
        // if (scaleY < 0 && scaleX > 0) adjustedAngle = adjustedAngle;       // Flipped Y only
        if (scaleX < 0 && scaleY < 0) adjustedAngle += 180f;                // Both axes flipped (double flip cancels out, but rotates fully)

        // Convert the adjusted angle to direction
        direction2 = new Vector2(Mathf.Cos(adjustedAngle * Mathf.Deg2Rad), Mathf.Sin(adjustedAngle * Mathf.Deg2Rad));

        adjustedAngle = transform.eulerAngles.z + ra - angle;

        // Handle single-axis flips correctly
        if (scaleX < 0 && scaleY > 0) adjustedAngle = 180f + adjustedAngle; // Flipped X only
        // if (scaleY < 0 && scaleX > 0) adjustedAngle = adjustedAngle;       // Flipped Y only
        if (scaleX < 0 && scaleY < 0) adjustedAngle += 180f;                // Both axes flipped (double flip cancels out, but rotates fully)

        // Convert the adjusted angle to direction
        direction3 = new Vector2(Mathf.Cos(adjustedAngle * Mathf.Deg2Rad), Mathf.Sin(adjustedAngle * Mathf.Deg2Rad));

        // Offset the origin
        // origin = (Vector2)transform.localPosition + raycastOffset;
    }
}
