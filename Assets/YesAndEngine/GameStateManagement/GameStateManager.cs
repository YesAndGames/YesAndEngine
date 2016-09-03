using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YesAndEngine.Components;
using YesAndEngine.ResourceManagement;

namespace YesAndEngine.GameStateManagement {

	// Root management component for the game state system.
	public class GameStateManager : SingletonMonobehavior<GameStateManager> {

		// Game states directory. Must be a subdirectory of /Resources/.
		// Defaults to /GameStates/, no beginning or end slash.
		[SerializeField]
		private string gameStatesDirectory = "GameStates";

		// Preloaded initial game state.
		[SerializeField]
		private IGameState initialGameState;

		// Stack of active game states stored by their string name.
		private Stack<string> gameStateStack = new Stack<string> ();

		// Reference to the current game state.
		private IGameState currentScreen;

		// Initialize the game state manager.
		protected override bool Awake () {

			// Only initialize if the singleton was properly initialized.
			if (base.Awake ()) {

				// Load the initial game state.
				if (!string.IsNullOrEmpty (initialGameState.name)) {
					SwitchState (initialGameState.name);
				}
				else {
					Debug.LogWarning ("No initial game state to load.", this);
				}

				// Successful initialization.
				return true;
			}

			// Failure to initialize.
			return false;
		}

		// Subscribe to monobehavior Update and pass to active state.
		void Update () {

			// Tick the current game screen if it exists.
			if (currentScreen != null) {
				currentScreen.OnUpdateState ();
			}

			// Update the resource loaded.
			AsyncResourceLoader.Update ();
		}

		// Completely switches the game state, clearing the stack and displaying the new screen.
		public void SwitchState (string id) {

			// Reconfigure stack.
			gameStateStack.Clear ();
			gameStateStack.Push (id);

			// Display new screen.
			DisplayNewGameScreen (id);
		}

		// Pushes a new screen onto the stack and displays it.
		public IGameState PushState (string id) {

			// Push to stack.
			gameStateStack.Push (id);

			// Display new screen.
			return DisplayNewGameScreen (id);
		}

		// Asyncronously pushes a new state to the top of the screen stack.
		public void PushStateAsync (string id, Action<IGameState> callback) {

			// Push to stack.
			gameStateStack.Push (id);

			// Display once loaded.
			DisplayNewGameScreenAsync (id, callback);
		}

		// Pops the top screen off of the stack, loading the state underneath.
		public IGameState PopState () {

			// An error is thrown if there is only one state on the stack, do nothing.
			if (gameStateStack.Count == 1) {
				Debug.LogError ("No screen behind the current one.");
				return null;
			}

			// Pop off stack.
			gameStateStack.Pop ();

			// Display new screen.
			return DisplayNewGameScreen (gameStateStack.Peek ());
		}

		// Asyncronously pops the current state.
		public void PopStateAsync (Action<IGameState> callback = null) {

			// An error is thrown if there is only one state on the stack, do nothing.
			if (gameStateStack.Count == 1) {
				Debug.LogError ("No screen behind the current one.");
				return;
			}

			// Pop off stack.
			gameStateStack.Pop ();

			// Display new screen.
			DisplayNewGameScreenAsync (gameStateStack.Peek (), callback);
		}

		// Pops the stack down to the specified state.
		// If the state is not on the stack, does nothing.
		public IGameState PopToState (string name) {

			// Base case: we are already on this screen.
			if (gameStateStack.Peek () == name) {
				return currentScreen;
			}

			// Keep track of what was popped in the emergency case that this fails.
			Stack<string> popped = new Stack<string> ();

			// Pop states until we find the proper one.
			while (true) {

				// There are no states left to pop.
				if (gameStateStack.Count == 0) {

					// Push all the states back on and exit out.
					while (popped.Count > 0) {
						gameStateStack.Push (popped.Pop ());
					}

					// Mark finished.
					Debug.LogError (string.Format ("Could not pop to state {0}: state not found on stack.", name));
					return null;
				}

				// If we found the screen, display it.
				if (gameStateStack.Peek () == name) {

					// Display the new screen.
					return DisplayNewGameScreen (gameStateStack.Peek ());
				}

				// Pop a state.
				popped.Push (gameStateStack.Pop ());
			}
		}

		// Get the current game state.
		public IGameState GetCurrentState () {
			return currentScreen;
		}

		// Prints the state stack to debug output.
		private void DebugPrintStateStack () {
			string final = "";
			string[] states = gameStateStack.ToArray ();
			for (int i = 0; i < states.Length; i++) {
				final += states[i];
				if (i != states.Length - 1) {
					final += " / ";
				}
			}

			Debug.LogWarning (final, this);
		}

		// Load and display new game screen asyncronously.
		private void DisplayNewGameScreenAsync (string id, Action<IGameState> callback) {
			StartCoroutine (AsyncLoadGameState (id, callback));
		}

		// Asyncronously load a game state using an Enumerator.
		private IEnumerator AsyncLoadGameState (string id, Action<IGameState> callback) {
			ResourceRequest request = Resources.LoadAsync (gameStatesDirectory + "/" + id, typeof (IGameState));
			yield return request;

			// Async preload state screen.
			IGameState state = request.asset as IGameState;
			state.PreloadAssetsAsync (loaded => {

				// Exit old game state.
				if (currentScreen != null) {
					currentScreen.OnExitState ();
				}

				// Destroy previous screen.
				DestroyAllChildren ();

				// Initialize new screen.
				IGameState stateScreen = Instantiate (loaded, transform.position, transform.rotation) as IGameState;
				currentScreen = stateScreen;

				// Attach to manager.
				stateScreen.OnInitializeState (this);

				// Invoke callback.
				if (callback != null) {
					callback (stateScreen);
				}
			});
		}

		// Displays a new game screen in the scene hierarchy.
		private IGameState DisplayNewGameScreen (string id) {

			// Exit old game state.
			if (currentScreen != null) {
				currentScreen.OnExitState ();
			}

			// Destroy previous screen.
			DestroyAllChildren ();

			// Instantiate new screen.
			IGameState state = Resources.Load<IGameState> (gameStatesDirectory + "/" + id);
			GameObject go = (GameObject)Instantiate (state.gameObject, transform.position, transform.rotation);
			IGameState stateScreen = go.GetComponent<IGameState> ();
			currentScreen = stateScreen;

			// Attach to manager.
			stateScreen.OnInitializeState (this);

			return stateScreen;
		}

		// Destroy all child GameObjects attached to this one.
		private void DestroyAllChildren () {
			List<GameObject> children = new List<GameObject> ();
			foreach (Transform child in gameObject.transform) children.Add (child.gameObject);
			children.ForEach (child => Destroy (child));
		}
	}
}