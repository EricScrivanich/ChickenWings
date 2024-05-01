//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Spawner.cs
//	A powerful and easy to use spawner component for PoolKit.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 - 2019 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
	using UnityEditor;
#endif

namespace HellTap.PoolKit {

	[DisallowMultipleComponent]
	public class Spawner : MonoBehaviour {

		// Editor Helpers
		#if UNITY_EDITOR
			public int tab = 0;												// Helps remember which tab we're using
		#endif

		[Header("Spawner Name")]
		public string spawnerName = "Spawner";								// Unique name of spawner

		// Status
		public State state = State.Stopped;									// The current state of the Spawner
			public enum State { Stopped, Playing, Paused }					// The possible States

		// References	
		Transform _theTransform = null;

		// When should this spawner start?
		[Header("Spawner Start")]
		public StartMode spawningBegins = StartMode.AutomaticallyOnStart; 	// How should this spawner start?
			public enum StartMode{ 	
									AutomaticallyOnStart, 					// Runs straight away on Start()
									AutomaticallyOnStartAfterDelay, 		// Runs after a delay after Start()
									AutomaticallyOnEnable, 					// Runs straight away OnEnable() [so it can be pooled!]
									AutomaticallyOnEnableAfterDelay, 		// Runs straight away OnEnable() after delay
									OnlyWhenCalledByScript, 				// The spawner only runs when Play() is called
									Never 									// Never play this spawner
								  }
		public float autoSpawnDelay = 1f;									// If using AutomaticallyAfterDelay, we use this!

		// How long should this spawner last?
		[Header("Spawner Duration")]
		public SpawnDuration spawnDuration = SpawnDuration.LoopForever;
			public enum SpawnDuration { 
				PlayOnce = 0, 												// Only perform a single spawn Cycle
				RepeatXCycles = 1,											// Play X number of spawn cycles
				SpawnXInstances = 4, 										// Keep playing until we spawn X number of instances
				CountdownTimer = 2, 										// Keep playing for a set duration
				LoopForever = 3												// Never ending loop.
			}
		public float durationCountdown = 5f;
			float _durationCountdownCurrent = 5f;
		public int durationRepeatXTimes = 3;								// "Times" are now called "Cycles".
			int _numberOfSpawnCycles = 0;									// internal count of how many spawn cycles we've performed
		public int durationSpawnXInstances = 3;								// How many spawned instances should we create?
			int _numberOfInstancesSpawned = 0;								// internal count of how many times we've spawned an instance


		// How often should we spawn an object?
		[Header("Spawner Frequency")]
		public SpawnerFrequencyMode frequencyMode = SpawnerFrequencyMode.FixedInterval;
			public enum SpawnerFrequencyMode { FixedInterval, RandomRange }
		public float frequencyFixedInterval = 1f;							// Fixed Interval in seconds
		public float frequencyRandomMin = 1f;								// Minimum value of random range
		public float frequencyRandomMax = 2f;								// Maximum value of random range
		
		public InstancesPerCycleMode instancesPerCycleMode = InstancesPerCycleMode.FixedNumber;
			public enum InstancesPerCycleMode { FixedNumber, RandomRange }
		public int instancesPerCycle = 1;									// How many intances should be spawned every interval?
		public int minInstancesPerCycle = 1;								// Min number of intances to be spawned every interval
		public int maxInstancesPerCycle = 2;								// Max number of intances to be spawned every interval
			int _currentInstancesPerCycle = 1;								// <- internal helper value

		// [NEW] Spawn Cycle Setup
		public Increment incrementPrefab = Increment.PerInstance;
		public Increment incrementPosition = Increment.PerInstance;
		public Increment incrementRandomOffsets = Increment.PerInstance;
		public Increment incrementRotation = Increment.PerInstance;
		public Increment incrementScale = Increment.PerInstance;
			public enum Increment{ 
							PerInstance, 									// Every time an instance is spawned
							PerCycle 										// Every time the spawner begins a new spawn cycle
						}	


		// What prefabs should it spawn?
		[Header("Prefabs To Spawn")]
		public PrefabSelection prefabSelection = PrefabSelection.SequenceAscending;
			public enum PrefabSelection { SequenceAscending, SequenceDescending, PingPongAscending, PingPongDescending, Random, RandomWithWeights } 
		public PrefabToSpawn[] prefabs = new PrefabToSpawn[0];

		// How should the instances be handled?
		[Header("Instance Rotation")]
		public RotationMode rotationMode = RotationMode.PrefabDefault;
			public enum RotationMode { PrefabDefault, SpawnerRotation, SpawnPointRotation, CustomEulerAngles, RandomRotation }
		public Vector3 customRotationEulerAngles = Vector3.zero;
		Transform _lastSpawnPointUsed = null;

		[Header("Instance Scale")]
		public ScaleMode scaleMode = ScaleMode.PrefabDefault;
			public enum ScaleMode { PoolDefault, PrefabDefault, SpawnerScale, CustomLocalScale, RandomRangeScale, RandomRangeProportionalScale }
		public Vector3 customLocalScale = Vector3.one;
		public Vector3 customLocalScaleMin = Vector3.zero;
		public Vector3 customLocalScaleMax = Vector3.one;
		public float customLocalScaleProportionalMin = 0f;
		public float customLocalScaleProportionalMax = 1f;

		[Header("Instance Parent")]
		public ParentMode reparentInstances = ParentMode.Ignore;
			public enum ParentMode { Ignore, ReparentToSpawner, ReparentToSpawnPoint, ReparentToCustomTransform }
		public Transform customParentTransform = null;

		// Where should we spawn the prefabs?
		[Header("Spawn Locations")]
		public SpawnLocation prefabsWillBeSpawned = SpawnLocation.AtThisTransform;
			public enum SpawnLocation { AtThisTransform, UsingTransformList, UsingLocalPositionList, UsingGlobalPositionList } 
		// This list only shows if we're using a Local Positions List
		public SpawningLocalTo spawnPositionsAreLocalTo = SpawningLocalTo.Spawner;
			public enum SpawningLocalTo { Spawner, CustomTransform }
			public Transform spawnLocalTo = null;	// If we're using a custom Transform, spawn positions will be local to this
		public SpawnPointSelection spawnPointSelection = SpawnPointSelection.SequenceAscending;
			public enum SpawnPointSelection { SequenceAscending, SequenceDescending, PingPongAscending, PingPongDescending, Random, RandomWithWeights } 

			// Transform List
			public TransformSpawnPoint[] spawnpointTransforms = new TransformSpawnPoint[1];
				[System.Serializable]
				public class TransformSpawnPoint {
					public Transform spawnPoint = null;	
					[Range(0,100)]
					public float randomWeight = 100f;			// The chance of this being selected (if using weights)
					
					// Make sure the randomWeights are correct
					public void Setup(){ randomWeight = Mathf.Clamp(randomWeight, 0f, 100f); }
					public Vector3 GetVector3(){
						if(spawnPoint!=null){ return spawnPoint.position; }
						return Vector3.zero;	// Return Vector3.zero if something goes wrong
					}
				}

			// Position List
			public PositionSpawnPoint[] spawnpointPositions = new PositionSpawnPoint[1];
				[System.Serializable]
				public class PositionSpawnPoint {
					public Vector3 spawnPoint = Vector3.zero;	
					[Range(0,100)]
					public float randomWeight = 100f;			// The chance of this being selected (if using weights)
					
					// Make sure the randomWeights are correct
					public void Setup(){ randomWeight = Mathf.Clamp(randomWeight, 0f, 100f); }
					// NOTE: For local positions, send the spawner position as the offset, for global send Vector3.zero
					public Vector3 GetVector3( Vector3 offset ){
						if(offset==Vector3.zero){ return spawnPoint; }	// <- Global Position
						return offset+spawnPoint;						// <- Local Position
					}
				}

		// Add Randomized Offsets for Spawn Point Positions
		public bool addSpawnPointRandomizationRange = false;
			public Vector3 spawnPointRandomizationRangeMin = new Vector3(-0.5f,-0.5f,-0.5f );
			public Vector3 spawnPointRandomizationRangeMax = new Vector3( 0.5f, 0.5f, 0.5f );

		// NEW: Allow clamping the randomization to have an absolute min and max distnce from it's source
		public bool addSpawnPointRandomizationMinMaxDistance = false;	
			public float spawnPointRandomizationMinDistance = 1f;
			public float spawnPointRandomizationMaxDistance = 10f;

		// API: C# Delegates & Events
		[Header("Delegates")]
		public bool enableSpawnerEvents = false;

			// DELEGATE: onSpawnerSpawn
			public delegate void OnSpawnerSpawnDelegate( Transform instance );	// Users can subscribe to events where this 
			public OnSpawnerSpawnDelegate onSpawnerSpawn;						// spawner spawns an instance.

			// DELEGATE: onSpawnerStart
			public delegate void OnSpawnerStartDelegate();		// Users can subscribe to events where this 
			public OnSpawnerStartDelegate onSpawnerStart;		// spawner starts (Playing).

			// DELEGATE: onSpawnerStop
			public delegate void OnSpawnerStopDelegate();		// Users can subscribe to events where this 
			public OnSpawnerStopDelegate onSpawnerStop;			// spawner stops.

			// DELEGATE: onSpawnerPause
			public delegate void OnSpawnerPauseDelegate();		// Users can subscribe to events where this 
			public OnSpawnerPauseDelegate onSpawnerPause;		// spawner pauses.

			// DELEGATE: onSpawnerResume
			public delegate void OnSpawnerResumeDelegate();		// Users can subscribe to events where this 
			public OnSpawnerResumeDelegate onSpawnerResume;		// spawner resumes playing.

			// DELEGATE: onSpawnerEnd
			public delegate void OnSpawnerEndDelegate();		// Users can subscribe to events where this 
			public OnSpawnerEndDelegate onSpawnerEnd;			// spawner finishes playing.

		
		// API: Easy to use Unity Events
		[Header("Unity Events")]
		public bool enableUnityEventSpawn = false;
		public bool enableUnityEventStart = false;
		public bool enableUnityEventStop = false;
		public bool enableUnityEventPause = false;
		public bool enableUnityEventResume = false;
		public bool enableUnityEventEnd = false;
		
		// UnityEvents
		public UnityTransformEvent OnSpawnerSpawnUnityEvent;
			[System.Serializable]
			public class UnityTransformEvent : UnityEvent<Transform>{}
		public UnityEvent OnSpawnerStartUnityEvent;
		public UnityEvent OnSpawnerStopUnityEvent;
		public UnityEvent OnSpawnerPauseUnityEvent;
		public UnityEvent OnSpawnerResumeUnityEvent;
		public UnityEvent OnSpawnerEndUnityEvent;


