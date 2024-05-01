//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	PoolKit.cs
//	This is the Core controller of the PoolKit System
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Using the HellTap.PoolKit namespace
namespace HellTap.PoolKit {

	public static class PoolKit {

		// SETTINGS
		public static RenameFormat renameObjectsInPool = RenameFormat.NoRenaming;
			public enum RenameFormat{ 	
										EasyToReadObjectNameWithPoolKitAndIndex, 
										EasyToReadObjectNameWithIndex, 
										ObjectNameWithPoolKitAndIndex, 
										ObjectNameWithIndex, 
										NoRenaming 
									}
		public static bool onlyRenameObjectsInEditor = true;	// Optimizes builds but keeps things readable in the editor.

		// CACHED POOLS & SPAWNERS
		public static Pool[] pools = new Pool[0];				// A list of all the pools we are currently managing.
		public static Spawner[] spawners = new Spawner[0];		// A list of all the spawners in the scene

		// DEBUGGING
		public static bool debugPoolKit = false;

		// ==================================================================================================================
		//	AUTOMATICALLY CREATE GLOBAL POOLS
		//	Users can choose to automatically create global pools
		// ==================================================================================================================
		
		private static bool _createdGlobalPools = false;
		[RuntimeInitializeOnLoadMethod]
		public static void CreateGlobalPools(){

			// Only ever run this once
			if( _createdGlobalPools == false ){

				// Attempt to load the Global Pools ...
				GlobalPools globalPools = Resources.Load("PoolKit Global Pools") as GlobalPools;
				
				// If we loaded it create the pools, otherwise show a warning message
				if( globalPools != null ){ globalPools.Create(); }
				else { Debug.LogWarning("POOLKIT: Global Pools Data could not be found. Skipping ..."); }

				// Set a limiter to make sure we never run this twice
				_createdGlobalPools = true;
			}
		}

		// ==================================================================================================================
		//	REGISTER / UNREGISTER POOLS
		//	Individual pools must register themselves to PoolKit in order to be managed.
		//	Pools are registered when they are created / enabled, and unregistered when they are destroyed / disabled.
		// ==================================================================================================================
		
		// ADD POOL
		internal static void RegisterPool( Pool pool ){

			// Stop this method if the pool reference in empty
			if(pool==null){ 
				Debug.LogWarning("POOLKIT - Cannot Add an empty reference to the pools." );
				return; 
			}

			// Make sure there isn't another pool using the same name
			if( PoolExists(pool.poolName) ){
			//	Debug.LogWarning("POOLKIT - Cannot register pool. There is already another pool using the name: " + pool.poolName );
				return; 
			}
			
			// Add the Pool
			if( Arrays.AddItemIfNotPresent( ref pools, pool ) == true ){
				if(debugPoolKit){ Debug.Log("POOLKIT - Registered Pool: " + pool.gameObject.name ); }

			} else if(debugPoolKit){
				Debug.Log("POOLKIT - The Pool: " + pool.gameObject.name + " could not be registered. It may already be present.");
			}
		}

		// UNREGISTER POOL
		internal static void UnregisterPool( Pool pool ){
			
			// Stop this method if the pool reference in empty
			if(pool==null){ 
				Debug.Log("POOLKIT - Cannot unregister an empty reference from the pools list." );
				return; 
			}

			// Remove the pool
			if( Arrays.RemoveItem( ref pools, ref pool ) == true ){
				if(debugPoolKit){ Debug.Log("POOLKIT - unregister Pool: " + pool.gameObject.name ); }

			} else if(debugPoolKit){ 
				Debug.Log("POOLKIT - Could not unregister the Pool: " + pool.gameObject.name + ". It may have already been removed." );
			}
		}

		// ==================================================================================================================
		//	POOL EXISTS
		//	Checks to see if a pool can be found by name
		// ==================================================================================================================
		
