using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class SimpleProjectileMovement : MonoBehaviour {

	public Vector3 direction = Vector3.forward;
	public float speed = 2f;

	// Use this for initialization
	void Update () {
		
		transform.Translate( direction.normalized * speed * Time.deltaTime, Space.Self );
	}
}