		// ==================================================================================================================
		//	CLASS: PREFAB TO SPAWN
		//	Class that holds info about the prefab, its pool, randomized weights and spawn methods
		// ==================================================================================================================

		[System.Serializable]
		public class PrefabToSpawn {

			// Variables
			[Header("Prefab To Spawn")]
			public GameObject prefab = null;							// The prefab to spawn
			
			// Randomizer
			[Header("Options")]
			[Range(0,100)]
			public float randomWeight = 100f;							// The chance of this being selected (if using weights)

			[Header("Helpers")]
			[System.NonSerialized] public Pool pool = null;											// The pool this belongs to
			[System.NonSerialized] public bool hasPool = false;										// Does Pool exist?
			[System.NonSerialized] public Vector3 defaultPrefabPosition = Vector3.zero;				// Cached Prefab Position
			[System.NonSerialized] public Quaternion defaultPrefabRotation = Quaternion.identity;	// Cached Prefab Rotation
			[System.NonSerialized] public Vector3 defaultPrefabLocalScale = Vector3.one;			// Cached Prefab Scale

			// Setup this class by asking PoolKit to find the pool that contains the prefab
			public void Setup(){
				
				// Setup Pool
				if( pool==null && prefab != null ){ pool = PoolKit.GetPoolContainingPrefab( prefab ); }
				if( pool!=null ){ hasPool = true; }

				// Cache the prefab default values
				if( prefab != null ){
					defaultPrefabPosition = prefab.transform.position;
					defaultPrefabRotation = prefab.transform.rotation;
					defaultPrefabLocalScale = prefab.transform.localScale;
				}

				// Make sure random weight is within bounds
				randomWeight = Mathf.Clamp(randomWeight, 0f, 100f);
			}

			// Spawn Method
			Transform _spawnedObject = null;
			public Transform  Spawn( bool usePoolScale, Vector3 position, Quaternion rotation, Vector3 localScale, Transform parent ){
				if( hasPool ){
					if(usePoolScale){ return pool.Spawn( prefab, position, rotation, parent ); }
					return pool.Spawn( prefab, position, rotation, localScale, parent );
				
				} else {
					_spawnedObject = Instantiate( prefab, position, rotation, parent ).transform;
					_spawnedObject.localScale = localScale;
					if(parent!=null){ _spawnedObject.parent = parent; }
					return _spawnedObject;
				}
			}
		}

		// ==================================================================================================================
		//	AWAKE
		//	Make sure the state is correct and cache needed references
		// ==================================================================================================================
	
		void Awake(){ 

			// Always make sure the state is stopped at awake
			state = State.Stopped; 

			// Cache the transform
			_theTransform = transform;

			// If we're using sequence / ping pong descending, make the first index the last prefab
			if( prefabSelection == PrefabSelection.SequenceDescending ||
				prefabSelection == PrefabSelection.PingPongDescending
			){ 
				// NOTE: On the first loop, it will subtract by 1 bringing it in bounds
				_gpIndex = prefabs.Length;
			
			} else {

				// NOTE: On the first loop, it will add by 1 bringing it in bounds
				_gpIndex = -1;
			}

			// If we're using sequence / ping pong descending for locations, make the first index the last location
			if( spawnPointSelection == SpawnPointSelection.SequenceDescending ||
				spawnPointSelection == SpawnPointSelection.PingPongDescending
			){ 
				// NOTE: On the first loop, it will subtract by 1 bringing it in bounds
				_gstIndex = spawnpointTransforms.Length;
				_gptIndex = spawnpointPositions.Length;

			} else {
				// NOTE: On the first loop, it will add by 1 bringing it in bounds
				_gstIndex = -1;
				_gptIndex = -1;
			}

			
		}

		// ==================================================================================================================
		//	START
		//	Use this for initialization
		// ==================================================================================================================
	
		IEnumerator Start () {
			
			// Loop through the Prefabs and set them up
			for(int i = 0; i < prefabs.Length; i++ ){ prefabs[i].Setup(); }

			// Setup values
			_durationCountdownCurrent = durationCountdown;

			// Start the spawner automatically at start
			if( spawningBegins == StartMode.AutomaticallyOnStart ){
				RestartAndPlay();
			
			// Start after delay
			} else if( spawningBegins == StartMode.AutomaticallyOnStartAfterDelay ){
				yield return new WaitForSeconds( autoSpawnDelay );
				RestartAndPlay();
			}

		}

		// ==================================================================================================================
		//	ON DESTROY
		//	To be on the safe side, we stop all coroutines just in case the spawner is destroyed early
		// ==================================================================================================================
	
		void OnDestroy(){ StopAllCoroutines(); }

		// ==================================================================================================================
		//	ON ENABLE
		//	Check to see if we should enable the spawner now
		// ==================================================================================================================
	
		void OnEnable(){

			// Register this spanwer with PoolKit
			PoolKit.RegisterSpawner(this);

			// Start the spawner automatically at start
			if( spawningBegins == StartMode.AutomaticallyOnEnable ){
				RestartAndPlay();
			
			// Start after delay
			} else if( spawningBegins == StartMode.AutomaticallyOnEnableAfterDelay ){
				StartCoroutine( OnEnableAfterDelay() );
			}
		}
			// We seperate this out into an IEnumerator to apply the delay
			IEnumerator OnEnableAfterDelay(){
				yield return new WaitForSeconds( autoSpawnDelay );
				RestartAndPlay();
			}

		// ==================================================================================================================
		//	ON DISABLE
		//	Always stop the spawner if it being made inactive
		// ==================================================================================================================
	
		void OnDisable(){

			// Stop particle system
			Stop();

			// Unregister this spawner with PoolKit
			PoolKit.UnregisterSpawner(this);
		}

		// ==================================================================================================================
		//	UPDATE
		//	The core loop that runs the spawner
		// ==================================================================================================================

		// Helpers
		[System.NonSerialized] bool runSpawner = false;						// This boolean tells the spawner to run the loop
		[System.NonSerialized] bool runSpawnerRequiresSetup = true;			// Should be true at start, this sets up the spawner
		[System.NonSerialized] float runSpawnerTimerUntilNextSpawn = 0f;	// Used as a timer between spawns

		// Method
		void Update(){

			// RUN SPAWNER: The runSpawner bool is used as a "ready" flag and lets us know when to begin
			// NOTE: Don't allow the spawner loop to run if spawningBegins is set to "Never"
			if( runSpawner == true && spawningBegins != StartMode.Never ){

				// -------------------------------------------------------------
				//	RESET THE SPAWNER IF NEEDED
				//	This is set on the first frame or if Restart / Play is used
				// -------------------------------------------------------------

				// Setup the Spawner
				if( runSpawnerRequiresSetup ){

					// Set state to playing
					state = State.Playing;

					// Reset Spawner Cycles
					_numberOfSpawnCycles = 0;
					_numberOfInstancesSpawned = 0;

					// Reset Countdown Timer if needed
					if( spawnDuration == SpawnDuration.CountdownTimer ){
						_durationCountdownCurrent = durationCountdown;
					}

					// Reset the timeout of the spawner
					runSpawnerTimerUntilNextSpawn = 0f;

					// Turn off the flag so this doesn't run every frame
					runSpawnerRequiresSetup = false;
					
				}

			
				// Keep the loop running as long as we're in the Playing state
				if ( state == State.Playing ){

					// -------------------------------------------------------------
					//	HANDLE SPAWNING
					// -------------------------------------------------------------

					// Subtract countdown until the next spawn
					if( runSpawnerTimerUntilNextSpawn > 0 ){
						runSpawnerTimerUntilNextSpawn -= Time.deltaTime;
					
					// When we reach 0, spawn the next object and reset the timer
					} else {

						// Calculate how many instances per cycle we'll be spawning ...
						if( instancesPerCycleMode == InstancesPerCycleMode.FixedNumber ){
							_currentInstancesPerCycle = instancesPerCycle;
						
						} else if( instancesPerCycleMode == InstancesPerCycleMode.RandomRange ){
							
							// Randomize the instances (we need to +1 the max int otherwise it will never be returned)
							_currentInstancesPerCycle = Random.Range( minInstancesPerCycle, maxInstancesPerCycle+1 );

							// To be safe, make sure we never exceed the maximum int
							if(_currentInstancesPerCycle>maxInstancesPerCycle){ _currentInstancesPerCycle = maxInstancesPerCycle; }
						}


						// Spawn a single instance
						if ( _currentInstancesPerCycle==1 ){ 
							Spawn();	// <- when we have a single instance per cycle, we can call spawn with defaults

						// Spawn multiple instances	
						} else if ( _currentInstancesPerCycle > 1 ){

							// Repeat using the current number of instances per cycle ...
							for( int i = _currentInstancesPerCycle; i > 0; i-- ){ 

								// Seperate the first iteration from the others ...
								if(i == _currentInstancesPerCycle){ 
									Spawn( null, true );	// <- true = first iteration
								} else { 
									Spawn( null, false);	// <- false = NOT the first iteration
								}

								// Stop creating instances if we've created the exact amount of instances we need
								if( EnoughInstancesWereCreatedToStopSpawner() == true ){ break; }
							}
						}

						// Increment Spawn Cycle
						_numberOfSpawnCycles++;

						// Reset Timer
						runSpawnerTimerUntilNextSpawn = GetSpawnDelay();

						// Check if loop is finished
						CheckIfSpawnerIsFinished();
					}
					
					// -------------------------------------------------------------
					//	HANDLE COUNTDOWN TIMER
					// -------------------------------------------------------------

					// If we're using a countdown timer, do that here
					if( spawnDuration == SpawnDuration.CountdownTimer ){
						
						// Subtract from the countdown every frame
						if( _durationCountdownCurrent > 0 ){
							_durationCountdownCurrent -= Time.deltaTime;
						
						// When we're finished, run the CheckIfSpawnerIsFinished() method to stop spawning
						} else {
							CheckIfSpawnerIsFinished();
						}
					}
					
				}
			}

			// Handle Delegates and Events
			if( enableSpawnerEvents || enableUnityEventSpawn || enableUnityEventStart || enableUnityEventStop || enableUnityEventPause || enableUnityEventResume || enableUnityEventEnd 
			){
				HandleEvents();
			}
		}

		// ==================================================================================================================
		//	HANDLE EVENTS
		//	If we need to send Events, we do this here by tracking the current (and previous) spawner state
		//	NOTE: The end event happens in CheckIfSpawnerIsFinished()
		// ==================================================================================================================

		// Helpers
		State _previousState = State.Stopped;		// The previous spawner state (used to help with delegates and events)

