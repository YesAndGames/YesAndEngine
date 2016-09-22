using UnityEditor;

namespace YesAndEditor {

	// Create a Unity popup window with a YesAndLayout object.
	public class YesAndEditorPopup : PopupWindowContent {

		// Layout builder.
		protected YesAndLayout Layout { get; private set; }

		// Called when this popup is opened.
		public override void OnOpen () {
			base.OnOpen ();
			Layout = new YesAndLayout ();
		}

		// Called when this popup is closed.
		public override void OnClose () {
			base.OnClose ();
			Layout.Cleanup ();
		}

		// Render the GUI on this popup.
		public override void OnGUI (UnityEngine.Rect rect) { }
	}
}