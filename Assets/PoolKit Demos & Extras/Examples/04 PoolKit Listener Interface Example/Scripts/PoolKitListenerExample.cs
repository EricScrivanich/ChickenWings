using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class PoolKitListenerExample : MonoBehaviour, IPoolKitListener {

	// Options
	[Header("Options")]
	public bool debugMessages = true;
	
	// On Spawn ( Required because this is an IPoolKitListener )
	public void OnSpawn ( Pool pool ) {

		// Show Debug Messages
		if(debugMessages){ Debug.Log( gameObject.name + " has just spawned from the pool: " + pool.poolName ); }
	}
	
	// On Despawn ( Required because this is an IPoolKitListener )
	public void OnDespawn () { 

		// Show Debug Messages
		if(debugMessages){ Debug.Log( gameObject.name + " has just despawned!" ); }
	}
}