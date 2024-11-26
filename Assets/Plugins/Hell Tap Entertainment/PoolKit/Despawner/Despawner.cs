//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Despawner.cs
//	A powerful and easy to use despawner component for PoolKit instances.
//	NOTE: This must be placed on the main prefab of the instance ( not any of its child GameObjects )
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HellTap.PoolKit {

	[DisallowMultipleComponent]
	public class Despawner : MonoBehaviour {

		// Editor Helpers
		#if UNITY_EDITOR
			public int tab = 0;												// Helps remember which tab we're using
		#endif
		
		[Header("References")]
		public Pool pool = null;				// NOTE: This will be set automatically by PoolKit when instantiated!
		Transform _thisTransform = null;
		public float aliveTime = 0f;			// How long has this instance been alive? (Spawned)
		Rigidbody rb = null;					// Cache rigidbody if there is one
		Rigidbody2D rb2D = null;				// Cache rigidbody if there is one

		[Header("When To Despawn")]
		public DespawnMode despawnMode = DespawnMode.AfterCountdown;
			public enum DespawnMode { 
				AfterCountdown = 0, 
				AfterCountdownWithRandomRange = 1, 
				AfterParticleSystemFinishes = 2, 
				AfterAudioSourceFinishes = 3, 
				AfterPhysicsOverlapEvent = 4, 
				AfterPhysicsCollisionEvent = 5, 
				AfterPhysicsTriggerEvent = 6, 
				AfterPhysicsCollision2DEvent = 7, 
				AfterPhysicsTrigger2DEvent = 8, 
				AfterPhysicsRaycastEvent = 10,				// <- NEW Raycast system
				AfterPhysicsRaycast2DEvent = 11,			// <- NEW Raycast system
				AfterCalledByScript = 9
			}
		
		// Countdown
		[Header("Despawn After Countdown")]
		public float despawnCountdown = 5f;
		public float despawnCountdownRandomMin = 4f;
		public float despawnCountdownRandomMax = 6f;
		float _currentDespawnCountdown = 0;

		// Particle System
		[Header("Despawn After Particle System")]
		public ReferenceLocation particleSystemToUse = ReferenceLocation.OnThisGameObject;
			public enum ReferenceLocation{ OnThisGameObject, OnAnotherGameObject }
		public ParticleSystem useThisParticleSystem = null;
		public bool playParticleSystemOnSpawn = true;

		// Audio Source
		[Header("Despawn After Audio Source")]
		public ReferenceLocation audioSourceToUse = ReferenceLocation.OnThisGameObject;
		public AudioSource useThisAudioSource = null;
		public bool playAudioSourceOnSpawn = true;

		// NOTE: On all colliders and trigger events, they must be attached to this GameObject.

		// Overlap Physics Events
		public OverlapType overlapType = OverlapType.Sphere3D;
			public enum OverlapType { Sphere3D, Circle2D, Box3D, Box2D }
		public Vector3 overlapOffset = Vector3.zero;
		public float overlapRadius = 1f;
		public Vector3 overlapScale = Vector3.one;
		public bool overlapAlsoDespawnAfterCountdown = true;
		public float overlapCountdown = 5f;

		// Collider Physics Events
		[Header("Despawn After Collider Event")]
		public bool colliderAlsoDespawnAfterCountdown = true;
		public float colliderCountdown = 5f;
		public bool useCollisionEnter = true;
		public bool useCollisionStay = false;
		public bool useCollisionExit = false;

		// Trigger Physics Events
		[Header("Despawn After Trigger Event")]
		public bool triggerAlsoDespawnAfterCountdown = true;
		public float triggerCountdown = 5f;
		public bool useTriggerEnter = true;
		public bool useTriggerStay = false;
		public bool useTriggerExit = false;

		// Collider 2D Physics Events
		[Header("Despawn After Collider 2D Event")]
		public bool collider2DAlsoDespawnAfterCountdown = true;
		public float collider2DCountdown = 5f;
		public bool useCollisionEnter2D = true;
		public bool useCollisionStay2D = false;
		public bool useCollisionExit2D = false;

		// Trigger 2D Physics Events
		[Header("Despawn After Trigger 2D Event")]
		public bool trigger2DAlsoDespawnAfterCountdown = true;
		public float trigger2DCountdown = 5f;
		public bool useTriggerEnter2D = true;
		public bool useTriggerStay2D = false;
		public bool useTriggerExit2D = false;

		// Raycast3D Physics Events
		[Header("Despawn After Raycast Event")]
		public bool raycast3DAlsoDespawnAfterCountdown = true;
		public float raycast3DCountdown = 5f;
		public int raycast3DmaxHits = 30;						// Size of RaycastHit array.
		public float raycast3DDistance = 1.5f;					// NOTE: Mathf.Infinity is also valid!
		public Vector3 raycast3DDirection = Vector3.forward;

		// Raycast2D Physics Events
		[Header("Despawn After Raycast 2D Event")]
		public bool raycast2DAlsoDespawnAfterCountdown = true;
		public float raycast2DCountdown = 5f;
		public int raycast2DmaxHits = 30;						// Size of RaycastHit array.
		public float raycast2DDistance = 1.5f;					// NOTE: Mathf.Infinity is also valid!
		public Vector2 raycast2DDirection = Vector2.right;
		public float raycast2DMinZDepth = -Mathf.Infinity;
		public float raycast2DMaxZDepth = Mathf.Infinity;


		// SHARED: Where should we recieve collision events? (or, where should we start raycast)
		[Header("Source Of Collision Events")]
		public CollisionSource sourceOfCollisions = CollisionSource.ThisGameObject;
			public enum CollisionSource { ThisGameObject, AnotherChildGameObject, ManualSetup }
		public GameObject collisionSourceGameObject = null;

		// SHARED: Reset Rigidbodies when they are spawned?
		public bool resetRigidbodyVelocitiesOnSpawn = true;

		// SHARED: Collision Filtering
		public LayerMask filterLayers = ~0;							// List of layers that are allowed (default = all)
		public string[] filterTags = new string[0];					// List of Tags that are allowed
		public QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;


		// Chain New Spawns To This Despawn
		[Header("Chain Spawning")]
		public Vector3 lastCollisionPoint = Vector3.zero;

		// The last GameObject this collided with collided to despawn
		public GameObject lastCollidedPhysicsGameObject = null;

		// more controls for prefab chaining here ...
		public ChainableSpawn[] chainableSpawns = new ChainableSpawn[0];


		// API: C# Delegates & Events
		[Header("Delegates")]
		public bool enableDespawnerEvents = false;

			// DELEGATE: onDespawnerDespawn
			public delegate void OnDespawnerDespawn();							// Users can subscribe to events where this 
			public OnDespawnerDespawn onDespawnerDespawn;						// despawner is despawned.

			// DELEGATE: onDespawnerCollided
			public delegate void OnDespawnerCollided( GameObject gameObject );	// Users can subscribe to events where this 
			public OnDespawnerCollided onDespawnerCollided;						// despawner collides with another object

			// DELEGATE: onDespawnerChainSpawn
			public delegate void OnDespawnerChainSpawn( Transform instance );	// Users can subscribe to events where this 
			public OnDespawnerChainSpawn onDespawnerChainSpawn;					// despawner chain spawns new instances


		// Chain New Spawns To This Despawn
		[Header("Unity Events")]
		public bool useOnDespawnUnityEvent = false;
		public UnityEvent OnDespawnUnityEvent = new UnityEvent();
		public bool useOnChainSpawnUnityEvent = false;
		public UnityTransformEvent OnChainSpawnUnityEvent;
			[System.Serializable]
			public class UnityTransformEvent : UnityEvent<Transform>{}
		public bool useOnPhysicsCollidedUnityEvent = false;
		public UnityGameObjectEvent OnPhysicsCollidedUnityEvent;
			[System.Serializable]
			public class UnityGameObjectEvent : UnityEvent<GameObject>{}

		// ==================================================================================================================
		//	CLASS: CHAINABLE SPAWN
		//	Run this when instantiated
		// ==================================================================================================================
		
		[System.Serializable]
		public class ChainableSpawn {

			[Header("References")]
			public GameObject prefab = null;					// The prefab to spawn
			public SpawnOptions spawnOptions = SpawnOptions.AlwaysSpawn;
			internal Pool pool = null;							// Reference to the pool the prefab belongs to
			internal Despawner despawner = null;				// Reference to the current despawner

			[Header("Physics Event Conditions")]
			public bool usePhysicsEventFilters = false;			// If this is true, we'll perform advanced checks
			public LayerMask filterLayers = ~0;					// List of layers that are allowed (default = all)
			public string[] filterTags = new string[0];			// List of Tags that are allowed
			public string[] filterNames = new string[0];		// List of GameObject names that are allowed

			[Header("Cached Prefab Helpers")]
			internal bool _hasPrefab = false;					// Checking with booleans is faster than GameObject == null?
			internal Vector3 _defaultPrefabPosition = Vector3.zero;
			internal Quaternion _defaultPrefabRotation = Quaternion.identity;
			internal Vector3 _defaultPrefabLocalScale = Vector3.one;			

			[Header("Spawn Position")]
			public ChainSpawnLocation spawnAt = ChainSpawnLocation.ThisTransform;
			public Transform customSpawnTransform = null;
			public Vector3 localPositionOffset = Vector3.zero;
			public bool addRandomizationRange = false;
			public Vector3 randomizationRangeMin = new Vector3(-0.1f,-0.1f,-0.1f );
			public Vector3 randomizationRangeMax = new Vector3( 0.1f, 0.1f, 0.1f );

			[Header("Spawn Rotation")]
			public RotationMode rotationMode = RotationMode.PrefabDefault;
				public enum RotationMode { PrefabDefault, ThisTransformRotation, CustomEulerAngles, RandomRotation }
			public Vector3 customRotationEulerAngles = Vector3.zero;	// <- this is also the offset for default and Transform

			[Header("Spawn Scale")]
			public ScaleMode scaleMode = ScaleMode.PrefabDefault;
				public enum ScaleMode {PrefabDefault, PoolDefault, ThisTransformScale, CustomLocalScale, RandomRangeScale, RandomRangeProportionalScale }
			public Vector3 customLocalScale = Vector3.one;
			public Vector3 customLocalScaleMin = Vector3.zero;
			public Vector3 customLocalScaleMax = Vector3.one;
			public float customLocalScaleProportionalMin = 0f;
			public float customLocalScaleProportionalMax = 1f;
			public CustomScaleOptions customScaleOptions = CustomScaleOptions.None;

			[Header("Repeat")]
			public TimesToSpawnMode timesToSpawnMode = TimesToSpawnMode.FixedNumber;
			public enum TimesToSpawnMode { FixedNumber, RandomRange }
			public int timesToSpawnThisObject = 1;									// How many intances should be spawned?
			public int minTimesToSpawnThisObject = 1;								// Min number of intances to be spawned
			public int maxTimesToSpawnThisObject = 2;								// Max number of intances to be spawned
			
			// Helpers
			Transform _spawnedT = null;
			int _currentTimesToSpawnThisObject = 1;									// <- helper for repeating spawns

			#if UNITY_EDITOR
				public bool tabIsOpen = true;										// <- Editor helper for when we foldout the chainspawning tabs
			#endif

			// Method
			internal void Spawn( bool triggeredByPhysicsEvent, GameObject referenceToLastCollidedPhysicsGameObject ){

				// Stop right away if there is no prefab
				if(_hasPrefab==false){ return; }

				// Don't spawn this if this is supposed to be a physics event and it isn't
				if( triggeredByPhysicsEvent == false && spawnOptions == SpawnOptions.SpawnOnlyOnPhysicsEvent ||

					// Don't spawn this if this IS a physics event and we shouldn't spawn
					triggeredByPhysicsEvent == true && spawnOptions == SpawnOptions.SpawnExceptOnPhysicsEvent ||

					// Don't spawn this if we should Never do it
					spawnOptions == SpawnOptions.NeverSpawn
				){ 
					return;
				}

				// If we're using a physics event with filters, return early if the checks aren't valid.
				if( spawnOptions == SpawnOptions.SpawnOnlyOnPhysicsEvent && 
					usePhysicsEventFilters == true &&
					CheckLayersAndTagsAndNames( referenceToLastCollidedPhysicsGameObject ) == false
				){
					return;		
				}

				// Calculate how many times to spawn this object
				if( timesToSpawnMode == TimesToSpawnMode.FixedNumber ){

					_currentTimesToSpawnThisObject = timesToSpawnThisObject;

				} else if( timesToSpawnMode == TimesToSpawnMode.RandomRange ){

					// Randomize the instances (we need to +1 the max int otherwise it will never be returned)
					_currentTimesToSpawnThisObject = Random.Range( minTimesToSpawnThisObject, maxTimesToSpawnThisObject+1 );

					// To be safe, make sure we never exceed the maximum int
					if(_currentTimesToSpawnThisObject>maxTimesToSpawnThisObject){ 
						_currentTimesToSpawnThisObject = maxTimesToSpawnThisObject; 
					}

				}

				// Repeat Spawns
				for( int i = 0; i < _currentTimesToSpawnThisObject; i++ ){

					// ---------------------------------------------------
					//	SPAWN / INSTANTIATE PREFAB AT POSITION & ROTATION
					//	NOTE: At first we use the Pool's default scale
					// ---------------------------------------------------

					// Make sure we have a pool to spawn with ...
					if(pool!=null){ 

						// Spawn the prefab and cache it to _spawnedT
						_spawnedT = pool.Spawn( prefab, GetSpawnPosition(), GetSpawnRotation() );

					// Otherwise, instantiate the prefab in the same way below ...	
					} else {

						// Spawn the prefab and cache it to _spawnedT
						_spawnedT = Instantiate( prefab, GetSpawnPosition(), GetSpawnRotation() ).transform;

					}

					// Make sure the object was actually spawned ...
					if(_spawnedT!=null ){

						// ---------
						//	OFFSET
						// ---------

						// Add Local Offset
						if( localPositionOffset != Vector3.zero ){ _spawnedT.localPosition += localPositionOffset; }

						// ---------
						//	SCALE
						// ---------

						// if we're not using the Pool Default scale, set the scale here ...
						if( scaleMode != ScaleMode.PoolDefault ){
							_spawnedT.localScale = GetSpawnScale();
						}

						// If we're using Delegates / Events do it here ...
						if( despawner.enableDespawnerEvents && despawner.onDespawnerChainSpawn != null ){
							despawner.onDespawnerChainSpawn( _spawnedT );
						}

						// If we're using UnityEvents do it here ...
						if( despawner.useOnChainSpawnUnityEvent && despawner.OnChainSpawnUnityEvent != null ){
							despawner.OnChainSpawnUnityEvent.Invoke( _spawnedT );
						}
					}

				}
			}

			// ==================================================================================================================
			//	CHECK LAYERS AND TAGS AND NAMES
			//	This checks a gameobject to make sure it matches the tag, layer and name filters set up in chain spawning
			// ==================================================================================================================
			
			// TURN OFF THIS DEBUG LAYERS AND TAGS THING LATER!
			bool _clatanTagFound = false;
			bool _clatanNameFound = false;
			internal bool CheckLayersAndTagsAndNames( GameObject go ){

				// If no gameobject is sent, return false right away
				if( go == null ){ return false; }

				// -----
				// TAGS
				// -----

				// If we've setup some filter tags, do the checks here ...
				if( filterTags.Length > 0 ){

					// Reset the _clatanTagFound bool
					_clatanTagFound = false;

					// Check to see if the GameObject is using an allowed tag
					for( int t = 0; t < filterTags.Length; t++ ){
						if( go.CompareTag( filterTags[t] ) ){

							// A matching tag was found, this object is valid so break the loop!
							_clatanTagFound = true;
							break;
						}
					}

					// If we didn't find an acceptable tag, return false
					if(_clatanTagFound == false){ return false; }

				}

				// ------
				// NAMES
				// ------

				// If we've setup some filter names, do the checks here ...
				if( filterNames.Length > 0 ){

					// Reset the _clatanNameFound bool
					_clatanNameFound = false;

					// Check to see if the GameObject is using an allowed name
					for( int n = 0; n < filterNames.Length; n++ ){
						if( go.name == filterNames[n] ){

							// A matching name was found, this object is valid so break the loop!
							_clatanNameFound = true;
							break;
						}
					}

					// If we didn't find a matching name, return false
					if(_clatanNameFound == false){ return false; }

				}

				// -------
				// LAYERS
				// -------

				// Make sure this GameObject's layer matches the layermask
				if ( filterLayers == (filterLayers | (1 << go.layer)) ){

					// Return true
					return true;
				}

				// Return false if the gameObject didn't match the layer
				return false;
			}

			// ==================================================================================================================
			//	GET SPAWN POSITION
			//	Get the position to use when spawning an intance
			// ==================================================================================================================

			[System.NonSerialized] Vector3 _getSpawnPosHelper = Vector3.zero;
			Vector3 GetSpawnPosition(){

				// Spawn the prefab at this transform
				if( spawnAt == ChainSpawnLocation.ThisTransform ){

					_getSpawnPosHelper = despawner._thisTransform.position;
					_getSpawnPosHelper = RandomizePosition( _getSpawnPosHelper );
					return _getSpawnPosHelper;
				
				// Spawn the prefab at a custom transform (should be a child object)
				} else if( spawnAt == ChainSpawnLocation.AnotherChildTransform ){

					_getSpawnPosHelper = customSpawnTransform != null ? customSpawnTransform.position : despawner._thisTransform.position;
					_getSpawnPosHelper = RandomizePosition( _getSpawnPosHelper );
					return _getSpawnPosHelper;
				
				// Spawn the prefab at the last Collision point
				} else if( spawnAt == ChainSpawnLocation.LastCollision ){

					_getSpawnPosHelper = despawner.lastCollisionPoint;
					_getSpawnPosHelper = RandomizePosition( _getSpawnPosHelper );
					return _getSpawnPosHelper;				
				}

				// If anything goes wrong, use this transform
				return despawner._thisTransform.position;
			}

			// ==================================================================================================================
			//	RANDOMIZE POSITION
			//	We can add some randomization to the spawn position which is great when we're repeating spawns
			// ==================================================================================================================

			Vector3 RandomizePosition( Vector3 pos ){
				// If we should randomize the position range, do it now
				if( addRandomizationRange ){
					pos += new Vector3 	(
											Random.Range(randomizationRangeMin.x, randomizationRangeMax.x),
											Random.Range(randomizationRangeMin.y, randomizationRangeMax.y),
											Random.Range(randomizationRangeMin.z, randomizationRangeMax.z)
										);
				}
				// Return the new position
				return pos;
			}

			// ==================================================================================================================
			//	GET SPAWN ROTATION
			//	Get the rotation to use when spawning an intance
			// ==================================================================================================================

			Quaternion GetSpawnRotation(){

				// Use the prefab's default Rotation
				if( rotationMode == RotationMode.PrefabDefault ){
					return _defaultPrefabRotation * Quaternion.Euler( customRotationEulerAngles );
				
				// Use this Transform's Scale
				} else if ( rotationMode == RotationMode.ThisTransformRotation ){
					return despawner._thisTransform.rotation * Quaternion.Euler( customRotationEulerAngles );
				
				// Use Custom EulerAngles
				} else if ( rotationMode == RotationMode.CustomEulerAngles ){
					return Quaternion.Euler( customRotationEulerAngles );

				// Use Random Rotation
				} else if ( rotationMode == RotationMode.RandomRotation ){
					return Random.rotation;
				}

				// If something goes wrong, use the despawner's Transform's rotation
				return despawner._thisTransform.rotation;

			}

			// ==================================================================================================================
			//	GET SPAWN SCALE
			//	Get the local scale to use when spawning an instance
			// ==================================================================================================================

			[System.NonSerialized] Vector3 _gssCurrentScale = Vector3.one;
			[System.NonSerialized] float _gssProportionalScale = 1f;
			[System.NonSerialized] float _gssVectorHelper = 1f;
			Vector3 GetSpawnScale(){

				// ==================================
				// Use the prefab's default Rotation
				// ==================================

				if( scaleMode == ScaleMode.PrefabDefault ){
					return _defaultPrefabLocalScale;
				
				// ===========================
				// Use this Transform's Scale
				// ===========================

				} else if ( scaleMode == ScaleMode.ThisTransformScale ){
					return despawner._thisTransform.localScale;

				// =======================
				// Use Custom Local Scale
				// =======================

				} else if ( scaleMode == ScaleMode.CustomLocalScale ){

					// MultiplyWithLocalScale
					if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScale ){

						// Calculate the current scale by multiplying the custom scale with the current local scale
						_gssCurrentScale.x = customLocalScale.x * despawner._thisTransform.localScale.x;
						_gssCurrentScale.y = customLocalScale.y * despawner._thisTransform.localScale.y;
						_gssCurrentScale.z = customLocalScale.z * despawner._thisTransform.localScale.z;
						
						return _gssCurrentScale;
					}

					// MultiplyWithSmallestLocalScaleVector
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithSmallestLocalScaleVector ){

						// Figure out the smallest vector
						_gssVectorHelper = Mathf.Min( Mathf.Min( despawner._thisTransform.localScale.x, despawner._thisTransform.localScale.y ), despawner._thisTransform.localScale.z );

						// Calculate the current scale by multiplying the custom scale with the current vector
						_gssCurrentScale.x = customLocalScale.x * _gssVectorHelper;
						_gssCurrentScale.y = customLocalScale.y * _gssVectorHelper;
						_gssCurrentScale.z = customLocalScale.z * _gssVectorHelper;
						
						return _gssCurrentScale;
					}

					// MultiplyWithLargestLocalScaleVector
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLargestLocalScaleVector ){

						// Figure out the largest vector
						_gssVectorHelper = Mathf.Max( Mathf.Max( despawner._thisTransform.localScale.x, despawner._thisTransform.localScale.y ), despawner._thisTransform.localScale.z );

						// Calculate the current scale by multiplying the custom scale with the current vector
						_gssCurrentScale.x = customLocalScale.x * _gssVectorHelper;
						_gssCurrentScale.y = customLocalScale.y * _gssVectorHelper;
						_gssCurrentScale.z = customLocalScale.z * _gssVectorHelper;
						
						return _gssCurrentScale;
					}

					// MultiplyWithAverageLocalScaleVector
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithAverageLocalScaleVector ){

						// Figure out the average vector
						_gssVectorHelper = ( despawner._thisTransform.localScale.x + despawner._thisTransform.localScale.y + despawner._thisTransform.localScale.z );
						if(_gssVectorHelper!=0){ _gssVectorHelper = _gssVectorHelper / 3f; }

						// Calculate the current scale by multiplying the custom scale with the current vector
						_gssCurrentScale.x = customLocalScale.x * _gssVectorHelper;
						_gssCurrentScale.y = customLocalScale.y * _gssVectorHelper;
						_gssCurrentScale.z = customLocalScale.z * _gssVectorHelper;
						
						return _gssCurrentScale;
					}

					// MultiplyWithLocalScaleX
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScaleX ){

						// Calculate the current scale by multiplying the custom scale with the local scale's X vector
						_gssCurrentScale.x = customLocalScale.x * despawner._thisTransform.localScale.x;
						_gssCurrentScale.y = customLocalScale.y * despawner._thisTransform.localScale.x;
						_gssCurrentScale.z = customLocalScale.z * despawner._thisTransform.localScale.x;
						
						return _gssCurrentScale;
					}

					// MultiplyWithLocalScaleY
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScaleY ){

						// Calculate the current scale by multiplying the custom scale with the local scale's Y vector
						_gssCurrentScale.x = customLocalScale.x * despawner._thisTransform.localScale.y;
						_gssCurrentScale.y = customLocalScale.y * despawner._thisTransform.localScale.y;
						_gssCurrentScale.z = customLocalScale.z * despawner._thisTransform.localScale.y;
						
						return _gssCurrentScale;
					}

					// MultiplyWithLocalScaleY
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScaleZ ){

						// Calculate the current scale by multiplying the custom scale with the local scale's Z vector
						_gssCurrentScale.x = customLocalScale.x * despawner._thisTransform.localScale.z;
						_gssCurrentScale.y = customLocalScale.y * despawner._thisTransform.localScale.z;
						_gssCurrentScale.z = customLocalScale.z * despawner._thisTransform.localScale.z;
						
						return _gssCurrentScale;
					}

					// Otherwise return the value directly
					return customLocalScale;
				
				// =======================
				// Use Random Range Scale
				// =======================
					
				} else if ( scaleMode == ScaleMode.RandomRangeScale ){

					// MultiplyWithLocalScale
					if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScale ){
						_gssCurrentScale.x = Random.Range( customLocalScaleMin.x, customLocalScaleMax.x ) * despawner._thisTransform.localScale.x;
						_gssCurrentScale.y = Random.Range( customLocalScaleMin.y, customLocalScaleMax.y ) * despawner._thisTransform.localScale.y;
						_gssCurrentScale.z = Random.Range( customLocalScaleMin.z, customLocalScaleMax.z ) * despawner._thisTransform.localScale.z;
						return _gssCurrentScale;
					}

					// MultiplyWithSmallestLocalScaleVector
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithSmallestLocalScaleVector ){

						// Figure out the smallest vector
						_gssVectorHelper = Mathf.Min( Mathf.Min( despawner._thisTransform.localScale.x, despawner._thisTransform.localScale.y ), despawner._thisTransform.localScale.z );

						// Setup current scale
						_gssCurrentScale.x = Random.Range( customLocalScaleMin.x, customLocalScaleMax.x ) * _gssVectorHelper;
						_gssCurrentScale.y = Random.Range( customLocalScaleMin.y, customLocalScaleMax.y ) * _gssVectorHelper;
						_gssCurrentScale.z = Random.Range( customLocalScaleMin.z, customLocalScaleMax.z ) * _gssVectorHelper;
						return _gssCurrentScale;
					}

					// MultiplyWithLargestLocalScaleVector
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLargestLocalScaleVector ){

						// Figure out the smallest vector
						_gssVectorHelper = Mathf.Max( Mathf.Max( despawner._thisTransform.localScale.x, despawner._thisTransform.localScale.y ), despawner._thisTransform.localScale.z );

						// Setup current scale
						_gssCurrentScale.x = Random.Range( customLocalScaleMin.x, customLocalScaleMax.x ) * _gssVectorHelper;
						_gssCurrentScale.y = Random.Range( customLocalScaleMin.y, customLocalScaleMax.y ) * _gssVectorHelper;
						_gssCurrentScale.z = Random.Range( customLocalScaleMin.z, customLocalScaleMax.z ) * _gssVectorHelper;
						return _gssCurrentScale;
					}

					// MultiplyWithAverageLocalScaleVector
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithAverageLocalScaleVector ){

						// Figure out the average vector
						_gssVectorHelper = ( despawner._thisTransform.localScale.x + despawner._thisTransform.localScale.y + despawner._thisTransform.localScale.z );
						if(_gssVectorHelper!=0){ _gssVectorHelper = _gssVectorHelper / 3f; }

						// Setup current scale
						_gssCurrentScale.x = Random.Range( customLocalScaleMin.x, customLocalScaleMax.x ) * _gssVectorHelper;
						_gssCurrentScale.y = Random.Range( customLocalScaleMin.y, customLocalScaleMax.y ) * _gssVectorHelper;
						_gssCurrentScale.z = Random.Range( customLocalScaleMin.z, customLocalScaleMax.z ) * _gssVectorHelper;
						return _gssCurrentScale;
						
					}

					// MultiplyWithLocalScaleX
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScaleX ){
						_gssCurrentScale.x = Random.Range( customLocalScaleMin.x, customLocalScaleMax.x ) * despawner._thisTransform.localScale.x;
						_gssCurrentScale.y = Random.Range( customLocalScaleMin.y, customLocalScaleMax.y ) * despawner._thisTransform.localScale.x;
						_gssCurrentScale.z = Random.Range( customLocalScaleMin.z, customLocalScaleMax.z ) * despawner._thisTransform.localScale.x;
						return _gssCurrentScale;
					}

					// MultiplyWithLocalScaleY
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScaleY ){
						_gssCurrentScale.x = Random.Range( customLocalScaleMin.x, customLocalScaleMax.x ) * despawner._thisTransform.localScale.y;
						_gssCurrentScale.y = Random.Range( customLocalScaleMin.y, customLocalScaleMax.y ) * despawner._thisTransform.localScale.y;
						_gssCurrentScale.z = Random.Range( customLocalScaleMin.z, customLocalScaleMax.z ) * despawner._thisTransform.localScale.y;
						return _gssCurrentScale;
					}

					// MultiplyWithLocalScaleZ
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScaleZ ){
						_gssCurrentScale.x = Random.Range( customLocalScaleMin.x, customLocalScaleMax.x ) * despawner._thisTransform.localScale.z;
						_gssCurrentScale.y = Random.Range( customLocalScaleMin.y, customLocalScaleMax.y ) * despawner._thisTransform.localScale.z;
						_gssCurrentScale.z = Random.Range( customLocalScaleMin.z, customLocalScaleMax.z ) * despawner._thisTransform.localScale.z;
						return _gssCurrentScale;
					}

					// Otherwise return it the normal way
					_gssCurrentScale.x = Random.Range( customLocalScaleMin.x, customLocalScaleMax.x );
					_gssCurrentScale.y = Random.Range( customLocalScaleMin.y, customLocalScaleMax.y );
					_gssCurrentScale.z = Random.Range( customLocalScaleMin.z, customLocalScaleMax.z );
					return _gssCurrentScale;
				
				// ====================================
				// Use Random Range Proportional Scale
				// ====================================

				} else if ( scaleMode == ScaleMode.RandomRangeProportionalScale ){
					
					// MultiplyWithLocalScale
					if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScale ){
						_gssProportionalScale = Random.Range( customLocalScaleProportionalMin, customLocalScaleProportionalMax );
						_gssCurrentScale.x = _gssProportionalScale * despawner._thisTransform.localScale.x;
						_gssCurrentScale.y = _gssProportionalScale * despawner._thisTransform.localScale.y;
						_gssCurrentScale.z = _gssProportionalScale * despawner._thisTransform.localScale.z;
						return _gssCurrentScale;
					}

					// MultiplyWithSmallestLocalScaleVector
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithSmallestLocalScaleVector ){

						// Figure out the smallest vector
						_gssVectorHelper = Mathf.Min( Mathf.Min( despawner._thisTransform.localScale.x, despawner._thisTransform.localScale.y ), despawner._thisTransform.localScale.z );

						// Setup current scale
						_gssProportionalScale = Random.Range( customLocalScaleProportionalMin, customLocalScaleProportionalMax );
						_gssCurrentScale.x = _gssProportionalScale * _gssVectorHelper;
						_gssCurrentScale.y = _gssProportionalScale * _gssVectorHelper;
						_gssCurrentScale.z = _gssProportionalScale * _gssVectorHelper;
						return _gssCurrentScale;
					}

					// MultiplyWithLargestLocalScaleVector
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLargestLocalScaleVector ){

						// Figure out the smallest vector
						_gssVectorHelper = Mathf.Max( Mathf.Max( despawner._thisTransform.localScale.x, despawner._thisTransform.localScale.y ), despawner._thisTransform.localScale.z );

						// Setup current scale
						_gssProportionalScale = Random.Range( customLocalScaleProportionalMin, customLocalScaleProportionalMax );
						_gssCurrentScale.x = _gssProportionalScale * _gssVectorHelper;
						_gssCurrentScale.y = _gssProportionalScale * _gssVectorHelper;
						_gssCurrentScale.z = _gssProportionalScale * _gssVectorHelper;
						return _gssCurrentScale;
					}

					// MultiplyWithAverageLocalScaleVector
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithAverageLocalScaleVector ){

						// Figure out the average vector
						_gssVectorHelper = ( despawner._thisTransform.localScale.x + despawner._thisTransform.localScale.y + despawner._thisTransform.localScale.z );
						if(_gssVectorHelper!=0){ _gssVectorHelper = _gssVectorHelper / 3f; }

						// Setup current scale
						_gssProportionalScale = Random.Range( customLocalScaleProportionalMin, customLocalScaleProportionalMax );
						_gssCurrentScale.x = _gssProportionalScale * _gssVectorHelper;
						_gssCurrentScale.y = _gssProportionalScale * _gssVectorHelper;
						_gssCurrentScale.z = _gssProportionalScale * _gssVectorHelper;
						return _gssCurrentScale;
						
					}

					// MultiplyWithLocalScaleX
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScaleX ){
						_gssProportionalScale = Random.Range( customLocalScaleProportionalMin, customLocalScaleProportionalMax );
						_gssCurrentScale.x = _gssProportionalScale * despawner._thisTransform.localScale.x;
						_gssCurrentScale.y = _gssProportionalScale * despawner._thisTransform.localScale.x;
						_gssCurrentScale.z = _gssProportionalScale * despawner._thisTransform.localScale.x;
						return _gssCurrentScale;
					}

					// MultiplyWithLocalScaleY
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScaleY ){
						_gssProportionalScale = Random.Range( customLocalScaleProportionalMin, customLocalScaleProportionalMax );
						_gssCurrentScale.x = _gssProportionalScale * despawner._thisTransform.localScale.y;
						_gssCurrentScale.y = _gssProportionalScale * despawner._thisTransform.localScale.y;
						_gssCurrentScale.z = _gssProportionalScale * despawner._thisTransform.localScale.y;
						return _gssCurrentScale;
					}

					// MultiplyWithLocalScaleZ
					else if( customScaleOptions == CustomScaleOptions.MultiplyWithLocalScaleZ ){
						_gssProportionalScale = Random.Range( customLocalScaleProportionalMin, customLocalScaleProportionalMax );
						_gssCurrentScale.x = _gssProportionalScale * despawner._thisTransform.localScale.z;
						_gssCurrentScale.y = _gssProportionalScale * despawner._thisTransform.localScale.z;
						_gssCurrentScale.z = _gssProportionalScale * despawner._thisTransform.localScale.z;
						return _gssCurrentScale;
					}

					// Do it the normal way - Randomize one axis and use the value for all the others
					_gssProportionalScale = Random.Range( customLocalScaleProportionalMin, customLocalScaleProportionalMax );
					_gssCurrentScale.x = _gssProportionalScale;
					_gssCurrentScale.y = _gssProportionalScale;
					_gssCurrentScale.z = _gssProportionalScale;
					return _gssCurrentScale;
				}

				// If something goes wrong, return Vector3.one
				return Vector3.one;

			}

		}

		// Enums
		public enum ChainSpawnLocation	{ 
											ThisTransform, 			// Uses this transform as the spawn location
											AnotherChildTransform, 	// Uses a custom CHILD transform as the spawn location
											LastCollision 			// Collisions = collision Point, Triggers = other position, 
										}							// default = this transform
			
		public enum SpawnOptions 		{
											AlwaysSpawn,				// This object will always be chain-spawned
											SpawnOnlyOnPhysicsEvent,	// Spawn only with physics event (ie, OnColliderEnter, etc)
											SpawnExceptOnPhysicsEvent,	// Only spawn if this is NOT a physics event
											NeverSpawn					// Never Spawn this (used for debugging, etc)
										}	

		// Enums
		public enum CustomScaleOptions	{ 
											None, 									// Does Nothing
											MultiplyWithLocalScale, 				// x result with the local scale
											MultiplyWithSmallestLocalScaleVector,	// x result with smallest vector of local scale
											MultiplyWithLargestLocalScaleVector, 	// x result with largest vector of local scale
											MultiplyWithAverageLocalScaleVector, 	// x result with average vector of local scale
											MultiplyWithLocalScaleX,				// x result with the local scale X vector
											MultiplyWithLocalScaleY,				// x result with the local scale Y vector
											MultiplyWithLocalScaleZ					// x result with the local scale Z vector
										}																

		// ==================================================================================================================
		//	AWAKE
		//	Run this when instantiated
		// ==================================================================================================================
		
		bool ranAwake = false;
		void Awake () {

			// Never allow Awake to be run twice
			if(ranAwake){ return; }

			// ------------------
			//	CACHE REFERENCES
			// ------------------

			// Cache the transform
			_thisTransform = transform;

			// Cache the Particle System
			if( particleSystemToUse == ReferenceLocation.OnThisGameObject ){
				useThisParticleSystem = GetComponent<ParticleSystem>();
			}

			// Cache the AudioSource
			if( audioSourceToUse == ReferenceLocation.OnThisGameObject ){
				useThisAudioSource = GetComponent<AudioSource>();
			}

			// Cache the Rigidbody if there is one
			rb = GetComponent<Rigidbody>();
			rb2D = GetComponent<Rigidbody2D>();

			// ------------------
			//	CHAINED PREFABS
			// ------------------

			// Loop through the chained Prefabs and set them up
			for( int i = 0; i < chainableSpawns.Length; i++ ){

				// Set the reference to the despawner
				chainableSpawns[i].despawner = this;

				// If the prefab exists, mark hasPrefab to true ...
				if( chainableSpawns[i].prefab != null ){
					chainableSpawns[i]._hasPrefab = true;

					// Cache the default Prefab Postition, Rotation and Scale
					chainableSpawns[i]._defaultPrefabPosition = chainableSpawns[i].prefab.transform.position;
					chainableSpawns[i]._defaultPrefabRotation = chainableSpawns[i].prefab.transform.rotation;
					chainableSpawns[i]._defaultPrefabLocalScale = chainableSpawns[i].prefab.transform.localScale;

					// Cache the Pool
					chainableSpawns[i].pool = PoolKit.GetPoolContainingPrefab( chainableSpawns[i].prefab );
				
				// Otherwise, make sure to mark 'hasPrefab' to false
				} else {
					chainableSpawns[i]._hasPrefab = false;
				}
			}

			// ------------------------------
			//	SETUP OVERLAP PHYSICS EVENTS
			// ------------------------------

			// NOTE: This event should ALWAYS use the despawner's gameObject.
			if( despawnMode == DespawnMode.AfterPhysicsOverlapEvent ){

				// Physics Overlap Event
				if( !gameObject.GetComponent<DespawnerEventOnPhysicsOverlap>() ){ 
					gameObject.AddComponent<DespawnerEventOnPhysicsOverlap>().despawner = this;
				}
			}

			// ------------------------
			//	GENERAL PHYSICS EVENTS
			// ------------------------

			// If we're using a collision event, set it up!
			else if( despawnMode == DespawnMode.AfterPhysicsCollisionEvent ||
				despawnMode == DespawnMode.AfterPhysicsTriggerEvent ||
				despawnMode == DespawnMode.AfterPhysicsCollision2DEvent ||
				despawnMode == DespawnMode.AfterPhysicsTrigger2DEvent ||
				despawnMode == DespawnMode.AfterPhysicsRaycastEvent ||
				despawnMode == DespawnMode.AfterPhysicsRaycast2DEvent
			){

				// Make sure GameObject Collisions Are Setup correctly
				if( sourceOfCollisions == CollisionSource.ThisGameObject ){
					collisionSourceGameObject = gameObject;

				// Make sure Another Child GameObject is setup correctly or show the user a message
				} else if ( sourceOfCollisions == CollisionSource.AnotherChildGameObject && collisionSourceGameObject == null ){
					Debug.LogWarning("POOLKIT (Despawner): The Despawner for gameObject '" + gameObject.name + "' does not have a valid GameObject set to use collision / raycast events! Please setup a Child GameObject to use in the inspector.");
				}

				// Setup the needed components automatically if we're not using ManualSetup
				if( sourceOfCollisions != CollisionSource.ManualSetup && collisionSourceGameObject != null ){

					// -----------------------
					//	SETUP COLLIDER EVENTS
					// -----------------------

					if( despawnMode == DespawnMode.AfterPhysicsCollisionEvent ){

						// On Collision Enter
						if( useCollisionEnter && !collisionSourceGameObject.GetComponent<DespawnerEventOnCollisionEnter>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnCollisionEnter>().despawner = this;
						}

						// On Collision Stay
						if( useCollisionStay && !collisionSourceGameObject.GetComponent<DespawnerEventOnCollisionStay>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnCollisionStay>().despawner = this;
						}

						// On Collision Exit
						if( useCollisionExit && !collisionSourceGameObject.GetComponent<DespawnerEventOnCollisionExit>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnCollisionExit>().despawner = this;
						}
					}

					// ----------------------
					//	SETUP TRIGGER EVENTS
					// ----------------------

					else if( despawnMode == DespawnMode.AfterPhysicsTriggerEvent ){

						// On Trigger Enter
						if( useTriggerEnter && !collisionSourceGameObject.GetComponent<DespawnerEventOnTriggerEnter>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnTriggerEnter>().despawner = this;
						}

						// On Trigger Stay
						if( useTriggerStay && !collisionSourceGameObject.GetComponent<DespawnerEventOnTriggerStay>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnTriggerStay>().despawner = this;
						}

						// On Trigger Exit
						if( useTriggerExit && !collisionSourceGameObject.GetComponent<DespawnerEventOnTriggerExit>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnTriggerExit>().despawner = this;
						}
					}

					// --------------------------
					//	SETUP COLLIDER 2D EVENTS
					// --------------------------

					else if( despawnMode == DespawnMode.AfterPhysicsCollision2DEvent ){

						// On Collision 2D Enter
						if( useCollisionEnter2D && !collisionSourceGameObject.GetComponent<DespawnerEventOnCollision2DEnter>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnCollision2DEnter>().despawner = this;
						}

						// On Collision 2D Stay
						if( useCollisionStay2D && !collisionSourceGameObject.GetComponent<DespawnerEventOnCollision2DStay>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnCollision2DStay>().despawner = this;
						}

						// On Collision 2D Exit
						if( useCollisionExit2D && !collisionSourceGameObject.GetComponent<DespawnerEventOnCollision2DExit>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnCollision2DExit>().despawner = this;
						}
					}

					// -------------------------
					//	SETUP TRIGGER 2D EVENTS
					// -------------------------

					else if( despawnMode == DespawnMode.AfterPhysicsTrigger2DEvent ){

						// On Trigger 2D Enter
						if( useTriggerEnter2D && !collisionSourceGameObject.GetComponent<DespawnerEventOnTrigger2DEnter>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnTrigger2DEnter>().despawner = this;
						}

						// On Trigger 2D Stay
						if( useTriggerStay2D && !collisionSourceGameObject.GetComponent<DespawnerEventOnTrigger2DStay>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnTrigger2DStay>().despawner = this;
						}

						// On Trigger 2D Exit
						if( useTriggerExit2D && !collisionSourceGameObject.GetComponent<DespawnerEventOnTrigger2DExit>() ){ 
							collisionSourceGameObject.AddComponent<DespawnerEventOnTrigger2DExit>().despawner = this;
						}
					}

					// -------------------------
					//	SETUP RAYCAST 3D EVENT
					// -------------------------

					else if( despawnMode == DespawnMode.AfterPhysicsRaycastEvent ){

						// Setup the despawner reference
						if( !collisionSourceGameObject.GetComponent<DespawnerEventRaycast3D>() ){
							collisionSourceGameObject.AddComponent<DespawnerEventRaycast3D>().despawner = this;
						}

						// Also, recreate the RaycastHit array on awake
						collisionSourceGameObject.GetComponent<DespawnerEventRaycast3D>()._raycastHitResults = new RaycastHit[raycast3DmaxHits];
					}

					// -------------------------
					//	SETUP RAYCAST 2D EVENT
					// -------------------------

					else if( despawnMode == DespawnMode.AfterPhysicsRaycast2DEvent ){

						// Setup the despawner reference
						if( !collisionSourceGameObject.GetComponent<DespawnerEventRaycast2D>() ){
							collisionSourceGameObject.AddComponent<DespawnerEventRaycast2D>().despawner = this;
						}

						// Also, recreate the RaycastHit array on awake
						collisionSourceGameObject.GetComponent<DespawnerEventRaycast2D>()._raycastHitResults = new RaycastHit2D[raycast2DmaxHits];
					}

				}
			}


			// Never run awake again
			ranAwake = true;
		}

		// ==================================================================================================================
		//	ON ENABLE
		//	Use this for initialization
		// ==================================================================================================================
		
		void OnEnable () {

			// Reset aliveTime
			aliveTime = 0f;

			// Reset Rigibody velocities
			if( rb != null && resetRigidbodyVelocitiesOnSpawn ){
				rb.linearVelocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}

			// Reset Rigibody2D velocities
			if( rb2D != null && resetRigidbodyVelocitiesOnSpawn ){
				rb2D.linearVelocity = Vector2.zero;
				rb2D.angularVelocity = 0f;
			}

			// Reset last collided physics gameObject
			lastCollidedPhysicsGameObject = null;

			// ----------------
			//	COUNTDOWN MODE
			// ----------------

			if( despawnMode == DespawnMode.AfterCountdown ){ 
				_currentDespawnCountdown = despawnCountdown;
			
			} 

			// -----------------------------
			//	COUNTDOWN RANDOM RANGE MODE
			// -----------------------------

			else if( despawnMode == DespawnMode.AfterCountdownWithRandomRange ){
				_currentDespawnCountdown = Random.Range( despawnCountdownRandomMin, despawnCountdownRandomMax );
			}

			// ----------------------
			//	PARTICLE SYSTEM MODE
			// ----------------------

			else if ( despawnMode == DespawnMode.AfterParticleSystemFinishes ){
				
				// Make sure we can see the Particle System first ...
				if( useThisParticleSystem != null ){
					
					// Play the Particle System
					if(playParticleSystemOnSpawn){ useThisParticleSystem.Play(); }

				// Otherwise, show warning and revert to countdown mode	
				} else {
					Debug.LogWarning( "POOLKIT (Despawner): " + _thisTransform.name + " could not find a Particle System to use. Automatically switching to Countdown Mode." );
					despawnMode = DespawnMode.AfterCountdown;
					_currentDespawnCountdown = despawnCountdown;
				}
			} 	

			// ----------------------
			//	AUDIOSOURCE MODE
			// ----------------------

			else if ( despawnMode == DespawnMode.AfterAudioSourceFinishes ){
				
				// Make sure we can see the AudioSource ...
				if( useThisAudioSource != null ){
					
					// Play the Audio
					if(playAudioSourceOnSpawn){ useThisAudioSource.Play(); }

				// Otherwise, show warning and revert to countdown mode	
				} else {
					Debug.LogWarning( "POOLKIT (Despawner): " + _thisTransform.name + " could not find an AudioSource to use. Automatically switching to Countdown Mode." );
					despawnMode = DespawnMode.AfterCountdown;
					_currentDespawnCountdown = despawnCountdown;
				}
			}

			// ----------------------
			//	OVERLAP MODE
			// ----------------------

			else if ( despawnMode == DespawnMode.AfterPhysicsOverlapEvent ){
				
				// Reset the countdown
				_currentDespawnCountdown = overlapCountdown;
			}

			// ----------------------
			//	COLLIDER MODE
			// ----------------------

			else if ( despawnMode == DespawnMode.AfterPhysicsCollisionEvent ){
				
				// Reset the countdown
				_currentDespawnCountdown = colliderCountdown;
			}

			// ----------------------
			//	TRIGGER MODE
			// ----------------------

			else if ( despawnMode == DespawnMode.AfterPhysicsTriggerEvent ){
				
				// Reset the countdown
				_currentDespawnCountdown = triggerCountdown;
			}

			// ----------------------
			//	COLLIDER 2D MODE
			// ----------------------

			else if ( despawnMode == DespawnMode.AfterPhysicsCollision2DEvent ){
				
				// Reset the countdown
				_currentDespawnCountdown = collider2DCountdown;
			}

			// ----------------------
			//	TRIGGER 2D MODE
			// ----------------------

			else if ( despawnMode == DespawnMode.AfterPhysicsTrigger2DEvent ){
				
				// Reset the countdown
				_currentDespawnCountdown = trigger2DCountdown;
			}

			// ----------------------
			//	RAYCAST MODE
			// ----------------------

			else if ( despawnMode == DespawnMode.AfterPhysicsRaycastEvent ){
				
				// Reset the countdown
				_currentDespawnCountdown = raycast3DCountdown;
			}

		}
		
		// ==================================================================================================================
		//	UPDATE
		//	Update is called once per frame
		// ==================================================================================================================
		
		void Update () {

			// Increment Alive Time
			aliveTime += Time.deltaTime;

			// ===============
			//	UPDATE METHOD
			// ===============
			
			// Run the correct update method based on the despawn mode
			if( despawnMode == DespawnMode.AfterCountdown ){ UpdateCountdown(); }
			else if( despawnMode == DespawnMode.AfterCountdownWithRandomRange ){ UpdateCountdown(); }
			else if( despawnMode == DespawnMode.AfterParticleSystemFinishes ){ UpdateParticleSystem(); }
			else if( despawnMode == DespawnMode.AfterAudioSourceFinishes ){ UpdateAudioSource(); }
			else if( despawnMode == DespawnMode.AfterPhysicsOverlapEvent ){ UpdateOverlapTimer(); }
			else if( despawnMode == DespawnMode.AfterPhysicsCollisionEvent ){ UpdateColliderTimer(); }
			else if( despawnMode == DespawnMode.AfterPhysicsTriggerEvent ){ UpdateTriggerTimer(); }
			else if( despawnMode == DespawnMode.AfterPhysicsCollision2DEvent ){ UpdateCollider2DTimer(); }
			else if( despawnMode == DespawnMode.AfterPhysicsTrigger2DEvent ){ UpdateTrigger2DTimer(); }
			else if( despawnMode == DespawnMode.AfterPhysicsRaycastEvent ){ UpdateRaycastTimer(); }

			

			// ===============
			//	API DESPAWN
			// ===============

			// NOTE: Users can use their own countdowns in addition to the built in despawn methods.
			// If we're waiting for the user to trigger a despawn by script, do it here ...
			else if( _userTriggeredCountdown == true ){

				// Countdown and then despawn
				if( _userDespawnCountdown > 0 ){ _userDespawnCountdown -= Time.deltaTime; }
				else { Despawn(); }
			}
		}

		// ==================================================================================================================
		//	UPDATE: COUNTDOWN
		//	The Update Method when we're tracking a countdown
		// ==================================================================================================================
		
		void UpdateCountdown(){

			// Do The Countdown
			if ( _currentDespawnCountdown > 0f ){ _currentDespawnCountdown -= Time.deltaTime; }
			else { 
				lastCollisionPoint = _thisTransform.position;
				Despawn();
			}
		}

		// ==================================================================================================================
		//	UPDATE: PARTICLE SYSTEM
		//	The Update Method when we're tracking a Particle System
		// ==================================================================================================================
		
		void UpdateParticleSystem(){

			// After a tiny amount of time (so we dont despawn it right away), check if the particle system ended
			if( aliveTime > 0.1f && useThisParticleSystem.IsAlive() == false ){
				lastCollisionPoint = _thisTransform.position;
				Despawn();
			}
		}

		// ==================================================================================================================
		//	UPDATE: AUDIO SOURCE
		//	The Update Method when we're tracking an AudioSource
		// ==================================================================================================================
		
		void UpdateAudioSource(){

			// After a tiny amount of time (so we dont despawn it right away), check if the sound ended
			if( aliveTime > 0.1f && useThisAudioSource.isPlaying == false ){
				lastCollisionPoint = _thisTransform.position;
				Despawn();
			}
		}

		// ==================================================================================================================
		//	OVERLAP EVENTS
		//	These methods are used when we're tracking Physics Overlap Events
		// ==================================================================================================================
		
		// UPDATE OVERLAP TIMER
		void UpdateOverlapTimer(){

			// Do The Countdown if we're also going to despawn after the countdown
			if( overlapAlsoDespawnAfterCountdown == true ){
				if ( _currentDespawnCountdown > 0f ){ _currentDespawnCountdown -= Time.deltaTime; }
				else { 

					lastCollisionPoint = _thisTransform.position;
					Despawn(); 
				}
			}
		}


		// ==================================================================================================================
		//	COLLISION EVENTS
		//	These methods are used when we're tracking Collision Events
		// ==================================================================================================================
		
		// UPDATE COLLIDER TIMER
		void UpdateColliderTimer(){

			// Do The Countdown if we're also going to despawn after the countdown
			if( colliderAlsoDespawnAfterCountdown == true ){
				if ( _currentDespawnCountdown > 0f ){ _currentDespawnCountdown -= Time.deltaTime; }
				else { 

					lastCollisionPoint = _thisTransform.position;
					Despawn(); 
				}
			}
		}
		
		/*
		// ON COLLISION ENTER
		void OnCollisionEnter( Collision collision ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterColliderEvent && useCollisionEnter &&
				// Make sure the collision is valid ...
				CheckColliderLimits( colliderLimits, collision.gameObject, colliderTag, colliderLayer )
			){
				lastCollisionPoint = collision.contacts[0].point;
				Despawn();
			}
		}

		// ON COLLISION STAY
		void OnCollisionStay( Collision collision ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterColliderEvent && useCollisionStay &&
				// Make sure the collision is valid ...
				CheckColliderLimits( colliderLimits, collision.gameObject, colliderTag, colliderLayer )
			){
				lastCollisionPoint = collision.contacts[0].point;
				Despawn();
			}
		}

		// ON COLLISION EXIT
		void OnCollisionExit( Collision collision ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterColliderEvent && useCollisionExit &&
				// Make sure the collision is valid ...
				CheckColliderLimits( colliderLimits, collision.gameObject, colliderTag, colliderLayer )
			){
				lastCollisionPoint = collision.contacts[0].point;
				Despawn();
			}
		}
		*/

		// ==================================================================================================================
		//	TRIGGER EVENTS
		//	These methods are used when we're tracking Trigger Events
		// ==================================================================================================================
		
		// UPDATE TRIGGER TIMER
		void UpdateTriggerTimer(){

			// Do The Countdown if we're also going to despawn after the countdown
			if( triggerAlsoDespawnAfterCountdown == true ){
				if ( _currentDespawnCountdown > 0f ){ _currentDespawnCountdown -= Time.deltaTime; }
				else { 

					lastCollisionPoint = _thisTransform.position;
					Despawn(); 
				}
			}
		}

		/*

		// ON TRIGGER ENTER
		void OnTriggerEnter( Collider other ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterTriggerEvent && useTriggerEnter &&
				// Make sure the collision is valid ...
				CheckColliderLimits( triggerLimits, other.gameObject, triggerTag, triggerLayer )
			){
				lastCollisionPoint = other.transform.position;
				Despawn();
			}
		}

		// ON TRIGGER STAY
		void OnTriggerStay( Collider other ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterTriggerEvent && useTriggerStay &&
				// Make sure the collision is valid ...
				CheckColliderLimits( triggerLimits, other.gameObject, triggerTag, triggerLayer )
			){
				lastCollisionPoint = other.transform.position;
				Despawn();
			}
		}

		// ON TRIGGER EXIT
		void OnTriggerExit( Collider other ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterTriggerEvent && useTriggerExit &&
				// Make sure the collision is valid ...
				CheckColliderLimits( triggerLimits, other.gameObject, triggerTag, triggerLayer )
			){
				lastCollisionPoint = other.transform.position;
				Despawn();
			}
		}
		*/

		// ==================================================================================================================
		//	COLLISION 2D EVENTS
		//	These methods are used when we're tracking Collision2D Events
		// ==================================================================================================================
		
		// UPDATE COLLIDER 2D TIMER
		void UpdateCollider2DTimer(){

			// Do The Countdown if we're also going to despawn after the countdown
			if( collider2DAlsoDespawnAfterCountdown == true ){
				if ( _currentDespawnCountdown > 0f ){ _currentDespawnCountdown -= Time.deltaTime; }
				else { 
					lastCollisionPoint = _thisTransform.position;
					Despawn();
				}
			}
		}
		/*
		// ON COLLISION 2D ENTER
		void OnCollisionEnter2D( Collision2D collision ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterCollider2DEvent && useCollisionEnter2D &&
				// Make sure the collision is valid ...
				CheckColliderLimits( collider2DLimits, collision.gameObject, collider2DTag, collider2DLayer )
			){
				lastCollisionPoint = collision.contacts[0].point;
				Despawn();
			}
		}

		// ON COLLISION 2D STAY
		void OnCollisionStay2D( Collision2D collision ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterCollider2DEvent && useCollisionStay2D &&
				// Make sure the collision is valid ...
				CheckColliderLimits( collider2DLimits, collision.gameObject, collider2DTag, collider2DLayer )
			){
				lastCollisionPoint = collision.contacts[0].point;
				Despawn();
			}
		}

		// ON COLLISION 2D EXIT
		void OnCollisionExit2D( Collision2D collision ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterCollider2DEvent && useCollisionExit2D &&
				// Make sure the collision is valid ...
				CheckColliderLimits( collider2DLimits, collision.gameObject, collider2DTag, collider2DLayer )
			){
				lastCollisionPoint = collision.contacts[0].point;
				Despawn();
			}
		}
		*/
		

		// ==================================================================================================================
		//	TRIGGER 2D EVENTS
		//	These methods are used when we're tracking Trigger2D Events
		// ==================================================================================================================
		
		// UPDATE TRIGGER 2D TIMER
		void UpdateTrigger2DTimer(){

			// Do The Countdown if we're also going to despawn after the countdown
			if( trigger2DAlsoDespawnAfterCountdown == true ){
				if ( _currentDespawnCountdown > 0f ){ _currentDespawnCountdown -= Time.deltaTime; }
				else { 
					lastCollisionPoint = _thisTransform.position;
					Despawn();
				}
			}
		}

		/*
		// ON TRIGGER 2D ENTER
		void OnTriggerEnter2D( Collider2D other ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterTrigger2DEvent && useTriggerEnter2D &&
				// Make sure the collision is valid ...
				CheckColliderLimits( trigger2DLimits, other.gameObject, trigger2DTag, trigger2DLayer )
			){
				lastCollisionPoint = other.transform.position;
				Despawn();
			}
		}

		// ON TRIGGER 2D STAY
		void OnTriggerStay2D( Collider2D other ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterTrigger2DEvent && useTriggerStay2D &&
				// Make sure the collision is valid ...
				CheckColliderLimits( trigger2DLimits, other.gameObject, trigger2DTag, trigger2DLayer )
			){
				lastCollisionPoint = other.transform.position;
				Despawn();
			}
		}

		// ON TRIGGER 2D EXIT
		void OnTriggerExit2D( Collider2D other ){
			// Make sure we can track collisions in this method ...
			if( despawnMode == DespawnMode.AfterTrigger2DEvent && useTriggerExit2D &&
				// Make sure the collision is valid ...
				CheckColliderLimits( trigger2DLimits, other.gameObject, trigger2DTag, trigger2DLayer )
			){
				lastCollisionPoint = other.transform.position;
				Despawn();
			}
		}
	*/

		// ==================================================================================================================
		//	CHECK COLLIDER LIMITS
		//	This method helps figure out if a collision is valid based on the user's preferences ...
		// ==================================================================================================================
		/*
		// Helper method to check if a collision with a GameObject is valid ...
		internal bool CheckColliderLimits( ColliderLimits cl, GameObject go, string tag, int layer ){
			
			if( // If we're allowing all gameobjects ...
				cl == ColliderLimits.AllGameObjects ||

				// If we're checking tags ...
				//cl == ColliderLimits.OnlyGameObjectsWithTag && go.tag == tag ||
				cl == ColliderLimits.OnlyGameObjectsWithTag && go.CompareTag( tag ) ||

				// If we're checking Layer ...
				cl == ColliderLimits.OnlyGameObjectsWithLayer && go.layer == layer ||

				// If we're checking both tags AND Layer ...
				//cl == ColliderLimits.OnlyGameObjectsWithTagAndLayer && go.tag == tag && go.layer == layer ||
				cl == ColliderLimits.OnlyGameObjectsWithTagAndLayer && go.layer == layer && go.CompareTag( tag ) ||

				// If we're checking EITHER tags OR Layer ...
				//cl == ColliderLimits.OnlyGameObjectsWithTagOrLayer && ( go.tag == tag || go.layer == layer )
				cl == ColliderLimits.OnlyGameObjectsWithTagAndLayer && ( go.layer == layer || go.CompareTag( tag ) )
			){

				return true;
			}

			// Return if the logic above didn't pass
			return false;
		}
		*/

		// ==================================================================================================================
		//	RAYCAST EVENTS
		//	These methods are used when we're tracking Trigger2D Events
		// ==================================================================================================================
		
		// UPDATE RAYCAST TIMER
		void UpdateRaycastTimer(){

			// Do The Countdown if we're also going to despawn after the countdown
			if( raycast3DAlsoDespawnAfterCountdown == true ){
				if ( _currentDespawnCountdown > 0f ){ _currentDespawnCountdown -= Time.deltaTime; }
				else { 
					lastCollisionPoint = _thisTransform.position;
					Despawn();
				}
			}
		}

		// ==================================================================================================================
		//	CHECK LAYERS AND TAGS
		//	This checks a gameobject to make sure it matches the global tag and layer filters set up by the user
		// ==================================================================================================================
		
		// TURN OFF THIS DEBUG LAYERS AND TAGS THING LATER!
		bool _clatTagFound = false;
		internal bool CheckLayersAndTags( GameObject go ){

			// If we've setup some filter tags, do the checks here ...
			if( filterTags.Length > 0 ){

				// Reset the _clatTagFound bool
				_clatTagFound = false;

				// Check to see if the GameObject is using an allowed tag
				for( int t = 0; t < filterTags.Length; t++ ){
					if( go.CompareTag( filterTags[t] ) ){

						// A matching tag was found, this object is valid so break the loop!
						_clatTagFound = true;
						break;
					}
				}

				// If we didn't find an acceptable tag, return false
				if(_clatTagFound == false){ return false; }

			}

			// Make sure this GameObject's layer matches the layermask
			if ( filterLayers == (filterLayers | (1 << go.layer)) ){

				// Return true
				return true;
			}

			// Return false if the gameObject didn't match the layer
			return false;
		}





		// ==================================================================================================================
		//	DESPAWN
		//	Despawn this instance now! NOTE: This is also available via the API
		// ==================================================================================================================
		
		// API: Helper variables
		bool _userTriggeredCountdown = false;
		float _userDespawnCountdown = 0f;
		
		// API: Allow user to trigger a despawn using a countdown
		public void Despawn( float despawnCountdown ){
			_userDespawnCountdown = despawnCountdown;
			_userTriggeredCountdown = true;
		}

		// API: Allow user to trigger a despawn using a random countdown
		public void Despawn( float minimumDespawnCountdown, float maximimDespawnCountdown ){
			_userDespawnCountdown = Random.Range( minimumDespawnCountdown, maximimDespawnCountdown );
			_userTriggeredCountdown = true;
		}


		// Main Despawn Method
		public void Despawn( bool triggeredByPhysicsEvent = false ){

			// Reset Alive Time
			aliveTime = 0f;

			// ----------------
			//	CHAIN SPAWNING
			// ----------------

			// Loop through the chained Prefabs set them up
			for( int i = 0; i < chainableSpawns.Length; i++ ){
				chainableSpawns[i].Spawn( triggeredByPhysicsEvent, lastCollidedPhysicsGameObject );
			}

			// ----------------------------
			// DO UNITY EVENTS / DELEGATES
			// ----------------------------

			// DELEGATES
			// Trigger Collision Delegate Event if this is a physics event and we have enabled them
			if( triggeredByPhysicsEvent == true && enableDespawnerEvents == true && onDespawnerCollided != null ){
				onDespawnerCollided( lastCollidedPhysicsGameObject ); 
			}

			// If we're using Delegates / Events do it here ...
			if( enableDespawnerEvents && onDespawnerDespawn != null ){ onDespawnerDespawn(); }

			// UNITY EVENTS
			// Trigger Collision Unity Events if this is a physics event and we have enabled them
			if( triggeredByPhysicsEvent == true && useOnPhysicsCollidedUnityEvent == true && OnPhysicsCollidedUnityEvent != null ){
				OnPhysicsCollidedUnityEvent.Invoke( lastCollidedPhysicsGameObject ); 
			}

			// Trigger Despawner Unity Events if we have enabled them
			if( useOnDespawnUnityEvent == true && OnDespawnUnityEvent != null ){ OnDespawnUnityEvent.Invoke(); }

			// ----------------
			// RESET VALUES
			// ----------------

			// Clean Up User despawn variables
			_userTriggeredCountdown = false;
			_userDespawnCountdown = 0f;

			// -------------
			//	DESPAWN
			// -------------

			// Tell Pool to despawn
			if(pool!=null){ pool.Despawn(_thisTransform); }

			// If we couldn't find the pool, we need to destroy it
			else {

				// Show a debug message if we're debugging pools
				if( PoolKit.debugPoolKit ){ 
					Debug.LogWarning( "POOLKIT (Despawner): " + _thisTransform.name + " could not find its original Pool to despawn to. It will be destroyed instead." ); 
				}

				// Destroy the gameObject
				Destroy( gameObject );
			}
		}

		// ==================================================================================================================
		//	ON DRAW GIZMOS SELECTED
		//	Editor Visualization of the Despawner Colliders
		// ==================================================================================================================
		
		#if UNITY_EDITOR
			Vector3 _gizmoScaleHelper = Vector3.one;
			Vector3 _sphereScaleHelper = Vector3.one;
			Vector3 _gizmoRayHelper = Vector3.one;
			Vector3 _gizmoRayHelper2 = Vector3.one;
			Transform _gizmoRayTransform = null;
			Color dimmedRayColor = new Color(1f,1f,0f,0.25f);
			Color actualRayColor = Color.red;

			void OnDrawGizmosSelected(){

				// ------------------
				//	RAYCAST 2D EVENT
				// ------------------

				// Show the collider visualizations for Physics Raycast 2D Events
				if( despawnMode == DespawnMode.AfterPhysicsRaycast2DEvent ){

					// Setup a ray Direction
					_gizmoRayHelper = Vector3.zero;
					_gizmoRayHelper2 = Vector3.zero;
					_gizmoRayTransform = null;

					// Use Ray Direction From this GameObject
					if( sourceOfCollisions == CollisionSource.ThisGameObject ){
						
						// Cache the transform we're using to shoot the ray
						_gizmoRayTransform = transform;

					// Use Ray Direction From Another GameObject
					} else if ( sourceOfCollisions == CollisionSource.AnotherChildGameObject && collisionSourceGameObject != null ){ 
        				
        				// Cache the transform we're using to shoot the ray
						_gizmoRayTransform = collisionSourceGameObject.transform;

					}

					// if the gizmo ray hasn't been set, end early!
					if(_gizmoRayTransform==null){ return; }	

					// Setup the rays we'll be drawing
					_gizmoRayHelper = _gizmoRayTransform.TransformDirection( raycast2DDirection.normalized ) * raycast2DDistance;
					_gizmoRayHelper2 = _gizmoRayTransform.TransformDirection( raycast2DDirection.normalized ) * 10000f;				

					// Draw Long Dimmed Out Direction Line
					Gizmos.color = dimmedRayColor;
					Gizmos.DrawRay( _gizmoRayTransform.position, _gizmoRayHelper2 );

					// Draw Exact Red Ray Line
					Gizmos.color = actualRayColor;
					Gizmos.DrawRay( _gizmoRayTransform.position, _gizmoRayHelper );

				// ------------------
				//	RAYCAST 3D EVENT
				// ------------------

				// Show the collider visualizations for Physics Raycast Events
				} else if( despawnMode == DespawnMode.AfterPhysicsRaycastEvent ){

					// Setup a ray Direction
					_gizmoRayHelper = Vector3.zero;
					_gizmoRayHelper2 = Vector3.zero;
					_gizmoRayTransform = null;

					// Use Ray Direction From this GameObject
					if( sourceOfCollisions == CollisionSource.ThisGameObject ){
						
						// Cache the transform we're using to shoot the ray
						_gizmoRayTransform = transform;

					// Use Ray Direction From Another GameObject
					} else if ( sourceOfCollisions == CollisionSource.AnotherChildGameObject && collisionSourceGameObject != null ){ 
        				
        				// Cache the transform we're using to shoot the ray
						_gizmoRayTransform = collisionSourceGameObject.transform;

					}

					// if the gizmo ray hasn't been set, end early!
					if(_gizmoRayTransform==null){ return; }	

					// Setup the rays we'll be drawing
					_gizmoRayHelper = _gizmoRayTransform.TransformDirection( raycast3DDirection.normalized ) * raycast3DDistance;
					_gizmoRayHelper2 = _gizmoRayTransform.TransformDirection( raycast3DDirection.normalized ) * 10000f;				

					// Draw Long Dimmed Out Direction Line
					Gizmos.color = dimmedRayColor;
					Gizmos.DrawRay( _gizmoRayTransform.position, _gizmoRayHelper2 );

					// Draw Exact Red Ray Line
					Gizmos.color = actualRayColor;
					Gizmos.DrawRay( _gizmoRayTransform.position, _gizmoRayHelper );
					

				// ---------------
				//	OVERLAP EVENT
				// ---------------

				// Show the collider visualizations for Physics Overlap Events
				} else if( despawnMode == DespawnMode.AfterPhysicsOverlapEvent ){

					// Set Gizmos Color to red
					Gizmos.color = Color.red;

					// Overlap Sphere 3D
					if( overlapType == OverlapType.Sphere3D ){

						// Scale the sphere using the largest of the scale axis
						_sphereScaleHelper.x = Mathf.Max( Mathf.Max(transform.lossyScale.x, transform.lossyScale.y), transform.lossyScale.z );
						_sphereScaleHelper.y = _sphereScaleHelper.x;
						_sphereScaleHelper.z = _sphereScaleHelper.x;

						// 3D Matrix
						Gizmos.matrix = Matrix4x4.TRS( transform.position, transform.rotation, _sphereScaleHelper );

						// Draw a sphere
						//Gizmos.DrawWireSphere(transform.position +  transform.TransformDirection( overlapOffset ), overlapRadius );
						Gizmos.DrawWireSphere( overlapOffset, overlapRadius );

					// Overlap Circle 2D
					} else if( overlapType == OverlapType.Circle2D ){

						// Scale the sphere using the largest of the scale axis
						_sphereScaleHelper.x = Mathf.Max( Mathf.Max(transform.lossyScale.x, transform.lossyScale.y), transform.lossyScale.z );
						_sphereScaleHelper.y = _sphereScaleHelper.x;
						_sphereScaleHelper.z = _sphereScaleHelper.x;

						// 2D Matrix
						Gizmos.matrix = Matrix4x4.TRS( transform.position, Quaternion.Euler(0,0, transform.eulerAngles.z), _sphereScaleHelper );

						// Draw a sphere
						//Gizmos.DrawWireSphere(transform.position +  transform.TransformDirection( overlapOffset ), overlapRadius );
						Gizmos.DrawWireSphere( overlapOffset, overlapRadius );


					// Overlap Box 3D
					} else if( overlapType == OverlapType.Box3D ){
						
						// 3D Matrix
						Gizmos.matrix = Matrix4x4.TRS( transform.position, transform.rotation, transform.lossyScale );

						// Draw a cube
						//Gizmos.DrawWireCube(transform.position +  transform.TransformDirection( overlapOffset ), overlapScale );
						Gizmos.DrawWireCube( overlapOffset, overlapScale );

					// Overlap Box 2D
					} else if( overlapType == OverlapType.Box2D ){
						
						// 2D Matrix
						Gizmos.matrix = Matrix4x4.TRS( transform.position, Quaternion.Euler(0,0, transform.eulerAngles.z), transform.lossyScale );

						// Help visualize Box2D by making the Z scale 0
						_gizmoScaleHelper = overlapScale;
						_gizmoScaleHelper.z = 0f;

						// Draw a cube
					//	Gizmos.DrawWireCube(transform.position +  transform.TransformDirection( overlapOffset ), _gizmoScaleHelper );
						Gizmos.DrawWireCube( overlapOffset, _gizmoScaleHelper );

					}
					
				}
			}
		#endif
	}
}