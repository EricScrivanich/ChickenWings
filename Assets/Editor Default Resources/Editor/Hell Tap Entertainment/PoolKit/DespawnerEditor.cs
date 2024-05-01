using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using HellTap.PoolKit;
using System.Reflection;

namespace HellTap.PoolKit {

	[CustomEditor(typeof(Despawner))]
	public class SpawnerEditor : Editor {

		// Static Icons
		static Texture2D headerIcon, spawnerIcon, spawnerSmallIcon, prefabSmallIcon, eventsIcon, despawnerIcon, despawnerSmallIcon, analyticsIcon, AnalyticsSmallIcon, buttonIcon, origin3dLabel, gearLabel, xLabel, cubeLabel, loopLabel, layersLabel, resizeLabel, timeLabel, playLabel, upButton, downButton, particleSystemIcon, audioSourceIcon, rigidbodyIcon, widthLabel, foldoutDown, foldoutRight;

		// =======================
		//	CORE EDITOR VARIABLES
		// =======================

		// Main Object
		Despawner o;

		// Helpers for tabs
		GUIContent[] tabGUIContent = new GUIContent[]{ new GUIContent(), new GUIContent(), new GUIContent() };
		string[] tabLabels = new string[]{ "  Despawner", "  Chain Spawning", " Events" };
		Texture2D[] tabIcons = new Texture2D[]{ despawnerSmallIcon, spawnerSmallIcon, gearLabel };
		
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	ON ENABLE
		//	Caches all editor icons, subscribes to delegates, and performs GameObject checks
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		void OnEnable(){

			// Cache the icons
			headerIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/PoolKit.png") as Texture2D;
			spawnerIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/Spawner.png") as Texture2D;
			spawnerSmallIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/SpawnerSmall.png") as Texture2D;
			prefabSmallIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/PrefabSmall.png") as Texture2D;
			eventsIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/Events.png") as Texture2D;
			despawnerIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/Despawner.png") as Texture2D;
			despawnerSmallIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/DespawnerSmall.png") as Texture2D;
			buttonIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/buttonLabel.png") as Texture2D;
			origin3dLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/origin3dLabel.png") as Texture2D;
			gearLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/gearLabel.png") as Texture2D;
			xLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/xLabel.png") as Texture2D;
			cubeLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/cubeLabel.png") as Texture2D; 
			loopLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/loopLabel.png") as Texture2D;
			layersLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/layersLabel.png") as Texture2D;
			resizeLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/resizeLabel.png") as Texture2D;
			timeLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/timeLabel.png") as Texture2D;
			playLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/playLabel.png") as Texture2D;
			widthLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/widthLabel.png") as Texture2D;
			upButton = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/upButton.png") as Texture2D;
			downButton = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/downButton.png") as Texture2D;

			// Cache the foldout icons
			foldoutDown = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Shared/foldoutDown.png") as Texture2D;
			foldoutRight = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Shared/foldoutRight.png") as Texture2D;
			
			// Check foldout icons to remove warnings in the editor
			if( foldoutDown != null && foldoutRight != null ){}

			// Grab Icons from UnityEditor
			particleSystemIcon = (Texture2D)EditorGUIUtility.ObjectContent(null, typeof(ParticleSystem)).image;
			audioSourceIcon = (Texture2D)EditorGUIUtility.ObjectContent(null, typeof(AudioSource)).image;
			rigidbodyIcon = (Texture2D)EditorGUIUtility.ObjectContent(null, typeof(Rigidbody)).image;

			// Update Tab Sources
			tabGUIContent = new GUIContent[]{ new GUIContent(), new GUIContent(), new GUIContent() };
			tabLabels = new string[]{ "  Despawner", "  Chain Spawning", " Events" };
			tabIcons = new Texture2D[]{ despawnerSmallIcon, spawnerSmallIcon, gearLabel };
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	ON INSPECTOR GUI
		//	Core Update Method
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Local Preferences
		bool _showPoolKitHeaders = true;
		bool _showTabHeaders = true;
		bool _showHelpfulNotes = true;

		// Method
		public override void OnInspectorGUI(){

			// Cache the Editor preferences for showing helpful notes and tab titles
			_showPoolKitHeaders = EditorPrefs.GetBool ("PoolKit_Editors_ShowPoolKitHeaders", true );
			_showTabHeaders = EditorPrefs.GetBool ("PoolKit_Editors_ShowTabHeaders", true );
			_showHelpfulNotes = EditorPrefs.GetBool ("PoolKit_Editors_ShowHelpfulNotes", true );

			// Render PoolKit Header
			if( _showPoolKitHeaders ){
				HTGUI.Header(headerIcon, "Despawner", "This tool allows you to setup how this GameObject will be despawned. When this happens, you can optionally spawn new objects in its place by configuring chain-spawning.", "PoolKit", 80, 13);
			} else {
				EditorGUILayout.Space();
			}

			// Cache the target correctly
			if( target as Despawner != null ){ o = target as Despawner; }

			// Start Body Template
			if( o != null ){

				// Setup the tabs
				for( int i = 0; i < tabGUIContent.Length; i++ ){
					tabGUIContent[i].text = tabLabels[i];
					tabGUIContent[i].image = tabIcons[i];
					tabGUIContent[i].tooltip = System.String.Empty;
				}

				// Show the tabs
				o.tab = HTGUI.HTSelectionGrid( null, o.tab, tabGUIContent, 3, true, HT_GUIStyles.GetSelectionGridGUIStyle() );

				// Start The body template
				//HTGUI.StartBody();
				HTGUI.StartBigLayout();

				// Do Content!
				DoContent();

				// End the body template
				HTGUI.EndBigLayout();
				//HTGUI.EndBody();
				//EditorGUILayout.Space();
			}

		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO CONTENT
		//	Do the content
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Method
		void DoContent(){

			// Do Despawner Setup
			if( o.tab == 0 ){ DoDespawnerSetup(); }
			if( o.tab == 1 ){ DoChainSpawningSetup(); }
			if( o.tab == 2 ){ DoEventsSetup(); }

			// If we changed anything in the GUI, mark the object as dirty (so it can be saved)
			if( GUI.changed ){
				EditorUtility.SetDirty( o );
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	SPACED SEPERATED LINE WITH TITLE
		//	Quick way to make a new section in the middle of a GUI Box
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		void SpacedSepLineTitle( string title, string subtitle ){
			
			EditorGUILayout.Space();
			HTGUI.SepLine();
			HTGUI.SectionTitle( title, subtitle );
			EditorGUILayout.Space();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO HELP BOX
		//	Quick way to make a help box with the correct color tints
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		void DoHelpBox( string text, MessageType messageType = MessageType.Info, bool wide = true, bool addTopSpace = false, bool addBottomSpace = false ){
			
			// Let the HT_GUIStyles class handle help boxes now
			HT_GUIStyles.DoHelpBox( text, messageType, wide, addTopSpace, addBottomSpace );
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	NUMBER FORMAT
		//	Converts a number and singluar / purable item to a readable string
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		const string _space = " ";
		string NumberFormat( float number, string singular, string plural ){
			if( number == 1f ){ return number.ToString() + _space + singular; }
			return number + _space + plural;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	START / END ICON COLUMN
		//	Creates an Icon on the left and sets up the content column on the right
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		string _empty = string.Empty;
		void StartIconColumn( Texture2D icon, int iconSizeX, int iconSizeY, int spacing = 16 ){

			if( _showTabHeaders ){ return; }

			// Start Row
			GUILayout.BeginHorizontal();

				// Icon Column
				GUILayout.BeginVertical( GUILayout.MinWidth(iconSizeX), GUILayout.MaxWidth(iconSizeX), GUILayout.MinHeight(iconSizeY), GUILayout.MaxHeight(iconSizeY) );
					GUILayout.Label( icon, GUILayout.MinWidth(iconSizeX), GUILayout.MaxWidth(iconSizeY), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false) );
				GUILayout.EndVertical();

				// Column With Spacing
				GUILayout.BeginVertical( GUILayout.MinWidth(spacing), GUILayout.MaxWidth(spacing), GUILayout.MinHeight(iconSizeY), GUILayout.MaxHeight(iconSizeY), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false) );
					GUILayout.Label( _empty, GUILayout.MinWidth(spacing), GUILayout.MaxWidth(spacing) );
				GUILayout.EndVertical();

				// Start Of Content Column
				GUILayout.BeginVertical();

		}

		void EndIconColumn( bool addSpace = true ){

			if( _showTabHeaders ){ return; }

				// End Of Content Column
				GUILayout.EndVertical();

			// Completely End Row
			GUILayout.EndHorizontal();

			// Add space at the end
			if(addSpace){ EditorGUILayout.Space(); }
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	IS CHILD OBJECT
		//	Checks to see if theTransform parameter is a child object of this prefab
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		bool IsChildObject( Transform theTransform ){

			// Make sure theTransform and the current despawner is valid ...
			if( theTransform != null && o != null ){

				// Set T to theTransform
				Transform t = theTransform;
				while( t.parent != null ){

					// If the parent of theTransform is the main GameObject, return true!
					if( t.parent == o.transform  ){
						return true;
					}

					// Otherwise, set t to the latest parent and keep moving up the hierarchy
					t = t.parent;
				}
			}

			// If we still haven't found the parent, return false.
			return false;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO DESPAWNER SETUP
		//	Do the Despawner Setup
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Default Layout Options
		GUILayoutOption[] defaultGUILayoutOptions = new GUILayoutOption[]{GUILayout.MinWidth(200), GUILayout.MaxWidth(600), GUILayout.MaxHeight(16)};
		Vector2 _v2_100_64 = new Vector2(100, 64);
		string _helpBoxString = string.Empty;

		// Method
		void DoDespawnerSetup(){

			// =================================
			//	TAB TITLE
			// =================================

			// Show Tab Headers
			if( _showTabHeaders ){

				// Start White Box
				HTGUI.StartWhiteBox();

					// Title
					EditorGUILayout.Space();
					HTGUI.Header( despawnerIcon, "Despawner", "This tab allows you to setup how this GameObject will be despawned and returned to the pool. ", System.String.Empty, _v2_100_64, 13
					);

				// End White Box
				HTGUI.EndWhiteBox();
			}

			// =================================
			//	MAIN TAB CONTENT
			// =================================

			// Start White Box
			HTGUI.StartWhiteBox();

				// Creates an Icon on the left and sets up the content column on the right
				StartIconColumn( despawnerIcon, 64, 64, 4 );

					// Title
					HTGUI.SectionTitle( "Despawner", "Setup how this GameObject will be despawned." );
					EditorGUILayout.Space();

					// Despawn Mode
					o.despawnMode = (Despawner.DespawnMode)HTGUI_UNDO.EnumField( o, "Despawn This GameObject", new GUIContent( "Despawn This GameObject:", gearLabel, _empty ), o.despawnMode, defaultGUILayoutOptions );


					// -----------------
					//	AFTER COUNTDOWN
					// -----------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterCountdown ){
					
						// Show countdown
						o.despawnCountdown = HTGUI_UNDO.FloatField( o, "Countdown In Seconds", new GUIContent( "Countdown In Seconds:", timeLabel, _empty ), o.despawnCountdown, defaultGUILayoutOptions  );

						// Show help 
						if( _showHelpfulNotes ){

							DoHelpBox( "After " + o.despawnCountdown.ToString() + " seconds, this GameObject will be despawned.");
						
						}
					}

					// -----------------------------------
					//	AFTER COUNTDOWN WITH RANDOM RANGE
					// -----------------------------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterCountdownWithRandomRange ){
					
						// Show countdown
						o.despawnCountdownRandomMin = HTGUI_UNDO.FloatField( o, "Min Countdown In Seconds", new GUIContent( "Min Countdown In Seconds:", timeLabel, _empty ), o.despawnCountdownRandomMin, defaultGUILayoutOptions  );
						o.despawnCountdownRandomMax = HTGUI_UNDO.FloatField( o, "Max Countdown In Seconds", new GUIContent( "Max Countdown In Seconds:", timeLabel, _empty ), o.despawnCountdownRandomMax, defaultGUILayoutOptions  );

						// Make sure the maximum range is not lower than the minimum range
						if( o.despawnCountdownRandomMax < o.despawnCountdownRandomMin ){ o.despawnCountdownRandomMax = o.despawnCountdownRandomMin; }

						// Show help 
						if( _showHelpfulNotes ){

							DoHelpBox( "After " + o.despawnCountdownRandomMin.ToString() + " to " + o.despawnCountdownRandomMax.ToString() + " seconds, this GameObject will be despawned.");
						
						}
					}

					// -----------------
					//	PARTICLE SYSTEM
					// -----------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterParticleSystemFinishes ){
					
						// Which GameObject To Track
						o.particleSystemToUse = (Despawner.ReferenceLocation)HTGUI_UNDO.EnumField( o, "Use The Particle System", new GUIContent( "Use The Particle System:", gearLabel, _empty ), o.particleSystemToUse, defaultGUILayoutOptions );

						// Choose Custom Object
						if( o.particleSystemToUse == Despawner.ReferenceLocation.OnAnotherGameObject){

							o.useThisParticleSystem = (ParticleSystem) HTGUI_UNDO.ObjectField( o, "Particle System To Use",  new GUIContent( "Particle System To Use:", particleSystemIcon, _empty ), o.useThisParticleSystem, true, defaultGUILayoutOptions );
						}

						// Auto-Play
						o.playParticleSystemOnSpawn = HTGUI_UNDO.ToggleField( o, "Auto-Play Particle System", new GUIContent( "Auto-Play Particle System:", playLabel, _empty ), o.playParticleSystemOnSpawn, defaultGUILayoutOptions );


						// Show help 
						if( _showHelpfulNotes ){

							// No Particle System exists on THIS gameObject
							if( o.particleSystemToUse == Despawner.ReferenceLocation.OnThisGameObject && 
								o.gameObject.GetComponent<ParticleSystem>() == null 
							){
								DoHelpBox( "IMPORTANT: This will never be despawned because no Particle System exists on this GameObject!", MessageType.Error );

							// No Particle System is set on a custom object
							} else if( o.particleSystemToUse == Despawner.ReferenceLocation.OnAnotherGameObject && 
								o.useThisParticleSystem == null
							){
								DoHelpBox( "IMPORTANT: This will never be despawned because no Particle System has been set!", MessageType.Error );

							// Custom object is not a child object
							} else if( o.particleSystemToUse == Despawner.ReferenceLocation.OnAnotherGameObject && 
								o.useThisParticleSystem != null &&
								IsChildObject( o.useThisParticleSystem.transform ) == false
							){
								DoHelpBox( "IMPORTANT: The chosen Particle System is NOT a child of this GameObject!", MessageType.Error );
							}

							// Otherwise, we're all good!
							else {
								DoHelpBox( "This GameObject will be despawned when the Particle System finishes." + ( o.playParticleSystemOnSpawn ? " When this GameObject is spawned, the Particle System will automatically play." : _empty ));
							}


						}
					}

					// -----------------
					//	AUDIO SOURCE
					// -----------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterAudioSourceFinishes ){
					
						// Which GameObject To Track
						o.audioSourceToUse = (Despawner.ReferenceLocation)HTGUI_UNDO.EnumField( o, "Use The Audio Source", new GUIContent( "Use The Audio Source:", gearLabel, _empty ), o.audioSourceToUse, defaultGUILayoutOptions );

						// Choose Custom Object
						if( o.audioSourceToUse == Despawner.ReferenceLocation.OnAnotherGameObject){

							o.useThisAudioSource = (AudioSource) HTGUI_UNDO.ObjectField( o, "Audio Source To Use",  new GUIContent( "Audio Source To Use:", audioSourceIcon, _empty ), o.useThisAudioSource, true, defaultGUILayoutOptions );
						}

						// Auto-Play
						o.playAudioSourceOnSpawn = HTGUI_UNDO.ToggleField( o, "Auto-Play Audio Source", new GUIContent( "Auto-Play Audio Source:", playLabel, _empty ), o.playAudioSourceOnSpawn, defaultGUILayoutOptions );


						// Show help 
						if( _showHelpfulNotes ){

							// No AudioSource exists on THIS gameObject
							if( o.audioSourceToUse == Despawner.ReferenceLocation.OnThisGameObject && 
								o.gameObject.GetComponent<AudioSource>() == null 
							){
								DoHelpBox( "IMPORTANT: This will never be despawned because no Audio Source exists on this GameObject!", MessageType.Error );

							// No Particle System is set on a custom object
							} else if( o.audioSourceToUse == Despawner.ReferenceLocation.OnAnotherGameObject && 
								o.useThisAudioSource == null
							){
								DoHelpBox( "IMPORTANT: This will never be despawned because no Audio Source has been set!", MessageType.Error );

							// Custom object is not a child object
							} else if( o.audioSourceToUse == Despawner.ReferenceLocation.OnAnotherGameObject && 
								o.useThisAudioSource != null &&
								IsChildObject( o.useThisAudioSource.transform ) == false
							){
								DoHelpBox( "IMPORTANT: The chosen Audio Source is NOT a child of this GameObject!", MessageType.Error );

							// No audio clip set!
							} else if( 

								(	// This gameobject has no Audio clip ...
									o.audioSourceToUse == Despawner.ReferenceLocation.OnThisGameObject && 
									o.gameObject.GetComponent<AudioSource>() != null &&
									o.gameObject.GetComponent<AudioSource>().clip == null
								)

							||
								
								(	// Child object has no Audio clip ...
									o.audioSourceToUse == Despawner.ReferenceLocation.OnAnotherGameObject && 
									o.useThisAudioSource != null &&
									o.useThisAudioSource.clip == null
								)
							){
								DoHelpBox( "IMPORTANT: The chosen Audio Source does NOT have an AudioClip set!", MessageType.Error );
							}

							// Otherwise, we're all good!
							else {
								DoHelpBox( "This GameObject will be despawned when the Audio Source finishes playing." + ( o.playAudioSourceOnSpawn ? " When this GameObject is spawned, the Audio Source will automatically play." : _empty ));
							}


						}
					}

					// ---------------------------
					//	ALL COLLISION-BASED EVENTS
					// ----------------------------

					// Show Source of Events when using collisions ...
					if( o.despawnMode == Despawner.DespawnMode.AfterPhysicsCollisionEvent ||
						o.despawnMode == Despawner.DespawnMode.AfterPhysicsTriggerEvent ||
						o.despawnMode == Despawner.DespawnMode.AfterPhysicsCollision2DEvent ||
						o.despawnMode == Despawner.DespawnMode.AfterPhysicsTrigger2DEvent
					){


						// Title - SOURCE OF EVENTS
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Source Of Events", "Set which collider should be tracked for triggering despawn events." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();

						// Source Of Collisions
						o.sourceOfCollisions = (Despawner.CollisionSource)HTGUI_UNDO.EnumField( o, "Source Collider", new GUIContent( "Source Collider:", gearLabel, _empty ), o.sourceOfCollisions, defaultGUILayoutOptions );
						
						// Show Custom GameObject
						if( o.sourceOfCollisions == Despawner.CollisionSource.ThisGameObject ){	

							// If we're using a 3D Collision, make sure we have a Collider
							if( (	o.despawnMode == Despawner.DespawnMode.AfterPhysicsCollisionEvent ||
									o.despawnMode == Despawner.DespawnMode.AfterPhysicsTriggerEvent ) &&
								o.gameObject.GetComponent<Collider>() == null 
							){
								DoHelpBox( "This GameObject doesn't have a Collider and cannot send any Events!", MessageType.Error );

							// If we're using a 2D Collision, make sure we have a Collider
							} else if(	(	o.despawnMode == Despawner.DespawnMode.AfterPhysicsCollision2DEvent ||
											o.despawnMode == Despawner.DespawnMode.AfterPhysicsTrigger2DEvent ) &&
										o.gameObject.GetComponent<Collider2D>() == null 
							){
								DoHelpBox( "This GameObject doesn't have a 2D Collider and cannot send any Events!", MessageType.Error );
									
							} else if( _showHelpfulNotes){
								DoHelpBox( "Events will be tracked using the collider on this GameObject. The relevant components will be automatically added at runtime to optimize performance." );
							}

						// Show Custom GameObject
						} else if( o.sourceOfCollisions == Despawner.CollisionSource.AnotherChildGameObject ){					

							o.collisionSourceGameObject = (GameObject) HTGUI_UNDO.ObjectField( o, "Use This GameObject",  new GUIContent( "Use This GameObject:", cubeLabel, _empty ), o.collisionSourceGameObject, true, defaultGUILayoutOptions );

							// Show Help Notes
							if( o.collisionSourceGameObject == null ){
								DoHelpBox( "You will not recieve any events because you haven't chosen a source GameObject!", MessageType.Error );
							}

							// Source GameObject doesn't have a collider
							else if( o.collisionSourceGameObject != null && o.collisionSourceGameObject.GetComponent<Collider>() == null ){
								DoHelpBox( "The GameObject you have selected doesn't have a collider and cannot send any Events!", MessageType.Error );
							}

							// Source GameObject isn't a child
							else if( o.collisionSourceGameObject != null && IsChildObject( o.collisionSourceGameObject.transform ) == false ){
								DoHelpBox( "The GameObject you have selected is NOT a child of this GameObject!", MessageType.Error );
							}

							// Show info
							else if( _showHelpfulNotes ){
								DoHelpBox( "Events will be tracked using the collider on the GameObject named: '" + o.collisionSourceGameObject.name +"'. The relevant components will be added to it at runtime to optimize performance." );
							}

						// Manual Setup
						} else if( _showHelpfulNotes && o.sourceOfCollisions == Despawner.CollisionSource.ManualSetup ){	
							DoHelpBox( "Advanced users can use the Manual Setup to place 'Despawner Event' components on specific child objects to customize the source of the Events." );
						}
					}

					// ---------------------------
					//	ALL RAYCAST-BASED EVENTS
					// ----------------------------

					// Show Source of Events when using Raycasts ...
					if( o.despawnMode == Despawner.DespawnMode.AfterPhysicsRaycastEvent ||
						o.despawnMode == Despawner.DespawnMode.AfterPhysicsRaycast2DEvent
					){


						// Title - SOURCE OF EVENTS
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Source Of Events", "Setup the Raycast start position." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();

						// Source Of Collisions
						o.sourceOfCollisions = (Despawner.CollisionSource)HTGUI_UNDO.EnumField( o, "Source of Raycast", new GUIContent( "Source of Raycast:", gearLabel, _empty ), o.sourceOfCollisions, defaultGUILayoutOptions );
						
						// Show Custom GameObject
						if( o.sourceOfCollisions == Despawner.CollisionSource.ThisGameObject ){	

							if( _showHelpfulNotes){
								DoHelpBox( "Raycasts will be performed from this GameObject. The relevant components will be automatically added at runtime to optimize performance." );
							}

						// Show Custom GameObject
						} else if( o.sourceOfCollisions == Despawner.CollisionSource.AnotherChildGameObject ){					

							o.collisionSourceGameObject = (GameObject) HTGUI_UNDO.ObjectField( o, "Use This GameObject",  new GUIContent( "Use This GameObject:", cubeLabel, _empty ), o.collisionSourceGameObject, true, defaultGUILayoutOptions );

							// Show Help Notes
							if( o.collisionSourceGameObject == null ){
								DoHelpBox( "Raycasts cannot be performed because you haven't chosen a source GameObject!", MessageType.Error );
							}

							// Source GameObject isn't a child
							else if( o.collisionSourceGameObject != null && IsChildObject( o.collisionSourceGameObject.transform ) == false ){
								DoHelpBox( "The GameObject you have selected is NOT a child of this GameObject!", MessageType.Error );
							}

							// Show info
							else if( _showHelpfulNotes ){
								DoHelpBox( "Raycasts will be performed using the GameObject named: '" + o.collisionSourceGameObject.name +"'. The relevant components will be added to it at runtime to optimize performance." );
							}

						// Manual Setup
						} else if( _showHelpfulNotes && o.sourceOfCollisions == Despawner.CollisionSource.ManualSetup ){	
							DoHelpBox( "It is not recommended to use Manual Setups on Raycast events.", MessageType.Warning );
						}
					}

					// -----------------------
					//	AFTER RAYCAST 3D EVENT
					// -----------------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterPhysicsRaycastEvent ){

						// Title - EVENTS TO USE
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "3D Raycast Settings", "Setup the direction and distance of your raycast as well as the maximum number of hits you can test against. Options to collide with triggers are also available. You can visualize the raycast in the Scene view, the yellow line represents its direction and the red line represents its distance." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();

						// Direction
						o.raycast3DDirection = HTGUI_UNDO.Vector3Field( o, "Raycast Direction", new GUIContent( "Raycast Direction:", origin3dLabel, _empty ), o.raycast3DDirection, defaultGUILayoutOptions  );

						// Distance
						o.raycast3DDistance = HTGUI_UNDO.FloatField( o, "Raycast Distance", new GUIContent( "Raycast Distance:", widthLabel, _empty ), o.raycast3DDistance, defaultGUILayoutOptions  );
						
						// Cache Size for RaycastHit[] array
						o.raycast3DmaxHits = HTGUI_UNDO.IntField( o, "Maximum Hits", new GUIContent( "Maximum Hits:", xLabel, _empty ), o.raycast3DmaxHits, defaultGUILayoutOptions  );

						// Collide With Triggers
						o.queryTriggerInteraction = (QueryTriggerInteraction)HTGUI_UNDO.EnumField( o, "Collide With Triggers", new GUIContent( "Collide With Triggers:", gearLabel, _empty ), o.queryTriggerInteraction, defaultGUILayoutOptions );


						// Title - SOURCE OF EVENTS
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Options", "In addition to using an optional despawner countdown, you can choose to reset the velocity of an attached Rigidbody when an instance of this prefab is spawned." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();

						// Reset Rigidbodies
						o.resetRigidbodyVelocitiesOnSpawn = HTGUI_UNDO.ToggleField( o, "Reset Velocities On Spawn", new GUIContent( "Reset Velocities On Spawn:", loopLabel, _empty ), o.resetRigidbodyVelocitiesOnSpawn, defaultGUILayoutOptions  );

						// Also use countdown
						o.raycast3DAlsoDespawnAfterCountdown = HTGUI_UNDO.ToggleField( o, "Use Countdown", new GUIContent( "Use Countdown:", gearLabel, _empty ), o.raycast3DAlsoDespawnAfterCountdown, defaultGUILayoutOptions  );
						
						// If we're also using a countdown, show the timer
						if( o.raycast3DAlsoDespawnAfterCountdown){
							o.raycast3DCountdown = HTGUI_UNDO.FloatField( o, "Despawn Countdown", new GUIContent( "Despawn Countdown:", timeLabel, _empty ), o.raycast3DCountdown, defaultGUILayoutOptions  );
							if( o.raycast3DCountdown < 0 ){ o.raycast3DCountdown = 0; } // Make sure countdown isn't less than 0

							// Show help 
							if( _showHelpfulNotes ){

								DoHelpBox( "If an Event doesn't trigger after " + o.raycast3DCountdown.ToString() + " seconds of being spawned, this GameObject will be despawned.");
							
							}
						}

					}

					// -----------------------
					//	AFTER RAYCAST 2D EVENT
					// -----------------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterPhysicsRaycast2DEvent ){

						// Title - EVENTS TO USE
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "2D Raycast Settings", "Setup the direction and distance of your raycast as well as the maximum number of hits you can test against. Options to limit Z depth are also available. You can visualize the raycast in the Scene view, the yellow line represents its direction and the red line represents its distance." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();

						// Direction
						o.raycast2DDirection = HTGUI_UNDO.Vector2Field( o, "Raycast Direction", new GUIContent( "Raycast Direction:", origin3dLabel, _empty ), o.raycast2DDirection, defaultGUILayoutOptions );

						// Distance
						o.raycast2DDistance = HTGUI_UNDO.FloatField( o, "Raycast Distance", new GUIContent( "Raycast Distance:", widthLabel, _empty ), o.raycast2DDistance, defaultGUILayoutOptions );
						
						// Cache Size for RaycastHit[] array
						o.raycast2DmaxHits = HTGUI_UNDO.IntField( o, "Maximum Hits", new GUIContent( "Maximum Hits:", xLabel, _empty ), o.raycast2DmaxHits, defaultGUILayoutOptions );

						// Minimum Z Depth
						o.raycast2DMinZDepth = HTGUI_UNDO.FloatField( o, "Minimum Z Depth", new GUIContent( "Minimum Z Depth:", gearLabel, _empty ), o.raycast2DMinZDepth, defaultGUILayoutOptions );

						// Maximum Z Depth
						o.raycast2DMaxZDepth = HTGUI_UNDO.FloatField( o, "Maximum Z Depth", new GUIContent( "Maximum Z Depth:", gearLabel, _empty ), o.raycast2DMaxZDepth, defaultGUILayoutOptions );


						// Title - SOURCE OF EVENTS
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Options", "In addition to using an optional despawner countdown, you can choose to reset the velocity of an attached Rigidbody when an instance of this prefab is spawned." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();

						// Reset Rigidbodies
						o.resetRigidbodyVelocitiesOnSpawn = HTGUI_UNDO.ToggleField( o, "Reset Velocities On Spawn", new GUIContent( "Reset Velocities On Spawn:", loopLabel, _empty ), o.resetRigidbodyVelocitiesOnSpawn, defaultGUILayoutOptions  );

						// Also use countdown
						o.raycast2DAlsoDespawnAfterCountdown = HTGUI_UNDO.ToggleField( o, "Use Countdown", new GUIContent( "Use Countdown:", gearLabel, _empty ), o.raycast2DAlsoDespawnAfterCountdown, defaultGUILayoutOptions  );
						
						// If we're also using a countdown, show the timer
						if( o.raycast2DAlsoDespawnAfterCountdown){
							o.raycast2DCountdown = HTGUI_UNDO.FloatField( o, "Despawn Countdown", new GUIContent( "Despawn Countdown:", timeLabel, _empty ), o.raycast2DCountdown, defaultGUILayoutOptions  );
							if( o.raycast2DCountdown < 0 ){ o.raycast2DCountdown = 0; } // Make sure countdown isn't less than 0

							// Show help 
							if( _showHelpfulNotes ){

								DoHelpBox( "If an Event doesn't trigger after " + o.raycast2DCountdown.ToString() + " seconds of being spawned, this GameObject will be despawned.");
							
							}
						}

					}

					// -----------------------
					//	AFTER OVERLAP EVENT
					// -----------------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterPhysicsOverlapEvent ){

						// Title - EVENTS TO USE
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Physics Overlap Shape", "Choose and customize what shape should be used to check for physics collisions. You can visualize the shape of the Physics Overlap in the Scene view, it will be presented with a red outline." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();


						// Overlap Type
						o.overlapType = (Despawner.OverlapType)HTGUI_UNDO.EnumField( o, "Physics Overlap Type", new GUIContent( "Physics Overlap Type:", gearLabel, _empty ), o.overlapType, defaultGUILayoutOptions );

						// Query Trigger Interaction
						if( o.overlapType == Despawner.OverlapType.Sphere3D ||
							o.overlapType == Despawner.OverlapType.Box3D
						){
							o.queryTriggerInteraction = (QueryTriggerInteraction)HTGUI_UNDO.EnumField( o, "Collide With Triggers", new GUIContent( "Collide With Triggers:", gearLabel, _empty ), o.queryTriggerInteraction, defaultGUILayoutOptions );
						}

						// Local Position Offset
						o.overlapOffset = HTGUI_UNDO.Vector3Field( o, "Local Position Offset", new GUIContent( "Local Position Offset:", origin3dLabel, _empty ), o.overlapOffset, defaultGUILayoutOptions  );

						// Show Sphere controls
						if( o.overlapType == Despawner.OverlapType.Sphere3D ||
							o.overlapType == Despawner.OverlapType.Circle2D
						){
							
							// Radius
							o.overlapRadius = HTGUI_UNDO.FloatField( o, "Radius", new GUIContent( "Radius:", resizeLabel, _empty ), o.overlapRadius, defaultGUILayoutOptions  );
						
						// Show Cube controls
						} else if(  o.overlapType == Despawner.OverlapType.Box3D ||
									o.overlapType == Despawner.OverlapType.Box2D 
						){
							
							// Size
							o.overlapScale = HTGUI_UNDO.Vector3Field( o, "Scale", new GUIContent( "Scale:", resizeLabel, _empty ), o.overlapScale, defaultGUILayoutOptions  );
						}


						// Title - SOURCE OF EVENTS
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Options", "In addition to using an optional despawner countdown, you can choose to reset the velocity of an attached Rigidbody when an instance of this prefab is spawned." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();

						// Reset Rigidbodies
						o.resetRigidbodyVelocitiesOnSpawn = HTGUI_UNDO.ToggleField( o, "Reset Velocities On Spawn", new GUIContent( "Reset Velocities On Spawn:", loopLabel, _empty ), o.resetRigidbodyVelocitiesOnSpawn, defaultGUILayoutOptions  );

						// Also use countdown
						o.overlapAlsoDespawnAfterCountdown = HTGUI_UNDO.ToggleField( o, "Use Countdown", new GUIContent( "Use Countdown:", gearLabel, _empty ), o.overlapAlsoDespawnAfterCountdown, defaultGUILayoutOptions  );
						
						// If we're also using a countdown, show the timer
						if( o.overlapAlsoDespawnAfterCountdown){
							o.overlapCountdown = HTGUI_UNDO.FloatField( o, "Despawn Countdown", new GUIContent( "Despawn Countdown:", timeLabel, _empty ), o.overlapCountdown, defaultGUILayoutOptions  );
							if( o.overlapCountdown < 0 ){ o.overlapCountdown = 0; } // Make sure countdown isn't less than 0

							// Show help 
							if( _showHelpfulNotes ){

								DoHelpBox( "If an Event doesn't trigger after " + o.overlapCountdown.ToString() + " seconds of being spawned, this GameObject will be despawned.");
							
							}
						}

					}

					// -----------------------
					//	AFTER COLLISION EVENT
					// -----------------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterPhysicsCollisionEvent ){

						// Title - EVENTS TO USE
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Options", "Choose which Events will despawn this GameObject. You can optionally reset the velocity on Rigidbodies when they are spawned as well as using a countdown in addition to Physics Events." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();
					
						// Collision Events
						o.useCollisionEnter = HTGUI_UNDO.ToggleField( o, "Use On Collision Enter", new GUIContent( "Use On Collision Enter:", rigidbodyIcon, _empty ), o.useCollisionEnter, defaultGUILayoutOptions  );
						o.useCollisionStay = HTGUI_UNDO.ToggleField( o, "Use On Collision Stay", new GUIContent( "Use On Collision Stay:", rigidbodyIcon, _empty ), o.useCollisionStay, defaultGUILayoutOptions  );
						o.useCollisionExit = HTGUI_UNDO.ToggleField( o, "Use On Collision Exit", new GUIContent( "Use On Collision Exit:", rigidbodyIcon, _empty ), o.useCollisionExit, defaultGUILayoutOptions  );

						// Reset Rigidbodies
						o.resetRigidbodyVelocitiesOnSpawn = HTGUI_UNDO.ToggleField( o, "Reset Velocities On Spawn", new GUIContent( "Reset Velocities On Spawn:", loopLabel, _empty ), o.resetRigidbodyVelocitiesOnSpawn, defaultGUILayoutOptions  );

						// Also use countdown
						o.colliderAlsoDespawnAfterCountdown = HTGUI_UNDO.ToggleField( o, "Use Countdown", new GUIContent( "Use Countdown:", gearLabel, _empty ), o.colliderAlsoDespawnAfterCountdown, defaultGUILayoutOptions  );
						
						// If we're also using a countdown, show the timer
						if( o.colliderAlsoDespawnAfterCountdown){
							o.colliderCountdown = HTGUI_UNDO.FloatField( o, "Despawn Countdown", new GUIContent( "Despawn Countdown:", timeLabel, _empty ), o.colliderCountdown, defaultGUILayoutOptions  );
							if( o.colliderCountdown < 0 ){ o.colliderCountdown = 0; } // Make sure countdown isn't less than 0

							// Show help 
							if( _showHelpfulNotes ){

								DoHelpBox( "If an Event doesn't trigger after " + o.colliderCountdown.ToString() + " seconds of being spawned, this GameObject will be despawned.");
							
							}
						}
					}


