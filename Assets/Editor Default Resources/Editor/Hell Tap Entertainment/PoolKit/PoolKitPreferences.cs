//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  PoolKitPreferences.cs
//
//	Unity Preferences For PoolKit.
//
//	PoolKit For Unity, Created By Melli Georgiou
//	© 2018 Hell Tap Entertainment LTD
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

// Use HellTap Namespace
namespace HellTap.PoolKit {

	// Class
	public class PoolKitPreferences {

		// ==============================================================================================
		//	PREFERENCES FIX FOR UNITY 2019.3 AND HIGHER
		// ==============================================================================================

		#if UNITY_2019_3_OR_NEWER
		
			private class HellTapSettingsProvider : SettingsProvider {

				// Core Constructor
				public HellTapSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) {}

				// On GUI Override that redirects the Settings Provider to use the original GUI Method
				public override void OnGUI(string searchContext){ PreferencesGUI(); }
			}
		 
			[SettingsProvider]
			static SettingsProvider HellTapPreferences(){ return new HellTapSettingsProvider("Preferences/PoolKit"); }

		#endif	

		// ==============================================================================================
		//	STANDARD PREFERENCES
		// ==============================================================================================

		// Have we loaded the prefs yet
		private static bool prefsLoaded = false;
		
		// The Preferences (Editors)
		public static bool showPoolKitHeaders = true;
		public static bool showTabHeaders = true;
		public static bool showHelpfulNotes = true;

		// The Preferences (Spawner)
		public static bool onlyShowWhenSelected = true;
		public static bool scaleGizmos = true;
		public static bool showSpawnPointLabels = true;
		public static bool showSpawnerName = true;
		public static float lineWidth = 2f;
		public static Color lineColor = new Color(0f,1f,1f,1f);

		// GUI Layout
		readonly static GUILayoutOption[] guiLayoutOptions = new GUILayoutOption[]{ GUILayout.Width(340), GUILayout.MinWidth(340), GUILayout.ExpandWidth(true) };
		
		// ==============================================================================================
		//	PREFERENCES GUI
		// ==============================================================================================

