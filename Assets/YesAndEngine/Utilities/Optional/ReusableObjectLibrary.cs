using UnityEngine;
using System;
using System.Collections.Generic;

namespace YesAndEngine.Utilities {

	// Manages a library of reusable objects, so that they don't need to be reinstantiated when altered.
	[Serializable]
	public abstract class ReusableObjectLibrary<T> {

		// Delegate for a reusable object event given an object and data.
		public delegate void ReusableObjectEventHandler (GameObject obj, T datum);

		// Even that fires when an object is built. Append to this event to modify the rebuilt object in a script.
		public ReusableObjectEventHandler OnRebuildObject;

		// The prefab used to instantiate new objects in this library.
		[SerializeField]
		private GameObject prefab;

		// Array of living game objects.
		private GameObject[] active;

		// Rebuilds a single active object given a T object with the relevant data.
		private void RebuildObject (GameObject obj, T datum) {
			if (OnRebuildObject != null) {
				OnRebuildObject (obj, datum);
			}
		}

		// Rebuilds the complete library of objects given a data set of T objects.
		public void RebuildLibrary (T[] data) {

			// Make sure that the array of active game objects is instatiated.
			if (active == null) {
				active = new GameObject[0];
			}

			// Create a new array that will get copied wholesomely in memory.
			GameObject[] working = new GameObject[data.Length];

			// Iterate through the data array and build objects.
			for (int i = 0; i < data.Length; i++) {

				// Add a new object if necessary.
				if (i >= active.Length) {
					working[i] = UnityEngine.Object.Instantiate (prefab);
				}

				// If not, pull it from the active list.
				else {
					working[i] = active[i];
				}

				// Rebuild the living object.
				RebuildObject (working[i], data[i]);
			}

			// Delete lingering objects.
			for (int i = data.Length; i < active.Length; i++) {
				UnityEngine.Object.Destroy (active[i]);
			}

			// Relocate active array.
			active = working;
		}

		// Override for RebuildLibrary optimized for generic Lists.
		public void RebuildLibrary (List<T> data) {

			// Make sure that the array of active game objects is instantiated.
			if (active == null) {
				active = new GameObject[0];
			}

			// Count the number of data points.
			int numData = data.Count;

			// Create a working array that will get copied wholesomely in memory.
			GameObject[] working = new GameObject[numData];

			// Iterate through the data array and build objects.
			for (int i = 0; i < numData; i++) {

				// Add a new object if necessary.
				if (i >= active.Length) {
					working[i] = UnityEngine.Object.Instantiate (prefab);
				}

				// If not, pull it from the active list.
				else {
					working[i] = active[i];
				}

				// Rebuild the living object.
				RebuildObject (working[i], data[i]);
			}

			// Delete lingering objects.
			for (int i = numData; i < active.Length; i++) {
				UnityEngine.Object.Destroy (active[i]);
			}

			// Relocate active array.
			active = working;
		}
	}
}
