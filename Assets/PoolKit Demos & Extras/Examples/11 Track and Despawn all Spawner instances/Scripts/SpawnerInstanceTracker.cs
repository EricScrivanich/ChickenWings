//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	SpawnerInstanceTracker.cs
//
//	An example script to track a spawner's instances so you can easily despawn all instances it has created.
//	Simply call:	DespawnAllInstancesInList() on this component to do so!
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class SpawnerInstanceTracker : MonoBehaviour {

	[Header("Find Your Spawner By Name")]
	public string spawnerName = "SPAWNER";

	[Header("Debug (Editor Only)")]
	public bool despawnInstancesWithSpaceBar = true;

	[Header("Tracker List (Debug)")]
	[SerializeField] private List<GameObject> spawnedInstances = new List<GameObject>();

	// Helpers
	Spawner spawner = null;
	List<int> referencesToRemove = new List<int>();
	int _listCount = 0;
	Pool _poolOfInstance = null;

	// ==================================================================================================================
	//	ON ENABLE
	//	Use this when the gameobject has been enabled
	// ==================================================================================================================
	
	void OnEnable(){

		// Clear the list
		spawnedInstances.Clear();

		// Cache the Spawner
		if(spawner==null){ spawner = PoolKit.GetSpawner( spawnerName ); }

		// Subscribe to the Spawner's OnSpawnerSpawn event
		if( spawner != null ){

			// Force the Spawner to allow spawner events
			spawner.enableSpawnerEvents = true;

			// Subscribe
			spawner.onSpawnerSpawn += onSpawnerSpawn;
		}
	}

	// ==================================================================================================================
	//	ON DISABLE
	//	Use this when the gameobject has been disabled
	// ==================================================================================================================
	
	void OnDisable(){

		// Unsubscribe from the Spawner's events
		if( spawner != null ){ spawner.onSpawnerSpawn -= onSpawnerSpawn; }
	}
	
	// ==================================================================================================================
	//	ON SPAWNER SPAWN
	//	This is sent by the spawner every time a new instance is spawned
	// ==================================================================================================================
	
	void onSpawnerSpawn( Transform theInstance ){
	
		// You should always check to make sure the instance is not null (this can happen if the pool reached its limit)
		if( theInstance != null ){ AddToListIfNotAlreadyIncluded( theInstance.gameObject ); }
		
	}

	// ==================================================================================================================
	//	ADD TO LIST IF NOT ALREADY INCLUDED
	//	Adds a GameObject to the spawnedInstances list if it is not already being tracked
	// ==================================================================================================================
	
	void AddToListIfNotAlreadyIncluded( GameObject go ){

		// Cache the list count
		_listCount = spawnedInstances.Count;

		// Loop through the spanwedInstances list
		for( int i = 0; i < _listCount; i++ ){

			// If we find that the object already exists, end now.
			if( go == spawnedInstances[i] ){ return; }
		}

		// If we didn't find the object already being tracked in the list, add it.
		spawnedInstances.Add( go );
	}


	// ==================================================================================================================
	//	UPDATE
	//	If the GameObject gets de-spawned from elsewhere, we need to update the spawnedInstances list.
	// ==================================================================================================================
		
	void Update(){

		// Debug This with the space bar
		#if UNITY_EDITOR
			if( despawnInstancesWithSpaceBar ){
				if( Input.GetKeyDown(KeyCode.Space) ){
					DespawnAllInstancesInList();
				}
			}
		#endif


		// Keep list updated
		AlwaysKeepListUpdated();
		
	}

	// ==================================================================================================================
	//	DESPAWN ALL INSTANCES IN LIST
	//	Tries to despawn all instances
	// ==================================================================================================================
	
	public void DespawnAllInstancesInList(){

		// Cache the list count
		_listCount = spawnedInstances.Count;

		// Loop through the spanwedInstances list
		for( int i = 0; i < _listCount; i++ ){

			// If this instance exists, and is active, despawn it
			if( spawnedInstances[i] != null &&
				spawnedInstances[i].activeInHierarchy == true && 
				spawnedInstances[i].activeSelf == true
			){
				
				// Cache the Pool of this spawned instance so we can despawn it
				_poolOfInstance =  PoolKit.FindPoolContainingInstance( spawnedInstances[i] );
				if(_poolOfInstance){ _poolOfInstance.Despawn( spawnedInstances[i] ); }
			}

		}

		// Clear the list when done
		spawnedInstances.Clear();
	}


	// ==================================================================================================================
	//	ALWAYS KEEP LIST UPDATED
	//	If the GameObject gets de-spawned from elsewhere, we need to update the spawnedInstances list.
	// ==================================================================================================================
	
	void AlwaysKeepListUpdated(){

		// Cache the list count
		_listCount = spawnedInstances.Count;

		// Loop through the spanwedInstances list
		for( int i = 0; i < _listCount; i++ ){

			// If this instance doesn't exist any more, or is already inactive (despawned), remove it from the list
			if( spawnedInstances[i] == null || 
				spawnedInstances[i].activeInHierarchy == false || 
				spawnedInstances[i].activeSelf == false 
			){
				referencesToRemove.Add(i);
			}
		}

		// Remove any items from the list if needed
		// NOTE: We do this backwards to make sure we don't break the list.
		for( int i = referencesToRemove.Count; i-->0;  ){
			referencesToRemove.RemoveAt(i);
		}
	}

}
