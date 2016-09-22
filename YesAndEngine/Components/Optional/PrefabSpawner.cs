using UnityEngine;

namespace YesAndEngine.Components {

	/// <summary>
	/// Spawns a prefab to its position on load and then destroys itself.
	/// This is used to get around nested prefab bugs in Unity.
	/// </summary>
	public class PrefabSpawner : MonoBehaviour {

		// Prefab to spawn.
		public GameObject prefab;

		// Instantiate the prefab and destroy self.
		void Awake () {
			GameObject instantiated = Instantiate (prefab) as GameObject;
			instantiated.transform.SetParent (transform.parent);
			instantiated.transform.localPosition = transform.localPosition;
			instantiated.transform.localScale = transform.localScale;
			instantiated.transform.localRotation = transform.localRotation;
			Destroy (gameObject);
		}
	}
}