		// The PreferenceItem attribute is deprecated in Unity 2019 and later.
		#if !UNITY_2019_3_OR_NEWER
			[PreferenceItem ("PoolKit")]
		#endif	
		static void PreferencesGUI () {
			
			// ----------------------
			//	LOAD PREFERENCES
			// ----------------------

			if (!prefsLoaded) { LoadSettings(); }
				
			
			// ----------------------
			//	PREFERENCES GUI
			// ----------------------

			// PoolKit Inspectors
			GUILayout.Label("PoolKit Editors", "BoldLabel");
			GUILayout.Label("Setup how inspectors display information in the Editor.");
			GUILayout.Space(4);
			showPoolKitHeaders = EditorGUILayout.Toggle ("Show PoolKit Headers", showPoolKitHeaders, guiLayoutOptions );
			showTabHeaders = EditorGUILayout.Toggle ("Show Tab Headers", showTabHeaders, guiLayoutOptions );
			showHelpfulNotes = EditorGUILayout.Toggle ("Show Helpful Notes", showHelpfulNotes, guiLayoutOptions );

			// Initial Space
			GUILayout.Space(8);

			// Live Asset Tracking
			GUILayout.Label("PoolKit Auto-Spawner", "BoldLabel");
			GUILayout.Label("Setup how Spawners display information in the Scene view.");
			GUILayout.Space(4);
			onlyShowWhenSelected = EditorGUILayout.Toggle ("Only Show When Selected", onlyShowWhenSelected, guiLayoutOptions );
			scaleGizmos = EditorGUILayout.Toggle ("Scale Gizmo Icons", scaleGizmos, guiLayoutOptions );
			showSpawnPointLabels = EditorGUILayout.Toggle ("Show Spawnpoint Labels", showSpawnPointLabels, guiLayoutOptions );
			showSpawnerName = EditorGUILayout.Toggle ("Show Name Of Spawner", showSpawnerName, guiLayoutOptions );
			lineWidth = EditorGUILayout.FloatField ("UI Line Width", lineWidth, guiLayoutOptions );
			lineColor = EditorGUILayout.ColorField ("UI Line And Label Color", lineColor, guiLayoutOptions );
			GUILayout.Space(8);

			// ----------------------
			//	SAVE CHANGES
			// ----------------------

			if (GUI.changed){
				EditorPrefs.SetBool ("PoolKit_Editors_ShowPoolKitHeaders", showPoolKitHeaders );
				EditorPrefs.SetBool ("PoolKit_Editors_ShowTabHeaders", showTabHeaders );
				EditorPrefs.SetBool ("PoolKit_Editors_ShowHelpfulNotes", showHelpfulNotes );
				EditorPrefs.SetBool ("PoolKit_Spawner_OnlyShowWhenSelected", onlyShowWhenSelected );
				EditorPrefs.SetBool ("PoolKit_Spawner_ScaleGizmos", scaleGizmos);
				EditorPrefs.SetBool ("PoolKit_Spawner_ShowSpawnPointLabels", showSpawnPointLabels);
				EditorPrefs.SetBool ("PoolKit_Spawner_ShowSpawnerName", showSpawnerName);
				EditorPrefs.SetFloat ("PoolKit_Spawner_LineWidth", lineWidth);
				EditorPrefs.SetFloat ("PoolKit_Spawner_LineColorR", lineColor.r );
				EditorPrefs.SetFloat ("PoolKit_Spawner_LineColorG", lineColor.g );
				EditorPrefs.SetFloat ("PoolKit_Spawner_LineColorB", lineColor.b );
				EditorPrefs.SetFloat ("PoolKit_Spawner_LineColorA", lineColor.a );
			}
			
			// ----------------------
			//	RESET SETTINGS
			// ----------------------

			GUILayout.Label("Reset Default Settings", "BoldLabel");
			GUILayout.Label("Click the button below to restore default settings.");
			if( GUILayout.Button( "Reset PoolKit Preferences" ) ){

				// Save the Settings
				EditorPrefs.SetBool ("PoolKit_Editors_ShowPoolKitHeaders", true );
				EditorPrefs.SetBool ("PoolKit_Editors_ShowTabHeaders", true );
				EditorPrefs.SetBool ("PoolKit_Editors_ShowHelpfulNotes", true );
				EditorPrefs.SetBool ("PoolKit_Spawner_OnlyShowWhenSelected", true );
				EditorPrefs.SetBool ("PoolKit_Spawner_ScaleGizmos", true );
				EditorPrefs.SetBool ("PoolKit_Spawner_ShowSpawnPointLabels", true );
				EditorPrefs.SetBool ("PoolKit_Spawner_ShowSpawnerName", true );
				EditorPrefs.SetFloat ("PoolKit_Spawner_LineWidth", 2f );
				EditorPrefs.SetFloat ("PoolKit_Spawner_LineColorR", 0f );
				EditorPrefs.SetFloat ("PoolKit_Spawner_LineColorG", 1f );
				EditorPrefs.SetFloat ("PoolKit_Spawner_LineColorB", 1f );
				EditorPrefs.SetFloat ("PoolKit_Spawner_LineColorA", 1f );

				// Reload Settings
				LoadSettings();
			}
		}

		// ==============================================================================================
		//	LOAD SETTINGS
		// ==============================================================================================

		static void LoadSettings() {

			// Load in the settings
			showPoolKitHeaders = EditorPrefs.GetBool ("PoolKit_Editors_ShowPoolKitHeaders", true );
			showTabHeaders = EditorPrefs.GetBool ("PoolKit_Editors_ShowTabHeaders", true );
			showHelpfulNotes = EditorPrefs.GetBool ("PoolKit_Editors_ShowHelpfulNotes", true );
			onlyShowWhenSelected = EditorPrefs.GetBool ("PoolKit_Spawner_OnlyShowWhenSelected", true );
			scaleGizmos = EditorPrefs.GetBool ("PoolKit_Spawner_ScaleGizmos", true );
			showSpawnPointLabels = EditorPrefs.GetBool ("PoolKit_Spawner_ShowSpawnPointLabels", true );
			showSpawnerName = EditorPrefs.GetBool ("PoolKit_Spawner_ShowSpawnerName", true );
			lineWidth = EditorPrefs.GetFloat ("PoolKit_Spawner_LineWidth", 2f );
			lineColor = new Color 	(
										EditorPrefs.GetFloat ("PoolKit_Spawner_LineColorR", 0f ),
										EditorPrefs.GetFloat ("PoolKit_Spawner_LineColorG", 1f ),
										EditorPrefs.GetFloat ("PoolKit_Spawner_LineColorB", 1f ),
										EditorPrefs.GetFloat ("PoolKit_Spawner_LineColorA", 1f )
									);

			// Mark Prefs As Loaded After loading settings
			prefsLoaded = true;
		}
	}
}