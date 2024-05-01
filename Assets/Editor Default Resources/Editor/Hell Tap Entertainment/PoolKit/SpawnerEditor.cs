using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using HellTap.PoolKit;
using System.Reflection;

namespace HellTap.PoolKit {

	[CustomEditor(typeof(Spawner))]
	public class DespawnerEditor : Editor {

		// Static Icons
		static Texture2D headerIcon, spawnerIcon, spawnerSmallIcon, prefabIcon, prefabSmallIcon, spawnPointIcon, spawnPointSmallIcon, poolIcon, poolSmallIcon, eventsIcon, buttonIcon, nextLabel, diceLabel, origin3dLabel, gearLabel, xLabel, cubeLabel, widthLabel, loopLabel, resizeLabel, timeLabel, playLabel, upButton, downButton;

		// =======================
		//	CORE EDITOR VARIABLES
		// =======================

		// Main Object
		Spawner o;

		// Helpers for tabs
		GUIContent[] tabGUIContent = new GUIContent[]{ new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent() };
		string[] tabLabels = new string[]{ "  Spawner", "  Prefabs", " Instances", " Spawn Points", " Events" };
		Texture2D[] tabIcons = new Texture2D[]{ spawnerSmallIcon, prefabSmallIcon, poolSmallIcon, spawnPointSmallIcon, gearLabel };
		
		// Helpers for sub tabs
	//	string[] subtabLabels = new string[]{ "  Prefab", "  Instances", "  Features", "  Advanced" };
	//	Texture2D[] subtabIcons = new Texture2D[]{ cubeLabel, poolSmallIcon, loadLabel, gearIcon };

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	ON ENABLE
		//	Caches all editor icons, subscribes to delegates, and performs GameObject checks
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		void OnEnable(){

			// Cache the icons
			headerIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/PoolKit.png") as Texture2D;
			spawnerIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/Spawner.png") as Texture2D;
			spawnerSmallIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/SpawnerSmall.png") as Texture2D;
			prefabIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/Prefab.png") as Texture2D;
			prefabSmallIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/PrefabSmall.png") as Texture2D;
			spawnPointIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/SpawnPoint.png") as Texture2D;
			spawnPointSmallIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/SpawnPointSmall.png") as Texture2D;
			poolIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/Pool.png") as Texture2D;
			poolSmallIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/poolSmall.png") as Texture2D;
			eventsIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/Events.png") as Texture2D;
			buttonIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/buttonLabel.png") as Texture2D;
			nextLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/nextLabel.png") as Texture2D;
			diceLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/diceLabel.png") as Texture2D;
			origin3dLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/origin3dLabel.png") as Texture2D;
			gearLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/gearLabel.png") as Texture2D;
			xLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/xLabel.png") as Texture2D;
			cubeLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/cubeLabel.png") as Texture2D; 
			widthLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/widthLabel.png") as Texture2D;
			loopLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/loopLabel.png") as Texture2D;
			resizeLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/resizeLabel.png") as Texture2D;
			timeLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/timeLabel.png") as Texture2D;
			playLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/playLabel.png") as Texture2D;
			upButton = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/upButton.png") as Texture2D;
			downButton = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/downButton.png") as Texture2D;

			// Update Tab Sources
			tabGUIContent = new GUIContent[]{ new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent() };
			tabLabels = new string[]{ "  Spawner", "  Prefabs", " Instances", " Spawn Points", " Events" };
			tabIcons = new Texture2D[]{ spawnerSmallIcon, prefabSmallIcon, poolSmallIcon, spawnPointSmallIcon, gearLabel };
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
				HTGUI.Header(headerIcon, "Spawner", "This tool allows you to setup a new PoolKit Spawner.", "PoolKit", 80, 13);
			} else {
				EditorGUILayout.Space();
			}

			// Cache the target correctly
			if( target as Spawner != null ){ o = target as Spawner; }

