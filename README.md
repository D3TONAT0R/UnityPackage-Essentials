# UnityEssentials

Collection of essential scripts and tools suitable for any type of Unity Project.

## Features

### Editor

- **In-editor texture creation** - Create blank textures, gradients and checkerboard textures directly in the editor
- **Mesh builder** - Generate meshes via script
- **Extended transform inspector** - Show additional information about transform objects
- **New menu items** - Toggle asset refresh, apply transforms to children, run builds, and more
- **Script templates** - New script templates with automatic namespace generation
- **(Optional) Reorganized asset menu** - Gone are the days of having a cluttered create asset menu
- **New scene view tools**
	- Prefab placement tool
	- Distance measurement tool
	- Randmize transform tool
	- Convex mesh builder
	- Reflection probe bounds tool
- **Extension methods** - Extensions for easier editor GUI creation
- **(Optional) Disabled Playmode shortcuts** - Disable common edit commands during playmode
- **Property drawer utility** - Helper class for working with SerializedProperties

### Runtime

- **Property Attributes** - A plethora of new attributes for fields in the inspector, including, but not limited to:
	- Show if / Enabled if (conditionally show / enable fields)
	- Read only
	- Progress bar
	- Buttons to call a specific method
	- Popups for Layers, Tags or even integers
- **Collections** - Includes lists that support polymorphism and serializable dictionaries
- **New Data Types** - Various new data types that are commonly used:
	- Float range (min / max values)
	- Material property name
	- Nullables for many common value types
- **Mesh Generators** - Scripts to simplify procedural mesh generation
	- Convex mesh generator (using MIConvexHull)
	- Mesh builder (for triangle, line and voxel meshes)
	- Topology converter (convert triangle meshes into lines and/or points)
- **Extension Methods** - Tons of new extension methods, available for:
	- Basic math
	- Vector swizzling
	- Vector math
	- Matrix4x4
	- Transforms
	- Textures
	- Terrain
	- Exceptions (add a custom message to an already thrown exception)
- **Repeating Timer** - A simple object useful for timing events at a specific interval or frame rate
- **Update Loop** - New event injection points on top of unity's own update loop
	- PreUpdate
	- PreLateUpdate
	- PostLateUpdate
	- PostFixedUpdate
- **Coroutine Tools** - Helper class for managing coroutines & delayed invocation of functions
- **Extra Gizmos** - Various new gizmo shapes (circles, cylinders and more)
- **Pooling System** - Reuse the same game objects instead of spawning and destroying them
- **Random Utilities** - Miscellaneous new random functions
- **Debug Utilities** - Helper functions to aid in debugging, including temporary gizmos

## Installation

### Option 1: Unity Package Manager

> Make sure to always target a specific tag of the package to avoid breaking changes.
>
> The latest tags can be found [here](https://github.com/D3TONAT0R/UnityPackage-Essentials/tags).

Open the Package Manager window, click on "Add Package from Git URL ...", then enter the following:
```
https://github.com/d3tonat0r/unitypackage-essentials.git#0.9.2
```

### Option 2: Manually Editing packages.json

Add the following line to your project's `Packages/manifest.json`:

```json
"com.example.package": "https://github.com/d3tonat0r/unitypackage-essentials.git#0.9.2"
```

### Option 3: Manual Installation (not recommended)

You can also download this repository and extract the ZIP file anywhere inside your project's Assets folder.