		// POOL EXISTS (By Name)
		public static bool PoolExists( string poolName ){

			// If the Pool name sent is empty, return null right away
			if( poolName == System.String.Empty ){ return false; }
			
			// Loop through the pools and return true if a pool exists with the same name
			//for( int i = 0; i<PoolKit.pools.Length; i++ ){
			for( int i = PoolKit.pools.Length-1; i > -1; i-- ){		
				if( PoolKit.pools[i] != null && PoolKit.pools[i].poolName == poolName ){ return true; }
			}

			// If we didn't find a pool by that name, return false
			return false;
		}

		// ==================================================================================================================
		//	GET / FIND POOL
		//	Returns a pool by name. Standard version is "GetPool" but we also have a shorthand version called "Find".
		// ==================================================================================================================
		
		// Helper
		private static Pool[] allPoolsInScene = new Pool[0];

		// GET POOL (By Name)
		public static Pool Find( string poolName ){ return GetPool(poolName); }		// Short hand: PoolKit.Find("Enemies")
		public static Pool FindPool( string poolName ){ return GetPool(poolName); }	// variation: PoolKit.FindPool("Enemies")
		public static Pool GetPool( string poolName ){								// Standard version: PoolKit.GetPool("Enemies")
			
			// If the Pool name sent is empty, return null right away
			if( poolName == System.String.Empty ){ return null; }

			// Loop through the pools and return true if a pool exists with the same name
			//for( int i = 0; i<PoolKit.pools.Length; i++ ){
			for( int i = PoolKit.pools.Length-1; i > -1; i-- ){	
				if( PoolKit.pools[i] != null && PoolKit.pools[i].poolName == poolName ){ 
					return PoolKit.pools[i]; 
				}
			}

			// If the Pool is not currently cached, see if we can find it in the scene
			// NOTE: this should usually only happen OnAwake or on the very first OnEnable
			allPoolsInScene = Object.FindObjectsOfType( typeof(Pool) ) as Pool[];
			for( int i = 0; i < allPoolsInScene.Length; i++ ){
				if( allPoolsInScene[i] != null && allPoolsInScene[i].poolName == poolName ){ 
					return allPoolsInScene[i]; 
				}
			}

			// If we didn't find a pool by that name, return null
			if(debugPoolKit){ Debug.LogWarning("POOLKIT - Couldn't find a pool named: '" + poolName + "'" ); }
			return null;
		}

		// ==================================================================================================================
		//	GET / FIND POOL OF PREFAB
		//	Finds and returns a pool that contains a prefab matching a name / prefab reference.
		//	NOTE: This is more performance hungry than GetPool()
		// ==================================================================================================================
		
		// FIND POOL CONTAINING PREFAB (By Name)
		public static Pool FindPoolContainingPrefab( string prefabName ){ return GetPoolContainingPrefab(prefabName); }	
		public static Pool GetPoolContainingPrefab( string prefabName ){ // eg: PoolKit.GetPoolContaining("EnemyPrefab")
			
			// If the prefab name sent is empty, return null right away
			if( prefabName == System.String.Empty ){ return null; }

			// Loop through the pools...
			//for( int i = 0; i<PoolKit.pools.Length; i++ ){
			for( int i = PoolKit.pools.Length-1; i > -1; i-- ){	
				if( PoolKit.pools[i] != null ){

					// Loop through the pool's items ...
					//for( int pi = 0; pi < PoolKit.pools[i].poolItems.Length; pi++ ){
					for( int pi = PoolKit.pools[i].poolItems.Length-1; pi > -1; pi-- ){	

						// If this pool contains the prefab, return the pool
						if( PoolKit.pools[i].poolItems[pi].prefabToPool.name == prefabName ){
							return PoolKit.pools[i];
						}
					}
				}
			}

			// If the Pool is not currently cached, see if we can find it in the scene
			// NOTE: this should usually only happen OnAwake or on the very first OnEnable
			allPoolsInScene = Object.FindObjectsOfType( typeof(Pool) ) as Pool[];
			for( int i = 0; i < allPoolsInScene.Length; i++ ){
				if( allPoolsInScene[i] != null ){

					// Loop through the pool's items ...
					//for( int pi = 0; pi < PoolKit.pools[i].poolItems.Length; pi++ ){
					for( int pi = allPoolsInScene[i].poolItems.Length-1; pi > -1; pi-- ){	

						// If this pool contains the prefab, return the pool
						if( allPoolsInScene[i].poolItems[pi].prefabToPool.name == prefabName ){
							return allPoolsInScene[i];
						}
					}
				}
			}

			// If we still didn't find a pool by that name, return null
			if(debugPoolKit){ Debug.LogWarning("POOLKIT - Couldn't find a pool containing the prefab: '" + prefabName + "'" ); }
			return null;
		}

