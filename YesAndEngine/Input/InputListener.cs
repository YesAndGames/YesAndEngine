using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Place this behavior on a GUI rect transform to listen for input on that space.
public class InputListener : MonoBehaviour,
	IPointerDownHandler,
	IPointerUpHandler,
	IPointerClickHandler,
	IDragHandler {

	// An event delegate with a PointerEventData argument.
	[Serializable]
	public class PointerEvent : UnityEvent<PointerEventData> { }

	// An event delegate with two Touch arguments.
	[Serializable]
	public class TwoTouchEvent : UnityEvent<Touch, Touch> { }

	// Fires on the frame a pointer goes down.
	[SerializeField]
	public PointerEvent PointerDown;

	// Fires on the frame a pointer goes up.
	[SerializeField]
	public PointerEvent PointerUp;

	// Fires on the frame a click or tap occurs.
	[SerializeField]
	public PointerEvent PointerClick;

	// Fires each frame the listener interprets a drag gesture.
	[SerializeField]
	public PointerEvent Drag;

	// Fires each frame the listener interprets a pinch or inverse pinch gesture.
	[SerializeField]
	public TwoTouchEvent Pinch;

	// Clean up this event listener and detach events.
	public void Dispose () {
		if (Drag != null) {
			Drag.RemoveAllListeners ();
		}
		if (Pinch != null) {
			Pinch.RemoveAllListeners ();
		}
	}

	// Interpret a pointer down event.
	public void OnPointerDown (PointerEventData eventData) {
		if (PointerDown != null) {
			PointerDown.Invoke (eventData);
		}
		UpdateTouchInput ();
	}

	// Interpret a pointer up event.
	public void OnPointerUp (PointerEventData eventData) {
		if (PointerUp != null) {
			PointerUp.Invoke (eventData);
		}
		UpdateTouchInput ();
	}

	// Interpret a pointer cllick event.
	public void OnPointerClick (PointerEventData eventData) {
		if (PointerClick != null) {
			PointerClick.Invoke (eventData);
		}
		UpdateTouchInput ();
	}

	// Interpret a drag event.
	public void OnDrag (PointerEventData eventData) {
		if (Drag != null) {
			Drag.Invoke (eventData);
		}
		UpdateTouchInput ();
	}

	// Interpret touch input that isn't fired by Unity input events.
	private void UpdateTouchInput () {

		// If there is at least one touch on the device...
		if (Input.touchCount > 0) {
			Touch touchZero = Input.GetTouch (0);

			// If there are two touches on the device...
			if (Input.touchCount == 2) {
				Touch touchOne = Input.GetTouch (1);

				// Fire a pinch event.
				if (Pinch != null) {
					Pinch.Invoke (touchZero, touchOne);
				}
			}
		}
	}
}
