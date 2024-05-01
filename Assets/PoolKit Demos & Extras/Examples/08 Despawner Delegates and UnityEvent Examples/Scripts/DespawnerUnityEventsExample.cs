//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	DespawnerUnityEventsExample.cs
//
//	An example script showing how to use a PoolKit Despawner's UnityEvents.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class DespawnerUnityEventsExample : MonoBehaviour {

	[Header("Options")]
	public bool showDebugMessagesOnDespawn = true;
	public bool showDebugMessagesOnCollisions = true;
	public bool showDebugMessagesOnChainSpawn = true;
	const string despawnerName = " Despawner (UE) ";
	
	// ==================================================================================================================
	//	EVENTS
	//	These events will be called by the Despawner if using UnityEvents
	// ==================================================================================================================
	
	// DESPAWNER JUST DESPAWNED!
	public void onDespawnerDespawn(){
		if(showDebugMessagesOnDespawn){ Debug.Log("DESPAWNER UNITYEVENTS EXAMPLE: " + despawnerName + " has despawned!" ); }
	}

	// DESPAWNER JUST COLLIDED WITH A GAMEOBJECT
	public void onDespawnerCollided( GameObject theCollidedGameObject ){
		if(showDebugMessagesOnCollisions){ 
			// You should always check to make sure the GameObject is not null
			if(theCollidedGameObject!=null){ Debug.Log("DESPAWNER UNITYEVENTS EXAMPLE: " + despawnerName + " Just collided with " + theCollidedGameObject.name ); }
		}
	}

	// DESPAWNER JUST CHAIN-SPAWNED AN INSTANCE
	public void onDespawnerChainSpawn( Transform theInstance ){
		if(showDebugMessagesOnChainSpawn){ 
			// You should always check to make sure the instance is not null
			if(theInstance!=null){ Debug.Log("DESPAWNER UNITYEVENTS EXAMPLE: " + despawnerName + " Just Chain-Spawned " + theInstance.name ); }
		}
	}


}
