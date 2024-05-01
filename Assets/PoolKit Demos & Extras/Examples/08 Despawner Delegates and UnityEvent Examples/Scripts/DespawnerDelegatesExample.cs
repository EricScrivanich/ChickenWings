//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	DespawnerDelegatesExample.cs
//
//	An example script showing how to subscribe / unsubscribe to PoolKit's Despawner Events
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class DespawnerDelegatesExample : MonoBehaviour {

	[Header("Set your despawner here ...")]
	public Despawner despawner = null;

	[Header("Options")]
	public bool forceDespawnerToAllowEvents = true;	// <- Optional!
	public bool showDebugMessagesOnDespawn = true;
	public bool showDebugMessagesOnCollisions = true;
	public bool showDebugMessagesOnChainSpawn = true;

	// ==================================================================================================================
	//	AWAKE
	//	Use this when the gameobject is first created
	// ==================================================================================================================
	
	void Awake(){

		// SUBSCRIBE
		// Subscribe to the Despawner's events
		if( despawner != null ){

			// <Optional> Force the Despawner to allow despawner events
			if(forceDespawnerToAllowEvents){ despawner.enableDespawnerEvents = true; }

			// Subscribe
			despawner.onDespawnerDespawn += onDespawnerDespawn;
			despawner.onDespawnerCollided += onDespawnerCollided;
			despawner.onDespawnerChainSpawn += onDespawnerChainSpawn;
		}
	}

	// ==================================================================================================================
	//	ON DESTROY
	//	Use this when the gameobject is about to be destroyed
	// ==================================================================================================================
	
	void OnDestroy(){

		// UNSUBSCRIBE
		// Unsubscribe from the Despawner's events
		if( despawner != null ){

			despawner.onDespawnerDespawn -= onDespawnerDespawn;
			despawner.onDespawnerCollided -= onDespawnerCollided;
			despawner.onDespawnerChainSpawn -= onDespawnerChainSpawn;
		}
	}
	
	// ==================================================================================================================
	//	EVENTS
	//	These events will be called by the Despawner if subscribed ...
	// ==================================================================================================================
	
	// DESPAWNER JUST DESPAWNED!
	void onDespawnerDespawn(){
		if(showDebugMessagesOnDespawn){ Debug.Log("DESPAWNER DELEGATES EXAMPLE: " + despawner.gameObject.name + " has despawned!" ); }
	}

	// DESPAWNER JUST COLLIDED WITH A GAMEOBJECT
	void onDespawnerCollided( GameObject theCollidedGameObject ){
		if(showDebugMessagesOnCollisions){ 
			// You should always check to make sure the GameObject is not null
			if(theCollidedGameObject!=null){ Debug.Log("DESPAWNER DELEGATES EXAMPLE: " + despawner.gameObject.name + " Just collided with " + theCollidedGameObject.name ); }
		}
	}

	// DESPAWNER JUST CHAIN-SPAWNED AN INSTANCE
	void onDespawnerChainSpawn( Transform theInstance ){
		if(showDebugMessagesOnChainSpawn){ 
			// You should always check to make sure the instance is not null
			if(theInstance!=null){ Debug.Log("DESPAWNER DELEGATES EXAMPLE: " + despawner.gameObject.name + " Just Chain-Spawned " + theInstance.name ); }
		}
	}



}
