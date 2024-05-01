using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float YPos;
    // Start is called before the first frame update
   

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector2(playerTransform.position.x, YPos);

    }
}
