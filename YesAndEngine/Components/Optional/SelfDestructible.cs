using UnityEngine;

namespace YesAndEngine.Components {

	// Append this component to GameObjects that need to fire a void event to self-destruct.
	public class SelfDestructible : MonoBehaviour {
		
		// If checked, this GameObject will self-destruct when the animation playing ends.
		public bool destroyWhenAnimationEnds = false;

		// If checked, also destroys the parent game object.
		public bool destroyParent = false;

		// Tracks the amount of time that has passed after this SelfDestructible is started.
		private float time = 0.0f;

		// Initialize this component.
		void Start () {
			time = 0.0f;
		}

		// Update this component.
		void Update () {
			time += Time.deltaTime;

			// Check if the animation is ended.
			if (destroyWhenAnimationEnds) {
				if (time >= GetComponent<Animation> ().clip.length) { SelfDestruct (); }
			}
		}

		// Destroy the GameObject this component is attached to.
		public void SelfDestruct () {
			if (destroyParent) {
				Destroy (transform.parent.gameObject);
			}
			else {
				Destroy (gameObject);
			}
		}
	}
}