using System.Collections;
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

		// Constants.
		private const float REACHED_TARGET_THRESHOLD = 0.1f;

		// The YesAndCamera attached to Camera.main.
		public static YesAndCamera main { get; private set; }

		// The current mode of this camera.
		public CameraControlMode Mode { get; private set; }

		// If set to true, this camera is restricted to motion inside its boundaries.
		public bool LockedToBounds { get; set; }

		// If set to true, this camera is allowed to move along the horizontal axis.
		public bool HorizontalMotionEnabled { get; set; }

		// If set to true, this camera is allowed to move along the vertical axis.
		public bool VerticalMotionEnabled { get; set; }

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

		// Position bounds.
		private float? leftBound;
		private float? rightBound;
		private float? topBound;
		private float? bottomBound;

		// The camera attached to this camera.
		private Camera cam;

		// The position the camera should be moving towards.
		private Vector3? targetPosition;

		// The orthographic size the camera should be moving towards.
		private float? targetSize;

		// The currenty velocity of this camera.
		private Vector3 velocity;

		// The list of targets currently being followed.
		private List<Transform> following;

		// Camera shake variables.
		private float shakePower;
		private float shakeDuration;

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
				RestrictVelocity ();
				transform.Translate (velocity);
				velocity *= Mathf.Pow (damping, Time.deltaTime);
				break;
			case CameraControlMode.Following:
				break;
			}

			// Update automatic camera motion.
			if (targetPosition != null) {
				Vector3 tar = targetPosition.Value;
				velocity = tar - transform.position;
				RestrictVelocity ();
				transform.Translate (velocity);
				if (Vector3.Distance (tar, transform.position) < REACHED_TARGET_THRESHOLD) {
					targetPosition = null;
				}
			}

			// Clamp to bounds.
			ClampPosition ();
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

		// Restrict to the specified bounds. Any bounds assigned to null will be ignored
		public void SetBounds (float? left = null, float? right = null, float? top = null, float? bottom = null) {
			leftBound = left;
			rightBound = right;
			topBound = top;
			bottomBound = bottom;
			LockedToBounds = true;
			HorizontalMotionEnabled = left != null || right != null;
			VerticalMotionEnabled = top != null || bottom != null;
		}

		// Release from all bounds.
		public void ReleaseBounds () {
			SetBounds (null, null, null, null);
			LockedToBounds = false;
			HorizontalMotionEnabled = true;
			VerticalMotionEnabled = true;
		}

		// Translate the camera by a vector.
		public void Translate (Vector2 translation) {
			transform.Translate (translation);
		}

		// Target the specified position, move there but do not follow.
		public void MoveTo (Vector2 target, float? size = null) {
			targetPosition = target;
			if (size != null) {
				targetSize = size.Value;
			}
		}

		// Focus on the specified group of transforms.
		public void FocusOn (params Transform[] targets) {
			float left = 0, right = 0, top = 0, bottom = 0;

			// Examine each focal point
			Vector2 focusPoint = Vector2.zero;
			foreach (Transform t in targets) {
				float x = t.position.x;
				float y = t.position.y;
				focusPoint.x += x;
				focusPoint.y += y;
				if (x < left) {
					left = x;
				}
				if (x > right) {
					right = x;
				}
				if (y > top) {
					top = y;
				}
				if (y < bottom) {
					bottom = y;
				}
			}

			// Calculate position and bounds
			int count = targets.Length;
			MoveTo (new Vector2 (focusPoint.x / count, focusPoint.y / count), cam.orthographicSize);
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
		public void Reset (Vector3? position = null, float? size = null, Color? clearColor = null) {

			// Reset the Camera component.
			if (position != null) {
				transform.position = position.Value;
			}
			if (size != null) {
				cam.orthographicSize = size.Value;
			}
			if (clearColor != null) {
				cam.backgroundColor = (Color)clearColor;
			}

			// Reset fields.
			Mode = CameraControlMode.Unlocked;
			ReleaseBounds ();
			targetPosition = null;
			targetSize = null;
			velocity = Vector3.zero;
			following.Clear ();
		}

		// Shake this camera.
		public void Shake (float power, float duration) {
			shakePower = power;
			shakeDuration = duration;
			StartCoroutine (Shaking ());
		}

		// Coroutine for camera shake.
		private IEnumerator Shaking () {

			float elapsed = 0.0f;
			Vector3 preShakePos = cam.transform.position;

			while (elapsed < shakeDuration) {

				elapsed += Time.deltaTime;

				float percentComplete = elapsed / shakeDuration;
				float damper = 1.0f - Mathf.Clamp (4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

				// map value to [-1, 1]
				float x = Random.value * 2.0f - 1.0f;
				float y = Random.value * 2.0f - 1.0f;

				x *= shakePower * damper;
				y *= shakePower * damper;

				x += preShakePos.x;
				y += preShakePos.y;

				transform.localPosition = new Vector3 (x, y, transform.position.y);

				yield return null;
			}

			transform.position = preShakePos;
		}

		// Restrict the current velocity given all the bounds.
		private void RestrictVelocity () {
			velocity.z = 0;

			if (!HorizontalMotionEnabled) {
				velocity.x = 0;
			}
			if (!VerticalMotionEnabled) {
				velocity.y = 0;
			}
		}

		// Clamp the current camera position.
		private void ClampPosition () {

			// Clamp bounds if they exist.
			float x = transform.position.x, y = transform.position.y;
			if (leftBound != null) {
				float l = (float)leftBound;
				x = x < l ? l : x;
			}
			if (rightBound != null) {
				float r = (float)rightBound;
				x = x > r ? r : x;
			}
			if (topBound != null) {
				float t = (float)topBound;
				y = y > t ? t : y;
			}
			if (bottomBound != null) {
				float b = (float)bottomBound;
				y = y < b ? b : y;
			}

			transform.position = new Vector3 (x, y, transform.position.z);
		}
	}
}