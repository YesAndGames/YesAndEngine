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

		// Create a 2D texture from a specified color.
		public static Texture2D Texture2DFromColor (Color color, int width = 1, int height = 1) {
			Color[] pix = new Color[width * height];

			for (int i = 0; i < pix.Length; i++)
				pix[i] = color;

			Texture2D result = new Texture2D (width, height);
			result.SetPixels (pix);
			result.Apply ();

			return result;
		}

		// Returns a randomized color.
		public static Color RandomColor () {
			return new Color (
			  Random.Range (0f, 1f),
			  Random.Range (0f, 1f),
			  Random.Range (0f, 1f),
			  1f);
		}

		// Stretch this rect transform to fill its container.
		public static void StretchRectTransform (this RectTransform rect, Transform container = null) {

			// Setup parent if necessary.
			if (container != null) {
				rect.SetParent (container, false);
			}

			// Stretch anchors.
			rect.anchorMax = Vector2.one;
			rect.anchorMin = Vector2.zero;
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = Vector2.zero;
		}

		// Append a new array element to the end of an array.
		public static T AddArrayElement<T> (ref T[] arr) where T : new() {
			T[] copy = new T[arr.Length + 1];
			arr.CopyTo (copy, 0);
			T newElement = new T ();
			copy[copy.Length - 1] = newElement;
			arr = copy;
			return newElement;
		}

		// Remove the element at the specified index from the specified array.
		public static void RemoveArrayElement<T> (ref T[] arr, int index) {
			T[] finished = new T[arr.Length - 1];
			for (int i = 0; i < arr.Length; i++) {
				if (i == index) {
					continue;
				}

				int copyIndex = i;
				if (i > index) {
					copyIndex--;
				}

				finished[copyIndex] = arr[i];
			}

			arr = finished;
		}
	}
}
