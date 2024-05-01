//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	PoolItem.cs
//	This class is a container for the individual Pool Items (prefabs) that make up a "Pool". Also handles preloading.
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

	[System.Serializable]
	public class PoolItem {

		[Header("Prefab")]
		public GameObject prefabToPool;									// The prefab we'll be using to prefabPool
		[System.NonSerialized] public int prefabToPoolInstanceID = -1;	// <- this is set dynamically at start using prefabToPool
		[System.NonSerialized] public Pool pool;						// Reference to the pool this belongs to

		[Header("Pool Item Options")]
		public PoolResizeOptions poolSizeOptions = PoolResizeOptions.KeepPoolSizeFixed;
			public enum PoolResizeOptions { KeepPoolSizeFixed, ExpandPoolWithinLimit, AlwaysExpandPoolWhenNeeded  }
		// NOTE: Only KeepPoolSizeFixed is compatible with Fixed Size Pools!	
		public int poolSize = 10;									// The size of the pool at start
		public int limitPoolSize = 100;								// The max size of the pool if we're using ExpandableWithMaxLimit
		public bool keepOrganized = true;							// Should we reparent objects in the hierarchy?
		public bool recycleSpawnedObjects = false;					// If we've hit the limit, recycle an existing active object
		[System.NonSerialized] public int recycleNextIndex = 0;		// If we're recycling objects, we track it with this value

		// Spawned Item Scale
		[Header("Scale Of Spawned Instances")]
		public bool resetScaleOnEverySpawn = false;						// By default, the scale is only set on instantiate
		public PoolScale spawnedItemScale = PoolScale.Ignore;			// How should we set the prefab's scale on spawn
		public enum PoolScale { Ignore, PrefabScale, PoolScale, CustomScale, RandomRangeCustomScale, RandomRangeProportionalScale }
			public Vector3 customSpawnScale = Vector3.one;				// If using a fixed custom scale, set it here
			public Vector3 customSpawnScaleMin = Vector3.zero;			// If using a custom range, set the minimum here
			public Vector3 customSpawnScaleMax = Vector3.one;			// If using a custom scale, set the maximum here
			public float customSpawnScaleProportionalMin = 0f;			// If using a custom Proportional Scale, set minimum here
			public float customSpawnScaleProportionalMax = 1f;			// If using a custom Proportional Scale, set maximum here

		// Spawned Item Layer
		[Header("Layer Of Spawned Instances")]	
		public bool resetLayerOnEverySpawn = false;									// default = layer is only set on instantiate
		public PoolLayer spawnedItemLayer = PoolLayer.Ignore;						// How should we set the prefab's layer on spawn
			public enum PoolLayer { Ignore, PrefabLayer, PoolLayer, CustomLayer }	// Choose how to scale instantiated objects
			public int customSpawnLayer = 0;										// If using a custom layer, set it here

		[Header("Lazy Instance Preloading")]
		// NOTE: Lazy Preloading is not compatible with Fixed Size Pools!
		public bool useLazyPreloading = false;						// Should the pooled items be created gradually over time?
		public int lazyPreloadingInstancesOnAwake = 1;				// How many instances should be preloaded immediately on Awake?
		public float lazyPreloadingInitialDelay = 1f;				// Wait for lazyPreloadingDelay in seconds before starting
		public int lazyPreloadingInstancesPerPass = 1;				// How many instances should we create on every pass?
		public float lazyPreloadingDelayBetweenPasses = 0.2f;		// Do a pass every 0.2 seconds (or 5 times a second)
		[System.NonSerialized] public bool runPreloadingUpdate = false;	// If we should be preloading, this will be set to true
		
		[Header("Auto-Despawn")]
		public bool enableAutoDespawn = false;						// Should spawned items be automatically despawned?
		public DespawnMode despawnMode = DespawnMode.Countdown;		// The type of despawn mode to use
			public enum DespawnMode { Countdown, CountdownRandomRange, WaitForAudioToFinish, WaitForParticleSystemToFinish }
		public float despawnAfterHowManySeconds = 5f;				// After X seconds, the prefabPool will despawn an instance
		public float despawnRandomRangeMin = 2f;					// The minimum random time to wait before despawning
		public float despawnRandomRangeMax = 5f;					// The maximum random time to wait before despawning

		[Header("Send Notifications To Instances")]
		public Notifications notifications = Notifications.None;	// Should we send OnSpawn and OnDespawn to instances?
			public enum Notifications 	{
											None,					// [FASTEST] Don't send any notifications
											PoolKitListeners, 		// [FAST] Send notifications to cached IPoolKitListeners
											SendMessage, 			// [SLOW] Send Message to core GameObject
											BroadcastMessage 		// [SLOWEST] Broadcast Message to all instance GameObjects
										}

		[Header("Delegates")]
		public bool enableInstantiateDelegates = false;				// When an instance is being instantiated, use delegates
		public bool enableDestroyDelegates = false;					// When an instance is being destroyed, use delegates 

		[Header("Helpers")]
		[System.NonSerialized] public int activeInstances = 0;		// The active instances of this specific prefab
		[System.NonSerialized] public int inactiveInstances = 0;	// The inactive instances of this specific prefab
		public int instanceCount {
			get{ return activeInstances + inactiveInstances; }
		}

		#if UNITY_EDITOR

			// Statistics
			[Header("Pool Statistics (Editor Only)")]				// Statistics will never be enabled in builds
			public int maxInstancesSpawnedAtOnce = 0;				// the max number of instances found to be active at once
			public int totalNumberOfSpawns = 0;						// How many times was this prefab spawned?
			public int totalNumberOfDespawns = 0;					// How many times was this prefab despawned?
			public int timeSincePoolLastInitializedAnInstance = 0;	// The timestamp when this prefabPool last initialized an object
			public int timeSincePoolLastSpawnedAnInstance = 0;		// The timestamp when this prefabPool last spawned an object
			public int timeSincePoolLastDespawnedAnInstance = 0;	// The timestamp when this prefabPool last despawned an object

			// Editor Helpers
			[Header("Editor Helpers")]
			public int tab = 0;										// Helps the Editor track what tab is open
			public float instanceSpawnedProgressBar = 0f;			// Helps to display the statistics instances progress bar
			public float instanceSpawnedProgressBarLerped = 0f;		// The lerped version of the above
			public bool prefabTabIsOpen = true;						// Controls the foldout of each pool item in the pool

		#endif

		// ==================================================================================================================
		//	PRELOAD POOL
		//	Allows a pool's instances to be preloaded over time
		//	NOTE: This runs from the Pool's Update() method, and also once on Awake()
		// ==================================================================================================================

		// Preload Helpers
		bool _puFirstPass = false;
		float _puInitialDelay = 0f;
		float _delayBetweenPassCounter = 0f;
		int _lazyPreloadCount = 0;	// How many instances have been preloaded for this object?

		//IEnumerator PreloadPool( PoolItem pi ){
		internal void PreloadUpdate(){	

			// Make sure we can see the pool
			if( pool!=null && runPreloadingUpdate ){

				// Debug.LogWarning("Preloading Started!");

				// --------------------------------
				//	PRELOADING UPDATE - FIRST PASS
				// --------------------------------

				// Create the first batch of preloaded instances On Awake
				if( _puFirstPass == false ){

					// CREATE INSTANCES ON AWAKE
					for( int i = 0; i < lazyPreloadingInstancesOnAwake; i++ ){
						
						// Create a new pooled object on every pass and check that the instance was created
						if( _lazyPreloadCount < poolSize &&
							pool.InstantiatePooledObject( this ) != null 
						){
							_lazyPreloadCount++;		// increment the preload Count
							// NOTE: We don't yield here as its on Awake so we're not worried about framerate yet
						}
					}

					// RESET INITIAL DELAY COUNTDOWN
					_puInitialDelay = lazyPreloadingInitialDelay;

					// Once we've created the first batch of instances, don't do this again
					_puFirstPass = true;

					// End the first pass here
					return;
				}

				// ------------------------------------
				//	PRELOADING UPDATE - INITIAL DELAY
				// ------------------------------------

				// Countdown the initial delay
				if( _puInitialDelay > 0f ){
					_puInitialDelay -= Time.deltaTime;

				} else {

					// --------------------------------------
					//	PRELOADING UPDATE - CREATE INSTANCES
					// --------------------------------------

					// If we haven't created enough instances yet ...
					if( _lazyPreloadCount < poolSize ){

						// While we're waiting for the next instantiation pass, countdown using deltaTime
						if( _delayBetweenPassCounter > 0 ){
							_delayBetweenPassCounter -= Time.deltaTime;

						// Start the next pass
						} else {

							// Loop the amount of passes we've setup
							for( int i = 0; i < lazyPreloadingInstancesPerPass; i++ ){
								
								// Create a new pooled object on every pass and check that the instance was created
								if( _lazyPreloadCount < poolSize &&
									pool.InstantiatePooledObject( this ) != null 
								){
									_lazyPreloadCount++;	// increment the preload Count
								}
							}

							// Delay between passes
							_delayBetweenPassCounter = lazyPreloadingDelayBetweenPasses;
						}

					// If we've created enough instances, stop the preloading update
					} else {
						runPreloadingUpdate = false;
					}
				}

				// Debug.LogWarning("Preloading Finished!");

			}

		}

	}
}
