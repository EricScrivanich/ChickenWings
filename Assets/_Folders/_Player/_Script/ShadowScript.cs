using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowScript : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    private SpriteRenderer shadowRenderer; // Reference to the shadow's SpriteRenderer component

    private float maxY = 3f; // Maximum Y position of the player
    private float minY = -4f; // Minimum Y position of the player

    private Vector2 maxScale = new Vector2(1.2f, 0.5f); // Maximum scale of the shadow
    private Vector2 minScale = new Vector2(0.8f, 0.35f); // Minimum scale of the shadow

    private float minOpacity = .2f; // Minimum opacity of the shadow
    private float maxOpacity = 0.5f; // Maximum opacity of the shadow
    private float bottomPos;
    private void Start()
    {
        shadowRenderer = GetComponent<SpriteRenderer>();
        bottomPos = BoundariesManager.GroundPosition - .1f;

    }
    void LateUpdate()
    {

        // Ensure the shadow follows the player's X position
        transform.position = new Vector2(transform.position.x, bottomPos);

        // Map the player's Y position to the shadow's scale
        float scaleRatio = Mathf.InverseLerp(minY, maxY, parentTransform.position.y);
        Vector2 shadowScale = Vector2.Lerp(minScale, maxScale, scaleRatio);
        transform.localScale = shadowScale;

        // Map the player's Y position to the shadow's opacity
        float opacityRatio = Mathf.InverseLerp(maxY, minY, parentTransform.position.y); // Note the inversion of minY and maxY
        Color shadowColor = shadowRenderer.color;
        shadowColor.a = Mathf.Lerp(minOpacity, maxOpacity, opacityRatio);
        shadowRenderer.color = shadowColor;
    }
}
