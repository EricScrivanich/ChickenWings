//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	PoolDelegatesInstantiateDestroyExample.cs
//
//	An example script showing how to subscribe / unsubscribe to PoolKit's Pool Instantiate and Destroy delegates.
//	You can use this if you need control over how objects are instantiated or destroyed. Overrding can take place
//	at the Pool level, or globally for all pools.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class PoolDelegatesInstantiateDestroyExample : MonoBehaviour {

	[Header("Find Your Pool By Prefab")]
	public GameObject prefabToOverride = null;
	Pool findPool;

	[Header("Options")]
	public bool useGlobalPoolKitOverride = false;
	public bool forcePoolToAllowDelegates = true;
	public bool debugSpawnedInstances = true;
	public bool debugDespawnedInstances = true;
	public bool showDebugMessages = true;
	
	// ==================================================================================================================
	//	ON ENABLE
	//	Use this when the gameobject has been enabled
	// ==================================================================================================================
	
	void OnEnable(){

		// ===================================
		//	OVERRIDE EVENTS AT THE POOL LEVEL
		// ===================================

		if( useGlobalPoolKitOverride == false ){
		
			// Find the pool (if we haven't already)
			if(findPool==null){ findPool = PoolKit.GetPoolContainingPrefab( prefabToOverride ); }
			
			// If we have found the pool, subscribe to the events
			if(findPool!=null ){

				// < Optional > Find the Pool Item containing the prefab we want to override, and make sure
				// it is turned on ... This is useful just in case we didn't enable it in the Pool's inspector!
				if(forcePoolToAllowDelegates){
					PoolItem pi = findPool.GetPoolItem( prefabToOverride );
					if(pi!=null){
						pi.enableInstantiateDelegates = true;
						pi.enableDestroyDelegates = true;
					}
				}

				// Subscribe to the events
				findPool.OnCreateInstance += OnCreateInstance;
				findPool.OnDestroyInstance += OnDestroyInstance;
			}

		// ====================================
		//	OVERRIDE EVENTS AT THE GLOBAL LEVEL
		// ====================================

		} else {

			// < Optional > Find the Pool Item containing the prefab we want to override, and make sure
			// it is turned on ... This is useful just in case we didn't enable it in the Pool's inspector!
			if(forcePoolToAllowDelegates){
			
				// Find the pool containing the prefab we want to track ...
				if(findPool==null){ findPool = PoolKit.GetPoolContainingPrefab( prefabToOverride ); }

				// If we fond the pool, make sure the instantiate and destroy delegates are enabled for the prefab (PoolItem)
				if( findPool != null ){
					PoolItem pi = findPool.GetPoolItem( prefabToOverride );
					if(pi!=null){
						pi.enableInstantiateDelegates = true;
						pi.enableDestroyDelegates = true;
					}
				}
			}

			// NOTE: We could skip the above code and just use this if we know the inspectors have been setup correctly!
			PoolKit.OnCreateInstance += OnCreateInstance;
			PoolKit.OnDestroyInstance += OnDestroyInstance;

		}
	}

	// ==================================================================================================================
	//	ON DISABLE
	//	Use this when the gameobject has been disabled
	// ==================================================================================================================
	
	void OnDisable(){

		// ===================================
		//	OVERRIDE EVENTS AT THE POOL LEVEL
		// ===================================

		if( useGlobalPoolKitOverride == false ){

			// If we found the pool, unsubscribe from the delegates
			if(findPool!=null){

				// Unsubscribe from the events
				findPool.OnCreateInstance -= OnCreateInstance;
				findPool.OnDestroyInstance -= OnDestroyInstance;
			}

		// ====================================
		//	OVERRIDE EVENTS AT THE GLOBAL LEVEL
		// ====================================

		} else {

			// Unsubscribe from the events
			PoolKit.OnCreateInstance -= OnCreateInstance;
			PoolKit.OnDestroyInstance -= OnDestroyInstance;
		}	
	}

	// ==================================================================================================================
	//	EVENTS
	//	These events will be called if a new instance is instantiated or destroyed
	// ==================================================================================================================
	
	// Event called when we must instantiate a new instance
	GameObject OnCreateInstance( GameObject prefab ) {
		if ( prefab != null ){ 

			// Show a message in the console (NOTE: this uses GC!)
			if(showDebugMessages){ 
				Debug.Log("POOL DELEGATE EXAMPLE: Instantiating instance of prefab: " + prefab.name + " via delegate!"); 
			}
			
			// NOTE: Make sure to return the created GameObject!
			return Instantiate( prefab );
		}

		// Return null if something goes wrong
		return null;
	}

	// Event called when the pool destroys an object
	void OnDestroyInstance( GameObject instance ) {
		if ( instance != null ){ 
			
			// Show a message in the console (NOTE: this uses GC!)
			if(showDebugMessages){ 
				Debug.Log("POOL DELEGATE EXAMPLE: Destroying instance: " + instance.name + " via delegate!"); 
			}

			Destroy(instance);
		}
	}
}
