using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRotation : MonoBehaviour
{

    private Vector3 rotation = new Vector3(0, 0, -25);
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = Vector3.zero;

    }
}
