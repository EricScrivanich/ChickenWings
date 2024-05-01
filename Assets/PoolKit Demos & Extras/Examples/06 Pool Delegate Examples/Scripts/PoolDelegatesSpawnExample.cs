//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	PoolDelegatesSpawnExample.cs
//
//	An example script showing how to subscribe / unsubscribe to PoolKit's Pool Spawn Events.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class PoolDelegatesSpawnExample : MonoBehaviour {

	[Header("Find Your Pool By Name")]
	public string poolName = "MyPool";
	Pool findPool;

	[Header("Options")]
	public bool debugSpawnedInstances = true;
	public bool debugDespawnedInstances = true;
	
	// ==================================================================================================================
	//	ON ENABLE
	//	Use this when the gameobject has been enabled
	// ==================================================================================================================
	
	void OnEnable(){
		
		// Find the pool (if we haven't already)
		if(findPool==null){ findPool = PoolKit.Find( poolName ); }
		
		// If we have found the pool, subscribe to the delegates
		if(findPool!=null ){

			// < Optional > Turn on Pool Events in case you've forgotten to set it in the Pool's inspector!
			findPool.enablePoolEvents = true;

			// Subscribe to the events
			findPool.onPoolSpawn += onPoolSpawn;
			findPool.onPoolDespawn += onPoolDespawn;

		}
	}

	// ==================================================================================================================
	//	ON DISABLE
	//	Use this when the gameobject has been disabled
	// ==================================================================================================================
	
	void OnDisable(){

		// If we found the pool, unsubscribe from the delegates
		if(findPool!=null){

			// Unsubscribe from the events
			findPool.onPoolSpawn -= onPoolSpawn;
			findPool.onPoolDespawn -= onPoolDespawn;
		}
	}

	// ==================================================================================================================
	//	EVENTS
	//	These events will be called by the pool when an instance is spawned or despawned.
	// ==================================================================================================================
	
	// Event called when the pool spawns an object
	void onPoolSpawn( Transform instance, Pool pool ) {
		if(debugSpawnedInstances){ 
			Debug.Log("POOL DELEGATE EXAMPLE: The Pool " + pool.poolName + " just spawned: " + instance.name );
		}
	}

	// Event called when the pool despawns an object
	void onPoolDespawn( Transform instance, Pool pool ) {
		if(debugDespawnedInstances){ 
			Debug.Log("POOL DELEGATE EXAMPLE: The Pool " + pool.poolName + " just despawned: " + instance.name );
		}
	}
}
