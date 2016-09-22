using UnityEngine;
using UnityEditor;
using System.Collections;

namespace YesAndEditor {

	// Yes And editor layer for the object inspector window.
	public abstract class YesAndInspector<T> : Editor where T:UnityEngine.Object {

		// Layout object for this editor.
		protected YesAndLayout Layout;

		// The object this editor targets.
		protected T selected;

		// Current event this frame.
		protected Event e;

		// Active scene view this frame.
		protected SceneView view;

		// If set to true, this editor has been initialized.
		private bool initialized = false;

		// If set to true, this inspector will draw the default inspector.
		private bool drawDefaultInspector = true;

		// Initialize this inspector.
		protected virtual void OnEnable () {
			initialized = false;
		}

		// Deinitialize this inspector.
		protected virtual void OnDisable () {
			initialized = false;
		}

		// Render the inspector GUI.
		public override void OnInspectorGUI () {

			// Make sure the editor gets initialized before trying to render anything.
			if (!initialized) {
				Reinitialize ();
			}

			// Call base if flagged to draw default inspector.
			if (drawDefaultInspector) {
				base.OnInspectorGUI ();
			}
		}

		// Render the scene GUI.
		protected virtual void OnSceneGUI () {

			// Make sure the editor gets initialized before trying to render anything.
			if (!initialized) {
				Reinitialize ();
			}

			// Cache the current event and scene view.
			e = Event.current;
			view = SceneView.currentDrawingSceneView;

			// Cache the current control and event types.
			int controlID = GUIUtility.GetControlID (FocusType.Passive);
			EventType et = e.GetTypeForControl (controlID);

			// Mouse click has occurred.
			if (et == EventType.MouseDown) {
				OnMouseClick (e.button);
			}

			// Repaint the view so it doesn't lag.
			view.Repaint ();
		}

		// Reinitialize this editor.
		protected virtual void Reinitialize () {

			// Initialize the layout object.
			if (Layout == null) {
				Layout = new YesAndLayout ();
			}

			// Cast and cache the target.
			if (selected == null) {
				selected = target as T;
			}

			// Flag initialized.
			initialized = true;
		}

		// Set the draw default inspector flag.
		protected void SetDrawDefaultInspector (bool drawDefault = true) {
			drawDefaultInspector = drawDefault;
		}

		// Returns true if the selected object is in the scene hierarchy.
		protected bool InScene () {

			// If the selected object is not a MonoBehavior, it can't ever be in the scene.
			if (selected as MonoBehaviour == null) {
				return false;
			}

			// Check the prefab type.
			PrefabType prefabType = PrefabUtility.GetPrefabType (selected);
			switch (prefabType) {
			case PrefabType.None:
			case PrefabType.PrefabInstance:
			case PrefabType.ModelPrefabInstance:
			case PrefabType.DisconnectedPrefabInstance:
				return true;
			default:
				return false;
			}
		}

		// Get the mouse position in the game scene. If flat, z will be 0.
		protected Vector3 GetSceneMousePosition (bool flat = true) {
			Vector3 mousePosition = e.mousePosition;
			mousePosition = HandleUtility.GUIPointToWorldRay (mousePosition).origin;
			if (flat) {
				mousePosition.z = 0;
			}
			return mousePosition;
		}

		// Called when the mouse is clicked.
		protected virtual void OnMouseClick (int button) {
		}

		// Absorb the current system event, preventing bubbling.
		protected void AbsorbEvent () {
			GUIUtility.hotControl = 0;
			e.Use ();
		}
	}
}