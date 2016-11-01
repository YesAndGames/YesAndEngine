using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YesAndEngine.Components {

	// An extension of the Unity Camera component that adds functionality and controls.
	public class YesAndCamera : MonoBehaviour {

		// The possible control states for a camera.
		public enum CameraControlMode {
			Locked,
			Unlocked,
			Following,
		}

		// The YesAndCamera attached to Camera.main.
		public static YesAndCamera main { get; private set; }

		// The current mode of this camera.
		public CameraControlMode Mode { get; private set; }

		// The dampening for velocity.
		[SerializeField, Range (0f, 1f)]
		private float damping = 0.9f;

		// The position the camera should be moving towards.
		private Vector3 targetPosition;

		// The currenty velocity of this camera.
		private Vector3 velocity;

		// The list of targets currently being followed.
		private List<Transform> following;

		// Initialize this component.
		void Awake () {
			main = Camera.main.GetComponent<YesAndCamera> ();
			following = new List<Transform> ();
			Lock ();
		}

		// Update this component.
		void Update () {
			switch (Mode) {
			case CameraControlMode.Locked:
				break;
			case CameraControlMode.Unlocked:
				velocity.z = 0;
				transform.Translate (velocity);
				break;
			case CameraControlMode.Following:
				break;
			}
		}

		// Update this component at fixed intervals.
		void FixedUpdate () {
			velocity *= damping;
		}

		// Lock this camera to any controls.
		public void Lock () {
			Reset ();
			Mode = CameraControlMode.Locked;
		}

		// Unlock this camera to scripted controls.
		public void Unlock () {
			Reset ();
			Mode = CameraControlMode.Unlocked;
		}

		// Begin following the list of targets.
		public void Follow (params Transform[] targets) {
			Reset ();
			following.AddRange (targets);
			Mode = CameraControlMode.Following;
		}

		// Drag this camera for a frame.
		public void Drag (PointerEventData pointer) {
			velocity = -(Camera.main.ScreenToWorldPoint (pointer.position) - Camera.main.ScreenToWorldPoint (pointer.position - pointer.delta));
		}

		// Reset fields between states.
		private void Reset () {
			targetPosition = transform.position;
			velocity = Vector3.zero;
			following.Clear ();
		}
	}
}