		// Method
		void HandleEvents(){

			// This is only triggered when the previous state and current state are not the same ...
			if( _previousState != state ){

				// --------------------------------
				//	PLAYING
				// --------------------------------

				// The current state is PLAYING
				if( state == State.Playing ){

					// If the previous state was "stopped", then we've just started playing normally ...
					if( _previousState == State.Stopped ){

						// Handle delegates / Events
						if( enableSpawnerEvents && onSpawnerStart != null ){ onSpawnerStart(); }

						// Handle Unity Events
						if( enableUnityEventStart && OnSpawnerStartUnityEvent != null ){ OnSpawnerStartUnityEvent.Invoke(); }
						
						// Sync the state
						_previousState = state;
						return;
					
					// If the previous state was "Paused", then we've just resumed ...
					} else if( _previousState == State.Paused ){

						// Handle delegates / Events
						if( enableSpawnerEvents && onSpawnerResume != null ){ onSpawnerResume(); }

						// Handle Unity Events
						if( enableUnityEventResume && OnSpawnerResumeUnityEvent != null ){ OnSpawnerResumeUnityEvent.Invoke(); }
						
						// Sync the state
						_previousState = state;
						return;
					}

				// --------------------------------
				//	STOPPED
				// --------------------------------

				// The current state is STOPPED
				} else if( state == State.Stopped ){

					// Handle delegates / Events
					if( enableSpawnerEvents && onSpawnerStop != null ){ onSpawnerStop(); }
					
					// Handle Unity Events
					if( enableUnityEventStop && OnSpawnerStopUnityEvent != null ){ OnSpawnerStopUnityEvent.Invoke(); }

					// Sync the state
					_previousState = state;
					return;

				// --------------------------------
				//	PAUSED
				// --------------------------------

				// The current state is PAUSED
				} else if( state == State.Paused ){

					// Handle delegates / Events
					if( enableSpawnerEvents && onSpawnerPause != null ){ onSpawnerPause(); }

					// Handle Unity Events
					if( enableUnityEventPause && OnSpawnerPauseUnityEvent != null ){ OnSpawnerPauseUnityEvent.Invoke(); }
					
					// Sync the state
					_previousState = state;
					return;
				}

				// If anything else happens, just sync the state
				_previousState = state;
				return;
			}
		}

		// ==================================================================================================================
		//	CHECK IF SPAWNER IS FINISHED
		//	Checks to see if the Spawner is finished. If so, we call the Stop() method.
		// ==================================================================================================================

		void CheckIfSpawnerIsFinished(){

			if( // If we're using the countdown timer and it has run out
				spawnDuration == SpawnDuration.CountdownTimer && _durationCountdownCurrent <= 0 ||

				// Or, if we have exceeded the number of times of Play Once
				spawnDuration == SpawnDuration.PlayOnce && _numberOfSpawnCycles >= 1 ||

				// Or, if we have exceeded the number of Repeat X Times
				spawnDuration == SpawnDuration.RepeatXCycles && _numberOfSpawnCycles >= durationRepeatXTimes ||

				// Or, if we have exceeded the number of spawned Instances
				spawnDuration == SpawnDuration.SpawnXInstances && _numberOfInstancesSpawned >= durationSpawnXInstances
			){

				// Stop the spawner
				Stop();

				// Handle delegates / Events
				if( enableSpawnerEvents && onSpawnerEnd != null ){ onSpawnerEnd(); }

				// Handle Unity Events
				if( enableUnityEventEnd && OnSpawnerEndUnityEvent != null ){ OnSpawnerEndUnityEvent.Invoke(); }

			}
		}

		// ==================================================================================================================
		//	ENOUGH INSTANCES WERE CREATED TO STOP SPAWNER
		//	Quick helper method that checks if the spawner should stop early (when counting instances)
		// ==================================================================================================================

		bool EnoughInstancesWereCreatedToStopSpawner(){

			// If we have exceeded the number of spawned Instances (and we're using that mode)
			if( spawnDuration == SpawnDuration.SpawnXInstances && _numberOfInstancesSpawned >= durationSpawnXInstances ){
				return true;
			}
			return false;
		}

		// ==================================================================================================================
		//	GET SPAWN DELAY
		//	Returns the amount of time between spawns as setup by the user
		// ==================================================================================================================

		float GetSpawnDelay(){

			// If we're using a fixed interval, return it
			if( frequencyMode == SpawnerFrequencyMode.FixedInterval ){
				return frequencyFixedInterval;

			// If we're using the Random range, calculate that and return it	
			} else {
				return Random.Range( frequencyRandomMin, frequencyRandomMax );
			}
		}

		// ==================================================================================================================
		//	CLAMP MAGNITUDE
		//	Clamps a Vector3 to be within a min and max range
		// ==================================================================================================================

		// Helpers
		[System.NonSerialized] float _v3Magnitude = 0f;
		[System.NonSerialized] Vector3 _v3Normalized = Vector3.zero;

		// Method
		public Vector3 ClampMagnitude( Vector3 v3, float min, float max ){
			
			// Cache the magnitude
			_v3Magnitude = v3.magnitude;
			
			// If we're dealing with a magnitude less than our minimum
			if ( _v3Magnitude < min ){
			
				_v3Normalized = v3 / _v3Magnitude;
				return _v3Normalized * min;
			
			// If we're dealing with a magnitude greater than our maximum
			} else if ( _v3Magnitude > max ){

				_v3Normalized = v3 / _v3Magnitude;
				return _v3Normalized * max;
			}
			 
			// No need to clamp at all
			return v3;
		}

		// ==================================================================================================================
		//	SPAWN
		//	Spawn a new item from the pool
		// ==================================================================================================================

		// Helpers
		[System.NonSerialized] PrefabToSpawn _spawnPTS = null;

		[System.NonSerialized] Vector3 _spawnPosition = Vector3.zero;
		[System.NonSerialized] Vector3 _spawnRandomOffset = Vector3.zero;
		[System.NonSerialized] Quaternion _spawnRotation = Quaternion.identity;
		[System.NonSerialized] Vector3 _spawnScale = Vector3.zero;

		[System.NonSerialized] Transform _spawnedInstance = null;
		[System.NonSerialized] bool _foundPrefabToUse = false;

		// CACHED VALUES FROM THE FIRST ITERATION
		[System.NonSerialized] PrefabToSpawn _prefabUsedWhenLastCycleBegan = null;		// The prefab we used when the last cycle began
		[System.NonSerialized] Vector3 _positionUsedWhenLastCycleBegan = Vector3.zero;	// The position we used when the last cycle began
		[System.NonSerialized] Vector3 _randomOffsetUsedWhenLastCycleBegan = Vector3.zero;	// The offset used when last cycle began
		[System.NonSerialized] Quaternion _rotationUsedWhenLastCycleBegan = Quaternion.identity;   // Rotation used when last cycle began
		[System.NonSerialized] Vector3 _scaleUsedWhenLastCycleBegan = Vector3.zero;		// The scale we used when the last cycle began


		// Method
		public void Spawn( GameObject useThisPrefab = null, bool cycleJustStarted = true ){	// NOTE: useThisPrefab is only used in an API call, not internally.
			
			// Only spawn if we're currently playing and we have actually setup some prefabs
			if( spawningBegins != StartMode.Never && prefabs.Length > 0 ){
				
				// --------------------
				//	HANDLE THE PREFAB
				// --------------------

				// If the user has used the API to spawn a specific prefab ...
				if( useThisPrefab != null ){

					// Set the _foundPrefabToUse bool to false so we can check if anything was found
					_foundPrefabToUse = false;

					// Loop through the prefabs and see if it exists ...
					for( int i = 0; i < prefabs.Length; i++ ){
						if(prefabs[i].prefab == useThisPrefab){ 
							_spawnPTS = prefabs[i];
							_foundPrefabToUse = true;
							break;
						}
					}

					// Show a warning message if this prefab doesn't exist in the pool, and then end it here!
					if( _foundPrefabToUse == false ){ 
						Debug.LogWarning( "POOLKIT (Spawner) The prefab '" + useThisPrefab.name + "' is not setup in this spawner and cannot be spawned. Try adding the prefab to the pool first!");
						return;
					}

				// Use the standard method
				} else {

					// Get the next prefab to spawn
					//_spawnPTS = GetPrefab();


					// NEW!

					// If the spawn cycle just started or incrementPrefab is set to increment on every instance,
					// we should update and get the currentPrefab ...
					if( cycleJustStarted == true || incrementPrefab == Increment.PerInstance ){

						// Get the next prefab to spawn and cache it for later in the cycle
						_spawnPTS = GetPrefab();
						_prefabUsedWhenLastCycleBegan = _spawnPTS;
					
					// Otherwise, if this is later in the cycle, use the previously cached prefab
					} else if( cycleJustStarted == false && incrementPrefab == Increment.PerCycle ){

						// Use the previously cached prefab when the cycle began
						_spawnPTS = _prefabUsedWhenLastCycleBegan;
					}
				}

				// --------------------
				//	SPAWN THE PREFAB
				// --------------------

				// Spawn the prefab if it is valid
				if( _spawnPTS != null ){

					// ---------------
					// CACHE POSITION
					// ---------------

					// If the spawn cycle just started or incrementPosition is set to increment on every instance ...
					if( cycleJustStarted == true || incrementPosition == Increment.PerInstance ){

						_spawnPosition = GetSpawnPosition();
						_positionUsedWhenLastCycleBegan = _spawnPosition; 

					// Otherwise, if this is later in the cycle, use the previously cached value
					} else if( cycleJustStarted == false && incrementPosition == Increment.PerCycle ){

						_spawnPosition = _positionUsedWhenLastCycleBegan;
					}

					// --------------------
					// CACHE RANDOM OFFSET
					// --------------------

					// If the spawn cycle just started or incrementRandomOffsets is set to increment on every instance ...
					if( cycleJustStarted == true || incrementRandomOffsets == Increment.PerInstance ){

						// Randomize the offset if we've enabled it
						if( addSpawnPointRandomizationRange ){
							_spawnRandomOffset.x = Random.Range(spawnPointRandomizationRangeMin.x, spawnPointRandomizationRangeMax.x);
							_spawnRandomOffset.y = Random.Range(spawnPointRandomizationRangeMin.y, spawnPointRandomizationRangeMax.y);
							_spawnRandomOffset.z = Random.Range(spawnPointRandomizationRangeMin.z, spawnPointRandomizationRangeMax.z);
						
							// NEW: Allows us to clamp the distance of the offset within a min and max range
							if( addSpawnPointRandomizationMinMaxDistance ){
								_spawnRandomOffset = ClampMagnitude( _spawnRandomOffset, spawnPointRandomizationMinDistance, spawnPointRandomizationMaxDistance );
							}

						// Otherwise, use Vector3.zero which won't apply any offsets
						} else {
							_spawnRandomOffset = Vector3.zero;
						}

						_randomOffsetUsedWhenLastCycleBegan = _spawnRandomOffset; 

					// Otherwise, if this is later in the cycle, use the previously cached value
					} else if( cycleJustStarted == false && incrementRandomOffsets == Increment.PerCycle ){

						_spawnRandomOffset = _randomOffsetUsedWhenLastCycleBegan;
					}

					// ---------------
					// CACHE ROTATION
					// ---------------

					// If the spawn cycle just started or incrementRotation is set to increment on every instance ...
					if( cycleJustStarted == true || incrementRotation == Increment.PerInstance ){

						// Get the next rotation to spawn and cache it for later in the cycle
						_spawnRotation = GetSpawnRotation(_spawnPTS);
						_rotationUsedWhenLastCycleBegan = _spawnRotation; 

					// Otherwise, if this is later in the cycle, use the previously cached value
					} else if( cycleJustStarted == false && incrementRotation == Increment.PerCycle ){

						_spawnRotation = _rotationUsedWhenLastCycleBegan;
					}

					// ---------------
					// CACHE SCALE
					// ---------------

					// If the spawn cycle just started or incrementScale is set to increment on every instance ...
					if( cycleJustStarted == true || incrementScale == Increment.PerInstance ){

						// Get the next scale to spawn and cache it for later in the cycle
						_spawnScale = GetSpawnScale(_spawnPTS);
						_scaleUsedWhenLastCycleBegan = _spawnScale; 

					// Otherwise, if this is later in the cycle, use the previously cached value
					} else if( cycleJustStarted == false && incrementScale == Increment.PerCycle ){

						_spawnScale = _scaleUsedWhenLastCycleBegan;
					}


					// ---------------
					// SPAWN IT!
					// ---------------

					_spawnedInstance = _spawnPTS.Spawn 	( 
															scaleMode == ScaleMode.PoolDefault ? true : false,
															_spawnPosition + _spawnRandomOffset, 
															_spawnRotation, 
															_spawnScale, 
															GetSpawnParent() 
														);

				
					/* Spawn Instance Now -> original way
					_spawnedInstance = _spawnPTS.Spawn 	( 
															scaleMode == ScaleMode.PoolDefault ? true : false,
															GetSpawnPosition(), 
															GetSpawnRotation(_spawnPTS), 
															GetSpawnScale(_spawnPTS), 
															GetSpawnParent() 
														);
					*/

					// Increment Spawner Cycles if we successfully spawned an object
					_numberOfInstancesSpawned++;

					// Handle delegates / Events
					if( enableSpawnerEvents && onSpawnerSpawn != null ){ onSpawnerSpawn( _spawnedInstance ); }

					// Handle Unity Events
					if( enableUnityEventSpawn && OnSpawnerSpawnUnityEvent != null ){ 
						OnSpawnerSpawnUnityEvent.Invoke( _spawnedInstance ); 
					}

				}
			}
		}

