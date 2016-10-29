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
		protected string gameStatesDirectory = "GameStates";

		// Preloaded initial game state.
		[SerializeField]
		protected IGameState initialGameState;

		// Stack of active game states stored by their string name.
		private Stack<string> gameStateStack = new Stack<string> ();

		// Fires when the current screen is ready to load the next one.
		private Action onReadyToLoadNextScreen;

#if UNITY_EDITOR

		// The game state run unit tests on when running from the editor.
		[SerializeField]
		protected IGameState unitTestGameState;

#endif

		// Reference to the current game state.
		public IGameState CurrentScreen {
			get {
				return currentScreen;
			}
		}
		private IGameState currentScreen;

		// Initialize the game state manager.
		protected override bool Awake () {

			// Only initialize if the singleton was properly initialized.
			if (base.Awake ()) {

#if UNITY_EDITOR
				// Run unit tests.
				if (unitTestGameState != null && !string.IsNullOrEmpty (unitTestGameState.name)) {
					SwitchState (unitTestGameState.name)
						.RunUnitTests ();
					return true;
				}
#endif

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
		protected virtual void Update () {

			// Tick the current game screen if it exists.
			if (currentScreen != null) {
				currentScreen.OnUpdateState ();
			}

			// Update the resource loaded.
			AsyncResourceLoader.Update ();
		}

		// Completely switches the game state, clearing the stack and displaying the new screen.
		public IGameState SwitchState (string id) {

			// Reconfigure stack.
			gameStateStack.Clear ();
			gameStateStack.Push (id);

			// Display new screen.
			return DisplayNewGameScreen (id);
		}

		// Pushes a new screen onto the stack and displays it.
		public IGameState PushState (string id) {

			// Push to stack.
			gameStateStack.Push (id);

			// Display new screen.
			return DisplayNewGameScreen (id);
		}

		// Asyncronously pushes a new state to the top of the screen stack.
		public void PushStateAsync (string id, Action<IGameState> callback = null) {

			// Push to stack.
			gameStateStack.Push (id);

			// Display once loaded.
			DisplayNewGameScreenAsync (id, callback);
		}

		// Pushes a state on the stack without loading it, modifying only the state history stack.
		public void PushStateHistory (string id) {
			gameStateStack.Push (id);
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

		// Pops a state off the stack without loading the one under it, modifying only the state history stack.
		public void PopStateHistory () {

			// An error is thrown if there is only one state on the stack, do nothing.
			if (gameStateStack.Count == 1) {
				Debug.LogError ("No screen behind the current one.");
				return;
			}

			// Pop off stack.
			gameStateStack.Pop ();
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

		// Internally unloads the state stack down to the specified state.
		// Does not load the specified state at the end of the pop, just prepares the stack history.
		public void PopHistoryToState (string name) {

			// Base case: we are already on this screen.
			if (gameStateStack.Peek () == name) {
				return;
			}

			// Keep track of what was popped in the emergency case that this fails.
			Stack<string> popped = new Stack<string> ();

			// Pop states until we find the proper one.
			while (gameStateStack.Count > 0) {

				// If we found the screen, exit out.
				if (gameStateStack.Peek () == name) {
					return;
				}

				// Pop a state.
				popped.Push (gameStateStack.Pop ());
			}

			// State not found, push all the history back on the stafck and error out.
			while (popped.Count > 0) {
				gameStateStack.Push (popped.Pop ());
			}
			Debug.LogError (string.Format ("Could not pop to state {0}: state not found on stack.", name));
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
			stateScreen.Preinitialize (this);
			stateScreen.OnInitializeState ();

			return stateScreen;
		}

		// Load and display new game screen asyncronously.
		private void DisplayNewGameScreenAsync (string id, Action<IGameState> callback) {

			// Listen for ready event to start load procedure.
			onReadyToLoadNextScreen += () => {
				StartCoroutine (AsyncLoadGameState (id, callback));
				onReadyToLoadNextScreen = null;
			};

			// Prepare to load the next screen
			PrepareToLoad (id);
		}

		// Asyncronously load a game state using an Enumerator.
		private IEnumerator AsyncLoadGameState (string id, Action<IGameState> callback) {
			ResourceRequest request = Resources.LoadAsync (gameStatesDirectory + "/" + id, typeof (IGameState));
			yield return request;

			// Async preload state screen.
			IGameState state = Instantiate (request.asset as IGameState, transform.position, transform.rotation) as IGameState;
			state.Preinitialize (this);
			state.PreloadAssetsAsync (() => {
				Debug.LogWarning ("Unloading " + currentScreen.name);

				// Exit old game state.
				if (currentScreen != null) {
					currentScreen.OnExitState ();
					Destroy (currentScreen.gameObject);
				}

				// Initialize new screen.
				currentScreen = state;

				Debug.LogWarning ("Initializing " + currentScreen.name);

				// Attach to manager.
				state.OnInitializeState ();

				// Invoke callback.
				if (callback != null) {
					callback (state);
				}

				// Finished loading.
				FinishLoading (state);
			});
		}

		// Prepare the screen to load the next screen with the specified ID. 
		protected virtual void PrepareToLoad (string id) {

			// By default, we are immediately ready to load the next screen.
			ReadyToLoad ();
		}

		// Call this when the current screen has been prepped to load the next one.
		protected void ReadyToLoad () {
			if (onReadyToLoadNextScreen != null) {
				onReadyToLoadNextScreen ();
			}
		}

		// Called when the screen is finished loading.
		protected virtual void FinishLoading (IGameState screen) { }

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

		// Destroy all child GameObjects attached to this one.
		private void DestroyAllChildren () {
			List<GameObject> children = new List<GameObject> ();
			foreach (Transform child in gameObject.transform) children.Add (child.gameObject);
			children.ForEach (child => Destroy (child));
		}
	}
}