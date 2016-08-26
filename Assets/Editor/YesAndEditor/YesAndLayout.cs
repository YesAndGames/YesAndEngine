using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YesAndEngine.EngineUtilities;

namespace YesAndEditor {

	// Use a YesAndStylesheet to create uniform or modified generic skins for YesAndLayouts.
	public class YesAndStylesheet {

		// The default label style.
		public GUIStyle LabelStyle;

		// Default style for alternating colored rows in lists of items.
		public GUIStyle EvenRowStyle, OddRowStyle;

		// Create a new YesAndStylesheet object.
		public YesAndStylesheet () {

			LabelStyle = new GUIStyle (GUI.skin.label);

			EvenRowStyle = new GUIStyle ();
			EvenRowStyle.normal.background = UnityTools.Texture2DFromColor (new Color (0.9f, 0.9f, 0.9f, 1f));

			OddRowStyle = new GUIStyle ();
		}
	}

	// Use a YesAndLayout to efficiently create a UnityEditor GUI.
	public class YesAndLayout {

		// The YesAndStylesheet used in this layout.
		public YesAndStylesheet Stylesheet;

		// Global view scroll position.
		private Vector2 globalScrollPosition = Vector2.zero;

		// Dictionary of automatic foldout flags.
		private Dictionary<string, bool> autoFoldoutFlags = new Dictionary<string, bool> ();

		// Dictionary of automatic foldout open flags.
		private Dictionary<string, bool> foldoutUseTracker = new Dictionary<string, bool> ();

		// Instantiate a new layout object.
		public YesAndLayout () {
			Stylesheet = new YesAndStylesheet ();
		}

		// Clean up this layout object.
		public void Cleanup () {
			autoFoldoutFlags.Clear ();
			foldoutUseTracker.Clear ();
		}

		// Initialize this frame for rendering. Call this at the start of the render loop.
		public void BeginGUI () {

			// Reset the foldout use flags.
			string[] keys = new string[foldoutUseTracker.Keys.Count];
			foldoutUseTracker.Keys.CopyTo (keys, 0);
			for (int i = 0; i < keys.Length; i++) {
				foldoutUseTracker [keys [i]] = false;
			}

			// Initialize the global scroll view.
			globalScrollPosition = EditorGUILayout.BeginScrollView (globalScrollPosition);
		}

		// Clean up this frame after rendering. Call this at the end of the render loop.
		public void EndGUI () {

			// Cleanup unused foldout flags.
			string[] keys = new string[foldoutUseTracker.Keys.Count];
			foldoutUseTracker.Keys.CopyTo (keys, 0);
			for (int i = 0; i < keys.Length; i++) {
				string key = keys [i];
				if (foldoutUseTracker [key] == false) {
					autoFoldoutFlags.Remove (key);
					foldoutUseTracker.Remove (key);
				}
			}

			// End the scroll view.
			EditorGUILayout.EndScrollView ();
		}

		// Renders a heading to the GUI.
		public void Heading (string label, params GUILayoutOption[] options) {
			GUIStyle style = new GUIStyle (Stylesheet.LabelStyle);
			style.fontStyle = FontStyle.Bold;
			EditorGUILayout.LabelField (label, style, options);
		}

		// Renders sub text to the GUI.
		public void SubText (string label, params GUILayoutOption[] options) {
			GUIStyle style = new GUIStyle (Stylesheet.LabelStyle);
			style.fontStyle = FontStyle.Italic;
			EditorGUILayout.LabelField (label, style, options);
		}

		// Draws text to the context as a label.
		public void Label (string label, bool flexWidthLabel = false, params GUILayoutOption[] options) {
			if (flexWidthLabel) {
				EditorGUILayout.LabelField (label, GUILayout.MaxWidth (GetLabelWidth (label)));
				GUILayout.FlexibleSpace ();
			}
			else {
				EditorGUILayout.LabelField (label, options);
			}
		}

		// Displays a warning label to the GUI.
		public void Warning (string label, params GUILayoutOption[] options) {
			GUIStyle style = new GUIStyle (Stylesheet.LabelStyle);
			style.fontStyle = FontStyle.Italic;
			style.normal.textColor = Color.red;
			EditorGUILayout.LabelField (label, style, options);
		}

