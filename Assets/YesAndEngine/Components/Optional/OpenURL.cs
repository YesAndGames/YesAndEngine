using UnityEngine;
using System.Collections;

namespace YesAndEngine.Components {

	// This script contains methods for opening URLs, other apps, and more from Unity events or scripts.
	public class OpenURL : MonoBehaviour {

		// Flags the app as paused, assuming the user minimized it.
		private bool appPaused = false;

		// Called when the app gets focused by the OS.
		void OnApplicationFocus (bool status) {
			appPaused = !status;
		}

		// Called when the app gets paused by the OS.
		void OnApplicationPaused (bool status) {
			appPaused = status;
		}

		// Open the specified URL.
		public void GoTo (string url) {
			Application.OpenURL (url);
		}

		// Opens the specified Facebook page.
		public void OpenFacebookPage (string page) {
			StartCoroutine ("OpenFacebookPageRoutine", page);
		}

		// Opens the specified Facebook page through a coroutine, preferring the app.
		private IEnumerator OpenFacebookPageRoutine (string page) {
			GoTo ("fb://page/" + page);
			yield return new WaitForSeconds (1f);
			if (!appPaused) {
				GoTo ("http://www.fb.com/" + page);
			}
		}

		// Opens the specified Twitter page.
		public void OpenTwitterPage (string handle) {
			StartCoroutine ("OpenTwitterPageRoutine", handle);
		}

		// Opens the specified Twitter page through a coroutine, preferring the app.
		private IEnumerator OpenTwitterPageRoutine (string page) {
			GoTo ("twitter:///user/?screen_name=" + page);
			yield return new WaitForSeconds (1f);
			if (!appPaused) {
				GoTo ("http://www.twitter.com/" + page);
			}
		}
	}
}