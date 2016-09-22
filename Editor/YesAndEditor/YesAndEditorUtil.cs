using UnityEditor;
using UnityEditor.SceneManagement;

namespace YesAndEditor {

	// Static class packed to the brim with helpful Unity editor utilities.
	public static class YesAndEditorUtil {

		// Forcefully mark open loaded scenes dirty and save them.
		[MenuItem ("File/Force Save")]
		public static void ForceSaveOpenScenes () {
			EditorSceneManager.MarkAllScenesDirty ();
			EditorSceneManager.SaveOpenScenes ();
		}
	}
}
