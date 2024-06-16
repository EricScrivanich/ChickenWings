using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] private float YPos;
    // Start is called before the first frame update

    private void Awake()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();

    }
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector2(playerTransform.position.x, YPos);

    }
}