		// FIND POOL CONTAINING PREFAB (By Prefab / Transform)
		public static Pool FindPoolContainingPrefab( Transform prefab ){ 
			// If the prefab sent is null, return null right away
			if( prefab == null ){ return null; }
			return GetPoolContainingPrefab(prefab.gameObject); 
		}	
		public static Pool GetPoolContainingPrefab( Transform prefab ){ 
			// If the prefab sent is null, return null right away
			if( prefab == null ){ return null; }
			return GetPoolContainingPrefab(prefab.gameObject); 
		}

		// FIND POOL CONTAINING PREFAB (By Prefab / GameObject)
		public static Pool FindPoolContainingPrefab( GameObject prefab ){ return GetPoolContainingPrefab(prefab); }	
		public static Pool GetPoolContainingPrefab( GameObject prefab ){ // eg: PoolKit.GetPoolContainingPrefab("EnemyPrefab")
			
			// If the prefab sent is null, return null right away
			if( prefab == null ){ return null; }

			// Loop through the pools...
			//for( int i = 0; i<PoolKit.pools.Length; i++ ){
			for( int i = PoolKit.pools.Length-1; i > -1; i-- ){	
				if( PoolKit.pools[i] != null ){

					// Loop through the pool's items ...
					//for( int pi = 0; pi < PoolKit.pools[i].poolItems.Length; pi++ ){
					for( int pi = PoolKit.pools[i].poolItems.Length-1; pi > -1; pi-- ){	

						// If this pool contains the prefab, return the pool
						if( PoolKit.pools[i].poolItems[pi].prefabToPool == prefab ){
							return PoolKit.pools[i];
						}
					}
				}
			}

			// If the Pool is not currently cached, see if we can find it in the scene
			// NOTE: this should usually only happen OnAwake or on the very first OnEnable
			allPoolsInScene = Object.FindObjectsOfType( typeof(Pool) ) as Pool[];
			for( int i = 0; i < allPoolsInScene.Length; i++ ){
				if( allPoolsInScene[i] != null ){

					// Loop through the pool's items ...
					//for( int pi = 0; pi < PoolKit.pools[i].poolItems.Length; pi++ ){
					for( int pi = allPoolsInScene[i].poolItems.Length-1; pi > -1; pi-- ){	

						// If this pool contains the prefab, return the pool
						if( allPoolsInScene[i].poolItems[pi].prefabToPool == prefab ){
							return allPoolsInScene[i];
						}
					}
				}
			}

			// If we didn't find a pool by that name, return null
			if(debugPoolKit){ Debug.LogWarning("POOLKIT - Couldn't find a pool containing the prefab: '" + prefab.name + "'" ); }
			return null;
		}

		// ==================================================================================================================
		//	GET / FIND POOL OF INSTANCE
		//	Finds and returns a pool that contains a instance matching the GameObject reference.
		//	NOTE: This is more performance hungry than GetPool() and GetPoolContainingPrefab()
		// ==================================================================================================================
		
		// FIND POOL CONTAINING INSTANCE (By Transform)
		public static Pool FindPoolContainingInstance( Transform instance ){ return GetPoolContainingInstance(instance.gameObject);}
		public static Pool GetPoolContainingInstance( Transform instance ){ return GetPoolContainingInstance(instance.gameObject);}

