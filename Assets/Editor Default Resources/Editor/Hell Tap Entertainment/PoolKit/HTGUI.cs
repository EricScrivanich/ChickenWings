////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	HTGUI.cs
//
//	HELL TAP EDITOR UI LIBRARY
//	Allows for easier GUI Creation by abstracting some of the GUI fields.
//	v2 -> Updated for PoolKit.
//
//	© 2015 - 2020 Melli Georgiou.
//	Hell Tap Entertainment LTD
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	INCLUDES:
//
//	GUI DESIGN / SPACER
//	Functions for spacing out boxes and layout.
//
//	GUI EDITOR FIELDS
//	LDC-Styled generic Editor fields
//
//	SPECIAL GUI EDITOR FUNCTIONS
//	Helps with special editor fields - Arrays, etc.
//
//	RESIZE ARRAY FUNCTIONS
//	Helps with resizing builtin Arrays of various types.
//
//	EDITOR WINDOWS
//	Helps to set an icon / title to editor windows.
//	
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using HellTap.PoolKit;

// Use HellTap Namespace
namespace HellTap.PoolKit {

	// Class
	public class HTGUI : Editor {

		// Textures
		static string loadPrefix = "Hell Tap Entertainment/PoolKit/Shared/"; // We have to do this to fix syntaxing colors (weird)	
		public static Texture2D hellTapIcon = EditorGUIUtility.Load(loadPrefix+"HellTapEditor.png") as Texture2D; 	
		public static Texture2D addButton = EditorGUIUtility.Load(loadPrefix+"addButton.png") as Texture2D; 
		public static Texture2D removeButton = EditorGUIUtility.Load(loadPrefix+"removeButton.png") as Texture2D; 
		
		// Default Values
		public static int defaultMinSizeX = 160;
		public static int defaultMaxSizeX = 160;
		public static int defaultMaxSizeY = 20;

	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//	GUI DESIGN / SPACER FUNCTIONS
	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// SEPLINE
		// Draws a seperator Line
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static void SepLine( GUIStyle guiStyle = null, bool addSpaceAtEnd = true ){

			// Draw Line
			if( guiStyle == null ){
				GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1));	
			} else {
				GUILayout.Box(string.Empty, guiStyle, GUILayout.ExpandWidth(true), GUILayout.Height(1));	
			}
			

			// Add vertical space at the end of title.
			if(addSpaceAtEnd){ GUILayout.Label(string.Empty, GUILayout.MaxHeight(5)); }
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	BIG Layout Functions
		//	Conveniantly begins and adds big boxes with spacing.
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static void StartBigLayout(){
			EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal();
					GUILayout.Label(string.Empty, GUILayout.MaxWidth(5)); // Extra space
					EditorGUILayout.BeginVertical();
		}

