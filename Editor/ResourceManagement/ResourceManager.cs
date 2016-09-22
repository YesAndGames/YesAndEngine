using UnityEngine;
using UnityEditor;
using System.IO;
using YesAndEngine.ResourceManagement;

namespace YesAndEditor {

	// Editor tools for managed resources.
	public static class ResourceManager {

		// Register a managed resource via a project asset.
		public static void Register<T> (this ManagedResource<T> resource, T asset, long id = 0) where T : UnityEngine.Object {

			// Format the path to the asset so it starts at the root Resources directory and drops the extension.
			string assetPath = AssetDatabase.GetAssetPath (asset).Replace ("Assets/Resources/", "");
			string extension = Path.GetExtension (assetPath);
			if (!string.IsNullOrEmpty (extension)) {
				assetPath = assetPath.Replace (extension, "");
			}

			// Default to collection index -1.
			int collectionIndex = -1;

			// If the asset is a Sprite, check if it is a part of a collection and determine its index.
			Sprite sprite = asset as Sprite;
			if (sprite != null) {
				Object[] sprites = Resources.LoadAll (assetPath);
				for (int i = 0; i < sprites.Length; i++) {
					if (sprites [i].GetInstanceID () == sprite.GetInstanceID ()) {
						collectionIndex = i;
					}
				}
			}

			// Serialize the resource path.
			resource.SetResourcePath (assetPath, collectionIndex, id);
		}
	}
}