		// FIND POOL CONTAINING INSTANCE (By GameObject)
		public static Pool FindPoolContainingInstance( GameObject instance ){ return GetPoolContainingInstance(instance); }	
		public static Pool GetPoolContainingInstance( GameObject instance ){ // eg: PoolKit.GetPoolContainingInstance(EnemyInstance)
			
			// If the isntance sent is null, return null right away
			if( instance == null ){ return null; }

			// Loop through the pools...
			//for( int i = 0; i<PoolKit.pools.Length; i++ ){
			for( int i = PoolKit.pools.Length-1; i > -1; i-- ){	
				if( PoolKit.pools[i] != null ){

					// If this is a Fixed Size pool, loop through the Spawned Objects Fixed Array
					if( PoolKit.pools[i].IsFixedSizePool() ){
						for( int so = PoolKit.pools[i].spawnedObjects.Length-1; so > -1; so-- ){	

							// If this pool contains the instance, return the pool
							if( PoolKit.pools[i].spawnedObjects[so].instance == instance ){
								return PoolKit.pools[i];
							}
						}
					
					// If this isn't a fixed size pool, loop through the Spawned Objects list ...
					} else {
						for( int so = PoolKit.pools[i].spawnedObjectsList.Count-1; so > -1; so-- ){	

							// If this pool contains the instance, return the pool
							if( PoolKit.pools[i].spawnedObjectsList[so].instance == instance ){
								return PoolKit.pools[i];
							}
						}

					}
				}
			}

			// If the Pool is not currently cached, see if we can find it in the scene
			// NOTE: this should usually only happen OnAwake or on the very first OnEnable
			allPoolsInScene = Object.FindObjectsOfType( typeof(Pool) ) as Pool[];
			for( int i = 0; i < allPoolsInScene.Length; i++ ){
				if( allPoolsInScene[i] != null ){

					// If this is a Fixed Size pool ...
					if( allPoolsInScene[i].IsFixedSizePool() ){
						
						// loop through the Spawned Objects Fixed Array
						for( int so = allPoolsInScene[i].spawnedObjects.Length-1; so > -1; so-- ){	

							// If this pool contains the instance, return the pool
							if( allPoolsInScene[i].spawnedObjects[so].instance == instance ){
								return allPoolsInScene[i];
							}
						}
					
					// If this isn't a fixed size pool...
					} else {

						// loop through the Spawned Objects list
						for( int so = allPoolsInScene[i].spawnedObjectsList.Count-1; so > -1; so-- ){	

							// If this pool contains the instance, return the pool
							if( allPoolsInScene[i].spawnedObjectsList[so].instance == instance ){
								return allPoolsInScene[i];
							}
						}
					}
				}
			}

			// If we didn't find a pool by that name, return null
			if(debugPoolKit){ Debug.LogWarning("POOLKIT - Couldn't find a pool containing the instance: '" + instance.name + "'" ); }
			return null;
		}


		// ==================================================================================================================
		//	REGISTER / UNREGISTER SPAWNERS
		//	Individual Spawners must register themselves to the PoolKit so users can easily find them through the API
		//	Spawners are registered when they are created / enabled, and unregistered when they are destroyed / disabled.
		//	NOTE: Spawners are allowed to have the same name, but PoolKit will only return the first matching object.
		// ==================================================================================================================
		
		// REGISTER SPAWNER
		internal static void RegisterSpawner( Spawner spawner ){

			// Stop this method if the spawner reference in empty
			if(spawner==null){ 
				Debug.LogWarning("POOLKIT - Cannot Add an empty reference to the spawners." );
				return; 
			}
			
			// Add the Spawner
			if( Arrays.AddItemIfNotPresent( ref spawners, spawner ) == true ){
				if(debugPoolKit){ Debug.Log("POOLKIT - Registered Spawner: " + spawner.gameObject.name ); }

			} else if(debugPoolKit){
				Debug.Log("POOLKIT - The Spawner: " + spawner.gameObject.name + " could not be registered. It may already be present.");
			}
		}

