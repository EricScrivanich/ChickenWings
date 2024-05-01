////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	HTE_GUIStyles.cs
//	Shortcut and memory efficient way to get GUIStyles in Editors
//
//	Created By Melli Georgiou
//	© 2020 Hell Tap Entertainment LTD
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

	// Using Unity Editor
	using UnityEditor;

	// Using the Hell Tap Namespace
	namespace HellTap.PoolKit {
		public static class HT_GUIStyles {		

			// Shared Values
			private static RectOffset rectOffset_4444 = new RectOffset (4, 4, 4, 4);
			private static RectOffset rectOffset_8888 = new RectOffset (8, 8, 8, 8);
			private static RectOffset rectOffset_16161616 = new RectOffset (16, 16, 16, 16);

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//	REMOVE COMPILATION WARNINGS
			//	This method doesn't do anything, it just compares some of the values to remove Editor warnings
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
			private static void RemoveCompilationWarnings(){

				// Reset shared values
				rectOffset_4444 = new RectOffset (4, 4, 4, 4);
				rectOffset_8888 = new RectOffset (8, 8, 8, 8);
				rectOffset_16161616 = new RectOffset (16, 16, 16, 16);

				// Check values to remove warnings in the editor
				if( rectOffset_4444 != rectOffset_8888 && 
					rectOffset_8888 != rectOffset_16161616 &&
					_proBackgroundColor2019_3 != Color.white
				){
					// Do nothing ...
				}
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//	GET SELECTION GRID STYLE
			//	This is a fix for the new Unity 2019.3 SelectionGrid button styles
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
			// Helpers
			private static bool _selectionGridSetup = false;
			private static GUIStyle _selectionGridGUIStyle = new GUIStyle();
			
			// Method
			public static GUIStyle GetSelectionGridGUIStyle(){

				// If the bold label isn't already setup, do it now
				if(_selectionGridSetup == false ){

					// Setup the GUIStyle
					_selectionGridGUIStyle = new GUIStyle(GUI.skin.button);
					
					// Build a custom selection grid on newer versions of Unity
					#if UNITY_2019_3_OR_NEWER

						// Make the style bold and add padding
						//_selectionGridGUIStyle.fontStyle = FontStyle.Bold;
						_selectionGridGUIStyle.padding = rectOffset_4444;

						// If we're not using the lighter UI skin, make sure tabs look good.
						if( EditorGUIUtility.isProSkin == false ){
						
							// The selected tab
							_selectionGridGUIStyle.onHover.textColor = Color.white;
							_selectionGridGUIStyle.onNormal.textColor = Color.white;

							// When we're pressing or just pressed the button
							_selectionGridGUIStyle.active.textColor = Color.white;
							_selectionGridGUIStyle.onActive.textColor = Color.white;
						}

					#endif

					// Mark as setup
					_selectionGridSetup = true;
				}

				// Return it
				return _selectionGridGUIStyle;
			}

			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//	GET HELP BOX STYLE
			//	This is a fix for the new Unity 2019.3 HelpBox styles
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
			// Helpers
			private static bool _helpBoxSetup = false;
			private static GUIStyle _helpBoxGUIStyle = new GUIStyle();

			// Method
			public static GUIStyle GetHelpBoxGUIStyle(){

				// If the bold label isn't already setup, do it now
				if(_helpBoxSetup == false ){

					// Setup the GUIStyle
					_helpBoxGUIStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("HelpBox");
					_helpBoxGUIStyle.padding = rectOffset_16161616;
					_helpBoxGUIStyle.margin = rectOffset_16161616;
					_helpBoxGUIStyle.alignment = TextAnchor.MiddleCenter;

					// Mark as setup
					_helpBoxSetup = true;
				}

				// Return it
				return _selectionGridGUIStyle;
			}


			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//	DO HELP BOX
			//	Automatically create a help box based on Unity version
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
			// Helpers
			const string k_helpBoxTextStart = "<size=10><i>";
			const string k_helpBoxTextEnd =  "</i></size>";
			const string k_lineBreak = "\n";
			readonly static Color _proBackgroundColor = new Color(1f,1f,0f,0.3f);
			readonly static Color _proBackgroundColor2019_3 = new Color(1f,1f,0f,0.1f);

			// Method
			public static void DoHelpBox( string text, MessageType messageType = MessageType.Info, bool wide = true, bool addTopSpace = false, bool addBottomSpace = false ){
			
				// Add space
				if(addTopSpace){ EditorGUILayout.Space(); }

				// If this is the light skin, give the help box a yellow background
				if( EditorGUIUtility.isProSkin == false ){ GUI.backgroundColor = _proBackgroundColor; }

				// Build a custom selection grid on newer versions of Unity
				#if UNITY_2019_3_OR_NEWER

					// Update the background color in Unity 2019.3x
					if( EditorGUIUtility.isProSkin == false ){ GUI.backgroundColor = _proBackgroundColor2019_3; }

					// Add space
					EditorGUILayout.Space();

					// Make a horizontal layout using the help box style
					GUILayout.BeginHorizontal( GetHelpBoxGUIStyle() );

						// Icons can be created here
					//	GUILayout.Label( gearLabel, GUILayout.Width(20), GUILayout.Height(20) );

						// Add a tiny bit of space
					//	GUILayout.Space(4);

						// Then, create a vertical layout with a wrapped text label
						GUILayout.BeginVertical();
							HTGUI.WrappedTextLabel( k_helpBoxTextStart + text + k_helpBoxTextEnd );
						GUILayout.EndVertical();

					GUILayout.EndHorizontal();


				// Otherwise, do the help box the normal way
				#else

					// Show Help Box
					EditorGUILayout.HelpBox( k_lineBreak + text + k_lineBreak, messageType, wide );

				#endif

				
				// Restore Background Color
				if( EditorGUIUtility.isProSkin == false ){  GUI.backgroundColor = Color.white; }

				// Add space
				if(addBottomSpace){ EditorGUILayout.Space(); }
			}
		}
	}

#endif










