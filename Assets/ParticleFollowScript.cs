
using UnityEngine;

public class ParticleFollowScript : MonoBehaviour
{
    private ParticleSystem ps;
    private float speed;
    // Start is called before the first frame update
    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

    }



    public void PlayParticle(float speedVar)
    {
        if (!this.gameObject.activeInHierarchy)
        {
            this.gameObject.SetActive(true);
        }
        speed = speedVar;
        ps.Play();
    }

}
