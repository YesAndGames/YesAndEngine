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
		[SerializeField, Range (0f, 1f), Tooltip ("Rate of momentum damping.")]
		private float damping = 0.1f;

		// The zoom rate while the camera is in orthographic mode.
		[SerializeField, Tooltip ("Rate of zoom in orthographic mode.")]
		private float orthoZoomRate = 1f;

		// Low bound for orthographic camera size.
		[SerializeField, Tooltip ("Minimum orthographic camera size.")]
		private float minOrthoSize = 4f;

		// High bound for orthographic camera size.
		[SerializeField, Tooltip ("Maximum orthographic camera size.")]
		private float maxOrthoSize = 10f;

		// The camera attached to this camera.
		private Camera cam;

		// The position the camera should be moving towards.
		private Vector3 targetPosition;

		// The currenty velocity of this camera.
		private Vector3 velocity;

		// The list of targets currently being followed.
		private List<Transform> following;

		// Initialize this component.
		void Awake () {
			main = Camera.main.GetComponent<YesAndCamera> ();
			cam = GetComponent<Camera> ();
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
				velocity *= Mathf.Pow (damping, Time.deltaTime);
				break;
			case CameraControlMode.Following:
				break;
			}
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

		// Zoom this camera using scroll pointer event data.
		public void Zoom (PointerEventData scrollData) {
			cam.orthographicSize = Mathf.Clamp (
				cam.orthographicSize -= scrollData.scrollDelta.y * orthoZoomRate * Time.deltaTime,
				minOrthoSize,
				maxOrthoSize
			);
		}

		// Zoom this camera using a pinch gesture.
		public void Zoom (Touch a, Touch b) {

			// Find the position in the previous frame of each touch.
			Vector2 aPrev = a.position - a.deltaPosition;
			Vector2 bPrev = b.position - b.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (aPrev - bPrev).magnitude;
			float touchDeltaMag = (a.position - b.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			// Change the orthographic size based on the change in distance between the touches.
			if (cam.orthographic) {
				cam.orthographicSize = Mathf.Clamp (
					cam.orthographicSize + deltaMagnitudeDiff * orthoZoomRate * Time.deltaTime,
					minOrthoSize,
					maxOrthoSize
				);
			}
		}

		// Reset fields between states.
		private void Reset () {
			targetPosition = transform.position;
			velocity = Vector3.zero;
			following.Clear ();
		}
	}
}