			// Start Body Template
			if( o != null ){

				// Setup the tabs
				for( int i = 0; i < tabGUIContent.Length; i++ ){
					tabGUIContent[i].text = tabLabels[i];
					tabGUIContent[i].image = tabIcons[i];
					tabGUIContent[i].tooltip = System.String.Empty;
				}

				// Show the tabs
				o.tab = HTGUI.HTSelectionGrid( null, o.tab, tabGUIContent, 5, true, HT_GUIStyles.GetSelectionGridGUIStyle() );

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

			// Do Spawner Setup
			if(o.tab == 0 ){ DoSpawnerSetup(); }

			// Do Prefabs Setup
			if(o.tab == 1 ){ DoPrefabSetup(); }

			// Do Instance Setup
			if(o.tab == 2 ){ DoInstanceSetup(); }

			// Do Location Setup
			if(o.tab == 3 ){ DoLocationSetup(); }

			// Do Events Setup
			if(o.tab == 4 ){ DoEventsSetup(); }

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
		//	DO SPAWNER SETUP
		//	Do the Spawner Setup
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Default Layout Options
		GUILayoutOption[] defaultGUILayoutOptions = new GUILayoutOption[]{ GUILayout.MinWidth(200), GUILayout.MaxWidth(400), GUILayout.MaxHeight(20)};
		Vector2 _v2_100_64 = new Vector2(100, 64);

		// Method
		void DoSpawnerSetup(){

			// =================================
			//	TAB TITLE
			// =================================

			// Show Tab Headers
			if( _showTabHeaders ){

				// Start White Box
				HTGUI.StartWhiteBox();

					// Title
					EditorGUILayout.Space();
					HTGUI.Header( spawnerIcon, "Spawner", "This tab allows you to setup the Spawner by giving it a name and defining it's core functionality. You can setup when the Spawner will start, how long it will spawn instances and the time between each spawn. Prefabs, spawn points, rotations and scale can be independantly updated per spawn cycle or per instance.", System.String.Empty, _v2_100_64, 13
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
				StartIconColumn( spawnerIcon, 64, 64, 4 );

					// Title
					HTGUI.SectionTitle( "Spawner Setup", "Give your spawner a unique name to make it available in the PoolKit API." );
					EditorGUILayout.Space();

					// Spawner Name
					o.spawnerName = HTGUI_UNDO.TextField( o, "Spawner Name", new GUIContent( "Spawner Name:", buttonIcon, _empty ), o.spawnerName, defaultGUILayoutOptions  );

				// Cleanly ends the Icon Column
				EndIconColumn();

					// Add Seperator
					EditorGUILayout.Space();
					HTGUI.SepLine();
					EditorGUILayout.Space();

					// Spawning Will Begin ...
					o.spawningBegins = (Spawner.StartMode)HTGUI_UNDO.EnumField( o, "Spawning Will Begin", new GUIContent( "Spawning Will Begin:", gearLabel, _empty ), o.spawningBegins, defaultGUILayoutOptions );
					
						// Show Delay if use selected a start mode that requires it
						if( o.spawningBegins == Spawner.StartMode.AutomaticallyOnStartAfterDelay ||
							o.spawningBegins == Spawner.StartMode.AutomaticallyOnEnableAfterDelay
						){
							o.autoSpawnDelay = HTGUI_UNDO.FloatField( o, "Spawner Delay", new GUIContent( "Spawner Delay In Seconds:", timeLabel, _empty ), o.autoSpawnDelay, defaultGUILayoutOptions  );
						}

						// Show help 
						if( _showHelpfulNotes ){

							if ( o.spawningBegins == Spawner.StartMode.OnlyWhenCalledByScript ){
								DoHelpBox( "The Spawner will not do anything until you tell it to play via script:\n\nSpawner mySpawner = PoolKit.GetSpawner(\""+  o.spawnerName + "\")\nif( mySpawner != null ){ mySpawner.Play(); }");
						
							} else if(	o.spawningBegins == Spawner.StartMode.Never ){
								DoHelpBox( "This Spawner will NEVER spawn anything until the startMode is changed." );
							
							} else if(	o.spawningBegins == Spawner.StartMode.AutomaticallyOnEnable ||
										o.spawningBegins == Spawner.StartMode.AutomaticallyOnEnableAfterDelay
							){
								DoHelpBox( "This Spawner will automatically begin every time OnEnable() is triggered" + ( o.spawningBegins == Spawner.StartMode.AutomaticallyOnEnableAfterDelay ? " and a delay of " + o.autoSpawnDelay.ToString() + " seconds has passed" : _empty ) + ". This can be used to create powerful 'spawnable' Spawners!" );
							
							} else if(	o.spawningBegins == Spawner.StartMode.AutomaticallyOnStart ||
										o.spawningBegins == Spawner.StartMode.AutomaticallyOnStartAfterDelay
							){
								DoHelpBox( "This Spawner will automatically begin once the Start() method fires" + 
									( o.spawningBegins == Spawner.StartMode.AutomaticallyOnStartAfterDelay ? " and a delay of " + o.autoSpawnDelay.ToString() + " seconds has passed" : _empty ) + "." );
							}
						}

					// Add Seperator
					EditorGUILayout.Space();
					HTGUI.SepLine();
					EditorGUILayout.Space();

					// Spawner Duration
					o.spawnDuration = (Spawner.SpawnDuration)HTGUI_UNDO.EnumField( o, "Spawner Duration", new GUIContent( "Spawner Duration:", playLabel, _empty ), o.spawnDuration, defaultGUILayoutOptions );

						// Cycles To Repeat
						if( o.spawnDuration == Spawner.SpawnDuration.RepeatXCycles ){
							o.durationRepeatXTimes = HTGUI_UNDO.IntField( o, "Spawn Cycles To Repeat", new GUIContent( "Spawn Cycles To Repeat:", xLabel, _empty ), o.durationRepeatXTimes, defaultGUILayoutOptions  );
						
						// Instances To Repeat
						} else if( o.spawnDuration == Spawner.SpawnDuration.SpawnXInstances ){
							o.durationSpawnXInstances = HTGUI_UNDO.IntField( o, "Instances To Spawn", new GUIContent( "Instances To Spawn:", xLabel, _empty ), o.durationSpawnXInstances, defaultGUILayoutOptions  );

						// Spawner Countdown
						} else if( o.spawnDuration == Spawner.SpawnDuration.CountdownTimer ){
							o.durationCountdown = HTGUI_UNDO.FloatField( o, "Countdown In Seconds", new GUIContent( "Countdown In Seconds:", timeLabel, _empty ), o.durationCountdown, defaultGUILayoutOptions  );
						}

						// Show help 
						if( _showHelpfulNotes ){

							// Play Once
							if ( o.spawnDuration == Spawner.SpawnDuration.PlayOnce ){
								DoHelpBox( "This Spawner will perform a single spawn cycle and stop.");
							
							// Repeat X Cycles
							} else if ( o.spawnDuration == Spawner.SpawnDuration.RepeatXCycles ){
								DoHelpBox( "This Spawner will perform " +  NumberFormat( o.durationRepeatXTimes, "Spawn Cycle", "Spawn Cycles" ) + " and stop.");

							// Spawn X Instances
							} else if ( o.spawnDuration == Spawner.SpawnDuration.SpawnXInstances ){
								DoHelpBox( "This Spawner will spawn " +  NumberFormat( o.durationSpawnXInstances, "Instance", "Instances" ) + " and stop.");
							
							// Countdown Timer
							} else if ( o.spawnDuration == Spawner.SpawnDuration.CountdownTimer ){
								DoHelpBox( "This Spawner will keep spawning instances until " +  NumberFormat( o.durationCountdown, "second", "seconds" ) + " has passed and stop.");
							
							// Loop Forever
							} else if ( o.spawnDuration == Spawner.SpawnDuration.LoopForever ){
								DoHelpBox( "This Spawner will keep spawning instances until it is destroyed or stopped from the API.");
							}
						}

					// Add Seperator
					EditorGUILayout.Space();
					HTGUI.SepLine();
					EditorGUILayout.Space();

					// Spawner Frequency
					o.frequencyMode = (Spawner.SpawnerFrequencyMode)HTGUI_UNDO.EnumField( o, "Spawner Frequency", new GUIContent( "Spawner Frequency:", widthLabel, _empty ), o.frequencyMode, defaultGUILayoutOptions );
					
						// Fixed Interval Between Spawns
						if( o.frequencyMode == Spawner.SpawnerFrequencyMode.FixedInterval ){
							o.frequencyFixedInterval = HTGUI_UNDO.FloatField( o, "Spawn Interval", new GUIContent( "Spawn Interval:", timeLabel, _empty ), o.frequencyFixedInterval, defaultGUILayoutOptions );
						}

						// Random Range Interval Between Spawns
						else if( o.frequencyMode == Spawner.SpawnerFrequencyMode.RandomRange ){
							o.frequencyRandomMin = HTGUI_UNDO.FloatField( o, "Minimum Interval In Seconds", new GUIContent( "Minimum Interval In Seconds:", timeLabel, _empty ), o.frequencyRandomMin, defaultGUILayoutOptions );
							o.frequencyRandomMax = HTGUI_UNDO.FloatField( o, "Maximum Interval In Seconds", new GUIContent( "Maximum Interval In Seconds:", timeLabel, _empty ), o.frequencyRandomMax, defaultGUILayoutOptions );

							// Make sure the max interval is at least the same size as the minimum
							if( o.frequencyRandomMax < o.frequencyRandomMin ){ o.frequencyRandomMax = o.frequencyRandomMin; }
						}


						// Intances Per Cycle Mode (Fixed Number, Random Range)
						o.instancesPerCycleMode = (Spawner.InstancesPerCycleMode)HTGUI_UNDO.EnumField( o, "Instances To Spawn", new GUIContent( "Instances To Spawn:", prefabSmallIcon, _empty ), o.instancesPerCycleMode, defaultGUILayoutOptions );

						// Fixed Number Of Instances Per Cycle
						if( o.instancesPerCycleMode == Spawner.InstancesPerCycleMode.FixedNumber ){
							o.instancesPerCycle = HTGUI_UNDO.IntField( o, "Instances Per Cycle", new GUIContent( "Instances Per Cycle:", xLabel, _empty ), o.instancesPerCycle, defaultGUILayoutOptions );
						}

						// Random Range Of Instances Per Cycle
						else if( o.instancesPerCycleMode == Spawner.InstancesPerCycleMode.RandomRange ){
							o.minInstancesPerCycle = HTGUI_UNDO.IntField( o, "Min Instances Per Cycle", new GUIContent( "Min Instances Per Cycle:", xLabel, _empty ), o.minInstancesPerCycle, defaultGUILayoutOptions );
							o.maxInstancesPerCycle = HTGUI_UNDO.IntField( o, "Max Instances Per Cycle", new GUIContent( "Max Instances Per Cycle:", xLabel, _empty ), o.maxInstancesPerCycle, defaultGUILayoutOptions );

							// Make sure the max value is at least the same size as the minimum
							if( o.maxInstancesPerCycle < o.minInstancesPerCycle ){ o.maxInstancesPerCycle = o.minInstancesPerCycle; }
						}

						// Show help 
						if( _showHelpfulNotes ){

							if( o.instancesPerCycleMode == Spawner.InstancesPerCycleMode.FixedNumber ){

								// Play Once
								if ( o.frequencyMode == Spawner.SpawnerFrequencyMode.FixedInterval ){
									DoHelpBox( "This Spawner will spawn " + NumberFormat(o.instancesPerCycle, "new intance", "new instances") + " every " + NumberFormat(o.frequencyFixedInterval, "second.", "seconds.") );
								
								// Repeat X Times
								} else if ( o.frequencyMode == Spawner.SpawnerFrequencyMode.RandomRange ){
									DoHelpBox( "This Spawner will spawn " + NumberFormat(o.instancesPerCycle, "new intance", "new instances") + " every " + o.frequencyRandomMin.ToString() + " to " + o.frequencyRandomMax.ToString() + " seconds." );
								}

							} else if( o.instancesPerCycleMode == Spawner.InstancesPerCycleMode.RandomRange ){

								// Play Once
								if ( o.frequencyMode == Spawner.SpawnerFrequencyMode.FixedInterval ){
									DoHelpBox( "This Spawner will spawn between " + o.minInstancesPerCycle.ToString() +" and " + NumberFormat(o.maxInstancesPerCycle, "new intance", "new instances") + " every " + NumberFormat(o.frequencyFixedInterval, "second.", "seconds.") );
								
								// Repeat X Times
								} else if ( o.frequencyMode == Spawner.SpawnerFrequencyMode.RandomRange ){
									DoHelpBox( "This Spawner will spawn between " + o.minInstancesPerCycle.ToString() +" and " + NumberFormat(o.maxInstancesPerCycle, "new intance", "new instances") + " every " + o.frequencyRandomMin.ToString() + " to " + o.frequencyRandomMax.ToString() + " seconds." );
								}
							}
						}

					// Add Seperator
					EditorGUILayout.Space();
					HTGUI.SepLine();
					EditorGUILayout.Space();

						// [NEW] Increment Prefab
						o.incrementPrefab = (Spawner.Increment)HTGUI_UNDO.EnumField( o, "Update Prefab: ", new GUIContent( "Update Prefab:", prefabSmallIcon, _empty ), o.incrementPrefab, defaultGUILayoutOptions );
						
						// [NEW] Increment Position
						o.incrementPosition = (Spawner.Increment)HTGUI_UNDO.EnumField( o, "Update Spawn Point: ", new GUIContent( "Update Spawn Point:", spawnPointSmallIcon, _empty ), o.incrementPosition, defaultGUILayoutOptions );

						// [NEW] Increment Random Offset
						o.incrementRandomOffsets = (Spawner.Increment)HTGUI_UNDO.EnumField( o, "Update Random Offset: ", new GUIContent( "Update Random Offset:", origin3dLabel, _empty ), o.incrementRandomOffsets, defaultGUILayoutOptions );

						// [NEW] Increment Rotation
						o.incrementRotation = (Spawner.Increment)HTGUI_UNDO.EnumField( o, "Update Instance Rotation: ", new GUIContent( "Update Instance Rotation:", loopLabel, _empty ), o.incrementRotation, defaultGUILayoutOptions );

						// [NEW] Increment Scale
						o.incrementScale = (Spawner.Increment)HTGUI_UNDO.EnumField( o, "Update Scale: ", new GUIContent( "Update Instance Scale:", resizeLabel, _empty ), o.incrementScale, defaultGUILayoutOptions );


						// Show help 
						if( _showHelpfulNotes ){

							DoHelpBox( 	"This spawner selects new prefabs " + IncrementToString(o.incrementPrefab) +
										".\nSpawn Point positions are updated " + IncrementToString(o.incrementPosition) + 
										".\nRandom Offset positions are updated " + IncrementToString(o.incrementRandomOffsets) + 
										".\nInstance rotations are updated " + IncrementToString(o.incrementRotation) + 
										".\nInstance scales are updated " + IncrementToString(o.incrementScale) + "." 
							);
						}

				// Cleanly ends the Icon Column
				//EndIconColumn();

			// End White Box
			HTGUI.EndWhiteBox();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	INCREMENT TO STRING
		//	Converts the Increment enum to a readable string.
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
		string IncrementToString( Spawner.Increment increment ){
			
			if( increment == Spawner.Increment.PerInstance ){
				return "every time an instance is spawned";
			} else if( increment == Spawner.Increment.PerCycle ){
				return "every time a new spawn cycle begins";
			}

			// Return empty string if something goes wrong
			return string.Empty;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO PREFAB SETUP
		//	Do the 'Prefabs To Spawn' Setup
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
		GUIStyle _prefabGUIStyle = new GUIStyle();
		bool _foundMissingPrefab = false;

		// Method
		void DoPrefabSetup(){

			// =================================
			//	TAB TITLE
			// =================================

			// Show Tab Headers
			if( _showTabHeaders ){

				// Start White Box
				HTGUI.StartWhiteBox();

					// Title
					EditorGUILayout.Space();
					HTGUI.Header( prefabIcon, "Prefabs", "This tab allows you to setup which prefabs will be spawned and which selection mode will be used to choose the next instance. ", System.String.Empty, _v2_100_64, 13
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
				StartIconColumn( prefabIcon, 64, 64, 4 );

					// Title
					HTGUI.SectionTitle( "Prefabs To Spawn", "Setup which prefabs will be spawned and how they will be selected." );
					EditorGUILayout.Space();

					// Prefab Selection Mode
					o.prefabSelection = (Spawner.PrefabSelection)HTGUI_UNDO.EnumField( o, "Prefab Selection Mode", new GUIContent( "Prefab Selection Mode:", nextLabel, _empty ), o.prefabSelection, defaultGUILayoutOptions );

				// Cleanly ends the Icon Column
				EndIconColumn();

				// Show help 
				if( _showHelpfulNotes ){

					// Sequence Ascending
					if ( o.prefabSelection == Spawner.PrefabSelection.SequenceAscending ){
						DoHelpBox( "When an instance is spawned, a prefab will be selected in ascending order using the list below." );
					
					// Sequence Descending 
					} else if ( o.prefabSelection == Spawner.PrefabSelection.SequenceDescending ){
						DoHelpBox( "When an instance is spawned, a prefab will be selected in descending order using the list below." );
					
					// PingPong Ascending
					} else if ( o.prefabSelection == Spawner.PrefabSelection.PingPongAscending ){
						DoHelpBox( "When an instance is spawned, a prefab will be selected in ascending and then descending order using the list below." );
					
					// PingPong Descending
					} else if ( o.prefabSelection == Spawner.PrefabSelection.PingPongDescending ){
						DoHelpBox( "When an instance is spawned, a prefab will be selected in descending and then ascending order using the list below." );
					
					// Random
					} else if ( o.prefabSelection == Spawner.PrefabSelection.Random ){
						DoHelpBox( "When an instance is spawned, a prefab will be selected at random from the list below." );
					
					// Random
					} else if ( o.prefabSelection == Spawner.PrefabSelection.RandomWithWeights ){
						DoHelpBox( "When an instance is spawned, a prefab will be selected at random from the list below using weights. Setting a chance to 0% means a prefab will never be chosen and 100% gives it a full chance." );
					
					}
				}

				// Add Seperator
				EditorGUILayout.Space();
				HTGUI.SepLine();
				EditorGUILayout.Space();

				// Make sure the prefabs list is NOT null
				if(o.prefabs==null){ o.prefabs = new Spawner.PrefabToSpawn[0]; }

				// Setup GUI Styles
				_prefabGUIStyle = EditorStyles.label;
				_prefabGUIStyle.fontStyle = FontStyle.Bold;

				// If we have prefabs setup, render them
				_foundMissingPrefab = false;
				if( o.prefabs.Length > 0 ){

					// ------------------------
					//	PREFAB OBJECT LIST
					// ------------------------

					// Loop through the list
					for( int i = 0; i < o.prefabs.Length; i++ ){

						// Start Row
						GUILayout.BeginHorizontal();

							// Check for missing GameObject ( tint prefab icon red)
							if( o.prefabs[i].prefab == null ){
								GUI.color = new Color( 1f, 0.5f, 0.5f, 1f );
								_foundMissingPrefab = true;
							}

							// Label
							GUILayout.Label( cubeLabel, GUILayout.MaxWidth(20), GUILayout.MinHeight(20), GUILayout.MaxHeight(20) );
							GUI.color = Color.white; // Reset color tint
							GUILayout.Label( "Prefab " + (i+1) + ":", _prefabGUIStyle, GUILayout.MinWidth(64), GUILayout.MaxWidth(64), GUILayout.MinHeight(20), GUILayout.MaxHeight(20));

							// Check for missing GameObject (tint background fields red)
							if( o.prefabs[i].prefab == null ){
								GUI.backgroundColor = new Color( 1f, 0.5f, 0.5f, 0.5f );
							}

							// Prefab GameObject
							o.prefabs[i].prefab = (GameObject) HTGUI_UNDO.ObjectField( o, "Prefab " + (i+1).ToString(), o.prefabs[i].prefab, false, new GUILayoutOption[]{ GUILayout.MinWidth(160) } );

							// Reset background color
							GUI.backgroundColor = Color.white;

							// Show Random Weights if we're using them
							if( o.prefabSelection == Spawner.PrefabSelection.RandomWithWeights ){

								// Label
								GUILayout.Label( diceLabel, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
								GUILayout.Label( "Chance:", _prefabGUIStyle, GUILayout.MinWidth(50), GUILayout.MaxWidth(50), GUILayout.MaxHeight(20));

								// Show Slider
								o.prefabs[i].randomWeight = HTGUI_UNDO.SliderField( o, "Random Weight Chance", o.prefabs[i].randomWeight, 0f, 100f, new GUILayoutOption[]{ GUILayout.MinWidth(120), GUILayout.MinHeight(20) } );
							}

							// Only allow the prefab items to be moved when the game isn't running
							if( Application.isPlaying == false ){

								// Add some space 
								GUILayout.Space(8);

								// Move Up
								if( o.prefabs.Length > 0 && i != 0 ){
									if( GUILayout.Button( new GUIContent( System.String.Empty, upButton, "Move Prefab Up"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
									){
										Undo.RecordObject ( o, "Move Prefab Up" );
										Arrays.Shift( ref o.prefabs, i, true );
									}
								
								// Add space when we don't draw this
								} else if( o.prefabs.Length > 2 ){
									//GUILayout.Space(28);

									GUI.enabled = false;
									GUILayout.Button( new GUIContent( System.String.Empty, upButton, "Move Prefab Up"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) );
									GUI.enabled = true;
								}

								// Move Down
								if( o.prefabs.Length > 0 && i != o.prefabs.Length-1 ){

									if( GUILayout.Button( new GUIContent( System.String.Empty, downButton, "Move Prefab Down"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) )  
									){
										Undo.RecordObject ( o, "Move Prefab Down" );
										Arrays.Shift( ref o.prefabs, i, false );
									}
								
								// Add space when we don't draw this
								} else if( o.prefabs.Length > 2 ){
									//GUILayout.Space(28);

									GUI.enabled = false;
									GUILayout.Button( new GUIContent( System.String.Empty, downButton, "Move Prefab Down"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) );
									GUI.enabled = true;
								}

								// Remove (we must ensure at least 1 item exists)
								if( o.prefabs.Length > 1 &&
									GUILayout.Button( new GUIContent( System.String.Empty, HTGUI.removeButton, "Remove Prefab" ), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
								){
									Undo.RecordObject ( o, "Remove Prefab" );
									Arrays.RemoveItemAtIndex( ref o.prefabs, i );
								}

							}

						// Completely End Row
						GUILayout.EndHorizontal();
						GUILayout.Space(1);
					}

					// At least one prefab was missing!
					if( _foundMissingPrefab ){
						DoHelpBox( "One or more prefabs above don't have a valid GameObject!" );
					}
				
				// No Prefabs Set yet!
				} else {

					GUILayout.Label( new GUIContent( "   This Spawner can't spawn any instances because there are no prefabs setup yet.\n   Press the green '+' icon below to add the first prefab!", prefabSmallIcon, _empty )  );
				}

				// Add Seperator
				EditorGUILayout.Space();
				HTGUI.SepLine();

				// Add / Remove Prefabs (Create Horizontal Row)
				if( !Application.isPlaying ){

					// Start Horizontal Row
					EditorGUILayout.BeginHorizontal();

						// Flexible Space
						GUILayout.FlexibleSpace();	
											
						// Remove Button			
						if( o.prefabs.Length == 0 ){ GUI.enabled = false; }			
						if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.removeButton, "Remove Last Prefab"), GUILayout.MaxWidth(32)) ) { 
							if( o.prefabs.Length > 0 ){	// <- We must always have at least 1 condition!
								Undo.RecordObject ( o, "Remove Last Prefab");
								System.Array.Resize(ref o.prefabs, o.prefabs.Length - 1 );
							}
						}

						// Reset GUI Enabled
						GUI.enabled = true;
										
						// Add Button							
						if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.addButton, "Add New Prefab"), GUILayout.MaxWidth(32))) { 
							Undo.RecordObject ( o, "Add New Prefab");
							System.Array.Resize(ref o.prefabs, o.prefabs.Length + 1 ); 
							o.prefabs[ o.prefabs.Length - 1 ] = new Spawner.PrefabToSpawn();	// <- Requires new instance!
						}

					// End Horizontal Row
					EditorGUILayout.EndHorizontal();

				}

			// End White Box
			HTGUI.EndWhiteBox();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO INSTANCE SETUP
		//	Do the Instance Setup
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Method
		void DoInstanceSetup(){

			// =================================
			//	TAB TITLE
			// =================================

			// Show Tab Headers
			if( _showTabHeaders ){

				// Start White Box
				HTGUI.StartWhiteBox();

					// Title
					EditorGUILayout.Space();
					HTGUI.Header( poolIcon, "Instances", "This tab allows you to configure how instances are setup when they are spawned. Instances can be rotated, scaled and parented to other objects using a variety of methods.", System.String.Empty, _v2_100_64, 13
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
				StartIconColumn( poolIcon, 64, 64, 4 );

					// Title
					HTGUI.SectionTitle( "Instances", "Setup how your instances will be parented, scaled and rotated when spawned." );
					EditorGUILayout.Space();

					// Spawner Name
					o.reparentInstances = (Spawner.ParentMode)HTGUI_UNDO.EnumField( o, "Instance Parent Mode", new GUIContent( "Instance Parent Mode:", gearLabel, _empty ), o.reparentInstances, defaultGUILayoutOptions );

					// Custom Parent Transform
					if( o.reparentInstances == Spawner.ParentMode.ReparentToCustomTransform ){
						o.customParentTransform = (Transform) HTGUI_UNDO.ObjectField( o, "Custom Parent Transform", new GUIContent( "Custom Parent Transform:", origin3dLabel, _empty ), o.customParentTransform, true, defaultGUILayoutOptions );
					}

				// Cleanly ends the Icon Column
				EndIconColumn();

				// Show help 
				if( _showHelpfulNotes ){

					// Ignore
					if ( o.reparentInstances == Spawner.ParentMode.Ignore ){
						DoHelpBox( "Your instances will not be parented to anything and use default settings." );
					
					// Reparent To Spawner
					} else if ( o.reparentInstances == Spawner.ParentMode.ReparentToSpawner ){
						DoHelpBox( "Your instances will be parented to this Spawner's Transform." );
					
					// Reparent To Spawn Point
					} else if ( o.reparentInstances == Spawner.ParentMode.ReparentToSpawnPoint ){
						DoHelpBox( "If you're using a Transform Location List, your instances will be parented to the last spawn point used." );
					
					// Reparent To Spawn Point
					} else if ( o.reparentInstances == Spawner.ParentMode.ReparentToCustomTransform ){
						if( o.customParentTransform != null ){
							DoHelpBox( "Your instances will be reparented to the custom transform: " + o.customParentTransform.name );
						} else {
							DoHelpBox( "NOTE: You have not set the 'Custom Parent Transform'. Default settings will be used!" );
						}
					}
				}

				// Add Seperator
				EditorGUILayout.Space();
				HTGUI.SepLine();
				EditorGUILayout.Space();

				// Rotation Mode
				o.rotationMode = (Spawner.RotationMode)HTGUI_UNDO.EnumField( o, "Instance Rotation Mode", new GUIContent( "Instance Rotation Mode:", gearLabel, _empty ), o.rotationMode, defaultGUILayoutOptions );
				
					// If we're using custom euler angles, show it!
					if( o.rotationMode == Spawner.RotationMode.CustomEulerAngles ){
						o.customRotationEulerAngles = HTGUI_UNDO.Vector3Field( o, "Custom Euler Angles", new GUIContent( "Custom Euler Angles:", loopLabel, _empty ), o.customRotationEulerAngles, defaultGUILayoutOptions  );
					}

					// If we're using custom euler angles, show it!
					if( o.rotationMode == Spawner.RotationMode.PrefabDefault || 
						o.rotationMode == Spawner.RotationMode.SpawnerRotation || 
						o.rotationMode == Spawner.RotationMode.SpawnPointRotation 
					){
						o.customRotationEulerAngles = HTGUI_UNDO.Vector3Field( o, "Offset Rotation", new GUIContent( "Offset Rotation:", loopLabel, _empty ), o.customRotationEulerAngles, defaultGUILayoutOptions  );
					}

					// Show help 
					if( _showHelpfulNotes ){

						if ( o.rotationMode == Spawner.RotationMode.PrefabDefault ){
							DoHelpBox( "Instances will be rotated using the default setting of the prefab." + (o.customRotationEulerAngles != Vector3.zero ? " A rotational offset of " + o.customRotationEulerAngles.ToString() + " will then be applied." : string.Empty ) );
					
						} else if ( o.rotationMode == Spawner.RotationMode.SpawnerRotation ){
							DoHelpBox( "Instances will be rotated to match the rotation of this Spawner." + (o.customRotationEulerAngles != Vector3.zero ? " A rotational offset of " + o.customRotationEulerAngles.ToString() + " will then be applied." : string.Empty ) );

						} else if ( o.rotationMode == Spawner.RotationMode.SpawnPointRotation ){
							if( o.prefabsWillBeSpawned == Spawner.SpawnLocation.UsingTransformList ){
								DoHelpBox( "Instances will be rotated to match the last Spawn Point's rotation." + (o.customRotationEulerAngles != Vector3.zero ? " A rotational offset of " + o.customRotationEulerAngles.ToString() + " will then be applied." : string.Empty ) );
							} else {
								DoHelpBox( "This configuration will not work correctly because your Spawn Points are not using a Transform List. The Spawner's rotation will be used instead!" + (o.customRotationEulerAngles != Vector3.zero ? " A rotational offset of " + o.customRotationEulerAngles.ToString() + " will then be applied." : string.Empty ), MessageType.Warning );
							}
						
						} else if ( o.rotationMode == Spawner.RotationMode.CustomEulerAngles ){
							DoHelpBox( "Instances will be rotated using the 'Custom Euler Angles' setting above: " + o.customRotationEulerAngles.ToString() );
						
						} else if ( o.rotationMode == Spawner.RotationMode.RandomRotation ){
							DoHelpBox( "Instances will be rotated randomly." );
						}
					}

				// Add Seperator
				EditorGUILayout.Space();
				HTGUI.SepLine();
				EditorGUILayout.Space();

				// Scale Mode
				o.scaleMode = (Spawner.ScaleMode)HTGUI_UNDO.EnumField( o, "Instance Scale Mode", new GUIContent( "Instance Scale Mode:", resizeLabel, _empty ), o.scaleMode, defaultGUILayoutOptions );
				
					// If we're using custom local scale, show it!
					if( o.scaleMode == Spawner.ScaleMode.CustomLocalScale ){
						o.customLocalScale = HTGUI_UNDO.Vector3Field( o, "Custom Local Scale", new GUIContent( "Custom Local Scale:", resizeLabel, _empty ), o.customLocalScale, defaultGUILayoutOptions  );
					}

					// If we're using Random Range, show it!
					else if( o.scaleMode == Spawner.ScaleMode.RandomRangeScale ){
						
						// Draw Fields
						o.customLocalScaleMin = HTGUI_UNDO.Vector3Field( o, "Minimum Local Scale", new GUIContent( "Minimum Local Scale:", resizeLabel, _empty ), o.customLocalScaleMin, defaultGUILayoutOptions  );
						o.customLocalScaleMax = HTGUI_UNDO.Vector3Field( o, "Maximum Local Scale", new GUIContent( "Maximum Local Scale:", resizeLabel, _empty ), o.customLocalScaleMax, defaultGUILayoutOptions  );

						// Make sure Max isn't smaller than Min
						if( o.customLocalScaleMax.x < o.customLocalScaleMin.x ){ 
							o.customLocalScaleMax.x = o.customLocalScaleMin.x; 
						}
						if( o.customLocalScaleMax.y < o.customLocalScaleMin.y ){ 
							o.customLocalScaleMax.y = o.customLocalScaleMin.y; 
						}
						if( o.customLocalScaleMax.z < o.customLocalScaleMin.z ){ 
							o.customLocalScaleMax.z = o.customLocalScaleMin.z; 
						}
					}

					// If we're using Random Range, show it!
					else if( o.scaleMode == Spawner.ScaleMode.RandomRangeProportionalScale ){
						
						// Draw Fields
						o.customLocalScaleProportionalMin = HTGUI_UNDO.FloatField( o, "Minimum Proportional Scale", new GUIContent( "Minimum Proportional Scale:", resizeLabel, _empty ), o.customLocalScaleProportionalMin, defaultGUILayoutOptions  );
						o.customLocalScaleProportionalMax = HTGUI_UNDO.FloatField( o, "Maximum Proportional Scale", new GUIContent( "Maximum Proportional Scale:", resizeLabel, _empty ), o.customLocalScaleProportionalMax, defaultGUILayoutOptions  );

						// Make sure Max isn't smaller than Min
						if( o.customLocalScaleProportionalMax < o.customLocalScaleProportionalMin ){ 
							o.customLocalScaleProportionalMax = o.customLocalScaleProportionalMin; 
						}
					}

					// Show help 
					if( _showHelpfulNotes ){

						if ( o.scaleMode == Spawner.ScaleMode.PoolDefault ){
							DoHelpBox( "Instances will be scaled using their associated Pool settings." );
					
						} else if ( o.scaleMode == Spawner.ScaleMode.PrefabDefault ){
							DoHelpBox( "Instances will be scaled using their default prefab scale." );
						
						} else if ( o.scaleMode == Spawner.ScaleMode.SpawnerScale ){
							DoHelpBox( "Instances will be scaled using the scale of this Spawner's Transform." );
						
						} else if ( o.scaleMode == Spawner.ScaleMode.CustomLocalScale ){
							DoHelpBox( "Instances will be scaled using the 'Custom Local Scale' setting above: " + o.customLocalScale.ToString());
						
						} else if ( o.scaleMode == Spawner.ScaleMode.RandomRangeScale ){
							DoHelpBox( "Instances will be scaled using the Random Range specified above: " + o.customLocalScaleMin.ToString() + " to " + o.customLocalScaleMax.ToString() + "."  );
						
						} else if ( o.scaleMode == Spawner.ScaleMode.RandomRangeProportionalScale ){
							DoHelpBox( "Instances will be scaled proportionally using the Random Range specified above: ( " + o.customLocalScaleProportionalMin.ToString() + " to " + o.customLocalScaleProportionalMax.ToString() + " )." );
						}
					}

			// End White Box
			HTGUI.EndWhiteBox();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO LOCATION SETUP
		//	Do the Location Setup
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
		GUIStyle _locationGUIStyle = new GUIStyle();
		bool _foundMissingTransform = false;
		string _helpBoxString = string.Empty;

		// Method
		void DoLocationSetup(){

			// =================================
			//	TAB TITLE
			// =================================

			// Show Tab Headers
			if( _showTabHeaders ){

				// Start White Box
				HTGUI.StartWhiteBox();

					// Title
					EditorGUILayout.Space();
					HTGUI.Header( spawnPointIcon, "Spawn Points", "This tab allows you to configure where instances will be spawned to. You can spawn instances directly to the Spawner, using a list of Transform objects or a list of Vector3 positions. There are many options to control each mode as well as selecting a variety of techniques on how the next Spawn Point will be chosen.", System.String.Empty, _v2_100_64, 13
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
				StartIconColumn( spawnPointIcon, 64, 64, 4 );

					// Title
					HTGUI.SectionTitle( "Spawn Locations", "Setup where your instances will be spawned." );
					EditorGUILayout.Space();

					// Prefabs Will Be Spawned ...
					o.prefabsWillBeSpawned = (Spawner.SpawnLocation)HTGUI_UNDO.EnumField( o, "Instances Will Be Spawned", new GUIContent( "Instances Will Be Spawned:", gearLabel, _empty ), o.prefabsWillBeSpawned, defaultGUILayoutOptions );

					// If we're using a local Position list, allow a user to have the positions local to another transform
					if( o.prefabsWillBeSpawned == Spawner.SpawnLocation.UsingLocalPositionList ){
						o.spawnPositionsAreLocalTo = (Spawner.SpawningLocalTo)HTGUI_UNDO.EnumField( o, "Spawn Positions Local To", new GUIContent( "Spawn Positions Local To:", origin3dLabel, _empty ), o.spawnPositionsAreLocalTo, defaultGUILayoutOptions );

						// If we're using a Custom Transform, show it
						if( o.spawnPositionsAreLocalTo == Spawner.SpawningLocalTo.CustomTransform ){
							o.spawnLocalTo = (Transform)HTGUI_UNDO.ObjectField( o, "Spawn Local To", new GUIContent( "Spawn Local To:", origin3dLabel, _empty ), o.spawnLocalTo, true, defaultGUILayoutOptions );
						}
					}

					// Show Selection Options if we're using a transform or position list
					if( o.prefabsWillBeSpawned == Spawner.SpawnLocation.UsingTransformList ||
						o.prefabsWillBeSpawned == Spawner.SpawnLocation.UsingLocalPositionList ||
						o.prefabsWillBeSpawned == Spawner.SpawnLocation.UsingGlobalPositionList 
					){
						o.spawnPointSelection = (Spawner.SpawnPointSelection)HTGUI_UNDO.EnumField( o, "Spawnpoint Selection Mode", new GUIContent( "Spawnpoint Selection Mode:", nextLabel, _empty ), o.spawnPointSelection, defaultGUILayoutOptions );
					}

				// Cleanly ends the Icon Column
				EndIconColumn();

				// Show help 
				if( _showHelpfulNotes ){

					// At This Transform
					if ( o.prefabsWillBeSpawned == Spawner.SpawnLocation.AtThisTransform ){
						DoHelpBox( "All instances will be spawned using the position of this Spawner." );
					
					// Using Transform List
					} else if ( o.prefabsWillBeSpawned == Spawner.SpawnLocation.UsingTransformList ){

						_helpBoxString = "All instances will be spawned using the Transform List below. ";
						_helpBoxString += GetSpawnPointSelectionModeHelp();
						DoHelpBox( _helpBoxString );
					
					// Using Local Position List
					} else if ( o.prefabsWillBeSpawned == Spawner.SpawnLocation.UsingLocalPositionList ){
						
						if( o.spawnPositionsAreLocalTo == Spawner.SpawningLocalTo.CustomTransform ){
							_helpBoxString = "All instances will be spawned using the Local Position List below. All Positions will be local to " + ( o.spawnLocalTo != null ? "the Transform '" + o.spawnLocalTo.name + "'. " : "the Spawner because you have NOT set the 'Spawn Local To' Transform. " ) ;
						} else {
							_helpBoxString = "All instances will be spawned using the Local Position List below. All Positions will be local to the Spawner. ";
						}
						_helpBoxString += GetSpawnPointSelectionModeHelp();

						DoHelpBox( _helpBoxString );
					
					// Using Global Position List
					} else if ( o.prefabsWillBeSpawned == Spawner.SpawnLocation.UsingGlobalPositionList ){

						_helpBoxString = "All instances will be spawned using the Global Position List below. ";
						_helpBoxString += GetSpawnPointSelectionModeHelp();
						DoHelpBox( _helpBoxString );
					
					}
				}

				// Add Space
				EditorGUILayout.Space();

				// Make sure the location lists are NOT null
				if(o.spawnpointTransforms==null){ o.spawnpointTransforms = new Spawner.TransformSpawnPoint[0]; }
				if(o.spawnpointPositions==null){ o.spawnpointPositions = new Spawner.PositionSpawnPoint[0]; }

				// Setup GUI Styles
				_locationGUIStyle = EditorStyles.label;
				_locationGUIStyle.fontStyle = FontStyle.Bold;

				// Show the Transform Location Array if selected
				if ( o.prefabsWillBeSpawned == Spawner.SpawnLocation.UsingTransformList ){
					DoTransformLocationArray();

				// Show the Position Location Array	if selected
				} else if ( o.prefabsWillBeSpawned == Spawner.SpawnLocation.UsingLocalPositionList ||
							o.prefabsWillBeSpawned == Spawner.SpawnLocation.UsingGlobalPositionList
				){
					DoPositionLocationArray();
				}

				// Add Seperator
				EditorGUILayout.Space();
				HTGUI.SepLine();
				EditorGUILayout.Space();

				// Title
				HTGUI.SectionTitle( "Randomized Offsets", "Random offsets may be applied to the spawn point positions above." );
				EditorGUILayout.Space();

				// Add Randomization Range
				o.addSpawnPointRandomizationRange = HTGUI_UNDO.ToggleField( o, "Apply Randomized Offset",  new GUIContent( "Apply Randomized Offset:", gearLabel, _empty ), o.addSpawnPointRandomizationRange, defaultGUILayoutOptions );

				// Show randomization range
				if( o.addSpawnPointRandomizationRange ){

					// Randomization Range Min
					o.spawnPointRandomizationRangeMin = HTGUI_UNDO.Vector3Field( o, "Minimum Random Range",  new GUIContent( "Minimum Random Range:", origin3dLabel, _empty ), o.spawnPointRandomizationRangeMin, defaultGUILayoutOptions );

					// Randomization Range Max
					o.spawnPointRandomizationRangeMax = HTGUI_UNDO.Vector3Field( o, "Maximum Random Range",  new GUIContent( "Maximum Random Range:", origin3dLabel, _empty ), o.spawnPointRandomizationRangeMax, defaultGUILayoutOptions );

					// Make sure the maximum ranges are not less than the minimum ranges
					if( o.spawnPointRandomizationRangeMax.x < o.spawnPointRandomizationRangeMin.x ){
						o.spawnPointRandomizationRangeMax.x = o.spawnPointRandomizationRangeMin.x;
					}
					if( o.spawnPointRandomizationRangeMax.y < o.spawnPointRandomizationRangeMin.y ){
						o.spawnPointRandomizationRangeMax.y = o.spawnPointRandomizationRangeMin.y;
					}
					if( o.spawnPointRandomizationRangeMax.z < o.spawnPointRandomizationRangeMin.z ){
						o.spawnPointRandomizationRangeMax.z = o.spawnPointRandomizationRangeMin.z;
					}

					// Show Helpful Notes
					if( _showHelpfulNotes ){
						DoHelpBox( "A randomized offset between " + o.spawnPointRandomizationRangeMin.ToString() + " and " + o.spawnPointRandomizationRangeMax.ToString() + " will be added to the spawn point position." );
					}

					// Add Seperator
					EditorGUILayout.Space();
					HTGUI.SepLine();
					EditorGUILayout.Space();

					// Title
					HTGUI.SectionTitle( "Clamp Randomized Offsets", "Randomized offsets may be clamped with a min and max magnitude." );
					EditorGUILayout.Space();

					// Add Randomization Range
					o.addSpawnPointRandomizationMinMaxDistance = HTGUI_UNDO.ToggleField( o, "Apply An Offset Clamp",  new GUIContent( "Apply An Offset Clamp:", gearLabel, _empty ), o.addSpawnPointRandomizationMinMaxDistance, defaultGUILayoutOptions );

					// Show Clamp Randomization Options
					if( o.addSpawnPointRandomizationMinMaxDistance ){

						// Randomization Clamp Min
						o.spawnPointRandomizationMinDistance = HTGUI_UNDO.FloatField( o, "Minimum Clamp",  new GUIContent( "Minimum Clamp:", origin3dLabel, _empty ), o.spawnPointRandomizationMinDistance, defaultGUILayoutOptions );

						// Randomization Clamp Max
						o.spawnPointRandomizationMaxDistance = HTGUI_UNDO.FloatField( o, "Maximum Clamp",  new GUIContent( "Maximum Clamp:", origin3dLabel, _empty ), o.spawnPointRandomizationMaxDistance, defaultGUILayoutOptions );

						// Make sure the max distance is always greater than the minimum distance
						if( o.spawnPointRandomizationMaxDistance <= o.spawnPointRandomizationMinDistance ){
							o.spawnPointRandomizationMaxDistance = o.spawnPointRandomizationMinDistance + 0.01f;
						}

						// Show Helpful Notes
						if( _showHelpfulNotes ){
							DoHelpBox( "The randomization offset will be clamped between a magnitude of " + o.spawnPointRandomizationMinDistance.ToString() + " and " + o.spawnPointRandomizationMaxDistance.ToString() + " from the spawn point position." );
						}
					}

				} else {

					// Make sure we turn off min max distance clamps if we turn off randomization
					o.addSpawnPointRandomizationMinMaxDistance = false;

					// Show Helpful Notes
					if( _showHelpfulNotes ){
						DoHelpBox( "No randomized offsets will be applied." );
					}
				}

			// End White Box
			HTGUI.EndWhiteBox();

		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	GET SPAWN POINT SELECTION MODE HELP
		//	Returns a string used to build the help box
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
		string GetSpawnPointSelectionModeHelp(){

			// SequenceAscending
			if( o.spawnPointSelection == Spawner.SpawnPointSelection.SequenceAscending ){
				return "The spawn point will be selected by going through the list in ascending order.";
			
			// SequenceDescending
			} else if( o.spawnPointSelection == Spawner.SpawnPointSelection.SequenceDescending ){
				return "The spawn point will be selected by going through the list in descending order.";
			
			// PingPongAscending
			} else if( o.spawnPointSelection == Spawner.SpawnPointSelection.PingPongAscending ){
				return "The spawn point will be selected by going through the list in ascending and then descending order.";
			
			// PingPongDescending
			} else if( o.spawnPointSelection == Spawner.SpawnPointSelection.PingPongDescending ){
				return "The spawn point will be selected by going through the list in descending and then ascending order.";
			
			// Random
			} else if( o.spawnPointSelection == Spawner.SpawnPointSelection.Random ){
				return "The spawn point will be selected at random.";
			
			// Random With Weights
			} else if( o.spawnPointSelection == Spawner.SpawnPointSelection.RandomWithWeights ){
				return "The spawn point will be selected at random by using weights. Setting a chance to 0% means a spawn point will never be chosen and 100% gives it a full chance.";			
			}

			// Return an empty string if something goes wrong
			return _empty;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO TRANSFORM LOCATION ARRAY
		//	Show the Transform SpawnPoint List
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
		void DoTransformLocationArray(){

			// Add Seperator
			HTGUI.SepLine();
			EditorGUILayout.Space();

			// If we have spawnpointTransforms setup, render them
			_foundMissingTransform = false;
			if( o.spawnpointTransforms.Length > 0 ){

				// ------------------------
				//	PREFAB OBJECT LIST
				// ------------------------

				// Loop through the list
				for( int i = 0; i < o.spawnpointTransforms.Length; i++ ){

					// Start Row
					GUILayout.BeginHorizontal();

						// Check for missing GameObject ( tint spawnPoint icon red)
						if( o.spawnpointTransforms[i].spawnPoint == null ){
							GUI.color = new Color( 1f, 0.5f, 0.5f, 1f );
							_foundMissingTransform = true;
						}

						// Label
						GUILayout.Label( origin3dLabel, GUILayout.MaxWidth(20), GUILayout.MinHeight(20), GUILayout.MaxHeight(20) );
						GUI.color = Color.white; // Reset color tint
						GUILayout.Label( "Point " + (i+1) + ":", _locationGUIStyle, GUILayout.MinWidth(64), GUILayout.MaxWidth(64), GUILayout.MinHeight(20), GUILayout.MaxHeight(20));

						// Check for missing GameObject (tint background fields red)
						if( o.spawnpointTransforms[i].spawnPoint == null ){
							GUI.backgroundColor = new Color( 1f, 0.5f, 0.5f, 0.5f );
						}

						// Spawn Point Transform (allow scene Transforms)
						o.spawnpointTransforms[i].spawnPoint = (Transform) HTGUI_UNDO.ObjectField( o, "Point " + (i+1).ToString(), o.spawnpointTransforms[i].spawnPoint, true, new GUILayoutOption[]{ GUILayout.MinWidth(160) } );

						// Reset background color
						GUI.backgroundColor = Color.white;

						// Show Random Weights if we're using them
						if( o.spawnPointSelection == Spawner.SpawnPointSelection.RandomWithWeights ){

							// Label
							GUILayout.Label( diceLabel, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
							GUILayout.Label( "Chance:", _locationGUIStyle, GUILayout.MinWidth(50), GUILayout.MaxWidth(50), GUILayout.MaxHeight(20));

							// Show Slider
							o.spawnpointTransforms[i].randomWeight = HTGUI_UNDO.SliderField( o, "Random Weight Chance", o.spawnpointTransforms[i].randomWeight, 0f, 100f, new GUILayoutOption[]{ GUILayout.MinWidth(120), GUILayout.MinHeight(20) } );
						}

						// Only allow the prefab items to be moved when the game isn't running
						if( Application.isPlaying == false ){

							// Add some space 
							GUILayout.Space(8);

							// Move Up
							if( o.spawnpointTransforms.Length > 0 && i != 0 ){
								if( GUILayout.Button( new GUIContent( System.String.Empty, upButton, "Move Spawn Point Up"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
								){
									Undo.RecordObject ( o, "Move Spawn Point Up" );
									Arrays.Shift( ref o.spawnpointTransforms, i, true );
								}
							
							// Add space when we don't draw this
							} else if( o.spawnpointTransforms.Length > 2 ){
								//GUILayout.Space(28);

								GUI.enabled = false;
								GUILayout.Button( new GUIContent( System.String.Empty, upButton, "Move Spawn Point Up"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) );
								GUI.enabled = true;
							}

							// Move Down
							if( o.spawnpointTransforms.Length > 0 && i != o.spawnpointTransforms.Length-1 ){

								if( GUILayout.Button( new GUIContent( System.String.Empty, downButton, "Move Spawn Point Down"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) )  
								){
									Undo.RecordObject ( o, "Move Spawn Point Down" );
									Arrays.Shift( ref o.spawnpointTransforms, i, false );
								}
							
							// Add space when we don't draw this
							} else if( o.spawnpointTransforms.Length > 2 ){
								//GUILayout.Space(28);

								GUI.enabled = false;
								GUILayout.Button( new GUIContent( System.String.Empty, downButton, "Move Spawn Point Down"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) );
								GUI.enabled = true;
							}

							// Remove (we must ensure at least 1 item exists)
							if( o.spawnpointTransforms.Length > 1 &&
								GUILayout.Button( new GUIContent( System.String.Empty, HTGUI.removeButton, "Remove Spawn Point" ), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
							){
								Undo.RecordObject ( o, "Remove Spawn Point" );
								Arrays.RemoveItemAtIndex( ref o.spawnpointTransforms, i );
							}

						}

					// Completely End Row
					GUILayout.EndHorizontal();
					GUILayout.Space(1);
				}

				// At least one prefab was missing!
				if( _foundMissingTransform ){
					DoHelpBox( "One or more spawn points above don't have a valid Transform!" );
				}
			
			// No Prefabs Set yet!
			} else {

				GUILayout.Label( new GUIContent( "   This Spawner can't spawn any instances because there are no spawn points setup yet.\n   Press the green '+' icon below to add the first spawn point!", prefabSmallIcon, _empty )  );
			}

			// Add Seperator
			EditorGUILayout.Space();
			EditorGUILayout.Space();
		//	HTGUI.SepLine();

			// Add / Remove Prefabs (Create Horizontal Row)
			if( !Application.isPlaying ){

				// Start Horizontal Row
				EditorGUILayout.BeginHorizontal();

					// Flexible Space
					GUILayout.FlexibleSpace();	
										
					// Remove Button			
					if( o.spawnpointTransforms.Length == 0 ){ GUI.enabled = false; }			
					if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.removeButton, "Remove Last Spawn Point"), GUILayout.MaxWidth(32)) ) { 
						if( o.spawnpointTransforms.Length > 0 ){	// <- We must always have at least 1 condition!
							Undo.RecordObject ( o, "Remove Last Spawn Point");
							System.Array.Resize(ref o.spawnpointTransforms, o.spawnpointTransforms.Length - 1 );
						}
					}

					// Reset GUI Enabled
					GUI.enabled = true;
									
					// Add Button							
					if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.addButton, "Add New Spawn Point"), GUILayout.MaxWidth(32))) { 
						Undo.RecordObject ( o, "Add New Spawn Point");
						System.Array.Resize(ref o.spawnpointTransforms, o.spawnpointTransforms.Length + 1 ); 
						o.spawnpointTransforms[ o.spawnpointTransforms.Length - 1 ] = new Spawner.TransformSpawnPoint();	// <- Requires new instance!
					}

				// End Horizontal Row
				EditorGUILayout.EndHorizontal();

			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO POSITION LOCATION ARRAY
		//	Show the Position SpawnPoint List
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
		void DoPositionLocationArray(){

			// Add Seperator
			HTGUI.SepLine();
			EditorGUILayout.Space();

			// If we have spawnpointPositions setup, render them
			if( o.spawnpointPositions.Length > 0 ){

				// ------------------------
				//	PREFAB OBJECT LIST
				// ------------------------

				// Loop through the list
				for( int i = 0; i < o.spawnpointPositions.Length; i++ ){

					// Start Row
					GUILayout.BeginHorizontal();

						// Label
						GUILayout.Label( origin3dLabel, GUILayout.MaxWidth(20), GUILayout.MinHeight(20), GUILayout.MaxHeight(20) );
						GUILayout.Label( "Point " + (i+1) + ":", _locationGUIStyle, GUILayout.MinWidth(64), GUILayout.MaxWidth(64), GUILayout.MinHeight(20), GUILayout.MaxHeight(20));

						// Spawn Point Transform
						o.spawnpointPositions[i].spawnPoint = HTGUI_UNDO.Vector3Field( o, "Point " + (i+1).ToString(), o.spawnpointPositions[i].spawnPoint, new GUILayoutOption[]{ GUILayout.MinWidth(160) } );

						// Reset background color
						GUI.backgroundColor = Color.white;

						// Show Random Weights if we're using them
						if( o.spawnPointSelection == Spawner.SpawnPointSelection.RandomWithWeights ){

							// Label
							GUILayout.Label( diceLabel, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
							GUILayout.Label( "Chance:", _locationGUIStyle, GUILayout.MinWidth(50), GUILayout.MaxWidth(50), GUILayout.MaxHeight(20));

							// Show Slider
							o.spawnpointPositions[i].randomWeight = HTGUI_UNDO.SliderField( o, "Random Weight Chance", o.spawnpointPositions[i].randomWeight, 0f, 100f, new GUILayoutOption[]{ GUILayout.MinWidth(120), GUILayout.MinHeight(20) } );
						}

						// Only allow the prefab items to be moved when the game isn't running
						if( Application.isPlaying == false ){

							// Add some space 
							GUILayout.Space(8);

							// Move Up
							if( o.spawnpointPositions.Length > 0 && i != 0 ){
								if( GUILayout.Button( new GUIContent( System.String.Empty, upButton, "Move Spawn Point Up"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
								){
									Undo.RecordObject ( o, "Move Spawn Point Up" );
									Arrays.Shift( ref o.spawnpointPositions, i, true );
								}
							
							// Add space when we don't draw this
							} else if( o.spawnpointPositions.Length > 2 ){
								//GUILayout.Space(28);

								GUI.enabled = false;
								GUILayout.Button( new GUIContent( System.String.Empty, upButton, "Move Spawn Point Up"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) );
								GUI.enabled = true;
							}

							// Move Down
							if( o.spawnpointPositions.Length > 0 && i != o.spawnpointPositions.Length-1 ){

								if( GUILayout.Button( new GUIContent( System.String.Empty, downButton, "Move Spawn Point Down"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) )  
								){
									Undo.RecordObject ( o, "Move Spawn Point Down" );
									Arrays.Shift( ref o.spawnpointPositions, i, false );
								}
							
							// Add space when we don't draw this
							} else if( o.spawnpointPositions.Length > 2 ){
								//GUILayout.Space(28);

								GUI.enabled = false;
								GUILayout.Button( new GUIContent( System.String.Empty, downButton, "Move Spawn Point Down"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) );
								GUI.enabled = true;
							}

							// Remove (we must ensure at least 1 item exists)
							if( o.spawnpointPositions.Length > 1 &&
								GUILayout.Button( new GUIContent( System.String.Empty, HTGUI.removeButton, "Remove Spawn Point" ), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
							){
								Undo.RecordObject ( o, "Remove Spawn Point" );
								Arrays.RemoveItemAtIndex( ref o.spawnpointPositions, i );
							}

						}

					// Completely End Row
					GUILayout.EndHorizontal();
					GUILayout.Space(1);
				}
			
			// No Prefabs Set yet!
			} else {

				GUILayout.Label( new GUIContent( "   This Spawner can't spawn any instances because there are no spawn points setup yet.\n   Press the green '+' icon below to add the first spawn point!", prefabSmallIcon, _empty )  );
			}

			// Add Seperator
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			//HTGUI.SepLine();

			// Add / Remove Prefabs (Create Horizontal Row)
			if( !Application.isPlaying ){

				// Start Horizontal Row
				EditorGUILayout.BeginHorizontal();

					// Flexible Space
					GUILayout.FlexibleSpace();	
										
					// Remove Button			
					if( o.spawnpointPositions.Length == 0 ){ GUI.enabled = false; }			
					if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.removeButton, "Remove Last Spawn Point"), GUILayout.MaxWidth(32)) ) { 
						if( o.spawnpointPositions.Length > 0 ){	// <- We must always have at least 1 condition!
							Undo.RecordObject ( o, "Remove Last Spawn Point");
							System.Array.Resize(ref o.spawnpointPositions, o.spawnpointPositions.Length - 1 );
						}
					}

					// Reset GUI Enabled
					GUI.enabled = true;
									
					// Add Button							
					if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.addButton, "Add New Spawn Point"), GUILayout.MaxWidth(32))) { 
						Undo.RecordObject ( o, "Add New Spawn Point");
						System.Array.Resize(ref o.spawnpointPositions, o.spawnpointPositions.Length + 1 ); 
						o.spawnpointPositions[ o.spawnpointPositions.Length - 1 ] = new Spawner.PositionSpawnPoint();	// <- Requires new instance!
					}

				// End Horizontal Row
				EditorGUILayout.EndHorizontal();

			}
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
					HTGUI.SectionTitle( "Delegates & Events", "If you want this Spawner to allow delegates and events, check the box below." );
					EditorGUILayout.Space();

					// Enable Spawner Events
					o.enableSpawnerEvents = HTGUI_UNDO.ToggleField( o, "Enable Delegates & Events", new GUIContent( "Enable Delegates & Events:", gearLabel, _empty ), o.enableSpawnerEvents, defaultGUILayoutOptions );


				// Cleanly ends the Icon Column
				EndIconColumn();

				// Show help 
				if( _showHelpfulNotes ){

					// Show information about delegates here ...
					if ( o.enableSpawnerEvents ){
						DoHelpBox( "// You can Subsribe to this Spawner's Events like this:\n\n"+ 

							/*
							"Spawner spawner = PoolKit.GetSpawner(\string.Empty + o.spawnerName + "\");\n"+
							"if( spawner != null ){\n\n"+
							"     spawner.onSpawnerSpawn += onSpawnerSpawn;\n"+
							"     spawner.onSpawnerStart += onSpawnerStart;\n"+
							"     spawner.onSpawnerStop += onSpawnerStop;\n"+
							"     spawner.onSpawnerPause += onSpawnerPause;\n"+
							"     spawner.onSpawnerResume += onSpawnerResume;\n"+
							"     spawner.onSpawnerEnd += onSpawnerEnd;\n"+
							"}"
							*/
							"Spawner spawner = PoolKit.GetSpawner(\"" + o.spawnerName + "\");\nif( spawner != null ){\n\n     spawner.onSpawnerSpawn += onSpawnerSpawn;\n     spawner.onSpawnerStart += onSpawnerStart;\n     spawner.onSpawnerStop += onSpawnerStop;\n     spawner.onSpawnerPause += onSpawnerPause;\n     spawner.onSpawnerResume += onSpawnerResume;\n     spawner.onSpawnerEnd += onSpawnerEnd;\n}"
						);
					
					}
				}

				// Add Seperator
				EditorGUILayout.Space();
				HTGUI.SepLine();
			//	EditorGUILayout.Space();

				// Title
				HTGUI.SectionTitle( "Unity Events", "If you want this Spawner to send any UnityEvents, activate them below:" );
				EditorGUILayout.Space();

				// Enable UnityEvent - On Spawner Spawn
				o.enableUnityEventSpawn = HTGUI_UNDO.ToggleField( o, "Enable On Spawner Spawn", new GUIContent( "Enable On Spawner Spawn:", gearLabel, _empty ), o.enableUnityEventSpawn, defaultGUILayoutOptions );

				// Enable UnityEvent - On Spawner Start
				o.enableUnityEventStart = HTGUI_UNDO.ToggleField( o, "Enable On Spawner Start", new GUIContent( "Enable On Spawner Start:", gearLabel, _empty ), o.enableUnityEventStart, defaultGUILayoutOptions );

				// Enable UnityEvent - On Spawner Stop
				o.enableUnityEventStop = HTGUI_UNDO.ToggleField( o, "Enable On Spawner Stop", new GUIContent( "Enable On Spawner Stop:", gearLabel, _empty ), o.enableUnityEventStop, defaultGUILayoutOptions );

				// Enable UnityEvent - On Spawner Pause
				o.enableUnityEventPause = HTGUI_UNDO.ToggleField( o, "Enable On Spawner Pause", new GUIContent( "Enable On Spawner Pause:", gearLabel, _empty ), o.enableUnityEventPause, defaultGUILayoutOptions );

				// Enable UnityEvent - On Spawner Resume
				o.enableUnityEventResume = HTGUI_UNDO.ToggleField( o, "Enable On Spawner Resume", new GUIContent( "Enable On Spawner Resume:", gearLabel, _empty ), o.enableUnityEventResume, defaultGUILayoutOptions );

				// Enable UnityEvent - On Spawner End
				o.enableUnityEventEnd = HTGUI_UNDO.ToggleField( o, "Enable On Spawner End", new GUIContent( "Enable On Spawner End:", gearLabel, _empty ), o.enableUnityEventEnd, defaultGUILayoutOptions );

				// If we have enabled any of the UnityEvents, make the GUI cleaner by adding a seperator line
				if( o.enableUnityEventSpawn || o.enableUnityEventStart || o.enableUnityEventStop || o.enableUnityEventPause || o.enableUnityEventResume || o.enableUnityEventEnd 
				){ 
						EditorGUILayout.Space();
						HTGUI.SepLine();
						EditorGUILayout.Space();
				}

				// Reset _firstUnityEventDisplayed flag
				_firstUnityEventDisplayed = false;

				// Draw the events
				if( o.enableUnityEventSpawn ){ FormatUnityEvent( "OnSpawnerSpawnUnityEvent", "On Spawner Spawn", "This UnityEvent is fired when a new instance is spawned. The Transform of the instance is sent as a parameter.", "This UnityEvent may have a small impact on performance. Only enable it if it is needed!" ); }

				if( o.enableUnityEventStart ){ FormatUnityEvent( "OnSpawnerStartUnityEvent", "On Spawner Start", "This UnityEvent is fired when the Spawner starts spawning instances." ); }

				if( o.enableUnityEventStop ){ FormatUnityEvent( "OnSpawnerStopUnityEvent", "On Spawner Stop", "This UnityEvent is fired when the Spawner stops spawning instances." ); }

				if( o.enableUnityEventPause ){ FormatUnityEvent( "OnSpawnerPauseUnityEvent", "On Spawner Pause", "This UnityEvent is fired when the Spawner is paused." ); }

				if( o.enableUnityEventResume ){ FormatUnityEvent( "OnSpawnerResumeUnityEvent", "On Spawner Resume", "This UnityEvent is fired when the Spawner is resumed." ); }

				if( o.enableUnityEventEnd ){ FormatUnityEvent( "OnSpawnerEndUnityEvent", "On Spawner End", "This UnityEvent is fired when the Spawner has finished spawning and come to an end." ); }


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









