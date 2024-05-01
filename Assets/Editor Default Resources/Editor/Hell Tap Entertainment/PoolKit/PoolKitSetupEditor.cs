using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using HellTap.PoolKit;
using System.Reflection;

namespace HellTap.PoolKit {

	[CustomEditor(typeof(PoolKitSetup))]
	public class PoolKitSetupEditor : Editor {

		// Static Icons
		static Texture2D headerIcon, buttonIcon, unityIcon, bugIcon, deleteIcon, gearIcon;
		//, buttonLabel, widthIcon, deleteIcon, hideIcon, upButton, downButton, gameObjectCustomIcon;

		// Helpers
		bool _hasGlobalPoolsAsChildren = false;
		bool _hasLocalPoolsAsChildren = false;

		// =======================
		//	CORE EDITOR VARIABLES
		// =======================

		// Main Object
		PoolKitSetup o;
		
		// Helpers for tabs
		//string[] tabLabels = new string[]{"GUI", "Content", "References"};
		//Texture2D[] tabIcons = new Texture2D[]{ buttonLabel, buttonIcon, cubeIcon };

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	ON ENABLE
		//	Caches all editor icons, subscribes to delegates, and performs GameObject checks
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		void OnEnable(){

			// Cache the icons
			headerIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/PoolKit.png") as Texture2D;
			buttonIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/buttonLabel.png") as Texture2D;
			unityIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/unityLabel.png") as Texture2D;
			bugIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/bugLabel.png") as Texture2D;
			deleteIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/deleteLabel.png") as Texture2D;
			gearIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/gearLabel.png") as Texture2D;

			/*
			buttonLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/buttonIconLabel.png") as Texture2D;
			widthIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/widthLabel.png") as Texture2D;
			deleteIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/deleteLabel.png") as Texture2D;
			hideIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/hideLabel.png") as Texture2D;
			upButton = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/upButton.png") as Texture2D;
			downButton = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/downButton.png") as Texture2D;
			*/


			// Check to see if any of this object's children are pools ...
			_hasGlobalPoolsAsChildren = false;
			_hasLocalPoolsAsChildren = false;

			// Loop through the children ...
			if( Selection.activeGameObject != null ){
				for( int i = 0; i < Selection.activeGameObject.transform.childCount; i++ ){
					if( Selection.activeGameObject.transform.GetChild(i) != null &&
						Selection.activeGameObject.transform.GetChild(i).GetComponent<Pool>() != null
					){
						if( Selection.activeGameObject.transform.GetChild(i).GetComponent<Pool>().dontDestroyOnLoad == true ){
							_hasGlobalPoolsAsChildren = true;
						} else if ( Selection.activeGameObject.transform.GetChild(i).GetComponent<Pool>().dontDestroyOnLoad == false ){
							_hasLocalPoolsAsChildren = true;
						}
					}
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO HELP BOX
		//	Quick way to make a help box with the correct color tints
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		void DoHelpBox( string text, MessageType messageType = MessageType.Info, bool wide = true, bool addTopSpace = false, bool addBottomSpace = false ){
			
			// Add space
			if(addTopSpace){ EditorGUILayout.Space(); }

			// If this is the light skin, give the help box a yellow background
			if( EditorGUIUtility.isProSkin == false ){ GUI.backgroundColor = new Color(1f,1f,0f,0.333f); }

			// Show Help Box
			EditorGUILayout.HelpBox( "\n" + text + "\n", messageType, wide );

			// Restore Background Color
			if( EditorGUIUtility.isProSkin == false ){  GUI.backgroundColor = Color.white; }

			// Add space
			if(addBottomSpace){ EditorGUILayout.Space(); }
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	ON INSPECTOR GUI
		//	Core Update Method
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Local Preferences
		bool _showPoolKitHeaders = true;
		bool _showHelpfulNotes = true;

		// Method
		public override void OnInspectorGUI(){

			// Cache the Editor preferences for showing helpful notes and tab titles
			_showPoolKitHeaders = EditorPrefs.GetBool ("PoolKit_Editors_ShowPoolKitHeaders", true );
			_showHelpfulNotes = EditorPrefs.GetBool ("PoolKit_Editors_ShowHelpfulNotes", true );

			// Render Header
			if( _showPoolKitHeaders){
				HTGUI.Header(headerIcon, "PoolKit Setup", "This tool allows you to update PoolKit settings and to create a group of global pools.", "PoolKit", 80, 13
					);
			} else {
				EditorGUILayout.Space();
			}

			// Cache the target correctly
			if( target as PoolKitSetup != null ){ o = target as PoolKitSetup; }

			// Start Body Template
			if( o != null ){

				// Do Main Tabs (this editor doesn't need main tabs)
				/*
				o.tab = HTGUI.HTSelectionGrid( null, o.tab, new GUIContent[]{ 
								
								new GUIContent("  "+tabLabels[0], tabIcons[0], ""),
								new GUIContent("  "+tabLabels[1], tabIcons[1], ""),
								new GUIContent("  "+tabLabels[2], tabIcons[2], "") 
					
							}, 3, true );
				*/
							

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
		
		void DoContent(){

			// Add Seperator Line
			//HTGUI.SepLine();

			// Do the Interaction Properties
			DoSettings();

			// If we changed anything in the GUI, mark the object as dirty (so it can be saved)
			if( GUI.changed ){
				EditorUtility.SetDirty( o );
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO SETTINGS
		//	Display the Editor fields for the PoolKitSetup
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void DoSettings(){

			// =================
			//	INTERACT SETUP
			// =================

			// Start White Box
			HTGUI.StartWhiteBox();

				// Title
				HTGUI.SectionTitle( "PoolKit Setup", "Setup the default PoolKit settings." );
				EditorGUILayout.Space();

				// Rename Objects In Pool
				o.updatePoolKitSettings = HTGUI_UNDO.ToggleField( o, "Update PoolKit Settings", gearIcon, "Update PoolKit Settings:", o.updatePoolKitSettings);

				// Disable GUI if we're not updating the PoolKit Settings
				if(!o.updatePoolKitSettings){ GUI.enabled = false; }

				// Rename Objects In Pool
				o.renameObjectsInPool = (PoolKit.RenameFormat)HTGUI_UNDO.EnumField( o, "Rename Objects In Pool", buttonIcon, "Rename objects in Pools:", o.renameObjectsInPool);

				// Show the next option only if the user hasn't selected "No Renaming"
				if( o.renameObjectsInPool != PoolKit.RenameFormat.NoRenaming ){
					
					// Only Rename Objects In Editor
					o.onlyRenameObjectsInEditor = HTGUI_UNDO.ToggleField( o, "Only Rename Objects In Editor", unityIcon, "Only Rename In Editor:", o.onlyRenameObjectsInEditor);

					// Show helpful notes
					if( _showHelpfulNotes && !o.onlyRenameObjectsInEditor ){
						DoHelpBox("Please note that frequently renaming GameObjects may impact performance. It is recommended to only do this within the Editor to not affect builds at runtime.");
						EditorGUILayout.Space();
					}
				}

				// Only Rename Objects In Editor
				o.debugPoolKit = HTGUI_UNDO.ToggleField( o, "Show Debug Messages", bugIcon, "Show Debug Messages:", o.debugPoolKit);

				// Enable GUI at this point
				if(!o.updatePoolKitSettings){ GUI.enabled = true; }

				EditorGUILayout.Space();
				HTGUI.SepLine();

				// Title
				HTGUI.SectionTitle( "Global Pool Group", "You can choose to use this as a container for your global pools by not destroying this GameObject when changing scenes. Global Pool Groups work great when saved as a prefab and placed in the first scene of your game.\n\nIf you want to setup a 'Global Pool Group' you should make sure that all Pools that are children of this GameObject are set to be 'global' by checking the 'Dont Destroy On Load' option below and on all of the child pools."
				 );
				EditorGUILayout.Space();

				// Only Rename Objects In Editor
				o.dontDestroyOnLoad = HTGUI_UNDO.ToggleField( o, "Don't Destroy GameObject On Load", deleteIcon, "Don't Destroy On Load:", o.dontDestroyOnLoad);

				// ------
				//	HELP
				// ------

				// If there are global pools (and NO local ones), tell the user they should activate dontDestroyOnLoad.
				if( _hasGlobalPoolsAsChildren && !_hasLocalPoolsAsChildren && o.dontDestroyOnLoad == false ){
					DoHelpBox( "This Pool Group is the parent of a global Pool. For this to work correctly, you should enable 'Don't Destroy GameObject On Load'.", MessageType.Error );
				
				// If there are NO global pools but there is local ones, tell the user they should uncheck dontDestroyOnLoad.
				} else if( !_hasGlobalPoolsAsChildren && _hasLocalPoolsAsChildren && o.dontDestroyOnLoad == true ){
					DoHelpBox( "This is the parent of a local Pool but is setup as a 'Global Pool Group'. If you want to destroy the local pools when changing scenes, you should disable 'Don't Destroy On Load'. Please note the current configuration will force the child pools to become global at runtime.", MessageType.Warning );

				// If there are BOTH local and global pools, let the user know this is going to mess things up.
				} else if( _hasGlobalPoolsAsChildren && _hasLocalPoolsAsChildren ){
					DoHelpBox( "This Global Pool Group is the parent of BOTH local and global Pools. This is likely to cause problems. They should either all be local or all be global. You can fix this by making all child pools global or to create two Pool groups; one for local pools and one for global pools.", MessageType.Error );
				
				// If this is a Global Pool Group but there are NO pools ...
				} else if( o.dontDestroyOnLoad == true && !_hasGlobalPoolsAsChildren && !_hasLocalPoolsAsChildren ){
					DoHelpBox( "This GameObject is setup as a 'Global Pool Group' but no Pools are children of this Transform.", MessageType.Warning );
				}

				// Add Space at the bottom
				EditorGUILayout.Space();

			// End White Box
			HTGUI.EndWhiteBox();

			// Add Space at the bottom
			EditorGUILayout.Space();
			
		}
	
	}
}









