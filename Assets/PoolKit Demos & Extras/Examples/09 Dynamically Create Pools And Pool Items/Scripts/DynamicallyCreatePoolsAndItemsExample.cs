//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	DynamicallyCreatePoolsAndItemsExample.cs
//
//	An example script showing how to create Pools and Pool Items (prefab pools) by script. The script allows you to
//	either add the pool item to an existing pool, or to add it to an entirely new Pool created with the API.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HellTap.PoolKit;

public class DynamicallyCreatePoolsAndItemsExample : MonoBehaviour {

	// The Pool Item To Setup
	[Header("Pool Item Setup")]
	public GameObject prefab = null;
	public int poolSize = 10;
	public bool keepOrganized = false;
	public bool recycleSpawnedObjects = false;
	public bool useLazyPreloading = false;
	public bool enableAutoDespawn = true;

	// Find Pool
	[Header("Add Pool Item To Existing Pool")]
	public string poolToFind = "MyPool";
	Pool pool = null;

	// Create New Pool
	[Header("Create New Pool?")]
	public bool shouldWeCreateANewPool = false;
	public string poolName = "My New Pool";
	public Pool.PoolType poolType = Pool.PoolType.Automatic;
	public bool enablePoolProtection = false;
	public bool enablePoolEvents = false;
	public bool dontDestroyOnLoad = false;
	public bool useThisGameObjectAsPool = true;

	// Create New Pool
	[Header("Simple Spawn Test")]
	public bool startSpawningAfterSetup = true;
	Pool finalPool = null;

	// ==================================================================================================================
	//	START
	//	Use this for initialization
	// ==================================================================================================================
	
	void Start () {
	
		// Create a Pool Item
		PoolItem myPI = new PoolItem();		// NOTE: Users can take a look at PoolItem.cs to see the class in more detail!

		// Prefab, Scale and layer setup
		myPI.prefabToPool = prefab;									// The prefab we're going to pool
		myPI.spawnedItemScale = PoolItem.PoolScale.PrefabScale;		// 	How should we set the prefab's scale on spawn
		myPI.customSpawnScale = Vector3.one;						// If using a custom scale, set it here
		myPI.spawnedItemLayer = PoolItem.PoolLayer.PrefabLayer;		// 	How should we set the prefab's layer on spawn
		myPI.customSpawnLayer = 0;									// If using a custom layer, set it here

		// Options
		myPI.poolSizeOptions = PoolItem.PoolResizeOptions.KeepPoolSizeFixed;	// Pool Resizing Options
		myPI.poolSize = poolSize;							// The size of the pool at start
		myPI.limitPoolSize = 100;							// The max size of the pool if we're using ExpandableWithMaxLimit
		myPI.keepOrganized = keepOrganized;					// Should we reparent objects in the hierarchy?
		myPI.recycleSpawnedObjects = recycleSpawnedObjects;	// If we've hit the limit, recycle an existing active object

		// Preloading
		myPI.useLazyPreloading = useLazyPreloading;			// Should the pooled items be created gradually over time?
		myPI.lazyPreloadingInstancesOnAwake = 1;			// How many instances should be preloaded immediately on Awake?
		myPI.lazyPreloadingInitialDelay = 1f;				// Wait for lazyPreloadingDelay in seconds before starting
		myPI.lazyPreloadingInstancesPerPass = 1;			// How many instances should we create on every pass?
		myPI.lazyPreloadingDelayBetweenPasses = 0.2f;		// Do a pass every 0.2 seconds (or 5 times a second)

		// Auto Despawn
		myPI.enableAutoDespawn = enableAutoDespawn;			// Should spawned items be automatically despawned?
		myPI.despawnMode = PoolItem.DespawnMode.Countdown;	// The type of despawn mode to use
		myPI.despawnAfterHowManySeconds = 5f;				// After X seconds, the prefabPool will despawn an instance
		myPI.despawnRandomRangeMin = 2f;					// The minimum random time to wait before despawning
		myPI.despawnRandomRangeMax = 5f;					// The maximum random time to wait before despawning

		// PoolKit Listeners
		myPI.notifications = PoolItem.Notifications.PoolKitListeners; // Should we send OnSpawn() and OnDespawn() to PoolKit Listeners?
	
		// Delegates
		myPI.enableInstantiateDelegates = false;			// When an instance is being instantiated, use delegates?
		myPI.enableDestroyDelegates = false;				// When an instance is being destroyed, use delegates?

		// ================================
		//	ADD POOL ITEM TO EXISTING POOL
		// ================================

		// If we're not creating a new pool, lets add it to an existing one
		if( shouldWeCreateANewPool == false ){
			
			// Find the pool we're going to add the prefab to
			pool = PoolKit.Find(poolToFind);

			// If we found the pool, add the Pool Item to it
			if(pool != null){ pool.Add( myPI ); }

			// For demo spawning to work, we just copy the pool reference
			finalPool = pool;
		
		// ================================
		//	ADD POOL ITEM TO NEW POOL
		// ================================

		// Create a new pool entirely
		} else if ( shouldWeCreateANewPool == true ){

			// Add Pool Item To An Entirely New Pool
			finalPool = PoolKit.CreatePool (
				poolName,											// Name of the new pool
				poolType,											// The type of the new pool ( eg, PoolType.Automatic )
				enablePoolEvents,									// Should we allow delegates to access this pool?
				enablePoolProtection,								// Should we enable Pool Protection?
				dontDestroyOnLoad,									// Don't destroy this pool when we're changing scenes
				new PoolItem[]{ myPI },								// A list of Pool Items
				useThisGameObjectAsPool ? gameObject : null			// What GameObject to add the pool to? Null = new.
			);
		}

		InvokeRepeating("Spawn", 1f, 1f);
	}
	

	public void Spawn(){

		if( finalPool != null ){ 
			finalPool.Spawn( prefab ); 
		}
	}

}
