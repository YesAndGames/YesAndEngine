using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace YesAndEditor {

	// Yes And Editor extension of the Unity EditorWindow.
	public abstract class YesAndEditorWindow : EditorWindow {

		// Show this editor window.
		public static T ShowWindow<T> (string title, params object[] args) where T : YesAndEditorWindow {
			T window = GetWindow<T> (title);
			window.Show ();
			window.Initialize (args);
			return window;
		}

		// This window's parent window.
		protected YesAndEditorWindow Parent { get; set; }

		// List of this window's children windows.
		protected List<YesAndEditorWindow> Children { get; private set; }

		// Layout builder.
		protected YesAndLayout Layout { get; private set; }

		// Initialize this editor window with show arguments.
		protected virtual void Initialize (params object[] args) { }

		// Initialize this window.
		protected virtual void OnEnable () {
			Reinitialize ();
		}

		// Clean up this window.
		protected virtual void OnDisable () {
			Layout.Cleanup ();
		}

		// Draw the GUI to this window.
		protected virtual void OnGUI () {
			Reinitialize ();
		}

		// Update this window.
		protected virtual void Update () {
			if (EditorApplication.isCompiling) {
				Close ();
			}
		}

		// Called when this window is destroyed.
		protected virtual void OnDestroy () {

			// Remove this child from parent.
			if (Parent != null) {
				Parent.RemoveChild (this);
			}

			// Close children.
			for (int i = 0; i < Children.Count; i++) {
				Children[i].Close ();
			}

			// Unload unused Resources.
			Resources.UnloadUnusedAssets ();
		}

		// Set this window's parent window.
		public void SetParent (YesAndEditorWindow parent) {

			// Remove old parent.
			if (Parent != null) {
				parent.RemoveChild (this);
			}

			// Setup new parent.
			if (parent != null) {
				parent.AddChild (this);
			}

			// Switch parents.
			Parent = parent;
		}

		// Add a child window to this window.
		public void AddChild (YesAndEditorWindow child) {
			Children.Add (child);
		}

		// Remove a child from this window.
		public void RemoveChild (YesAndEditorWindow child) {
			Children.Remove (child);
		}

		// Mark the scene as dirty so Unity recognizes that a scene save is demanded.
		public void MarkProjectDirty () {
			EditorSceneManager.MarkAllScenesDirty ();
		}

		// Reinitialize this window because something is missing.
		private void Reinitialize () {

			// Missing layout object.
			if (Layout == null) {
				Layout = new YesAndLayout ();
			}

			// Missing child list.
			if (Children == null) {
				Children = new List<YesAndEditorWindow> ();
			}
		}
	}
}