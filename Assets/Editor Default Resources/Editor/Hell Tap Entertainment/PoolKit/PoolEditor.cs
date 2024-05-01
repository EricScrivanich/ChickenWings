using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using HellTap.PoolKit;
using System.Reflection;

namespace HellTap.PoolKit {

	[CustomEditor(typeof(Pool))]
	public class PoolEditor : Editor {

		// Static Icons
		static Texture2D headerIcon, prefabIcon, prefabSmallIcon, poolIcon, poolSmallIcon, analyticsIcon, AnalyticsSmallIcon, buttonIcon, nextLabel, shieldLabel, cubeLabel, loadLabel, widthLabel, groupLabel, loopLabel, layersLabel, resizeLabel, timeLabel, deleteIcon, hideIcon, upButton, downButton, gearIcon, foldoutDown, foldoutRight;

		// =======================
		//	CORE EDITOR VARIABLES
		// =======================

		// Main Object
		Pool o;

		// Helpers for tabs
		GUIContent[] tabGUIContent = new GUIContent[]{ new GUIContent(), new GUIContent(), new GUIContent()  };
		string[] tabLabels = new string[]{ "  Pool", "  Prefabs", " Statistics" };
		Texture2D[] tabIcons = new Texture2D[]{ poolSmallIcon, prefabSmallIcon, AnalyticsSmallIcon };
		