		// UNREGISTER SPAWNER
		internal static void UnregisterSpawner( Spawner spawner ){
			
			// Stop this method if the Spawner reference in empty
			if(spawner==null){ 
				Debug.Log("POOLKIT - Cannot unregister an empty reference from the spawners list." );
				return; 
			}

			// Remove the spawner
			if( Arrays.RemoveItem( ref spawners, ref spawner ) == true ){
				if(debugPoolKit){ Debug.Log("POOLKIT - unregister Spawner: " + spawner.gameObject.name ); }

			} else if(debugPoolKit){ 
				Debug.Log("POOLKIT - Could not unregister the Spawner: " + spawner.gameObject.name + ". It may have already been removed." );
			}
		}

		// ==================================================================================================================
		//	GET / FIND SPAWNER
		//	Returns a Spawner by name. Main version is "GetSpawner" but we also have another version called "FindSpawner".
		// ==================================================================================================================
		
		// Helper
		private static Spawner[] allSpawnersInScene = new Spawner[0];

		// GET SPAWNER (By Name) eg: ( PoolKit.FindSpawner("Rocks") || PoolKit.GetSpawner("Rocks") )
		public static Spawner FindSpawner( string spawnerName ){ return GetSpawner(spawnerName); }
		public static Spawner GetSpawner( string spawnerName ){
			
			// Loop through the spawner and return true if a pool exists with the same name
			//for( int i = 0; i<PoolKit.spawners.Length; i++ ){
			for( int i = PoolKit.spawners.Length-1; i > -1; i-- ){	
				if( PoolKit.spawners[i] != null && PoolKit.spawners[i].spawnerName == spawnerName ){ 
					return PoolKit.spawners[i]; 
				}
			}

			// If the Spawner is not currently cached, see if we can find it in the scene
			// NOTE: this should usually only happen OnAwake or on the very first OnEnable
			allSpawnersInScene = Object.FindObjectsOfType( typeof(Spawner) ) as Spawner[];
			for( int i = 0; i < allSpawnersInScene.Length; i++ ){
				if( allSpawnersInScene[i] != null && allSpawnersInScene[i].spawnerName == spawnerName ){ 
					return allSpawnersInScene[i]; 
				}
			}

			// If we didn't find a spawner by that name, return null
			if(debugPoolKit){ Debug.LogWarning("POOLKIT - Couldn't find a spawner named: '" + spawnerName + "'" ); }
			return null;
		}


		// ==================================================================================================================
		//	CREATE POOL
		//	Creates a new GameObject with a Pool component dynamically via script. Returns a reference to the new Pool.
		//	If a gameobject is supplied, the pool will be created on that instead.
		// ==================================================================================================================
		
		// Variation Method Names
		public static Pool Add( string poolName, Pool.PoolType poolType, bool enablePoolProtection, bool enablePoolEvents, bool dontDestroyOnLoad, PoolItem[] poolItems, GameObject usingGameObject = null ){
			return CreatePool( poolName, poolType, enablePoolProtection, enablePoolEvents, dontDestroyOnLoad, poolItems, usingGameObject );
		}
		public static Pool AddPool( string poolName, Pool.PoolType poolType, bool enablePoolProtection, bool enablePoolEvents, bool dontDestroyOnLoad, PoolItem[] poolItems, GameObject usingGameObject = null ){
			return CreatePool( poolName, poolType, enablePoolProtection, enablePoolEvents, dontDestroyOnLoad, poolItems, usingGameObject );
		}
		public static Pool Create( string poolName, Pool.PoolType poolType, bool enablePoolProtection, bool enablePoolEvents, bool dontDestroyOnLoad, PoolItem[] poolItems, GameObject usingGameObject = null ){
			return CreatePool( poolName, poolType, enablePoolProtection, enablePoolEvents, dontDestroyOnLoad, poolItems, usingGameObject );
		}

