# UnityEssentials

Collection of essential scripts and tools suitable for any type of Unity Project.

## Features

### Editor

- **In-editor texture creation** - Create blank PNG/PSD files, gradients and checkerboard textures directly in the editor
- **Extended transform inspector** - Shows additional information about transform objects in the inspector
- **New menu items** - Toggle asset refresh, apply transforms to children, run builds, and more
- **Script templates** - New script templates with automatic (optional) namespace generation
- **(Optional) Reorganized asset menu** - Gone are the days of having a cluttered Create Asset menu
- **New scene view tools**
	- Prefab placement tool
	- Distance measurement tool
	- Randomize transform tool
	- Convex mesh builder
	- Reflection probe bounds tool
- **Extension methods** - Extensions for many common types, including editor GUI related ones
- **Property drawer utility** - Helper class for working with SerializedProperties

### Runtime

- **Property Attributes** - A plethora of new attributes for fields in the inspector, including, but not limited to:
	- Show if / Enabled if (conditionally show / enable fields)
	- Read only
	- Progress bar
	- Buttons to call a specific method
	- Popups for Layers, Tags or even integers
	- AnimationCurve parameters (set limits and color)
	- Monospace text fields
- **Collections** - Includes lists that support polymorphism and serializable dictionaries
- **New Data Types** - Various new data types that are commonly used:
	- Float range (min / max value)
	- Material property name
	- Nullables for many common value types
- **Mesh Generators** - Scripts to simplify procedural mesh generation
	- Convex mesh generator (using MIConvexHull)
	- Mesh builder (for triangle, line and voxel meshes)
	- Topology converter (convert triangle meshes into lines and/or point clouds)
- **Extension Methods** - Tons of new extension methods, available for:
	- Basic math
	- Colors
	- Vectors (includes math and shader-like swizzling methods)
	- Matrix4x4
	- Transforms
	- Textures
	- Terrains
	- Exceptions (add a custom message to an already thrown exception)
- **Repeating Timer** - A simple object useful for timing events at a specific interval or frame rate
- **Update Loop** - New event injection points on top of unity's own update loop and ability to run static methods in any update loop using attributes or event subscriptions
	- PreUpdate
	- PreLateUpdate
	- PostLateUpdate
	- PostFixedUpdate
- **Coroutine Tools** - Helper class for managing coroutines and delayed invocation of functions, including support for static coroutines
- **Extra Gizmos** - Various new gizmo shapes (circles, cylinders, arrows, texts and more)
- **Pooling System** - Reuse the same game objects instead of spawning and destroying them
- **Random Utilities** - Miscellaneous new random functions, including weighted distribution functions
- **Debug Utilities** - Helper functions to aid in debugging, including temporary gizmos

## Installation

> [!WARNING]
> It is highly recommended to target a specific release when installing to avoid breaking changes when unity reinstalls the package. This can be done by appending '#(tag-name)' at the end of the repo URL, such as '#1.0.0'.
> A list of currently available tags can be found [here](https://github.com/D3TONAT0R/UnityPackage-Essentials/tags).

### Option 1: Unity Package Manager

Open the Package Manager window, click on "Add Package from Git URL ...", then enter the following:
```
https://github.com/d3tonat0r/unitypackage-essentials.git
```

### Option 2: Manually Editing packages.json

Add the following line to your project's `Packages/manifest.json`:

```json
"com.github.d3tonat0r.essentials": "https://github.com/d3tonat0r/unitypackage-essentials.git"
```
