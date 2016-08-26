using UnityEngine;

namespace YesAndEngine.Components {

	/// <summary>
	/// This class is used to dispatch very basic animation events
	/// through the Mecanim animator. To use, hook events to the delegates
	/// that fire, and fire the method somewhere during the animation.
	/// </summary>
	public class AnimationEventDispatcher : MonoBehaviour {

		// Delegate for an animation message event.
		public delegate void AnimationEventDispatchDelegate (string message);

		// Subscribe to this event to listen for messages from attached animators.
		public AnimationEventDispatchDelegate OnAnimationEvent;

		// Call this method to fire an animation event with a message from an animation.
		public void FireAnimationEvent (string message) {
			if (OnAnimationEvent != null) {
				OnAnimationEvent (message);
			}
		}
	}
}
