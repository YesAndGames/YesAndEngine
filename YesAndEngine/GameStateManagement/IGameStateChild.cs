using UnityEngine;

namespace YesAndEngine.GameStateManagement {

	// A child component under an IGameState component that subscribes
	// to its events, but does not need all of its functionality.
	public abstract class IGameStateChild : MonoBehaviour {

		// Parent game state property.
		protected IGameState Master { get; private set; }

		// Initialize this child component.
		public virtual void OnInitializeState (IGameState master) {
			this.Master = master;
		}

		// Update this child component.
		public virtual void OnUpdateState () {
		}

		// Clean up this child component on exit.
		public virtual void OnExitState () {
		}
	}
}