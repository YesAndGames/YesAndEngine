using UnityEngine;

namespace YesAndEngine.EngineComponents {

	// Enforces singleton behavior for a Monobehavior component.
	public abstract class SingletonMonobehavior<T> : MonoBehaviour where T : SingletonMonobehavior<T> {

		// Singleton instance of this component.
		protected static T instance;

		// Initialize this component.
		protected virtual bool Awake () {

			// Check if the instance is already assigned.
			if (instance == null) {

				// Assign the instance.
				instance = GetComponent<T> ();
			}

			// Otherwise we need to destroy this component.
			else {

				// Properly destroy this instance.
				if (Application.isPlaying) {
					Destroy (this);
				}
				else {
					DestroyImmediate (this);
				}

				// Print error and false out.
				Debug.LogError ("Cannot initialize more than one instance of " + typeof (T), instance);
				return false;
			}

			// Successful initialization.
			return true;
		}
	}
}
