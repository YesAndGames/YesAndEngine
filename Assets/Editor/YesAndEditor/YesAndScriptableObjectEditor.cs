using UnityEngine;
using UnityEditor;

namespace YesAndEditor {

	// Yes And Editor layer for scriptable object inspectors.
	public abstract class YesAndScriptableObjectEditor<T> : YesAndInspector<T> where T:ScriptableObject {
	}
}