		// Helpers for sub tabs
		string[] subtabLabels = new string[]{ "  Prefab", "  Instances", "  Features", "  Advanced" };
		Texture2D[] subtabIcons = new Texture2D[]{ cubeLabel, poolSmallIcon, loadLabel, gearIcon };

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	ON ENABLE
		//	Caches all editor icons, subscribes to delegates, and performs GameObject checks
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		void OnEnable(){

			// Cache the icons
			headerIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/PoolKit.png") as Texture2D;
			prefabIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/Prefab.png") as Texture2D;
			prefabSmallIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/PrefabSmall.png") as Texture2D;
			poolIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/Pool.png") as Texture2D;
			poolSmallIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/poolSmall.png") as Texture2D;
			analyticsIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/Analytics.png") as Texture2D;
			AnalyticsSmallIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Headers/AnalyticsSmall.png") as Texture2D;
			buttonIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/buttonLabel.png") as Texture2D;
			nextLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/nextLabel.png") as Texture2D;
			shieldLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/shieldLabel.png") as Texture2D;
			cubeLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/cubeLabel.png") as Texture2D; 
			loadLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/loadLabel.png") as Texture2D;
			widthLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/widthLabel.png") as Texture2D;
			groupLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/groupIcon.png") as Texture2D;
			loopLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/loopLabel.png") as Texture2D;
			layersLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/layersLabel.png") as Texture2D;
			resizeLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/resizeLabel.png") as Texture2D;
			timeLabel = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/timeLabel.png") as Texture2D;
			deleteIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/deleteLabel.png") as Texture2D;
			hideIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/hideLabel.png") as Texture2D;
			gearIcon = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/gearLabel.png") as Texture2D;
			upButton = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/upButton.png") as Texture2D;
			downButton = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Generics/downButton.png") as Texture2D;

			// Cache the foldout icons
			foldoutDown = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Shared/foldoutDown.png") as Texture2D;
			foldoutRight = EditorGUIUtility.Load("Hell Tap Entertainment/PoolKit/Shared/foldoutRight.png") as Texture2D;

			// Check foldout icons to remove warnings in the editor
			if( foldoutDown != null && foldoutRight != null ){}

			// Subscribe To Editor Update
			EditorApplication.update += EditorUpdate;

			// ------------------------
			// Update core tab values
			// ------------------------

			// Helpers for tabs
			tabGUIContent = new GUIContent[]{ new GUIContent(), new GUIContent(), new GUIContent()  };
			tabLabels = new string[]{ "  Pool", "  Prefabs", " Statistics" };
			tabIcons = new Texture2D[]{ poolSmallIcon, prefabSmallIcon, AnalyticsSmallIcon };
		
			// Helpers for sub tabs
			subtabLabels = new string[]{ "  Prefab", "  Instances", "  Features", "  Advanced" };
			subtabIcons = new Texture2D[]{ cubeLabel, poolSmallIcon, loadLabel, gearIcon };
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	ON DISABLE
		//	unsubscribes from delegates
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		void OnDisable(){

			// Unsubscribe from Editor Update
			EditorApplication.update -= EditorUpdate;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	EDITOR UPDATE
		//	Allows us to have a continuous update loop in the inspector
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		void EditorUpdate(){

			// Keep repainting the editor only when we're looking at statistics
			if( o != null && o.tab == 2){
				Repaint();
			}
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

			// Render Header
			if( _showPoolKitHeaders ){
				HTGUI.Header(headerIcon, "Pool", "This tool allows you to setup a new PoolKit Pool.", "PoolKit", 80, 13);
			} else {
				EditorGUILayout.Space();
			}

			// Cache the target correctly
			if( target as Pool != null ){ o = target as Pool; }

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

			// Cache the Editor preferences for showing helpful notes and tab titles
			_showTabHeaders = EditorPrefs.GetBool ("PoolKit_Editors_ShowTabHeaders", true );
			_showHelpfulNotes = EditorPrefs.GetBool ("PoolKit_Editors_ShowHelpfulNotes", true );

			// Do the Pool Settings
			if(o.tab == 0 ){ DoPoolSettings(); }

			// Do the Pool Items
			else if(o.tab == 1 ){ DoPoolItems(); }

			// Do the Statistics
			else if(o.tab == 2 ){ DoPoolStatistics(); }

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
		//	GET POOL SUMMARY
		//	A short summary of how this pool works is created for the user
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		string _poolSummaryString = string.Empty;
		string GetPoolSummary(){

			// First, get the information about the Pool Type
			if( o.poolType == Pool.PoolType.FixedArray ){
				_poolSummaryString = "The Pool '" + o.poolName + "' uses a Fixed Array which has the best performance but cannot be resized. This means the number of pooled objects are fixed and lazy preloading cannot be used.";
			
			} else if( o.poolType == Pool.PoolType.DynamicList ){
				_poolSummaryString = "The Pool '" + o.poolName + "' uses a Dynamic List which is slightly slower than Fixed Arrays but are more flexible. This means the number of pooled objects can grow over time and features like lazy preloading can be used.";
			
			} else if( o.poolType == Pool.PoolType.Automatic ){
				_poolSummaryString = "The Pool '" + o.poolName + "' is set to Automatic which selects what is best based on the features you enable.";
			}


			// If we're using Pool Protection
			if( o.enablePoolProtection ){
				_poolSummaryString += "\n\nPool Protection is enabled which comes at a slight performance cost. This can automatically rebuild the pool if you accidentally destroy an instance instead of despawning it.";
			} else {
				_poolSummaryString += "\n\nPool Protection is disabled which allows the pool to run faster but doesn't check for deleted instances which can cause null reference exceptions. Make sure not to destroy your instances!";
			}

			

			// If we're destroying on Load
			if(  o.dontDestroyOnLoad ){
				_poolSummaryString += "\n\nThis is a global Pool and will not be destroyed when changing scenes.";
			} else {
				_poolSummaryString += "\n\nThis is a local Pool and will be destroyed when changing scenes.";
			}

			// If we're allowing delegates and events
			if( o.enablePoolEvents ){
				_poolSummaryString += " Delegates will be used to send events to other scripts via the API.";
			} else {
				_poolSummaryString += " Delegates will be ignored which makes the pool slightly faster.";
			}

			// Return the completed string
			return _poolSummaryString;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO POOL SETTINGS
		//	Display the Editor fields for the Pool
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void DoPoolSettings(){

			// =================================
			//	TAB TITLE
			// =================================

			// Show Tab Headers
			if( _showTabHeaders ){

				// Start White Box
				HTGUI.StartWhiteBox();

					// Title
					EditorGUILayout.Space();
					HTGUI.Header( poolIcon, "Pool", "This tab allows you to name your Pool system and define how it works. These are options that are shared across all of the prefabs that make up the pool. If you have 'Show Helpful Tips' enabled in PoolKit's Preferences, an overiew of your settings will also be presented to you.", System.String.Empty, _v2_100_64, 13
					);

				// End White Box
				HTGUI.EndWhiteBox();
			}


			// =================
			//	INTERACT SETUP
			// =================

			// Start White Box
			HTGUI.StartWhiteBox();

				// Title
				HTGUI.SectionTitle( "Pool Name", "Give this pool a unique name so it can be found using the PoolKit API." );
				EditorGUILayout.Space();

				// Only Rename Objects In Editor
				o.poolName = HTGUI_UNDO.TextField( o, "Pool Name", buttonIcon, "Pool Name:", o.poolName );
				if(o.poolName == System.String.Empty ){ o.poolName = "My Pool Name"; }

				// New Section
				SpacedSepLineTitle( "Pool Options", _showHelpfulNotes ? "Below is an overview of how this Pool works:" : "Setup how your Pool will work using the options below:" );

				// Pool Type Mesage
				if( _showHelpfulNotes == true ){
					DoHelpBox( GetPoolSummary(), MessageType.Info, true, false, true );
				}

				// Rename Objects In Pool
				o.poolType = (Pool.PoolType)HTGUI_UNDO.EnumField( o, "Pool Type", gearIcon, "Pool Type:", o.poolType);

				// Force poolWasJustCreatedByAPI to be false when creating a pool!
				o.poolWasJustCreatedByAPI = false;

				// Enable Pool Protection
				o.enablePoolProtection = HTGUI_UNDO.ToggleField( o, "Enable Pool Protection", shieldLabel, "Enable Pool Protection:", o.enablePoolProtection);

				// Don't Destroy On Load
				o.dontDestroyOnLoad = HTGUI_UNDO.ToggleField( o, "Don't Destroy On Load", deleteIcon, "Don't Destroy On Load:", o.dontDestroyOnLoad);

				// Enable Pool Events
				o.enablePoolEvents = HTGUI_UNDO.ToggleField( o, "Enable Delegates & Events", nextLabel, "Enable Delegates & Events:", o.enablePoolEvents);

				// Add Space at the bottom
				EditorGUILayout.Space();

				// If we've set this as a global pool but it has a parent that is not a Global Pool Group
				if( o.dontDestroyOnLoad == true && o.transform.parent != null &&
					o.transform.parent.GetComponent<PoolKitSetup>() == null
				){
					DoHelpBox( "This Pool is set to be global but is not configured correctly. This must either not have a parent Transform or be a child of a 'Global Pool Group'.", MessageType.Error );

				// If we've set this as a global pool but it has a parent that is not a Global Pool Group
				} else if( o.dontDestroyOnLoad == true && o.transform.parent != null &&
					o.transform.parent.GetComponent<PoolKitSetup>() != null &&
					o.transform.parent.GetComponent<PoolKitSetup>().dontDestroyOnLoad == false
				){
					DoHelpBox( "This Pool is set to be global but is part of a 'Global Pool Group' that is not configured correctly. You should make sure 'Don't Destroy On Load' is enabled on the gameObject: '" + o.transform.parent.name + "'.", MessageType.Error );
				}


			// End White Box
			HTGUI.EndWhiteBox();
			
		}

		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	DO POOL ITEMS
		//	Display the Editor fields for the Pool Items
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Helpers
		PoolItem _poolItem = null;
		string _poolItemHeader = string.Empty;
		GUIContent _guiContentHeader = new GUIContent();
		GUILayoutOption[] defaultGUILayoutOptions = new GUILayoutOption[]{ GUILayout.MinWidth(300), GUILayout.MaxWidth(400)};
		GUIContent[] subTabGUIContent = new GUIContent[]{ new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent() };

		// Constant Helpers
		const string _boldLabel = "BoldLabel";
		const string _noPrefabSetupYet = "No Prefab Has Been Assigned Yet!";
		const string _poolItemHeaderStartDark = "  <color=#7f7f7fff>Prefab ";	// <- Light grey
		const string _poolItemHeaderStartLight = "  <color=#4c4c4cff>Prefab ";	// <- Dark grey
			string PoolItemHeaderStart(){
				if( EditorGUIUtility.isProSkin == true ){ return _poolItemHeaderStartDark; }
				return _poolItemHeaderStartLight;
			}
		const string _poolItemHeaderSeperator = ":   </color>";
		const string _instance = "instance";
		const string _instances = "instances";
		const string _second = "second";
		const string _seconds = "seconds";
		Vector2 _v2_100_64 = new Vector2(100, 64);

		// Method
		void DoPoolItems(){

			// =================================
			//	TAB TITLE
			// =================================

			// Show Tab Headers
			if( _showTabHeaders ){

				// End White Box
				HTGUI.StartWhiteBox();

					// Title
					EditorGUILayout.Space();
					HTGUI.Header( prefabIcon, "Prefabs", "This tab allows you to setup which prefabs will be a part of this Pool. Even though each of these prefabs will share the same Pool system, they work independently with their own customizable options and features.", System.String.Empty, _v2_100_64, 13
					);

				// End White Box
				HTGUI.EndWhiteBox();

			}


			// Make sure the PoolItems array is not null
			if( o.poolItems == null ){ o.poolItems = new PoolItem[0]; }

			// Loop through each pool Item
			if( o.poolItems != null && o.poolItems.Length > 0 ){
				for( int i = 0; i < o.poolItems.Length; i++ ){

					// Make sure Pool Item is valid
					_poolItem = o.poolItems[i];
					if(_poolItem!=null){
						
						// =================================
						//	HEADER
						// =================================

						if( _poolItem.prefabToPool == null){
							if( EditorGUIUtility.isProSkin == false ){ 
								GUI.backgroundColor = new Color( 1f, 0.25f, 0.25f, 0.075f);
							} else {
								GUI.backgroundColor = new Color( 1f, 0f, 0f, 1f);
							}
							
						}

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
															(!_poolItem.prefabTabIsOpen ? foldoutRight : foldoutDown ),
															"Click this button to minimize and expand the Prefab." ),

											// Here we set the style and layout
											foldoutGUIStyle, GUILayout.MaxWidth(22), GUILayout.MinHeight(14), GUILayout.MaxHeight(14)) 
										){ 
											_poolItem.prefabTabIsOpen = !_poolItem.prefabTabIsOpen;
										}

									// Original version
									#else


										// Create a button that mimics the foldout
										if( GUILayout.Button( 

											// Get Unity's default foldout icons to display here with a tooltip!
											new GUIContent( string.Empty,
															(!_poolItem.prefabTabIsOpen ? EditorStyles.foldout.active.background : EditorStyles.foldout.onActive.background),
															"Click this button to minimize and expand the Prefab." ),

											// Here we set the style and layout
											foldoutGUIStyle, GUILayout.MaxWidth(24), GUILayout.MinHeight(16), GUILayout.MaxHeight(16)) 

										){ 
											_poolItem.prefabTabIsOpen = !_poolItem.prefabTabIsOpen;
										}
										

									#endif

									GUILayout.FlexibleSpace();
								EditorGUILayout.EndVertical();

								// -------------
								// TITLE, ETC.
								// -------------

								// Setup a larger bold font to use
								GUIStyle boldEventTitleGUIStyle = new GUIStyle(_boldLabel);
								boldEventTitleGUIStyle.fontSize = 12;
								boldEventTitleGUIStyle.richText = true;

								// Precache the title using the logic comment
								if( _poolItem.prefabToPool != null ){
									_poolItemHeader = _poolItem.prefabToPool.name;
									if(_poolItemHeader.Length > 64 ){ _poolItemHeader = _poolItemHeader.Substring(0,64) + " ..."; }
								} else {
									_poolItemHeader = _noPrefabSetupYet;
								}


								// Draw the icon and label using a GUIContent
								_guiContentHeader.text = PoolItemHeaderStart() + (i+1).ToString() + _poolItemHeaderSeperator + _poolItemHeader;
								_guiContentHeader.image = prefabSmallIcon;
								GUILayout.Label( _guiContentHeader, boldEventTitleGUIStyle, GUILayout.MaxHeight(24));

								// Space
								GUILayout.Label(string.Empty, GUILayout.MaxWidth(5), GUILayout.MaxHeight(5) );

								// Only allow the prefab items to be moved when the game isn't running
								if( Application.isPlaying == false ){

									// Move Event Up
									if( o.poolItems.Length > 0 && i != 0 &&
										GUILayout.Button( new GUIContent( System.String.Empty, upButton, "Move Pool Item Up"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
									){
										Undo.RecordObject ( o, "Move Pool Item Up" );
										Arrays.Shift( ref o.poolItems, i, true );
									}

									// Move Event Down
									if( o.poolItems.Length > 0 && i != o.poolItems.Length-1 &&
										GUILayout.Button( new GUIContent( System.String.Empty, downButton, "Move Pool Item Down"), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
									){
										Undo.RecordObject ( o, "Move Pool Item Down" );
										Arrays.Shift( ref o.poolItems, i, false );
									}

									// Destroy Pool Item (we must ensure at least 1 pool item exists)
									if( o.poolItems.Length > 1 &&
										GUILayout.Button( new GUIContent( System.String.Empty, HTGUI.removeButton, "Remove Pool Item" ), GUILayout.MinWidth(24), GUILayout.MaxWidth(24) ) 
									){
										Undo.RecordObject ( o, "Remove Pool Item" );
										Arrays.RemoveItemAtIndex( ref o.poolItems, i );
									}

								}
								
							EditorGUILayout.EndHorizontal();

							// If we closed the prefab tab, don't show any of the content
							if(_poolItem.prefabTabIsOpen == false ){
								HTGUI.EndWhiteBox();
								continue;
							}

							// =================================
							//	MAIN BOX
							// =================================

							// Add Space and seperator
							EditorGUILayout.Space();
							HTGUI.SepLine();

							// Show the tabs only if we've set a prefab to pool
							if( _poolItem.prefabToPool != null){

								// Setup the tabs
								for( int i2 = 0; i2 < subTabGUIContent.Length; i2++ ){
									subTabGUIContent[i2].text = subtabLabels[i2];
									subTabGUIContent[i2].image = subtabIcons[i2];
									subTabGUIContent[i2].tooltip = System.String.Empty;
								}

								// Show the tabs
								_poolItem.tab = HTGUI.HTSelectionGrid( null, _poolItem.tab, subTabGUIContent, 4, false, HT_GUIStyles.GetSelectionGridGUIStyle() );

								// Add Seperator Line
								HTGUI.SepLine();

							// Otherwise, force tab to be 0
							} else {
								_poolItem.tab = 0;
							}
							//EditorGUILayout.Space();

							// =================================
							//	0 PREFAB TAB
							// =================================

							if( _poolItem.tab == 0 ){

								// Prefab Title
								HTGUI.SectionTitle( "Prefab", "Drag and drop a prefab from the Project pane to have it pooled." );
								EditorGUILayout.Space();

								// Show the prefab to Pool
								_poolItem.prefabToPool = (GameObject)HTGUI_UNDO.ObjectField( o, "Prefab To Pool", new GUIContent("Prefab To Pool:", cubeLabel, System.String.Empty), _poolItem.prefabToPool, false, defaultGUILayoutOptions );

								// Show the extra options only if we've setup the prefab
								if( _poolItem.prefabToPool != null){

									// Pool Size Options Title
									SpacedSepLineTitle( "Pool Size Options", "Setup how the pool handles its size." );

									// Allow Pool Size Options to be changed, only if we're not using a Fixed Array
									if( o.poolType == Pool.PoolType.FixedArray ){

										// Show Pool Size Notes
										if( _showHelpfulNotes ){
											DoHelpBox( "Pools using 'Fixed Arrays' must keep the pool size fixed." );
											EditorGUILayout.Space();
										}

										// Otherwise, make sure the Pool is set to stay fixed.
										_poolItem.poolSizeOptions = PoolItem.PoolResizeOptions.KeepPoolSizeFixed;

										// Show a dimmed version so the user can see it is greyed out.
										GUI.enabled = false;
										_poolItem.poolSizeOptions = (PoolItem.PoolResizeOptions)HTGUI_UNDO.EnumField( o, "Pool Size Options", new GUIContent( "Pool Size Options:", gearIcon, System.String.Empty ), _poolItem.poolSizeOptions, defaultGUILayoutOptions);
										GUI.enabled = true;

									} else {

										// Show information about the different pool size options
										/*
										if( _showHelpfulNotes ){
											
											// Keep Pool Size Fixed
											if( _poolItem.poolSizeOptions == PoolItem.PoolResizeOptions.KeepPoolSizeFixed ){
												DoHelpBox( "This pool is using a fixed size of " + _poolItem.poolSize.ToString() + " instances."  );
												EditorGUILayout.Space();
											}

											// Expand Pool Within Limit
											else if( _poolItem.poolSizeOptions == PoolItem.PoolResizeOptions.ExpandPoolWithinLimit ){
												DoHelpBox( "This pool is using a default size of " + _poolItem.poolSize.ToString() + " instances and will automatically grow when needed to a maximum size of " + _poolItem.limitPoolSize.ToString() + "."  );
												EditorGUILayout.Space();
											}

											// Always Expand Pool When Needed
											else if( _poolItem.poolSizeOptions == PoolItem.PoolResizeOptions.AlwaysExpandPoolWhenNeeded ){
												DoHelpBox( "This pool is using a default size of " + _poolItem.poolSize.ToString() + " but will always create new instances when needed."  );
												EditorGUILayout.Space();
											}
										}
										*/

										// Show Pool Size Options
										_poolItem.poolSizeOptions = (PoolItem.PoolResizeOptions)HTGUI_UNDO.EnumField( o, "Pool Size Options", new GUIContent( "Pool Size Options:", gearIcon, System.String.Empty ), _poolItem.poolSizeOptions, defaultGUILayoutOptions);
									}

									// Pool Size At Start
									_poolItem.poolSize = HTGUI_UNDO.IntField( o, "Default Pool Size", new GUIContent( "Default Pool Size:", widthLabel, System.String.Empty), _poolItem.poolSize, defaultGUILayoutOptions );

									// If we're using a Dynamic list, allow the default size to be 0
									if( o.poolType == Pool.PoolType.DynamicList ){
										if(_poolItem.poolSize < 0){ _poolItem.poolSize = 0; }

									// Make sure the pool has at least 1 instance for fixed arrays or automatic
									} else {
										if(_poolItem.poolSize <= 0){ _poolItem.poolSize = 1; }
									}

									// Show The Limit Pool Size button if done
									if( _poolItem.poolSizeOptions == PoolItem.PoolResizeOptions.ExpandPoolWithinLimit ){

										_poolItem.limitPoolSize = HTGUI_UNDO.IntField( o, "Maximum Pool Size", new GUIContent( "Maximum Pool Size:", widthLabel, System.String.Empty), _poolItem.limitPoolSize, defaultGUILayoutOptions );

										// Automatically correct the limit Pool Size if its too small
										if( _poolItem.limitPoolSize < _poolItem.poolSize+1 ){ 
											_poolItem.limitPoolSize = _poolItem.poolSize+1;
										}
									}

								}

							}

							// =================================
							//	1 INSTANCES TAB
							// =================================

							if( _poolItem.tab == 1 ){

								// Scaling Title
								HTGUI.SectionTitle( "Instance Scale", "Set how instances should be scaled when instantiated." );
								EditorGUILayout.Space();

								// Spawned Instance Scaling
								_poolItem.spawnedItemScale = (PoolItem.PoolScale)HTGUI_UNDO.EnumField( o, "Spawned Instance Scaling", new GUIContent( "Spawned Instance Scaling:", resizeLabel, System.String.Empty ), _poolItem.spawnedItemScale, defaultGUILayoutOptions);

								// Show Resize options if we're not ignoring it:
								if( _poolItem.spawnedItemScale != PoolItem.PoolScale.Ignore ){

									// Custom Spawn Scale
									if( _poolItem.spawnedItemScale == PoolItem.PoolScale.CustomScale ){
										_poolItem.customSpawnScale = HTGUI_UNDO.Vector3Field( o, "Custom Spawn Scale", new GUIContent( "Custom Spawn Scale:", resizeLabel, System.String.Empty), _poolItem.customSpawnScale, defaultGUILayoutOptions );
									}

									// Custom Spawn Scale
									else if( _poolItem.spawnedItemScale == PoolItem.PoolScale.RandomRangeCustomScale ){
										
										// Minimum Scale
										_poolItem.customSpawnScaleMin = HTGUI_UNDO.Vector3Field( o, "Minimum Spawn Scale", new GUIContent( "Minimum Spawn Scale:", resizeLabel, System.String.Empty), _poolItem.customSpawnScaleMin, defaultGUILayoutOptions );

										// Maximum Scale
										_poolItem.customSpawnScaleMax = HTGUI_UNDO.Vector3Field( o, "Maximum Spawn Scale", new GUIContent( "Maximum Spawn Scale:", resizeLabel, System.String.Empty), _poolItem.customSpawnScaleMax, defaultGUILayoutOptions );
									
									}

									// Custom Spawn Scale
									else if( _poolItem.spawnedItemScale == PoolItem.PoolScale.RandomRangeProportionalScale ){
										
										// Minimum Scale
										_poolItem.customSpawnScaleProportionalMin = HTGUI_UNDO.FloatField( o, "Minimum Spawn Scale", new GUIContent( "Minimum Spawn Scale:", resizeLabel, System.String.Empty), _poolItem.customSpawnScaleProportionalMin, defaultGUILayoutOptions );

										// Maximum Scale
										_poolItem.customSpawnScaleProportionalMax = HTGUI_UNDO.FloatField( o, "Maximum Spawn Scale", new GUIContent( "Maximum Spawn Scale:", resizeLabel, System.String.Empty), _poolItem.customSpawnScaleProportionalMax, defaultGUILayoutOptions );
									}

									// Reset Scale On Every Spawn
									_poolItem.resetScaleOnEverySpawn = HTGUI_UNDO.ToggleField( o, "Reset Scale On Every Spawn", new GUIContent( "Reset Scale On Every Spawn:", loopLabel, System.String.Empty), _poolItem.resetScaleOnEverySpawn, defaultGUILayoutOptions );

									// Show helpful notes about Instance scaling
									if( _showHelpfulNotes && _poolItem.resetScaleOnEverySpawn ){
										DoHelpBox( "NOTE: Resetting an instance's scale every time it is spawned can impact performance."  );
										EditorGUILayout.Space();
									}
								}
								
								// Layer - Title
								SpacedSepLineTitle( "Instance Layer", "Set what layer instances should use when instantiated." );

								// Spawned Instance Layer
								_poolItem.spawnedItemLayer = (PoolItem.PoolLayer)HTGUI_UNDO.EnumField( o, "Spawned Instance Layer", new GUIContent( "Spawned Instance Layer:", layersLabel, System.String.Empty ), _poolItem.spawnedItemLayer, defaultGUILayoutOptions);

								// Show the custom layer setting if enabled
								if( _poolItem.spawnedItemLayer == PoolItem.PoolLayer.CustomLayer ){

									// Reset Scale On Every Spawn
									_poolItem.customSpawnLayer = HTGUI_UNDO.LayerField( o, "Custom Instance Layer", new GUIContent( "Custom Instance Layer:", layersLabel, System.String.Empty), _poolItem.customSpawnLayer, defaultGUILayoutOptions );

								}

								// Show the reset layer setting if we're not ignoring it
								if( _poolItem.spawnedItemLayer != PoolItem.PoolLayer.Ignore ){

									// Reset Layer On Every Spawn
									_poolItem.resetLayerOnEverySpawn = HTGUI_UNDO.ToggleField( o, "Reset Layer On Every Spawn", new GUIContent( "Reset Layer On Every Spawn:", loopLabel, System.String.Empty), _poolItem.resetLayerOnEverySpawn, defaultGUILayoutOptions );

									// Show helpful notes about Instance scaling
									if( _showHelpfulNotes && _poolItem.resetLayerOnEverySpawn ){
										DoHelpBox( "NOTE: Resetting an instance's layer every time it is spawned can impact performance."  );
										EditorGUILayout.Space();
									}
								}

								// Organize Instances - Title
								SpacedSepLineTitle( "Keep Instances Organized", "Keep instances parented to this pool. This is recommended for persistant (global) pools as it helps to stop instances from being destroyed when changing scenes." );

								// Keep Instances Organized
								_poolItem.keepOrganized = HTGUI_UNDO.ToggleField( o, "Keep Instances Organized", new GUIContent( "Keep Instances Organized:", groupLabel, System.String.Empty), _poolItem.keepOrganized, defaultGUILayoutOptions );

									// Show helpful notes about keeping instances organized
									if( _showHelpfulNotes && _poolItem.keepOrganized ){
										DoHelpBox( "Please note that reparenting may affect performance."  );
										EditorGUILayout.Space();
									}

								// Organize Instances - Title
								SpacedSepLineTitle( "Recycle Spawned Instances", "If you run out of instances you can choose to recycle (despawn and spawn) a previously spawned instance in the scene. This may affect performance slightly but works great with Fixed Arrays." );

								// Recycle Spawned Instances
								_poolItem.recycleSpawnedObjects = HTGUI_UNDO.ToggleField( o, "Recycle Spawned Instances", new GUIContent( "Recycle Spawned Instances:", loopLabel, System.String.Empty), _poolItem.recycleSpawnedObjects, defaultGUILayoutOptions );

							}

							// =================================
							//	2 FEATURES TAB
							// =================================

							if( _poolItem.tab == 2 ){

								// Prefab Title
								HTGUI.SectionTitle( "Lazy Preloading", o.poolType == Pool.PoolType.FixedArray ? "Lazy Preloading isn't possible with Pools using a Fixed Array." : "You may choose to grow the pool by preloading instances over time." );
								EditorGUILayout.Space();

								// Allow Pool Size Options to be changed, only if we're not using a Fixed Array
								if( o.poolType == Pool.PoolType.FixedArray ){

									// Show Pool Size Notes
									if( _showHelpfulNotes ){
										DoHelpBox( "Pools using 'Fixed Arrays' cannot use lazy preloading." );
										EditorGUILayout.Space();
									}

									// If we're using a fixed array, make sure lazy preloading is disabled
									_poolItem.useLazyPreloading = false;

									// Enable Lazy Preloading
									GUI.enabled = false;
									_poolItem.useLazyPreloading = HTGUI_UNDO.ToggleField( o, "Enable Lazy Preloading", new GUIContent( "Enable Lazy Preloading:", loadLabel, System.String.Empty), _poolItem.useLazyPreloading, defaultGUILayoutOptions );
									GUI.enabled = true;

								} else {

									// Enable Lazy Preloading
									_poolItem.useLazyPreloading = HTGUI_UNDO.ToggleField( o, "Enable Lazy Preloading", new GUIContent( "Enable Lazy Preloading:", loadLabel, System.String.Empty), _poolItem.useLazyPreloading, defaultGUILayoutOptions );
								
								}

								// Show the options if we're using lazy preloading
								if( _poolItem.useLazyPreloading == true ){

									// Instances To Create On Awake
									_poolItem.lazyPreloadingInstancesOnAwake = HTGUI_UNDO.IntField( o, "Instances Created On Awake", new GUIContent( "Instances Created On Awake:", widthLabel, System.String.Empty), _poolItem.lazyPreloadingInstancesOnAwake, defaultGUILayoutOptions );
									if(_poolItem.lazyPreloadingInstancesOnAwake < 1){ _poolItem.lazyPreloadingInstancesOnAwake = 1; }

									// Initial Delay
									_poolItem.lazyPreloadingInitialDelay = HTGUI_UNDO.FloatField( o, "Initial Delay", new GUIContent( "Initial Delay:", timeLabel, System.String.Empty), _poolItem.lazyPreloadingInitialDelay, defaultGUILayoutOptions );
									if(_poolItem.lazyPreloadingInitialDelay < 0f ){ _poolItem.lazyPreloadingInitialDelay = 0f; }

									// Instances Per Pass
									_poolItem.lazyPreloadingInstancesPerPass = HTGUI_UNDO.IntField( o, "Instances To Create Per Pass", new GUIContent( "Instances To Create Per Pass:", loopLabel, System.String.Empty), _poolItem.lazyPreloadingInstancesPerPass, defaultGUILayoutOptions );
									if(_poolItem.lazyPreloadingInstancesPerPass < 1){ _poolItem.lazyPreloadingInstancesPerPass = 1; }

									// Delay Between Passes
									_poolItem.lazyPreloadingDelayBetweenPasses = HTGUI_UNDO.FloatField( o, "Delay Between Passes", new GUIContent( "Delay Between Passes:", timeLabel, System.String.Empty), _poolItem.lazyPreloadingDelayBetweenPasses, defaultGUILayoutOptions );
									if(_poolItem.lazyPreloadingDelayBetweenPasses < 0){ _poolItem.lazyPreloadingDelayBetweenPasses = 0; }

									// Reset Default Settings
									GUILayout.BeginHorizontal();
										GUILayout.FlexibleSpace();
										if( GUILayout.Button(" Reset Default Settings ") ){
											Undo.RecordObject ( o, "Reset Default Preload Settings" );
											_poolItem.lazyPreloadingInstancesOnAwake = 1;
											_poolItem.lazyPreloadingInitialDelay = 1f;
											_poolItem.lazyPreloadingInstancesPerPass = 1;
											_poolItem.lazyPreloadingDelayBetweenPasses = 0.2f;
										}
									GUILayout.EndHorizontal();

									// Show Lazy Preloading Notes
									if( _showHelpfulNotes && _poolItem.useLazyPreloading ){
										EditorGUILayout.Space();
										DoHelpBox( NumberFormat( _poolItem.lazyPreloadingInstancesOnAwake, _instance, _instances ) + " will be created before an initial delay of " + NumberFormat( _poolItem.lazyPreloadingInitialDelay, _second, _seconds ) + ". When completed, " + NumberFormat( _poolItem.lazyPreloadingInstancesPerPass, _instance, _instances ) + " will be created every " + NumberFormat( _poolItem.lazyPreloadingDelayBetweenPasses,_second, _seconds ) + " until " + NumberFormat( _poolItem.poolSize, _instance + " has", _instances + " have" ) + " been created."

										 );
										EditorGUILayout.Space();
									}

								}

								// Instances - Title
								SpacedSepLineTitle( "Automatic Despawning", "You can setup automatic despawning for this prefab." );

								// Enable Automatic Despawning
								_poolItem.enableAutoDespawn = HTGUI_UNDO.ToggleField( o, "Enable Auto-Despawning", new GUIContent( "Enable Auto-Despawning:", hideIcon, System.String.Empty), _poolItem.enableAutoDespawn, defaultGUILayoutOptions );

								
								// Show the options if we're using lazy preloading
								if( _poolItem.enableAutoDespawn == true ){

									// Despawn Mode
									_poolItem.despawnMode = (PoolItem.DespawnMode)HTGUI_UNDO.EnumField( o, "Despawn Mode", new GUIContent( "Despawn Mode:", gearIcon, System.String.Empty ), _poolItem.despawnMode, defaultGUILayoutOptions);

									// Despawn Countdown
									if( _poolItem.despawnMode == PoolItem.DespawnMode.Countdown ){

										// Countdown In Seconds
										_poolItem.despawnAfterHowManySeconds = HTGUI_UNDO.FloatField( o, "Countdown In Seconds:", new GUIContent( "Countdown In Seconds:", timeLabel, System.String.Empty), _poolItem.despawnAfterHowManySeconds, defaultGUILayoutOptions );
										if(_poolItem.despawnAfterHowManySeconds < 0){_poolItem.despawnAfterHowManySeconds = 0; }

										// Show Helpful Notes
										if( _showHelpfulNotes ){
											EditorGUILayout.Space();
											DoHelpBox( "All instances of this prefab will be automatically despawned after " + NumberFormat( _poolItem.despawnAfterHowManySeconds, _second, _seconds ) + "." );
										}
									}

									// Despawn Countdown Random Range
									else if( _poolItem.despawnMode == PoolItem.DespawnMode.CountdownRandomRange ){

										// Countdown In Seconds (Minimum)
										_poolItem.despawnRandomRangeMin = HTGUI_UNDO.FloatField( o, "Min Countdown In Seconds:", new GUIContent( "Min Countdown In Seconds:", timeLabel, System.String.Empty), _poolItem.despawnRandomRangeMin, defaultGUILayoutOptions );
										if(_poolItem.despawnRandomRangeMin < 0){_poolItem.despawnRandomRangeMin = 0; }

										// Countdown In Seconds (Maximum)
										_poolItem.despawnRandomRangeMax = HTGUI_UNDO.FloatField( o, "Max Countdown In Seconds:", new GUIContent( "Max Countdown In Seconds:", timeLabel, System.String.Empty), _poolItem.despawnRandomRangeMax, defaultGUILayoutOptions );
										if(_poolItem.despawnRandomRangeMax < _poolItem.despawnRandomRangeMin){
											_poolItem.despawnRandomRangeMax = _poolItem.despawnRandomRangeMin;
										}

										// Show Helpful Notes
										if( _showHelpfulNotes ){
											EditorGUILayout.Space();
											DoHelpBox( "All instances of this prefab will be automatically despawned after " + _poolItem.despawnRandomRangeMin.ToString() + " to " + _poolItem.despawnRandomRangeMax.ToString() + " seconds." );
										}
									}

									// Wait For Audio To Finish
									else if( _poolItem.despawnMode == PoolItem.DespawnMode.WaitForAudioToFinish ){
										
										// Show Helpful Notes
										if( _showHelpfulNotes ){
											EditorGUILayout.Space();
											DoHelpBox( "If an Audio Source on the instance has stopped playing it will be automatically despawned. This will only work if an Audio Source exists on the prefab and set to Auto-Play." );
										}
									}

									// Wait For Particle System To Finish
									else if( _poolItem.despawnMode == PoolItem.DespawnMode.WaitForParticleSystemToFinish ){
										
										// Show Helpful Notes
										if( _showHelpfulNotes ){
											EditorGUILayout.Space();
											DoHelpBox( "If a Particle System on the instance has stopped playing it will be automatically despawned. This will only work if a Particle System exists on the prefab and set to Auto-Play." );
										}
									}

								}
								
							}

							// =================================
							//	3 AUTO-DESPAWN TAB
							// =================================

							if( _poolItem.tab == 3 ){

								// Notifications Title
								HTGUI.SectionTitle( "Notifications", "Enabling Notifications can trigger the 'OnSpawn' and 'OnDespawn' methods of your instances. Using the 'Pool Kit Listener' interface is the recommended approach as it takes advantage of custom caching and is extremely fast. If you don't need custom methods to be invoked, you can also use the 'OnEnable' and 'OnDisable' methods in MonoBehaviour." );
								EditorGUILayout.Space();

								// Notification Mode
								_poolItem.notifications = (PoolItem.Notifications)HTGUI_UNDO.EnumField( o, "Despawn Mode", new GUIContent( "Notification Mode:", nextLabel, System.String.Empty ), _poolItem.notifications, defaultGUILayoutOptions);

								// Show Helpful Notes
								if( _showHelpfulNotes ){
									
									// Pool Kit Listeners
									if( _poolItem.notifications == PoolItem.Notifications.PoolKitListeners ){

										EditorGUILayout.Space();
										DoHelpBox( "// Don't forget to setup your instance scripts to use the IPoolKitListener interface like this:\n\n=========================================\n\n"+
											"using HellTap.PoolKit;\n\n"+
											"public class ExampleScript : MonoBehaviour, IPoolKitListener {\n\n"+
											"     public void OnSpawn( Pool pool ){}\n"+
											"     public void OnDespawn(){}\n}"
										);
									}

									// Send Message
									else if( _poolItem.notifications == PoolItem.Notifications.SendMessage ){
										
										EditorGUILayout.Space();
										DoHelpBox( "Notifications using 'Send Message' only attempt to invoke methods on the instance's GameObject. This is much slower than the IPoolKitListener interface but faster than the 'Broadcast Message' approach." );
									}

									// Broadcast Message
									else if( _poolItem.notifications == PoolItem.Notifications.BroadcastMessage ){
										
										EditorGUILayout.Space();
										DoHelpBox( "Notifications using 'Broadcast Message' attempt to invoke methods on the instance's GameObject and all of it's children. Even though it is easy to use, it is very slow, especially when dealing with large pools." );
									}
								}


								// Delegates - Title
								SpacedSepLineTitle( "Enable Delegates & Events", "You can allow scripts to override how instances are instantiated and destroyed by using delegates and events in the API. Enabling this may incur a small performance cost." );

								// Enable Instantiation Delegates
								_poolItem.enableInstantiateDelegates = HTGUI_UNDO.ToggleField( o, "Enable Instantiation Events", new GUIContent( "Enable Instantiation Events:", gearIcon, System.String.Empty), _poolItem.enableInstantiateDelegates, defaultGUILayoutOptions );

								// Enable Instantiation Delegates
								_poolItem.enableDestroyDelegates = HTGUI_UNDO.ToggleField( o, "Enable Destroy Events", new GUIContent( "Enable Destroy Events:", gearIcon, System.String.Empty), _poolItem.enableDestroyDelegates, defaultGUILayoutOptions );
								

								// Show Helpful Notes if we've enabled either of these
								if( _showHelpfulNotes && 
									( _poolItem.enableInstantiateDelegates || _poolItem.enableDestroyDelegates ) 
								){
									
									EditorGUILayout.Space();
									DoHelpBox( "=========================================\n" +
												"// Override the Pool's Events like this:\n=========================================\n\n" +
												"     PoolKit.GetPool(\" + o.poolName + \").OnCreateInstance += OnCreateInstance;\n"+
												"     PoolKit.GetPool(\" + o.poolName + \").OnDestroyInstance += OnDestroyInstance;\n\n"+
												"=========================================\n" +
												"// You can Use Replacement Methods like this:\n=========================================\n\n"+
												"     GameObject OnCreateInstance( GameObject prefab ) {}\n"+
												"     void OnDestroyInstance( GameObject instance ) {}"
									);
								}
							}



						// End White Box
						HTGUI.EndWhiteBox();

					}
				}
			}

			// Add / Remove Pool Items (Create Horizontal Row)
			if( !Application.isPlaying ){

				// Start Horizontal Row
				EditorGUILayout.BeginHorizontal();

					// Flexible Space
					GUILayout.FlexibleSpace();	
										
					// Remove Button			
					if( o.poolItems.Length == 0 ){ GUI.enabled = false; }			
					if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.removeButton, "Remove Last Pool Item"), GUILayout.MaxWidth(32)) ) { 
						if( o.poolItems.Length > 0 ){	// <- We must always have at least 1 condition!
							Undo.RecordObject ( o, "Remove Last Pool Item");
							System.Array.Resize(ref o.poolItems, o.poolItems.Length - 1 );
						}
					}

					// Reset GUI Enabled
					GUI.enabled = true;
									
					// Add Button							
					if( GUILayout.Button( new GUIContent( string.Empty, HTGUI.addButton, "Add New Pool Item"), GUILayout.MaxWidth(32))) { 
						Undo.RecordObject ( o, "Add New Pool Item");
						System.Array.Resize(ref o.poolItems, o.poolItems.Length + 1 ); 
						o.poolItems[ o.poolItems.Length - 1 ] = new PoolItem();	// <- We need to set new Pool Item
					}

				// End Horizontal Row
				EditorGUILayout.EndHorizontal();

			}

			// Add space underneith
			EditorGUILayout.Space();
			EditorGUILayout.Space();
				

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
		//	DO POOL STATISTICS
		//	Shows the pool statistics in the Editor
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Helpers
		public GUIStyle noneStyle = GUIStyle.none;
		public GUIStyle richTextLabelStyle = new GUIStyle();
		public GUIStyle _statisticsHeaderStyle = new GUIStyle();
		GUILayoutOption[] progressBarGUILayoutOptions = new GUILayoutOption[]{GUILayout.MinHeight(18)};
		Rect _rt = new Rect();
		float _percentagePoolSize = 0f;

		// Method
		void DoPoolStatistics(){

			// --------------------
			//	Setup GUIStyles
			// --------------------

			// Setup GUIStyles based on the current Unity Pro skin
			if( EditorGUIUtility.isProSkin == true ){
				richTextLabelStyle.normal.textColor = Color.white;
				_statisticsHeaderStyle.normal.textColor = Color.white;
			} else {
				richTextLabelStyle.normal.textColor = Color.black;
				_statisticsHeaderStyle.normal.textColor = Color.black;
			}

			// =================================
			//	TAB TITLE
			// =================================

			// Show Tab Headers
			if( _showTabHeaders ){

				// End White Box
				HTGUI.StartWhiteBox();

					// Title
					EditorGUILayout.Space();
					HTGUI.Header( analyticsIcon, "Statistics", "This tab allows you to better balance the performance of your pool by analyzing its usage. Details for each of your prefabs and instances are visible when running the game in the Editor.", System.String.Empty, _v2_100_64, 13
					);

					if( !Application.isPlaying ){
						
						EditorGUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
							GUILayout.Label(">> Press Play In The Editor To View Realtime Statistics <<", _boldLabel );
							GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();

						// Add Space
						EditorGUILayout.Space();
						
					}

				// End White Box
				HTGUI.EndWhiteBox();
			
			} else {

				if( !Application.isPlaying ){
						
					EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(">> Press Play In The Editor To View Realtime Statistics <<", _boldLabel );
						GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();

					// Add Space
					EditorGUILayout.Space();
					
				}
			}

			// If the game isn't running, end it here.
			if( !Application.isPlaying ){

				// Add Space
				EditorGUILayout.Space();
				
				// End the UI
				return;
			}
				
			// -------------------------
			//	LOOP THROUGH POOL ITEMS
			// -------------------------

			// Make sure the PoolItems array is not null
			if( o.poolItems == null ){ o.poolItems = new PoolItem[0]; }

			// Make sure richTextLabel has richText enabled
			richTextLabelStyle.richText = true;
			richTextLabelStyle.fontSize = 10;

			_statisticsHeaderStyle.fontStyle = FontStyle.Bold;
			_statisticsHeaderStyle.fontSize = 12;

			// Loop through each Pool Item
			if( o.poolItems != null && o.poolItems.Length > 0 ){
				for( int i = 0; i < o.poolItems.Length; i++ ){

					// Make sure Pool Item is valid
					_poolItem = o.poolItems[i];
					if( _poolItem!=null && _poolItem.prefabToPool != null ){

						// Start White Box
						HTGUI.StartWhiteBox();

							// Reset helper values if the game isn't running in the editor
							if( !Application.isPlaying ){
								_poolItem.instanceSpawnedProgressBar = 0f;
								_poolItem.instanceSpawnedProgressBarLerped = 0f;
							}

							// --------------------
							//	PREFAB TITLE
							// --------------------

							// Show the title of this prefab
							EditorGUILayout.BeginHorizontal();
								GUILayout.Label(
								 	new GUIContent( "  " + _poolItem.prefabToPool.name, cubeLabel, System.String.Empty ),
								 	_statisticsHeaderStyle,
								 	new GUILayoutOption[]{ GUILayout.MaxHeight(24) }
								);
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.Space();

							// ------------------------
							//	INSTANCES PROGRESS BAR
							// ------------------------

							EditorGUILayout.BeginHorizontal();
								GUILayout.Space(4);

								// Make an invisible box in GUILayout so we can use the Rect
								GUILayout.Box( string.Empty, noneStyle, progressBarGUILayoutOptions );
								_rt = GUILayoutUtility.GetLastRect();

								// Do Progress bar
								//EditorGUI.ProgressBar( _rt, _poolItem.instanceCount == 0 ? 0f : ((float)_poolItem.activeInstances /(float)_poolItem.instanceCount ), _poolItem.activeInstances.ToString() + " / " + _poolItem.instanceCount.ToString() + " Instances Spawned" );

								EditorGUI.ProgressBar( _rt, _poolItem.instanceSpawnedProgressBarLerped, _poolItem.activeInstances.ToString() + " / " + _poolItem.instanceCount.ToString() + " Instances Spawned" );

								GUILayout.Space(4);
							EditorGUILayout.EndHorizontal();

							// Add Space and Seperator
							EditorGUILayout.Space();
							EditorGUILayout.Space();
							HTGUI.SepLine();
							EditorGUILayout.Space();


							// Begin Horizontal Row
							EditorGUILayout.BeginHorizontal();

								// Left Column With Space
								EditorGUILayout.BeginVertical();
									GUILayout.Label( _space, GUILayout.MinWidth(4), GUILayout.MaxWidth(4) );
								EditorGUILayout.EndVertical();

								// Begin Vertical Column
								EditorGUILayout.BeginVertical( GUILayout.MinWidth(160), GUILayout.MaxWidth(160),GUILayout.ExpandWidth(false) );

									// -----------------------
									//	TIME SINCE LAST SPAWN
									// -----------------------

									// Last Spawned
									EditorGUILayout.BeginHorizontal();
										GUILayout.Label(
										 	new GUIContent( "  <b>Last Spawned: </b>  " + EpochFormat( _poolItem.timeSincePoolLastSpawnedAnInstance ), timeLabel, System.String.Empty ),
										 	richTextLabelStyle,
										 	new GUILayoutOption[]{ GUILayout.MaxHeight(20) }
										);
									EditorGUILayout.EndHorizontal();

									// -------------------------
									//	TIME SINCE LAST DESPAWN
									// -------------------------

									// Last Spawned
									EditorGUILayout.BeginHorizontal();
										GUILayout.Label(
										 	new GUIContent( "  <b>Last Despawned: </b>  " + EpochFormat( _poolItem.timeSincePoolLastDespawnedAnInstance ), timeLabel, System.String.Empty ),
										 	richTextLabelStyle,
										 	new GUILayoutOption[]{ GUILayout.MaxHeight(20) }
										);
									EditorGUILayout.EndHorizontal();

									// ------------------------------
									//	TIME SINCE LAST INSTANTIATED
									// ------------------------------

									// Last Spawned
									EditorGUILayout.BeginHorizontal();
										GUILayout.Label(
										 	new GUIContent( "  <b>Last Instantiated: </b>  " + EpochFormat( _poolItem.timeSincePoolLastInitializedAnInstance ), timeLabel, System.String.Empty ),
										 	richTextLabelStyle,
										 	new GUILayoutOption[]{ GUILayout.MaxHeight(20) }
										);
									EditorGUILayout.EndHorizontal();
							
								
								// End Vertical Column
								EditorGUILayout.EndVertical();

								// Begin Vertical Column < HORIZONTAL SPACE >
								EditorGUILayout.BeginVertical();
									GUILayout.Label( _space, GUILayout.MinWidth(24) );
								//	GUILayout.FlexibleSpace();
								EditorGUILayout.EndVertical();

								// Begin Vertical Column
								EditorGUILayout.BeginVertical( GUILayout.MinWidth(160), GUILayout.MaxWidth(160),GUILayout.ExpandWidth(false) );

									// ------------------------
									//	MAX INSTANCES SPAWNED
									// ------------------------

									// Last Spawned
									EditorGUILayout.BeginHorizontal();
										GUILayout.Label(
										 	new GUIContent( "  <b>Max Instances Spawned At Once: </b>  " + _poolItem.maxInstancesSpawnedAtOnce.ToString(), cubeLabel, System.String.Empty ),
										 	richTextLabelStyle,
										 	new GUILayoutOption[]{ GUILayout.MaxHeight(20) }
										);
									EditorGUILayout.EndHorizontal();

									// ------------------------
									//	TOTAL NUMBER OF SPAWNS
									// ------------------------

									// Last Spawned
									EditorGUILayout.BeginHorizontal();
										GUILayout.Label(
										 	new GUIContent( "  <b>Total Number Of Spawns: </b>  " + _poolItem.totalNumberOfSpawns.ToString(), cubeLabel, System.String.Empty ),
										 	richTextLabelStyle,
										 	new GUILayoutOption[]{ GUILayout.MaxHeight(20) }
										);
									EditorGUILayout.EndHorizontal();

									// --------------------------
									//	TOTAL NUMBER OF DESPAWNS
									// --------------------------

									// Last Spawned
									EditorGUILayout.BeginHorizontal();
										GUILayout.Label(
										 	new GUIContent( "  <b>Total Number Of Despawns: </b>  " + _poolItem.totalNumberOfDespawns.ToString(), cubeLabel, System.String.Empty ),
										 	richTextLabelStyle,
										 	new GUILayoutOption[]{ GUILayout.MaxHeight(20) }
										);
									EditorGUILayout.EndHorizontal();


								// End Vertical Column
								EditorGUILayout.EndVertical();

								// Right Column With Space
								EditorGUILayout.BeginVertical();
									GUILayout.Label( _space, GUILayout.MinWidth(4), GUILayout.MaxWidth(4) );
								EditorGUILayout.EndVertical();

							// End Horizontal Row
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.Space();
								
							// -----------------
							//	RECOMMENDATIONS
							// -----------------

							// Only show recommendations while the game is playing ...
							if( Application.isPlaying ){

								// If the instance count is larger than the default pool size, calculate the percentage ...
								if(_poolItem.instanceCount > _poolItem.poolSize ){

									// Calculate the percentage pool size
									//_percentagePoolSize =  Mathf.Round(( (float)_poolItem.instanceCount / (float)_poolItem.poolSize ) * 100f);

									_percentagePoolSize = Mathf.Round(( (float)_poolItem.instanceCount / (float)_poolItem.poolSize ));

									// If the current pool size is more than 20% bigger than the default, suggest to make it bigger
									if( _percentagePoolSize > 0.2f ){

										// Add Space and Seperator
										EditorGUILayout.Space();
										HTGUI.SepLine();

										// Show Help Box
										DoHelpBox( "This pool is currently " + _percentagePoolSize.ToString("F1") + "x larger than the initial default size of " +  _poolItem.poolSize.ToString() + ". Constantly instantiating new pool items is costly. In order to stabilize performance, consider setting the 'Default Pool' size to " + _poolItem.instanceCount.ToString() + " for the prefab named '" + _poolItem.prefabToPool.name + "'." );
									}
								}
							}


						// End White Box
						HTGUI.EndWhiteBox();
					}
				}

				// Add Space
				EditorGUILayout.Space();
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	EPOCH FORMAT
		//	Converts a epoch int (unix timestamp) into a readable format
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Helpers
		int _epochRemainderSeconds = 0;
		int _epochMinutes = 0;

		// Method
		string EpochFormat( int epochTime ){

			// Convert the time into seconds elapsed
			epochTime = Epoch.SecondsElapsed(epochTime);

			// if its within the first minute
			if( epochTime < 60){
				return epochTime + " Seconds Ago";

			// if its within the first hour
			} else if ( epochTime < 3600 ){
				_epochRemainderSeconds = epochTime % 60;	// <- left over seconds
				_epochMinutes = (epochTime - _epochRemainderSeconds) / 60;
				return NumberFormat( _epochMinutes, " Minute, ", " Minutes, " ) + _epochRemainderSeconds.ToString() + " Seconds Ago";
			}

			// Otherwise ...
			return "More than an hour ago";
		}

	}
}









