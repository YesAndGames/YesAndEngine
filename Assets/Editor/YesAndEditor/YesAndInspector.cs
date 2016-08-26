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

		// If set to true, this inspector will draw the default inspector.
		private bool drawDefaultInspector = true;

		// Initialize this inspector.
		protected virtual void OnEnable () {
		}

		// Render the inspector GUI.
		public override void OnInspectorGUI () {
			Reinitialize ();
			if (drawDefaultInspector) {
				base.OnInspectorGUI ();
			}
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
	}
}