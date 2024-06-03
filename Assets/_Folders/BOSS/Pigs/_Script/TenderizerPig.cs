using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenderizerPig : MonoBehaviour, ICollectible
{
    private CircleCollider2D detection;
    private Animator anim;
    [SerializeField] private GameObject Hammer;
    public Transform player; // Reference to the player
    public Transform eye; // Reference to the eye
    public Transform pupil; // Reference to the pupil
    public float eyeRadius = 0.5f; // Radius within which the pupil can move
    private void Start()
    {
        anim = GetComponent<Animator>();
        detection = GetComponent<CircleCollider2D>();
        if (!Hammer.activeInHierarchy)
        {
            detection.enabled = false;
        }



    }
    void Update()
    {
        if (player != null)
        {
            Vector3 direction = player.position - eye.position; // Calculate the direction to the player
            direction.z = 0; // Ensure it's 2D
            direction.Normalize(); // Normalize the direction

            // Move the pupil within the eye's radius
            pupil.localPosition = direction * eyeRadius;
        }
    }

    public void Collected()
    {
        detection.enabled = false;
        anim.SetTrigger("SwingTrigger");
    }
    // Update is called once per frame

}
