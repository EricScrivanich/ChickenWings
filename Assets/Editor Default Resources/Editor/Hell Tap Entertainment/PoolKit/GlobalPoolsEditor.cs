using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using HellTap.PoolKit;
using System.Reflection;

namespace HellTap.PoolKit {

	[CustomEditor(typeof(GlobalPools))]
	public class GlobalPoolsEditor : Editor {

		// Static Icons
		static Texture2D headerIcon, prefabSmallIcon, upButton, downButton;
		//, buttonLabel, widthIcon, deleteIcon, hideIcon, upButton, downButton, gameObjectCustomIcon;

		// =======================
		//	CORE EDITOR VARIABLES
		// =======================

		// Main Object
		GlobalPools o;
		
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
			prefabSmallIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/PrefabSmall.png") as Texture2D;
			upButton = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/upButton.png") as Texture2D;
			downButton = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/downButton.png") as Texture2D;

		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO HELP BOX
		//	Quick way to make a help box with the correct color tints
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		void DoHelpBox( string text, MessageType messageType = MessageType.Info, bool wide = true, bool addTopSpace = false, bool addBottomSpace = false ){
			
			// Let the HT_GUIStyles class handle help boxes now
			HT_GUIStyles.DoHelpBox( text, messageType, wide, addTopSpace, addBottomSpace );
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
			if(_showHelpfulNotes){ /* This removes console warnings*/ }

			// Render Header
			if( _showPoolKitHeaders){
				HTGUI.Header(headerIcon, "PoolKit Global Pools", "Automatically create a list of global pools when the game starts.", "PoolKit", 80, 13
					);
			} else {
				EditorGUILayout.Space();
			}

			// Cache the target correctly
			if( target as GlobalPools != null ){ o = target as GlobalPools; }

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
		//	Display the Editor fields for the GlobalPools
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Helpers
		GUILayoutOption[] defaultGUILayoutOptions = new GUILayoutOption[]{ GUILayout.MinWidth(300), GUILayout.MaxWidth(400)};
		bool _foundProblemInList = false;
		bool _allowWarningsThisFrame = true;

		// Method
		void DoSettings(){

			// =================
			//	INTERACT SETUP
			// =================

			// Start White Box
			HTGUI.StartWhiteBox();

				// Title
				HTGUI.SectionTitle( "Global Pools", "Drag and drop prefabs of your global pools to automatically create them when the game starts." );
				EditorGUILayout.Space();


				// Show Helpful Notes
				if(_showHelpfulNotes){
					
					DoHelpBox( "You can easily create global pools by selecting 'GameObject > PoolKit > Create Global Pool' in the menu. Save the GameObject as a prefab and drag it into the list below." );
					
					EditorGUILayout.Space();
					HTGUI.SepLine();
					EditorGUILayout.Space();
			
				} else {

					EditorGUILayout.Space();

				}


				// Loop through the global Pools
				if( o.globalPools.Length > 0 ){
					for( int i = 0; i < o.globalPools.Length; i++ ){

						// Show GUI Warning color
						if( o.globalPools[i] == null ||
							o.globalPools[i] != null && o.globalPools[i].GetComponent<Pool>() == null ||
							o.globalPools[i] != null && o.globalPools[i].GetComponent<Pool>() != null &&
									o.globalPools[i].GetComponent<Pool>().dontDestroyOnLoad == false
						){
							if( EditorGUIUtility.isProSkin == false ){ 
								GUI.backgroundColor = new Color( 1f, 0.25f, 0.25f, 0.25f);
							} else {
								GUI.backgroundColor = new Color( 1f, 0f, 0f, 1f);
							}
							
						}


						// Start Horizontal Row
						EditorGUILayout.BeginHorizontal();

							o.globalPools[i] = (GameObject)HTGUI_UNDO.ObjectField( o, "Pool Prefab " + (i+1).ToString(), new GUIContent("Pool Prefab " + (i+1).ToString() + ": ", prefabSmallIcon, System.String.Empty), o.globalPools[i], false, defaultGUILayoutOptions );
					

							// Reset GUI Color
							GUI.backgroundColor = Color.white;

							// Only allow the prefab items to be moved when the game isn't running
							if( Application.isPlaying == false ){

								// This will be set if we do an operation that changes the list
								_allowWarningsThisFrame = true;

								// Add some space 
								GUILayout.Space(8);

								// Move Up
								if( o.globalPools.Length > 0 && i != 0 ){
									if( GUILayout.Button( new GUIContent( System.String.Empty, upButton, "Move Global Pool Up"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
									){
										Undo.RecordObject ( o, "Move Global Pool Up" );
										Arrays.Shift( ref o.globalPools, i, true );
										_allowWarningsThisFrame = false;
									}
								
								// Add space when we don't draw this
								} else if( o.globalPools.Length > 2 ){
									//GUILayout.Space(28);

									GUI.enabled = false;
									GUILayout.Button( new GUIContent( System.String.Empty, upButton, "Move Global Pool Up"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) );
									GUI.enabled = true;
								}

								// Move Down
								if( o.globalPools.Length > 0 && i != o.globalPools.Length-1 ){

									if( GUILayout.Button( new GUIContent( System.String.Empty, downButton, "Move Global Pool Down"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) )  
									){
										Undo.RecordObject ( o, "Move Global Pool Down" );
										Arrays.Shift( ref o.globalPools, i, false );
										_allowWarningsThisFrame = false;
									}
								
								// Add space when we don't draw this
								} else if( o.globalPools.Length > 2 ){
									//GUILayout.Space(28);

									GUI.enabled = false;
									GUILayout.Button( new GUIContent( System.String.Empty, downButton, "Move Global Pool Down"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) );
									GUI.enabled = true;
								}

								// Remove (we must ensure at least 1 item exists)
								if( o.globalPools.Length > 1 &&
									GUILayout.Button( new GUIContent( System.String.Empty, HTGUI.removeButton, "Remove Global Pool" ), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
								){
									Undo.RecordObject ( o, "Remove Global Pool" );
									Arrays.RemoveItemAtIndex( ref o.globalPools, i );
									_allowWarningsThisFrame = false;
								}

							}


						// End Horizontal Row
						EditorGUILayout.EndHorizontal();

						// ----------
						//	WARNINGS
						// ----------

						if( _allowWarningsThisFrame ){ 

							// Prefab is empty
							if(	o.globalPools[i] == null ){
								//DoHelpBox( "This prefab has not been set! This pool will be skipped.", MessageType.Warning );
								//EditorGUILayout.Space();

							// Prefab doesn't have a pool
							} else if(	o.globalPools[i] != null && o.globalPools[i].GetComponent<Pool>() == null ){
								DoHelpBox( "This prefab does not have a Pool component! This pool will be skipped.", MessageType.Warning );
								EditorGUILayout.Space();
							
							// Pool is not global
							} else if(	o.globalPools[i] != null && o.globalPools[i].GetComponent<Pool>() != null &&
										o.globalPools[i].GetComponent<Pool>().dontDestroyOnLoad == false
							){
								DoHelpBox( "The pool prefab '" + o.globalPools[i].name + "' is not configured to be global. Make sure 'Dont Destroy On Load' is enabled. This pool will be skipped.", MessageType.Warning );
								EditorGUILayout.Space();
							}
						}

					}

				// No Pools have been setup	
				} else {
					GUILayout.Label( new GUIContent( "   There are no global pools setup yet.\n   Press the green '+' icon below to add the first prefab!", prefabSmallIcon, "" )  );
				}

				EditorGUILayout.Space();
				EditorGUILayout.Space();
				HTGUI.SepLine();

				// Add / Remove Pool Items (Create Horizontal Row)
				if( !Application.isPlaying ){

					// Start Horizontal Row
					EditorGUILayout.BeginHorizontal();

						// Flexible Space
						GUILayout.FlexibleSpace();	
											
						// Remove Button			
						if( o.globalPools.Length == 0 ){ GUI.enabled = false; }			
						if( GUILayout.Button( new GUIContent( "", HTGUI.removeButton, "Remove Last Global Pool"), GUILayout.MaxWidth(32)) ) { 
							if( o.globalPools.Length > 0 ){	// <- We must always have at least 1 condition!
								Undo.RecordObject ( o, "Remove Last Global Pool");
								System.Array.Resize(ref o.globalPools, o.globalPools.Length - 1 );
							}
						}

						// Reset GUI Enabled
						GUI.enabled = true;
										
						// Add Button							
						if( GUILayout.Button( new GUIContent( "", HTGUI.addButton, "Add New Global Pool"), GUILayout.MaxWidth(32))) { 
							Undo.RecordObject ( o, "Add New Global Pool");
							System.Array.Resize(ref o.globalPools, o.globalPools.Length + 1 ); 
							o.globalPools[ o.globalPools.Length - 1 ] = null;
						}

					// End Horizontal Row
					EditorGUILayout.EndHorizontal();

				}

				// Only check for duplicates when the player isn't running ...
				if( Application.isPlaying == false ){

					_foundProblemInList = false;

					// Loop through the list twice
					for( int a = 0; a < o.globalPools.Length; a++ ){
						for( int b = 0; b < o.globalPools.Length; b++ ){

							// Duplicate prefabs
							if( a != b && o.globalPools[a] != null && o.globalPools[b] != null 
								&& o.globalPools[a] == o.globalPools[b] 
							){

								// Mark this as a problem
								_foundProblemInList = true;

								// Show Message
								EditorGUILayout.Space();
								DoHelpBox( "The Pool Prefabs " + (a+1).ToString() + " and " + (b+1).ToString() + " are duplicates! Please remove one of them as this configuration is likely to cause problems.", MessageType.Error );

								// break the loop
								break;

							// If we're not checking against the same prefab, and the pool names match ...
							} else if ( 
								o.globalPools[a] != null && o.globalPools[b] != null && o.globalPools[a] != o.globalPools[b] && 
								o.globalPools[a].GetComponent<Pool>() != null && o.globalPools[b].GetComponent<Pool>() != null &&
								o.globalPools[a].GetComponent<Pool>().poolName == o.globalPools[b].GetComponent<Pool>().poolName 
							){
								
								// Mark this as a problem
								_foundProblemInList = true;

								// Show Message
								EditorGUILayout.Space();
								DoHelpBox( "The Pool Prefabs " + (a+1).ToString() + " and " + (b+1).ToString() + " are using the Pool Name '" + o.globalPools[a].GetComponent<Pool>().poolName + "'. Every pool should have a unique Pool Name so you can access them properly.", MessageType.Error );

								// break the loop
								break;
							}
						}

						// If we've found a problem, break out of the outer loop as well
						if( _foundProblemInList ){ break; }
					}
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









