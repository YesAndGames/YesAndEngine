# YesAndEngine
Yes And Games engine extension for the Unity3D game engine and editor.

# Namespaces
The various features and engine components are broken up into namespaces, and grouped into the global namespaces YesAndEngine and YesAndEditor. When importing the .unitypackage, optional directories can be excluded and required directories must not, but many scripts will merge into the same namespace. In other words, namespaces do not indicate whether or not the engine depends on a script.

## YesAndEngine
The **YesAndEngine** namespace contains no classes directly in the root, but has several sub namespaces that contain scripts and components. YesAndEngine C# classes include Unity components and utility classes that will get built with your game.

### GameStateManagement
The **YesAndEngine.GameStateManagement** namespace is the highlight feature of the engine. This system circumvents Unity's scene system to provide clarity and control over load times and awkward coupling and decoupling of UI, state management, and scene level components. IGameState prefabs take the place of Unity's .scene assets, allowing you to couple or decouple your UI from your scene on your own terms, asynchronously load resources at various stages in the state management pipeline, and maintain a flexible stack of your game states.

### Components
The **YesAndEngine.Components** namespace contains a plethora of helpful MonoBehaviour components for use throughout your code and scene assembly.

### Utilities
The **YesAndEngine.Utilities** namespace contains a plethora of helpful C# classes and scripts for use throughout your codebase.

## YesAndEditor
The **YesAndEditor** namespace contains extensions of UnityEditor components and utility classes that should not be included in your build, and will likely throw errors at build time if classes that must get built depend on them.
