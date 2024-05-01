//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	SpawnerUnityEventsExample.cs
//
//	An example script showing how to use a PoolKit Spawner's UnityEvents.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class SpawnerUnityEventsExample : MonoBehaviour {

	[Header("Options")]
	public bool showDebugMessagesOnSpawn = true;
	public bool showOtherDebugMessages = true;
	const string spawnerName = " Spawner (UE) ";
	
	// ==================================================================================================================
	//	EVENTS
	//	These events will be called using the 
	// ==================================================================================================================
	
	// SPAWNER JUST SPAWNED AN INSTANCE
	public void onSpawnerSpawn( Transform theInstance ){
		if(showDebugMessagesOnSpawn){ 
			// You should always check to make sure the instance is not null (this can happen if the pool reached its limit)
			if(theInstance!=null){ Debug.Log("SPAWNER UNITYEVENTS EXAMPLE: " + spawnerName + " Just Spawned " + theInstance.name ); }
		}
	}

	// SPAWNER JUST STARTED
	public void onSpawnerStart(){
		if(showOtherDebugMessages){ Debug.Log("SPAWNER UNITYEVENTS EXAMPLE: " + spawnerName + " has Started!" ); }
	}

	// SPAWNER JUST STOPPED
	public void onSpawnerStop(){
		if(showOtherDebugMessages){ Debug.Log("SPAWNER UNITYEVENTS EXAMPLE: " + spawnerName + " has Stopped!" ); }
	}

	// SPAWNER JUST PAUSED
	public void onSpawnerPause(){
		if(showOtherDebugMessages){ Debug.Log("SPAWNER UNITYEVENTS EXAMPLE: " + spawnerName + " has paused!" ); }
	}

	// SPAWNER JUST RESUMED
	public void onSpawnerResume(){
		if(showOtherDebugMessages){ Debug.Log("SPAWNER UNITYEVENTS EXAMPLE: " + spawnerName + " has resumed!" ); }
	}

	// SPAWNER JUST ENDED
	public void onSpawnerEnd(){
		if(showOtherDebugMessages){ Debug.Log("SPAWNER UNITYEVENTS EXAMPLE: " + spawnerName + " has ended!" ); }
	}


}
