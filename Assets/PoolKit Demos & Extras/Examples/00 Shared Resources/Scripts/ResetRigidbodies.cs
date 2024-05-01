using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetRigidbodies : MonoBehaviour {

	Rigidbody rb = null;

	// Use this for initialization
	void Awake() {
		rb = GetComponent<Rigidbody>();
	}
	
	// OnEnable (activates on spawn)
	void OnEnable () {
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}
}
