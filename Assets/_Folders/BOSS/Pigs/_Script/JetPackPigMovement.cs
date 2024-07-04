using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPackPigMovement : MonoBehaviour
{
    public float speed;
    private float speedVar;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * speedVar * Time.deltaTime);

        if (transform.position.x < -30)
        {
            speedVar = 0;
        }
    }

    private void OnEnable()
    {
        speedVar = speed;

    }
}