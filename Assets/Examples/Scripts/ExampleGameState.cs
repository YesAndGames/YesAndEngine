using UnityEngine;
using System.Collections;
using YesAndEngine.GameStateManagement;

// An example game state.
public class ExampleGameState : IGameState {

	// Initialize this game state.
	public override void OnInitializeState (GameStateManager manager) {
		base.OnInitializeState (manager);

		Debug.Log ("Initializing preloaded example game state!");
	}
}
