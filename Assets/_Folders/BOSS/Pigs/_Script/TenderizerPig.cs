using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenderizerPig : MonoBehaviour, ICollectible
{
    public float speed;
    public bool hasHammer;
    private CircleCollider2D detection;
    private float amplitude = .3f;
    private float frequency = 1.4f;
    private float initialY;
    private Animator anim;
    [SerializeField] private GameObject Hammer;
    private Transform player; // Reference to the player
    public Transform eye; // Reference to the eye
    public Transform pupil; // Reference to the pupil
    public float eyeRadius = 0.5f; // Radius within which the pupil can move

    private void Awake()
    {
        detection = GetComponent<CircleCollider2D>();
    }
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        anim = GetComponent<Animator>();





    }
    private void OnEnable()
    {
        initialY = transform.position.y;
        detection.enabled = hasHammer;
        Hammer.SetActive(hasHammer);

    }
    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);


        float y = Mathf.Sin(transform.position.x * frequency) * amplitude + initialY;
        transform.position = new Vector2(transform.position.x, y);
        if (player != null && hasHammer)
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
        Invoke("PlaySound", .5f);

    }

    private void PlaySound()
    {
        AudioManager.instance.PlayPigHammerSwingSound();
    }
    // Update is called once per frame

}