		// ==================================================================================================================
		//	GET PREFAB
		//	Get the next prefab to instance
		// ==================================================================================================================

		// Helper
		[System.NonSerialized] public int _gpIndex = 0;				// The index of the prefab to use
		[System.NonSerialized] public bool _gpIndexPingPong = false;	// The direction of the ping pong

		PrefabToSpawn GetPrefab(){

			// Check that we have at least a single prefab in the list
			if( prefabs.Length == 0 ){ return null; }

			// If there is only one prefab in the list, just return that
			else if(prefabs.Length==1){ return prefabs[0]; }

			// Sequence Ascending
			else if( prefabSelection == PrefabSelection.SequenceAscending ){ return GetPrefabSequenceAscending(); }

			// Sequence Descending
			else if( prefabSelection == PrefabSelection.SequenceDescending ){ return GetPrefabSequenceDescending(); }

			// Ping-Pong Ascending
			else if( prefabSelection == PrefabSelection.PingPongAscending ){ return GetPrefabPingPongAscending(); }

			// Ping-Pong Descending
			else if( prefabSelection == PrefabSelection.PingPongDescending ){ return GetPrefabPingPongDescending(); }

			// Random
			else if( prefabSelection == PrefabSelection.Random ){ return GetPrefabRandom(); }

			// Random With Weights
			else if( prefabSelection == PrefabSelection.RandomWithWeights ){ return GetPrefabRandomWithWeights(); }

			// If something goes wrong, return the first prefab
			return prefabs[0];
		}

			// --------------------
			// SEQUENCE ASCENDING
			// --------------------

			PrefabToSpawn GetPrefabSequenceAscending(){
				
				// Add to the index
				_gpIndex++;

				// If we've reached the end, loop back to the first entry
				if( _gpIndex >= prefabs.Length ){ _gpIndex = 0; }

				// Return the prefab
				return prefabs[_gpIndex];
			}

			// --------------------
			// SEQUENCE DESCENDING
			// --------------------

			PrefabToSpawn GetPrefabSequenceDescending(){
				
				// Subtract from the index
				_gpIndex--;

				// If we've reached the end, loop back to the last entry
				if( _gpIndex < 0 ){ _gpIndex = prefabs.Length-1; }

				// Return the prefab
				return prefabs[_gpIndex];
			}

			// --------------------
			// PING-PONG ASCENDING
			// --------------------

			PrefabToSpawn GetPrefabPingPongAscending(){
				
				// Ping Pong FALSE
				if( _gpIndexPingPong == false ){

					// Add to the index
					_gpIndex++;

					// If we've reached the end, loop back to the first entry
					if( _gpIndex >= prefabs.Length ){ 
						_gpIndex = prefabs.Length-2; 
						_gpIndexPingPong = true;
					}

				// Ping Pong TRUE
				} else if( _gpIndexPingPong == true ){

					// Subtract from the index
					_gpIndex--;

					// If we've gone further than the start, go back to the first entry
					if( _gpIndex < 0 ){ 
						_gpIndex = 1; 
						_gpIndexPingPong = false;
					}
				}

				// Return the prefab
				return prefabs[_gpIndex];
			}

			// --------------------
			// PING-PONG DESCENDING
			// --------------------

			PrefabToSpawn GetPrefabPingPongDescending(){
				
				// Ping Pong FALSE
				if( _gpIndexPingPong == false ){

					// Subtract from the index
					_gpIndex--;

					// If we've gone further than the start, go back to the first entry
					if( _gpIndex < 0 ){ 
						_gpIndex = 1; 
						_gpIndexPingPong = true;
					}

				// Ping Pong TRUE
				} else if( _gpIndexPingPong == true ){

					// Add to the index
					_gpIndex++;

					// If we've reached the end, loop back to the first entry
					if( _gpIndex >= prefabs.Length ){ 
						_gpIndex = prefabs.Length-2; 
						_gpIndexPingPong = false;
					}
				}

				// Return the prefab
				return prefabs[_gpIndex];
			}

			// --------------------
			// RANDOM
			// --------------------

			PrefabToSpawn GetPrefabRandom(){
				
				// Add to the index
				_gpIndex = Random.Range( 0, prefabs.Length );

				// Keep it within range
				if( _gpIndex >= prefabs.Length ){ _gpIndex = prefabs.Length-1; }

				// Return the prefab
				return prefabs[_gpIndex];
			}

			// --------------------
			// RANDOM WITH WEIGHTS
			// --------------------

			// Helpers
			[System.NonSerialized] float[] _gpwwProbabilityList = new float[0];
			[System.NonSerialized] float _gpwwTotal = 0;
			[System.NonSerialized] float _gpwwRandomPoint = 0;

			// Method
			// Based on Unity's "Choose" Example: https://docs.unity3d.com/Manual/RandomNumbers.html
			PrefabToSpawn GetPrefabRandomWithWeights(){
				
				// Convert the prefab list into a list of probability floats (randomWeights)
				if( _gpwwProbabilityList.Length != prefabs.Length ){
					_gpwwProbabilityList = new float[prefabs.Length];
				}
				for( int pi = 0; pi < prefabs.Length; pi++ ){
					_gpwwProbabilityList[pi] = prefabs[pi].randomWeight;
				}

				// Reset total
				_gpwwTotal = 0;

				// Loop through the probabilities and sum up the total
				for( int p = 0; p < _gpwwProbabilityList.Length; p++ ){
					_gpwwTotal += _gpwwProbabilityList[p];
				}

				// Get a random value (0-1) and multiply it by the summed total
				_gpwwRandomPoint = Random.value * _gpwwTotal;

				// Loop through probabilities again ...
				for (int i = 0; i < _gpwwProbabilityList.Length; i++) {
					
					// if the random point is less than this prefab's randomWeight, return it
					if (_gpwwRandomPoint < _gpwwProbabilityList[i]) {
						return prefabs[i];
					
					// Otherwise, subtract the randomWeight from the random point and continue
					} else {
						_gpwwRandomPoint -= _gpwwProbabilityList[i];
					}
				}

				// If nothing was found, return the last item in the prefab list
				return prefabs[_gpwwProbabilityList.Length - 1];
			}

		// ==================================================================================================================
		//	GET SPAWN ROTATION
		//	Get the rotation to use when spawning an instance
		// ==================================================================================================================

		Quaternion GetSpawnRotation( PrefabToSpawn pts ){

			// Use the prefab's default Rotation
			if( rotationMode == RotationMode.PrefabDefault ){
				return pts.defaultPrefabRotation * Quaternion.Euler( customRotationEulerAngles );
			
			// Use Spawner Transform's Scale
			} else if ( rotationMode == RotationMode.SpawnerRotation ){
				return _theTransform.rotation * Quaternion.Euler( customRotationEulerAngles );

			// Use Last Spawn Point Rotation
			} else if ( rotationMode == RotationMode.SpawnPointRotation ){

				// If we cached the last spawnPoint, use its rotation
				if( _lastSpawnPointUsed != null ){
					return _lastSpawnPointUsed.rotation * Quaternion.Euler( customRotationEulerAngles );
				
				// If something goes wrong, use the Transform's rotation
				} else {
					return _theTransform.rotation * Quaternion.Euler( customRotationEulerAngles );
				}

			// Use Custom EulerAngles
			} else if ( rotationMode == RotationMode.CustomEulerAngles ){
				return Quaternion.Euler( customRotationEulerAngles );

			// Use Random Rotation
			} else if ( rotationMode == RotationMode.RandomRotation ){
				return Random.rotation;

			}

			// If something goes wrong, use the Transform's rotation
			return _theTransform.rotation;

		}

		// ==================================================================================================================
		//	GET SPAWN SCALE
		//	Get the local scale to use when spawning an instance
		// ==================================================================================================================

