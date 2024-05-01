////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	HTGUI_UNDO.cs
//
//	HELL TAP EDITOR UI LIBRARY
//	Provides a wrapper for HTGUI with undo functionality built-in.
//	NOTE: ObjectField methods are actually replacements as they use generics.
//
//	© 2017 Melli Georgiou.
//	Hell Tap Entertainment LTD
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using HellTap.PoolKit;

// Use HellTap Namespace
namespace HellTap.PoolKit {

	public static class HTGUI_UNDO {

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	TEXT FIELD
		//	Conveniantly Creates Text Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Pre-formatted with Icon and Label
		public static string TextField( Object recordUndoObj, string undoText, Texture2D icon, string label, string defaultString ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Create a new variable to hold the Field
				var newField = HTGUI.TextField(icon, label, defaultString );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultString;
		}

		// (With GUILayoutOptions) This recreates how HTGUI.TextField works and includes GUIContent, layout and guistyle overrides
		public static string TextField( Object recordUndoObj, string undoText, GUIContent guicontent, string defaultString, GUILayoutOption[] options, GUIStyle overrideGUIStyle = null ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				// Setup GUIStyle
				if( overrideGUIStyle == null ){ overrideGUIStyle = GUI.skin.textField; }
			
				// Name / Title of object
				EditorGUILayout.BeginHorizontal();
					GUILayout.Label( guicontent.image, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
					GUILayout.Label( guicontent.text, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
					GUILayout.FlexibleSpace();
					var newField = EditorGUILayout.TextField ( defaultString, overrideGUIStyle, options );
				EditorGUILayout.EndHorizontal();

			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultString;
		}

		// (With GUILayoutOptions & no guicontent) This is a new wrapper for the EditorGUILayout.TextField
		public static string TextField( Object recordUndoObj, string undoText, string defaultString, GUILayoutOption[] options, GUIStyle overrideGUIStyle = null ){

			// Setup GUIStyle
			if( overrideGUIStyle == null ){ overrideGUIStyle = GUI.skin.textField; }

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				var newField = EditorGUILayout.TextField (new GUIContent("", null, ""), defaultString, overrideGUIStyle, options );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultString;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	INT FIELD
		//	Conveniantly Creates Float Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Pre-formatted with Icon and Label
		public static int IntField( Object recordUndoObj, string undoText, Texture2D icon, string label, int defaultVal ) { 
		
			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Create a new variable to hold the Field
				var newField = HTGUI.IntField(icon, label, defaultVal );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText );
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions) This is a new wrapper for the EditorGUILayout.IntField
		public static int IntField( Object recordUndoObj, string undoText, GUIContent guicontent, int defaultVal, GUILayoutOption[] options, int iconSize = 18 ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				// Begin Horizontal
				GUILayout.BeginHorizontal();

					if( guicontent.image != null ){ GUILayout.Label(guicontent.image, GUILayout.MinWidth(iconSize),GUILayout.MaxWidth(iconSize),GUILayout.MinHeight(iconSize),GUILayout.MaxHeight(iconSize) ); }
					if( guicontent.text != ""){ GUILayout.Label(guicontent.text, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(iconSize)); }
					GUILayout.FlexibleSpace();
					var newField = EditorGUILayout.IntField (new GUIContent("", null, guicontent.tooltip), defaultVal, options );

				// End Horizontal	
				GUILayout.EndHorizontal();
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions & no guicontent) This is a new wrapper for the EditorGUILayout.IntField
		public static int IntField( Object recordUndoObj, string undoText, int defaultVal, GUILayoutOption[] options ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				var newField = EditorGUILayout.IntField (new GUIContent("", null, ""), defaultVal, options );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	FLOAT FIELD
		//	Conveniantly Creates Float Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Pre-formatted with Icon and Label
		public static float FloatField( Object recordUndoObj, string undoText, Texture2D icon, string label, float defaultVal ) { 
		
			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Create a new variable to hold the Field
				var newField = HTGUI.FloatField(icon, label, defaultVal );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText );
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions) This is a new wrapper for the EditorGUILayout.FloatField
		public static float FloatField( Object recordUndoObj, string undoText, GUIContent guicontent, float defaultVal, GUILayoutOption[] options, int iconSize = 18 ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				// Begin Horizontal
				GUILayout.BeginHorizontal();

					if( guicontent.image != null ){ GUILayout.Label(guicontent.image, GUILayout.MinWidth(iconSize),GUILayout.MaxWidth(iconSize),GUILayout.MinHeight(iconSize),GUILayout.MaxHeight(iconSize) ); }
					if( guicontent.text != ""){ GUILayout.Label(guicontent.text, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(iconSize)); }
					GUILayout.FlexibleSpace();
					var newField = EditorGUILayout.FloatField (new GUIContent("", null, guicontent.tooltip), defaultVal, options );

				// End Horizontal	
				GUILayout.EndHorizontal();

			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions & no guicontent) This is a new wrapper for the EditorGUILayout.FloatField
		public static float FloatField( Object recordUndoObj, string undoText, float defaultVal, GUILayoutOption[] options ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				var newField = EditorGUILayout.FloatField (new GUIContent("", null, ""), defaultVal, options );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	SLIDER FLOAT FIELD
		//	Conveniantly Creates Slider Float Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Pre-formatted with Icon and Label
		public static float SliderField( Object recordUndoObj, string undoText, Texture2D icon, string label, float defaultVal, float min, float max ) { 
		
			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Create a new variable to hold the Field
				var newField = HTGUI.SliderField(icon, label, defaultVal, min, max );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText );
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions) This is a new wrapper for the EditorGUILayout.Slider
		public static float SliderField( Object recordUndoObj, string undoText, GUIContent guicontent, float defaultVal, float min, float max, GUILayoutOption[] options, int iconSize = 18 ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				// Begin Horizontal
				GUILayout.BeginHorizontal();

					if(guicontent.image != null ){ GUILayout.Label(guicontent.image, GUILayout.MinWidth(iconSize),GUILayout.MaxWidth(iconSize),GUILayout.MinHeight(iconSize),GUILayout.MaxHeight(iconSize) ); }
					if(guicontent.text != ""){ GUILayout.Label(guicontent.text, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(iconSize)); }
					GUILayout.FlexibleSpace();
					var newField = EditorGUILayout.Slider (new GUIContent("", null, guicontent.tooltip), defaultVal, min, max,options );

				// End Horizontal	
				GUILayout.EndHorizontal();

			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions & no guicontent) This is a new wrapper for the EditorGUILayout.Slider
		public static float SliderField( Object recordUndoObj, string undoText, float defaultVal, float min, float max, GUILayoutOption[] options ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				var newField = EditorGUILayout.Slider (new GUIContent("", null, ""), defaultVal, min, max, options );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	VECTOR2 FIELD
		//	Conveniantly Creates Vector2 Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Pre-formatted with Icon and Label
		public static Vector2 Vector3Field( Object recordUndoObj, string undoText, Texture2D icon, string label, Vector2 defaultVal ) { 
			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Create a new variable to hold the Field
				var newField = HTGUI.Vector2Field(icon, label, defaultVal );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText );
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions) This is a new wrapper for the EditorGUILayout.Vector3
		public static Vector2 Vector2Field( Object recordUndoObj, string undoText, GUIContent guicontent, Vector2 defaultVal, GUILayoutOption[] options, int iconSize = 18 ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
					
				// Begin Horizontal
				GUILayout.BeginHorizontal();
					
					if(guicontent.image != null ){ GUILayout.Label(guicontent.image, GUILayout.MinWidth(iconSize),GUILayout.MaxWidth(iconSize),GUILayout.MinHeight(iconSize),GUILayout.MaxHeight(iconSize) ); }
					if( guicontent.text != ""){ GUILayout.Label(guicontent.text, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(iconSize)); }
					GUILayout.FlexibleSpace();
					var newField = EditorGUILayout.Vector2Field (new GUIContent("", null, guicontent.tooltip), defaultVal, options );

				// End Horizontal	
				GUILayout.EndHorizontal();

			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions & no guicontent) This is a new wrapper for the EditorGUILayout.Vector3
		public static Vector2 Vector3Field( Object recordUndoObj, string undoText, Vector2 defaultVal, GUILayoutOption[] options ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				var newField = EditorGUILayout.Vector2Field (new GUIContent("", null, ""), defaultVal, options );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	VECTOR3 FIELD
		//	Conveniantly Creates Vector3 Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Pre-formatted with Icon and Label
		public static Vector3 Vector3Field( Object recordUndoObj, string undoText, Texture2D icon, string label, Vector3 defaultVal ) { 
			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Create a new variable to hold the Field
				var newField = HTGUI.Vector3Field(icon, label, defaultVal );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText );
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions) This is a new wrapper for the EditorGUILayout.Vector3
		public static Vector3 Vector3Field( Object recordUndoObj, string undoText, GUIContent guicontent, Vector3 defaultVal, GUILayoutOption[] options, int iconSize = 18 ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
					
				// Begin Horizontal
				GUILayout.BeginHorizontal();
					
					if(guicontent.image != null ){ GUILayout.Label(guicontent.image, GUILayout.MinWidth(iconSize),GUILayout.MaxWidth(iconSize),GUILayout.MinHeight(iconSize),GUILayout.MaxHeight(iconSize) ); }
					if( guicontent.text != ""){ GUILayout.Label(guicontent.text, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(iconSize)); }
					GUILayout.FlexibleSpace();
					var newField = EditorGUILayout.Vector3Field (new GUIContent("", null, guicontent.tooltip), defaultVal, options );

				// End Horizontal	
				GUILayout.EndHorizontal();

			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions & no guicontent) This is a new wrapper for the EditorGUILayout.Vector3
		public static Vector3 Vector3Field( Object recordUndoObj, string undoText, Vector3 defaultVal, GUILayoutOption[] options ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				var newField = EditorGUILayout.Vector3Field (new GUIContent("", null, ""), defaultVal, options );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	TOGGLE FIELD
		//	Conveniantly Creates bool Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Pre-formatted with Icon and Label
		public static bool ToggleField( Object recordUndoObj, string undoText, Texture2D icon, string label, bool defaultVal ) { 
		
			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Create a new variable to hold the Field
				var newField = HTGUI.ToggleField(icon, label, defaultVal );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText );
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions) This is a new wrapper for the EditorGUILayout.Toggle bool
		public static bool ToggleField( Object recordUndoObj, string undoText, GUIContent guicontent, bool defaultVal, GUILayoutOption[] options, int iconSize = 18 ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				// Begin Horizontal
				GUILayout.BeginHorizontal();

					if( guicontent.image != null ){ GUILayout.Label(guicontent.image, GUILayout.MinWidth(iconSize),GUILayout.MaxWidth(iconSize),GUILayout.MinHeight(iconSize),GUILayout.MaxHeight(iconSize) ); }
					if( guicontent.text != ""){ GUILayout.Label(guicontent.text, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(iconSize)); }
					GUILayout.FlexibleSpace();
					var newField = EditorGUILayout.Toggle (new GUIContent("", null, guicontent.tooltip), defaultVal, options );

				// End Horizontal	
				GUILayout.EndHorizontal();

			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}

		// (With GUILayoutOptions & no guicontent) This is a new wrapper for the EditorGUILayout.Toggle bool
		public static bool ToggleField( Object recordUndoObj, string undoText, bool defaultVal, GUILayoutOption[] options ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				var newField = EditorGUILayout.Toggle (new GUIContent("", null, ""), defaultVal, options );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultVal;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	ENUM FIELD
		//	Conveniantly Creates Enum Popup Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Pre-formatted with Icon and Label
		public static System.Enum EnumField( Object recordUndoObj, string undoText, Texture2D icon, string label, System.Enum defaultEnum){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Create a new variable to hold the Field
				var newField = HTGUI.EnumField(icon, label, defaultEnum );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultEnum;
		}

		// (With GUILayoutOptions) This is a new wrapper for the EditorGUILayout.EnumPopup EnumPopup
		public static System.Enum EnumField( Object recordUndoObj, string undoText, GUIContent guicontent, System.Enum defaultEnum, GUILayoutOption[] options, int iconSize = 18 ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				// Begin Horizontal
				GUILayout.BeginHorizontal();

					if( guicontent.image != null ){ GUILayout.Label(guicontent.image, GUILayout.MinWidth(iconSize),GUILayout.MaxWidth(iconSize),GUILayout.MinHeight(iconSize),GUILayout.MaxHeight(iconSize) ); }
					if( guicontent.text != ""){ GUILayout.Label(guicontent.text, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(iconSize)); }
					GUILayout.FlexibleSpace();
					var newField = EditorGUILayout.EnumPopup (new GUIContent("", null, guicontent.tooltip), defaultEnum, options );

				// End Horizontal	
				GUILayout.EndHorizontal();
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultEnum;
		}

		// (With GUILayoutOptions & no guicontent) This is a new wrapper for the EditorGUILayout.EnumPopup EnumPopup
		public static System.Enum EnumField( Object recordUndoObj, string undoText, System.Enum defaultEnum, GUILayoutOption[] options ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				var newField = EditorGUILayout.EnumPopup (new GUIContent("", null, ""), defaultEnum, options );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultEnum;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	MASK FIELD
		//	Conveniantly Creates Layer / Custom Mask Field Objects
		//	NOTE: This doesn't work when trying to use it for LayerMasks.
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Pre-formatted with Icon and Label
		public static int MaskField( Object recordUndoObj, string undoText, Texture2D icon, string label, int defaultMask, string[] displayedOption = null ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Create a new variable to hold the Field
				var newField = HTGUI.MaskField(icon, label, defaultMask, displayedOption );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultMask;
		}

		// (With GUILayoutOptions) This is a new wrapper for the EditorGUILayout.MaskField
		public static int MaskField( Object recordUndoObj, string undoText, GUIContent guicontent, int defaultMask, GUILayoutOption[] options, string[] displayedOption = null, int iconSize = 18 ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				// Begin Horizontal
				GUILayout.BeginHorizontal();

					if( guicontent.image != null ){ GUILayout.Label(guicontent.image, GUILayout.MinWidth(iconSize),GUILayout.MaxWidth(iconSize),GUILayout.MinHeight(iconSize),GUILayout.MaxHeight(iconSize) ); }
					if( guicontent.text != ""){ GUILayout.Label(guicontent.text, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(iconSize)); }
					GUILayout.FlexibleSpace();

					// Assume we want to use this as a LayerMask if the displayedOption is blank
					if( displayedOption == null ){
						displayedOption = new string[32];
						for ( int i = 0; i < 32; i++ ) {
							displayedOption[i] = LayerMask.LayerToName( i );
						}
					}

					// Show MaskField
					var newField = EditorGUILayout.MaskField (new GUIContent("", null, guicontent.tooltip), defaultMask, displayedOption, options );

				// End Horizontal	
				GUILayout.EndHorizontal();
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultMask;
		}

		// (With GUILayoutOptions & no guicontent) This is a new wrapper for the EditorGUILayout.MaskField
		public static int MaskField( Object recordUndoObj, string undoText, int defaultMask, GUILayoutOption[] options, string[] displayedOption = null ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Assume we want to use this as a LayerMask if the displayedOption is blank
				if( displayedOption == null ){
					displayedOption = new string[32];
					for ( int i = 0; i < 32; i++ ) {
						displayedOption[i] = LayerMask.LayerToName( i );
					}
				}

				// Show MaskField
				var newField = EditorGUILayout.MaskField (new GUIContent("", null, ""), defaultMask, displayedOption, options );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultMask;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	LAYER FIELD
		//	Conveniantly Creates Layer Fields (UNTESTED)
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Pre-formatted with Icon and Label
		public static int LayerField( Object recordUndoObj, string undoText, Texture2D icon, string label, int defaultLayer ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Create a new variable to hold the Field
				var newField = HTGUI.LayerField(icon, label, defaultLayer );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultLayer;
		}

		// (With GUILayoutOptions) This is a new wrapper for the EditorGUILayout.LayerField
		public static int LayerField( Object recordUndoObj, string undoText, GUIContent guicontent, int defaultLayer, GUILayoutOption[] options, int iconSize = 18 ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				// Begin Horizontal
				GUILayout.BeginHorizontal();

					if( guicontent.image != null ){ GUILayout.Label(guicontent.image, GUILayout.MinWidth(iconSize),GUILayout.MaxWidth(iconSize),GUILayout.MinHeight(iconSize),GUILayout.MaxHeight(iconSize) ); }
					if( guicontent.text != ""){ GUILayout.Label(guicontent.text, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(iconSize)); }
					GUILayout.FlexibleSpace();
					var newField = EditorGUILayout.LayerField (new GUIContent("", null, guicontent.tooltip), defaultLayer, options );

				// End Horizontal	
				GUILayout.EndHorizontal();
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultLayer;
		}

		// (With GUILayoutOptions & no guicontent) This is a new wrapper for the EditorGUILayout.LayerField
		public static int LayerField( Object recordUndoObj, string undoText, int defaultLayer, GUILayoutOption[] options ){

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();
			
				var newField = EditorGUILayout.LayerField (new GUIContent("", null, ""), defaultLayer, options );
			
			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newField;
			}

			// If anything goes wrong, return the original value
			return defaultLayer;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	OBJECT FIELD
		//	NOTE: The Object Fields are not wrappers but in fact replacements (Generics are tricky to pass and return)
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Generic Version - this should work with all types that can be converted to UnityEngine.Object
		public static T ObjectField<T>( Object recordUndoObj, string undoText, Texture2D icon, string label, T defaultVal, bool allowSceneObjects ) where T : UnityEngine.Object {

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Name / Title of object
				EditorGUILayout.BeginHorizontal();
					GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
					GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
					var newObjectField = EditorGUILayout.ObjectField( (UnityEngine.Object)defaultVal, typeof(T), allowSceneObjects) as T;
				EditorGUILayout.EndHorizontal();

			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newObjectField;
			}

			// Return the new defaultVal.
			return defaultVal;
		}

		// Generic Version (with GUILayout options) - this should work with all types that can be converted to UnityEngine.Object
		public static T ObjectField<T>( Object recordUndoObj, string undoText, GUIContent guiContent, T defaultVal, bool allowSceneObjects, GUILayoutOption[] options ) where T : UnityEngine.Object {

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				// Begin Horizontal
				GUILayout.BeginHorizontal();

					GUILayout.Label(guiContent.image, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
					GUILayout.Label(guiContent.text, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
					GUILayout.FlexibleSpace();
					var newObjectField = EditorGUILayout.ObjectField( new GUIContent("", null, guiContent.tooltip), (UnityEngine.Object)defaultVal, typeof(T), allowSceneObjects, options) as T;

				// End Horizontal	
				GUILayout.EndHorizontal();

			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newObjectField;
			}

			// Return the new defaultVal.
			return defaultVal;
		}

		// Generic Version (with GUILayout options no GUIContent) - should work with all types that can be converted to UnityEngine.Object
		public static T ObjectField<T>( Object recordUndoObj, string undoText, T defaultVal, bool allowSceneObjects, GUILayoutOption[] options ) where T : UnityEngine.Object {

			// Create a new variable to hold the Field
			EditorGUI.BeginChangeCheck();

				var newObjectField = EditorGUILayout.ObjectField( new GUIContent("", null, ""), (UnityEngine.Object)defaultVal, typeof(T), allowSceneObjects, options) as T;

			// If a GUI Control has been updated while we updated the above value, record the undo!
			if (EditorGUI.EndChangeCheck()){

				// Record the undo object and set the reference to the new value
				Undo.RecordObject ( recordUndoObj, undoText);
				return newObjectField;
			}

			// Return the new defaultVal.
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	UNITY EVENT
		//	This allows Unity Events to be done in a single line.
		//	Example:	HTGUI_UNDO.UnityEventField( serializedObject, "testEvent" );
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void UnityEventField( SerializedObject serializedObject, string nameOfUnityEvent ){

			// Find the property corresponding to the UnityEvent we want to edit.
	        var prop = serializedObject.FindProperty( nameOfUnityEvent );

	        // Draw the Inspector widget for this property.
	        EditorGUILayout.PropertyField(prop, true);

	        // Commit changes to the property back to the component we're editing.
	        serializedObject.ApplyModifiedProperties();
		}

	}
}







