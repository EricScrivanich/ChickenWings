using UnityEngine;

public class FireballEjectorScript : MonoBehaviour
{
    private ParticleSystem ps;
    [SerializeField] private Pignosorous parentScript;
    [SerializeField] private Transform fireballSpawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        // ps = GetComponent<ParticleSystem>();
    }
    public void Eject()
    {
        // ps.Play();
        parentScript.EjectFireball(-transform.up, fireballSpawnPoint.position, transform.eulerAngles.z);

    }
}