		[System.NonSerialized] float _gssProportionalScale = 1f;
		Vector3 GetSpawnScale( PrefabToSpawn pts ){

			// Use the prefab's default Rotation
			if( scaleMode == ScaleMode.PrefabDefault ){
				return pts.defaultPrefabLocalScale;
			
			// Use Spawner Transform's Scale
			} else if ( scaleMode == ScaleMode.SpawnerScale ){
				return _theTransform.localScale;

			// Use Custom Local Scale
			} else if ( scaleMode == ScaleMode.CustomLocalScale ){
				return customLocalScale;
			
			// Use Random Range Scale
			} else if ( scaleMode == ScaleMode.RandomRangeScale ){
				return new Vector3 (
										Random.Range( customLocalScaleMin.x, customLocalScaleMax.x ),
										Random.Range( customLocalScaleMin.y, customLocalScaleMax.y ),
										Random.Range( customLocalScaleMin.z, customLocalScaleMax.z )
									);
			
			// Use Random Range Proportional Scale
			} else if ( scaleMode == ScaleMode.RandomRangeProportionalScale ){
				
				_gssProportionalScale = Random.Range( customLocalScaleProportionalMin, customLocalScaleProportionalMax );
				return new Vector3 ( _gssProportionalScale, _gssProportionalScale, _gssProportionalScale );
			}

			// If something goes wrong, return Vector3.one
			return Vector3.one;

		}

		// ==================================================================================================================
		//	GET SPAWN PARENT
		//	Get what parent we should use when spawning an instance
		// ==================================================================================================================

		Transform GetSpawnParent( Transform spawnPoint = null ){

			// If we're not reparenting this instance, return null
			if( reparentInstances == ParentMode.Ignore ){
				return null;

			// Use this Spawner's Transform
			} else if( reparentInstances == ParentMode.ReparentToSpawner ){
				return _theTransform;

			// Use the Spawn Point 
			} else if( reparentInstances == ParentMode.ReparentToSpawnPoint ){
				
				// If we're not using spawn points but this is selected, just use the Spawner's transform
				if( spawnPoint == null ){
					return _theTransform;

				// Otherwise, return the current spawn point	
				} else {
					Debug.LogWarning("WE NEED TO IMPLEMENT THIS!!");
				}

			// Use the custom Transform
			} else if( reparentInstances == ParentMode.ReparentToCustomTransform ){
				return customParentTransform;

			}

			// If something goes wrong, return null
			return null;

		}

		// ==================================================================================================================
		//	GET SPAWN POSITION
		//	We get the location of where to spawn the instance (as a vector3 position)
		// ==================================================================================================================

		Vector3 GetSpawnPosition(){

			// If we're supposed to spawn at this transform, send the transform
			if( prefabsWillBeSpawned == SpawnLocation.AtThisTransform ){
				return _theTransform.position;

			// If we're supposed to spawn using the Transform List...
			} else if( prefabsWillBeSpawned == SpawnLocation.UsingTransformList ){
				return ( GetSpawnPositionFromTransformList() );

			// If we're supposed to spawn using the Positions list (local) ...
			} else if( prefabsWillBeSpawned == SpawnLocation.UsingLocalPositionList ){
				return ( GetSpawnPositionFromPositionList(false) );

			// If we're supposed to spawn using the Positions list (global) ...
			} else if( prefabsWillBeSpawned == SpawnLocation.UsingGlobalPositionList ){
				return ( GetSpawnPositionFromPositionList(true) );

			}
			

			// Do the others later!
			return _theTransform.position;
		}

		// ==================================================================================================================
		//	GET SPAWN POSITION FROM TRANSFORM LIST
		//	We get the correct position using the Transform List
		// ==================================================================================================================

		// Helper
		[System.NonSerialized] public int _gstIndex = 0;				// The index of the prefab to use
		[System.NonSerialized] public bool _gstIndexPingPong = false;	// The direction of the ping pong

		Vector3 GetSpawnPositionFromTransformList(){

			// Check that we have at least a single Transform in the list
			if( spawnpointTransforms.Length == 0 ){ 
				_lastSpawnPointUsed = _theTransform;
				return _lastSpawnPointUsed.position;

			// If there is only one prefab in the list, just return that
			} else if(spawnpointTransforms.Length==1){ 
			//	return spawnpointTransforms[0].GetVector3();
				return SetupAndReturnTransformSpawnPoint( spawnpointTransforms[0] );

			// Sequence Ascending
			} else if( spawnPointSelection == SpawnPointSelection.SequenceAscending ){ 
			//	return GetTransformSequenceAscending().GetVector3();
				return SetupAndReturnTransformSpawnPoint( GetTransformSequenceAscending() );

			// Sequence Descending
			} else if( spawnPointSelection == SpawnPointSelection.SequenceDescending ){ 
			//	return GetTransformSequenceDescending().GetVector3();
				return SetupAndReturnTransformSpawnPoint( GetTransformSequenceDescending() );

			// Ping-Pong Ascending
			} else if( spawnPointSelection == SpawnPointSelection.PingPongAscending ){ 
			//	return GetTransformPingPongAscending().GetVector3();
				return SetupAndReturnTransformSpawnPoint( GetTransformPingPongAscending() );

			// Ping-Pong Descending
			} else if( spawnPointSelection == SpawnPointSelection.PingPongDescending ){ 
			//	return GetTransformPingPongDescending().GetVector3();
				return SetupAndReturnTransformSpawnPoint( GetTransformPingPongDescending() );

			// Random
			} else if( spawnPointSelection == SpawnPointSelection.Random ){ 
			//	return GetTransformRandom().GetVector3();
				return SetupAndReturnTransformSpawnPoint( GetTransformRandom() );

			// Random With Weights
			} else if( spawnPointSelection == SpawnPointSelection.RandomWithWeights ){ 
			//	return GetTransformRandomWithWeights().GetVector3();
				return SetupAndReturnTransformSpawnPoint( GetTransformRandomWithWeights() );
			}

			// If something goes wrong, return the spawner's transform
			return _theTransform.position;
		}

			// ---------------------------------------
			// SETUP AND RETURN TRANSFORM SPAWN POINT
			//	This caches the last spawnPoint used
			// ---------------------------------------

			Vector3 SetupAndReturnTransformSpawnPoint( TransformSpawnPoint tsp ){
				_lastSpawnPointUsed = tsp.spawnPoint;
				return tsp.GetVector3();
			}

			// --------------------
			// SEQUENCE ASCENDING
			// --------------------

			TransformSpawnPoint GetTransformSequenceAscending(){
				
				// Add to the index
				_gstIndex++;

				// If we've reached the end, loop back to the first entry
				if( _gstIndex >= spawnpointTransforms.Length ){ _gstIndex = 0; }

				// Return the prefab
				return spawnpointTransforms[_gstIndex];
			}

			// --------------------
			// SEQUENCE DESCENDING
			// --------------------

			TransformSpawnPoint GetTransformSequenceDescending(){
				
				// Subtract from the index
				_gstIndex--;

				// If we've reached the end, loop back to the last entry
				if( _gstIndex < 0 ){ _gstIndex = spawnpointTransforms.Length-1; }

				// Return the prefab
				return spawnpointTransforms[_gstIndex];
			}

			// --------------------
			// PING-PONG ASCENDING
			// --------------------

			TransformSpawnPoint GetTransformPingPongAscending(){
				
				// Ping Pong FALSE
				if( _gstIndexPingPong == false ){

					// Add to the index
					_gstIndex++;

					// If we've reached the end, loop back to the first entry
					if( _gstIndex >= spawnpointTransforms.Length ){ 
						_gstIndex = spawnpointTransforms.Length-2; 
						_gstIndexPingPong = true;
					}

				// Ping Pong TRUE
				} else if( _gstIndexPingPong == true ){

					// Subtract from the index
					_gstIndex--;

					// If we've gone further than the start, go back to the first entry
					if( _gstIndex < 0 ){ 
						_gstIndex = 1; 
						_gstIndexPingPong = false;
					}
				}

				// Return the prefab
				return spawnpointTransforms[_gstIndex];
			}

			// --------------------
			// PING-PONG DESCENDING
			// --------------------

			TransformSpawnPoint GetTransformPingPongDescending(){
				
				// Ping Pong FALSE
				if( _gstIndexPingPong == false ){

					// Subtract from the index
					_gstIndex--;

					// If we've gone further than the start, go back to the first entry
					if( _gstIndex < 0 ){ 
						_gstIndex = 1; 
						_gstIndexPingPong = true;
					}

				// Ping Pong TRUE
				} else if( _gstIndexPingPong == true ){

					// Add to the index
					_gstIndex++;

					// If we've reached the end, loop back to the first entry
					if( _gstIndex >= spawnpointTransforms.Length ){ 
						_gstIndex = spawnpointTransforms.Length-2; 
						_gstIndexPingPong = false;
					}
				}

				// Return the prefab
				return spawnpointTransforms[_gstIndex];
			}

			// --------------------
			// RANDOM
			// --------------------

			TransformSpawnPoint GetTransformRandom(){
				
				// Add to the index
				_gstIndex = Random.Range( 0, spawnpointTransforms.Length );

				// Keep it within range
				if( _gstIndex >= spawnpointTransforms.Length ){ _gstIndex = spawnpointTransforms.Length-1; }

				// Return the prefab
				return spawnpointTransforms[_gstIndex];
			}

			// --------------------
			// RANDOM WITH WEIGHTS
			// --------------------

			// Helpers
			[System.NonSerialized] float[] _gtwwProbabilityList = new float[0];
			[System.NonSerialized] float _gtwwTotal = 0;
			[System.NonSerialized] float _gtwwRandomPoint = 0;

			// Method
			// Based on Unity's "Choose" Example: https://docs.unity3d.com/Manual/RandomNumbers.html
			TransformSpawnPoint GetTransformRandomWithWeights(){
				
				// Convert the prefab list into a list of probability floats (randomWeights)
				if( _gtwwProbabilityList.Length != spawnpointTransforms.Length ){
					_gtwwProbabilityList = new float[spawnpointTransforms.Length];
				}
				for( int pi = 0; pi < spawnpointTransforms.Length; pi++ ){
					_gtwwProbabilityList[pi] = spawnpointTransforms[pi].randomWeight;
				}

				// Reset total
				_gtwwTotal = 0;

				// Loop through the probabilities and sum up the total
				for( int p = 0; p < _gtwwProbabilityList.Length; p++ ){
					_gtwwTotal += _gtwwProbabilityList[p];
				}

				// Get a random value (0-1) and multiply it by the summed total
				_gtwwRandomPoint = Random.value * _gtwwTotal;

				// Loop through probabilities again ...
				for (int i = 0; i < _gtwwProbabilityList.Length; i++) {
					
					// if the random point is less than this spawnPoint's randomWeight, return it
					if (_gtwwRandomPoint < _gtwwProbabilityList[i]) {
						return spawnpointTransforms[i];
					
					// Otherwise, subtract the randomWeight from the random point and continue
					} else {
						_gtwwRandomPoint -= _gtwwProbabilityList[i];
					}
				}

				// If nothing was found, return the last item in the spawnPoint list
				return spawnpointTransforms[_gtwwProbabilityList.Length - 1];
			}