		// Draws an interactive toggle that reflects a boolean value.
		public bool Toggle (string label, bool value, bool right = true, bool flexWidthLabel = false, params GUILayoutOption[] options) {
			bool final;
			if (flexWidthLabel) {
				BH ();
				if (right) {
					Label (label, true);
				}
				final = EditorGUILayout.Toggle (GUIContent.none, value, options);
				if (!right) {
					GUILayout.FlexibleSpace ();
					EditorGUILayout.LabelField (label, GUILayout.MaxWidth (GetLabelWidth (label)));
				}
				EH ();
			}
			else {
				final = right ?
					EditorGUILayout.Toggle (label, value, options) :
					EditorGUILayout.ToggleLeft (label, value, options);
			}
			return final;
		}

		// Draws an interactive integer input field to the context.
		public int IntField (string label, int value, bool flexWidthLabel = false, params GUILayoutOption[] options) {
			int final;
			if (flexWidthLabel) {
				BH ();
				Label (label, true);
				final = EditorGUILayout.IntField (GUIContent.none, value, options);
				EH ();
			}
			else {
				final = EditorGUILayout.IntField (label, value, options);
			}
			return final;
		}

		// Draws an interactive long input field.
		public long LongField (string label, long value) {
			return EditorGUILayout.LongField (label, value);
		}

		// Draws an interactive string input field to the context.
		public string StringField (string label, string value, bool flexWidthLabel = false, params GUILayoutOption[] options) {
			string final;
			if (flexWidthLabel) {
				BH ();
				Label (label, true);
				final = EditorGUILayout.TextField (GUIContent.none, value, options);
				EH ();
			}
			else {
				final = EditorGUILayout.TextField (label, value, options);
			}
			return final;
		}

		// Draws an interactive string array editor.
		public string[] StringArrayField (string label, string[] value) {

			// Make sure that the array is instantiated.
			if (value == null) {
				value = new string[0];
			}

			// Cache a scoped copy of the array to return later.
			string[] array = value;

			// Fold the list of elements in a foldout.
			if (Foldout (label)) {

				// Cache values to use locally throughout array manipulation.
				int arrayLength = array.Length;
				int i;

				// Create a field to resize the array.
				int newLength = arrayLength;
				newLength = IntField ("Size", newLength);

				// Check if the length changed.
				if (newLength != arrayLength) {

					// Clamp the length above zero.
					if (newLength < 0) {
						newLength = 0;
					}

					// Create a new array to modify.
					string[] newArray = new string [newLength];

					// Iterate through each index in the new array.
					for (i = 0; i < newLength; i++) {

						// If possible, copy the element at the current index.
						if (i < arrayLength) {
							newArray [i] = array [i];
						}

						// Otherwise, initialize the element at the current index to the default value.
						else {
							newArray [i] = string.Empty;
						}
					}

					// Copy the new array.
					array = newArray;
					arrayLength = newLength;
				}

				// Create object fields for each element in the array.
				for (i = 0; i < arrayLength; i++) {
					array [i] = StringField ("Element " + i, array [i]);
				}
			}

			// Return the array.
			return array;
		}

		// Draws an interactive color input field.
		public Color ColorField (string label, Color value, bool flexWidthLabel = false, params GUILayoutOption[] options) {
			Color final;
			if (flexWidthLabel) {
				BH ();
				Label (label, true);
				final = EditorGUILayout.ColorField (GUIContent.none, value, options);
				EH ();
			}
			else {
				final = EditorGUILayout.ColorField (label, value, options);
			}
			return final;
		}

		// Draw an interactive dropdown with a list o string options and return the selected index.
		public int Dropdown (string label, int selected, string[] options) {
			return EditorGUILayout.Popup (label, selected, options);
		}

		// Draw an interactive slider that locks to and returns an integer value.
		public int IntSlider (string label, int value, int minValue, int maxValue, int step = 1) {
			int lockedVal = (value / step) * step;
			return EditorGUILayout.IntSlider (label, lockedVal, minValue, maxValue);
		}

		// Draw an interactive enum dropdown to the context.
		public T EnumField<T> (string label, Enum value, bool flexWidthLabel = false, params GUILayoutOption[] options) where T : struct {
			T final;
			if (flexWidthLabel) {
				BH ();
				Label (label, true);
				final = (T)(object)EditorGUILayout.EnumPopup (GUIContent.none, value, options);
				EH ();
			}
			else {
				final = (T)(object)EditorGUILayout.EnumPopup (label, value, options);
			}
			return final;
		}

