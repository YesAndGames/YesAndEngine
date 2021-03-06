using System;
using System.Collections.Generic;
using UnityEngine;
using YesAndEngine.Utilities;

namespace YesAndEngine.GameStateManagement {

	// A game state managed by the game state management system.
	public class IGameState : MonoBehaviour {

		// The manager controlling this state.
		public GameStateManager Manager { get; private set; }

		// Dictionary of game state children under this game state.
		private Dictionary<string, IGameStateChild> stateChildren;

#if UNITY_EDITOR

		// Unit test this game state.
		public virtual void RunUnitTests () {
			Debug.LogWarning ("Running unit tests on " + name + "...", this);
		}

#endif

		// Preload assets for this state and fire the callback action when finished.
		public virtual void PreloadAssetsAsync (Action callback) {

			// By default, assume no assets need to be loaded and fire callback immediately.
			if (callback != null) {
				callback ();
			}
		}

		// Preinitialize this game state by attaching it to the game state manager.
		// This method shouldn't be overriden.
		public void Preinitialize (GameStateManager manager) {

			// Assign the game state manager.
			Manager = manager;
			transform.SetAndClampParent (manager.transform, false);
		}

		// Initialize this game state.
		public virtual void OnInitializeState () {

			// Find and initialize children game states.
			stateChildren = new Dictionary<string, IGameStateChild> ();
			Component[] children = GetComponentsInChildren<IGameStateChild> (true);
			for (int i = 0; i < children.Length; i++) {
				IGameStateChild child = children[i] as IGameStateChild;
				child.OnInitializeState (this);
				stateChildren.Add (child.name, child);
			}

			// Unload unused assets once we're finished loading.
			Resources.UnloadUnusedAssets ();
		}

		// Update this game state.
		public virtual void OnUpdateState () {

			// Update children.
			foreach (IGameStateChild child in stateChildren.Values) {
				child.OnUpdateState ();
			}
		}

		// Clean up this game state when it exits.
		public virtual void OnExitState () {

			// Exit children.
			foreach (IGameStateChild child in stateChildren.Values) {
				child.OnExitState ();
			}
		}

		// Return a child game state reference via its string name.
		protected IGameStateChild GetChild (string name) {
			return stateChildren[name];
		}
	}
}