		// ==================================================================================================================
		//	GET SPAWN POSITION FROM POSITION LIST
		//	We get the correct position using the Position List
		// ==================================================================================================================

		// Helper
		[System.NonSerialized] public int _gptIndex = 0;							// The index of the prefab to use
		[System.NonSerialized] public bool _gptIndexPingPong = false;				// The direction of the ping pong
		[System.NonSerialized] public Vector3 _gptpositionOffset = Vector3.zero;	// Used for local / global positions

		Vector3 GetSpawnPositionFromPositionList( bool globalPositions ){

			// ===============================
			// SETUP LOCAL / GLOBAL POSITIONS
			// ===============================

			// Setup the position offset (this helps to convert between global / local positions )
			if( globalPositions == true ){ _gptpositionOffset = Vector3.zero; }
			else { 

				// If postions are local to a custom transform, set that up here ...
				if( spawnPositionsAreLocalTo == SpawningLocalTo.CustomTransform && spawnLocalTo != null ){
					_gptpositionOffset = spawnLocalTo.position; 

				// Otherwise, spawn local to the spawner
				} else {
					_gptpositionOffset = _theTransform.position; 
				}

			}

			// =====================================
			//	GET THE NEXT POSITION FROM THE LIST
			// =====================================

			// Check that we have at least a single Transform in the list
			if( spawnpointPositions.Length == 0 ){ 
				return _theTransform.position;

			// If there is only one prefab in the list, just return that
			} else if(spawnpointPositions.Length==1){ 
				return spawnpointPositions[0].GetVector3( _gptpositionOffset );

			// Sequence Ascending
			} else if( spawnPointSelection == SpawnPointSelection.SequenceAscending ){ 
				return GetPositionSequenceAscending().GetVector3( _gptpositionOffset );

			// Sequence Descending
			} else if( spawnPointSelection == SpawnPointSelection.SequenceDescending ){ 
				return GetPositionSequenceDescending().GetVector3( _gptpositionOffset );

			// Ping-Pong Ascending
			} else if( spawnPointSelection == SpawnPointSelection.PingPongAscending ){ 
				return GetPositionPingPongAscending().GetVector3( _gptpositionOffset );

			// Ping-Pong Descending
			} else if( spawnPointSelection == SpawnPointSelection.PingPongDescending ){ 
				return GetPositionPingPongDescending().GetVector3( _gptpositionOffset );

			// Random
			} else if( spawnPointSelection == SpawnPointSelection.Random ){ 
				return GetPositionRandom().GetVector3( _gptpositionOffset );

			// Random With Weights
			} else if( spawnPointSelection == SpawnPointSelection.RandomWithWeights ){ 
				return GetPositionRandomWithWeights().GetVector3( _gptpositionOffset );
			}

			// If something goes wrong, return the spawner's position
			return _theTransform.position;
		}

			// --------------------
			// SEQUENCE ASCENDING
			// --------------------

			PositionSpawnPoint GetPositionSequenceAscending(){
				
				// Add to the index
				_gptIndex++;

				// If we've reached the end, loop back to the first entry
				if( _gptIndex >= spawnpointPositions.Length ){ _gptIndex = 0; }

				// Return the prefab
				return spawnpointPositions[_gptIndex];
			}

			// --------------------
			// SEQUENCE DESCENDING
			// --------------------

			PositionSpawnPoint GetPositionSequenceDescending(){
				
				// Subtract from the index
				_gptIndex--;

				// If we've reached the end, loop back to the last entry
				if( _gptIndex < 0 ){ _gptIndex = spawnpointPositions.Length-1; }

				// Return the prefab
				return spawnpointPositions[_gptIndex];
			}

			// --------------------
			// PING-PONG ASCENDING
			// --------------------

			PositionSpawnPoint GetPositionPingPongAscending(){
				
				// Ping Pong FALSE
				if( _gptIndexPingPong == false ){

					// Add to the index
					_gptIndex++;

					// If we've reached the end, loop back to the first entry
					if( _gptIndex >= spawnpointPositions.Length ){ 
						_gptIndex = spawnpointPositions.Length-2; 
						_gptIndexPingPong = true;
					}

				// Ping Pong TRUE
				} else if( _gptIndexPingPong == true ){

					// Subtract from the index
					_gptIndex--;

					// If we've gone further than the start, go back to the first entry
					if( _gptIndex < 0 ){ 
						_gptIndex = 1; 
						_gptIndexPingPong = false;
					}
				}

				// Return the prefab
				return spawnpointPositions[_gptIndex];
			}

			// --------------------
			// PING-PONG DESCENDING
			// --------------------

			PositionSpawnPoint GetPositionPingPongDescending(){
				
				// Ping Pong FALSE
				if( _gptIndexPingPong == false ){

					// Subtract from the index
					_gptIndex--;

					// If we've gone further than the start, go back to the first entry
					if( _gptIndex < 0 ){ 
						_gptIndex = 1; 
						_gptIndexPingPong = true;
					}

				// Ping Pong TRUE
				} else if( _gptIndexPingPong == true ){

					// Add to the index
					_gptIndex++;

					// If we've reached the end, loop back to the first entry
					if( _gptIndex >= spawnpointPositions.Length ){ 
						_gptIndex = spawnpointPositions.Length-2; 
						_gptIndexPingPong = false;
					}
				}

				// Return the prefab
				return spawnpointPositions[_gptIndex];
			}

			// --------------------
			// RANDOM
			// --------------------

			PositionSpawnPoint GetPositionRandom(){
				
				// Add to the index
				_gptIndex = Random.Range( 0, spawnpointPositions.Length );

				// Keep it within range
				if( _gptIndex >= spawnpointPositions.Length ){ _gptIndex = spawnpointPositions.Length-1; }

				// Return the prefab
				return spawnpointPositions[_gptIndex];
			}

			// --------------------
			// RANDOM WITH WEIGHTS
			// --------------------

			// Helpers
			[System.NonSerialized] float[] _gposwwProbabilityList = new float[0];
			[System.NonSerialized] float _gposwwTotal = 0;
			[System.NonSerialized] float _gposwwRandomPoint = 0;

			// Method
			// Based on Unity's "Choose" Example: https://docs.unity3d.com/Manual/RandomNumbers.html
			PositionSpawnPoint GetPositionRandomWithWeights(){
				
				// Convert the prefab list into a list of probability floats (randomWeights)
				if( _gposwwProbabilityList.Length != spawnpointPositions.Length ){
					_gposwwProbabilityList = new float[spawnpointPositions.Length];
				}
				for( int pi = 0; pi < spawnpointPositions.Length; pi++ ){
					_gposwwProbabilityList[pi] = spawnpointPositions[pi].randomWeight;
				}

				// Reset total
				_gposwwTotal = 0;

				// Loop through the probabilities and sum up the total
				for( int p = 0; p < _gposwwProbabilityList.Length; p++ ){
					_gposwwTotal += _gposwwProbabilityList[p];
				}

				// Get a random value (0-1) and multiply it by the summed total
				_gposwwRandomPoint = Random.value * _gposwwTotal;

				// Loop through probabilities again ...
				for (int i = 0; i < _gposwwProbabilityList.Length; i++) {
					
					// if the random point is less than this spawnPoint's randomWeight, return it
					if (_gposwwRandomPoint < _gposwwProbabilityList[i]) {
						return spawnpointPositions[i];
					
					// Otherwise, subtract the randomWeight from the random point and continue
					} else {
						_gposwwRandomPoint -= _gposwwProbabilityList[i];
					}
				}

				// If nothing was found, return the last item in the spawnPoint list
				return spawnpointPositions[_gposwwProbabilityList.Length - 1];
			}


		// ==================================================================================================================
		//	API METHODS
		//	The core public methods for the Spawner
		// ==================================================================================================================

		// API: STOP THE SPAWNER
		public void Stop(){

			state = State.Stopped;
			runSpawner = false;
			
		}

		// API: PLAY THE SPAWNER
		public void Play(){

			// Only Play the spawner if we're not already playing
			if( state != State.Playing ){

				// If we were currently stopped, reset the setup on the next frame
				if( state == State.Stopped ){ runSpawnerRequiresSetup = true; }

				// If we're currently Paused, we should set the state to Playing so it can continue ... 
				else if( state == State.Paused ){ state = State.Playing; }
					
				// Run the spawner!
				runSpawner = true;
				
			}
		}

		// API: RESTART AND PLAY THE SPAWNER
		public void RestartAndPlay(){

			// This will always reset the spawner even if it is already playing!
			runSpawnerRequiresSetup = true;
			runSpawner = true;
		}

		// API: PAUSE the spawner
		public void Pause(){

			// Only Pause the spawner if we are currently playing
			if( state == State.Playing ){
				state = State.Paused;
			}
		}

		// API: RESUME the spawner
		public void Resume(){

			// If the spawner is paused, then resume where we left off
			if( state == State.Paused ){
				state = State.Playing;

			// If we're stopped, restart the spawner	
			} else if( state == State.Stopped ){
				Play();
			}

			// NOTE: If we're already playing, we ignore this.
		}

		// ==================================================================================================================
		//	ADVANCED API METHODS
		//	The more advanced public API methods for the Spawner
		// ==================================================================================================================

		// API: CAN SPAWN?
		public bool CanSpawn(){
			
			// Return true if the spawner is ready to create new instances ...
			if( spawningBegins != StartMode.Never /*&& state == State.Playing*/ && prefabs.Length > 0 ){
				return true;
			}

			// Otherwise, return false
			return false;
		}

		// API: ADD PREFAB TO SPAWNER
		public void AddPrefabToSpawner( GameObject prefab, float randomWeight = 100f ){
			
			// Make sure the user isn't trying to add an empty prefab
			if(prefab==null){ 
				Debug.LogWarning("POOLKIT (Spawner): Cannot add a prefab that is null.");
				return; 
			}

			// Add a new PrefabToSpawn item to the prefabs array
			if( Arrays.AddItem( ref prefabs, new PrefabToSpawn() ) ){

				// Set the prefab and randomWeight and then Setup the class.
				prefabs[prefabs.Length-1].prefab = prefab;
				prefabs[prefabs.Length-1].randomWeight = randomWeight;
				prefabs[prefabs.Length-1].Setup();
			}
		}

