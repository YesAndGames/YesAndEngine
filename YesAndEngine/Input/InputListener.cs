using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Place this behavior on a GUI rect transform to listen for input on that space.
public class InputListener : MonoBehaviour,
	IPointerDownHandler,
	IPointerUpHandler,
	IDragHandler,
	IBeginDragHandler,
	IEndDragHandler,
	IScrollHandler {

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

	// Fires on the frame where a drag begins.
	[SerializeField]
	public PointerEvent BeginDrag;

	// Fires on the frame a drag ends.
	[SerializeField]
	public PointerEvent EndDrag;

	// Fires each frame the listener interprets scroll input.
	[SerializeField]
	public PointerEvent Scroll;

	// Fires each frame the listener interprets a pinch or inverse pinch gesture.
	[SerializeField]
	public TwoTouchEvent Pinch;

	// If set to true, the current gesture chain is a drag gesture.
	private bool isDragGesture;

	// Called when this component is destroyed by Unity.
	void OnDestroy () {
		Dispose ();
	}

	// Clean up this event listener and detach events.
	public void Dispose () {
		if (PointerDown != null) {
			PointerDown.RemoveAllListeners ();
			PointerDown = null;
		}
		if (PointerUp != null) {
			PointerUp.RemoveAllListeners ();
			PointerUp = null;
		}
		if (PointerClick != null) {
			PointerClick.RemoveAllListeners ();
			PointerClick = null;
		}
		if (Drag != null) {
			Drag.RemoveAllListeners ();
			Drag = null;
		}
		if (BeginDrag != null) {
			BeginDrag.RemoveAllListeners ();
			BeginDrag = null;
		}
		if (EndDrag != null) {
			EndDrag.RemoveAllListeners ();
			EndDrag = null;
		}
		if (Scroll != null) {
			Scroll.RemoveAllListeners ();
			Scroll = null;
		}
		if (Pinch != null) {
			Pinch.RemoveAllListeners ();
			Pinch = null;
		}
	}

	// Interpret a pointer down event.
	public void OnPointerDown (PointerEventData eventData) {
		isDragGesture = false;
		if (PointerDown != null) {
			PointerDown.Invoke (eventData);
		}
		UpdateTouchInput ();
	}

	// Interpret a pointer up event.
	public void OnPointerUp (PointerEventData eventData) {
		if (!isDragGesture) {
			if (PointerClick != null) {
				PointerClick.Invoke (eventData);
			}
		}
		if (PointerUp != null) {
			PointerUp.Invoke (eventData);
		}
		UpdateTouchInput ();
		isDragGesture = false;
	}

	// Interpret a drag event.
	public void OnDrag (PointerEventData eventData) {
		if (Drag != null) {
			Drag.Invoke (eventData);
		}
		UpdateTouchInput ();
	}

	// Interpret a begin drag event.
	public void OnBeginDrag (PointerEventData eventData) {
		isDragGesture = true;
		if (BeginDrag != null) {
			BeginDrag.Invoke (eventData);
		}
		UpdateTouchInput ();
	}

	// Interpret an end drag event.
	public void OnEndDrag (PointerEventData eventData) {
		if (EndDrag != null) {
			EndDrag.Invoke (eventData);
		}
		UpdateTouchInput ();
	}

	// Interpret a scroll event.
	public void OnScroll (PointerEventData eventData) {
		if (Scroll != null) {
			Scroll.Invoke (eventData);
		}
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