					// -----------------------
					//	AFTER TRIGGER EVENT
					// -----------------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterPhysicsTriggerEvent ){

						// Title - EVENTS TO USE
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Options", "Choose which Events will despawn this GameObject. You can optionally reset the velocity on Rigidbodies when they are spawned as well as using a countdown in addition to Physics Events." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();
					
						// Trigger Events
						o.useTriggerEnter = HTGUI_UNDO.ToggleField( o, "Use On Trigger Enter", new GUIContent( "Use On Trigger Enter:", rigidbodyIcon, _empty ), o.useTriggerEnter, defaultGUILayoutOptions  );
						o.useTriggerStay = HTGUI_UNDO.ToggleField( o, "Use On Trigger Stay", new GUIContent( "Use On Trigger Stay:", rigidbodyIcon, _empty ), o.useTriggerStay, defaultGUILayoutOptions  );
						o.useTriggerExit = HTGUI_UNDO.ToggleField( o, "Use On Trigger Exit", new GUIContent( "Use On Trigger Exit:", rigidbodyIcon, _empty ), o.useTriggerExit, defaultGUILayoutOptions  );

						// Reset Rigidbodies
						o.resetRigidbodyVelocitiesOnSpawn = HTGUI_UNDO.ToggleField( o, "Reset Velocities On Spawn", new GUIContent( "Reset Velocities On Spawn:", loopLabel, _empty ), o.resetRigidbodyVelocitiesOnSpawn, defaultGUILayoutOptions  );

