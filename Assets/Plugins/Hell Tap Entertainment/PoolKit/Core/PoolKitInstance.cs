//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	PoolKitInstance.cs
//	This class contains information about instantiated PrefabItems.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HellTap.PoolKit {

	[System.Serializable]
	public class PoolKitInstance { // SpawnedObject

	// ==================================================================================================================
	//	POOLKIT INSTANCE - SPAWNED OBJECT CLASS
	//	Tracks the currently poolItemed instances and also pre-caches any listeners, etc
	// ==================================================================================================================
		
		// The Instance
		public GameObject instance = null;			// The current instance
		public string instanceName = "";			// Cached version of instance.name
		public Transform instanceTransform = null;	// Cached Transform
		public int instanceID = -1;					// The unique ID of the actual instance created

		// Helpers
		public bool isSpawned = false;				// Is this instance active right now? 
		public bool isRectTransform = false;		// Pre-cache if this is a RectTransform (helps with parenting)
		public float currentDespawnTimer = 5f;		// If we're using automatic despawning, this is the countdown
		public float aliveTime = 0f;				// Track how long this instance has been spawned
		//public int spawnedTime = 0f;

		// The Original Prefab
		public GameObject prefab = null;			// The prefab used to create it
		public string prefabOriginalName = "";		// The original prefab name (used for quicker spawns by name)
		public int prefabInstanceID = -1;			// The unique ID of the prefab (used for faster internal comparisons)
		
		// References
		[System.NonSerialized] public PoolItem poolItem = null;		// Reference of the PoolItem controlling this instance
		[System.NonSerialized] public Pool pool = null;				// Reference to the Pool this instance belongs to
		
		// Extra references for built-in features
		public Despawner despawner = null;							// If this has a despawner, set it up!
		public AudioSource aSource = null;							// The component instance for AudioSource
		public ParticleSystem pSystem = null;						// The component instance for ParticleSystem

		// Notifications / Listeners
		public PoolItem.Notifications notifications = PoolItem.Notifications.None;	// Local version of what Notifications to use
		public IPoolKitListener[] listeners = new IPoolKitListener[0];	// Cached PoolKit listeners for this item
		public bool hasListeners = false;								// Easy way to check if listeners are available				
		
		// ===================================================================================================
		//	CONSTRUCTOR
		//	Easy way to setup a new PoolKitInstance by just passing along the core variables
		// ===================================================================================================

		// CONSTRUCTOR USED INTERNALLY BY POOLKIT
		public PoolKitInstance( GameObject theInstance, GameObject thePrefab, PoolItem thePoolItem, Pool ThePool ){
			
			// Setup the instance and pre-cache its instanceID
			instance = theInstance;
			instanceName = theInstance.name;
			instanceTransform = theInstance.transform;
			instanceID = theInstance.GetInstanceID();

			// Setup the original prefab and pre-cache its instanceID
			prefab = thePrefab;
			prefabInstanceID = thePrefab.GetInstanceID();
			prefabOriginalName = thePrefab.name;

			// Cache the pool, poolItem and some helper variables
			poolItem = thePoolItem;
			pool = ThePool;
			isSpawned = false;
			isRectTransform = (instanceTransform is RectTransform);

			// Setup Despawner
			despawner = instance.GetComponent<Despawner>();
			if( despawner != null ){ despawner.pool = pool; }

			// Setup Automatic Despawns
			if( poolItem.enableAutoDespawn ){

				// Simple Countdown
				if( poolItem.despawnMode == PoolItem.DespawnMode.Countdown ){
					currentDespawnTimer = poolItem.despawnAfterHowManySeconds;
				}

				// Random Range
				else if( poolItem.despawnMode == PoolItem.DespawnMode.CountdownRandomRange ){
					currentDespawnTimer = UnityEngine.Random.Range( poolItem.despawnRandomRangeMin, poolItem.despawnRandomRangeMax );
				}

				// Wait For Audio
				else if( poolItem.despawnMode == PoolItem.DespawnMode.WaitForAudioToFinish ){
					aSource = instance.GetComponent<AudioSource>();			// <- This will be none if it doesn't exist anyway
				}

				// Wait For Particle System
				else if( poolItem.despawnMode == PoolItem.DespawnMode.WaitForParticleSystemToFinish ){
					pSystem = instance.GetComponent<ParticleSystem>();		// <- This will be none if it doesn't exist anyway
				}
			}

			// Cache a local copy of the Pool Item's notification type
			notifications = poolItem.notifications;

			// Cache all the IPoolKitListeners if the feature is enabled
			if( notifications == PoolItem.Notifications.PoolKitListeners ){
			
				// Find all the IPoolKitListeners on this gameObject and its children
				listeners = instance.GetComponentsInChildren<IPoolKitListener>(true);

				// Setup hasListeners to help with fast checks
				if(listeners.Length > 0 ){ hasListeners = true; }
			}

		}

		// ===================================================================================================
		//	EMPTY CONSTRUCTOR
		//	Used mainly by the Clone Method
		// ===================================================================================================

		public PoolKitInstance(){

			instance = null;
			instanceName = "";
			instanceTransform = null;
			instanceID = -1;

			isSpawned = false;
			isRectTransform = false;
			currentDespawnTimer = 5;
			aliveTime = 0;

			prefab = null;
			prefabOriginalName = "";
			prefabInstanceID = -1;

			poolItem = null;
			pool = null;

			aSource = null;
			pSystem = null;

			notifications = PoolItem.Notifications.None;
			listeners = new IPoolKitListener[0];
			hasListeners = false;

		}

		// ===================================================================================================
		//	CLONE
		//	Utility Method to help deep copy PoolKitInstances
		// ===================================================================================================

		public static PoolKitInstance Clone ( PoolKitInstance pki ){

			// Create a new PKI
			PoolKitInstance newPKI = new PoolKitInstance();

			// If the original PKI is valid, copy the values
			if( pki != null ){

				newPKI.instance = pki.instance;
				newPKI.instanceName = pki.instanceName;
				newPKI.instanceTransform = pki.instanceTransform;
				newPKI.instanceID = pki.instanceID;

				newPKI.isSpawned = pki.isSpawned;
				newPKI.isRectTransform = pki.isRectTransform;
				newPKI.currentDespawnTimer = pki.currentDespawnTimer;
				newPKI.aliveTime = pki.aliveTime;

				newPKI.prefab = pki.prefab;
				newPKI.prefabOriginalName = pki.prefabOriginalName;
				newPKI.prefabInstanceID = pki.prefabInstanceID;

				newPKI.poolItem = pki.poolItem;
				newPKI.pool = pki.pool;

				newPKI.aSource = pki.aSource;
				newPKI.pSystem = pki.pSystem;

				newPKI.notifications = pki.notifications;
				newPKI.listeners = pki.listeners;
				newPKI.hasListeners = pki.hasListeners;
			}

			return newPKI;
		}

		// ========================================================================================================
		//	BROADCAST SPAWN
		//	Tell Cached Objects to Spawn
		// ========================================================================================================

		// Helper
		const string onSpawn = "OnSpawn";

		// Method
		public void BroadCastSpawn(){

			// Don't Send Any Notifications
			if( notifications == PoolItem.Notifications.None ){
				return;
			}

			// Use PoolKit Listeners
			else if( notifications == PoolItem.Notifications.PoolKitListeners && hasListeners ){
				for( int i = listeners.Length-1; i>=0; i-- ){
					if(listeners[i]!=null){ listeners[i].OnSpawn( pool ); }
				}
			}

			// Use SendMessage
			else if( notifications == PoolItem.Notifications.SendMessage ){
				instance.SendMessage( onSpawn, pool, SendMessageOptions.DontRequireReceiver );
			}

			// Use BroadcastMessage
			else if( notifications == PoolItem.Notifications.SendMessage ){
				instance.BroadcastMessage( onSpawn, pool, SendMessageOptions.DontRequireReceiver );
			}
		}

		// ========================================================================================================
		//	BROADCAST DESPAWN
		//	Tell Objects to Despawn
		// ========================================================================================================

		// Helper
		const string onDespawn = "OnDespawn";

		// Method
		public void BroadCastDespawn(){

			// Don't Send Any Notifications
			if( notifications == PoolItem.Notifications.None ){
				return;
			}

			// Use PoolKit Listeners
			else if( notifications == PoolItem.Notifications.PoolKitListeners && hasListeners ){
				for( int i = listeners.Length-1; i>=0; i-- ){
					if(listeners[i]!=null){ listeners[i].OnDespawn(); }
				}
			}

			// Use SendMessage
			else if( notifications == PoolItem.Notifications.SendMessage ){
				instance.SendMessage( onSpawn, pool, SendMessageOptions.DontRequireReceiver );
			}

			// Use BroadcastMessage
			else if( notifications == PoolItem.Notifications.SendMessage ){
				instance.BroadcastMessage( onSpawn, pool, SendMessageOptions.DontRequireReceiver );
			}
		}

	}
}
