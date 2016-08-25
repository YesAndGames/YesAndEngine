using UnityEngine;

namespace YesAndEngine.EngineUtilities {

	// Static class with a bunch of useful Unity tools.
	public static class UnityTools {

		// Safely set the parent of this transform and clamp to it, resetting its transform properties to a blank slate.
		// Unity sometimes applies finicky transform artifact properties that you don't want copied into the scene. This
		// gives you default transform values to work with.
		public static Transform SetAndClampParent (this Transform transform, Transform parent, bool worldPositionStays = false) {
			transform.SetParent (parent, worldPositionStays);
			transform.localScale = Vector3.one;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			return transform;
		}
	}
}