		// Create Pool method
		public static Pool CreatePool( string poolName, Pool.PoolType poolType, bool enablePoolProtection, bool enablePoolEvents, bool dontDestroyOnLoad, PoolItem[] poolItems, GameObject usingGameObject = null ){

			// Make sure we have at least one pool item setup
			if( poolItems == null || poolItems.Length == 0 ){ 
				Debug.LogWarning("POOLKIT - Creating a pool requires at least 1 PoolItem.");
				return null;
			}

			// Create or use the supplied GameObject
			if( usingGameObject == null){ usingGameObject = new GameObject(poolName); }

			// Setup the pool
			Pool pool = usingGameObject.AddComponent<Pool>();
			pool.poolName = poolName;
			pool.poolType = poolType;
			pool.enablePoolProtection = enablePoolProtection;
			pool.enablePoolEvents = enablePoolEvents;
			pool.dontDestroyOnLoad = dontDestroyOnLoad;
			pool.poolItems = poolItems;

			// We must now run StartFromAPI() on the pool
			pool.StartFromAPI();

			// Return the pool
			return pool;

		}


		// ==================================================================================================================
		//	DESPAWN ALL
		//	Despawns all instances from every pool
		// ==================================================================================================================

		// Core Method
		public static void DespawnAll(){

			// Loop through the pools and destroy them
			//for( int i = 0; i<PoolKit.pools.Length; i++ ){
			for( int i = PoolKit.pools.Length-1; i > -1; i-- ){	
				if( PoolKit.pools[i] != null ){ PoolKit.pools[i].DespawnAll(); }
			}
		}

		// ==================================================================================================================
		//	DESPAWN ALL LOCAL POOLS
		//	Despawns all instances from every local pool
		// ==================================================================================================================

		// Core Method
		public static void DespawnAllLocal(){ DespawnAllLocalPools(); }
		public static void DespawnAllLocalPools(){

			// Loop through the pools and destroy them
			//for( int i = 0; i<PoolKit.pools.Length; i++ ){
			for( int i = PoolKit.pools.Length-1; i > -1; i-- ){	
				if( PoolKit.pools[i] != null && PoolKit.pools[i].IsGlobalPool() == false ){ 
					PoolKit.pools[i].DespawnAll(); 
				}
			}
		}

		// ==================================================================================================================
		//	DESPAWN ALL GLOBAL POOLS
		//	Despawns all instances from every global pool
		// ==================================================================================================================

		// Core Method
		public static void DespawnAllGlobal(){ DespawnAllGlobalPools(); }
		public static void DespawnAllGlobalPools(){

			// Loop through the pools and destroy them
			//for( int i = 0; i<PoolKit.pools.Length; i++ ){
			for( int i = PoolKit.pools.Length-1; i > -1; i-- ){	
				if( PoolKit.pools[i] != null && PoolKit.pools[i].IsGlobalPool() == true ){ 
					PoolKit.pools[i].DespawnAll(); 
				}
			}
		}

		// ==================================================================================================================
		//	DESTROY POOL
		//	Attempts to find and destroy a specific Pool (and it's GameObject by name )
		// ==================================================================================================================
		
		// Variation Method Names
		public static bool Remove( string poolName ){ return DestroyPool(poolName); }
		public static bool RemovePool( string poolName ){ return DestroyPool(poolName); }

		// Core Method
		public static bool DestroyPool( string poolName ){

			// Attempt to find the Pool ...
			Pool poolToDestroy = GetPool( poolName );

			// If it exists, destroy it and return true...
			if( poolToDestroy != null ){

				Object.Destroy( poolToDestroy.gameObject );
				return true;

			// Otherwise, show a warning and return false
			} else {

				if(debugPoolKit){ Debug.LogWarning("POOLKIT - A pool named '" + poolName + "' couldn't be destroyed because it couldn't be found."); }
				return false;
			}
		}

		// ==================================================================================================================
		//	DESTROY ALL POOLS
		//	Goes through the internal pool and list and attempts to destroy them all
		// ==================================================================================================================
		
		// Variation Method Names
		public static void RemoveAll(){ DestroyAllPools(); }
		public static void RemoveAllPools(){ DestroyAllPools(); }
		public static void DestroyAll(){ DestroyAllPools(); }