						// Also use countdown
						o.triggerAlsoDespawnAfterCountdown = HTGUI_UNDO.ToggleField( o, "Use Countdown", new GUIContent( "Use Countdown:", gearLabel, _empty ), o.triggerAlsoDespawnAfterCountdown, defaultGUILayoutOptions  );
						
						// If we're also using a countdown, show the timer
						if( o.triggerAlsoDespawnAfterCountdown){
							o.triggerCountdown = HTGUI_UNDO.FloatField( o, "Despawn Countdown", new GUIContent( "Despawn Countdown:", timeLabel, _empty ), o.triggerCountdown, defaultGUILayoutOptions  );
							if( o.triggerCountdown < 0 ){ o.triggerCountdown = 0; } // Make sure countdown isn't less than 0

							// Show help 
							if( _showHelpfulNotes ){

								DoHelpBox( "If an Event doesn't trigger after " + o.triggerCountdown.ToString() + " seconds of being spawned, this GameObject will be despawned.");
							
							}
						}
					}


					// --------------------------
					//	AFTER COLLISION 2D EVENT
					// --------------------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterPhysicsCollision2DEvent ){

						// Title - EVENTS TO USE
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Options", "Choose which Events will despawn this GameObject. You can optionally reset the velocity on Rigidbodies when they are spawned as well as using a countdown in addition to Physics Events." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();
					
