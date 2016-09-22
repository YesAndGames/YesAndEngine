using UnityEngine;
using System;
using System.Collections.Generic;

namespace YesAndEngine.ResourceManagement {

	// Manages loading and firing events for loaded resources.
	public static class AsyncResourceLoader {

		// List of singular asyncronously loading resource requests.
		private static List<AsyncResourceRequest> singleRequests = new List<AsyncResourceRequest> ();

		// List of asyncronous resource bundles.
		private static List<AsyncResourceRequestBundle> bundleRequests = new List<AsyncResourceRequestBundle> ();

		// Manage the specified resource request.
		public static void Load (ResourceRequest request, Action<ResourceRequest> callback = null) {
			singleRequests.Add (new AsyncResourceRequest (request, callback));
		}

		// Manage a bundle of resource requests and call a callback when they are all finished.
		public static void Load (Action callback, params ResourceRequest[] requests) {
			bundleRequests.Add (new AsyncResourceRequestBundle (requests, callback));
		}

		// Manage a bundle of resource requests organized in a List and call a callback when they are all finished.
		public static void Load (Action callback, List<ResourceRequest> requests) {
			ResourceRequest[] requestArray = new ResourceRequest[requests.Count];
			for (int i = 0; i < requestArray.Length; i++) {
				requestArray [i] = requests [i];
			}
			bundleRequests.Add (new AsyncResourceRequestBundle (requestArray, callback));
		}

		// Update all pending async load requests.
		public static void Update () {

			// Keep a running list of the requests that are finished and need to get cleared in a cleanup pass.
			List<int> toRemove = new List<int> ();
			int i;

			// Update pass for single requests.
			for (i = 0; i < singleRequests.Count; i++) {

				// Update the request.
				AsyncResourceRequest request = singleRequests [i];
				request.Tick ();

				// If done, mark for removal and fire callback.
				if (request.IsDone ()) {
					toRemove.Add (i);
				}
			}

			// Garbage collection pass for single requests.
			for (i = 0; i < toRemove.Count; i++) {
				singleRequests.RemoveAt (toRemove [i]);
			}

			// Clear garbage collection tracker.
			toRemove.Clear ();

			// Update pass for bundle requests.
			for (i = 0; i < bundleRequests.Count; i++) {

				// Update the request bundle.
				AsyncResourceRequestBundle bundle = bundleRequests [i];
				bundle.Tick ();

				// If done, mark for removal and fire callback.
				if (bundle.IsDone ()) {
					toRemove.Add (i);
				}
			}

			// Garbage collection pass for bundle requests.
			for (i = 0; i < toRemove.Count; i++) {
				bundleRequests.RemoveAt (toRemove [i]);
			}
		}

		// Wrapper around ResourceRequest that checks for done and fires events.
		private class AsyncResourceRequest {

			// The resource request this object manages.
			private readonly ResourceRequest request;

			// The callback method for when this resource loads.
			private Action<ResourceRequest> callback;

			// Constructor for an AsyncResourceRequest.
			public AsyncResourceRequest (ResourceRequest request, Action<ResourceRequest> callback) {
				this.request = request;
				this.callback = callback;
			}

			// Update this resource request.
			public void Tick () {
				if (IsDone ()) {
					if (callback != null) {
						callback (request);
						callback = null;
					}
				}
			}

			// Check if this resource is loaded and manage it. Return the current load progress.
			public float Progress () {
				return request.progress;
			}

			// Returns a boolean indicating whether or not this resource is done loading.
			public bool IsDone () {
				return request.isDone;
			}
		}

		// A bundle of async resource requests.
		private class AsyncResourceRequestBundle {

			// Array of resource requests that this object manages.
			private readonly AsyncResourceRequest[] asyncRequests;

			// Callback for when this bundle is finished loading.
			private Action callback;

			// Overall load progress for this bundle.
			private float progress = 0;

			// Flag that indicates whether or not this bundle is done loading.
			private bool isDone = false;

			// Constructor for an AsyncResourceRequestBundle.
			public AsyncResourceRequestBundle (ResourceRequest[] requests, Action callback) {

				// Create AsyncResourceRequest objects for each request.
				asyncRequests = new AsyncResourceRequest[requests.Length];
				for (int i = 0; i < requests.Length; i++) {
					asyncRequests [i] = new AsyncResourceRequest (requests [i], null);
				}

				// Register callback.
				this.callback = callback;
			}

			// Update this asset bundle.
			public void Tick () {
				float progressSum = 0;
				bool done = true;

				// Update each async request and recalculate data.
				for (int i = 0; i < asyncRequests.Length; i++) {
					AsyncResourceRequest request = asyncRequests [i];
					request.Tick ();
					progressSum += request.Progress ();
					if (!request.IsDone ()) {
						done = false;
					}
				}

				// If finished, fire callback.
				if (done && callback != null) {
					callback ();
					callback = null;
				}

				// Record data.
				progress = progressSum / asyncRequests.Length;
				isDone = done;
			}

			// Gets the overall progress of this bundle load.
			public float Progress () {
				return progress;
			}

			// Returns a flag indicating whether or not this bundle is done loading.
			public bool IsDone () {
				return isDone;
			}
		}
	}
}