using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;

namespace YesAndEditor {

	// Static class packed to the brim with helpful Unity editor utilities.
	public static class YesAndEditorUtil {

		// Forcefully mark open loaded scenes dirty and save them.
		[MenuItem ("File/Force Save")]
		public static void ForceSaveOpenScenes () {
			EditorSceneManager.MarkAllScenesDirty ();
			EditorSceneManager.SaveOpenScenes ();
		}

		// Mark an object editor-only.
		public static void SetEditorOnly(this MonoBehaviour obj, bool editorOnly = true) {
			if (editorOnly) {
				obj.gameObject.hideFlags ^= HideFlags.NotEditable;
				obj.gameObject.hideFlags ^= HideFlags.HideAndDontSave;
			}
			else {
				obj.gameObject.hideFlags &= HideFlags.NotEditable;
				obj.gameObject.hideFlags &= HideFlags.HideAndDontSave;
			}
		}
	}
}