						// Collision Events
						o.useCollisionEnter2D = HTGUI_UNDO.ToggleField( o, "Use On Collision Enter 2D", new GUIContent( "Use On Collision Enter 2D:", rigidbodyIcon, _empty ), o.useCollisionEnter2D, defaultGUILayoutOptions  );
						o.useCollisionStay2D = HTGUI_UNDO.ToggleField( o, "Use On Collision Stay 2D", new GUIContent( "Use On Collision Stay 2D:", rigidbodyIcon, _empty ), o.useCollisionStay2D, defaultGUILayoutOptions  );
						o.useCollisionExit2D = HTGUI_UNDO.ToggleField( o, "Use On Collision Exit 2D", new GUIContent( "Use On Collision Exit 2D:", rigidbodyIcon, _empty ), o.useCollisionExit2D, defaultGUILayoutOptions  );

						// Reset Rigidbodies
						o.resetRigidbodyVelocitiesOnSpawn = HTGUI_UNDO.ToggleField( o, "Reset Velocities On Spawn", new GUIContent( "Reset Velocities On Spawn:", loopLabel, _empty ), o.resetRigidbodyVelocitiesOnSpawn, defaultGUILayoutOptions  );

						// Also use countdown
						o.collider2DAlsoDespawnAfterCountdown = HTGUI_UNDO.ToggleField( o, "Use Countdown", new GUIContent( "Use Countdown:", gearLabel, _empty ), o.collider2DAlsoDespawnAfterCountdown, defaultGUILayoutOptions  );
						