		public static void EndBigLayout( int extraSpace = 0 ){
					EditorGUILayout.EndVertical();
					GUILayout.Label(string.Empty, GUILayout.MaxWidth(5+extraSpace)); // Extra space
				EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	White Box Functions
		//	Conveniantly begins and adds boxes with spacing.
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static void StartWhiteBox(){

			#if UNITY_2019_3_OR_NEWER

				// Setup a new modified box skin for Unity's new GUI
				GUIStyle modifiedBox = GUI.skin.GetStyle("Box");

				// If we're not using the lighter UI skin, preserve the white backgrounds we used to have
				if( EditorGUIUtility.isProSkin == false ){
					modifiedBox.normal.background = Texture2D.whiteTexture;
				}

				// Create the group normally using the modified box style
				EditorGUILayout.BeginHorizontal( modifiedBox );
					GUILayout.Label(string.Empty, GUILayout.MaxWidth(5));
					EditorGUILayout.BeginVertical();
						GUILayout.Label(string.Empty, GUILayout.MaxHeight(5));


			#else

				EditorGUILayout.BeginHorizontal("Box");
					GUILayout.Label(string.Empty, GUILayout.MaxWidth(5));
					EditorGUILayout.BeginVertical();
						GUILayout.Label(string.Empty, GUILayout.MaxHeight(5));

			#endif
		}

		public static void EndWhiteBox( bool extraSpaceAtBottom = true ){			
					GUILayout.Label(string.Empty, GUILayout.MaxHeight(5));
				EditorGUILayout.EndVertical();
				GUILayout.Label(string.Empty, GUILayout.MaxWidth(5));	
			EditorGUILayout.EndHorizontal();

			// Add vertical space at the end of every box.
			if(extraSpaceAtBottom){ GUILayout.Label(string.Empty, GUILayout.MaxHeight(5)); }
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	Custom Box Functions
		//	Create a custom box with custom margins, padding and GUIStyle. We can use the "End White Box" to close this.
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static void StartCustomBox( 	int marginL = 0,  int marginR = 0,  int marginT = 0,  int marginB = 0,
		 									int paddingL = 0,  int paddingR = 0,  int paddingT = 0,  int paddingB = 0,
		 									GUIStyle guistyle = null
		){

			GUIStyle boxStyle = new GUIStyle( guistyle == null ? GUI.skin.box : guistyle  );
			boxStyle.margin = new RectOffset(marginL,marginR,marginT,marginB);
			boxStyle.padding = new RectOffset(paddingL,paddingR,paddingT,paddingB);

			EditorGUILayout.BeginHorizontal( boxStyle );
				GUILayout.Label(string.Empty, GUILayout.MaxWidth(5));
				EditorGUILayout.BeginVertical();
					GUILayout.Label(string.Empty, GUILayout.MaxHeight(5));
		}

		// End Custom Box just sends it to the standard EndWhiteBox function as it works the same way.
		public static void EndCustomBox( bool extraSpaceAtBottom = true ){ EndWhiteBox(extraSpaceAtBottom); }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	Start Indent
		//	Adds a horizontal Indent - supports default and custom sized indents
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void StartIndent(){
			StartIndent(16);
		}

		public static void StartIndent( int indent ){
			// Start Indent
			GUILayout.BeginHorizontal();
			GUILayout.Label(string.Empty, GUILayout.MinWidth(indent), GUILayout.MaxWidth(indent) );
			GUILayout.BeginVertical();
		}

		public static void EndIndent(){
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	Color To Hex
		//	We use this function to insert a color as a hex, also handles the alpha channel too!
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
		public static string ColorToHex( Color32 color  ){ return ColorToHex( color, color.a ); }
		public static string ColorToHex( Color32 color, float maxAlpha ){
			
			// Handle Alpha
			maxAlpha = Mathf.Clamp(maxAlpha, 0, 1 );					// Make sure the supplied alpha is within range (0-1)

			// The alpha channel is actually a byte with a range of 0 - 225
			int alpha = color.a;	// This seems to convert the byte into an int.
			if( alpha > maxAlpha*255 ){ alpha = Mathf.RoundToInt(maxAlpha*255); }
			//Debug.Log(alpha);

			// Make sure the hex is lowercase to work
			// NOTE: The trick to get the alpha working is to multiply it by 255.
			string hex = ( color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + (alpha).ToString("X2") ).ToLower(); 
			return hex;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	Header
		//	Template for most Editor Headers
		//	NOTE: Updated in v2 to have custom icon sizes, word-wrapping and product miniTitle.
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void Header( Texture2D headerIcon, string title, string subtitle, string miniTitle = "", int iconSize = 64, int titleSize = 0 ){

			// Vertical Space
			GUILayout.Label(string.Empty, GUILayout.MaxHeight(5));

			// Do Title	
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(string.Empty, GUILayout.MaxWidth(5));
				GUILayout.Label(headerIcon, GUILayout.MaxWidth(iconSize), GUILayout.MaxHeight(iconSize) );
				EditorGUILayout.BeginVertical();
					
				// Setup a new text style 
				GUIStyle textStyle = new GUIStyle( GUI.skin.label );
				textStyle.richText = true;
				textStyle.wordWrap = true;
				string HalfColorHex = ColorToHex( textStyle.normal.textColor, 0.75f );

				// Create Text Area with custom GUIStyle
				GUILayout.Label( 
									// Do Mini Title: eg "DialogKit"
									( miniTitle != string.Empty ? "<size=9><color=#" + HalfColorHex + "><B>" + miniTitle + "</B></color></size>\n" : string.Empty ) +

									// Do Title (with or without a custom title size)
									( titleSize == 0 ? "<B>" + title + "</B>\n\n" : "<B><size="+titleSize.ToString()+">" + title + "</size></B>\n\n" ) +
									
									// Finally, do Subtitle, and use the custom textstyle for the whole thing.
									subtitle, textStyle);

				EditorGUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			// Add vertical space at the end of title.
			GUILayout.Label(string.Empty, GUILayout.MaxHeight(5));
		}

		// This version takes a Vector2 to set the icon
		public static void Header( Texture2D headerIcon, string title, string subtitle, string miniTitle, Vector2 iconSize, int titleSize = 0 ){

			// Vertical Space
			GUILayout.Label(string.Empty, GUILayout.MaxHeight(5));

			// Do Title	
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(string.Empty, GUILayout.MaxWidth(5));
				GUILayout.Label(headerIcon, GUILayout.MaxWidth(iconSize.x), GUILayout.MaxHeight(iconSize.y) );
				EditorGUILayout.BeginVertical();
					
				// Setup a new text style 
				GUIStyle textStyle = new GUIStyle( GUI.skin.label );
				textStyle.richText = true;
				textStyle.wordWrap = true;
				string HalfColorHex = ColorToHex( textStyle.normal.textColor, 0.75f );

				// Create Text Area with custom GUIStyle
				GUILayout.Label( 
									// Do Mini Title: eg "DialogKit"
									( miniTitle != string.Empty ? "<size=9><color=#" + HalfColorHex + "><B>" + miniTitle + "</B></color></size>\n" : string.Empty ) +

									// Do Title (with or without a custom title size)
									( titleSize == 0 ? "<B>" + title + "</B>\n\n" : "<B><size="+titleSize.ToString()+">" + title + "</size></B>\n\n" ) +
									
									// Finally, do Subtitle, and use the custom textstyle for the whole thing.
									subtitle, textStyle);

				EditorGUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			// Add vertical space at the end of title.
			GUILayout.Label(string.Empty, GUILayout.MaxHeight(5));
		}

	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//	BODY TEMPLATE
	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	Start Body Template
		//	This is a shortcut function that caches and verifies the custom component, starts the layout options and returns it
		//	NOTE: This version is updated for HTGUI V2. It fixes a bug where multiple components wouldn't work using this method.
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static Object StartBody( Object target ){
			
			// Make sure we have selected a GameObject
			if( target != null ){

				// Cache the component as an Object
				Object o = target;
				if( o != null ){

					// Start Layout and White Box, and return the object
					StartBigLayout();
					StartWhiteBox();
					return o;
				}

			}

			// Return null if something went wrong
			return null;
		}

		// This is a version without checking if the object is valid
		public static void StartBody(){
			
			// Start Layout and White Box, and return the object
			StartBigLayout();
			StartWhiteBox();

		}


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	End Body Template
		//	This is a shortcut function that ends the layout of the main body of the custom editor
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void EndBody(){

			// End Layout
			EndWhiteBox();
			EndBigLayout();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	Section Title
		//	Shows a Bold Title and a subtitle underneith it.
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void SectionTitle( string title, string subtitle, int titleSize = 0 ){

			// Setup a new text style 
			GUIStyle textStyle = new GUIStyle( GUI.skin.label );
			textStyle.richText = true;
			textStyle.wordWrap = true;

			// Create Text Area with custom GUIStyle
			if( titleSize == 0 ){
				GUILayout.Label( "<B>" + title + "</B>\n\n" + subtitle, textStyle);
			} else {
				GUILayout.Label( "<B><size=" + titleSize.ToString() + ">" + title + "</size></B>\n\n" + subtitle, textStyle);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	Wrapped Text Label
		//	Shows some wrapped text
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void WrappedTextLabel( string text ){

			// Setup a new text style 
			GUIStyle textStyle = new GUIStyle( GUI.skin.label );
			textStyle.richText = true;
			textStyle.wordWrap = true;
			GUILayout.Label( text, textStyle );
			
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	Section Title With Icon
		//	Shows a Bold Title and a subtitle underneith it.
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void SectionTitle( Texture2D icon, string title, string subtitle, int iconSize = 64, int titleSize = 0 ){

			// Setup a new text style 
			GUIStyle textStyle = new GUIStyle( GUI.skin.label );
			textStyle.richText = true;
			textStyle.wordWrap = true;

			// Start Horizontal Row
			GUILayout.BeginHorizontal();

				// Do Icon
				if(icon){
					GUILayout.Label(icon, GUILayout.MinWidth(iconSize), GUILayout.MaxWidth(iconSize), GUILayout.MinHeight(iconSize), GUILayout.MaxHeight(iconSize) );
					GUILayout.Space(8);
				}

				// Create Text Area with custom GUIStyle
				if( titleSize == 0 ){
					GUILayout.Label( "<B>" + title + "</B>\n\n" + subtitle, textStyle);
				} else {
					GUILayout.Label( "<B><size=" + titleSize.ToString() + ">" + title + "</size></B>\n\n" + subtitle, textStyle);
				}
	
			// Start Horizontal Row
			GUILayout.EndHorizontal();
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	Mini Header
		//	Shows a Bold Title and a small icon next to it
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void MiniHeader( Texture2D icon, string title, int iconSize = 18 ){

			EditorGUILayout.BeginHorizontal();
				GUILayout.Label( icon, GUILayout.MinWidth(iconSize),GUILayout.MaxWidth(iconSize),GUILayout.MinHeight(iconSize),GUILayout.MaxHeight(iconSize));
				GUIStyle boldHeader = new GUIStyle(GUI.skin.label);
				boldHeader.fontStyle = FontStyle.Bold;
				GUILayout.Label(title, boldHeader, GUILayout.MinHeight(iconSize), GUILayout.MaxHeight(iconSize));
			EditorGUILayout.EndHorizontal();
		}

	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//	GUI EDITOR FUNCTIONS
	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	TEXT FIELD
		//	Conveniantly Creates Text Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static string TextField( Texture2D icon, string label, string defaultString ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
				defaultString = EditorGUILayout.TextField (string.Empty, defaultString);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultString.
			return defaultString;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	TEXT FIELD (MULTILINE)
		//	Conveniantly Creates Text Field Objects with multi-lines. Supports with or without a custom value
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static string TextFieldMuliLine( Texture2D icon, string label, string defaultString  ) { 
			defaultString = TextFieldMuliLine( icon, label, defaultString, 60);
			return defaultString;
		}

		public static string TextFieldMuliLine( Texture2D icon, string label, string defaultString, int height ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
				defaultString = EditorGUILayout.TextField(string.Empty, defaultString, GUILayout.MinHeight(height) );
			EditorGUILayout.EndHorizontal();

			// Return the new defaultString.
			return defaultString;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	INT FIELD
		//	Conveniantly Creates Float Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static int IntField( Texture2D icon, string label, int defaultVal ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
				defaultVal = EditorGUILayout.IntField (string.Empty, defaultVal);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	 FLOAT FIELD
		//	Conveniantly Creates Float Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static float FloatField( Texture2D icon, string label, float defaultVal ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
				defaultVal = EditorGUILayout.FloatField (string.Empty, defaultVal);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	SLIDER FIELD
		//	Conveniantly Creates Slider (Float) Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Forward this function using the default values
		public static float SliderField( Texture2D icon, string label, float defaultVal, float min, float max ) { 
			return SliderField( icon, label, defaultVal, min, max, defaultMinSizeX, defaultMaxSizeX, defaultMaxSizeY ); 
		}

		// Full Function with all arguments
		public static float SliderField( Texture2D icon, string label, float defaultVal, float min, float max, int minSizeX, int maxSizeX, int maxHeight ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(minSizeX), GUILayout.MaxWidth(maxSizeX), GUILayout.MaxHeight(maxHeight));
				defaultVal = EditorGUILayout.Slider (string.Empty, defaultVal, min, max);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	INT SLIDER FIELD
		//	Conveniantly Creates Slider (Float) Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Forward this function using the default values
		public static int IntSliderField( Texture2D icon, string label, int defaultVal, int min, int max ) { 
			return IntSliderField( icon, label, defaultVal, min, max, defaultMinSizeX, defaultMaxSizeX, defaultMaxSizeY ); 
		}

		// Full Function with all arguments
		public static int IntSliderField( Texture2D icon, string label, int defaultVal, int min, int max , int minSizeX, int maxSizeX, int maxHeight ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(minSizeX), GUILayout.MaxWidth(maxSizeX), GUILayout.MaxHeight(maxHeight));
				defaultVal = EditorGUILayout.IntSlider (string.Empty, defaultVal, min, max);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	 TOGGLE FIELD
		//	Conveniantly Creates Boolean Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// Forward this function using the default values
		public static bool ToggleField( Texture2D icon, string label, bool defaultBool ) { 
			return ToggleField( icon, label, defaultBool, defaultMinSizeX, defaultMaxSizeX, defaultMaxSizeY ); 
		}

		// Full Function with all arguments
		public static bool ToggleField( Texture2D icon, string label, bool defaultBool, int minSizeX, int maxSizeX, int maxHeight ) { 
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(minSizeX), GUILayout.MaxWidth(maxSizeX), GUILayout.MaxHeight(maxHeight));
				defaultBool = EditorGUILayout.Toggle (string.Empty, defaultBool);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultString.
			return defaultBool;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	 ENUM FIELD
		//	Conveniantly Creates Enum Popup Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static System.Enum EnumField( Texture2D icon, string label, System.Enum defaultEnum ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				if(label!=string.Empty){GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));}
				defaultEnum = EditorGUILayout.EnumPopup (string.Empty, defaultEnum);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultString.
			return defaultEnum;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	MASK FIELD
		//	Conveniantly Creates Layer Mask Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static int MaskField( Texture2D icon, string label, int defaultValue, string[] displayedOption = null ) { 
		
			// Assume we want to use this as a LayerMask if the displayedOption is blank
			if( displayedOption == null ){
				displayedOption = new string[32];
				for ( int i = 0; i < 32; i++ ) {
					displayedOption[i] = LayerMask.LayerToName( i );
				}
			}

			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				if(label!=string.Empty){GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));}
				defaultValue = EditorGUILayout.MaskField (string.Empty, defaultValue, displayedOption );
			EditorGUILayout.EndHorizontal();

			// Return the new defaultString.
			return defaultValue;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	LAYER FIELD
		//	Conveniantly Creates Layer Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static int LayerField( Texture2D icon, string label, int defaultValue ) { 

			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				if(label!=string.Empty){GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));}
				defaultValue = EditorGUILayout.LayerField (string.Empty, defaultValue );
			EditorGUILayout.EndHorizontal();

			// Return the new defaultString.
			return defaultValue;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	 VECTOR2 FIELD
		//	Conveniantly Creates Float Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static Vector2 Vector2Field( Texture2D icon, string label, Vector2 defaultVal ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));			
				defaultVal = EditorGUILayout.Vector2Field (string.Empty, defaultVal);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	 VECTOR3 FIELD
		//	Conveniantly Creates Float Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static Vector3 Vector3Field( Texture2D icon, string label, Vector3 defaultVal ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));			
				defaultVal = EditorGUILayout.Vector3Field (string.Empty, defaultVal);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	 OBJECT FIELD - COLOR
		//	Conveniantly Creates Color Field Objects
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Color
		public static Color ColorField( Texture2D icon, string label, Color defaultVal  ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
				defaultVal = EditorGUILayout.ColorField(defaultVal);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;
		}

		// Color with overrides
		public static Color ColorField( Texture2D icon, string label, Color defaultVal, int xSize  ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
				GUILayout.FlexibleSpace();
				defaultVal = EditorGUILayout.ColorField(defaultVal, GUILayout.MinWidth(xSize), GUILayout.MaxWidth(xSize) );
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	 FOLDOUT FIELD
		//	Conveniantly Creates Boolean fold out fields
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		public static bool FoldOut( Texture2D icon, string label, bool defaultVal  ) { 

			// Horizontal Group
			EditorGUILayout.BeginHorizontal();
				if(icon!=null){
					GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
					GUILayout.Label(string.Empty, GUILayout.MinWidth(6), GUILayout.MaxWidth(6), GUILayout.MaxHeight(20) );
				}
				defaultVal = EditorGUILayout.Foldout(defaultVal, " "+label);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;	
		}

		public static bool FoldOutBold( Texture2D icon, string label, bool defaultVal  ) { 

			// Horizontal Group
			EditorGUILayout.BeginHorizontal();
				if(icon!=null){
					GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
					GUILayout.Label(string.Empty, GUILayout.MinWidth(6), GUILayout.MaxWidth(6), GUILayout.MaxHeight(20) );
				}

				GUIStyle theBoldStyle = new GUIStyle(EditorStyles.foldout);
				theBoldStyle.fontStyle = FontStyle.Bold;
				defaultVal = EditorGUILayout.Foldout(defaultVal, " "+label, theBoldStyle);
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;	
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	 SELECTION GRID FIELD
		//	Conveniantly Creates a set of string-based tabs, with a single optional icon in the beginning
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// SELECTION GRID
		public static int HTSelectionGrid( Texture2D icon, int defaultIndex, string[] strings, int rows, bool useBigLayout ){
			
			// Begin Change Check
			EditorGUI.BeginChangeCheck();

				// Start Big Layout
				if(useBigLayout){ StartBigLayout(); }
				
				// Create a horizontal row with the icon and selection grid
				EditorGUILayout.BeginHorizontal();
					if(icon!=null){ GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) ); }
					var topic = GUILayout.SelectionGrid( defaultIndex, strings, rows );
				EditorGUILayout.EndHorizontal();	

				// Add Space and End Layout
				GUILayout.Label(string.Empty, GUILayout.MaxHeight(5));
				if(useBigLayout){ EndBigLayout(); }

			// If we tapped into a tab, unfocus any of the controls
			if (EditorGUI.EndChangeCheck()){

				// Remove focus when switching tabs
				GUI.FocusControl(null);
			}

			return topic;
		}

		// SELECTION GRID
		public static int HTSelectionGrid( Texture2D icon, int defaultIndex, GUIContent[] content, int rows, bool useBigLayout, int maxHeight = 24 ){

			// Begin Change Check
			EditorGUI.BeginChangeCheck();
			
				// Start Big Layout
				if(useBigLayout){ StartBigLayout(); }

				// Create a horizontal row with the icon and selection grid
				EditorGUILayout.BeginHorizontal();
					if(icon!=null){ GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(maxHeight) ); }
					var topic = GUILayout.SelectionGrid( defaultIndex, content, rows, GUILayout.MaxHeight(maxHeight) );
				EditorGUILayout.EndHorizontal();	

				// Add Space and End Layout
				GUILayout.Label(string.Empty, GUILayout.MaxHeight(5));
				if(useBigLayout){ EndBigLayout(); }

			// If we tapped into a tab, unfocus any of the controls
			if (EditorGUI.EndChangeCheck()){

				// Remove focus when switching tabs
				GUI.FocusControl(null);
			}

			return topic;
		}

		// SELECTION GRID (with style)
		public static int HTSelectionGrid( Texture2D icon, int defaultIndex, GUIContent[] content, int rows, bool useBigLayout, GUIStyle guiStyle, int maxHeight = 24 ){

			// Begin Change Check
			EditorGUI.BeginChangeCheck();
			
				// Start Big Layout
				if(useBigLayout){ StartBigLayout(); }

				// Create a horizontal row with the icon and selection grid
				EditorGUILayout.BeginHorizontal();
					if(icon!=null){ GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(maxHeight) ); }
					var topic = GUILayout.SelectionGrid( defaultIndex, content, rows, guiStyle, GUILayout.MaxHeight(maxHeight) );
				EditorGUILayout.EndHorizontal();	

				// Add Space and End Layout
				GUILayout.Label(string.Empty, GUILayout.MaxHeight(5));
				if(useBigLayout){ EndBigLayout(); }

			// If we tapped into a tab, unfocus any of the controls
			if (EditorGUI.EndChangeCheck()){

				// Remove focus when switching tabs
				GUI.FocusControl(null);
			}

			return topic;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	 OBJECT FIELD
		//	Conveniantly Creates Object Fields
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Generic Version - this should work with all types that can be converted to UnityEngine.Object
		public static T ObjectField<T>( Texture2D icon, string label, T defaultVal, bool allowSceneObjects ) where T : UnityEngine.Object {

			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
				defaultVal = EditorGUILayout.ObjectField( (UnityEngine.Object)defaultVal, typeof(T), allowSceneObjects) as T;
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;
		}

		// GameObject
		public static GameObject ObjectFieldGO( Texture2D icon, string label, GameObject defaultVal, bool allowSceneObjects  ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
				defaultVal = EditorGUILayout.ObjectField(defaultVal, typeof(GameObject), allowSceneObjects) as GameObject;
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;
		}

		// Transform
		public static Transform ObjectFieldTransform( Texture2D icon, string label, Transform defaultVal, bool allowSceneObjects  ) { 
		
			// Name / Title of object
			EditorGUILayout.BeginHorizontal();
				GUILayout.Label(icon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20) );
				GUILayout.Label(label, GUILayout.MinWidth(160), GUILayout.MaxWidth(160), GUILayout.MaxHeight(20));
				defaultVal = EditorGUILayout.ObjectField(defaultVal, typeof(Transform), allowSceneObjects) as Transform;
			EditorGUILayout.EndHorizontal();

			// Return the new defaultVal.
			return defaultVal;
		}


	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//	EDITOR WINDOWS
	// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//	SET WINDOW TITLE
		//	Allows us to set the icon and title text on an Editor Window
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		// static dictionary to help with the functions below
		public static Dictionary<EditorWindow, GUIContent> _winTitleContentByEditor;

		// Sets the icon of an editor window, without changing the title.
	    public static void SetWindowTitle( EditorWindow editor, Texture icon ){ 
	    	SetWindowTitle(editor, icon, null); 
	    }

	    // Sets the icon and title of an editor window.
	    public static void SetWindowTitle( EditorWindow editor, Texture icon, string title ){

			// ====================================================
			//	THE UNITY 4 ROUTINE
			// ====================================================

			#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7

		    	// Setup a placeholder for titleContent
		        GUIContent titleContent;

		        if (_winTitleContentByEditor == null){ _winTitleContentByEditor = new Dictionary<EditorWindow, GUIContent>(); }

		        if (_winTitleContentByEditor.ContainsKey(editor) ){
		            titleContent = _winTitleContentByEditor[editor];
		            if (titleContent != null) {
		                if (titleContent.image != icon) titleContent.image = icon;
		                if (title != null && titleContent.text != title) titleContent.text = title;
		                return;
		            }
		            _winTitleContentByEditor.Remove(editor);
		        }
		        
		        titleContent = GetWinTitleContent(editor);

		        if (titleContent != null) {
		            if (titleContent.image != icon){ titleContent.image = icon; }
		            if (title != null && titleContent.text != title){ titleContent.text = title; }
		            _winTitleContentByEditor.Add(editor, titleContent);
		        }

			// ====================================================
			//	THE UNITY 5+ ROUTINE - Way easier and works better!
			// ====================================================

	        #else

	        	editor.titleContent = new GUIContent(title, icon);

	        #endif
	    }

	    // Get the Editor Window's Title Content ( part of the Unity 4 routine)
	    #if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3  || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	    public static GUIContent GetWinTitleContent( EditorWindow editor ) {

	        BindingFlags bFlags = BindingFlags.Instance | BindingFlags.NonPublic;
	        PropertyInfo p = typeof(EditorWindow).GetProperty("cachedTitleContent", bFlags);
	        if (p == null) return null;
	        return p.GetValue(editor, null) as GUIContent;
	    }
	    #endif	

	}
}