		// Core Method
		public static void DestroyAllPools(){

			// Loop through the pools and destroy them
			//for( int i = 0; i<PoolKit.pools.Length; i++ ){
			for( int i = PoolKit.pools.Length-1; i > -1; i-- ){	
				if( PoolKit.pools[i] != null ){
					Object.Destroy( PoolKit.pools[i].gameObject );
				}
			}
		}

		// ==================================================================================================================
		//	DESTROY ALL LOCAL POOLS
		//	Goes through the internal pool and list and attempts to destroy all local pools
		// ==================================================================================================================
		
		// Variation Method Names
		public static void RemoveAllLocal(){ DestroyAllLocalPools(); }
		public static void RemoveAllLocalPools(){ DestroyAllLocalPools(); }
		public static void DestroyAllLocal(){ DestroyAllLocalPools(); }

		// Core Method
		public static void DestroyAllLocalPools(){

			// Loop through the pools and destroy them only if they are local pools
			//for( int i = 0; i<PoolKit.pools.Length; i++ ){
			for( int i = PoolKit.pools.Length-1; i > -1; i-- ){	
				if( PoolKit.pools[i] != null && PoolKit.pools[i].IsGlobalPool() == false ){
					Object.Destroy( PoolKit.pools[i].gameObject );
				}
			}
		}

		// ==================================================================================================================
		//	DESTROY ALL GLOBAL POOLS
		//	Goes through the internal pool and list and attempts to destroy all global pools
		// ==================================================================================================================
		
		// Variation Method Names
		public static void RemoveAllGlobal(){ DestroyAllGlobalPools(); }
		public static void RemoveAllGlobalPools(){ DestroyAllGlobalPools(); }
		public static void DestroyAllGlobal(){ DestroyAllGlobalPools(); }

		// Core Method
		public static void DestroyAllGlobalPools(){

			// Loop through the pools and destroy them only if they are global pools
			//for( int i = 0; i<PoolKit.pools.Length; i++ ){
			for( int i = PoolKit.pools.Length-1; i > -1; i-- ){	
				if( PoolKit.pools[i] != null && PoolKit.pools[i].IsGlobalPool() == true ){
					Object.Destroy( PoolKit.pools[i].gameObject );
				}
			}
		}

		// ==================================================================================================================
		//	GLOBAL INSTANTIATION AND DESTROY OVERRIDES
		//
		//	This section allows avanced users to override how objects are instantiated by using delegates.
		//	By default, objects are instantiated using Object.Instantiate(obj,pos,rot). Otherwise, the delegate will be
		//	used in its place.
		//
		//	NOTE: Individual pool items must be individually selected to allow "Enable Instantiate Delegates" and
		//	"Enable Destroy Delegates" to work.
		// ==================================================================================================================
		
		// -----------------
		//	CREATE INSTANCE
		// -----------------

		// If at least one delegate is added to PoolKit.OnCreateInstance, that will be used instead!
		public delegate GameObject CreateInstanceDelegate( GameObject prefab );
		public static CreateInstanceDelegate OnCreateInstance;
	
		// METHOD: Create Prefab Instance
		internal static GameObject InstantiatePrefab( GameObject prefab ){

			// If the user has setup delegates, intercept the instantiation here
			if (PoolKit.OnCreateInstance != null){
				return PoolKit.OnCreateInstance( prefab );
			
			// Otherwise, use the default Unity way ( Object.Instantiate() )
			} else {
				return Object.Instantiate( prefab ) as GameObject;
			}
		}

		// ------------------
		//	DESTROY INSTANCE
		// ------------------

		// If at least one delegate is added to PoolKit.OnDestroyInstance, that will be used instead!
		public delegate void DestroyInstanceDelegate( GameObject instance );
		public static DestroyInstanceDelegate OnDestroyInstance;
		
		// METHOD: Destroy Prefab Instance
		internal static void DestroyInstance( GameObject instance ){
			
			// If the user has setup delegates, override the destroy method here
			if (PoolKit.OnDestroyInstance != null){
				PoolKit.OnDestroyInstance( instance );
			
			// Otherwise, use the default Unity way ( Object.Destroy() )
			} else {
				Object.Destroy( instance );
			}
		}

	}
}