		// API: REMOVE PREFAB FROM SPAWNER
		public void RemovePrefabFromSpawner ( GameObject prefab ){

			// Loop through the prefabs and find a matching prefab
			for( int i = 0; i < prefabs.Length; i++ ){
				if( prefabs[i].prefab == prefab ){
					
					// Remove the PrefabToSpawn from the array
					Arrays.RemoveItemAtIndex( ref prefabs, i );

					// stop the method as soon as we've removed an item
					return;
				}
			}
		}

		// API: REPLACE PREFAB IN SPAWNER
		public void ReplacePrefabInSpawner( GameObject oldPrefab, GameObject newPrefab ){

			// Make sure the user isn't trying to add an empty prefab
			if(newPrefab==null){ 
				Debug.LogWarning("POOLKIT (Spawner): Cannot add a prefab that is null.");
				return; 
			}

			// Loop through the prefabs and find a matching prefab
			for( int i = 0; i < prefabs.Length; i++ ){
				if( prefabs[i].prefab == oldPrefab ){
					
					// Replace the prefab
					prefabs[i].prefab = newPrefab;
					prefabs[i].pool = null;
					prefabs[i].hasPool = false;

					// Setup this PrefabToSpawn again
					prefabs[i].Setup();

					// stop the method as soon as we've replaced an item
					return;
				}
			}
		}

		// API: SET RANDOMIZATION WEIGHT OF PREFAB (BY PREFAB)
		public void SetRandomWeightOfPrefab( GameObject prefab, float newWeight ){

			// Loop through the prefabs and find a matching prefab
			for( int i = 0; i < prefabs.Length; i++ ){
				if( prefabs[i].prefab == prefab ){
					
					// Set Random Weight
					prefabs[i].randomWeight = newWeight;

					// stop the method as soon as we've removed an item
					return;
				}
			}
		}

		// API: SET RANDOMIZATION WEIGHT OF PREFAB (BY ARRAY POSITION)
		public void SetRandomWeightOfPrefab( int arrayIndex, float newWeight ){

			// Make sure the array index is within range
			if( arrayIndex >= 0 && arrayIndex < prefabs.Length ){
					
				// Set Random Weight
				prefabs[arrayIndex].randomWeight = newWeight;
			
			// Otherwise, show an error
			} else {
				Debug.LogWarning("POOLKIT (Spawner): Couldn't change random weight because the array index is out of range.");
			}
		}

		// API: SET RANDOMIZATION WEIGHT OF VECTOR3 POSITION (BY ARRAY POSITION)
		public void SetRandomWeightOfVector3Position( int arrayIndex, float newWeight ){

			// Make sure the array index is within range
			if( arrayIndex >= 0 && arrayIndex < spawnpointPositions.Length ){
					
				// Set Random Weight
				spawnpointPositions[arrayIndex].randomWeight = newWeight;
			
			// Otherwise, show an error
			} else {
				Debug.LogWarning("POOLKIT (Spawner): Couldn't change random weight because the array index is out of range.");
			}
		}

		// API: SET RANDOMIZATION WEIGHT OF TRANSFORM POSITION (BY ARRAY POSITION)
		public void SetRandomWeightOfTransformPosition( int arrayIndex, float newWeight ){

			// Make sure the array index is within range
			if( arrayIndex >= 0 && arrayIndex < spawnpointTransforms.Length ){
					
				// Set Random Weight
				spawnpointTransforms[arrayIndex].randomWeight = newWeight;
			
			// Otherwise, show an error
			} else {
				Debug.LogWarning("POOLKIT (Spawner): Couldn't change random weight because the array index is out of range.");
			}
		}

		// API: SET SPAWNPOINT POSITION
		public void SetSpawnPointPosition( int arrayIndex, Vector3 newPosition ){

			// Make sure the array index is within range
			if( arrayIndex >= 0 && arrayIndex < spawnpointPositions.Length ){
					
				// Set Random Weight
				spawnpointPositions[arrayIndex].spawnPoint = newPosition;
			
			// Otherwise, show an error
			} else {
				Debug.LogWarning("POOLKIT (Spawner): Couldn't set spawnpoint position because the array index is out of range.");
			}
		}

		// ==================================================================================================================
		//	DEBUG MODE
		//	Test API, etc in the Editor
		// ==================================================================================================================
		/*
		#if UNITY_EDITOR

			void LateUpdate(){

				// Play
				if( Input.GetKeyUp(KeyCode.P) ){
					Play();
				}

				// Restart & Play
				if( Input.GetKeyUp(KeyCode.O) ){
					RestartAndPlay();
				}

				// Stop
				if( Input.GetKeyUp(KeyCode.S) ){
					Stop();
				}

				// Resume
				if( Input.GetKeyUp(KeyCode.R) ){
					Resume();
				}

				// Pause
				if( Input.GetKeyUp(KeyCode.Escape) ){
					Pause();
				}

			}

		#endif
		*/

// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//	UNITY EDITOR ONLY
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		// ==================================================================================================================
		//	VISUAL UI / GIZMOS
		//	Show Gizmos in the editor to make the spawner easier to work with. This uses PoolKit Preferences!
		// ==================================================================================================================
		
		#if UNITY_EDITOR

			// Locally Cached Editor Settings
			[System.NonSerialized] bool _onlyShowWhenSelected = true;
			[System.NonSerialized] bool _shouldScaleGizmos = true;
			[System.NonSerialized] bool _shouldShowSpawnPointLabels = true;
			[System.NonSerialized] bool _shouldShowSpawnerName = true;
			[System.NonSerialized] float _lineWidth = 2f;
			[System.NonSerialized] Color _lineColor = new Color(0f,1f,1f,1f);

			// Gizmo Paths
			const string _gizmoSpawnerPath = "Hell Tap Entertainment/PoolKit/PoolKitSpawner.png";
			const string _gizmoSpawnPointPath = "Hell Tap Entertainment/PoolKit/PoolKitSpawnPoint.png";

			// Helper Variables
			[System.NonSerialized] float _sceneCamDistance = 0f;
			[System.NonSerialized] float _uiAlpha = 0f;
			[System.NonSerialized] Color _replaceGUIColor = new Color();
			[System.NonSerialized] float _dynamicLabelUIScale = -3f;
			[System.NonSerialized] bool _sceneCamInRange = false;
			[System.NonSerialized] float _labelViewableRange = 25f;
			[System.NonSerialized] Vector2 _uiLabelOffset = new Vector2(0,50);
			[System.NonSerialized] GUIStyle _uiLabelStyle = new GUIStyle();
			[System.NonSerialized] Rect _currentLabelRect = new Rect();

			//==================================================================================================================
			//	ON DRAW GIZMOS / SELECTED
			//	Handles when to render the gizmos
			//==================================================================================================================

			// If we should show gizmos all the time, do it here ...
			void OnDrawGizmos () {
				UpdateGizmoSettings();
				if( _onlyShowWhenSelected == false ){ 
					DrawGizmoUI(); 
				}
			}

			// If we should show gizmos only when selected, do it here ...
			void OnDrawGizmosSelected () {
				UpdateGizmoSettings();
				if( _onlyShowWhenSelected == true ){ 
					DrawGizmoUI(); 
				}
			}

			//==================================================================================================================
			//	DRAW TEXT LABEL
			//	Renders the cool spawnpoint and spawner name labels in the Scene View
			//==================================================================================================================

			// Helpers
			const string dtlNewColorLine = "\n<color=#";
			const string dtlEndBraket = ">";
			const string dtlEndColor = "</color>";
			[System.NonSerialized] float dtl_labelViewableRange = 30f;
			[System.NonSerialized] float dtlFadeRange = 0f;

			// Method
			void DrawTextLabel( Vector3 position, string line1Text, string line2Text, bool useWhiteText = false, 
								float width = 100f, float height = 32f, bool ignoreCameraRange = false, float extraFadeRange = 0f 
			){

				// Make sure we can see the camera in the scene first ...
				if( SceneView.currentDrawingSceneView != null && SceneView.currentDrawingSceneView.camera != null ){

					// IF THE CAMERA IS IN FRONT OF THE LABEL, STOP NOW!
					if( Vector3.Dot( (position - SceneView.currentDrawingSceneView.camera.transform.position), // Heading
									 SceneView.currentDrawingSceneView.camera.transform.forward 	// Forward of camera
									) <= 0f	// NOTE: -1 is totally behind, 1 is totally in front!
					){
						//Debug.Log("Camera is in front of label!");
						return;
					}

					// If we should handle the camera range, do it now ...
					if( ignoreCameraRange == false ){

						dtl_labelViewableRange = _labelViewableRange + extraFadeRange;
						dtlFadeRange = 10f;

			 			// Cache the distance between the camera and the spawn point
			 			_sceneCamDistance = Vector3.Distance( position, SceneView.currentDrawingSceneView.camera.transform.position );
			 			_sceneCamInRange = ( _sceneCamDistance < dtl_labelViewableRange ? true : false );
			 			
			 			// Setup the alpha for fading in the labels
			 			if( _sceneCamInRange && _sceneCamDistance < dtl_labelViewableRange && _sceneCamDistance > dtl_labelViewableRange-dtlFadeRange){
			 				_uiAlpha = (dtlFadeRange - Mathf.Abs( (dtl_labelViewableRange - dtlFadeRange) - _sceneCamDistance )) * (1/dtlFadeRange);
			 			} else if(_sceneCamInRange==false){ 
			 				_uiAlpha = 0f;
			 			} else { 
			 				_uiAlpha = 1f;
			 			}

						// If the camera is not in range, don't set up anything else
				 		if(_sceneCamInRange==false){ return; }

				 	// Otherwise, force uiAlpha to be 1 so it renders!
			 		} else {

			 			_uiAlpha = 1f;
			 		}

					// START THE HANDLES GUI
				 	Handles.BeginGUI();

			 		GUI.backgroundColor = Color.black;
			 		if(useWhiteText){ 
			 			_uiLabelStyle.normal.textColor = Color.white; 
			 			_uiLabelStyle.richText = true;
			 		}

			 		// Set GUI Opacity without memory allocations
			 		_replaceGUIColor.r = GUI.color.r;
			 		_replaceGUIColor.g = GUI.color.g;
			 		_replaceGUIColor.b = GUI.color.b;
			 		_replaceGUIColor.a = _uiAlpha;
			 		GUI.color = _replaceGUIColor;

			 		// Setup the Rect without any memory allocations
			 		_currentLabelRect.x = (HandleUtility.WorldToGUIPoint( position ).x - (width*0.5f)) + _uiLabelOffset.x;
			 		_currentLabelRect.y = (HandleUtility.WorldToGUIPoint( position ).y - (height*0.5f)) + -_uiLabelOffset.y;
			 		_currentLabelRect.width = width;
			 		_currentLabelRect.height = height;

			 		// Try to balance out the Y position as we zoom in and out
			 		_currentLabelRect.y = _currentLabelRect.y + ( (_labelViewableRange - _sceneCamDistance) * _dynamicLabelUIScale );

			 		// Make sure the we don't move the label lower than the actual target Y position
			 		if( _currentLabelRect.y > (HandleUtility.WorldToGUIPoint( position ).y - (height*0.5f)) + -_uiLabelOffset.y ){
			 			_currentLabelRect.y = (HandleUtility.WorldToGUIPoint( position ).y - (height*0.5f)) + -_uiLabelOffset.y;
			 		}

			 		// Draw the label and apply the automatic white color setting with alpha on the second line
			 		if( line2Text != System.String.Empty ){
			 			GUI.Label(  _currentLabelRect, line1Text + dtlNewColorLine + ColorToHex( Color.white, _uiAlpha ) + dtlEndBraket + line2Text + dtlEndColor , _uiLabelStyle );
			 		} else {
			 			GUI.Label(  _currentLabelRect, line1Text, _uiLabelStyle );
			 		}

			 		// Restore GUI Color
			 		GUI.color = Color.white;

			 		// Restore GUI Style text color
			 		_uiLabelStyle.normal.textColor = _lineColor; 

			 		// END THE HANDLES GUI
			        Handles.EndGUI();
		    	}
			}


