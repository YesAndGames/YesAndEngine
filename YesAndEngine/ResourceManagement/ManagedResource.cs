using UnityEngine;

namespace YesAndEngine.ResourceManagement {

	// A ManagedResource is a serializable object that maintains a path and index to an actual asset in a
	// Unity Resources directory.
	[System.Serializable]
	public class ManagedResource<T> where T : UnityEngine.Object {

		// Returns the resource path.
		public string ResourcePath { get { return resourcePath.ToLower (); } }

		// Returns the resource ID.
		public long ID { get { return resourceID; } }

		// Returns the collection index.
		public int CollectionIndex {
			get {
				return collectionIndex;
			}
		}

		// An optional identifier for this resource, for if it needs to be indexed or searched.
		[SerializeField]
		private long resourceID = 0;

		// The string path to the asset assuming it's in a folder named resources.
		[SerializeField]
		private string resourcePath = string.Empty;

		// If the option is contained in a collection or asset bundle like a sliced texture, this stores in index.
		[SerializeField]
		private int collectionIndex = -1;

		// Load and return the managed asset.
		public virtual T Load () {
			T loaded = null;

			if (collectionIndex < 0) {
				loaded = Resources.Load<T> (resourcePath.ToLower ());
			}
			else {
				Object[] collection = Resources.LoadAll (resourcePath.ToLower ());

				// If there is an issue related to this asset's index, then we just can't load it.
				if (collectionIndex < collection.Length) {
					loaded = collection[collectionIndex] as T;
				}
			}

			return loaded;
		}

		// Prepare and return a ResourceRequest for the managed asset.
		public virtual ResourceRequest LoadAsync () {
			return Resources.LoadAsync<T> (resourcePath.ToLower ());
		}

		// Set the asset's resource path internally.
		public void SetResourcePath (string path, int index = -1, long id = 0) {
			resourceID = id;
			resourcePath = path;
			collectionIndex = index;
		}

		// Copy this managed resource from another.
		public void CopyFrom (ManagedResource<T> other) {

			// Make sure other exists.
			if (other != null) {
				resourceID = other.ID;
				resourcePath = other.ResourcePath;
				collectionIndex = other.CollectionIndex;
			}

			// If it doesn't, revert to default values.
			else {
				resourceID = 0;
				resourcePath = string.Empty;
				collectionIndex = -1;
			}
		}
	}
}
