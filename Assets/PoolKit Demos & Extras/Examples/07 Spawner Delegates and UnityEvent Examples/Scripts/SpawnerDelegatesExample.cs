//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	SpawnerDelegatesExample.cs
//
//	An example script showing how to subscribe / unsubscribe to PoolKit's Spawner Events
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class SpawnerDelegatesExample : MonoBehaviour {

	[Header("Find Your Spawner By GameObject Name")]
	public string spawnerName = "SPAWNER";
	Spawner spawner = null;

	[Header("Options")]
	public bool forceSpawnerToAllowEvents = true;	// <- Optional!
	public bool showDebugMessagesOnSpawn = true;
	public bool showOtherDebugMessages = true;

	// ==================================================================================================================
	//	ON ENABLE
	//	Use this when the gameobject has been enabled
	// ==================================================================================================================
	
	void OnEnable(){

		// Cache the Spawner
		if(spawner==null){ spawner = PoolKit.GetSpawner( spawnerName ); }

		// SUBSCRIBE
		// Subscribe to the Spawner's events
		if( spawner != null ){

			// <Optional> Force the Spawner to allow spawner events
			if(forceSpawnerToAllowEvents){ spawner.enableSpawnerEvents = true; }

			// Subscribe
			spawner.onSpawnerSpawn += onSpawnerSpawn;
			spawner.onSpawnerStart += onSpawnerStart;
			spawner.onSpawnerStop += onSpawnerStop;
			spawner.onSpawnerPause += onSpawnerPause;
			spawner.onSpawnerResume += onSpawnerResume;
			spawner.onSpawnerEnd += onSpawnerEnd;
		}
	}

	// ==================================================================================================================
	//	ON DISABLE
	//	Use this when the gameobject has been disabled
	// ==================================================================================================================
	
	void OnDisable(){

		// UNSUBSCRIBE
		// Unsubscribe from the Spawner's events
		if( spawner != null ){

			spawner.onSpawnerSpawn -= onSpawnerSpawn;
			spawner.onSpawnerStart -= onSpawnerStart;
			spawner.onSpawnerStop -= onSpawnerStop;
			spawner.onSpawnerPause -= onSpawnerPause;
			spawner.onSpawnerResume -= onSpawnerResume;
			spawner.onSpawnerEnd -= onSpawnerEnd;
		}
	}
	
	// ==================================================================================================================
	//	EVENTS
	//	These events will be called by the Spawner if subscribed ...
	// ==================================================================================================================
	
	// SPAWNER JUST SPAWNED AN INSTANCE
	void onSpawnerSpawn( Transform theInstance ){
		if(showDebugMessagesOnSpawn){ 
			// You should always check to make sure the instance is not null (this can happen if the pool reached its limit)
			if(theInstance!=null){ Debug.Log("SPAWNER DELEGATES EXAMPLE: " + spawnerName + " Just Spawned " + theInstance.name ); }
		}
	}

	// SPAWNER JUST STARTED
	void onSpawnerStart(){
		if(showOtherDebugMessages){ Debug.Log("SPAWNER DELEGATES EXAMPLE: " + spawnerName + " has Started!" ); }
	}

	// SPAWNER JUST STOPPED
	void onSpawnerStop(){
		if(showOtherDebugMessages){ Debug.Log("SPAWNER DELEGATES EXAMPLE: " + spawnerName + " has Stopped!" ); }
	}

	// SPAWNER JUST PAUSED
	void onSpawnerPause(){
		if(showOtherDebugMessages){ Debug.Log("SPAWNER DELEGATES EXAMPLE: " + spawnerName + " has paused!" ); }
	}

	// SPAWNER JUST RESUMED
	void onSpawnerResume(){
		if(showOtherDebugMessages){ Debug.Log("SPAWNER DELEGATES EXAMPLE: " + spawnerName + " has resumed!" ); }
	}

	// SPAWNER JUST ENDED
	void onSpawnerEnd(){
		if(showOtherDebugMessages){ Debug.Log("SPAWNER DELEGATES EXAMPLE: " + spawnerName + " has ended!" ); }
	}


}