		// Draws a generic interactive object field to the context.
		public T ObjectField<T> (string label, T value, bool allowSceneObject = false) where T : UnityEngine.Object {
			return EditorGUILayout.ObjectField (label, value, typeof(T), allowSceneObject) as T;
		}

		// Draws a generic interactive object array field to the context.
		public T[] ObjectArrayField<T> (string label, T[] value, bool allowSceneObjects = false) where T : UnityEngine.Object {

			// Make sure that the array is instantiated.
			if (value == null) {
				value = new T[0];
			}

			// Cache a scoped copy of the array to return later.
			T[] array = value;
			
			// Fold the list of elements in a foldout.
			if (Foldout (label)) {

				// Cache values to use locally throughout array manipulation.
				int arrayLength = array.Length;
				int i;

				// Create a field to resize the array.
				int newLength = arrayLength;
				newLength = IntField ("Size", newLength);

				// Check if the length changed.
				if (newLength != arrayLength) {
					
					// Clamp the length above zero.
					if (newLength < 0) {
						newLength = 0;
					}

					// Create a new array to modify.
					T[] newArray = new T [newLength];

					// Iterate through each index in the new array.
					for (i = 0; i < newLength; i++) {

						// If possible, copy the element at the current index.
						if (i < arrayLength) {
							newArray [i] = array [i];
						}

						// Otherwise, initialize the element at the current index to the default value.
						else {
							newArray [i] = default (T);
						}
					}

					// Copy the new array.
					array = newArray;
					arrayLength = newLength;
				}

				// Create object fields for each element in the array.
				for (i = 0; i < arrayLength; i++) {
					array [i] = ObjectField<T> ("Element " + i, array [i], allowSceneObjects);
				}
			}

			// Return the array.
			return array;
		}

		// Draws a button and returns true if the button is pressed this frame.
		public bool Button (string label, params GUILayoutOption[] options) {
			return GUILayout.Button (label, options);
		}

		// Render an automatic foldout with an identifier.
		public bool Foldout (string identifier) {
			return Foldout (identifier, identifier);
		}

		// Render an automatic foldout with a label that differs from its identifier.
		public bool Foldout (string identifier, string label) {
			
			// Check if this is a new foldout.
			if (!autoFoldoutFlags.ContainsKey (identifier)) {
				
				// Create a hash entry for the foldout flag.
				autoFoldoutFlags.Add (identifier, false);
				foldoutUseTracker.Add (identifier, false);
			}

			// Mark this foldout as used.
			foldoutUseTracker [identifier] = true;
			
			// Render the foldout.
			autoFoldoutFlags [identifier] = EditorGUILayout.Foldout (autoFoldoutFlags [identifier], label);
			
			// Return the flag.
			return autoFoldoutFlags [identifier];
		}

		// Begin a horizontal layout.
		public void BH (GUIStyle style = null) {
			if (style == null) {
				EditorGUILayout.BeginHorizontal ();
			}
			else {
				EditorGUILayout.BeginHorizontal (style);
			}
		}

		// End a horizontal layout.
		public void EH () {
			EditorGUILayout.EndHorizontal ();
		}

		// Begin a vertical layout.
		public void BV () {
			EditorGUILayout.BeginVertical ();
		}

		// End a vertical layout.
		public void EV () {
			EditorGUILayout.EndVertical ();
		}

		// Add space between layout elements.
		public void Space () {
			EditorGUILayout.Space ();
		}

		// Override for defining the space size.
		public void Space (float pixels) {
			GUILayout.Space (pixels);
		}

		// Create flexible space.
		public void FlexibleSpace () {
			GUILayout.FlexibleSpace ();
		}

		// Draw a sprite to the GUI.
		public void DrawSprite(Rect position, Sprite sprite, Vector2 size)
		{
			Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
				sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
			Vector2 actualSize = size;

			actualSize.y *= (sprite.rect.height / sprite.rect.width);
			GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), sprite.texture, spriteRect);
		}

		// Calculates the pixel width of a label.
		private float GetLabelWidth (string label) {
			Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
			return textDimensions.x;
		}
	}
}