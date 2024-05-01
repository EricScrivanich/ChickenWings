//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Pool.cs
//	This script allows you to setup a "Pool" of spawnable objects.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HellTap.PoolKit {

	public class Pool : MonoBehaviour {

		// ROOT TRANSFORM
		// Pool Root Transform ( this transform is used as the group to hold all pool items )
		Transform _poolItemRootTransform = null;			// This is the cached reference to this transform
		int _poolItemRootLayer = 0;							// The layer of this gameObject
		int _poolItemRootTransformInstanceID = -1;			// The Instance ID of the cached reference above
		
		// Pool Options
		[Header("Pool Options")]
		public string poolName = "My Pool Name";			// The name of this pool (users can search for it by this name)
		public PoolType poolType = PoolType.Automatic;		// Users can select which types of pool to use
			public enum PoolType { Automatic, FixedArray, DynamicList }
			bool _usingFixedSizePool = true;				// Fixed size poolItems are better for performance ( use Array[] )
			public bool IsFixedSizePool(){ return _usingFixedSizePool; }
		public bool poolWasJustCreatedByAPI = true;			// NOTE: This should automatically be turned off in the inspectors
			bool _poolReady = false;						// Stops initialization happening more than once
		public bool enablePoolEvents = false;				// Allows us to turn on Delegate Events with hardly any overhead
		public bool enablePoolProtection = true;			// In exchange for slightly slower pools, we can protect the pool from
															// users destroying instances by having the pool rebuild itself.
		public bool dontDestroyOnLoad = false;				// Make this pool persistant across scenes.

		// Global Pool Helper
		bool _isGlobalPool = false;							// A bool that stores the global status of the pool
		public bool IsGlobalPool(){ return _isGlobalPool; }	// A Public method to find out if the pool is global 

		// The Prefabs
		[Header("Pool Items")]
		public PoolItem[] poolItems = new PoolItem[0];		// Setup the different prefabs we'll be Pooling

		// The Spawned Objects (these are private fields but visible in the Editor for debugging)
		[Header("Spawned Objects")]
		[SerializeField] internal PoolKitInstance[] spawnedObjects = new PoolKitInstance[0];	// Used in Fixed Size Pool Mode ( Array[] )
		[SerializeField] internal List<PoolKitInstance> spawnedObjectsList;	// Used In Dynamic Size Pool Mode ( List<> )
			
		// ---------------------------------------------
		// EVENTS:
		// NOTE: Requires 'enablePoolEvents' to be true
		// ---------------------------------------------
		
		// DELEGATE: onPoolSpawn
		public delegate void OnPoolSpawnDelegate( Transform instance, Pool pool );		// Users can subscribe to events where 
		public OnPoolSpawnDelegate onPoolSpawn;											// this pool spawns an instance.

		// DELEGATE: onPoolDespawn
		public delegate void OnPoolDespawnDelegate( Transform instance, Pool pool );	// Users can subscribe to events where 
		public OnPoolDespawnDelegate onPoolDespawn;										// this pool despawns an instance.

		#if UNITY_EDITOR

			// Editor Helpers
			[Header("Editor Helpers")]
			public int tab = 0;															// Helps the Editor track what tab is open

		#endif

		
		// ==================================================================================================================
		//	START FROM API
		//	Because we can't use constructors on Monobehaviours, we use this as a workaround. We're doing this because
		//	Awake() is called before we have time to override the values when creating a pool dynamically. 
		//	NOTE: This is called from PoolKit.CreatePool()
		// ==================================================================================================================

		internal void StartFromAPI(){

			// Only allow this to be run if the pool was just created by the API
			if( poolWasJustCreatedByAPI ){

				// Turn off the default value
				poolWasJustCreatedByAPI = false;

				// Run Awake() and then OnEnable() in order
				Awake();
				OnEnable();
			}
		}


		// ==================================================================================================================
		//	ADD
		//	New Pool Items can be added at runtime
		// ==================================================================================================================

		public bool Add( PoolItem pi ){

			// Check if the new Pool Item is valid and contains a prefab
			if( pi != null && pi.prefabToPool != null ){

				// ---------------------
				// CHECK FOR DUPLICATES
				// ---------------------

				// Make sure the prefab isn't the same as any other in the pool
				for( int i = 0; i < poolItems.Length; i++ ){
					if( poolItems[i].prefabToPool == pi.prefabToPool ){
						Debug.LogWarning( "POOLKIT (Pool) - Could not add a new Pool Item because the prefab '" + pi.prefabToPool.name + "' already exists in the pool." );
						return false;
					}
				}

				// ---------------
				// INITIAL SETUP
				// ---------------

				// Pre-Cache all Prefab InstanceIDs in the poolItem items
				pi.prefabToPoolInstanceID = pi.prefabToPool.GetInstanceID();

				// Cache this pool
				pi.pool = this;

				#if UNITY_EDITOR
					// Reset the statistics in the editor
					pi.maxInstancesSpawnedAtOnce = 0;
				#endif

				// -------------------------------------
				// MAKE COMPATIBLE WITH FIXED SIZE POOL
				// -------------------------------------

				if( _usingFixedSizePool == true ){
					
					// Don't allow Lazy Preloading
					if( pi.useLazyPreloading ){
						Debug.LogWarning( "POOLKIT (Pool) - You cannot add a Pool Item with Lazy Preloading to a fixed sized pool! Preloading will be disabled." );
						pi.useLazyPreloading = false;
					}

					// Don't Allow anything other than Fixed Size Pools
					if( pi.poolSizeOptions != PoolItem.PoolResizeOptions.KeepPoolSizeFixed ){
						Debug.LogWarning( "POOLKIT (Pool) - You can only add Pool Items that 'KeepPoolSizeFixed' to a fixed sized Pool. This will be automatically changed." );
						pi.poolSizeOptions = PoolItem.PoolResizeOptions.KeepPoolSizeFixed;
					}
				}
				
				// ADD THE POOL TO THE POOL ITEMS LIST
				Arrays.AddItem( ref poolItems, pi );

				// Set pi to the reference in PoolItems
				pi = poolItems[ poolItems.Length - 1 ];

				// -------------------------------------
				// INSTANTIATE THE NEW ITEMS
				// -------------------------------------

				// INSTANTIATE NEW ITEMS WHEN USING FIXED SIZE POOL ...
				if( _usingFixedSizePool == true ){

					// Setup the next Array index to use (it will be the current length of the spawned object array)
					fixedSizeSpawnCount = spawnedObjects.Length;

					// Make the SpawnedObjects Array bigger to fit in the new entries ...
					spawnedObjects = Arrays.Combine( spawnedObjects, new PoolKitInstance[pi.poolSize] );

					// Then, Proceed to create all of the objects immediately	
					for ( int i = 0; i < pi.poolSize; i++){
						InstantiatePooledObject( pi, fixedSizeSpawnCount );	// <- 2nd argument is the slot of the array
						fixedSizeSpawnCount++;
					}

				// INSTANTIATE NEW ITEMS WHEN USING A DYNAMIC LIST ...
				} else {

					// Should we use Lazy Preloading?
					if( pi.poolSize > 0 && pi.useLazyPreloading == true ){
						
						// Turn on the preloading update, and do the first loop right away
						pi.runPreloadingUpdate = true;
						pi.PreloadUpdate();

					// Otherwise, Preload all of the object immediately	
					} else {

						for ( int i = 0; i < pi.poolSize; i++){
							InstantiatePooledObject( pi, -1 );	// <- Notice that there is NOT a second argument here
						}
					}
				}
			}

			// If the Pool Item couldn't be added, return false
			return false;
		}


		// ==================================================================================================================
		//	AWAKE
		//	We setup the pool on Awake() so that preloaded objects are available before Start()
		// ==================================================================================================================

		// Helpers
		[SerializeField] int fixedSizeSpawnCount = 0;

		// Method
		void Awake(){ 

			// If we've created the Pool via the API, it needs to be setup first!
			if(poolWasJustCreatedByAPI){ return; }

			// ==========================
			//	EARLY SETUP
			// ==========================

			// Only do this once!
			if(_poolReady==true){ return; }
			
			// Cache the root transform where we'll keep instances organized in the hierarchy
			_poolItemRootTransform = transform;
			_poolItemRootLayer = gameObject.layer;
			_poolItemRootTransformInstanceID = _poolItemRootTransform.GetInstanceID();

			// ===========================
			//	SETUP GLOBAL / LOCAL POOL
			// ===========================

			// Allow Setting Don't Destroy On Load if this doesn't have a parent
			if( dontDestroyOnLoad == true && transform.parent == null ){ 
				
				DontDestroyOnLoad( gameObject );
				_isGlobalPool = true;

			// If we're using a Global Pool Group (with PoolKitSetup), mark as global but dont use DontDestroyOnLoad
			} else if ( transform.parent != null && transform.parent.GetComponent<PoolKitSetup>() != null && transform.parent.GetComponent<PoolKitSetup>().dontDestroyOnLoad ){
				
				dontDestroyOnLoad = true;	// <- The PoolKitSetup options overrides this, so we should set it to true
				_isGlobalPool = true;
			
			// This is an unsupported configuration
			} else if( dontDestroyOnLoad == true ){
				
				Debug.LogWarning("POOLKIT: The Pool " + poolName + " is set to be global but is not configured correctly. It must either not have a parent Transform or be the child of a GameObject with a PoolKitSetup component configured as a 'Global Pool Group'.");
				_isGlobalPool = false;
			}

			// ==========================
			//	REGISTER POOL
			// ==========================

			// Register Pool on awake so all pools are setup before the Start method
			PoolKit.RegisterPool(this);

			// ==========================
			//	POOL TYPE SETUP
			// ==========================

			// If user selected Fixed Array specifically, set it
			if( poolType == PoolType.FixedArray ){
				
				if( CanUseFixedArray() == true ){
					_usingFixedSizePool = true;
				} else {
					Debug.LogWarning("POOLKIT (Pool) - The pool '" + poolName + "' was setup to be a fixed array but is using features that are only compatible with the 'Dynamic List' mode (such as preloading or creating new instances on demand). This pool has been automatically converted to a Dynamic List so functionality can continue.");
					_usingFixedSizePool = false;
				}

			// If user selected Dynamic List specifically, set it
			} else if( poolType == PoolType.DynamicList ){
				_usingFixedSizePool = false;

			// If user selected Automatic, figure out what they need!
			} else if( poolType == PoolType.Automatic ){

				// If we can use a fixed array, do it!
				if( CanUseFixedArray() == true ){
					_usingFixedSizePool = true;

				// Otherwise, use the List	
				} else {
					_usingFixedSizePool = false;
				}
			}

			// ==========================
			//	PRE-CALCULATED VARIABLES
			// ==========================

			// Loop through the pool items ...
			for (int i = 0; i < poolItems.Length; i++ ){
				if ( poolItems[i] != null ){
				
					// Pre-Cache all Prefab InstanceIDs in the poolItem items
					if( poolItems[i].prefabToPool != null ){
						poolItems[i].prefabToPoolInstanceID = poolItems[i].prefabToPool.GetInstanceID();
					}  else {
						poolItems[i].prefabToPoolInstanceID = -1;	// -1 means that no prefab was found in this case.
					}

					// Cache this pool
					poolItems[i].pool = this;

					#if UNITY_EDITOR
						// Reset the statistics in the editor
						poolItems[i].maxInstancesSpawnedAtOnce = 0;
					#endif

				}
			}


			// =======================
			//	SETUP FIXED SIZE POOL
			// =======================

			if( _usingFixedSizePool == true ){

				// Create an empty array with a precreated number of empty entries (based on total preloaded counts)
				spawnedObjects = new PoolKitInstance[ CalculateHowLongThePreloadedArrayShouldBe() ];

				// Reset the fixed Size Spawn Count
				fixedSizeSpawnCount = 0;

				// Loop through all the item poolItems and preload all needed instances
				for (int a = 0; a < poolItems.Length; a++ ){	

					// Lazy Preloading is NOT allowed on Fixed Size Pools!
					if( poolItems[a].useLazyPreloading == true ){

						// Turn it off
						poolItems[a].useLazyPreloading = false;

						// Show a warning message
						Debug.LogWarning("POOLKIT (Pool): You have enabled lazy preloading on the Fixed Size Pool named '" + poolName + "' but this is not allowed. Fixed size pools cannot be changed at runtime. To remove this warning, disable 'Use Lazy Preloading' or don't use a fixed size pool.");
					}

					// Then, Proceed to preload all of the objects immediately	
					for ( int b = 0; b < poolItems[a].poolSize; b++){
						InstantiatePooledObject( poolItems[a], fixedSizeSpawnCount );	// <- 2nd argument is the slot of the array
						fixedSizeSpawnCount++;
					}
				}

			// =======================
			//	SETUP LIST POOL
			// =======================

			} else {

				// Clear the poolItemed objects list
				spawnedObjectsList = new List<PoolKitInstance>();

				// Loop through all the item poolItems and preload all needed instances
				for (int a = 0; a < poolItems.Length; a++ ){

					// Should we use Lazy Preloading?
					if( poolItems[a].poolSize > 0 && poolItems[a].useLazyPreloading == true ){
						
						// Turn on the preloading update, and do the first loop right away
						poolItems[a].runPreloadingUpdate = true;
						poolItems[a].PreloadUpdate();

					// Otherwise, Preload all of the object immediately	
					} else {

						for ( int b = 0; b < poolItems[a].poolSize; b++){
							InstantiatePooledObject( poolItems[a], -1 );	// <- Notice that there is NOT a second argument here
						}
					}
				}
			}

			// =======================
			//	MARK POOL AS READY
			// =======================

			_poolReady = true;	
		}

		// ==================================================================================================================
		//	CAN USE FIXED ARRAY?
		//	Use this when the gameobject has been enabled
		// ==================================================================================================================
		
		bool CanUseFixedArray(){
			
			// Loop through the pool items and look for features that require a dynamic list
			for (int i = 0; i < poolItems.Length; i++ ){
				
				// If the pool item is setup to expand its instances on demand or use lazy preloading, return false!
				if ( poolItems[i] != null &&
					(
						//poolItems[i].expandPoolWhenNeeded ||
						poolItems[i].poolSizeOptions == PoolItem.PoolResizeOptions.AlwaysExpandPoolWhenNeeded ||
						poolItems[i].poolSizeOptions == PoolItem.PoolResizeOptions.ExpandPoolWithinLimit ||
						poolItems[i].useLazyPreloading
					)
				){
					return false;
				}
			}

			// No features are being used that are incompatible with a Fixed Array!
			return true;
		}

		// ==================================================================================================================
		//	ON ENABLE
		//	Use this when the gameobject has been enabled
		// ==================================================================================================================
		
		void OnEnable() {

			// If we've created the Pool via the API, it needs to be setup first!
			if(poolWasJustCreatedByAPI){ return; }
			
			// Register this pool with PoolKit
			PoolKit.RegisterPool(this);
		}

		// ==================================================================================================================
		//	ON DISABLE
		//	Use this when the gameobject has been disabled
		// ==================================================================================================================
		
		void OnDisable(){

			// Unregister this pool with PoolKit
			PoolKit.UnregisterPool(this);
		}

		// ==================================================================================================================
		//	ON DESTROY
		//	Use this when this component / gameObject is destroyed
		// ==================================================================================================================
		
		void OnDestroy(){

			// Stop any Coroutines when destroying
			StopAllCoroutines();

			// Destroy all instances from the Spawned Objects Array
			if( spawnedObjects != null ){
				for( int so = 0; so < spawnedObjects.Length; so++ ){
					if( spawnedObjects[so]!=null ){ 
						if(spawnedObjects[so].instance!=null){ 

							// Destroy normally
							if( spawnedObjects[so].poolItem.enableDestroyDelegates == false ){
								Destroy(spawnedObjects[so].instance); 
							
							// Destroy using delegates
							} else {
								DestroyInstance(spawnedObjects[so].instance);
							}
						}		
					}
				}
			}

			// Destroy all instances from the Spawned Objects List
			if( spawnedObjectsList != null ){
				for( int sol = 0; sol < spawnedObjectsList.Count; sol++ ){
					if( spawnedObjectsList[sol]!=null ){
						if(spawnedObjectsList[sol].instance!=null){ 

							// Destroy normally
							if( spawnedObjectsList[sol].poolItem.enableDestroyDelegates == false ){
								Destroy(spawnedObjectsList[sol].instance); 
							
							// Destroy using delegates
							} else {
								DestroyInstance(spawnedObjectsList[sol].instance);
							} 

						}
					}
				}
			}

			// Clear the Spawned Objects array / list to avoid memory leaks
			if(spawnedObjects!=null){ Arrays.Clear( ref spawnedObjects ); }
			if( spawnedObjectsList != null ){ spawnedObjectsList.Clear(); }
			
		}

		// ==================================================================================================================
		//	CALCULATE HOW LONG THE PRELOADED ARRAY SHOULD BE
		//	Figure out the total number of instances to create
		// ==================================================================================================================

		// Helpers
		[System.NonSerialized] int chltpasbTotalCount = 0;

		// Method
		private int CalculateHowLongThePreloadedArrayShouldBe(){
			chltpasbTotalCount = 0;
			for (int i = 0; i < poolItems.Length; i++ ){	
				chltpasbTotalCount += poolItems[i].poolSize;
			}
			return chltpasbTotalCount;
		}

		// ==================================================================================================================
		//	GAMEOBJECT IS NULL
		//	Fastest possible methods to check if a gameobject is null
		// ==================================================================================================================
		
		public bool GameObjectIsNull( GameObject go ){

			/*
			#if UNITY_EDITOR
				// NOTE: Documentation notes that the optimization doesnt work in the Editor because of some weird
				// Unity hack. So in the editor, we should just return go == null
				// so it may need to use 'return go == null'.
					
				// THESE WORK												// [Settings: Record, No deep profile]
				//return go == null;											// 0.01 -> 0.02 (mostly 0.01)
				return null == go;										// 0.01 -> 0.02 (mostly 0.01)
				//return !go;													// 0.01 -> 0.02 (mostly 0.01) 

				// THESE DONT WORK
				//return GameObject.Equals(go, null);					// <- doesn't work
				//return System.Object.Equals( go, null );				// <- doesnt work
				//return System.Object.ReferenceEquals( go, null );		// <- doesnt work
				
				// SUPPOSEDLY THE BEST ONE
				//return (object) go == null;							// <- doesnt work
			#else
				return (object) go == null;
			#endif
			*/

			return go == null;										// 0.01 -> 0.02 (mostly 0.01)
		}

		// These methods are faster than checking if go == null. But it will not return true if a GameObject is "missing" 
		// because it was destroyed. It has to be EXPLICITLY NULL.
		public bool GameObjectIsExplicitlyNull( GameObject go ){
			return (object) go == null;
		}
		public bool TransformIsExplicitlyNull( Transform t ){
			return (object) t == null;
		}

		// ==================================================================================================================
		//	UPDATE / UPDATE POOL
		//	Runs every frame to ensure integrity of the pool, track instance counts, auto-despawning and other features
		// ==================================================================================================================
		
		// Helpers
		[System.NonSerialized] PoolKitInstance _updatePoolKitInstance = null;
		[System.NonSerialized] PoolItem _updatePoolKitInstancePoolItem = null;
		[System.NonSerialized] float cachedDeltaTime = 0f;
		[System.NonSerialized] List<int> arrayIndexesToFix = new List<int>();

		// Method
		public void Update(){

			// If we've created the Pool via the API, it needs to be setup first!
			if(poolWasJustCreatedByAPI){ return; }

			// Update the Pool
			PoolUpdate(true); 

		}

		// PoolUpdate Method (we seperate this so we can re-run it without deltaTime when we fix the pool)
		void PoolUpdate( bool applyDeltaTime ){	

			// Cache DeltaTime
			cachedDeltaTime = Time.deltaTime;

			// ==================
			//	SETUP POOL ITEMS
			// ==================

			// Loop through spawned Objects and reset the instance counters
			for( int i = poolItems.Length-1; i > -1; i-- ){

				// Help Statistics Progress Bar In Editor
				#if UNITY_EDITOR
					if( applyDeltaTime ){
						
						poolItems[i].instanceSpawnedProgressBar = poolItems[i].instanceCount == 0 ? 0f : ((float)poolItems[i].activeInstances /(float)poolItems[i].instanceCount );
						
						poolItems[i].instanceSpawnedProgressBarLerped = Mathf.MoveTowards( poolItems[i].instanceSpawnedProgressBarLerped, poolItems[i].instanceSpawnedProgressBar, Time.deltaTime*2 );
					}
				#endif
				
				// Reset counters
				poolItems[i].activeInstances = 0;
				poolItems[i].inactiveInstances = 0;

				// Run Preloading on each Pool Item that needs it
				if( poolItems[i].runPreloadingUpdate == true ){ poolItems[i].PreloadUpdate(); }

			}	

			// =======================
			//	UPDATE FIXED POOL
			// =======================

			// Loop through the fixed array ...
			if( _usingFixedSizePool == true ){
			
				// Loop through spawned Objects
				for( int i = spawnedObjects.Length-1; i > -1; i-- ){

					// Cache the spawned Object
					_updatePoolKitInstance = spawnedObjects[i];

					// If the instance doesn't exist anymore, it means it was destroyed and we must fix the Array
					if( enablePoolProtection && GameObjectIsNull( _updatePoolKitInstance.instance ) == true ){ 
						arrayIndexesToFix.Add(i);
					}

					// Cache the PoolItem
					_updatePoolKitInstancePoolItem = _updatePoolKitInstance.poolItem;

					// Make sure this object is currently spawned (and active)
					if( _updatePoolKitInstance.isSpawned == true ){ // <- this should be faster than activeInHierarchy

						// ACTIVE INSTANCES
						_updatePoolKitInstancePoolItem.activeInstances++;

						// STATISTICS
						// Track the number of active instances
						#if UNITY_EDITOR
							if( _updatePoolKitInstancePoolItem.maxInstancesSpawnedAtOnce <
								_updatePoolKitInstancePoolItem.activeInstances
							){ 
								_updatePoolKitInstancePoolItem.maxInstancesSpawnedAtOnce = _updatePoolKitInstancePoolItem.activeInstances;
							}
						#endif


						// APPLY DELTA TIME FUNCTIONALITY
						// If we're updating deltaTimes on this loop, do it...
						if( applyDeltaTime ){

							// Always increment aliveTime
							_updatePoolKitInstance.aliveTime += cachedDeltaTime;

							// AUTOMATIC DESPAWNS
							// If we've setup auto-despawns, handle it here
							if( _updatePoolKitInstancePoolItem.enableAutoDespawn ){

								// Handle countdown timers if we're using a countdown or random range
								if( _updatePoolKitInstancePoolItem.despawnMode == PoolItem.DespawnMode.Countdown ||
									_updatePoolKitInstancePoolItem.despawnMode == PoolItem.DespawnMode.CountdownRandomRange
								){

									// Countdown the timer
									_updatePoolKitInstance.currentDespawnTimer -= cachedDeltaTime;

									// If the timer has fallen below 0, despawn the instance
									if( _updatePoolKitInstance.currentDespawnTimer <= 0f ){
										//Despawn( _updatePoolKitInstance.instance );
										DespawnInstance( i );
									}
								}
								
								// If we're waiting for an audiosource to finish ...
								else if( _updatePoolKitInstancePoolItem.despawnMode == PoolItem.DespawnMode.WaitForAudioToFinish &&
										_updatePoolKitInstance.aliveTime > 0.5f &&
										_updatePoolKitInstance.aSource != null && _updatePoolKitInstance.aSource.isPlaying == false
								){
									//Despawn( _updatePoolKitInstance.instance );
									DespawnInstance( i );
								}

								// If we're waiting for a particle system to finish ...
								else if( _updatePoolKitInstancePoolItem.despawnMode == PoolItem.DespawnMode.WaitForParticleSystemToFinish && 
										_updatePoolKitInstance.pSystem != null && _updatePoolKitInstance.pSystem.IsAlive() == false
								){
									//Despawn( _updatePoolKitInstance.instance );
									DespawnInstance( i );
								}
							}
						}
					
					} else {

						// INACTIVE INSTANCES
						_updatePoolKitInstancePoolItem.inactiveInstances++;
					}
				}

			// =======================
			//	UPDATE LIST POOL
			// =======================

			// Loop through the dynamic list ...
			} else {

				// Loop through spawned Objects List
				for( int i = spawnedObjectsList.Count-1; i > -1; i-- ){
					
					// Cache the spawned Object
					_updatePoolKitInstance = spawnedObjectsList[i];

					// If the instance doesn't exist anymore, it means it was destroyed and we must fix the Array
					if( enablePoolProtection && GameObjectIsNull( _updatePoolKitInstance.instance ) ){ 
						arrayIndexesToFix.Add(i); 
					}

					// Cache the Pool Item
					_updatePoolKitInstancePoolItem = _updatePoolKitInstance.poolItem;
					
					// Make sure this object is currently spawned (and active)
					if( _updatePoolKitInstance.isSpawned == true ){ // <- this should be faster than activeInHierarchy
						
						// ACTIVE INSTANCES
						_updatePoolKitInstancePoolItem.activeInstances++;

						// STATISTICS
						// Track the number of active instances
						#if UNITY_EDITOR
							if( _updatePoolKitInstancePoolItem.maxInstancesSpawnedAtOnce <
								_updatePoolKitInstancePoolItem.activeInstances
							){ 
								_updatePoolKitInstancePoolItem.maxInstancesSpawnedAtOnce = _updatePoolKitInstancePoolItem.activeInstances;
							}
						#endif

						// APPLY DELTA TIME FUNCTIONALITY
						// If we're updating deltaTimes on this loop, do it...
						if( applyDeltaTime ){	

							// Always increment aliveTime
							_updatePoolKitInstance.aliveTime += cachedDeltaTime;

							// AUTOMATIC DESPAWNS
							// If we've setup auto-despawns, handle it here
							if( _updatePoolKitInstancePoolItem.enableAutoDespawn ){

								// Handle countdown timers if we're using a countdown or random range
								if( _updatePoolKitInstancePoolItem.despawnMode == PoolItem.DespawnMode.Countdown ||
									_updatePoolKitInstancePoolItem.despawnMode == PoolItem.DespawnMode.CountdownRandomRange
								){

									// Countdown the timer
									_updatePoolKitInstance.currentDespawnTimer -= cachedDeltaTime;

									// If the timer has fallen below 0, despawn the instance
									if( _updatePoolKitInstance.currentDespawnTimer <= 0f ){
										// Despawn( _updatePoolKitInstance.instance );
										DespawnInstance( i );
									}
								}
								
								// If we're waiting for an audiosource to finish ...
								else if( _updatePoolKitInstancePoolItem.despawnMode == PoolItem.DespawnMode.WaitForAudioToFinish &&
										_updatePoolKitInstance.aliveTime > 0.5f &&
										_updatePoolKitInstance.aSource != null && _updatePoolKitInstance.aSource.isPlaying == false
								){
									// Despawn( _updatePoolKitInstance.instance );
									DespawnInstance( i );
								}

								// If we're waiting for a particle system to finish ...
								else if( _updatePoolKitInstancePoolItem.despawnMode == PoolItem.DespawnMode.WaitForParticleSystemToFinish &&
										_updatePoolKitInstance.pSystem != null && _updatePoolKitInstance.pSystem.IsAlive() == false
								){
									// Despawn( _updatePoolKitInstance.instance );
									DespawnInstance( i );
								}
							}
						}
					
					} else {
					
						// INACTIVE INSTANCES
						_updatePoolKitInstancePoolItem.inactiveInstances++;
					}
				}
			}

			// ==========================================================================
			//	FIX ARRAY
			//	This allows us to fix all objects in one go and keep the values updated
			// ==========================================================================

			if( enablePoolProtection && arrayIndexesToFix.Count > 0 ){

				// Show a warning that some instances have been destroyed
				Debug.LogWarning(
					"POOLKIT (Pool) - One or more instances from the pool '" + poolName + "' have been destroyed. You should never destroy pooled instances as this will trigger an automatic rebuild of the pool and severely reduce performance. Please use Pool.Despawn( gameObject ) instead!"
				);

				// Loop through all the problem indexes and fix them
				for( int i = 0; i < arrayIndexesToFix.Count; i++ ){ FixBrokenPool( arrayIndexesToFix[i] ); }

				// Reset the array so it doesn't re-run the fixes next loop
				arrayIndexesToFix.Clear();

				// Re-run the UpdatePool method (without updating deltaTimes) to get accurate counts
				PoolUpdate( false );
			}
		}

		// ==================================================================================================================
		//	GET RECYCLE INFO <string>
		//	Find out if this object can be recycled
		// ==================================================================================================================

		// Get Recycle Info (using the GameObject's name)
		public int GetRecycleInfo( string goName ){

			// Loop through the pool item and try to find the prefab ...
			for( int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPool.name == goName ){

					// If we've enabled Recycled Spawned Objects, return the last index used ...
					if( poolItems[i].recycleSpawnedObjects == true ){
						return poolItems[i].recycleNextIndex;
					} else {
						return -1;
					}

				}
			}

			// Otherwise, return -1 if nothing was found
			return -1;
		}

		// Get Recycle Info (using the GameObject's Instance ID )
		public int GetRecycleInfo( int instanceID ){

			// Loop through the pool item and try to find the prefab ...
			for( int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPoolInstanceID == instanceID ){

					// If we've enabled Recycled Spawned Objects, return the last index used ...
					if( poolItems[i].recycleSpawnedObjects == true ){
						return poolItems[i].recycleNextIndex;
					} else {
						return -1;
					}

				}
			}

			// Otherwise, return -1 if nothing was found
			return -1;
		}



		// ==================================================================================================================
		//	SPAWN OBJECT <string>
		//	Get the next available instance (by using the name of the object)
		// ==================================================================================================================

		// HELPERS
		[System.NonSerialized] int recycleNextIndex = 0;						// Used to find the next index to recycle
		[System.NonSerialized] Transform spawnStringLocalScale = null;			// Used for localScale methods
		[System.NonSerialized] Transform spawnToGO = null;						// Used to convert Transforms to GameObjects

		// ---------------------------------------------------------------------------------------
		//	CONVENIENT TRANSFORM => GAMEOBJECT VERSIONS
		// ---------------------------------------------------------------------------------------

		// Method ( standard )
		public GameObject SpawnGO( string goName, Vector3 position, Quaternion rotation, Transform parent = null ){
			spawnToGO = Spawn( goName, position, rotation, parent ); 
			if(spawnToGO != null){ return spawnToGO.gameObject; }
			return null;
		}

		// Shortcut ( prefab name only )
		public GameObject SpawnGO( string goName ){ 
			spawnToGO = Spawn( goName, Vector3.zero, Quaternion.identity, null ); 
			//if(spawnToGO != null){ return spawnToGO.gameObject; }
			if( TransformIsExplicitlyNull(spawnToGO) == false ){ return spawnToGO.gameObject; }
			return null;
		}

		// Shortcut ( prefab name, position, rotation using eulerangles,  <optional> parent )
		public GameObject SpawnGO( string goName, Vector3 position, Vector3 eulerRotation, Transform parent = null ){
			spawnToGO = Spawn( goName, position, Quaternion.Euler(eulerRotation), parent ); 
			//if(spawnToGO != null){ return spawnToGO.gameObject; }
			if( TransformIsExplicitlyNull(spawnToGO) == false ){ return spawnToGO.gameObject; }
			return null;
		}

		// Shortcut with localScale functionality: name, position, rotation, localScale, <optional> parent
		public GameObject SpawnGO( string goName, Vector3 position, Quaternion rotation, Vector3 localScale, Transform parent = null ){
			spawnStringLocalScale = Spawn( goName, position, rotation, parent );
			//if( spawnStringLocalScale!=null ){ 
			if( TransformIsExplicitlyNull(spawnStringLocalScale) == false ){ 
				spawnStringLocalScale.localScale = localScale;
				return spawnStringLocalScale.gameObject; 
			}
			return null;
		}

		// Shortcut with Localscale functionality (using EulerAngles)
		public GameObject SpawnGO( string goName, Vector3 position, Vector3 eulerRotation, Vector3 localScale, Transform parent = null ){
			spawnStringLocalScale = Spawn( goName, position, Quaternion.Euler(eulerRotation), parent );
			//if( spawnStringLocalScale!=null ){ 
			if( TransformIsExplicitlyNull(spawnStringLocalScale) == false ){ 	
				spawnStringLocalScale.localScale = localScale;
				return spawnStringLocalScale.gameObject; 
			}
			return null;
		}

		// ---------------------------------------------------------------------------------------
		//	RETURNS TRANSFORM
		// ---------------------------------------------------------------------------------------

		// Shortcut ( prefab name only )
		public Transform Spawn( string goName ){ 
			return Spawn( goName, Vector3.zero, Quaternion.identity, null ); 
		}

		// Shortcut ( prefab name, position, rotation using eulerangles,  <optional> parent )
		public Transform Spawn( string goName, Vector3 position, Vector3 eulerRotation, Transform parent = null ){
			return Spawn( goName, position, Quaternion.Euler(eulerRotation), parent ); 
		}

		// Shortcut with localScale functionality: name, position, rotation, localScale, <optional> parent
		public Transform Spawn( string goName, Vector3 position, Quaternion rotation, Vector3 localScale, Transform parent = null ){
			spawnStringLocalScale = Spawn( goName, position, rotation, parent );
			//if(spawnStringLocalScale!=null){ spawnStringLocalScale.localScale = localScale; }
			if( TransformIsExplicitlyNull(spawnStringLocalScale) == false ){ spawnStringLocalScale.localScale = localScale; }
			return spawnStringLocalScale;
		}

		// Shortcut with Localscale functionality (using EulerAngles)
		public Transform Spawn( string goName, Vector3 position, Vector3 eulerRotation, Vector3 localScale, Transform parent = null ){
			spawnStringLocalScale = Spawn( goName, position, Quaternion.Euler(eulerRotation), parent );
			//if(spawnStringLocalScale!=null){ spawnStringLocalScale.localScale = localScale; }
			if( TransformIsExplicitlyNull(spawnStringLocalScale) == false ){ spawnStringLocalScale.localScale = localScale; }
			return spawnStringLocalScale;
		}

		// Method ( standard )
		public Transform Spawn( string goName, Vector3 position, Quaternion rotation, Transform parent = null ){

			// =======================
			//	SPAWN FROM FIXED POOL
			// =======================

			// Loop through the fixed array ...
			if( _usingFixedSizePool == true ){

				// GET NEXT POOLED OBJECT
				// Loop through the poolItemed objects and see if we can find an inactive gameobject to spawn
				//for ( int i = 0; i < spawnedObjects.Count; i++ ){
				for ( int i = 0; i < spawnedObjects.Length; i++ ){
					if ( //!spawnedObjects[i].instance.activeInHierarchy && 
						!spawnedObjects[i].isSpawned &&	// <- this should be much faster than activeInHierarchy
						spawnedObjects[i].prefabOriginalName == goName 
					){

						// Show A Debug Message if the instance was found to have been deleted
						if( enablePoolProtection && GameObjectIsNull( spawnedObjects[i].instance ) ){
							Debug.LogWarning(
								"POOLKIT - An instance of the prefab '" + spawnedObjects[i].instanceName + "' from the pool '" + poolName + "' seems to have been destroyed. Never destroy pooled instances as this will trigger an automatic rebuild of the pool and severely reduce performance. Please use Pool.Despawn( gameObject ) instead!"
							);

							// Fix the pool by removing the problematic entry
							FixBrokenPool( i );

							// Return null
							return null;
						}
						
						// Setup Spawned Object
						SetupPoolKitInstance( spawnedObjects[i], position, rotation, parent );

						#if UNITY_EDITOR
							// Track Spawn Requests for statistics
							spawnedObjects[i].poolItem.totalNumberOfSpawns++;
							spawnedObjects[i].poolItem.timeSincePoolLastSpawnedAnInstance = Epoch.Current();
						#endif

						// Return it
						return spawnedObjects[i].instanceTransform;
					}
				}

				// ======================================
				//	RECYCLE AN ACTIVE INSTANCE IF NEEDED
				// ======================================

				// Get the Recycle Next Index (return -1 if not allowed)
				recycleNextIndex = GetRecycleInfo( goName );

				// Handle recyclying instances if the limit was used and reached.
				if ( recycleNextIndex >= 0 ){

					// Make sure the recycleNextIndex is within range of the spawned index array
					if( recycleNextIndex >= spawnedObjects.Length ){ recycleNextIndex = 0; }

					// loop through the items again, starting from the recycleNextIndex
					for ( int i = recycleNextIndex; i < spawnedObjects.Length; i++ ){
						
						// This time, we're looking for the first matching instance we find (ignoring if its spawned)
						if ( spawnedObjects[i].prefabOriginalName == goName ){

							// Setup the next FirstInFirstOutIndex to use the next entry in the array
							spawnedObjects[i].poolItem.recycleNextIndex = i+1;

							// Despawn the first instance
							//Despawn( spawnedObjects[i].instance );
							DespawnInstance( i );

							#if UNITY_EDITOR
								// Track Despawn Requests for statistics
								spawnedObjects[i].poolItem.totalNumberOfDespawns++;
								spawnedObjects[i].poolItem.timeSincePoolLastDespawnedAnInstance = Epoch.Current();
							#endif

							// Setup Spawned Object
							SetupPoolKitInstance( spawnedObjects[i], position, rotation, parent );

							#if UNITY_EDITOR
								// Track Spawn Requests for statistics
								spawnedObjects[i].poolItem.totalNumberOfSpawns++;
								spawnedObjects[i].poolItem.timeSincePoolLastSpawnedAnInstance = Epoch.Current();
							#endif

							// Return it
							return spawnedObjects[i].instanceTransform;
						}
					}
				}

				// If haven't found anything and using a fixed list, don't allow pools to be resized. Return null
				return null;

			// =======================
			//	SPAWN FROM LIST POOL
			// =======================

			// Loop through the list ...
			} else {

				// GET NEXT POOLED OBJECT
				// Loop through the poolItemed objects and see if we can find an inactive gameobject to spawn
				//for ( int i = 0; i < spawnedObjects.Count; i++ ){
				for ( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if ( //!spawnedObjectsList[i].instance.activeInHierarchy && 
						!spawnedObjectsList[i].isSpawned &&	// <- this should be much faster than activeInHierarchy
						spawnedObjectsList[i].prefabOriginalName == goName 
					){

						// Show A Debug Message if the instance was found to have been deleted
						if( enablePoolProtection && GameObjectIsNull( spawnedObjectsList[i].instance ) ){
							Debug.LogWarning(
								"POOLKIT - An instance of the prefab '" + spawnedObjectsList[i].instanceName + "' from the pool '" + poolName + "' seems to have been destroyed. Never destroy pooled instances as this will trigger an automatic rebuild of the pool and severely reduce performance. Please use Pool.Despawn( gameObject ) instead!"
							);
							
							// Fix the pool by removing the problematic entry
							FixBrokenPool( i );

							// Return null
							return null;
						}
						
						// Setup Spawned Object
						SetupPoolKitInstance( spawnedObjectsList[i], position, rotation, parent );

						#if UNITY_EDITOR
							// Track Spawn Requests for statistics
							spawnedObjectsList[i].poolItem.totalNumberOfSpawns++;
							spawnedObjectsList[i].poolItem.timeSincePoolLastSpawnedAnInstance = Epoch.Current();
						#endif

						// Return it
						return spawnedObjectsList[i].instanceTransform;
					}
				}

				// ======================================
				//	RECYCLE AN ACTIVE INSTANCE IF NEEDED
				// ======================================

				// Get the Recycle Next Index (return -1 if not allowed)
				recycleNextIndex = GetRecycleInfo( goName );

				// Handle recyclying instances if the limit was used and reached.
				if ( recycleNextIndex >= 0 ){

					// Make sure the recycleNextIndex is within range of the spawned index array
					if( recycleNextIndex >= spawnedObjectsList.Count ){ recycleNextIndex = 0; }

					// loop through the items again, starting from the recycleNextIndex
					for ( int i = recycleNextIndex; i < spawnedObjectsList.Count; i++ ){
						
						// This time, we're looking for the first matching instance we find (ignoring if its spawned)
						if ( spawnedObjectsList[i].prefabOriginalName == goName ){

							// Setup the next FirstInFirstOutIndex to use the next entry in the array
							spawnedObjectsList[i].poolItem.recycleNextIndex = i+1;

							// Despawn the first instance
							//Despawn( spawnedObjectsList[i].instance );
							DespawnInstance( i );

							#if UNITY_EDITOR
								// Track Despawn Requests for statistics
								spawnedObjectsList[i].poolItem.totalNumberOfDespawns++;
								spawnedObjectsList[i].poolItem.timeSincePoolLastDespawnedAnInstance = Epoch.Current();
							#endif

							// Setup Spawned Object
							SetupPoolKitInstance( spawnedObjectsList[i], position, rotation, parent );

							#if UNITY_EDITOR
								// Track Spawn Requests for statistics
								spawnedObjectsList[i].poolItem.totalNumberOfSpawns++;
								spawnedObjectsList[i].poolItem.timeSincePoolLastSpawnedAnInstance = Epoch.Current();
							#endif

							// Return it
							return spawnedObjectsList[i].instanceTransform;
						}
					}

					// BUGFIX FOR RECYCLING
					// If we started from the last instance in the list, we would have come to the end of spawnedObjects without finding it.
					// Try the loop once more starting from index 0 up until 'recycleNextIndex' to see if we can find it in the first half of the range!
					if( recycleNextIndex > 0 ){
						for ( int i = 0; i < recycleNextIndex; i++ ){

							// This time, we're looking for the first matching instance we find (ignoring if its spawned)
							if ( spawnedObjectsList[i].prefabOriginalName == goName ){

								// Setup the next FirstInFirstOutIndex to use the next entry in the array
								spawnedObjectsList[i].poolItem.recycleNextIndex = i+1;

								// Despawn the first instance
								//Despawn( spawnedObjectsList[i].instance );
								DespawnInstance( i );

								#if UNITY_EDITOR
									// Track Despawn Requests for statistics
									spawnedObjectsList[i].poolItem.totalNumberOfDespawns++;
									spawnedObjectsList[i].poolItem.timeSincePoolLastDespawnedAnInstance = Epoch.Current();
								#endif

								// Setup Spawned Object
								SetupPoolKitInstance( spawnedObjectsList[i], position, rotation, parent );

								#if UNITY_EDITOR
									// Track Spawn Requests for statistics
									spawnedObjectsList[i].poolItem.totalNumberOfSpawns++;
									spawnedObjectsList[i].poolItem.timeSincePoolLastSpawnedAnInstance = Epoch.Current();
								#endif

								// Return it
								return spawnedObjectsList[i].instanceTransform;
							}
						}
					}
				}

			}


			// AUTOMATICALLY CREATE NEW INSTANCES IF REQUIRED
			// No free objects are available, see if we are allowed to create more instances ...
			// Loop through the items of the poolItem and see if the name matches
			for (int i = 0; i < poolItems.Length; i++ ){
				if ( poolItems[i].prefabToPool.name == goName ){
					// As we know the names match, if we are allowed to create new instances, do it!
					//if ( poolItems[i].expandPoolWhenNeeded ){
					if( poolItems[i].poolSizeOptions == PoolItem.PoolResizeOptions.AlwaysExpandPoolWhenNeeded || 
						poolItems[i].poolSizeOptions == PoolItem.PoolResizeOptions.ExpandPoolWithinLimit &&
						( poolItems[i].activeInstances + poolItems[i].inactiveInstances ) < poolItems[i].limitPoolSize
					){

						#if UNITY_EDITOR
							// Track Spawn Requests for statistics
							poolItems[i].totalNumberOfSpawns++;
							poolItems[i].timeSincePoolLastSpawnedAnInstance = Epoch.Current();
						#endif

						// Return the newly instantiated object ( and also Setup the spawned object during the process )
						return InstantiatePooledObject(poolItems[i], true, position, rotation, parent );

					// Otherwise, break the loop early	
					} else { break; }
				}
			}

			// If there is nothing to return, return null.
			return null;
		}

		

		// ==================================================================================================================
		//	SPAWN OBJECT <GameObject>
		//	Get the next available instance (by using the GameObject prefab reference)
		// ==================================================================================================================

		// Helper
		[System.NonSerialized] int _spawnInstanceID = 0;
		[System.NonSerialized] Transform spawnPrefabLocalScale = null;			// Used for localScale methods
		
		// --------------------------------------------------------------------------------------
		//	CONVENIENT TRANSFORM => GAMEOBJECT VERSIONS
		//	NOTE: These methods can take either a Transform or GameObject as a prefab reference
		// ---------------------------------------------------------------------------------------

		// Method ( standard )
		public GameObject SpawnGO( GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null ){
			spawnToGO = Spawn( prefab, position, rotation, parent ); 
			//if(spawnToGO != null){ return spawnToGO.gameObject; }
			if( TransformIsExplicitlyNull(spawnToGO) == false ){ return spawnToGO.gameObject; }
			return null;
		}
		public GameObject SpawnGO( Transform prefab, Vector3 position, Quaternion rotation, Transform parent = null ){
			spawnToGO = Spawn( prefab, position, rotation, parent ); 
			//if(spawnToGO != null){ return spawnToGO.gameObject; }
			if( TransformIsExplicitlyNull(spawnToGO) == false ){ return spawnToGO.gameObject; }
			return null;
		}

		// Shortcut ( prefab only )
		public GameObject SpawnGO( GameObject prefab ){ 
			spawnToGO = Spawn( prefab, Vector3.zero, Quaternion.identity, null ); 
			//if(spawnToGO != null){ return spawnToGO.gameObject; }
			if( TransformIsExplicitlyNull(spawnToGO) == false ){ return spawnToGO.gameObject; }
			return null;
		}
		public GameObject SpawnGO( Transform prefab ){ 
			spawnToGO = Spawn( prefab, Vector3.zero, Quaternion.identity, null ); 
			//if(spawnToGO != null){ return spawnToGO.gameObject; }
			if( TransformIsExplicitlyNull(spawnToGO) == false ){ return spawnToGO.gameObject; }
			return null;
		}

		// Shortcut ( prefab , position, rotation using eulerangles,  <optional> parent )
		public GameObject SpawnGO( GameObject prefab, Vector3 position, Vector3 eulerRotation, Transform parent = null ){
			spawnToGO = Spawn( prefab, position, Quaternion.Euler(eulerRotation), parent ); 
			//if(spawnToGO != null){ return spawnToGO.gameObject; }
			if( TransformIsExplicitlyNull(spawnToGO) == false ){ return spawnToGO.gameObject; }
			return null;
		}
		public GameObject SpawnGO( Transform prefab, Vector3 position, Vector3 eulerRotation, Transform parent = null ){
			spawnToGO = Spawn( prefab, position, Quaternion.Euler(eulerRotation), parent ); 
			//if(spawnToGO != null){ return spawnToGO.gameObject; }
			if( TransformIsExplicitlyNull(spawnToGO) == false ){ return spawnToGO.gameObject; }
			return null;
		}

		// Shortcut with localScale functionality: prefab, position, rotation, localScale, <optional> parent
		public GameObject SpawnGO( GameObject prefab, Vector3 position, Quaternion rotation, Vector3 localScale, Transform parent = null ){
			spawnStringLocalScale = Spawn( prefab, position, rotation, parent );
			//if( spawnStringLocalScale!=null ){ 
			if( TransformIsExplicitlyNull(spawnStringLocalScale) == false ){
				spawnStringLocalScale.localScale = localScale;
				return spawnStringLocalScale.gameObject; 
			}
			return null;
		}
		public GameObject SpawnGO( Transform prefab, Vector3 position, Quaternion rotation, Vector3 localScale, Transform parent = null ){
			spawnStringLocalScale = Spawn( prefab, position, rotation, parent );
			//if( spawnStringLocalScale!=null ){ 
			if( TransformIsExplicitlyNull(spawnStringLocalScale) == false ){
				spawnStringLocalScale.localScale = localScale;
				return spawnStringLocalScale.gameObject; 
			}
			return null;
		}

		// Shortcut with Localscale functionality (using EulerAngles)
		public GameObject SpawnGO( GameObject prefab, Vector3 position, Vector3 eulerRotation, Vector3 localScale, Transform parent = null ){
			spawnStringLocalScale = Spawn( prefab, position, Quaternion.Euler(eulerRotation), parent );
			//if( spawnStringLocalScale!=null ){ 
			if( TransformIsExplicitlyNull(spawnStringLocalScale) == false ){
				spawnStringLocalScale.localScale = localScale;
				return spawnStringLocalScale.gameObject; 
			}
			return null;
		}
		public GameObject SpawnGO( Transform prefab, Vector3 position, Vector3 eulerRotation, Vector3 localScale, Transform parent = null ){
			spawnStringLocalScale = Spawn( prefab, position, Quaternion.Euler(eulerRotation), parent );
			//if( spawnStringLocalScale!=null ){ 
			if( TransformIsExplicitlyNull(spawnStringLocalScale) == false ){
				spawnStringLocalScale.localScale = localScale;
				return spawnStringLocalScale.gameObject; 
			}
			return null;
		}


		// ---------------------------------------------------------------------------------------
		//	RETURNS TRANSFORM
		// ---------------------------------------------------------------------------------------

		// Shortcut  ( prefab Only )
		public Transform Spawn( GameObject prefab ){ return Spawn(prefab, Vector3.zero, Quaternion.identity, null ); }
		public Transform Spawn( Transform prefab ){ return Spawn(prefab, Vector3.zero, Quaternion.identity, null ); }

		// Shortcut ( prefab name, position, rotation using eulerangles,  <optional> parent )
		public Transform Spawn( GameObject prefab, Vector3 position, Vector3 eulerRotation, Transform parent = null ){
			return Spawn( prefab, position, Quaternion.Euler(eulerRotation), parent ); 
		}
		public Transform Spawn( Transform prefab, Vector3 position, Vector3 eulerRotation, Transform parent = null ){
			return Spawn( prefab, position, Quaternion.Euler(eulerRotation), parent ); 
		}

		// Shortcut with localScale functionality: name, position, rotation, localScale, <optional> parent
		public Transform Spawn( GameObject prefab, Vector3 position, Quaternion rotation, Vector3 localScale, Transform parent = null ){
			spawnPrefabLocalScale = Spawn( prefab, position, rotation, parent );
			//if(spawnPrefabLocalScale!=null){ spawnPrefabLocalScale.localScale = localScale; }
			if( TransformIsExplicitlyNull(spawnPrefabLocalScale) == false ){ spawnPrefabLocalScale.localScale = localScale; }
			return spawnPrefabLocalScale;
		}
		public Transform Spawn( Transform prefab, Vector3 position, Quaternion rotation, Vector3 localScale, Transform parent = null ){
			spawnPrefabLocalScale = Spawn( prefab, position, rotation, parent );
			//if(spawnPrefabLocalScale!=null){ spawnPrefabLocalScale.localScale = localScale; }
			if( TransformIsExplicitlyNull(spawnPrefabLocalScale) == false ){ spawnPrefabLocalScale.localScale = localScale; }
			return spawnPrefabLocalScale;
		}

		// Shortcut with Localscale functionality (using EulerAngles)
		public Transform Spawn( GameObject prefab, Vector3 position, Vector3 eulerRotation, Vector3 localScale, Transform parent = null ){
			spawnPrefabLocalScale = Spawn( prefab, position, Quaternion.Euler(eulerRotation), parent );
			//if( spawnPrefabLocalScale!=null ){ 
			if( TransformIsExplicitlyNull(spawnPrefabLocalScale) == false ){
				spawnPrefabLocalScale.localScale = localScale;
				return spawnPrefabLocalScale; 
			}
			return null;
		}
		public Transform Spawn( Transform prefab, Vector3 position, Vector3 eulerRotation, Vector3 localScale, Transform parent = null ){
			spawnPrefabLocalScale = Spawn( prefab, position, Quaternion.Euler(eulerRotation), parent );
			//if( spawnPrefabLocalScale!=null ){ 
			if( TransformIsExplicitlyNull(spawnPrefabLocalScale) == false ){
				spawnPrefabLocalScale.localScale = localScale;
				return spawnPrefabLocalScale; 
			}
			return null;
		}

		// ---------------------------------------------------------------------------------------
		//	TRANSFORM PREFAB OVERLOAD
		// ---------------------------------------------------------------------------------------

		// Transform Overload
		public Transform Spawn( Transform prefab, Vector3 position, Quaternion rotation, Transform parent = null  ){ 
			return Spawn( prefab.gameObject, position, rotation, parent ); 
		}

		// Core Method
		public Transform Spawn( GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null  ){

			// When spawning a new object, we use the InstanceID instead of a direct GameObject comparison as its faster
			_spawnInstanceID = prefab.GetInstanceID();

			// =======================
			//	SPAWN FROM FIXED POOL
			// =======================

			// Loop through the fixed array ...
			if( _usingFixedSizePool == true ){

				// GET NEXT POOLED OBJECT
				// Loop through the poolItemed objects and see if we can find an inactive gameobject to spawn
				//for ( int i = 0; i < spawnedObjects.Count; i++ ){
				for ( int i = 0; i < spawnedObjects.Length; i++ ){
					if ( //!spawnedObjects[i].instance.activeInHierarchy &&
						 !spawnedObjects[i].isSpawned &&	// <- this should be much faster than activeInHierarchy
						 spawnedObjects[i].prefabInstanceID == _spawnInstanceID
					){

						// Show A Debug Message if the instance was found to have been deleted
						if( enablePoolProtection && GameObjectIsNull( spawnedObjects[i].instance ) ){
							Debug.LogWarning(
								"POOLKIT - An instance of the prefab '" + spawnedObjects[i].instanceName + "' from the pool '" + poolName + "' seems to have been destroyed. Never destroy pooled instances as this will trigger an automatic rebuild of the pool and severely reduce performance. Please use Pool.Despawn( gameObject ) instead!"
							);
							
							// Fix the pool by removing the problematic entry
							FixBrokenPool( i );

							// Return null
							return null;
						}

						// Setup Spawned Object
						SetupPoolKitInstance( spawnedObjects[i], position, rotation, parent );

						#if UNITY_EDITOR
							// Track Spawn Requests for statistics
							spawnedObjects[i].poolItem.totalNumberOfSpawns++;
							spawnedObjects[i].poolItem.timeSincePoolLastSpawnedAnInstance = Epoch.Current();
						#endif

						// Return the instance
						return spawnedObjects[i].instanceTransform;
					}
				}

				// ======================================
				//	RECYCLE AN ACTIVE INSTANCE IF NEEDED
				// ======================================

				// Get the Recycle Next Index (return -1 if not allowed)
				recycleNextIndex = GetRecycleInfo( _spawnInstanceID );

				// Handle recyclying instances if the limit was used and reached.
				if ( recycleNextIndex >= 0 ){

					// Make sure the recycleNextIndex is within range of the spawned index array
					if( recycleNextIndex >= spawnedObjects.Length ){ recycleNextIndex = 0; }

					// loop through the items again, starting from the recycleNextIndex
					for ( int i = recycleNextIndex; i < spawnedObjects.Length; i++ ){
						
						// This time, we're looking for the first matching instance we find (ignoring if its spawned)
						if ( spawnedObjects[i].prefabInstanceID == _spawnInstanceID ){

							// Setup the next FirstInFirstOutIndex to use the next entry in the array
							spawnedObjects[i].poolItem.recycleNextIndex = i+1;

							// Despawn the first instance
							//Despawn( spawnedObjects[i].instance );
							DespawnInstance( i );

							#if UNITY_EDITOR
								// Track Despawn Requests for statistics
								spawnedObjects[i].poolItem.totalNumberOfDespawns++;
								spawnedObjects[i].poolItem.timeSincePoolLastDespawnedAnInstance = Epoch.Current();
							#endif

							// Setup Spawned Object
							SetupPoolKitInstance( spawnedObjects[i], position, rotation, parent );

							#if UNITY_EDITOR
								// Track Spawn Requests for statistics
								spawnedObjects[i].poolItem.totalNumberOfSpawns++;
								spawnedObjects[i].poolItem.timeSincePoolLastSpawnedAnInstance = Epoch.Current();
							#endif

							// Return it
							return spawnedObjects[i].instanceTransform;
						}
					}

					// BUGFIX FOR RECYCLING
					// If we started from the last instance in the list, we would have come to the end of spawnedObjects without finding it.
					// Try the loop once more starting from index 0 up until 'recycleNextIndex' to see if we can find it in the first half of the range!
					if( recycleNextIndex > 0 ){
						for ( int i = 0; i < recycleNextIndex; i++ ){
							
							// This time, we're looking for the first matching instance we find (ignoring if its spawned)
							if ( spawnedObjects[i].prefabInstanceID == _spawnInstanceID ){

								// Setup the next FirstInFirstOutIndex to use the next entry in the array
								spawnedObjects[i].poolItem.recycleNextIndex = i+1;

								// Despawn the first instance
								//Despawn( spawnedObjects[i].instance );
								DespawnInstance( i );

								#if UNITY_EDITOR
									// Track Despawn Requests for statistics
									spawnedObjects[i].poolItem.totalNumberOfDespawns++;
									spawnedObjects[i].poolItem.timeSincePoolLastDespawnedAnInstance = Epoch.Current();
								#endif

								// Setup Spawned Object
								SetupPoolKitInstance( spawnedObjects[i], position, rotation, parent );

								#if UNITY_EDITOR
									// Track Spawn Requests for statistics
									spawnedObjects[i].poolItem.totalNumberOfSpawns++;
									spawnedObjects[i].poolItem.timeSincePoolLastSpawnedAnInstance = Epoch.Current();
								#endif

								// Return it
								return spawnedObjects[i].instanceTransform;
							}
						}
					}

				}

				// If we're using a fixed list, don't allow poolItems to be resized. Return null
				return null;

			// =======================
			//	SPAWN FROM LIST POOL
			// =======================

			} else {

				// GET NEXT POOLED OBJECT
				// Loop through the poolItemed objects and see if we can find an inactive gameobject to spawn
				//for ( int i = 0; i < spawnedObjects.Count; i++ ){
				for ( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if ( // !spawnedObjectsList[i].instance.activeInHierarchy &&
						 !spawnedObjectsList[i].isSpawned &&	// <- this should be much faster than activeInHierarchy
						 spawnedObjectsList[i].prefabInstanceID == _spawnInstanceID
					){

						// Show A Debug Message if the instance was found to have been deleted
						if( enablePoolProtection && GameObjectIsNull( spawnedObjectsList[i].instance ) ){
							Debug.LogWarning(
								"POOLKIT - An instance of the prefab '" + spawnedObjectsList[i].instanceName + "' from the pool '" + poolName + "' seems to have been destroyed. Never destroy pooled instances as this will trigger an automatic rebuild of the pool and severely reduce performance. Please use Pool.Despawn( gameObject ) instead!"
							);
							
							// Fix the pool by removing the problematic entry
							FixBrokenPool( i );

							// Return null
							return null;
						}

						// Setup Spawned Object
						SetupPoolKitInstance( spawnedObjectsList[i], position, rotation, parent );

						#if UNITY_EDITOR
							// Track Spawn Requests for statistics
							spawnedObjectsList[i].poolItem.totalNumberOfSpawns++;
							spawnedObjectsList[i].poolItem.timeSincePoolLastSpawnedAnInstance = Epoch.Current();
						#endif	

						// Return the instance
						return spawnedObjectsList[i].instanceTransform;
					}
				}

				// ======================================
				//	RECYCLE AN ACTIVE INSTANCE IF NEEDED
				// ======================================

				// Get the Recycle Next Index (return -1 if not allowed)
				recycleNextIndex = GetRecycleInfo( _spawnInstanceID );

				// Handle recyclying instances if the limit was used and reached.
				if ( recycleNextIndex >= 0 ){

					// Make sure the recycleNextIndex is within range of the spawned index array
					if( recycleNextIndex >= spawnedObjectsList.Count ){ recycleNextIndex = 0; }

					// loop through the items again, starting from the recycleNextIndex
					for ( int i = recycleNextIndex; i < spawnedObjectsList.Count; i++ ){
						
						// This time, we're looking for the first matching instance we find (ignoring if its spawned)
						if ( spawnedObjectsList[i].prefabInstanceID == _spawnInstanceID ){

							// Setup the next FirstInFirstOutIndex to use the next entry in the array
							spawnedObjectsList[i].poolItem.recycleNextIndex = i+1;

							// Despawn the first instance
							//Despawn( spawnedObjectsList[i].instance );
							DespawnInstance( i );

							#if UNITY_EDITOR
								// Track Despawn Requests for statistics
								spawnedObjectsList[i].poolItem.totalNumberOfDespawns++;
								spawnedObjectsList[i].poolItem.timeSincePoolLastDespawnedAnInstance = Epoch.Current();
							#endif

							// Setup Spawned Object
							SetupPoolKitInstance( spawnedObjectsList[i], position, rotation, parent );

							#if UNITY_EDITOR
								// Track Spawn Requests for statistics
								spawnedObjectsList[i].poolItem.totalNumberOfSpawns++;
								spawnedObjectsList[i].poolItem.timeSincePoolLastSpawnedAnInstance = Epoch.Current();
							#endif

							// Return it
							return spawnedObjectsList[i].instanceTransform;
						}
					}
				}
			}


			// AUTOMATICALLY CREATE NEW INSTANCES IF REQUIRED
			// No free objects are available, see if we are allowed to create more instances ...
			// Loop through the items of the poolItem and see if the InstanceID matches
			for (int i = 0; i < poolItems.Length; i++ ){
				if ( poolItems[i].prefabToPoolInstanceID == _spawnInstanceID ){
					// As we know the names match, if we are allowed to create new instances, do it!
					//if ( poolItems[i].expandPoolWhenNeeded ){
					if( poolItems[i].poolSizeOptions == PoolItem.PoolResizeOptions.AlwaysExpandPoolWhenNeeded || 
						poolItems[i].poolSizeOptions == PoolItem.PoolResizeOptions.ExpandPoolWithinLimit &&
						( poolItems[i].activeInstances + poolItems[i].inactiveInstances ) < poolItems[i].limitPoolSize
					){

						#if UNITY_EDITOR
							// Track Spawn Requests for statistics
							poolItems[i].totalNumberOfSpawns++;
							poolItems[i].timeSincePoolLastSpawnedAnInstance = Epoch.Current();
						#endif

						// Return the newly instantiated object ( and also Setup the spawned object during the process )
						return InstantiatePooledObject(poolItems[i], true, position, rotation, parent );

					// Otherwise, break the loop early	
					} else { break; }
				}
			}

			// If there is nothing to return, return null.
			return null;
		}

		// ==================================================================================================================
		//	FIX BROKEN POOL
		//	After an instance is detected to have been destroyed, we need to rebuild the poolItem (which is very performance
		//	hungry, especially for fixed size Array poolItems).
		// ==================================================================================================================

		// Helpers
		PoolItem fixBrokenPoolItem = null;

		// Method
		private void FixBrokenPool( int indexToFix ){
			
			// Reset Broken Pool Item
			fixBrokenPoolItem = null;

			// Remove the broken index from the Spawned Objects Fixed Size Array
			if( _usingFixedSizePool ){

				// Remove it from the Array
				//Arrays.RemoveItemAtIndex( ref spawnedObjects, indexToFix );


				// Cache the original Pool Item
				fixBrokenPoolItem = spawnedObjects[indexToFix].poolItem;

				// Remove it from the Array
				Arrays.RemoveItemAtIndex( ref spawnedObjects, indexToFix );

				// If were were able to cache the original Pool Item, Create a new instance to replace it
				if( fixBrokenPoolItem != null ){
					
					// Add an extra PoolKit Instance at the end of the Array
					Arrays.AddItem( ref spawnedObjects, new PoolKitInstance() );

					// Instantiate a new instance in that slot
					InstantiatePooledObject( fixBrokenPoolItem, spawnedObjects.Length-1 );	// <- 2nd argument is the array slot
				}


			// Remove the broken index from the Spawned Objects List
			} else {

				// Remove it from the list
				//spawnedObjectsList.RemoveAt( indexToFix );


				// Cache the original Pool Item
				fixBrokenPoolItem = spawnedObjectsList[indexToFix].poolItem;

				// Remove it from the list
				spawnedObjectsList.RemoveAt( indexToFix );

				// If were were able to cache the original Pool Item, Create a new instance to replace it
				if( fixBrokenPoolItem != null ){

					// Instantiate a new instance in that slot
					InstantiatePooledObject( fixBrokenPoolItem, -1 );	// -1 because index isn't needed in Lists
				}
			}
		}

		// ==================================================================================================================
		//	REBUILD ALL INSTANCE COUNTS
		//	When we have to rebuild an array, we need to update all the poolKit instance counts
		// ==================================================================================================================

		/*
		void RebuildAllInstanceCounts(){

			// Wait 1 frame
			Debug.Log("RebuildAllInstanceCounts");

			// First, reset all of the instance counts:
			for( int pi = 0; pi < poolItems.Length; pi++ ){
				if(poolItems[pi]!=null){
					poolItems[pi].activeInstances = 0;
					poolItems[pi].inactiveInstances = 0;
				}
			}

			// Using the Array ...
			if( _usingFixedSizePool ){

				// Loop through all the spawned Objects and count up the instances
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].poolItem != null ){
						if( spawnedObjects[i].isSpawned == true ){
							spawnedObjects[i].poolItem.activeInstances++;
						} else {
							spawnedObjects[i].poolItem.inactiveInstances++;
						}
					}
				}

			// Using List ...
			} else {

				// Loop through all the spawned Objects and count up the instances
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].poolItem != null ){
						if( spawnedObjectsList[i].isSpawned == true ){
							spawnedObjectsList[i].poolItem.activeInstances++;
						} else {
							spawnedObjectsList[i].poolItem.inactiveInstances++;
						}
					}
				}
			}
		}
		*/


		// ==================================================================================================================
		//	SETUP SPAWNED OBJECT
		//	After an object is spawned, we broadcast events, set its position, rotation, etc.
		// ==================================================================================================================

		// Helper
		[System.NonSerialized] PoolItem soPoolItem = null;
		[System.NonSerialized] float _spkiProportionalScale = 1f;
		[System.NonSerialized] Vector3 _spkiTempV3 = Vector3.one;

		// Method
		private void SetupPoolKitInstance( 
			PoolKitInstance spawnedObject, Vector3 position, Quaternion rotation, Transform parent = null
		){

			// =================
			//	SETUP PLACEMENT
			// =================

			spawnedObject.instanceTransform.position = position;
			spawnedObject.instanceTransform.rotation = rotation;

			// Cache PoolItem as we access it quite a lot here
			soPoolItem = spawnedObject.poolItem;

			// -> New bit

			// =============
			//	SETUP SCALE
			// =============

			// If we should update the scale on every spawn, do it ...
			if( soPoolItem.resetScaleOnEverySpawn == true ){

				if ( soPoolItem.spawnedItemScale == PoolItem.PoolScale.Ignore ){
					// Do Nothing
				
				} else if ( soPoolItem.spawnedItemScale == PoolItem.PoolScale.PrefabScale ){
					spawnedObject.instanceTransform.localScale = soPoolItem.prefabToPool.transform.localScale;
				
				} else if( soPoolItem.spawnedItemScale == PoolItem.PoolScale.PoolScale ){
					spawnedObject.instanceTransform.localScale = Vector3.one;
				
				} else if( soPoolItem.spawnedItemScale == PoolItem.PoolScale.CustomScale ){
					spawnedObject.instanceTransform.localScale = soPoolItem.customSpawnScale;
				
				} else if( soPoolItem.spawnedItemScale == PoolItem.PoolScale.RandomRangeCustomScale ){
					
					// Use the temporary Vector3 to setup the new values (this doesn't require memory allocations)
					_spkiTempV3.x = UnityEngine.Random.Range( soPoolItem.customSpawnScaleMin.x, soPoolItem.customSpawnScaleMax.x );
					_spkiTempV3.y = UnityEngine.Random.Range( soPoolItem.customSpawnScaleMin.y, soPoolItem.customSpawnScaleMax.y );
					_spkiTempV3.z = UnityEngine.Random.Range( soPoolItem.customSpawnScaleMin.z, soPoolItem.customSpawnScaleMax.z );

					// Set the new scale
					spawnedObject.instanceTransform.localScale = _spkiTempV3;

				} else if( soPoolItem.spawnedItemScale == PoolItem.PoolScale.RandomRangeProportionalScale ){

					// Randomize the proportional scale
					_spkiProportionalScale = UnityEngine.Random.Range( soPoolItem.customSpawnScaleProportionalMin, soPoolItem.customSpawnScaleProportionalMax );
					
					// Use the temporary Vector3 to setup the new values (this doesn't require memory allocations)
					_spkiTempV3.x = _spkiProportionalScale;
					_spkiTempV3.y = _spkiProportionalScale;
					_spkiTempV3.z = _spkiProportionalScale;

					// Set the new scale
					spawnedObject.instanceTransform.localScale = _spkiTempV3;
				}
			}

			// =============
			//	SETUP LAYER
			// =============

			// If we should update the layer on every spawn, do it ...
			if( soPoolItem.resetLayerOnEverySpawn == true ){

				if ( soPoolItem.spawnedItemLayer == PoolItem.PoolLayer.Ignore ){
					// Do Nothing
				} else if ( soPoolItem.spawnedItemLayer == PoolItem.PoolLayer.PrefabLayer ){
					SetLayerRecursively( spawnedObject.instance, spawnedObject.instanceTransform, spawnedObject.poolItem.prefabToPool.layer );
				} else if( soPoolItem.spawnedItemLayer == PoolItem.PoolLayer.PoolLayer ){
					SetLayerRecursively( spawnedObject.instance, spawnedObject.instanceTransform, _poolItemRootLayer );
				} else if( soPoolItem.spawnedItemLayer == PoolItem.PoolLayer.CustomLayer ){
					SetLayerRecursively( spawnedObject.instance, spawnedObject.instanceTransform, spawnedObject.poolItem.customSpawnLayer);
				}
			}

			// <- end of new bit

			// ==============
			//	SETUP PARENT
			// ==============

			// Handle setting the parent
			if ( parent != null ){

				// When setting the parent, we also factor in if this was a RectTransform or not ... 
				spawnedObject.instanceTransform.SetParent(parent, !spawnedObject.isRectTransform );

			// Reparent the object back to the poolItem if needed
			} else if ( soPoolItem.keepOrganized && 
						spawnedObject.instanceTransform.parent.GetInstanceID() != _poolItemRootTransformInstanceID 
			){
				
				// If a new instance was created, it won't be reparented
				spawnedObject.instanceTransform.SetParent( _poolItemRootTransform, !spawnedObject.isRectTransform );
			}

			// ===============
			//	SETUP HELPERS
			// ===============

			// Reset Alive Time
			spawnedObject.aliveTime = 0f;

			// Setup Automatic Despawns
			if( soPoolItem.enableAutoDespawn ){

				// Simple Countdown
				if( soPoolItem.despawnMode == PoolItem.DespawnMode.Countdown ){
					spawnedObject.currentDespawnTimer = soPoolItem.despawnAfterHowManySeconds;
				}

				// Random Range
				else if( spawnedObject.poolItem.despawnMode == PoolItem.DespawnMode.CountdownRandomRange ){
					spawnedObject.currentDespawnTimer = UnityEngine.Random.Range( 
															soPoolItem.despawnRandomRangeMin, 
															soPoolItem.despawnRandomRangeMax 
														);
				}
			}

			// ===============
			//	MAKE ACTIVE
			// ===============

			// Set the object active
			spawnedObject.instance.SetActive(true);
			spawnedObject.isSpawned = true;


			// ===============
			//	HANDLE EVENTS
			// ===============

			// Run OnSpawn Method
			spawnedObject.BroadCastSpawn();

			// Send any PoolKit Spawn Events
			if( enablePoolEvents && onPoolSpawn != null ){ 
				onPoolSpawn( spawnedObject.instanceTransform, this );
			}

		}

		// ==================================================================================================================
		//	DESPAWN ALL
		//	Despawns All instances in this pool
		// ==================================================================================================================

		// Method
		public void DespawnAll(){

			// =========================
			//	DESPAWN FROM FIXED POOL
			// =========================

			// Loop through the fixed array ...
			if( _usingFixedSizePool == true ){

				// Loop through the instances and despawn everything that is currently spawned
				for ( int i = 0; i < spawnedObjects.Length; i++ ){
					if ( spawnedObjects[i].isSpawned == true ){ DespawnInstance( i ); }
				}

			// =======================
			//	DESPAWN FROM LIST
			// =======================

			} else {

				// Loop through the instances and despawn everything that is currently spawned
				for ( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if ( spawnedObjectsList[i].isSpawned == true ){ DespawnInstance( i ); }
				}
			}
		}

		// ==================================================================================================================
		//	DESPAWN ALL <Transform> / <GameObject>
		//	Despawns All instances of a specific Prefab in this pool by using the prefab GameObject / Transform
		// ==================================================================================================================

		[System.NonSerialized] int _despawnAllPrefabInstanceID = 0;

		// Prefab GameObject / Transform Method
		public void DespawnAll( Transform prefab ){ DespawnAll(prefab.gameObject); }
		public void DespawnAll( GameObject prefab ){

			// Cache the instance ID
			_despawnAllPrefabInstanceID = prefab.GetInstanceID();
			
			// =========================
			//	DESPAWN FROM FIXED POOL
			// =========================

			// Loop through the fixed array ...
			if( _usingFixedSizePool == true ){

				// Loop through the instances and despawn everything that is currently spawned and matches the prefab
				for ( int i = 0; i < spawnedObjects.Length; i++ ){
					if ( 	spawnedObjects[i].isSpawned == true &&
							spawnedObjects[i].prefabInstanceID == _despawnAllPrefabInstanceID
					){ 
						DespawnInstance( i ); 
					}
				}

			// =======================
			//	DESPAWN FROM LIST
			// =======================

			} else {

				// Loop through the instances and despawn everything that is currently spawned and matches the prefab
				for ( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if ( 	spawnedObjectsList[i].isSpawned == true &&
							spawnedObjectsList[i].prefabInstanceID == _despawnAllPrefabInstanceID
					){ 
						DespawnInstance( i ); 
					}
				}
			}
		}

		// ==================================================================================================================
		//	DESPAWN ALL <string>
		//	Despawns All instances of a specific Prefab in this pool by using the prefab's name
		// ==================================================================================================================

		// Prefab Name (string) Method
		public void DespawnAll( string prefabName ){
			
			// =========================
			//	DESPAWN FROM FIXED POOL
			// =========================

			// Loop through the fixed array ...
			if( _usingFixedSizePool == true ){

				// Loop through the instances and despawn everything that is currently spawned and matches the prefab
				for ( int i = 0; i < spawnedObjects.Length; i++ ){
					if ( 	spawnedObjects[i].isSpawned == true &&
							spawnedObjects[i].prefabOriginalName == prefabName
					){ 
						DespawnInstance( i ); 
					}
				}

			// =======================
			//	DESPAWN FROM LIST
			// =======================

			} else {

				// Loop through the instances and despawn everything that is currently spawned and matches the prefab
				for ( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if ( 	spawnedObjectsList[i].isSpawned == true &&
							spawnedObjectsList[i].prefabOriginalName == prefabName
					){ 
						DespawnInstance( i ); 
					}
				}
			}
		}

		// ==================================================================================================================
		//	DESPAWN INSTANCE
		//	Allows us to directly despawn a PoolKitInstance by index (this simplifies the code as we use it in multiple places)
		//	This is an internal helper only, users should use Pool.Despawn().
		// ==================================================================================================================

		[System.NonSerialized] PoolKitInstance despawnHelperPoolKitInstance = null;
		private bool DespawnInstance( int despawnIndex ){

			// =========================
			//	DESPAWN FROM FIXED POOL
			// =========================

			// Loop through the fixed array ...
			if( _usingFixedSizePool == true ){

				// Cache the spawned object
				despawnHelperPoolKitInstance = spawnedObjects[despawnIndex];

				// Show A Debug Message if the instance was found to have been deleted
				if( enablePoolProtection && GameObjectIsNull( despawnHelperPoolKitInstance.instance ) ){
					Debug.LogWarning(
						"POOLKIT - An instance of the prefab '" + despawnHelperPoolKitInstance.instanceName + "' from the pool '" + poolName + "' seems to have been destroyed. Never destroy pooled instances as this will trigger an automatic rebuild of the pool and severely reduce performance. Please use Pool.Despawn( gameObject ) instead!"
					);

					// Fix the pool by removing the problematic entry
					FixBrokenPool( despawnIndex );

					// Return null
					return false;
				}

				// Run OnDespawn Method
				despawnHelperPoolKitInstance.BroadCastDespawn();

				// Send any PoolKit Despawn Events
				if( enablePoolEvents && onPoolDespawn != null ){ 
					onPoolDespawn( despawnHelperPoolKitInstance.instanceTransform, this ); 
				}

				// De-activate the GameObject (despawn it)
				despawnHelperPoolKitInstance.instance.SetActive(false);
				despawnHelperPoolKitInstance.isSpawned = false;

				// If the poolItem is setup to reparent objects back to parentRootTransform, do it
				if( despawnHelperPoolKitInstance.poolItem.keepOrganized &&
					despawnHelperPoolKitInstance.instanceTransform.parent.GetInstanceID() != _poolItemRootTransformInstanceID
				){ 
					//despawnHelperPoolKitInstance.instanceTransform.parent = _poolItemRootTransform; 
					despawnHelperPoolKitInstance.instanceTransform.SetParent(_poolItemRootTransform, false); 
				}

				// Reset Alive Time
				despawnHelperPoolKitInstance.aliveTime = 0;

				#if UNITY_EDITOR
					// Track Despawn Requests for statistics
					spawnedObjects[despawnIndex].poolItem.totalNumberOfDespawns++;
					spawnedObjects[despawnIndex].poolItem.timeSincePoolLastDespawnedAnInstance = Epoch.Current();
				#endif

				// Update active instances in pool
			//	spawnedObjects[despawnIndex].poolItem.activeInstances--;
			//	spawnedObjects[despawnIndex].poolItem.inactiveInstances++;			

				// Return true if completed successfully
				return true;

			// =======================
			//	DESPAWN FROM LIST
			// =======================

			// Loop through the list ...
			} else {

				// Cache the spawned object
				despawnHelperPoolKitInstance = spawnedObjectsList[despawnIndex];

				// Show A Debug Message if the instance was found to have been deleted
				if( enablePoolProtection && GameObjectIsNull( despawnHelperPoolKitInstance.instance ) ){
					Debug.LogWarning(
						"POOLKIT - An instance of the prefab '" + despawnHelperPoolKitInstance.instanceName + "' from the pool '" + poolName + "' seems to have been destroyed. Never destroy pooled instances as this will trigger an automatic rebuild of the pool and severely reduce performance. Please use Pool.Despawn( gameObject ) instead!"
					);
					
					// Fix the pool by removing the problematic entry
					FixBrokenPool( despawnIndex );

					// Return null
					return false;
				}

				// Run OnDespawn Method
				despawnHelperPoolKitInstance.BroadCastDespawn();

				// Send any PoolKit Despawn Events
				if( enablePoolEvents && onPoolDespawn != null ){ 
					onPoolDespawn( despawnHelperPoolKitInstance.instanceTransform, this ); 
				}

				// De-activate the GameObject (despawn it)
				despawnHelperPoolKitInstance.instance.SetActive(false);
				despawnHelperPoolKitInstance.isSpawned = false;

				// If the poolItem is setup to reparent objects back to parentRootTransform, do it
				if( despawnHelperPoolKitInstance.poolItem.keepOrganized &&
					despawnHelperPoolKitInstance.instanceTransform.parent.GetInstanceID() != _poolItemRootTransformInstanceID
				){ 
					//despawnHelperPoolKitInstance.instanceTransform.parent = _poolItemRootTransform; 
					despawnHelperPoolKitInstance.instanceTransform.SetParent(_poolItemRootTransform, false);
				}

				// Reset Alive Time
				despawnHelperPoolKitInstance.aliveTime = 0;

				#if UNITY_EDITOR
					// Track Despawn Requests for statistics
					spawnedObjectsList[despawnIndex].poolItem.totalNumberOfDespawns++;
					spawnedObjectsList[despawnIndex].poolItem.timeSincePoolLastDespawnedAnInstance = Epoch.Current();
				#endif

				// Update active instances in pool
			//	spawnedObjectsList[despawnIndex].poolItem.activeInstances--;
			//	spawnedObjectsList[despawnIndex].poolItem.inactiveInstances++;			

				// After despawning the object, return true!
				return true;

			}
		}

		// ==================================================================================================================
		//	DESPAWN OBJECT <GameObject>
		//	Despawn a specific instance ( by sending the GameObject reference as an argument )
		// ==================================================================================================================

		// Helper
		[System.NonSerialized] int _despawnInstanceID = 0;

		// Overload Method
		public bool Despawn( Transform theInstance ){ 
			if(theInstance==null){ return false; }
			return Despawn(theInstance.gameObject); 
		}

		// Method
		public bool Despawn( GameObject theInstance ){

			// If the instance is null, return false
			if( theInstance==null ){ return false; }

			// When spawning a new object, we use the InstanceID instead of a direct GameObject comparison as its faster
			_despawnInstanceID = theInstance.GetInstanceID();

			// =========================
			//	DESPAWN FROM FIXED POOL
			// =========================

			// Loop through the fixed array ...
			if( _usingFixedSizePool == true ){

				// Loop through the poolItemed objects and see if we can find the gameobject to despawn
				for ( int i = 0; i < spawnedObjects.Length; i++ ){
					if ( spawnedObjects[i].instanceID == _despawnInstanceID ){
						return DespawnInstance( i );
					}
				}

			// =======================
			//	DESPAWN FROM LIST
			// =======================

			// Loop through the list ...
			} else {

				// Loop through the poolItemed objects and see if we can find the gameobject to despawn
				for ( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if ( spawnedObjectsList[i].instanceID == _despawnInstanceID ){
						return DespawnInstance( i );
					}
				}
			}

			// If something went wrong, return false
			return false;
		}

		// ==================================================================================================================
		//	INSTANTIATE A POOLED OBJECT
		//	This sets up / instantiates a gameObject to be used within the poolItem.
		//	NOTES: This method is used in a variety of ways, to create new preloaded instances at start, or add them 
		//	dynamically if we need to extend the list, etc.
		// ==================================================================================================================

		// Helpers
		[System.NonSerialized] GameObject _iapoGO = null;
		[System.NonSerialized] Transform _iapoGOT = null;
		[System.NonSerialized] PoolKitInstance _iapoSO = null;
		[System.NonSerialized] float _iapoProportionalScale = 1f;
		[System.NonSerialized] Vector3 _iapoTempV3 = Vector3.one;

		// Simple Overload to use the basic version (preloading objects)
		internal Transform InstantiatePooledObject( PoolItem poolItem, int useArrayIndex = -1 ){
			return InstantiatePooledObject( poolItem, false, Vector3.zero, Quaternion.identity, null, useArrayIndex );
		}

		// This sets up / instantiates a gameObject to be used within the poolItem.
		internal Transform InstantiatePooledObject( 
			
			// Core variables
			PoolItem poolItem, 

			// These variables are needed if we're also setting up the spawned object (extending the list)
			bool setupPoolKitInstance, Vector3 position, Quaternion rotation, Transform parent = null, int useArrayIndex = -1
		){

			// =============
			//	INSTANTIATE
			// =============

			// Instantiate normally ...
			if( poolItem.enableInstantiateDelegates == false ){
				_iapoGO = Instantiate<GameObject>(poolItem.prefabToPool);
			
			// Use delegates to handle instantiation
			} else {
				_iapoGO = InstantiatePrefab( poolItem.prefabToPool );
			}

			_iapoGOT = _iapoGO.transform;

			// Set the instance to be parented to the transform if setup that way
			//if(poolItem.keepOrganized){ _iapoGOT.parent = _poolItemRootTransform; }
			if(poolItem.keepOrganized){ _iapoGOT.SetParent(_poolItemRootTransform, false); }

			// Make the GameObject inactive at start (despawned)
			_iapoGO.SetActive(false);

			// =============
			//	RENAMING
			// =============

			// Rename the GameObject
			RenameInstance( _iapoGO, useArrayIndex );

			// =============
			//	SETUP SCALE
			// =============

			if ( poolItem.spawnedItemScale == PoolItem.PoolScale.Ignore ){
				// Do Nothing
			} else if ( poolItem.spawnedItemScale == PoolItem.PoolScale.PrefabScale ){
				_iapoGO.transform.localScale = poolItem.prefabToPool.transform.localScale;
			} else if( poolItem.spawnedItemScale == PoolItem.PoolScale.PoolScale ){
				_iapoGO.transform.localScale = Vector3.one;
			} else if( poolItem.spawnedItemScale == PoolItem.PoolScale.CustomScale ){
				_iapoGO.transform.localScale = poolItem.customSpawnScale;
			} else if( poolItem.spawnedItemScale == PoolItem.PoolScale.RandomRangeCustomScale ){

				// Use the temporary Vector3 to setup the new values (this doesn't require memory allocations)
				_iapoTempV3.x = UnityEngine.Random.Range( poolItem.customSpawnScaleMin.x, poolItem.customSpawnScaleMax.x );
				_iapoTempV3.y = UnityEngine.Random.Range( poolItem.customSpawnScaleMin.y, poolItem.customSpawnScaleMax.y );
				_iapoTempV3.z = UnityEngine.Random.Range( poolItem.customSpawnScaleMin.z, poolItem.customSpawnScaleMax.z );

				// Set the new local scale
				_iapoGO.transform.localScale = _iapoTempV3;
			
			} else if( poolItem.spawnedItemScale == PoolItem.PoolScale.RandomRangeProportionalScale ){

				// Randomize the proportional scale
				_iapoProportionalScale = UnityEngine.Random.Range( poolItem.customSpawnScaleProportionalMin, poolItem.customSpawnScaleProportionalMax );

				// Use the temporary Vector3 to setup the new values (this doesn't require memory allocations)
				_iapoTempV3.x = _iapoProportionalScale;
				_iapoTempV3.y = _iapoProportionalScale;
				_iapoTempV3.z = _iapoProportionalScale;

				// Set the new local scale
				_iapoGO.transform.localScale = _iapoTempV3;
			}

			// =============
			//	SETUP LAYER
			// =============

			if ( poolItem.spawnedItemLayer == PoolItem.PoolLayer.Ignore ){
				// Do Nothing
			} else if ( poolItem.spawnedItemLayer == PoolItem.PoolLayer.PrefabLayer ){
				SetLayerRecursively( _iapoGO, _iapoGOT, poolItem.prefabToPool.layer );
			} else if( poolItem.spawnedItemLayer == PoolItem.PoolLayer.PoolLayer ){
				SetLayerRecursively( _iapoGO, _iapoGOT, _poolItemRootLayer );
			} else if( poolItem.spawnedItemLayer == PoolItem.PoolLayer.CustomLayer ){
				SetLayerRecursively( _iapoGO, _iapoGOT, poolItem.customSpawnLayer);
			}

			// ========================
			//	SETUP POOLKIT INSTANCE
			// ========================

			// If we need to also setup the spawned object, we use a slightly different approach
			// NOTE: This is only called when no new instances are available and "Auto Create New Instances If Needed"
			// is enabled.
			if( setupPoolKitInstance ){

				// First, cache the newly spawned object
				_iapoSO = new PoolKitInstance( _iapoGO, poolItem.prefabToPool, poolItem, this );

				// If we need to add something to the array, add it to the end
				if( useArrayIndex == -1 ){

					// USING FIXED POOL
					if( _usingFixedSizePool == true ){
						Arrays.AddItemFastest( ref spawnedObjects, _iapoSO );
					
					// USING LIST
					} else {
						spawnedObjectsList.Add( _iapoSO );
					}

				// Otherwise, use the Array Index to overwrite a specific slot in the array / list
				} else {

					// USING FIXED POOL
					if( _usingFixedSizePool == true ){
						spawnedObjects[useArrayIndex] = new PoolKitInstance( _iapoGO, poolItem.prefabToPool, poolItem, this );
						
					// USING LIST
					} else {
						spawnedObjectsList[useArrayIndex] = new PoolKitInstance( _iapoGO, poolItem.prefabToPool, poolItem, this );
					}
				}

				// Now, set it up
				SetupPoolKitInstance( _iapoSO, position, rotation, parent );

			// This way is better for performance so always do it like this when possible
			} else {

				// If we need to add something new to the array, add it to the end
				if( useArrayIndex == -1 ){

					// USING FIXED POOL
					if( _usingFixedSizePool == true ){
						Arrays.AddItemFastest (  
							ref spawnedObjects, 
							new PoolKitInstance( _iapoGO, poolItem.prefabToPool, poolItem, this ) 
						);
					
					// USING LIST
					} else {
						spawnedObjectsList.Add( new PoolKitInstance( _iapoGO, poolItem.prefabToPool, poolItem, this ) );
					}

				// Otherwise, use the Array Index to overwrite a specific slot in the array	
				// NOTE: This is used at the start when preloading an Array[]
				} else {

					// USING FIXED POOL
					if( _usingFixedSizePool == true ){
						spawnedObjects[useArrayIndex] = new PoolKitInstance( _iapoGO, poolItem.prefabToPool, poolItem, this );

					// USING LIST
					} else {
						spawnedObjectsList[useArrayIndex] = new PoolKitInstance( _iapoGO, poolItem.prefabToPool, poolItem, this );
					}
				}
			}

			#if UNITY_EDITOR
				// Handle statistics
				poolItem.timeSincePoolLastInitializedAnInstance = Epoch.Current();
			#endif

			// Add another despawned instance to the poolItem Count
			poolItem.inactiveInstances++;

			// Return the Transform
			return _iapoGOT;
		}

		// ==================================================================================================================
		//	RENAME INSTANCE
		//	Names the instance using the total count of instances, padded to 4 digits
		// ==================================================================================================================

		// Helpers
		const string renamePrefix = "_PoolKit_";
		const string renameUnderscore = "_";
		const string renameSpace = " ";
		const string renameClone = "(Clone)";
		const string renameEmtpty = "";
		const string renameIndexFormat = "0000";

		// Method
		private void RenameInstance( GameObject instance, int useArrayIndex = -1 ){
			
			// Make sure we can see the PoolKit settings ...
			if(	
				// Make sure we should be renaming the object in the editor
				( Application.isEditor || !Application.isEditor && PoolKit.onlyRenameObjectsInEditor == false )
			){
				
				// Easy To Read With Pool Kit And Index
				if( PoolKit.renameObjectsInPool == PoolKit.RenameFormat.EasyToReadObjectNameWithPoolKitAndIndex ){
					instance.name = instance.name.Replace(renameClone,renameEmtpty).Replace(renameSpace, renameUnderscore) + renamePrefix+(GetTotalInstanceCount(useArrayIndex) + 1).ToString( renameIndexFormat );
					
				// Easy To Read With Index
				} else if( PoolKit.renameObjectsInPool == PoolKit.RenameFormat.EasyToReadObjectNameWithIndex ){
					instance.name = instance.name.Replace(renameClone,renameEmtpty).Replace(renameSpace, renameUnderscore) + renameUnderscore+(GetTotalInstanceCount(useArrayIndex) + 1).ToString( renameIndexFormat );

				// Object Name With Pool Kit And Index
				} else if( PoolKit.renameObjectsInPool == PoolKit.RenameFormat.ObjectNameWithPoolKitAndIndex ){
					instance.name += renamePrefix+(GetTotalInstanceCount(useArrayIndex) + 1).ToString( renameIndexFormat );

				// Object Name With Index
				} else if( PoolKit.renameObjectsInPool == PoolKit.RenameFormat.ObjectNameWithIndex ){
					instance.name += renameUnderscore+(GetTotalInstanceCount(useArrayIndex) + 1).ToString( renameIndexFormat );
				}

				// NOTE: No renaming is simply ignored.
			}
		}

		// ==================================================================================================================
		//	GET TOTAL INSTANCE COUNT
		//	Returns the total of all spawned and despawned instances in this pool. This shouldn't be used for the API!
		//	NOTE: for the fixed array, we can also send the useArrayIndex which will just return that as an index
		// ==================================================================================================================

		private int GetTotalInstanceCount( int useArrayIndex = -1){
			if( useArrayIndex != -1 ){ return useArrayIndex; }
			else if( _usingFixedSizePool == true ){ return spawnedObjects.Length; }
			return spawnedObjectsList.Count;
		}

		// ==================================================================================================================
		//	SET LAYER RECURSIVELY
		//	Sets a GameObject and all of its children to a new layer
		// ==================================================================================================================

		// Method
		private void SetLayerRecursively( GameObject go, Transform theTransform, int newLayer ){

			// End early if theTransform is null
			if( GameObjectIsNull( go ) ){ return; }

			// Set this GameObject layer to the new layer
			go.layer = newLayer;

			// Loop through the children and do the same ...
			for( int i = 0; i < theTransform.childCount; i++ ){	
				SetLayerRecursively( go, theTransform.GetChild(i), newLayer );
			}
		}

		// ==================================================================================================================
		//	LOCAL INSTANTIATION AND DESTROY OVERRIDES
		//
		//	Allows users to override how instances are instantiated or destroyed. They can do it at the pool level (here)
		//	or override it globally using PoolKit.cs in the same way.
		//
		//	NOTE: Individual pool items must be individually selected to allow "Enable Instantiate Delegates" and
		//	"Enable Destroy Delegates" to work.
		// ==================================================================================================================

		// =================
		//	CREATE INSTANCE
		// =================

		// If at least one delegate is added to this Pool.OnCreateInstance, that will be used instead!
		public delegate GameObject CreateInstanceDelegate( GameObject prefab );
		public CreateInstanceDelegate OnCreateInstance;

		// Instantiate Prefab Method
		internal GameObject InstantiatePrefab( GameObject prefab ){

			// If the user has setup delegates, intercept the instantiation here
			if (OnCreateInstance != null){
				return OnCreateInstance( prefab );

			// Otherwise, use the default Unity way ( Object.Instantiate() )	
			} else {
				return PoolKit.InstantiatePrefab( prefab );
			}
		}

		// ==================
		//	DESTROY INSTANCE
		// ==================

		// If at least one delegate is added to this Pool.DestroyInstanceDelegates, that will be used instead!
		public delegate void DestroyInstanceDelegate( GameObject instance );
		public DestroyInstanceDelegate OnDestroyInstance;

		// Destroy Instance Method
		internal void DestroyInstance( GameObject instance ){
			
			// If the user has setup delegates, override the destroy method here
			if (OnDestroyInstance != null) {
				OnDestroyInstance( instance );

			// Otherwise, use the default Unity way ( Object.Destroy() )	
			} else {
				PoolKit.DestroyInstance( instance );
			}
		}

		// ==================================================================================================================
		//	API - GET POOL ITEM
		//	Allows users to get a pool item for a specific prefab
		// ==================================================================================================================

		// API: Get the total of all spawned and despawned instances in this pool
		public PoolItem GetPoolItem( GameObject prefab ){
			
			// Find the Pool Item controlling the prefab ...
			for( int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPool == prefab){
					return poolItems[i];
				}
			}

			// If nothing was found, return null
			return null;
		}

		// ==================================================================================================================
		//	API - GET INSTANCE COUNT
		//	Returns the total of all spawned and despawned instances in this pool
		// ==================================================================================================================

		// API: Get the total of all spawned and despawned instances in this pool
		public int GetInstanceCount(){
			if( _usingFixedSizePool == true ){ return spawnedObjects.Length; }
			return spawnedObjectsList.Count;
		}

		// API: Get the the active and inactive instance count of a specific prefab in this pool (By GameObject)
		public int GetInstanceCount( GameObject prefab ){
			for(int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPool == prefab ){
					return poolItems[i].activeInstances + poolItems[i].inactiveInstances;
				}
			}
			return 0;
		}

		// API: Get the the active and inactive instance count of a specific prefab in this pool (By Transform)
		public int GetInstanceCount( Transform prefab ){
			for(int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPool == prefab.gameObject ){
					return poolItems[i].activeInstances + poolItems[i].inactiveInstances;
				}
			}
			return 0;
		}

		// API: Get the the active and inactive instance count of a specific prefab in this pool (By Name)
		public int GetInstanceCount( string prefabName ){
			for(int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPool.name == prefabName ){
					return poolItems[i].activeInstances + poolItems[i].inactiveInstances;
				}
			}
			return 0;
		}

		// ==================================================================================================================
		//	API - GET ACTIVE INSTANCE COUNT
		//	Returns the total count of active instances for either ALL prefabs or a specific active prefab (by reference or name)
		// ==================================================================================================================

		// API: Get the the count of ALL active instances in this pool
		[System.NonSerialized] private int gaic = 0;
		public int GetActiveInstanceCount(){
			gaic = 0; 
			for(int i = 0; i < poolItems.Length; i++ ){
				gaic += poolItems[i].activeInstances;
			}
			return gaic;
		}

		// API: Get the the active instance count of a specific prefab in this pool (By GameObject)
		public int GetActiveInstanceCount( GameObject prefab ){
			for(int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPool == prefab ){
					return poolItems[i].activeInstances;
				}
			}
			return 0;
		}

		// API: Get the the active instance count of a specific prefab in this pool (By Transform)
		public int GetActiveInstanceCount( Transform prefab ){
			for(int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPool == prefab.gameObject ){
					return poolItems[i].activeInstances;
				}
			}
			return 0;
		}

		// API: Get the the active instance count of a specific prefab in this pool (By Name)
		public int GetActiveInstanceCount( string prefabName ){
			for(int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPool.name == prefabName ){
					return poolItems[i].activeInstances;
				}
			}
			return 0;
		}

		// ==================================================================================================================
		//	API - GET INACTIVE INSTANCE COUNT
		//	Returns the total count of inactive instances for either ALL prefabs or a specific active prefab (by reference or name)
		// ==================================================================================================================

		// API: Get the the count of ALL inactive instances in this pool
		[System.NonSerialized] private int giic = 0;
		public int GetInactiveInstanceCount(){
			giic = 0; 
			for(int i = 0; i < poolItems.Length; i++ ){
				giic += poolItems[i].inactiveInstances;
			}
			return giic;
		}

		// API: Get the the inactive instance count of a specific prefab in this pool (By GameObject)
		public int GetInactiveInstanceCount( GameObject prefab ){
			for(int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPool == prefab ){
					return poolItems[i].inactiveInstances;
				}
			}
			return 0;
		}

		// API: Get the the inactive instance count of a specific prefab in this pool (By Transform)
		public int GetInactiveInstanceCount( Transform prefab ){
			for(int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPool == prefab.gameObject ){
					return poolItems[i].inactiveInstances;
				}
			}
			return 0;
		}

		// API: Get the the inactive instance count of a specific prefab in this pool (By Name)
		public int GetInactiveInstanceCount( string prefabName ){
			for(int i = 0; i < poolItems.Length; i++ ){
				if( poolItems[i].prefabToPool.name == prefabName ){
					return poolItems[i].inactiveInstances;
				}
			}
			return 0;
		}

		// ==================================================================================================================
		//	API - GET  INSTANCES
		//	Returns a cloned array of all the PoolKitInstances (spawned and despawned)
		//	NOTE: This is safe to give users because it is a deep copy of the original array
		// ==================================================================================================================

		// API: Get an array of both active and inactive instances in this pool
		public PoolKitInstance[] GetPoolKitInstances(){

			// Clone / Convert the correct spawned Objects list to a PoolKitInstance[]
			if( _usingFixedSizePool ){ 
				return (PoolKitInstance[]) Array.ConvertAll( spawnedObjects, a => (PoolKitInstance)PoolKitInstance.Clone(a));
			} else { 
				//return (PoolKitInstance[]) spawnedObjectsList.ToArray(); 
				return (PoolKitInstance[]) Array.ConvertAll( spawnedObjectsList.ToArray(), a => (PoolKitInstance)PoolKitInstance.Clone(a));
			}
		}

		// ==================================================================================================================
		//	API - GET  INSTANCES
		//	Returns an array of all instances, or all intances of a specific prefab
		// ==================================================================================================================

		// API: Get an array of both active and inactive instances in this pool
		public Transform[] GetInstances(){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].instanceTransform != null ){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].instanceTransform != null ){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}


		// API: Get an array of both active and inactive instances of a specific prefab in this pool ( by GameObject )
		public Transform[] GetInstances( GameObject prefab ){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].prefab == prefab &&
						spawnedObjects[i].instanceTransform != null 
					){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].prefab == prefab &&
						spawnedObjectsList[i].instanceTransform != null 
					){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}

		// API: Get an array of both active and inactive instances of a specific prefab in this pool ( by Transform )
		public Transform[] GetInstances( Transform prefab ){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].prefab == prefab.gameObject &&
						spawnedObjects[i].instanceTransform != null 
					){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].prefab == prefab.gameObject &&
						spawnedObjectsList[i].instanceTransform != null 
					){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}

		// API: Get an array of both active and inactive instances of a specific prefab in this pool ( by Transform )
		public Transform[] GetInstances( string prefabName ){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].prefabOriginalName == prefabName &&
						spawnedObjects[i].instanceTransform != null 
					){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].prefabOriginalName == prefabName &&
						spawnedObjectsList[i].instanceTransform != null 
					){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}

		// ==================================================================================================================
		//	API - GET ACTIVE INSTANCES
		//	Returns an array of all the active instances
		// ==================================================================================================================

		// API: Get an array of active instances in this pool
		public Transform[] GetActiveInstances(){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].isSpawned &&
						spawnedObjects[i].instanceTransform != null 
					){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].instanceTransform != null ){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}


		// API: Get an array of active instances of a specific prefab in this pool ( by GameObject )
		public Transform[] GetActiveInstances( GameObject prefab ){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefab == prefab &&
						spawnedObjects[i].instanceTransform != null 
					){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefab == prefab &&
						spawnedObjectsList[i].instanceTransform != null 
					){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}

		// API: Get an array of active instances of a specific prefab in this pool ( by Transform )
		public Transform[] GetActiveInstances( Transform prefab ){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefab == prefab.gameObject &&
						spawnedObjects[i].instanceTransform != null 
					){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefab == prefab.gameObject &&
						spawnedObjectsList[i].instanceTransform != null 
					){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}

		// API: Get an array of active instances of a specific prefab in this pool ( by Transform )
		public Transform[] GetActiveInstances( string prefabName ){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefabOriginalName == prefabName &&
						spawnedObjects[i].instanceTransform != null 
					){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefabOriginalName == prefabName &&
						spawnedObjectsList[i].instanceTransform != null 
					){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}

		// ==================================================================================================================
		//	API - GET INACTIVE INSTANCES
		//	Returns an array of all the inactive instances
		// ==================================================================================================================

		// API: Get an array of inactive instances in this pool
		public Transform[] GetInactiveInstances(){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( !spawnedObjects[i].isSpawned &&
						spawnedObjects[i].instanceTransform != null 
					){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( !spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].instanceTransform != null ){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}


		// API: Get an array of inactive instances of a specific prefab in this pool ( by GameObject )
		public Transform[] GetInactiveInstances( GameObject prefab ){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( !spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefab == prefab &&
						spawnedObjects[i].instanceTransform != null 
					){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( !spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefab == prefab &&
						spawnedObjectsList[i].instanceTransform != null 
					){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}

		// API: Get an array of inactive instances of a specific prefab in this pool ( by Transform )
		public Transform[] GetInactiveInstances( Transform prefab ){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( !spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefab == prefab.gameObject &&
						spawnedObjects[i].instanceTransform != null 
					){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( !spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefab == prefab.gameObject &&
						spawnedObjectsList[i].instanceTransform != null 
					){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}

		// API: Get an array of inactive instances of a specific prefab in this pool ( by Transform )
		public Transform[] GetInactiveInstances( string prefabName ){
			
			// Create a new list to copy the instances
			List<Transform> newList = new List<Transform>();

			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( !spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefabOriginalName == prefabName &&
						spawnedObjects[i].instanceTransform != null 
					){
						newList.Add( spawnedObjects[i].instanceTransform );
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and copy only the entries that have valid Transforms
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( !spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefabOriginalName == prefabName &&
						spawnedObjectsList[i].instanceTransform != null 
					){
						newList.Add( spawnedObjectsList[i].instanceTransform );
					}
				}
			}

			// Return the new list as an array
			return newList.ToArray();
		}

		// ==================================================================================================================
		//	API - HAS ACTIVE INSTANCES
		//	Utility method that returns a bool if any active instances, or a specific active prefab instance exists
		// ==================================================================================================================

		// API: Find out if this pool has ANY active instances
		public bool HasActiveInstances(){
			
			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].isSpawned ){ return true; }
				}

			// LIST
			} else {

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].isSpawned ){ return true; }
				}
			}

			// If no active instances were found, return false
			return false;
		}

		// API: Find out if this pool has a specific active instance ( by GameObject )
		public bool HasActiveInstances( GameObject prefab ){
			
			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefab == prefab
					){ 
						return true; 
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefab == prefab
					){ 
						return true; 
					}
				}
			}

			// If no active instances were found, return false
			return false;
		}

		// API: Find out if this pool has a specific active instance ( by Transform )
		public bool HasActiveInstances( Transform prefab ){
			
			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefab == prefab.gameObject
					){ 
						return true; 
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefab == prefab.gameObject
					){ 
						return true; 
					}
				}
			}

			// If no active instances were found, return false
			return false;
		}

		// API: Find out if this pool has a specific active instance ( by Name )
		public bool HasActiveInstances( string prefabName ){
			
			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefabOriginalName == prefabName
					){ 
						return true; 
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefabOriginalName == prefabName
					){ 
						return true; 
					}
				}
			}

			// If no active instances were found, return false
			return false;
		}

		// ==================================================================================================================
		//	API - HAS INACTIVE INSTANCES
		//	Utility method that returns a bool if any inactive instances, or a specific active prefab instance exists
		// ==================================================================================================================

		// API: Find out if this pool has ANY inactive instances
		public bool HasInactiveInstances(){
			
			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( !spawnedObjects[i].isSpawned ){ return true; }
				}

			// LIST
			} else {

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( !spawnedObjectsList[i].isSpawned ){ return true; }
				}
			}

			// If no inactive instances were found, return false
			return false;
		}

		// API: Find out if this pool has a specific inactive instance ( by GameObject )
		public bool HasInactiveInstances( GameObject prefab ){
			
			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( !spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefab == prefab
					){ 
						return true; 
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( !spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefab == prefab
					){ 
						return true; 
					}
				}
			}

			// If no inactive instances were found, return false
			return false;
		}

		// API: Find out if this pool has a specific inactive instance ( by Transform )
		public bool HasInactiveInstances( Transform prefab ){
			
			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( !spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefab == prefab.gameObject
					){ 
						return true; 
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( !spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefab == prefab.gameObject
					){ 
						return true; 
					}
				}
			}

			// If no inactive instances were found, return false
			return false;
		}

		// API: Find out if this pool has a specific inactive instance ( by Name )
		public bool HasInactiveInstances( string prefabName ){
			
			// ARRAY
			if( _usingFixedSizePool ){

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjects.Length; i++ ){
					if( !spawnedObjects[i].isSpawned &&
						spawnedObjects[i].prefabOriginalName == prefabName
					){ 
						return true; 
					}
				}

			// LIST
			} else {

				// Loop through the spawned objects and check their spawned status
				for( int i = 0; i < spawnedObjectsList.Count; i++ ){
					if( !spawnedObjectsList[i].isSpawned &&
						spawnedObjectsList[i].prefabOriginalName == prefabName
					){ 
						return true; 
					}
				}
			}

			// If no inactive instances were found, return false
			return false;
		}

	}
}