			//==================================================================================================================
			//	UPDATE GIZMO SETTINGSS
			//	Connects to the Preferences in the Editor to setup the UI
			//==================================================================================================================

			// Helpers
			const string PoolKit_Spawner_OnlyShowWhenSelected = "PoolKit_Spawner_OnlyShowWhenSelected";
			const string PoolKit_Spawner_ScaleGizmos = "PoolKit_Spawner_ScaleGizmos";
			const string PoolKit_Spawner_ShowSpawnPointLabels = "PoolKit_Spawner_ShowSpawnPointLabels";
			const string PoolKit_Spawner_ShowSpawnerName = "PoolKit_Spawner_ShowSpawnerName";
			const string PoolKit_Spawner_LineWidth = "PoolKit_Spawner_LineWidth";
			const string PoolKit_Spawner_LineColorR = "PoolKit_Spawner_LineColorR";
			const string PoolKit_Spawner_LineColorG = "PoolKit_Spawner_LineColorG";
			const string PoolKit_Spawner_LineColorB = "PoolKit_Spawner_LineColorB";
			const string PoolKit_Spawner_LineColorA = "PoolKit_Spawner_LineColorA";

			// Method
			void UpdateGizmoSettings(){

				// Cache all the EditorPrefs settings
				_onlyShowWhenSelected = UnityEditor.EditorPrefs.GetBool (PoolKit_Spawner_OnlyShowWhenSelected, true );
				_shouldScaleGizmos = UnityEditor.EditorPrefs.GetBool (PoolKit_Spawner_ScaleGizmos, true );
				_shouldShowSpawnPointLabels = UnityEditor.EditorPrefs.GetBool (PoolKit_Spawner_ShowSpawnPointLabels, true );
				_shouldShowSpawnerName = UnityEditor.EditorPrefs.GetBool (PoolKit_Spawner_ShowSpawnerName, true );

				_lineWidth = UnityEditor.EditorPrefs.GetFloat (PoolKit_Spawner_LineWidth, 2f );
				_lineColor = new Color 	(
											UnityEditor.EditorPrefs.GetFloat (PoolKit_Spawner_LineColorR, 0f ),
											UnityEditor.EditorPrefs.GetFloat (PoolKit_Spawner_LineColorG, 1f ),
											UnityEditor.EditorPrefs.GetFloat (PoolKit_Spawner_LineColorB, 1f ),
											UnityEditor.EditorPrefs.GetFloat (PoolKit_Spawner_LineColorA, 1f )
										);

		 		// Setup the GUI Style to use for the labels
		 		if( _uiLabelStyle.normal.textColor != _lineColor ){
			 		_uiLabelStyle = new GUIStyle(EditorStyles.miniButton);
			 		_uiLabelStyle.richText = true;
			 		_uiLabelStyle.fontSize = 10;
			 		_uiLabelStyle.alignment = TextAnchor.MiddleCenter;
			 		_uiLabelStyle.normal.background = Texture2D.whiteTexture;
					_uiLabelStyle.normal.textColor = _lineColor;
				}
			}

			//==================================================================================================================
			//	COLOR TO HEX
			//	We use this function to return a color as a hex, also handles the alpha channel too!
			//==================================================================================================================

			// Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
			const string _X2 = "X2";
			int _colorToHexAlpha = 0;
			string ColorToHex( Color32 color  ){ return ColorToHex( color, color.a ); }
			string ColorToHex( Color32 color, float maxAlpha ){
				
				// Handle Alpha
				maxAlpha = Mathf.Clamp01(maxAlpha);		// Make sure the supplied alpha is within range (0-1)

				// The alpha channel is actually a byte with a range of 0 - 225
				_colorToHexAlpha = color.a;	// This seems to convert the byte into an int.
				if( _colorToHexAlpha > maxAlpha*255 ){ _colorToHexAlpha = Mathf.RoundToInt(maxAlpha*255); }
				//Debug.Log(_colorToHexAlpha);

				// Make sure the hex is lowercase to work
				// NOTE: The trick to get the alpha working is to multiply it by 255.
				return ( color.r.ToString(_X2) + color.g.ToString(_X2) + color.b.ToString(_X2) + (_colorToHexAlpha).ToString(_X2) ).ToLower();
			}

			//==================================================================================================================
			//	DRAW GIZMO UI
			//	The main method that draws the UI in the Scene View
			//==================================================================================================================

			// Helpers
			const string dguiTransform = "Transform ";
			const string dguiPosition = "Position ";
			const string dguiStartBold = "<b>";
			const string dguiEndBold = "</b>";

			// Core method to show UI helpers
			void DrawGizmoUI(){

				// Draw main Spawner Gizmo
				Gizmos.DrawIcon(transform.position, _gizmoSpawnerPath, _shouldScaleGizmos );

				// Show spawn points for the transform list
				if( prefabsWillBeSpawned == SpawnLocation.UsingTransformList ){

					for( int i = 0; i < spawnpointTransforms.Length; i++ ){
						if( spawnpointTransforms[i] != null && spawnpointTransforms[i].spawnPoint != null ){
							
							// Draw Spawn Point Icon
							Gizmos.DrawIcon( spawnpointTransforms[i].spawnPoint.position, _gizmoSpawnPointPath, _shouldScaleGizmos );

							// Draw Line
							UnityEditor.Handles.color = _lineColor;
							UnityEditor.Handles.DrawDottedLine( transform.position, spawnpointTransforms[i].spawnPoint.position, _lineWidth );

							// Draw text label
							if( _shouldShowSpawnPointLabels){
								DrawTextLabel(	spawnpointTransforms[i].spawnPoint.position, 
												dguiTransform + (i+1).ToString(),
												spawnpointTransforms[i].spawnPoint.name
											);
							}
						}
					}
				
				// Show spawn points for the Position list (Local)
				} else if( prefabsWillBeSpawned == SpawnLocation.UsingLocalPositionList ){

					for( int i = 0; i < spawnpointPositions.Length; i++ ){
						if( spawnpointPositions[i] != null ){
							
							// If we're using a custom override, show that here
							if( spawnPositionsAreLocalTo == SpawningLocalTo.CustomTransform && spawnLocalTo != null ){

								// Draw Spawn Point Icon
								Gizmos.DrawIcon( ( spawnLocalTo.position + spawnpointPositions[i].spawnPoint ), _gizmoSpawnPointPath, _shouldScaleGizmos );

								// Draw Line
								UnityEditor.Handles.color = _lineColor;
								UnityEditor.Handles.DrawDottedLine( spawnLocalTo.position, ( spawnLocalTo.position + spawnpointPositions[i].spawnPoint ), _lineWidth );

								// Draw text label
								if( _shouldShowSpawnPointLabels){
									DrawTextLabel( spawnLocalTo.position + spawnpointPositions[i].spawnPoint, 
													dguiPosition + (i+1).ToString(),
													spawnpointPositions[i].spawnPoint.ToString()
												);
								}
							
							// If we're not using a custom override, display the line normally
							} else {

								// Draw Spawn Point Icon
								Gizmos.DrawIcon( ( transform.position + spawnpointPositions[i].spawnPoint ), _gizmoSpawnPointPath, _shouldScaleGizmos );

								// Draw Line
								UnityEditor.Handles.color = _lineColor;
								UnityEditor.Handles.DrawDottedLine( transform.position, ( transform.position + spawnpointPositions[i].spawnPoint ), _lineWidth );

								// Draw text label
								if( _shouldShowSpawnPointLabels){
									DrawTextLabel( transform.position + spawnpointPositions[i].spawnPoint, 
													dguiPosition + (i+1).ToString(),
													spawnpointPositions[i].spawnPoint.ToString()
												);
								}
							}
						}

						// If we're using a custom override, connect the custom point to the spawner in the editor
						if( spawnPositionsAreLocalTo == SpawningLocalTo.CustomTransform && spawnLocalTo != null ){
							UnityEditor.Handles.DrawLine( transform.position, spawnLocalTo.position );
						}
					}

				// Show spawn points for the Position list (Global)
				} else if( prefabsWillBeSpawned == SpawnLocation.UsingGlobalPositionList ){

					for( int i = 0; i < spawnpointPositions.Length; i++ ){
						if( spawnpointPositions[i] != null ){
							
							// Draw Spawn Point Icon
							Gizmos.DrawIcon( spawnpointPositions[i].spawnPoint, _gizmoSpawnPointPath, _shouldScaleGizmos );
							
							// Draw Line
							UnityEditor.Handles.color = _lineColor;
							UnityEditor.Handles.DrawDottedLine( transform.position, spawnpointPositions[i].spawnPoint, _lineWidth );

							// Draw text label
							if( _shouldShowSpawnPointLabels){
								DrawTextLabel(	spawnpointPositions[i].spawnPoint, 
												dguiPosition + (i+1).ToString(),
												spawnpointPositions[i].spawnPoint.ToString()
											);
							}
						}
					}
				}

				// Do Label for the spawner
				if( _shouldShowSpawnerName ){
					DrawTextLabel( transform.position, dguiStartBold+spawnerName+dguiEndBold, System.String.Empty, true, 100f, 24f, false, 15f );
				}


				// Reset gizmo / Handle colors
				UnityEditor.Handles.color = Color.white;
			}

		#endif

	}
}