						// If we're also using a countdown, show the timer
						if( o.collider2DAlsoDespawnAfterCountdown){
							o.collider2DCountdown = HTGUI_UNDO.FloatField( o, "Despawn Countdown", new GUIContent( "Despawn Countdown:", timeLabel, _empty ), o.collider2DCountdown, defaultGUILayoutOptions  );
							if( o.collider2DCountdown < 0 ){ o.collider2DCountdown = 0; } // Make sure countdown isn't less than 0

							// Show help 
							if( _showHelpfulNotes ){

								DoHelpBox( "If an Event doesn't trigger after " + o.collider2DCountdown.ToString() + " seconds of being spawned, this GameObject will be despawned.");
							
							}
						}
					}


					// ------------------------
					//	AFTER TRIGGER 2D EVENT
					// ------------------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterPhysicsTrigger2DEvent ){

						// Title - EVENTS TO USE
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Options", "Choose which Events will despawn this GameObject. You can optionally reset the velocity on Rigidbodies when they are spawned as well as using a countdown in addition to Physics Events." );
						EditorGUILayout.Space();
						EditorGUILayout.Space();
					
						// Trigger Events
						o.useTriggerEnter2D = HTGUI_UNDO.ToggleField( o, "Use On Trigger Enter 2D", new GUIContent( "Use On Trigger Enter 2D:", rigidbodyIcon, _empty ), o.useTriggerEnter2D, defaultGUILayoutOptions  );
						o.useTriggerStay2D = HTGUI_UNDO.ToggleField( o, "Use On Trigger Stay 2D", new GUIContent( "Use On Trigger Stay 2D:", rigidbodyIcon, _empty ), o.useTriggerStay2D, defaultGUILayoutOptions  );
						o.useTriggerExit2D = HTGUI_UNDO.ToggleField( o, "Use On Trigger Exit 2D", new GUIContent( "Use On Trigger Exit 2D:", rigidbodyIcon, _empty ), o.useTriggerExit2D, defaultGUILayoutOptions  );

						// Reset Rigidbodies
						o.resetRigidbodyVelocitiesOnSpawn = HTGUI_UNDO.ToggleField( o, "Reset Velocities On Spawn", new GUIContent( "Reset Velocities On Spawn:", loopLabel, _empty ), o.resetRigidbodyVelocitiesOnSpawn, defaultGUILayoutOptions  );

						// Also use countdown
						o.trigger2DAlsoDespawnAfterCountdown = HTGUI_UNDO.ToggleField( o, "Use Countdown", new GUIContent( "Use Countdown:", gearLabel, _empty ), o.trigger2DAlsoDespawnAfterCountdown, defaultGUILayoutOptions  );
						
						// If we're also using a countdown, show the timer
						if( o.trigger2DAlsoDespawnAfterCountdown){
							o.trigger2DCountdown = HTGUI_UNDO.FloatField( o, "Despawn Countdown", new GUIContent( "Despawn Countdown:", timeLabel, _empty ), o.trigger2DCountdown, defaultGUILayoutOptions  );
							if( o.trigger2DCountdown < 0 ){ o.trigger2DCountdown = 0; } // Make sure countdown isn't less than 0

							// Show help 
							if( _showHelpfulNotes ){

								DoHelpBox( "If an Event doesn't trigger after " + o.trigger2DCountdown.ToString() + " seconds of being spawned, this GameObject will be despawned.");
							
							}
						}
					}

					// --------------------------
					//	COLLISION FILTERING
					// --------------------------

					// Show Source of Events when using collisions ...
					if( o.despawnMode == Despawner.DespawnMode.AfterPhysicsOverlapEvent ||
						o.despawnMode == Despawner.DespawnMode.AfterPhysicsCollisionEvent ||
						o.despawnMode == Despawner.DespawnMode.AfterPhysicsTriggerEvent ||
						o.despawnMode == Despawner.DespawnMode.AfterPhysicsCollision2DEvent ||
						o.despawnMode == Despawner.DespawnMode.AfterPhysicsTrigger2DEvent ||
						o.despawnMode == Despawner.DespawnMode.AfterPhysicsRaycastEvent ||
						o.despawnMode == Despawner.DespawnMode.AfterPhysicsRaycast2DEvent 
					){

						// Title
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Filter Layers", "Setup a Layer Mask to limit collisions to specific layers. Only objects that match the layermask below can despawn this object.");
						EditorGUILayout.Space();
						EditorGUILayout.Space();

						// Filter Layers
						o.filterLayers = HTGUI_UNDO.MaskField( o, "Filter These Layers",  new GUIContent("Filter These Layers", layersLabel, string.Empty), o.filterLayers, defaultGUILayoutOptions, null, 20 );

						// Title
						EditorGUILayout.Space();
						HTGUI.SepLine();
						HTGUI.SectionTitle( "Filter Tags", "You can filter collisions with GameObjects by using Tags. Only the tags listed below can despawn this Object.");
						EditorGUILayout.Space();
						//EditorGUILayout.Space();

						// Show help 
						if( _showHelpfulNotes ){

							if( o.filterTags.Length == 0 ){
								DoHelpBox( "GameObject Tags will be ignored because you have not setup any to filter.");
							}

							// Add space underneith
							EditorGUILayout.Space();
						
						// Otherwise show some extra space
						} else {
							EditorGUILayout.Space();
						}

						// Loop through each of the block Tags			
						if( o.filterTags != null ){
							for( int i = 0; i<o.filterTags.Length; i++ ){
								if(o.filterTags[i]!=null){ 

									// Trigger With Input Get Button
									o.filterTags[i] = HTGUI_UNDO.TextField( o, "Allow GameObject Tag " + (i+1).ToString(),  new GUIContent("Allow GameObject Tag " + (i+1).ToString(), buttonIcon, string.Empty), o.filterTags[i], defaultGUILayoutOptions );

								}
							}
						}

						// Add space underneith
						EditorGUILayout.Space();

						// Add / Remove Conditions (Create Horizontal Row)
						EditorGUILayout.BeginHorizontal();

							// Flexible Space
							GUILayout.FlexibleSpace();	
												
							// Remove Button			
							if( o.filterTags.Length == 0 ){ GUI.enabled = false; }			
							if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.removeButton, "Remove Last Filter Tag"), GUILayout.MaxWidth(32)) ) { 
								if( o.filterTags.Length > 0 ){	// <- We must always have at least 1 condition!
									Undo.RecordObject ( o, "Remove Last Filter Tag");
									System.Array.Resize(ref o.filterTags, o.filterTags.Length - 1 );
								}
							}

							// Reset GUI Enabled
							GUI.enabled = true;
											
							// Add Button							
							if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.addButton, "Add New Filter Tag"), GUILayout.MaxWidth(32))) { 
								Undo.RecordObject ( o, "Add New Filter Tag");
								System.Array.Resize(ref o.filterTags, o.filterTags.Length + 1 ); 
								o.filterTags[ o.filterTags.Length - 1 ] = string.Empty;	// <- We need to set an empty string otherwise it will be null.
							}

						// End Horizontal Row
						EditorGUILayout.EndHorizontal();

						// Add space underneith
						EditorGUILayout.Space();

					}

					// --------------------------
					//	AFTER CALLED BY SCRIPT
					// --------------------------

					// Spawning Will Begin ...
					if( o.despawnMode == Despawner.DespawnMode.AfterCalledByScript ){

						// Show help 
						if( _showHelpfulNotes ){

							DoHelpBox( "You can use the API to despawn this GameObject and still take advantage of chain spawning like this:\n\nDespawner despawner = gameObject.GetComponent<Despawner>();\nif( despawner != null ){ despawner.Despawn(); }");
						
						}

					}


				// Cleanly ends the Icon Column
				EndIconColumn();

			// End White Box
			HTGUI.EndWhiteBox();
		}


		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO CHAIN SPAWNING SETUP
		//	Do the Chain Spawning Setup
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		Despawner.ChainableSpawn _chainableSpawn = null;
		string _chainableSpawnHeader = string.Empty;
		GUIContent _guiContentHeader = new GUIContent();
		const string _boldLabel = "BoldLabel";

		const string _noPrefabSetupYet = "No Prefab Has Been Assigned Yet!";
		const string _chainableSpawnHeaderStartDark = "  <color=#7f7f7fff>Prefab ";	// <- Light grey
		const string _chainableSpawnHeaderStartLight = "  <color=#4c4c4cff>Prefab ";	// <- Dark grey
			string ChainableSpawnHeaderStart(){
				if( EditorGUIUtility.isProSkin == true ){ return _chainableSpawnHeaderStartDark; }
				return _chainableSpawnHeaderStartLight;
			}
		const string _chainableSpawnHeaderSeperator = ":   </color>";

		// Method
		void DoChainSpawningSetup(){

			// =================================
			//	TAB TITLE
			// =================================

			// Show Tab Headers
			if( _showTabHeaders ){

				// Start White Box
				HTGUI.StartWhiteBox();

					// Title
					EditorGUILayout.Space();
					HTGUI.Header( spawnerIcon, "Chain Spawning", "This tab allows you to setup how other prefabs will be spawned when this GameObject is despawned.", System.String.Empty, _v2_100_64, 13
					);

				// End White Box
				HTGUI.EndWhiteBox();
			}

			// =================================
			//	MAIN TAB CONTENT
			// =================================


			// Make sure the chainable spawns array is valid
			if( o.chainableSpawns == null ){ o.chainableSpawns = new Despawner.ChainableSpawn[0]; }

			// Loop through the array
			for( int i = 0; i < o.chainableSpawns.Length; i++ ){


				// Make sure Pool Item is valid
				_chainableSpawn = o.chainableSpawns[i];
				if(_chainableSpawn!=null){
					
					// =================================
					//	HEADER
					// =================================

					if( _chainableSpawn.prefab == null){
						if( EditorGUIUtility.isProSkin == false ){ 
							GUI.backgroundColor = new Color( 1f, 0.25f, 0.25f, 0.075f);
						} else {
							GUI.backgroundColor = new Color( 1f, 0f, 0f, 1f);
						}
						
					}

					// Start the white box
					HTGUI.StartWhiteBox();

						// Reset Background color
						GUI.backgroundColor = Color.white;

						// Event Counter And Label 
						EditorGUILayout.BeginHorizontal();

							// ---------------
							// FOLDOUT BUTTON
							// ---------------

							// Create a new style for the Event Header / Note
							GUIStyle foldoutGUIStyle = new GUIStyle( GUI.skin.label );
							foldoutGUIStyle.fontStyle = FontStyle.Bold;
							foldoutGUIStyle.alignment = TextAnchor.MiddleLeft;
							foldoutGUIStyle.margin = new RectOffset(0,0,0,0);
							foldoutGUIStyle.padding = new RectOffset(0,0,0,0);

							// Setup the correct spacing to show the button / foldout
							EditorGUILayout.BeginVertical( GUILayout.MinWidth(16), GUILayout.MaxWidth(16), GUILayout.MinHeight(32), GUILayout.MaxHeight(32) );
								GUILayout.FlexibleSpace();

								// Foldout icons are missing in unity 2019.3+
								#if UNITY_2019_3_OR_NEWER

									// Push up the foldout icon in newer unity versions
									foldoutGUIStyle.padding = new RectOffset(0,0,0,2);

									// Create a button that mimics the foldout
									if( GUILayout.Button( 

										// Get Unity's default foldout icons to display here with a tooltip!
										new GUIContent( string.Empty,
														(!o.chainableSpawns[i].tabIsOpen ? foldoutRight : foldoutDown ),
														"Click this button to minimize and expand this prefab entry." ),

										// Here we set the style and layout
										foldoutGUIStyle, GUILayout.MaxWidth(22), GUILayout.MinHeight(14), GUILayout.MaxHeight(14)) 
									){ 
										o.chainableSpawns[i].tabIsOpen = !o.chainableSpawns[i].tabIsOpen;
									}

								// Original version
								#else

									// Create a button that mimics the foldout
									if( GUILayout.Button( 

										// Get Unity's default foldout icons to display here with a tooltip!
										new GUIContent( string.Empty,
														(!o.chainableSpawns[i].tabIsOpen ? EditorStyles.foldout.active.background : EditorStyles.foldout.onActive.background),
														"Click this button to minimize and expand this prefab entry." ),

										// Here we set the style and layout
										foldoutGUIStyle, GUILayout.MaxWidth(24), GUILayout.MinHeight(16), GUILayout.MaxHeight(16)) 
									){ 
										o.chainableSpawns[i].tabIsOpen = !o.chainableSpawns[i].tabIsOpen;
									}

								#endif

								GUILayout.FlexibleSpace();
							EditorGUILayout.EndVertical();

							// ->

							// Setup a larger bold font to use
							GUIStyle boldEventTitleGUIStyle = new GUIStyle(_boldLabel);
							boldEventTitleGUIStyle.fontSize = 12;
							boldEventTitleGUIStyle.richText = true;

							// Precache the title using the logic comment
							if( _chainableSpawn.prefab != null ){
								_chainableSpawnHeader =_chainableSpawn.prefab.name;
								if(_chainableSpawnHeader.Length > 64 ){ _chainableSpawnHeader = _chainableSpawnHeader.Substring(0,64) + " ..."; }
							} else {
								_chainableSpawnHeader = _noPrefabSetupYet;
							}

							// Draw the icon and label using a GUIContent
							_guiContentHeader.text = ChainableSpawnHeaderStart() + (i+1).ToString() + _chainableSpawnHeaderSeperator + _chainableSpawnHeader;
							_guiContentHeader.image = prefabSmallIcon;
							GUILayout.Label( _guiContentHeader, boldEventTitleGUIStyle, GUILayout.MaxHeight(24));

							// Space
							GUILayout.Label(string.Empty, GUILayout.MaxWidth(5), GUILayout.MaxHeight(5) );

							// Only allow the prefab items to be moved when the game isn't running
							if( Application.isPlaying == false ){

								// Move Slot Up
								if( o.chainableSpawns.Length > 0 && i != 0 &&
									GUILayout.Button( new GUIContent( System.String.Empty, upButton, "Move Spawnable Item Up"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
								){
									Undo.RecordObject ( o, "Move Spawnable Item Up" );
									Arrays.Shift( ref o.chainableSpawns, i, true );
								}

								// Move Slot Down
								if( o.chainableSpawns.Length > 0 && i != o.chainableSpawns.Length-1 &&
									GUILayout.Button( new GUIContent( System.String.Empty, downButton, "Move Spawnable Item Down"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
								){
									Undo.RecordObject ( o, "Move Spawnable Item Down" );
									Arrays.Shift( ref o.chainableSpawns, i, false );
								}

								// Destroy Spawnable Item (we must ensure at least 1 spawnable item exists)
								if( o.chainableSpawns.Length > 1 &&
									GUILayout.Button( new GUIContent( System.String.Empty, HTGUI.removeButton, "Remove Spawnable Item" ), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
								){
									Undo.RecordObject ( o, "Remove Spawnable Item" );
									Arrays.RemoveItemAtIndex( ref o.chainableSpawns, i );
								}

							}
							
						EditorGUILayout.EndHorizontal();

						// BUGFIX for console error when deleting a chainable item from the header bar
						// Make sure we're still in range for the Array.
						if( i < o.chainableSpawns.Length && o.chainableSpawns[i].tabIsOpen ){

							// =================================
							//	MAIN BOX
							// =================================

							// Add Space and seperator
							EditorGUILayout.Space();
							HTGUI.SepLine();
							GUILayout.Label( "Prefab To Spawn", _boldLabel );

							// Extra space in newer versions of Unity
							#if UNITY_2019_3_OR_NEWER
								EditorGUILayout.Space();
							#endif

							// Prefab
							o.chainableSpawns[i].prefab = (GameObject) HTGUI_UNDO.ObjectField( o, "Prefab",  new GUIContent( "Prefab:", cubeLabel, _empty ), o.chainableSpawns[i].prefab, false, defaultGUILayoutOptions );

							// This prefab is using the same as the GameObject (unfortunately this only shows up if the
							// player clicks on the prefab in the Project Pane)
							if( o.chainableSpawns[i].prefab != null && o.chainableSpawns[i].prefab == o.gameObject ){
								DoHelpBox( "IMPORTANT! You should never use the same prefab to chain spawn! This could cause an infinate loop and crash your game!", MessageType.Error );
							}

							// Make sure we have a prefab set before showing the other options
							if( o.chainableSpawns[i].prefab != null ){
							
								// Spawn Condition
								o.chainableSpawns[i].spawnOptions = (Despawner.SpawnOptions) HTGUI_UNDO.EnumField( o, "Spawn Options",  new GUIContent( "Spawn Options:", gearLabel, _empty ), o.chainableSpawns[i].spawnOptions, defaultGUILayoutOptions );

								// Allow users to setup conditional filters on physics events
								if( o.chainableSpawns[i].spawnOptions == Despawner.SpawnOptions.SpawnOnlyOnPhysicsEvent ){

									o.chainableSpawns[i].usePhysicsEventFilters = HTGUI_UNDO.ToggleField( o, "Use Physics Event Filters",  new GUIContent( "Use Physics Event Filters:", gearLabel, _empty ), o.chainableSpawns[i].usePhysicsEventFilters, defaultGUILayoutOptions );
								}

								// Show Helpful Notes
								if( _showHelpfulNotes ){

									// -------------------
									//	Always Spawn
									// ------------------

									if( o.chainableSpawns[i].spawnOptions == Despawner.SpawnOptions.AlwaysSpawn ){
										DoHelpBox( "This prefab will always be Chain-Spawned." );

									}

									// -----------------------------
									//	Spawn Only On Physics Event
									// -----------------------------

									else if( o.chainableSpawns[i].spawnOptions == Despawner.SpawnOptions.SpawnOnlyOnPhysicsEvent ){

										if( o.chainableSpawns[i].usePhysicsEventFilters == true ){
											DoHelpBox( "This prefab will only be Chain-Spawned if it was triggered from one of the Physics Events and passes the filter checks below." );
										} else {
											DoHelpBox( "This prefab will only be Chain-Spawned if it was triggered from one of the Physics Events (Collision, Trigger, Collision2D, Trigger2D, etc.)." );
										}
									}

									// -------------------------------
									//	Spawn Except On Physics Event
									// -------------------------------

									else if( o.chainableSpawns[i].spawnOptions == Despawner.SpawnOptions.SpawnExceptOnPhysicsEvent ){
										DoHelpBox( "This prefab will only be Chain-Spawned if it was NOT triggered by a Physics Event." );

									}

									// -------------------
									//	Never Spawn
									// ------------------

									if( o.chainableSpawns[i].spawnOptions == Despawner.SpawnOptions.NeverSpawn ){
										DoHelpBox( "This prefab will never be Chain-Spawned. This setting is useful when you want to temporarily disable an item from the list to test in the Editor. Note that you should remove this item from finished builds to optimize your game." );

									}
								}

								
								// -------------------------------
								//	CHAIN SPAWN PHYSICS FILTERING
								// -------------------------------

								// If we're using physics events allow us to use physics filtering if enabled ...
								if( o.chainableSpawns[i].spawnOptions == Despawner.SpawnOptions.SpawnOnlyOnPhysicsEvent &&
									o.chainableSpawns[i].usePhysicsEventFilters
								){


									// ---------------
									//	FILTER LAYERS
									// ---------------

									// Title
									EditorGUILayout.Space();
									HTGUI.SepLine();
								//	HTGUI.SectionTitle( "Filter Layers", "Setup a Layer Mask to limit chain spawning to specific layers. Only objects that match the layermask below can chain spawn this prefab.");
								//	EditorGUILayout.Space();
								//	EditorGUILayout.Space();

									// Filter Layers
									GUILayout.Label( "Filter Layers", _boldLabel );

									// Extra space in newer versions of Unity
									#if UNITY_2019_3_OR_NEWER
										EditorGUILayout.Space();
									#endif

									// Filter Layers
									o.chainableSpawns[i].filterLayers = HTGUI_UNDO.MaskField( o, "Filter These Layers",  new GUIContent("Filter These Layers", layersLabel, string.Empty), o.chainableSpawns[i].filterLayers, defaultGUILayoutOptions, null, 20 );

									if(_showHelpfulNotes){ DoHelpBox( "Only objects that match the layermask above can chain spawn this prefab.");}


									// ---------------
									//	FILTER TAGS
									// ---------------

									// Title
									EditorGUILayout.Space();
									HTGUI.SepLine();
									//HTGUI.SectionTitle( "Filter Tags", "You can filter chain spawning with GameObjects by using Tags. Only objects with the tags listed below can chain spawn this prefab.");
									//EditorGUILayout.Space();
									//EditorGUILayout.Space();

									// Filter Layers
									GUILayout.Label( "Filter Tags", _boldLabel );

									// Extra space in newer versions of Unity
									#if UNITY_2019_3_OR_NEWER
										EditorGUILayout.Space();
									#endif


									// Show help 
									if( _showHelpfulNotes ){

										if( o.chainableSpawns[i].filterTags.Length == 0 ){
											DoHelpBox( "GameObject Tags will be ignored because you have not setup any to filter.");
										}

										// Add space underneith
										//EditorGUILayout.Space();
									
									// Otherwise show some extra space
									} else {
										//EditorGUILayout.Space();
									}

									// Loop through each of the block Tags			
									if( o.chainableSpawns[i].filterTags != null ){
										for( int t = 0; t < o.chainableSpawns[i].filterTags.Length; t++ ){
											if(o.chainableSpawns[i].filterTags[t]!=null){ 

												// Trigger With Input Get Button
												o.chainableSpawns[i].filterTags[t] = HTGUI_UNDO.TextField( o, "Allow GameObject Tag " + (t+1).ToString(),  new GUIContent("Allow GameObject Tag " + (t+1).ToString(), buttonIcon, string.Empty), o.chainableSpawns[i].filterTags[t], defaultGUILayoutOptions );

											}
										}
									}

									// Add space underneith
									EditorGUILayout.Space();

									// Add / Remove Conditions (Create Horizontal Row)
									EditorGUILayout.BeginHorizontal();

										// Flexible Space
										GUILayout.FlexibleSpace();	
															
										// Remove Button			
										if( o.chainableSpawns[i].filterTags.Length == 0 ){ GUI.enabled = false; }			
										if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.removeButton, "Remove Last Filter Tag"), GUILayout.MaxWidth(32)) ) { 
											if( o.chainableSpawns[i].filterTags.Length > 0 ){	// <- We must always have at least 1 condition!
												Undo.RecordObject ( o, "Remove Last Filter Tag");
												System.Array.Resize(ref o.chainableSpawns[i].filterTags, o.chainableSpawns[i].filterTags.Length - 1 );
												GUIUtility.ExitGUI();
											}
										}

										// Reset GUI Enabled
										GUI.enabled = true;
														
										// Add Button							
										if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.addButton, "Add New Filter Tag"), GUILayout.MaxWidth(32))) { 
											Undo.RecordObject ( o, "Add New Filter Tag");
											System.Array.Resize(ref o.chainableSpawns[i].filterTags, o.chainableSpawns[i].filterTags.Length + 1 ); 
											o.chainableSpawns[i].filterTags[ o.chainableSpawns[i].filterTags.Length - 1 ] = string.Empty;	// <- We need to set an empty string otherwise it will be null.
											GUIUtility.ExitGUI();
										}

									// End Horizontal Row
									EditorGUILayout.EndHorizontal();

									// Add space underneith
									EditorGUILayout.Space();

									// Show helpful note if we've setup some filter tags
									if( _showHelpfulNotes && o.chainableSpawns[i].filterTags.Length > 0 ){
										DoHelpBox( "Only objects with the tags listed above can chain spawn this prefab.");
									}



									// ---------------
									//	FILTER NAMES
									// ---------------

									// Title
									EditorGUILayout.Space();
									HTGUI.SepLine();
									//HTGUI.SectionTitle( "Filter Names", "You can filter chain spawning with GameObjects by name. Only objects with the names listed below can chain spawn this prefab.");
									//EditorGUILayout.Space();
									//EditorGUILayout.Space();

									// Filter Layers
									GUILayout.Label( "Filter Names", _boldLabel );

									// Extra space in newer versions of Unity
									#if UNITY_2019_3_OR_NEWER
										EditorGUILayout.Space();
									#endif


									// Show help 
									if( _showHelpfulNotes ){

										if( o.chainableSpawns[i].filterNames.Length == 0 ){
											DoHelpBox( "GameObject Names will be ignored because you have not setup any to filter.");
										}

										// Add space underneith
									//	EditorGUILayout.Space();
									
									// Otherwise show some extra space
									} else {
									//	EditorGUILayout.Space();
									}

									// Loop through each of the block Tags			
									if( o.chainableSpawns[i].filterNames != null ){
										for( int t = 0; t < o.chainableSpawns[i].filterNames.Length; t++ ){
											if(o.chainableSpawns[i].filterNames[t]!=null){ 

												// Trigger With Input Get Button
												o.chainableSpawns[i].filterNames[t] = HTGUI_UNDO.TextField( o, "Allow GameObject Name " + (t+1).ToString(),  new GUIContent("Allow GameObject Name " + (t+1).ToString(), buttonIcon, string.Empty), o.chainableSpawns[i].filterNames[t], defaultGUILayoutOptions );

											}
										}
									}

									// Add space underneith
									EditorGUILayout.Space();

									// Add / Remove Conditions (Create Horizontal Row)
									EditorGUILayout.BeginHorizontal();

										// Flexible Space
										GUILayout.FlexibleSpace();	
															
										// Remove Button			
										if( o.chainableSpawns[i].filterNames.Length == 0 ){ GUI.enabled = false; }			
										if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.removeButton, "Remove Last Filter Name"), GUILayout.MaxWidth(32)) ) { 
											if( o.chainableSpawns[i].filterNames.Length > 0 ){	// <- We must always have at least 1 condition!
												Undo.RecordObject ( o, "Remove Last Filter Name");
												System.Array.Resize(ref o.chainableSpawns[i].filterNames, o.chainableSpawns[i].filterNames.Length - 1 );
												GUIUtility.ExitGUI();
											}
										}

										// Reset GUI Enabled
										GUI.enabled = true;
														
										// Add Button							
										if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.addButton, "Add New Filter Name"), GUILayout.MaxWidth(32))) { 
											Undo.RecordObject ( o, "Add New Filter Name");
											System.Array.Resize(ref o.chainableSpawns[i].filterNames, o.chainableSpawns[i].filterNames.Length + 1 ); 
											o.chainableSpawns[i].filterNames[ o.chainableSpawns[i].filterNames.Length - 1 ] = string.Empty;	// <- We need to set an empty string otherwise it will be null.
											GUIUtility.ExitGUI();
										}

									// End Horizontal Row
									EditorGUILayout.EndHorizontal();

									// Add space underneith
									EditorGUILayout.Space();

									// Show helpful note if we've setup some filter tags
									if( _showHelpfulNotes && o.chainableSpawns[i].filterNames.Length > 0 ){
										DoHelpBox( "Only objects with the exact name listed above can chain spawn this prefab.");
									}


								}


								// =================================
								//	SPAWN POSITION
								// =================================

								EditorGUILayout.Space();
								HTGUI.SepLine();
								GUILayout.Label( "Position", _boldLabel );

								// Extra space in newer versions of Unity
								#if UNITY_2019_3_OR_NEWER
									EditorGUILayout.Space();
								#endif

								// Spawn Position
								o.chainableSpawns[i].spawnAt = (Despawner.ChainSpawnLocation) HTGUI_UNDO.EnumField( o, "Spawn Position At",  new GUIContent( "Spawn Position At:", origin3dLabel, _empty ), o.chainableSpawns[i].spawnAt, defaultGUILayoutOptions );

								// If we're using a Custom Child Transform, set it
								if( o.chainableSpawns[i].spawnAt == Despawner.ChainSpawnLocation.AnotherChildTransform ){
									o.chainableSpawns[i].customSpawnTransform = (Transform) HTGUI_UNDO.ObjectField( o, "Another Child Transform",  new GUIContent( "Another Child Transform:", origin3dLabel, _empty ), o.chainableSpawns[i].customSpawnTransform, true, defaultGUILayoutOptions );
									

									// Show warning message - no spawn transform set!
									if( o.chainableSpawns[i].customSpawnTransform == null ){
										DoHelpBox( "The Child Transform is NOT set! This will default back to using the Spawner's position!", MessageType.Warning );
									}

									// Show warning message - the spawn transform is NOT a child object!
									else if( o.chainableSpawns[i].customSpawnTransform != null &&
										IsChildObject( o.chainableSpawns[i].customSpawnTransform ) == false
									){
										DoHelpBox( "The Transform is NOT a child of this GameObject! This will default back to using the Spawner's position!", MessageType.Warning );
									}
								}

								// Local Position Offset
								o.chainableSpawns[i].localPositionOffset = HTGUI_UNDO.Vector3Field( o, "Local Position Offset",  new GUIContent( "Local Position Offset:", origin3dLabel, _empty ), o.chainableSpawns[i].localPositionOffset, defaultGUILayoutOptions );

								// Add Randomization Range
								EditorGUILayout.Space();
								o.chainableSpawns[i].addRandomizationRange = HTGUI_UNDO.ToggleField( o, "Apply Randomized Offset",  new GUIContent( "Apply Randomized Offset:", gearLabel, _empty ), o.chainableSpawns[i].addRandomizationRange, defaultGUILayoutOptions );

								// Show randomization range
								if( o.chainableSpawns[i].addRandomizationRange ){

									// Randomization Range Min
									o.chainableSpawns[i].randomizationRangeMin = HTGUI_UNDO.Vector3Field( o, "Minimum Random Range",  new GUIContent( "Minimum Random Range:", origin3dLabel, _empty ), o.chainableSpawns[i].randomizationRangeMin, defaultGUILayoutOptions );

									// Randomization Range Max
									o.chainableSpawns[i].randomizationRangeMax = HTGUI_UNDO.Vector3Field( o, "Maximum Random Range",  new GUIContent( "Maximum Random Range:", origin3dLabel, _empty ), o.chainableSpawns[i].randomizationRangeMax, defaultGUILayoutOptions );

									// Make sure the maximum ranges are not less than the minimum ranges
									if( o.chainableSpawns[i].randomizationRangeMax.x < o.chainableSpawns[i].randomizationRangeMin.x ){
										o.chainableSpawns[i].randomizationRangeMax.x = o.chainableSpawns[i].randomizationRangeMin.x;
									}
									if( o.chainableSpawns[i].randomizationRangeMax.y < o.chainableSpawns[i].randomizationRangeMin.y ){
										o.chainableSpawns[i].randomizationRangeMax.y = o.chainableSpawns[i].randomizationRangeMin.y;
									}
									if( o.chainableSpawns[i].randomizationRangeMax.z < o.chainableSpawns[i].randomizationRangeMin.z ){
										o.chainableSpawns[i].randomizationRangeMax.z = o.chainableSpawns[i].randomizationRangeMin.z;
									}

								}

								// Show Helpful Notes
								if( _showHelpfulNotes ){

									// -------------------
									// Position Spawn At
									// ------------------

									if( o.chainableSpawns[i].spawnAt == Despawner.ChainSpawnLocation.ThisTransform ){
										_helpBoxString = "An instance of the prefab '" + o.chainableSpawns[i].prefab.name + "' will be spawned at this Transform's position. ";

									} else if( o.chainableSpawns[i].spawnAt == Despawner.ChainSpawnLocation.AnotherChildTransform ){

										// If there is an issue with the custom Spawn Transform, we'll use the Transform Spawner
										if( o.chainableSpawns[i].customSpawnTransform == null || IsChildObject( o.chainableSpawns[i].customSpawnTransform ) == false ){
											_helpBoxString = "An instance of the prefab '" + o.chainableSpawns[i].prefab.name + "' will be spawned at this Transform's position as there is a problem with the configuration above. ";
										} else {
											_helpBoxString = "An instance of the prefab '" + o.chainableSpawns[i].prefab.name + "' will be spawned using the position of '" + o.chainableSpawns[i].customSpawnTransform.name + "'. ";
										}

									} else if( o.chainableSpawns[i].spawnAt == Despawner.ChainSpawnLocation.LastCollision ){
										_helpBoxString = "The prefab '" + o.chainableSpawns[i].prefab.name + "' will be spawned using the position of the last collision point. If that cannot be determined, this Transform's position will be used instead. ";
									}
									
									// -----------------------
									// Local Position Offset
									// -----------------------

									if( o.chainableSpawns[i].localPositionOffset != Vector3.zero ){
										_helpBoxString += "Using that position, a local offset of " + o.chainableSpawns[i].localPositionOffset.ToString() + " will be applied. ";
									}

									// -----------------------
									// Randomization Offset
									// -----------------------

									if( o.chainableSpawns[i].addRandomizationRange ){
										_helpBoxString += "Finally, a randomized offset between " + o.chainableSpawns[i].randomizationRangeMin.ToString() + " and " + o.chainableSpawns[i].randomizationRangeMax.ToString() + " will be added to get the final result.";
									}

									DoHelpBox( _helpBoxString );
								}

								// =================================
								//	SPAWN ROTATION
								// =================================

								EditorGUILayout.Space();
								HTGUI.SepLine();
								GUILayout.Label( "Rotation", _boldLabel );

								// Extra space in newer versions of Unity
								#if UNITY_2019_3_OR_NEWER
									EditorGUILayout.Space();
								#endif

								// Spawn Rotation
								o.chainableSpawns[i].rotationMode = (Despawner.ChainableSpawn.RotationMode) HTGUI_UNDO.EnumField( o, "Rotate Instance Using",  new GUIContent( "Rotate Instance Using:", loopLabel, _empty ), o.chainableSpawns[i].rotationMode, defaultGUILayoutOptions );

								// If we're using Custom Euler Angles, show it here
								if( o.chainableSpawns[i].rotationMode == Despawner.ChainableSpawn.RotationMode.CustomEulerAngles ){
									o.chainableSpawns[i].customRotationEulerAngles = HTGUI_UNDO.Vector3Field( o, "Custom Euler Angles",  new GUIContent( "Custom Euler Angles:", loopLabel, _empty ), o.chainableSpawns[i].customRotationEulerAngles, defaultGUILayoutOptions );
								}

								// If we're using Custom Euler Angles, show it here
								if( o.chainableSpawns[i].rotationMode == Despawner.ChainableSpawn.RotationMode.PrefabDefault || 
									o.chainableSpawns[i].rotationMode == Despawner.ChainableSpawn.RotationMode.ThisTransformRotation
								){
									o.chainableSpawns[i].customRotationEulerAngles = HTGUI_UNDO.Vector3Field( o, "Offset Rotation",  new GUIContent( "Offset Rotation:", loopLabel, _empty ), o.chainableSpawns[i].customRotationEulerAngles, defaultGUILayoutOptions );
								}

								// -----------------------
								//	Rotation Notes
								// -----------------------

								// Show Helpful Notes
								if( _showHelpfulNotes ){

									if( o.chainableSpawns[i].rotationMode == Despawner.ChainableSpawn.RotationMode.PrefabDefault ){
										DoHelpBox( "The instance will be rotated using the prefab's default settings." + (o.chainableSpawns[i].customRotationEulerAngles != Vector3.zero ? " A rotational offset of " + o.chainableSpawns[i].customRotationEulerAngles.ToString() + " will then be applied." : string.Empty ) );
									
									} else if( o.chainableSpawns[i].rotationMode == Despawner.ChainableSpawn.RotationMode.ThisTransformRotation ){
										DoHelpBox( "The instance will be rotated using the current rotation of this Transform." + (o.chainableSpawns[i].customRotationEulerAngles != Vector3.zero ? " A rotational offset of " + o.chainableSpawns[i].customRotationEulerAngles.ToString() + " will then be applied." : string.Empty ) );
									
									} else if( o.chainableSpawns[i].rotationMode == Despawner.ChainableSpawn.RotationMode.CustomEulerAngles ){
										DoHelpBox( "The instance will be rotated using the custom Euler Angles: " + o.chainableSpawns[i].customRotationEulerAngles.ToString()  );
									
									} else if( o.chainableSpawns[i].rotationMode == Despawner.ChainableSpawn.RotationMode.RandomRotation ){
										DoHelpBox( "The instance will be rotated randomly." );
									}
								}

								// =================================
								//	SPAWN SCALE
								// =================================

								EditorGUILayout.Space();
								HTGUI.SepLine();
								GUILayout.Label( "Scale", _boldLabel );

								// Extra space in newer versions of Unity
								#if UNITY_2019_3_OR_NEWER
									EditorGUILayout.Space();
								#endif

								// Spawn Rotation
								o.chainableSpawns[i].scaleMode = (Despawner.ChainableSpawn.ScaleMode) HTGUI_UNDO.EnumField( o, "Scale Instance Using",  new GUIContent( "Scale Instance Using:", resizeLabel, _empty ), o.chainableSpawns[i].scaleMode, defaultGUILayoutOptions );

								// Multiply With Local Scale
								if(	o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.CustomLocalScale ||
									o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.RandomRangeScale ||
									o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.RandomRangeProportionalScale
								){
									o.chainableSpawns[i].customScaleOptions = (Despawner.CustomScaleOptions) HTGUI_UNDO.EnumField( o, "Process Scale Options",  new GUIContent( "Process Scale Options:", resizeLabel, _empty ), o.chainableSpawns[i].customScaleOptions, defaultGUILayoutOptions );
								}

								// If we're using Custom Scale, show it here
								if( o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.CustomLocalScale ){
									o.chainableSpawns[i].customLocalScale = HTGUI_UNDO.Vector3Field( o, "Custom Local Scale",  new GUIContent( "Custom Local Scale:", resizeLabel, _empty ), o.chainableSpawns[i].customLocalScale, defaultGUILayoutOptions );
								}

								// If we're using Random Range Scale, show it here
								if( o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.RandomRangeScale ){
									o.chainableSpawns[i].customLocalScaleMin = HTGUI_UNDO.Vector3Field( o, "Minimum Local Scale",  new GUIContent( "Minimum Local Scale:", resizeLabel, _empty ), o.chainableSpawns[i].customLocalScaleMin, defaultGUILayoutOptions );

									o.chainableSpawns[i].customLocalScaleMax = HTGUI_UNDO.Vector3Field( o, "Maximum Local Scale",  new GUIContent( "Maximum Local Scale:", resizeLabel, _empty ), o.chainableSpawns[i].customLocalScaleMax, defaultGUILayoutOptions );
								}

								// If we're using a Proportional Random Range Scale, show it here
								if( o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.RandomRangeProportionalScale ){
									o.chainableSpawns[i].customLocalScaleProportionalMin = HTGUI_UNDO.FloatField( o, "Minimum Local Scale",  new GUIContent( "Minimum Local Scale:", resizeLabel, _empty ), o.chainableSpawns[i].customLocalScaleProportionalMin, defaultGUILayoutOptions );

									o.chainableSpawns[i].customLocalScaleProportionalMax = HTGUI_UNDO.FloatField( o, "Maximum Local Scale",  new GUIContent( "Maximum Local Scale:", resizeLabel, _empty ), o.chainableSpawns[i].customLocalScaleProportionalMax, defaultGUILayoutOptions );
								}

								// -----------------------
								//	Scale Notes
								// -----------------------

								// Show Helpful Notes
								if( _showHelpfulNotes ){

									if( o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.PrefabDefault ){
										DoHelpBox( "The instance will be scaled using the prefab's default settings." );
									
									} else if( o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.PoolDefault ){
										DoHelpBox( "The instance will not be scaled and instead use the default settings of the Pool." );
									
									} else if( o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.ThisTransformScale ){
										DoHelpBox( "The instance will be scaled by matching this Transform's local scale."  );
									
									} else if( o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.CustomLocalScale ){
										DoHelpBox( "The instance will be scaled to: " + o.chainableSpawns[i].customLocalScale.ToString() + "." + DoScaleOptionHelpText( o.chainableSpawns[i].customScaleOptions ) );
									
									} else if( o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.RandomRangeScale ){
										DoHelpBox( "The instance will be scaled randomly between: " + o.chainableSpawns[i].customLocalScaleMin.ToString() + " and " + o.chainableSpawns[i].customLocalScaleMax.ToString() + "." + DoScaleOptionHelpText( o.chainableSpawns[i].customScaleOptions )  );
									
									} else if( o.chainableSpawns[i].scaleMode == Despawner.ChainableSpawn.ScaleMode.RandomRangeProportionalScale ){
										DoHelpBox( "The instance will be scaled randomly using a proportional scale between: " + o.chainableSpawns[i].customLocalScaleProportionalMin.ToString() + " and " + o.chainableSpawns[i].customLocalScaleProportionalMax.ToString() + "." + DoScaleOptionHelpText( o.chainableSpawns[i].customScaleOptions )  );
									}
								}

								// =================================
								//	REPEAT
								// =================================

								EditorGUILayout.Space();
								HTGUI.SepLine();
								GUILayout.Label( "Repeat", _boldLabel );

								// Extra space in newer versions of Unity
								#if UNITY_2019_3_OR_NEWER
									EditorGUILayout.Space();
								#endif

								// Spawn Rotation
								o.chainableSpawns[i].timesToSpawnMode = (Despawner.ChainableSpawn.TimesToSpawnMode) HTGUI_UNDO.EnumField( o, "Times To Spawn",  new GUIContent( "Times To Spawn:", gearLabel, _empty ), o.chainableSpawns[i].timesToSpawnMode, defaultGUILayoutOptions );

								// Fixed Number
								if( o.chainableSpawns[i].timesToSpawnMode == Despawner.ChainableSpawn.TimesToSpawnMode.FixedNumber ){

									// Times To Spawn This Object
									o.chainableSpawns[i].timesToSpawnThisObject = HTGUI_UNDO.IntField( o, "Instances To Spawn",  new GUIContent( "Instances To Spawn:", xLabel, _empty ), o.chainableSpawns[i].timesToSpawnThisObject, defaultGUILayoutOptions );

									// Show Helpful Notes
									if( _showHelpfulNotes ){
										DoHelpBox( "Exactly " +  NumberFormat( o.chainableSpawns[i].timesToSpawnThisObject, "instance", "instances" ) + " will be spawned using these settings." );
									}

								// Random Range
								} else if( o.chainableSpawns[i].timesToSpawnMode == Despawner.ChainableSpawn.TimesToSpawnMode.RandomRange ){

									// Min Times To Spawn This Object
									o.chainableSpawns[i].minTimesToSpawnThisObject = HTGUI_UNDO.IntField( o, "Min Instances To Spawn",  new GUIContent( "Min Instances To Spawn:", xLabel, _empty ), o.chainableSpawns[i].minTimesToSpawnThisObject, defaultGUILayoutOptions );

									// Max Times To Spawn This Object
									o.chainableSpawns[i].maxTimesToSpawnThisObject = HTGUI_UNDO.IntField( o, "Max Instances To Spawn",  new GUIContent( "Max Instances To Spawn:", xLabel, _empty ), o.chainableSpawns[i].maxTimesToSpawnThisObject, defaultGUILayoutOptions );

									// Make sure the max interval is at least the same size as the minimum
									if( o.chainableSpawns[i].maxTimesToSpawnThisObject < o.chainableSpawns[i].minTimesToSpawnThisObject ){
										o.chainableSpawns[i].maxTimesToSpawnThisObject = o.chainableSpawns[i].minTimesToSpawnThisObject; 
									}

									// Show Helpful Notes
									if( _showHelpfulNotes ){
										DoHelpBox( "Between " + o.chainableSpawns[i].minTimesToSpawnThisObject.ToString() + " and " +  NumberFormat( o.chainableSpawns[i].maxTimesToSpawnThisObject, "instance", "instances" ) + " will be spawned using these settings." );
									}
								}

								

								

							}
						}

					// End White Box
					HTGUI.EndWhiteBox();
				}

				
			}

			// Add / Remove Pool Items (Create Horizontal Row)
			if( !Application.isPlaying ){

				// Start Horizontal Row
				EditorGUILayout.BeginHorizontal();

					// Flexible Space
					GUILayout.FlexibleSpace();	
										
					// Remove Button			
					if( o.chainableSpawns.Length == 0 ){ GUI.enabled = false; }			
					if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.removeButton, "Remove Last Spawnable Item"), GUILayout.MaxWidth(32)) ) { 
						if( o.chainableSpawns.Length > 0 ){	// <- We must always have at least 1 condition!
							Undo.RecordObject ( o, "Remove Last Spawnable Item");
							System.Array.Resize(ref o.chainableSpawns, o.chainableSpawns.Length - 1 );
						}
					}

					// Reset GUI Enabled
					GUI.enabled = true;
									
					// Add Button							
					if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.addButton, "Add New Spawnable Item"), GUILayout.MaxWidth(32))) { 
						Undo.RecordObject ( o, "Add New Spawnable Item");
						System.Array.Resize(ref o.chainableSpawns, o.chainableSpawns.Length + 1 ); 
						o.chainableSpawns[ o.chainableSpawns.Length - 1 ] = new Despawner.ChainableSpawn();
					}

				// End Horizontal Row
				EditorGUILayout.EndHorizontal();

				// Add space underneith
				EditorGUILayout.Space();
				EditorGUILayout.Space();

			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO EVENTS SETUP
		//	Do the Events Setup
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		string DoScaleOptionHelpText( Despawner.CustomScaleOptions option ){
			
			// MultiplyWithLocalScale
			if( option == Despawner.CustomScaleOptions.MultiplyWithLocalScale ){ 
				return "The result will be multiplied with this Transform's local scale."; 
			}

			// MultiplyWithSmallestLocalScaleVector
			else if( option == Despawner.CustomScaleOptions.MultiplyWithSmallestLocalScaleVector ){ 
				return "The result will be multiplied with the smallest vector of this Transform's local scale."; 
			}

			// MultiplyWithLargestLocalScaleVector
			else if( option == Despawner.CustomScaleOptions.MultiplyWithLargestLocalScaleVector ){ 
				return "The result will be multiplied with the largest vector of this Transform's local scale."; 
			}

			// MultiplyWithLocalScaleX
			else if( option == Despawner.CustomScaleOptions.MultiplyWithLocalScaleX ){ 
				return "The result will be multiplied with this Transform's local X scale."; 
			}

			// MultiplyWithLocalScaleY
			else if( option == Despawner.CustomScaleOptions.MultiplyWithLocalScaleY ){ 
				return "The result will be multiplied with this Transform's local Y scale."; 
			}

			// MultiplyWithLocalScaleZ
			else if( option == Despawner.CustomScaleOptions.MultiplyWithLocalScaleZ ){ 
				return "The result will be multiplied with this Transform's local Z scale."; 
			}

			// Otherwise, return nothing
			return string.Empty;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO EVENTS SETUP
		//	Do the Events Setup
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Method
		void DoEventsSetup(){

			// =================================
			//	TAB TITLE
			// =================================

			// Show Tab Headers
			if( _showTabHeaders ){

				// Start White Box
				HTGUI.StartWhiteBox();

					// Title
					EditorGUILayout.Space();
					HTGUI.Header( eventsIcon, "Events", "This tab allows you to optionally enable delegates and UnityEvents. Each UnityEvent can be enabled independantly to optimize performance. ", System.String.Empty, _v2_100_64, 13
					);

				// End White Box
				HTGUI.EndWhiteBox();
			}

			// =================================
			//	MAIN TAB CONTENT
			// =================================

			// Start White Box
			HTGUI.StartWhiteBox();

				// Creates an Icon on the left and sets up the content column on the right
				StartIconColumn( eventsIcon, 64, 64, 4 );

					// Title
					HTGUI.SectionTitle( "Delegates & Events", "If you want this Despawner to allow delegates and events, check the box below." );
					EditorGUILayout.Space();

					// Enable Despawner Events
					o.enableDespawnerEvents = HTGUI_UNDO.ToggleField( o, "Enable Delegates & Events", new GUIContent( "Enable Delegates & Events:", gearLabel, _empty ), o.enableDespawnerEvents, defaultGUILayoutOptions );


				// Cleanly ends the Icon Column
				EndIconColumn();

				// Show help 
				if( _showHelpfulNotes ){

					// Show information about delegates here ...
					if ( o.enableDespawnerEvents ){
						DoHelpBox( "// You can Subsribe to this Despawner's Events like this:\n\n"+ 

							"Despawner despawner = gameObject.GetComponent<Despawner>()\n"+
							"if( despawner != null ){\n\n"+
							"     despawner.onDespawnerDespawn += onDespawnerDespawn;\n"+
							"     despawner.onDespawnerCollided += onDespawnerCollided;\n"+
							"     despawner.onDespawnerChainSpawn += onDespawnerChainSpawn;\n"+
							"}"
							
						);
					
					}
				}

				
				// Add Seperator
				EditorGUILayout.Space();
				HTGUI.SepLine();
			//	EditorGUILayout.Space();

				// Title
				HTGUI.SectionTitle( "Unity Events", "If you want this Despawner to send any UnityEvents, activate them below:" );
				EditorGUILayout.Space();

				// Enable UnityEvent - On Despawn
				o.useOnDespawnUnityEvent = HTGUI_UNDO.ToggleField( o, "Enable On Despawn", new GUIContent( "Enable On Despawn:", gearLabel, _empty ), o.useOnDespawnUnityEvent, defaultGUILayoutOptions );

				// Enable UnityEvent - On Collided
				o.useOnPhysicsCollidedUnityEvent = HTGUI_UNDO.ToggleField( o, "Enable On Physics Collided", new GUIContent( "Enable On Physics Collided:", gearLabel, _empty ), o.useOnPhysicsCollidedUnityEvent, defaultGUILayoutOptions );

				// Enable UnityEvent - On Chain Spawn
				o.useOnChainSpawnUnityEvent = HTGUI_UNDO.ToggleField( o, "Enable On Chain Spawn", new GUIContent( "Enable On Chain Spawn:", gearLabel, _empty ), o.useOnChainSpawnUnityEvent, defaultGUILayoutOptions );
				
				EditorGUILayout.Space();

				// If we have enabled any of the UnityEvents, make the GUI cleaner by adding a seperator line
				if( o.useOnDespawnUnityEvent || o.useOnPhysicsCollidedUnityEvent || o.useOnChainSpawnUnityEvent ){ 
						EditorGUILayout.Space();
						HTGUI.SepLine();
						EditorGUILayout.Space();
				}

				// Reset _firstUnityEventDisplayed flag
				_firstUnityEventDisplayed = false;

				// Draw Despawn Unity Event
				if( o.useOnDespawnUnityEvent ){ FormatUnityEvent( "OnDespawnUnityEvent", "On Despawn", "This UnityEvent is fired when this component triggers a despawn." ); }

				// Draw Physics Collided Unity Event
				if( o.useOnPhysicsCollidedUnityEvent ){ FormatUnityEvent( "OnPhysicsCollidedUnityEvent", "On Physics Collided", "This UnityEvent is fired when one of the despawner's physics events detected a collision. The GameObject of the collision is passed as a parameter.", "This UnityEvent may have a small impact on performance if over-used. Only enable it if it is needed!" ); }

				// Draw Chain Spawn Unity Event
				if( o.useOnChainSpawnUnityEvent ){ FormatUnityEvent( "OnChainSpawnUnityEvent", "On Chain Spawn", "This UnityEvent is fired when a new instance is chain-spawned. The Transform of the new instance is passed as a parameter.", "This UnityEvent may have a small impact on performance. Only enable it if it is needed!" ); }

			// End White Box
			HTGUI.EndWhiteBox();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	FORMAT UNITYEVENT
		//	Show a UnityEvent with the correct spacing, formatting, titles and help tips
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		bool _firstUnityEventDisplayed = false;
		void FormatUnityEvent( string unityEventName, string title = "", string subtitle = "", string helpText = "" ){
			
			// Add Seperator line if a Unity Event has already been drawn
			if( _firstUnityEventDisplayed == true ){
				HTGUI.SepLine();
				EditorGUILayout.Space();
			}

			// Draw title and subtitle if set
			if( title != _empty ){
				HTGUI.SectionTitle( title, subtitle );
				EditorGUILayout.Space();
			}

			// Do Help Text if sent
			if( _showHelpfulNotes && helpText != _empty ){
				DoHelpBox( helpText );
				EditorGUILayout.Space();
			}

			// Make the unity events slightly darker when using the normal skin
		//	if( EditorGUIUtility.isProSkin == false ){ GUI.backgroundColor = new Color( 0.925f, 0.925f, 0.925f, 0.9f); }
		//	else { GUI.backgroundColor = new Color( 0.9f, 0.9f, 0.9f, 0.5f); }

			// Draw Unity Event
			HTGUI_UNDO.UnityEventField( serializedObject, unityEventName );
			
			// Reset Tint
		//	GUI.backgroundColor = Color.white;
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			// Set _firstUnityEventDisplayed flag
			_firstUnityEventDisplayed = true;
		}

	}
}









